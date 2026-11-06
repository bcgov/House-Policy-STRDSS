using DotNet.Testcontainers.Configurations;
using Testcontainers.PostgreSql;
using Xunit;

namespace StrDss.Data.IntegrationTest
{
    public sealed class PostgresFixture : IAsyncLifetime
    {
        public const string DatabaseName = "integration_tests";

        private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgis/postgis:18-3.6")
            .WithDatabase(DatabaseName)
            .WithUsername("integration_test")
            .WithPassword("integration_test")
            .WithBindMount(Path.Combine(AppContext.BaseDirectory, "database-init"),
                "/docker-entrypoint-initdb.d", AccessMode.ReadOnly)
            .Build();

        public string ConnectionString => _container.GetConnectionString();

        public Task InitializeAsync() => _container.StartAsync();

        public async Task DisposeAsync() => await _container.DisposeAsync();
    }

    [CollectionDefinition(Name)]
    public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
    {
        public const string Name = "PostgreSQL integration tests";
    }
}
