using Hangfire;

namespace StrDss.Service.Hangfire
{
    public class HangfireJobs
    {
        private IRentalListingReportService _linstingReportService;
        private IDelistingService _delistingService;
        private IRentalListingService _listingService;
        private ITakedownConfirmationService _tdcService;
        private IBizLicenceService _bizLicService;

        public HangfireJobs(IRentalListingReportService listingReportService, IRentalListingService listingService, IDelistingService delistingService,
            ITakedownConfirmationService tdcService, IBizLicenceService bizLicService)
        {
            _linstingReportService = listingReportService;
            _delistingService = delistingService;
            _listingService = listingService;
            _tdcService = tdcService;
            _bizLicService = bizLicService;
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessRentalListingReports()
        {
            await _linstingReportService.ProcessRentalReportUploadAsync();
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessTakedownRequestBatchEmails()
        {
            await _delistingService.ProcessTakedownRequestBatchEmailsAsync();
        }

        //[Queue("default")]
        //[SkipSameJob]
        //[AutomaticRetry(Attempts = 0)]
        //public async Task CleanUpAddresses()
        //{
        //    await _linstingReportService.CleaupAddressAsync();
        //}

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
        public async Task ProcessTakedownConfirmation()
        {
            await _tdcService.ProcessTakedownConfrimationAsync();
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessBusinessLicences()
        {
            await _bizLicService.ProcessBizLicenceUploadAsync();
        }


    }
}
