namespace server.AppHost.Resources;

internal sealed class HangfireResources(IDistributedApplicationBuilder builder, DatabaseResources database)
{
    public IResourceBuilder<ProjectResource>? Configure()
    {
        var enabled = bool.TryParse(
            builder.Configuration["AppHost:Services:Hangfire:Enabled"], out var configuredEnabled)
            && configuredEnabled;

        if (enabled)
        {
            var resource = database.Configure(builder.AddProject<Projects.StrDss_Hangfire>("strdss-hangfire"));
            return resource;
        }

        return null;
    }
}
