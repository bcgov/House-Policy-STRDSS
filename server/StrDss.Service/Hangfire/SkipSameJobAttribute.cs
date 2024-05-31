using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using System.Text.Json;
using Hangfire;
using Npgsql;
using StrDss.Common;

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
            var cutoffTime = DateTime.UtcNow.AddHours(-12);

            foreach (var processingJob in allJobs)
            {
                if (processingJob.Value.StartedAt < cutoffTime)
                {
                    Console.WriteLine($"[Hangfire] Deleting stalled job: {processingJob.Key} {processingJob.Value.InProcessingState} ");
                    BackgroundJob.Delete(processingJob.Key);
                }
            }

            //skip same job
            var fingerprints = monitor.ProcessingJobs(0, 999999999)
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
                //DeleteHangfireLock($"hangfire:{resource}");
                //var distributedLock = filterContext.Connection.AcquireDistributedLock(resource, timeout);
                //filterContext.Items["DistributedLock"] = distributedLock;

                Console.WriteLine(ex);
                throw;
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

        private void DeleteHangfireLock(string resourceName)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var connString = $"Host={dbHost};Username={dbUser};Password={dbPass};Database={dbName};Port={dbPort};";

            string sql = @"DELETE FROM Hangfire.Lock WHERE Resource = @ResourceName;";

            using var connection = new NpgsqlConnection(connString);
            connection.Open();

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ResourceName", resourceName);
            command.ExecuteNonQuery();
        }
    }
}
