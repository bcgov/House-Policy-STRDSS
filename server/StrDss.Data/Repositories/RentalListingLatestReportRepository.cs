using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using StrDss.Data.Entities;
using StrDss.Data.Telemetry;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingLatestReportRepository
    {
        /// <summary>
        /// Refreshes <c>dss_rental_listing_latest_report_mv</c> and then vacuums and analyzes it.
        /// Each statement runs as its own autocommit command.
        /// The method duration is the full maintenance time, and each statement records its own duration.
        /// </summary>
        Task RefreshLatestReportPeriodAsync();
    }

    public class RentalListingLatestReportRepository : IRentalListingLatestReportRepository
    {
        internal const string RefreshStatementName = "refresh";
        internal const string VacuumAnalyzeStatementName = "vacuum_analyze";

        internal const string RefreshConcurrentlySql =
            "REFRESH MATERIALIZED VIEW CONCURRENTLY dss_rental_listing_latest_report_mv";

        internal const string VacuumAnalyzeSql =
            "VACUUM ANALYZE dss_rental_listing_latest_report_mv";

        internal static readonly string[] MaintenanceStatements =
        [
            RefreshConcurrentlySql,
            VacuumAnalyzeSql
        ];

        private readonly DssDbContext _dbContext;

        public RentalListingLatestReportRepository(DssDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RefreshLatestReportPeriodAsync()
        {
            if (_dbContext.Database.CurrentTransaction != null)
            {
                throw new InvalidOperationException(
                    "Refreshing dss_rental_listing_latest_report_mv requires autocommit. An open transaction is active.");
            }

            await _dbContext.Database.OpenConnectionAsync();
            try
            {
                await ExecuteMaintenanceStatementsAsync(_dbContext.Database.GetDbConnection());
            }
            finally
            {
                await _dbContext.Database.CloseConnectionAsync();
            }
        }

        internal static async Task ExecuteMaintenanceStatementsAsync(DbConnection connection)
        {
            foreach (var sql in MaintenanceStatements)
            {
                var statement = MaintenanceStatementName(sql);
                var started = Stopwatch.GetTimestamp();
                try
                {
                    await using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.Transaction = null;
                    await command.ExecuteNonQueryAsync();
                    RecordStatementDuration(statement, started, "success");
                }
                catch (Exception)
                {
                    RecordStatementDuration(statement, started, "error");
                    throw;
                }
            }
        }

        internal static string MaintenanceStatementName(string sql)
        {
            if (sql == RefreshConcurrentlySql)
            {
                return RefreshStatementName;
            }

            if (sql == VacuumAnalyzeSql)
            {
                return VacuumAnalyzeStatementName;
            }

            throw new InvalidOperationException(
                $"No maintenance metric name is defined for '{sql}'.");
        }

        private static void RecordStatementDuration(string statement, long started, string outcome)
        {
            var tags = new TagList
            {
                { "repository", nameof(IRentalListingLatestReportRepository) },
                { "statement", statement },
                { "outcome", outcome },
            };

            RepositoryMetrics.MaintenanceStatementDuration.Record(
                Stopwatch.GetElapsedTime(started).TotalSeconds,
                tags);
        }
    }
}
