using Hangfire;
using StrDss.Common;
using StrDss.Data.Repositories;

namespace StrDss.Service.Hangfire
{
    public class HangfireJobs
    {
        private IRentalListingReportService _linstingReportService;
        private IDelistingService _delistingService;
        private IRentalListingService _listingService;
        private ITakedownConfirmationService _tdcService;
        private IBizLicenceService _bizLicService;
        private IUploadDeliveryService _uploadService;
        private IRegistrationService _registrationService;
        private IRentalListingLatestReportRepository _latestReportRepository;

        public HangfireJobs(IRentalListingReportService listingReportService, IRentalListingService listingService, IDelistingService delistingService,
            ITakedownConfirmationService tdcService, IBizLicenceService bizLicService, IUploadDeliveryService uploadService, IRegistrationService registrationService,
            IRentalListingLatestReportRepository latestReportRepository)
        {
            _linstingReportService = listingReportService;
            _delistingService = delistingService;
            _listingService = listingService;
            _tdcService = tdcService;
            _bizLicService = bizLicService;
            _uploadService = uploadService;
            _registrationService = registrationService;
            _latestReportRepository = latestReportRepository;
        }

        /// <summary>
        /// Refreshes the latest-report materialized view after a listing upload is fully processed.
        /// Enqueued with <see cref="BackgroundJob.Enqueue"/>; this is not a recurring job.
        /// <see cref="SkipSameJobAttribute"/> drops a second enqueue while one is queued or running.
        /// </summary>
        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task RefreshLatestRentalListingReportAsync()
        {
            await _latestReportRepository.RefreshLatestReportPeriodAsync();
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessTakedownRequestBatchEmails()
        {
            await _delistingService.ProcessTakedownRequestBatchEmailsAsync();
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task CreateRentalListingExportFiles()
        {
            await _listingService.CreateRentalListingExportFiles();
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessUpload()
        {
            var listingDataToProcessExists = await _listingService.ListingDataToProcessExists();

            if (!listingDataToProcessExists)
            {
                await _listingService.ResetLgTransferFlag();
            }

            var upload = await _uploadService.GetNonTakedownUploadToProcessAsync();

            // takedown upload can have not-found listings which never gets processed until the listings exist
            // so process it only when there's nothing to process
            upload ??= await _uploadService.GetUploadToProcessAsync(UploadDeliveryTypes.TakedownData);

            if (upload == null) return;

            var reportType = upload.UploadDeliveryType;

            switch (reportType)
            {
                case UploadDeliveryTypes.ListingData:
                    await _linstingReportService.ProcessRentalReportUploadAsync(upload);
                    break;
                case UploadDeliveryTypes.TakedownData:
                    await _tdcService.ProcessTakedownConfirmationUploadsAsync();
                    break;
                case UploadDeliveryTypes.LicenceData:
                    await _bizLicService.ProcessBizLicenceUploadMainAsync(upload);
                    break;
                case UploadDeliveryTypes.RegistrationData:
                    await _registrationService.ProcessRegistrationDataUploadAsync(upload);
                    break;
                default:
                    break;
            }
        }
    }
}
