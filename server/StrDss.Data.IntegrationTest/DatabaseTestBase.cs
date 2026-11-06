using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using Xunit;

namespace StrDss.Data.IntegrationTest
{
    public abstract class DatabaseTestBase : IAsyncLifetime
    {
        private readonly PostgresFixture _fixture;
        private readonly string _databaseName = $"test_{Guid.NewGuid():N}";
        private bool _databaseCreated;
        private string _connectionString = null!;

        protected DatabaseTestBase(PostgresFixture fixture)
        {
            _fixture = fixture;
        }

        protected DssDbContext DbContext { get; private set; } = null!;

        protected ICurrentUser CurrentUser { get; } = new CurrentUser
        {
            UserGuid = Guid.NewGuid(),
            UserName = "integration_test"
        };

        protected IMapper Mapper { get; } = new MapperConfiguration(configuration =>
            configuration.AddMaps(typeof(DssDbContext).Assembly)).CreateMapper();

        public async Task InitializeAsync()
        {
            try
            {
                await ExecuteAdminSqlAsync($"CREATE DATABASE \"{_databaseName}\" TEMPLATE \"{PostgresFixture.DatabaseName}\"");
                _databaseCreated = true;
                _connectionString = new NpgsqlConnectionStringBuilder(_fixture.ConnectionString)
                {
                    Database = _databaseName,
                    Pooling = false
                }.ConnectionString;

                DbContext = CreateDbContext();
            }
            catch
            {
                await DisposeAsync();
                throw;
            }
        }

        protected DssDbContext CreateDbContext(params IInterceptor[] interceptors)
        {
            var options = new DbContextOptionsBuilder<DssDbContext>()
                .UseNpgsql(_connectionString, options => options.UseNetTopologySuite())
                .AddInterceptors(interceptors)
                .Options;

            return new DssDbContext(options, CurrentUser, NullLogger<StrDssLogger>.Instance);
        }

        public async Task DisposeAsync()
        {
            if (DbContext != null)
            {
                await DbContext.DisposeAsync();
                DbContext = null!;
            }

            if (_databaseCreated)
            {
                await ExecuteAdminSqlAsync($"DROP DATABASE IF EXISTS \"{_databaseName}\" WITH (FORCE)");
                _databaseCreated = false;
            }
        }

        private async Task ExecuteAdminSqlAsync(string sql)
        {
            var connectionString = new NpgsqlConnectionStringBuilder(_fixture.ConnectionString)
            {
                Database = "postgres",
                Pooling = false
            }.ConnectionString;

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
