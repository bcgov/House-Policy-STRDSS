using Microsoft.EntityFrameworkCore;
using StrDss.Data.Entities;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingLatestReportRepository
    {
        /// <summary>
        /// Refreshes <c>dss_rental_listing_latest_report_mv</c> and then vacuums and analyzes it.
        /// Each statement runs as its own autocommit command. The duration of this method is the full maintenance time.
        /// </summary>
        Task RefreshLatestReportPeriodAsync();
    }

    public class RentalListingLatestReportRepository : IRentalListingLatestReportRepository
    {
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
                var connection = _dbContext.Database.GetDbConnection();
                foreach (var sql in MaintenanceStatements)
                {
                    await using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.Transaction = null;
                    await command.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                await _dbContext.Database.CloseConnectionAsync();
            }
        }
    }
}
