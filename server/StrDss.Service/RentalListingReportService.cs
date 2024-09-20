﻿using AutoMapper;
using CsvHelper;
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
using StrDss.Service.CsvHelpers;
using StrDss.Service.EmailTemplates;
using StrDss.Service.HttpClients;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace StrDss.Service
{
    public interface IRentalListingReportService
    {
        Task ProcessRentalReportUploadAsync();
        Task CleaupAddressAsync();
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
        private IBizLicenceRepository _bizLicRepo;
        private IConfiguration _config;

        public RentalListingReportService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IOrganizationRepository orgRepo, IUploadDeliveryRepository uploadRepo, IRentalListingReportRepository reportRepo, IPhysicalAddressRepository addressRepo,
            IGeocoderApi geocoder, IUserRepository userRepo, IEmailMessageService emailService, IEmailMessageRepository emailRepo, IBizLicenceRepository bizLicRepo,
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
            _bizLicRepo = bizLicRepo;
            _config = config;
        }
        public async Task ProcessRentalReportUploadAsync()
        {
            var upload = await _uploadRepo.GetUploadToProcessAsync(UploadDeliveryTypes.ListingData);

            if (upload != null)
            {
                await ProcessRentalReportUploadAsync(upload);
            }
        }

        private async Task ProcessRentalReportUploadAsync(DssUploadDelivery upload)
        {
            var processStopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Processing Rental Listing Report {upload.ProvidingOrganizationId} - {upload.ReportPeriodYm} - {upload.ProvidingOrganization.OrganizationNm}");

            var reportPeriodYm = (DateOnly)upload.ReportPeriodYm!;
            var lineCount = await _uploadRepo.GetTotalNumberOfUploadLines(upload.UploadDeliveryId);
            var count = 0;

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

            var linesToProcess = await _uploadRepo.GetUploadLinesToProcessAsync(upload.UploadDeliveryId);

            var errors = new Dictionary<string, List<string>>();
            var csvConfig = CsvHelperUtils.GetConfig(errors, false);

            var memoryStream = new MemoryStream(upload.SourceBin!);
            using TextReader textReader = new StreamReader(memoryStream, Encoding.UTF8);

            using var csv = new CsvReader(textReader, csvConfig);

            csv.Read();
            var headerExists = csv.ReadHeader();
            var hasError = false;
            var isLastLine = false;
            var processedCount = 0;

            while (csv.Read())
            {
                if (processedCount >= 100) break; //To process 100 lines per job

                count++;
                isLastLine = count == lineCount;

                var row = csv.GetRecord<RentalListingRowUntyped>(); //it has been parsed once, so no exception expected.

                var exists = linesToProcess.Any(x => x.OrgCd == row.OrgCd && x.SourceRecordNo == row.ListingId);

                if (!exists) continue;

                var uploadLine = await _uploadRepo.GetUploadLineAsync(upload.UploadDeliveryId, row.OrgCd, row.ListingId);

                if (uploadLine == null || uploadLine.IsProcessed)
                {
                    _logger.LogInformation($"Skipping listing - ({row.OrgCd} - {row.ListingId})");
                    continue;
                }

                _logger.LogInformation($"Processing listing ({row.OrgCd} - {row.ListingId})");

                var stopwatch = Stopwatch.StartNew();
                hasError = !await ProcessUploadLine(report, upload, uploadLine, row, isLastLine);
                stopwatch.Stop();

                processedCount++;

                _logger.LogInformation($"Finishing listing ({row.OrgCd} - {row.ListingId}): {stopwatch.Elapsed.TotalMilliseconds} milliseconds - {processedCount}/100");
            }

            if (!isLastLine)
            {
                processStopwatch.Stop();
                _logger.LogInformation($"Processed {processedCount} lines: {report.ReportPeriodYm.ToString("yyyy-MM")}, {report.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
                return;
            }

            if (hasError || (await _uploadRepo.UploadHasErrors(upload.UploadDeliveryId)))
            {
                if (upload.UpdUserGuid == null) return;

                var user = await _userRepo.GetUserByGuid((Guid)upload.UpdUserGuid!);

                if (user == null) return;

                var history = await _uploadRepo.GetRentalListingUpload(upload.UploadDeliveryId);

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

                emailEntity.ExternalMessageNo = await template.SendEmail();

                _unitOfWork.Commit();

                _unitOfWork.CommitTransaction(transaction);
            }

            processStopwatch.Stop();
            _logger.LogInformation($"Finished: {report.ReportPeriodYm.ToString("yyyy-MM")}, {report.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
        }

        private async Task<bool> ProcessUploadLine(DssRentalListingReport report, DssUploadDelivery upload, DssUploadLine uploadLine, RentalListingRowUntyped row, bool isLastLine)
        {
            var errors = new Dictionary<string, List<string>>();
            
            _validator.Validate(Entities.RentalListingRowUntyped, row, errors);

            if (errors.Count > 0)
            {
                SaveUploadLine(uploadLine, errors, true, "");

                if (isLastLine)
                {
                    await _reportRepo.UpdateInactiveListings(upload.ProvidingOrganizationId);
                }

                _unitOfWork.Commit();
                return false;
            }

            var offeringOrg = await _orgRepo.GetOrganizationByOrgCdAsync(row.OrgCd); //already validated in the file upload

            using var tran = _unitOfWork.BeginTransaction();

            var listing = await CreateOrUpdateRentalListing(report, offeringOrg, row);

            AddContacts(listing, row);

            var (physicalAddress, systemError) = await CreateOrGetPhysicalAddress(listing, row);

            listing.LocatingPhysicalAddress = physicalAddress;

            SaveUploadLine(uploadLine, errors, false, systemError);

            _unitOfWork.Commit();

            if (systemError.IsNotEmpty())
            {
                if (isLastLine)
                {
                    await _reportRepo.UpdateInactiveListings(upload.ProvidingOrganizationId);
                }

                tran.Commit();
                return false;
            }

            var (needUpdate, masterListing) = await CreateOrUpdateMasterListing(report.ReportPeriodYm, listing, offeringOrg, row, physicalAddress);

            if (needUpdate)
            {
                AddContacts(masterListing, row);
            }

            _unitOfWork.Commit();

            await _reportRepo.UpdateListingStatus(upload.ProvidingOrganizationId, masterListing.RentalListingId);

            _unitOfWork.Commit();

            if (isLastLine)
            {
                await _reportRepo.UpdateInactiveListings(upload.ProvidingOrganizationId);
            }

            tran.Commit();
            return true;
        }

        private void SaveUploadLine(DssUploadLine uploadLine, Dictionary<string, List<string>> errors, bool isValidationFailure, string systemError)
        {
            uploadLine.IsValidationFailure = isValidationFailure;
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

            // brandnew address
            var error = "";
            if (physicalAddress == null)
            {
                var newAddress = new DssPhysicalAddress
                {
                    OriginalAddressTxt = row.RentalAddress,
                };

                error = await _geocoder.GetAddressAsync(newAddress);

                if (error.IsEmpty() && newAddress.LocationGeometry is not null && newAddress.LocationGeometry is Point point)
                {
                    newAddress.ContainingOrganizationId = await _orgRepo.GetContainingOrganizationId(point);
                }

                await _addressRepo.AddPhysicalAddressAsync(newAddress);
                return (newAddress, error);
            }

            // same address as before
            if (physicalAddress!.OriginalAddressTxt.ToLower().Trim() == address.ToLower().Trim())
            {
                return (physicalAddress, "");
            }

            // different address after user edit or confirmation
            if ((physicalAddress.IsMatchCorrected != null && physicalAddress.IsMatchCorrected.Value) 
                || (physicalAddress.IsMatchVerified != null && physicalAddress.IsMatchVerified.Value))
            {
                //create a new physical address, link to the previous address, and flag is_changed_original_address
                var newAddress = _mapper.Map<DssPhysicalAddress>(physicalAddress);
                newAddress.OriginalAddressTxt = row.RentalAddress;
                newAddress.PhysicalAddressId = 0;

                newAddress.IsSystemProcessing = true;
                newAddress.IsChangedOriginalAddress = true;
                newAddress.ReplacingPhysicalAddressId = physicalAddress.PhysicalAddressId;

                listing.IsChangedOriginalAddress = true;

                _addressRepo.ReplaceAddress(listing, newAddress);

                return (newAddress, error);
            }
            // different address without user edit or confirmation
            else
            {
                var newAddress = new DssPhysicalAddress
                {
                    OriginalAddressTxt = row.RentalAddress,
                };

                error = await _geocoder.GetAddressAsync(newAddress);

                if (error.IsEmpty() && newAddress.LocationGeometry is not null && newAddress.LocationGeometry is Point point)
                {
                    newAddress.ContainingOrganizationId = await _orgRepo.GetContainingOrganizationId(point);
                }

                newAddress.IsSystemProcessing = true;
                newAddress.IsChangedOriginalAddress = true;
                newAddress.ReplacingPhysicalAddressId = physicalAddress.PhysicalAddressId;

                listing.IsChangedOriginalAddress = true;

                if (physicalAddress.ContainingOrganizationId != newAddress.ContainingOrganizationId)
                {
                    listing.IsLgTransferred = true;
                    listing.LgTransferDtm = DateTime.UtcNow;
                }                

                _addressRepo.ReplaceAddress(listing, newAddress);

                return (newAddress, error);
            }
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

            masterListing.EffectiveHostNm = CommonUtils.SanitizeAndUppercaseString(row.PropertyHostNm);

            var managingOrgId = physicalAddress.ContainingOrganizationId.HasValue ?
                await _orgRepo.GetManagingOrgId(physicalAddress.ContainingOrganizationId.Value) : null;

            var needBusinessLicenceLink = await NeedBusinessLicenceLink(masterListing, managingOrgId);

            if (needBusinessLicenceLink)
            {
                if (!string.IsNullOrEmpty(masterListing.BusinessLicenceNo) && managingOrgId.HasValue)
                {
                    var sanitizedBizLicNo = CommonUtils.SanitizeAndUppercaseString(masterListing.BusinessLicenceNo);

                    var (businessLicenceId, businessLicenceNo) = await _bizLicRepo.GetMatchingBusinessLicenceIdAndNo(
                        managingOrgId.Value,
                        sanitizedBizLicNo
                    );

                    masterListing.GoverningBusinessLicenceId = businessLicenceId;
                    masterListing.EffectiveBusinessLicenceNo = businessLicenceNo ?? sanitizedBizLicNo;
                }
                else
                {
                    masterListing.GoverningBusinessLicenceId = null;
                    masterListing.EffectiveBusinessLicenceNo = string.Empty;
                }
            }

            (masterListing.NightsBookedQty, masterListing.SeparateReservationsQty) = 
                await _reportRepo.GetYtdValuesOfListingAsync(reportPeriodYm, offeringOrg.OrganizationId, masterListing.PlatformListingNo);

            //if these are changed, the master listing must be updated as well
            masterListing.IsLgTransferred = listing.IsLgTransferred != null ? listing.IsLgTransferred : masterListing.IsLgTransferred;
            masterListing.LgTransferDtm = listing.LgTransferDtm != null ? listing.LgTransferDtm : masterListing.LgTransferDtm;
            masterListing.IsChangedOriginalAddress = listing.IsChangedOriginalAddress != null ? listing.IsChangedOriginalAddress : masterListing.IsChangedOriginalAddress;

            return (true, masterListing);
        }

        private async Task<bool> NeedBusinessLicenceLink(DssRentalListing masterListing, long? managingOrgId)
        {
            // If there's no existing link, a link is needed
            if (masterListing.GoverningBusinessLicenceId == null)
                return true;

            // Get business licence number and organization ID from the repository
            var (blNo, orgId) = await _bizLicRepo.GetBizLicenceNoAndLgId(masterListing.GoverningBusinessLicenceId.Value);

            // A link is needed if the business licence number is null (not possible due to FK restriction)
            if (string.IsNullOrEmpty(blNo))
                return true;

            // A de-link is needed if the listing has no jurisdiction
            if (!managingOrgId.HasValue)
                return true;

            // A re-link is needed if the listing has been reassigned to a different jurisdiction
            if (managingOrgId.Value != orgId)
                return true;

            // Keep the overridden link if the business licence has been changed
            if (masterListing.IsChangedBusinessLicence == true)
                return false;

            // A re-link is needed if the platform BL has been changed.
            return CommonUtils.SanitizeAndUppercaseString(blNo) != CommonUtils.SanitizeAndUppercaseString(masterListing.BusinessLicenceNo ?? "");
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

        public async Task CleaupAddressAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            var addresses = await _addressRepo.GetPhysicalAddressesToCleanUpAsync();

            var totalCount = addresses.Count;
            var processedCount = 0;

            foreach (var address in addresses)
            {
                var stopwatchForGeocoder = Stopwatch.StartNew();

                var error = await _geocoder.GetAddressAsync(address);

                if (error.IsEmpty() && address.LocationGeometry is not null && address.LocationGeometry is Point point)
                {
                    address.ContainingOrganizationId = await _orgRepo.GetContainingOrganizationId(point);
                    address.IsSystemProcessing = true;
                }
                else
                {
                    address.IsSystemProcessing = false; //system error
                }

                processedCount++;

                stopwatchForGeocoder.Stop();

                var validationErrors = ValidateStringLengths(address);
                if (validationErrors.Any())
                {
                    _logger.LogWarning($"Address Cleanup: {address.OriginalAddressTxt} - Address validation errors: {string.Join(", ", validationErrors)}");

                    await _addressRepo.ReloadAddressAsync(address);
                    address.IsSystemProcessing = false;
                }

                _logger.LogInformation($"Address Cleanup (geocoder): {stopwatchForGeocoder.Elapsed.TotalMilliseconds} milliseconds - {processedCount}/{totalCount}");

                _unitOfWork.Commit();
            }

            stopwatch.Stop();
            _logger.LogInformation($"Address Cleanup Finished: {stopwatch.Elapsed.TotalSeconds} seconds");
        }

        private static List<string> ValidateStringLengths(DssPhysicalAddress address)
        {
            var errors = new List<string>();
            var maxLengths = new Dictionary<string, int>
                {
                    { nameof(DssPhysicalAddress.OriginalAddressTxt), 250 },
                    { nameof(DssPhysicalAddress.MatchAddressTxt), 250 },
                    { nameof(DssPhysicalAddress.SiteNo), 50 },
                    { nameof(DssPhysicalAddress.BlockNo), 50 },
                    { nameof(DssPhysicalAddress.UnitNo), 50 },
                    { nameof(DssPhysicalAddress.CivicNo), 50 },
                    { nameof(DssPhysicalAddress.StreetNm), 50 },
                    { nameof(DssPhysicalAddress.StreetTypeDsc), 50 },
                    { nameof(DssPhysicalAddress.StreetDirectionDsc), 50 },
                    { nameof(DssPhysicalAddress.LocalityNm), 50 },
                    { nameof(DssPhysicalAddress.LocalityTypeDsc), 50 },
                    { nameof(DssPhysicalAddress.ProvinceCd), 5 }
                };

            foreach (var property in address.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(address) as string;
                    if (value != null && maxLengths.ContainsKey(property.Name) && value.Length > maxLengths[property.Name])
                    {
                        errors.Add($"{property.Name} exceeds maximum length of {maxLengths[property.Name]} characters.");
                    }
                }
            }

            return errors;
        }

    }
}
