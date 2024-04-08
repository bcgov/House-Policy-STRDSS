using Hangfire;

namespace StrDss.Service.Hangfire
{
    public class HangfireJobs
    {
        private IRentalListingReportService _linstingService;

        public HangfireJobs(IRentalListingReportService listingService)
        {
            _linstingService = listingService;
        }

        [Queue("default")]
        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessRentalListingReport()
        {
            await _linstingService.ProcessReportAsync();
        }
    }
}
