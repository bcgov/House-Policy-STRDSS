using Hangfire.Common;
using Hangfire.Server;
using Hangfire;

namespace StrDss.Service.Hangfire
{
    /// <summary>
    /// Usage:
    /// 
    ///     [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
    /// 
    /// </summary>

    public class DisableConcurrentExecutionAttribute : JobFilterAttribute, IServerFilter
    {
        private readonly int _timeoutInSeconds;

        public DisableConcurrentExecutionAttribute(int timeoutInSeconds)
        {
            _timeoutInSeconds = timeoutInSeconds;
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var jobId = filterContext.BackgroundJob.Id;
            var connection = JobStorage.Current.GetConnection();
            var currentJob = connection.GetJobData(jobId);

            if (currentJob != null && currentJob.State == "Processing")
            {
                throw new InvalidOperationException("This job is already being processed.");
            }
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }
    }
}
