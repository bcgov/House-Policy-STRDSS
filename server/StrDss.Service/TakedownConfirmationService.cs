using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Service.CsvHelpers;
using System.Diagnostics;

namespace StrDss.Service
{
    public interface ITakedownConfirmationService
    {
        Task ProcessTakedownConfirmationUploadAsync(DssUploadDelivery upload);
        Task ProcessTakedownConfirmationUploadsAsync();
    }
    public class TakedownConfirmationService : ServiceBase, ITakedownConfirmationService
    {
        private IUploadDeliveryRepository _uploadRepo;
        private IRentalListingReportRepository _listingRepo;
        private IEmailMessageRepository _emailRepo;
        private IOrganizationRepository _orgRepo;

        public TakedownConfirmationService(
            IUploadDeliveryRepository uploadRepo, IRentalListingReportRepository listingRepo, IEmailMessageRepository emailRepo, IOrganizationRepository orgRepo,
            ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _uploadRepo = uploadRepo;
            _listingRepo = listingRepo;
            _emailRepo = emailRepo;
            _orgRepo = orgRepo;
        }

        public async Task ProcessTakedownConfirmationUploadsAsync()
        {
            var uploads = await _uploadRepo.GetUploadsToProcessForOneYearAsync(UploadDeliveryTypes.TakedownData);

            foreach (var upload in uploads)
            {
                await ProcessTakedownConfirmationUploadAsync(upload);
            }
        }
        
        public async Task ProcessTakedownConfirmationUploadAsync(DssUploadDelivery upload)
        {
            var processStopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Processing Takedown Confirmation ({upload.UploadDeliveryId}) {upload.ProvidingOrganization.OrganizationNm} - {upload.ReportPeriodYm}");

            var processedCount = 0;

            var header = upload.SourceHeaderTxt ?? "";

            var linesToProcess = await _uploadRepo.GetUploadLineEntitiesToProcessAsync(upload.UploadDeliveryId);
            var lineCount = linesToProcess.Count;

            foreach (var line in linesToProcess)
            {
                var errors = new Dictionary<string, List<string>>();
                var csvConfig = CsvHelperUtils.GetConfig(errors, false);

                var csv = header + Environment.NewLine + line.SourceLineTxt;
                var textReader = new StringReader(csv);
                var csvReader = new CsvReader(textReader, csvConfig);

                var (orgCd, listingId) = await ProcessLine(upload, header, line, csvReader);
                processedCount++;
            }

            upload.UploadStatus = UploadStatus.Processed;
            upload.RegistrationStatus = UploadStatus.Processed;
            upload.UploadLinesTotal = lineCount;
            upload.UploadLinesProcessed = processedCount;
            upload.UploadLinesSuccess = processedCount;
            upload.UploadLinesError = 0;
            upload.RegistrationLinesSuccess = 0;
            upload.RegistrationLinesError = 0;
            _unitOfWork.Commit();

            processStopwatch.Stop();

            _logger.LogInformation($"Finished Takedown Confirmation: {upload.ReportPeriodYm?.ToString("yyyy-MM")}, {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
        }

        private async Task<(string orgCd, string listingId)> ProcessLine(DssUploadDelivery upload, string header, DssUploadLine line, CsvReader csvReader)
        {
            csvReader.Read();

            csvReader.ReadHeader();

            csvReader.Read();

            var row = csvReader.GetRecord<UploadLine>(); //it has been parsed once, so no exception expected.

            _logger.LogDebug($"Takedown Confirmation - Processing listing ({row.OrgCd} - {row.ListingId})");

            var org = await _orgRepo.GetOrganizationByOrgCdAsync(row.OrgCd);

            var listing = await _listingRepo.GetMasterListingAsync(org.OrganizationId, row.ListingId);

            if (listing == null)
            {
                _logger.LogDebug($"Takedown Confirmation - Skipping listing (not found) - ({row.OrgCd} - {row.ListingId})");
                return (row.OrgCd, row.ListingId);
            }

            if (listing.IsTakenDown == true)
            {
                _logger.LogDebug($"Takedown Confirmation - Skipping listing (already taken down) - ({row.OrgCd} - {row.ListingId})");
            }
            else
            {
                listing.IsTakenDown = true;
                listing.TakeDownReason = row.TakeDownReason;

                var emailEntity = new DssEmailMessage
                {
                    EmailMessageType = EmailMessageTypes.CompletedTakedown,
                    MessageDeliveryDtm = DateTime.UtcNow,
                    MessageTemplateDsc = "",
                    IsHostContactedExternally = false,
                    IsSubmitterCcRequired = false,
                    LgPhoneNo = null,
                    UnreportedListingNo = null,
                    HostEmailAddressDsc = null,
                    LgEmailAddressDsc = null,
                    CcEmailAddressDsc = null,
                    UnreportedListingUrl = null,
                    LgStrBylawUrl = null,
                    InvolvedInOrganizationId = upload.ProvidingOrganizationId,
                    ConcernedWithRentalListingId = listing.RentalListingId
                };

                await _emailRepo.AddEmailMessage(emailEntity);
            }

            line.IsProcessed = true;
            line.IsRegistrationFailure = false;

            _unitOfWork.Commit();

            _logger.LogInformation($"Takedown Confirmation - listing taken down successfully - ({row.OrgCd} - {row.ListingId})");

            return (row.OrgCd, row.ListingId);
        }
    }
}
