using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using System.Text.Json;
using Hangfire;

namespace StrDss.Service.Hangfire
{
    public sealed class SkipSameJobAttribute : JobFilterAttribute, IClientFilter, IServerFilter
    {
        private readonly int _timeoutInSeconds = 1;

        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnCreating(CreatingContext context)
        {
            var job = context.Job;
            var jobFingerprint = GetJobFingerprint(job);

            //delete stalled jobs
            var monitor = context.Storage.GetMonitoringApi();
            var allJobs = monitor.ProcessingJobs(0, 999999999);
            var cutoffTime = DateTime.UtcNow.AddMinutes(-5);

            foreach (var processingJob in allJobs)
            {
                if (processingJob.Value.StartedAt < cutoffTime)
                {
                    BackgroundJob.Delete(processingJob.Key);
                }
            }

            var fingerprints = allJobs
                .Select(x => GetJobFingerprint(x.Value.Job))
                .ToList();

            fingerprints.AddRange(
                monitor.EnqueuedJobs("default", 0, 999999999)
                .Select(x => GetJobFingerprint(x.Value.Job))
            );

            foreach (var fingerprint in fingerprints)
            {
                if (jobFingerprint != fingerprint)
                    continue;

                context.Canceled = true;

                return;
            }
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var resource = GetJobFingerprint(filterContext.BackgroundJob.Job);

            var timeout = TimeSpan.FromSeconds(_timeoutInSeconds);

            try
            {
                var distributedLock = filterContext.Connection.AcquireDistributedLock(resource, timeout);
                filterContext.Items["DistributedLock"] = distributedLock;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (!filterContext.Items.ContainsKey("DistributedLock"))
            {
                return;
            }

            var distributedLock = (IDisposable)filterContext.Items["DistributedLock"];
            distributedLock.Dispose();
        }

        private string GetJobFingerprint(Job job)
        {
            var args = "";

            if (job.Args.Count > 0)
            {
                args = "-" + JsonSerializer.Serialize(job.Args);
            }

            return $"{job.Type.FullName}-{job.Method.Name}{args}";
        }
    }
}
