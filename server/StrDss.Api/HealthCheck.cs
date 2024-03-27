using System.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace StrDss.Api
{
    public class HealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public HealthCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    if (connection.State == ConnectionState.Open)
                    {
                        return HealthCheckResult.Healthy("Database connection is healthy.");
                    }
                    else
                    {
                        return HealthCheckResult.Unhealthy("Could not establish a connection to the database.");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return HealthCheckResult.Unhealthy("An error occurred while connecting to the database.", ex);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("An error occurred while connecting to the database.", ex);
            }
        }
    }
}
