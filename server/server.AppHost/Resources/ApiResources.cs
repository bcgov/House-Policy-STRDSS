namespace server.AppHost.Resources;

internal sealed class ApiResources(IDistributedApplicationBuilder builder, DatabaseResources database)
{
    public IResourceBuilder<ProjectResource>? Configure()
    {
        var enabled = !bool.TryParse(
            builder.Configuration["AppHost:Services:Api:Enabled"], out var configuredEnabled)
            || configuredEnabled;

        return enabled
            ? database.Configure(builder.AddProject<Projects.StrDss_Api>("strdss-api"))
            : null;
    }
}
