using Microsoft.Extensions.Configuration;
using Npgsql;

namespace StrDss.Data
{
    public class ConnectionStringProvider
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Returns the connection string built from configuration.
        /// </summary>
        /// <param name="configure">
        /// Optional callback that receives the builder after the configured values are applied,
        /// so callers can customize it (e.g. Timeout, CommandTimeout).
        /// </param>
        public string GetConnectionString(Action<NpgsqlConnectionStringBuilder>? configure = null)
        {
            var builder = new NpgsqlConnectionStringBuilder();

            builder.Host = Require("DB_HOST");
            builder.Database = Require("DB_NAME");
            builder.Username = Require("DB_USER");
            builder.Password = Require("DB_PASS");

            // An omitted DB_PORT leaves Npgsql's default, which is the PostgreSQL port 5432.
            var dbPort = _configuration.GetValue<string>("DB_PORT");
            if (!string.IsNullOrWhiteSpace(dbPort))
            {
                if (!int.TryParse(dbPort, out var port) || port is < 1 or > 65535)
                    throw new InvalidOperationException($"DB_PORT '{dbPort}' is not a valid port. Expected an integer between 1 and 65535.");

                builder.Port = port;
            }

            configure?.Invoke(builder);

            return builder.ConnectionString;
        }

        private string Require(string key)
        {
            var value = _configuration.GetValue<string>(key);
            
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{key} must be configured.");
            }

            return value;
        }
    }
}
