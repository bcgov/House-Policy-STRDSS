using Hangfire;

namespace StrDss.Service.Hangfire
{
    public class HangfireJobs
    {
        private IRentalListingReportService _linstingReportService;
        private IDelistingService _delistingService;
        private IRentalListingService _listingService;

        public HangfireJobs(IRentalListingReportService listingReportService, IRentalListingService listingService, IDelistingService delistingService)
        {
            _linstingReportService = listingReportService;
            _delistingService = delistingService;
            _listingService = listingService;
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

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task CleanUpAddresses()
        {
            await _linstingReportService.CleaupAddressAsync();
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task CreateRentalListingExportFiles()
        {
            await _listingService.CreateRentalListingExportFiles();
        }
    }
}
