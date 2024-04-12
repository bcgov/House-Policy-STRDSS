using Hangfire;

namespace StrDss.Service.Hangfire
{
    public class HangfireJobs
    {
        private IRentalListingReportService _linstingService;
        private IDelistingService _delistingService;

        public HangfireJobs(IRentalListingReportService listingService, IDelistingService delistingService)
        {
            _linstingService = listingService;
            _delistingService = delistingService;
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessRentalListingReport()
        {
            await _linstingService.ProcessReportAsync();
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessTakedownRequestBatchEmails()
        {
            await _delistingService.ProcessTakedownRequestBatchEmailsAsync();
        }
    }
}
