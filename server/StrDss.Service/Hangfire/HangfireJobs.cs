using Hangfire;
using StrDss.Common;

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

        public HangfireJobs(IRentalListingReportService listingReportService, IRentalListingService listingService, IDelistingService delistingService,
            ITakedownConfirmationService tdcService, IBizLicenceService bizLicService, IUploadDeliveryService uploadService)
        {
            _linstingReportService = listingReportService;
            _delistingService = delistingService;
            _listingService = listingService;
            _tdcService = tdcService;
            _bizLicService = bizLicService;
            _uploadService = uploadService;
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
            var upload = await _uploadService.GetUploadToProcessAsync();

            if (upload == null) return;

            var reportType = upload.UploadDeliveryType;

            switch (reportType)
            {
                case UploadDeliveryTypes.ListingData:
                    await _linstingReportService.ProcessRentalReportUploadAsync(upload);
                    break;
                case UploadDeliveryTypes.TakedownData:
                    await _tdcService.ProcessTakedownConfirmationUploadAsync(upload);
                    break;
                case UploadDeliveryTypes.LicenceData:
                    await _bizLicService.ProcessBizLicenceUploadMainAsync(upload);
                    break;
                default:
                    break;
            }
        }
    }
}
