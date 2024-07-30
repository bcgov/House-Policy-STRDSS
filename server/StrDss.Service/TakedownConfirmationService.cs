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
        Task ProcessTakedownConfrimationAsync();
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

        public async Task ProcessTakedownConfrimationAsync()
        {
            var upload = await _uploadRepo.GetUploadToProcessAsync(UploadDeliveryTypes.TakedownData);

            if (upload != null)
            {
                await ProcessTakedownConfirmationUploadAsync(upload);
            }
        }

        private async Task ProcessTakedownConfirmationUploadAsync(DssUploadDelivery upload)
        {
            var processStopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Processing Takedown Confirmation {upload.ProvidingOrganizationId} - {upload.ReportPeriodYm} - {upload.ProvidingOrganization.OrganizationNm}");

            var reportPeriodYm = (DateOnly)upload.ReportPeriodYm!;
            var lineCount = await _uploadRepo.GetTotalNumberOfUploadLines(upload.UploadDeliveryId);
            var count = 0;

            var header = "rpt_period,org_cd,listing_id,rpt_type"; //todo: upload.Header

            var linesToProcess = await _uploadRepo.GetUploadLineEntitiesToProcessAsync(upload.UploadDeliveryId);

            foreach (var line in linesToProcess)
            {
                count++;
                var errors = new Dictionary<string, List<string>>();
                var csvConfig = CsvHelperUtils.GetConfig(errors, false);

                var csvString = header + Environment.NewLine + line.SourceLineTxt;
                var textReader = new StringReader(csvString);
                var csvReader = new CsvReader(textReader, csvConfig);

                await ProcessLine(upload, header, line, textReader, csvReader);
            }

            processStopwatch.Stop();
            _logger.LogInformation($"Finished: {upload.ReportPeriodYm?.ToString("yyyy-MM")}, {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
        }

        private async Task ProcessLine(DssUploadDelivery upload, string header, DssUploadLine line, StringReader textReader, CsvReader csvReader)
        {
            var stopwatch = Stopwatch.StartNew();

            csvReader.Read();

            csvReader.ReadHeader();

            while (csvReader.Read())
            {
                var row = csvReader.GetRecord<UploadLine>(); //it has been parsed once, so no exception expected.

                _logger.LogInformation($"Processing listing ({row.OrgCd} - {row.ListingId})");

                var org = await _orgRepo.GetOrganizationByOrgCdAsync(row.OrgCd);

                var listing = await _listingRepo.GetMasterListingAsync(org.OrganizationId, row.ListingId);

                if (listing == null)
                {
                    _logger.LogInformation($"Skipping listing - ({row.OrgCd} - {row.ListingId})");
                    continue;
                }

                listing.IsTakenDown = true;
                line.IsProcessed = true;

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

                _unitOfWork.Commit();

                stopwatch.Stop();
                _logger.LogInformation($"Finishing listing ({row.OrgCd} - {row.ListingId}): {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
            }
        }
    }
}
