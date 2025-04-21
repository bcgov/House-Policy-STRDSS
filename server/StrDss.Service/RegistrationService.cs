using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using StrDss.Service.CsvHelpers;
using StrDss.Service.EmailTemplates;
using StrDss.Service.HttpClients;
using System.Diagnostics;
using System.Text;
using NetTopologySuite.Geometries;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StrDss.Service
{
    public interface IRegistrationService
    {
        Task ProcessRegistrationDataUploadAsync(DssUploadDelivery upload);
    }
    public class RegistrationService : ServiceBase, IRegistrationService
    {
        private IUploadDeliveryRepository _uploadRepo;
        private IRentalListingReportRepository _reportRepo;
        private IPhysicalAddressRepository _addressRepo;
        private IUserRepository _userRepo;
        private IEmailMessageService _emailService;
        private IEmailMessageRepository _emailRepo;
        private IConfiguration _config;
        private IPermitValidationService _permitValidation;
        private string? _apiAccount;

        public RegistrationService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IUploadDeliveryRepository uploadRepo, IRentalListingReportRepository reportRepo, IPhysicalAddressRepository addressRepo,
            IUserRepository userRepo, IEmailMessageService emailService, IEmailMessageRepository emailRepo, IConfiguration config, IPermitValidationService permitValidation,
            ILogger<StrDssLogger> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _uploadRepo = uploadRepo;
            _reportRepo = reportRepo;
            _addressRepo = addressRepo;
            _userRepo = userRepo;
            _emailService = emailService;
            _emailRepo = emailRepo;
            _config = config;
            _permitValidation = permitValidation;
        }

        public async Task ProcessRegistrationDataUploadAsync(DssUploadDelivery upload)
        {
            var processStopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Processing Registration Data {upload.ProvidingOrganizationId} - {upload.ProvidingOrganization.OrganizationNm}");

            int lineCount = await _uploadRepo.GetTotalNumberOfUploadLines(upload.UploadDeliveryId);
            int count = 0;
            List<UploadLineToProcess> linesToProcess = await _uploadRepo.GetUploadLinesToProcessAsync(upload.UploadDeliveryId);
            Dictionary<string, List<string>> errors = new();
            CsvHelper.Configuration.CsvConfiguration csvConfig = CsvHelperUtils.GetConfig(errors, false);
            MemoryStream memoryStream = new(upload.SourceBin!);

            using TextReader textReader = new StreamReader(memoryStream, Encoding.UTF8);
            using var csv = new CsvReader(textReader, csvConfig);
            csv.Read();
            bool headerExists = csv.ReadHeader();
            bool hasError = false;
            bool isLastLine = false;
            int processedCount = 0;
            int errorCount = 0;

            while (csv.Read())
            {
                count++;
                isLastLine = count == lineCount;
                RegistrationDataRowUntyped row = csv.GetRecord<RegistrationDataRowUntyped>(); //it has been parsed once, so no exception expected.

                if (!linesToProcess.Any(x => x.OrgCd == row.OrgCd && x.SourceRecordNo == row.RegNo)) continue;

                var uploadLine = await _uploadRepo.GetUploadLineAsync(upload.UploadDeliveryId, row.OrgCd, row.RegNo);

                if (uploadLine == null || uploadLine.IsProcessed)
                {
                    _logger.LogInformation($"Skipping registration data - ({row.OrgCd} - {row.RegNo})");
                    continue;
                }

                _logger.LogInformation($"Processing registration data - ({row.OrgCd} - {row.RegNo})");

                var stopwatch = Stopwatch.StartNew();
                hasError = !await ProcessUploadLine(upload, upload.ProvidingOrganizationId, uploadLine, row, isLastLine);
                stopwatch.Stop();

                processedCount++;
                if (hasError) errorCount++;

                _logger.LogInformation($"Finishing listing ({row.OrgCd} - {row.RegNo}): {stopwatch.Elapsed.TotalMilliseconds} milliseconds - {processedCount}");
            }

            if (!isLastLine)
            {
                processStopwatch.Stop();
                _logger.LogInformation($"Processed {processedCount} lines: {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
                return;
            }

            // Update the upload with the status and counts
            upload.UploadStatus = errorCount > 0 ? UploadStatus.Failed : UploadStatus.Processed;
            upload.RegistrationStatus = errorCount > 0 ? UploadStatus.Failed : UploadStatus.Processed;
            upload.UploadLinesTotal = lineCount;
            upload.UploadLinesProcessed = processedCount;
            upload.UploadLinesSuccess = processedCount - errorCount;
            upload.UploadLinesError = errorCount;
            upload.RegistrationLinesSuccess = processedCount - errorCount;
            upload.RegistrationLinesError = errorCount;
            _unitOfWork.Commit();

            if (upload.UploadUserGuid == null) return;
            var user = await _userRepo.GetUserByGuid((Guid)upload.UploadUserGuid!);
            if (user == null) return;


            var adminEmail = _config.GetValue<string>("ADMIN_EMAIL");
            if (adminEmail == null) return;

            using var transaction = _unitOfWork.BeginTransaction();

            //upload.UploadDeliveryId
            var template = new RegistrationValidationComplete(_emailService)
            {
                UserName = $"{user!.GivenNm}",
                Link = "https://www2.gov.bc.ca/assets/gov/housing-and-tenancy/tools-for-government/short-term-rentals/quickstartguide_validation_for_minor_platforms_final.pdf",
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


            processStopwatch.Stop();
            _logger.LogInformation($"Finished: {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
        }

        private async Task<bool> ProcessUploadLine(DssUploadDelivery upload, long OrgId, DssUploadLine uploadLine, RegistrationDataRowUntyped row, bool isLastLine)
        {
            // Does this upload line meet the validation rules
            var errors = new Dictionary<string, List<string>>();
            _validator.Validate(Entities.RegistrationDataRowUntyped, row, errors);
            if (errors.Count > 0)
            {
                SaveUploadLine(uploadLine, errors.ParseErrorWithUnderScoredKeyName(), true);
                return false;
            }

            bool isValid = false;
            string registrationTxt = "";
            if (!string.IsNullOrEmpty(row.RegNo))
            {
                // There is a reg no present, so we need to validate it
                (isValid, registrationTxt) = await _permitValidation.ValidateRegistrationPermitAsync(row.RegNo, row.RentalUnit, row.RentalStreet, row.RentalPostal);
                if (isValid)
                {
                    if (!string.IsNullOrEmpty(row.ListingId))
                    {
                        var listing = await _reportRepo.GetMasterListingAsync(OrgId, row.ListingId);
                        if (listing != null)
                        {
                            using var tran = _unitOfWork.BeginTransaction();
                            // Set the registration value here
                            listing.BcRegistryNo = row.RegNo;

                            var physicalAddress = await _addressRepo.GetPhysicalAdderssFromMasterListingAsync(listing.OfferingOrganizationId, listing.PlatformListingNo, row.RentalAddress);
                            if (physicalAddress != null)
                            {
                                // Set the unit, street, and postal code values here
                                physicalAddress.RegRentalUnitNo = row.RentalUnit;
                                physicalAddress.RegRentalStreetNo = row.RentalStreet;
                                physicalAddress.RegRentalPostalCode = row.RentalPostal;
                            }
                            tran.Commit();
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(row.RentalAddress)) // We have an address, so we need to determine if the property is in an exempt jurisdiction
            {
                (isValid, registrationTxt) = await _permitValidation.CheckStraaExemptionStatus(row.RentalAddress);
             }
            else
            {
                // We shouldn't ever hit here, but here we are.
                isValid = false;
                registrationTxt = RegistrationValidationText.DataInvalid;
            }

            SaveUploadLine(uploadLine, registrationTxt, !isValid);
            return isValid;
        }

        private void SaveUploadLine(DssUploadLine uploadLine, string registrationTxt, bool isRegistrationFailure)
        {
            uploadLine.IsRegistrationFailure = isRegistrationFailure;
            uploadLine.RegistrationTxt = registrationTxt;
            uploadLine.IsProcessed = true;
            _unitOfWork.Commit();
        }
    }
}
