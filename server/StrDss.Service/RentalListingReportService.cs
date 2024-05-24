using AutoMapper;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using StrDss.Model.RentalReportDtos;
using StrDss.Model.UserDtos;
using StrDss.Service.CsvHelpers;
using StrDss.Service.EmailTemplates;
using StrDss.Service.HttpClients;
using System.Text;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IRentalListingReportService
    {
        Task<Dictionary<string, List<string>>> ValidateAndParseUploadAsync(string reportPeriod, long orgId, string hashValue, TextReader textReader, List<DssUploadLine> lines);
        Task<Dictionary<string, List<string>>> UploadRentalReport(string reportPeriod, long orgId, Stream stream);
        Task ProcessRentalReportUploadsAsync();
        Task<PagedDto<RentalUploadHistoryViewDto>> GetRentalListingUploadHistory(long? platformId, int pageSize, int pageNumber, string orderBy, string direction);
        Task<byte[]?> GetRentalListingErrorFile(long uploadId);
    }
    public class RentalListingReportService : ServiceBase, IRentalListingReportService
    {
        private IOrganizationRepository _orgRepo;
        private IUploadDeliveryRepository _uploadRepo;
        private IRentalListingReportRepository _reportRepo;
        private IPhysicalAddressRepository _addressRepo;
        private IGeocoderApi _geocoder;
        private IUserRepository _userRepo;
        private IEmailMessageService _emailService;
        private IEmailMessageRepository _emailRepo;
        private IConfiguration _config;

        public RentalListingReportService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IOrganizationRepository orgRepo, IUploadDeliveryRepository uploadRepo, IRentalListingReportRepository reportRepo, IPhysicalAddressRepository addressRepo,
            IGeocoderApi geocoder, IUserRepository userRepo, IEmailMessageService emailService, IEmailMessageRepository emailRepo,
            IConfiguration config, ILogger<StrDssLogger> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _orgRepo = orgRepo;
            _uploadRepo = uploadRepo;
            _reportRepo = reportRepo;
            _addressRepo = addressRepo;
            _geocoder = geocoder;
            _userRepo = userRepo;
            _emailService = emailService;
            _emailRepo = emailRepo;
            _config = config;
        }

        public async Task<Dictionary<string, List<string>>> UploadRentalReport(string reportPeriod, long orgId, Stream stream)
        {
            byte[] sourceBin;
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                sourceBin = memoryStream.ToArray();
            }

            var hashValue = sourceBin.GetSha256Hash();

            stream.Position = 0;
            using TextReader textReader = new StreamReader(stream, Encoding.UTF8);

            var uploadLines = new List<DssUploadLine>();

            var errors = await ValidateAndParseUploadAsync(reportPeriod, orgId, hashValue, textReader, uploadLines);

            if (errors.Count > 0) return errors;

            var entity = new DssUploadDelivery
            {
                UploadDeliveryType = UploadDeliveryTypes.RentalReport,
                ReportPeriodYm = new DateOnly(Convert.ToInt32(reportPeriod.Substring(0, 4)), Convert.ToInt32(reportPeriod.Substring(5, 2)), 1),
                SourceHashDsc = hashValue,
                SourceBin = sourceBin,
                ProvidingOrganizationId = orgId,
                UpdUserGuid = _currentUser.UserGuid
            };

            foreach (var line in uploadLines)
            {
                entity.DssUploadLines.Add(line);
            }

            await _uploadRepo.AddUploadDeliveryAsync(entity);

            _unitOfWork.Commit();

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> ValidateAndParseUploadAsync(string reportPeriod, long orgId, string hashValue, TextReader textReader, List<DssUploadLine> uploadLines)
        {
            var errors = new Dictionary<string, List<string>>();

            var regex = RegexDefs.GetRegexInfo(RegexDefs.YearMonth);
            if (!Regex.IsMatch(reportPeriod, regex.Regex))
            {
                errors.AddItem("ReportPeriod", regex.ErrorMessage);
                return errors;
            }

            var firstDayOfReportMonth = new DateOnly(Convert.ToInt32(reportPeriod.Substring(0, 4)), Convert.ToInt32(reportPeriod.Substring(5, 2)), 1);
            var firstDayOfCurrentMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            if (firstDayOfReportMonth >= firstDayOfCurrentMonth)
            {
                errors.AddItem("ReportPeriod", "Report period cannot be current or future month.");
            }

            var isDuplicate = await _uploadRepo.IsDuplicateRentalReportUploadAsnyc(firstDayOfReportMonth, orgId, hashValue);
            if (isDuplicate)
            {
                errors.AddItem("File", "The file has already been uploaded");
                return errors;
            }

            var platform = await _orgRepo.GetOrganizationByIdAsync(orgId);
            if (platform == null)
            {
                errors.AddItem("OrganizationId", $"Organization ID [{orgId}] doesn't exist.");
            }
            else if (platform.OrganizationType != OrganizationTypes.Platform)
            {
                errors.AddItem("OrganizationId", $"Organization type of the organization [{orgId}] is not {OrganizationTypes.Platform}.");
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            var csvConfig = CsvHelperUtils.GetConfig(errors, false);

            using var csv = new CsvReader(textReader, csvConfig);

            var mandatoryFields = new string[] { "rpt_period", "org_cd", "listing_id" };

            csv.Read();
            var headerExists = csv.ReadHeader();

            if (!headerExists)
            {
                errors.AddItem("File", "Header deosn't exist.");
                return errors;
            }

            if (!CheckCommonMandatoryFields(csv.HeaderRecord, mandatoryFields, errors))
            {
                return errors;
            }

            var reportPeriodMismatch = 0;
            var reportPeriodMissing = 0;
            var orgCdMissing = 0;
            var invalidOrgCds = new List<string>();
            var listingIdMissing = 0;
            var listingIds = new List<string>();
            var duplicateListingIds = new List<string>();

            var orgCds = new List<string>();

            while (csv.Read())
            {
                RentalListingRowUntyped row = null!;

                try
                {
                    row = csv.GetRecord<RentalListingRowUntyped>();

                    if (row.RptPeriod != reportPeriod) reportPeriodMismatch++;
                    if (row.RptPeriod.IsEmpty()) reportPeriodMissing++;
                    if (row.OrgCd.IsEmpty()) orgCdMissing++;
                    if (row.ListingId.IsEmpty()) listingIdMissing++;

                    if (row.OrgCd.IsNotEmpty() && !orgCds.Contains(row.OrgCd.ToUpper())) orgCds.Add(row.OrgCd.ToUpper());
                    if (!listingIds.Contains($"{row.OrgCd}-{row.ListingId.ToLower()}"))
                    {
                        listingIds.Add($"{row.OrgCd}-{row.ListingId.ToLower()}");
                        uploadLines.Add(new DssUploadLine
                        {
                            IsValidationFailure = false,
                            IsSystemFailure = false,
                            IsProcessed = false,
                            SourceOrganizationCd = row.OrgCd,
                            SourceRecordNo = row.ListingId,
                            SourceLineTxt = csv.Parser.RawRecord
                        });
                    }
                    else
                    {
                        duplicateListingIds.Add($"{row.OrgCd}-{row.ListingId.ToLower()}");
                    }
                }
                catch (TypeConverterException ex)
                {
                    errors.AddItem(ex.MemberMapData.Member.Name, ex.Message);
                    break;
                }
                catch (CsvHelper.MissingFieldException)
                {
                    break;
                }
                catch (CsvHelper.ReaderException ex)
                {
                    _logger.LogWarning(ex.Message);
                    throw;
                }
                catch (CsvHelperException ex)
                {
                    _logger.LogInformation(ex.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }
            }

            var validOrgCds = (await _orgRepo.GetManagingOrgCdsAsync(orgId)).Select(x => x.ToUpper());

            foreach (var org in orgCds)
            {
                if (!validOrgCds.Contains(org))
                {
                    invalidOrgCds.Add(org);
                }
            }

            if (reportPeriodMismatch > 0)
            {
                errors.AddItem("rpt_period", $"Report period mismatch found in {reportPeriodMismatch} record(s). The file contains report period(s) other than the selected period of {reportPeriod}");
            }

            if (reportPeriodMissing > 0)
            {
                errors.AddItem("rpt_period", $"Report period missing in {reportPeriodMissing} record(s). Please provide a report period.");
            }

            if (orgCdMissing > 0)
            {
                errors.AddItem("org_cd", $"Organization code missing in {orgCdMissing} record(s). Please provide an organization code.");
            }

            if (invalidOrgCds.Count > 0)
            {
                errors.AddItem("org_cd", $"Invalid organization code(s) found: {string.Join(", ", invalidOrgCds.ToArray())}. Please use one of the following valid organization code(s): {string.Join(", ", validOrgCds)}");
            }

            if (listingIdMissing > 0)
            {
                errors.AddItem("listing_id", $"Listing ID missing in {listingIdMissing} record(s). Please provide a listing ID.");
            }

            if (duplicateListingIds.Count > 0)
            {
                errors.AddItem("listing_id", $"Duplicate listing ID(s) found: {string.Join(", ", duplicateListingIds.ToArray())}. Each listing ID must be unique within an organization code.");
            }

            return errors;
        }

        private bool CheckCommonMandatoryFields(string[] headers, string[] mandatoryFields, Dictionary<string, List<string>> errors)
        {
            headers = CsvHelperUtils.GetLowercaseFieldsFromCsvHeaders(headers);

            foreach (var field in mandatoryFields)
            {
                if (!headers.Any(x => x.ToLower() == field))
                    errors.AddItem("File", $"Header [{field}] is missing");
            }

            if (errors.Count > 0)
                errors.AddItem("File", "Please ensure the file headers are correct");

            return errors.Count == 0;
        }

        public async Task ProcessRentalReportUploadsAsync()
        {
            var uploads = await _uploadRepo.GetRentalReportUploadsToProcessAsync();

            foreach (var upload in uploads)
            {
                await ProcessRentalReportUploadAsync(upload);
            }
        }

        private async Task ProcessRentalReportUploadAsync(DssUploadDelivery upload)
        {
            _logger.LogInformation($"Processing Rental Listing Report {upload.ProvidingOrganizationId} - {upload.ReportPeriodYm} - {upload.ProvidingOrganization.OrganizationNm}");

            var reportPeriodYm = (DateOnly)upload.ReportPeriodYm!;

            var report = await _reportRepo.GetRentalListingReportAsync(upload.ProvidingOrganizationId, reportPeriodYm);

            if (report == null)
            {
                report = new DssRentalListingReport
                {
                    ReportPeriodYm = reportPeriodYm,
                    IsCurrent = false,
                    ProvidingOrganization = upload.ProvidingOrganization,
                };

                await _reportRepo.AddRentalListingReportAsync(report);

                _unitOfWork.Commit();
            }

            var errors = new Dictionary<string, List<string>>();
            var csvConfig = CsvHelperUtils.GetConfig(errors, false);

            var memoryStream = new MemoryStream(upload.SourceBin!);
            using TextReader textReader = new StreamReader(memoryStream, Encoding.UTF8);

            using var csv = new CsvReader(textReader, csvConfig);

            csv.Read();
            var headerExists = csv.ReadHeader();
            var hasError = false;

            while (csv.Read())
            {
                var row = csv.GetRecord<RentalListingRowUntyped>(); //it has been parsed once, so no exception expected.
                var uploadLine = upload.DssUploadLines.FirstOrDefault(x => x.SourceOrganizationCd == row.OrgCd && x.SourceRecordNo == row.ListingId);

                if (uploadLine == null)
                {
                    continue; //already processed
                }

                if (await ProcessUploadLine(report, upload, uploadLine, row, csv.Parser.RawRecord))
                    hasError = true;
            }

            if (hasError)
            {
                if (upload.UpdUserGuid == null) return;

                var user = await _userRepo.GetUserByGuid((Guid)upload.UpdUserGuid!);

                if (user == null) return;

                var history = await _reportRepo.GetRentalListingUpload(upload.UploadDeliveryId);

                if (history == null) return;

                var adminEmail = _config.GetValue<string>("ADMIN_EMAIL");
                if (adminEmail == null) return;

                using var transaction = _unitOfWork.BeginTransaction();

                //upload.UploadDeliveryId
                var template = new ListingUploadError(_emailService)
                {
                    UserName = $"{user!.GivenNm}",
                    NumErrors = (long)history.Errors!,
                    Link = GetHostUrl() + "/upload-listing-history",
                    To = new string[] { user!.EmailAddressDsc! },
                    Info = $"{EmailMessageTypes.ListingUploadError} for {user.FamilyNm}, {user.GivenNm}",
                    From = adminEmail
                };

                var emailEntity = new DssEmailMessage
                {
                    EmailMessageType = template.EmailMessageType,
                    MessageDeliveryDtm = DateTime.UtcNow,
                    MessageTemplateDsc = template.GetContent(),
                    IsHostContactedExternally = false,
                    IsSubmitterCcRequired = false,
                    LgPhoneNo = null,
                    UnreportedListingNo = null,
                    HostEmailAddressDsc = null,
                    LgEmailAddressDsc = null,
                    CcEmailAddressDsc = null,
                    UnreportedListingUrl = null,
                    LgStrBylawUrl = null,
                    InitiatingUserIdentityId = user.UserIdentityId,
                    AffectedByUserIdentityId = user.UserIdentityId,
                    InvolvedInOrganizationId = upload.ProvidingOrganizationId
                };

                await _emailRepo.AddEmailMessage(emailEntity);

                emailEntity.ExternalMessageNo = await template.SendEmail(adminEmail);

                _unitOfWork.Commit();

                _unitOfWork.CommitTransaction(transaction);
            }

            await _reportRepo.UpdateListingStatus(report.ProvidingOrganizationId);

            _unitOfWork.Commit();
        }

        private async Task<bool> ProcessUploadLine(DssRentalListingReport report, DssUploadDelivery upload, DssUploadLine uploadLine, RentalListingRowUntyped row, string rawRecord)
        {
            var errors = new Dictionary<string, List<string>>();
            _validator.Validate(Entities.RentalListingRowUntyped, row, errors);

            if (errors.Count > 0)
            {
                SaveUploadLine(uploadLine, errors, true, "");
                _unitOfWork.Commit();
                return true;
            }

            var offeringOrg = await _orgRepo.GetOrganizationByOrgCdAsync(row.OrgCd); //already validated in the file upload

            using var tran = _unitOfWork.BeginTransaction();

            var listing = await CreateOrUpdateRentalListing(report, offeringOrg, row);

            AddContacts(listing, row);

            var (physicalAddress, systemError) = await CreateOrGetPhysicalAddress(listing, row);

            listing.LocatingPhysicalAddress = physicalAddress;

            SaveUploadLine(uploadLine, errors, false, systemError);

            _unitOfWork.Commit();

            var (needUpdate, masterListing) = await CreateOrUpdateMasterListing(report.ReportPeriodYm, listing, offeringOrg, row, physicalAddress);

            if (needUpdate)
            {
                AddContacts(masterListing, row);
            }

            _unitOfWork.Commit();

            tran.Commit();

            return systemError.IsEmpty();
        }

        private void SaveUploadLine(DssUploadLine uploadLine, Dictionary<string, List<string>> errors, bool isValid, string systemError)
        {
            uploadLine.IsValidationFailure = isValid;
            uploadLine.ErrorTxt = errors.ParseErrorWithUnderScoredKeyName();

            uploadLine.IsSystemFailure = systemError.IsNotEmpty();
            if (uploadLine.IsSystemFailure)
            {
                uploadLine.ErrorTxt = systemError;
            }

            uploadLine.IsProcessed = true;
        }

        private async Task<DssRentalListing> CreateOrUpdateRentalListing(DssRentalListingReport report, OrganizationDto offeringOrg, RentalListingRowUntyped row)
        {
            var listing = await _reportRepo.GetRentalListingAsync(report.RentalListingReportId, offeringOrg.OrganizationId, row.ListingId);

            if (listing == null)
            {
                listing = _mapper.Map<DssRentalListing>(row);
                await _reportRepo.AddRentalListingAsync(listing);
            }
            else
            {
                _mapper.Map(row, listing);
                _reportRepo.DeleteListingContacts(listing.RentalListingId);
            }

            listing.IncludingRentalListingReportId = report.RentalListingReportId;
            listing.OfferingOrganizationId = offeringOrg.OrganizationId;

            return listing;
        }

        private async Task<(DssPhysicalAddress, string)> CreateOrGetPhysicalAddress(DssRentalListing listing, RentalListingRowUntyped row)
        {
            var address = row.RentalAddress;

            var physicalAddress = await _addressRepo.GetPhysicalAdderssFromMasterListingAsync(listing.OfferingOrganizationId, listing.PlatformListingNo, address);

            var error = "";
            if (physicalAddress == null)
            {
                physicalAddress = new DssPhysicalAddress
                {
                    OriginalAddressTxt = row.RentalAddress,
                };

                error = await _geocoder.GetAddressAsync(physicalAddress);

                if (error.IsEmpty() && physicalAddress.LocationGeometry is not null && physicalAddress.LocationGeometry is Point point)
                {
                    physicalAddress.ContainingOrganizationId = await _orgRepo.GetContainingOrganizationId(point);
                }

                await _addressRepo.AddPhysicalAddressAsync(physicalAddress);
            }

            return (physicalAddress, error);
        }

        private async Task<(bool needUpdate, DssRentalListing masterListing)> CreateOrUpdateMasterListing(DateOnly reportPeriodYm, DssRentalListing listing, OrganizationDto offeringOrg, RentalListingRowUntyped row, DssPhysicalAddress physicalAddress)
        {
            var masterListing = await _reportRepo.GetMasterListingAsync(offeringOrg.OrganizationId, listing.PlatformListingNo);            

            if (masterListing == null)
            {
                masterListing = _mapper.Map<DssRentalListing>(row);
                await _reportRepo.AddRentalListingAsync(masterListing);
            }
            else
            {
                if (reportPeriodYm < masterListing!.DerivedFromRentalListing!.IncludingRentalListingReport!.ReportPeriodYm)
                    return (false, masterListing);

                _mapper.Map(row, masterListing);
                _reportRepo.DeleteListingContacts(masterListing.RentalListingId);
            }

            masterListing.IncludingRentalListingReportId = null;
            masterListing.OfferingOrganizationId = offeringOrg.OrganizationId;
            masterListing.DerivedFromRentalListingId = listing.RentalListingId;
            masterListing.LocatingPhysicalAddressId = physicalAddress.PhysicalAddressId;

            return (true, masterListing);
        }

        private void AddContacts(DssRentalListing listing, RentalListingRowUntyped row)
        {
            AddContact(listing, "", row.PropertyHostNm, row.PropertyHostEmail, row.PropertyHostPhone, row.PropertyHostFax, row.PropertyHostAddress, 1, true);
            AddContact(listing, row.SupplierHost1Id, row.SupplierHost1Nm, row.SupplierHost1Email, row.SupplierHost1Phone, row.SupplierHost1Fax, row.SupplierHost1Address, 1, false);
            AddContact(listing, row.SupplierHost2Id, row.SupplierHost2Nm, row.SupplierHost2Email, row.SupplierHost2Phone, row.SupplierHost2Fax, row.SupplierHost2Address, 2, false);
            AddContact(listing, row.SupplierHost3Id, row.SupplierHost3Nm, row.SupplierHost3Email, row.SupplierHost3Phone, row.SupplierHost3Fax, row.SupplierHost3Address, 3, false);
            AddContact(listing, row.SupplierHost4Id, row.SupplierHost4Nm, row.SupplierHost4Email, row.SupplierHost4Phone, row.SupplierHost4Fax, row.SupplierHost4Address, 4, false);
            AddContact(listing, row.SupplierHost5Id, row.SupplierHost5Nm, row.SupplierHost5Email, row.SupplierHost5Phone, row.SupplierHost5Fax, row.SupplierHost5Address, 5, false);
        }

        private void AddContact(DssRentalListing listing, string hostNo, string name, string email, string phone, string fax, string address, short conatctNo, bool isOwner)
        {
            if (name.IsNotEmpty() || hostNo.IsNotEmpty() || email.IsNotEmpty() || phone.IsNotEmpty() || fax.IsNotEmpty() || address.IsNotEmpty())
            {
                listing.DssRentalListingContacts.Add(new DssRentalListingContact
                {
                    IsPropertyOwner = isOwner,
                    ListingContactNbr = conatctNo,
                    SupplierHostNo = hostNo,
                    FullNm = name,
                    PhoneNo = phone,
                    FaxNo = fax,
                    FullAddressTxt = address,
                    EmailAddressDsc = email,
                });
            }
        }

        public async Task<PagedDto<RentalUploadHistoryViewDto>> GetRentalListingUploadHistory(long? platformId, int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _reportRepo.GetRentalListingUploadHistory(platformId, pageSize, pageNumber, orderBy, direction);
        }

        public async Task<byte[]?> GetRentalListingErrorFile(long uploadId)
        {
            var upload = await _uploadRepo.GetRentalListingErrorLines(uploadId);

            if (upload == null) return null;

            if (_currentUser.OrganizationType == OrganizationTypes.Platform)
            {
                if (upload.ProvidingOrganizationId != _currentUser.OrganizationId)
                {
                    return null;
                }
            }                

            var memoryStream = new MemoryStream(upload.SourceBin!);
            using TextReader textReader = new StreamReader(memoryStream, Encoding.UTF8);

            var errors = new Dictionary<string, List<string>>();
            var csvConfig = CsvHelperUtils.GetConfig(errors, false);

            using var csv = new CsvReader(textReader, csvConfig);

            var contents = new StringBuilder();

            csv.Read();
            var header = csv.Parser.RawRecord.TrimEndNewLine() + ",errors";

            contents.AppendLine(header);

            foreach(var line in upload.DssUploadLines)
            {
                if (!line.IsValidationFailure && !line.IsSystemFailure) continue;

                contents.AppendLine(line.SourceLineTxt.TrimEndNewLine() + $",\"{line.ErrorTxt}\"");
            }

            return Encoding.UTF8.GetBytes(contents.ToString());
        }
    }
}
