namespace server.AppHost.Resources;

internal sealed class DatabaseResources
{
    private readonly IResourceBuilder<ParameterResource> databaseName;
    private readonly IResourceBuilder<ParameterResource> databaseUser;
    private readonly IResourceBuilder<ParameterResource> databasePassword;
    private readonly IResourceBuilder<ParameterResource>? databaseHost;
    private readonly IResourceBuilder<ParameterResource>? databasePort;
    private readonly IResourceBuilder<ContainerResource>? postgis;

    public DatabaseResources(IDistributedApplicationBuilder builder)
    {
        var postgresMode = builder.Configuration["AppHost:Services:Postgres:Mode"] ?? "Aspire";
        var useExternalDatabase = postgresMode.Equals("External", StringComparison.OrdinalIgnoreCase);
        if (!useExternalDatabase && !postgresMode.Equals("Aspire", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unsupported AppHost:Services:Postgres:Mode '{postgresMode}'. Supported values are 'Aspire' and 'External'.");
        }

        if (useExternalDatabase)
        {
            databaseName = builder.AddParameter("external-postgres-database", GetRequiredExternalSetting(builder, "Database"));
            databaseUser = builder.AddParameter("external-postgres-user", GetRequiredExternalSetting(builder, "User"));
            databasePassword = builder.AddParameter("external-postgres-password", GetRequiredExternalSetting(builder, "Password"), secret: true);
            databaseHost = builder.AddParameter("external-postgres-host", GetRequiredExternalSetting(builder, "Host"));
            databasePort = builder.AddParameter(
                "external-postgres-port",
                builder.Configuration["AppHost:Services:Postgres:External:Port"] ?? "5432");
        }
        else
        {
            databaseName = builder.AddParameter("postgres-database", "strdssdev");
            databaseUser = builder.AddParameter("postgres-user", "strdssdev");
            databasePassword = builder.AddParameter("postgres-password", "strdssdev", secret: true);

            postgis = builder.AddContainer("postgis", "postgis/postgis", "18-3.6")
                .WithEnvironment("POSTGRES_DB", databaseName)
                .WithEnvironment("POSTGRES_USER", databaseUser)
                .WithEnvironment("POSTGRES_PASSWORD", databasePassword)
                .WithEndpoint(name: "postgres", targetPort: 5432)
                .WithBindMount(Path.Combine(builder.AppHostDirectory, "database-init"), "/docker-entrypoint-initdb.d", isReadOnly: true)
                .WithVolume("strdss-postgis-data", "/var/lib/postgresql");
        }
    }

    public IResourceBuilder<ProjectResource> Configure(IResourceBuilder<ProjectResource> project)
    {
        project.WithEnvironment("DB_NAME", databaseName)
            .WithEnvironment("DB_USER", databaseUser)
            .WithEnvironment("DB_PASS", databasePassword);

        if (postgis is not null)
        {
            project.WithEnvironment("DB_HOST", postgis.GetEndpoint("postgres").Property(EndpointProperty.Host))
                .WithEnvironment("DB_PORT", postgis.GetEndpoint("postgres").Property(EndpointProperty.Port))
                .WaitFor(postgis);
        }
        else
        {
            project.WithEnvironment("DB_HOST", databaseHost!)
                .WithEnvironment("DB_PORT", databasePort!);
        }

        return project;
    }

    private static string GetRequiredExternalSetting(IDistributedApplicationBuilder builder, string settingName)
    {
        var key = $"AppHost:Services:Postgres:External:{settingName}";
        var value = builder.Configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{key} must be configured when PostgreSQL mode is 'External'.");
        }

        return value;
    }
}
