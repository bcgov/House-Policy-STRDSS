namespace server.AppHost.Resources;

internal sealed class FrontendResources(IDistributedApplicationBuilder builder)
{
    public IResourceBuilder<ContainerResource>? Configure(IResourceBuilder<ProjectResource>? api)
    {
        var enabled = !bool.TryParse(
            builder.Configuration["AppHost:Services:Frontend:Enabled"], out var configuredEnabled)
            || configuredEnabled;

        if (!enabled)
        {
            return null;
        }

        IResourceBuilder<ContainerResource> frontend = builder.AddDockerfile("frontend", "../../frontend", "Dockerfile.local")
            .WithHttpEndpoint(port: 4200, targetPort: 3001)
            .WithEnvironment("SSO_HOST", "https://dev.loginproxy.gov.bc.ca/auth")
            .WithEnvironment("ENV_NAME", "dev")
            .WithEnvironment("SM_LOGOFF_URL", "https://logontest7.gov.bc.ca/clp-cgi/logoff.cgi/%22")
            .WithEnvironment("RENTAL_LISTING_REPORT_MAX_SIZE", "10")
            .WithEnvironment("BUISINESS_LICENCE_MAX_SIZE", "10")
            .WithEnvironment("ADDRESS_SCORE", "90")
            .WithEnvironment("VALIDATE_REGISTRATION_MAX_SIZE", "10");

        if (api is not null)
        {
            frontend.WithEnvironment("API_URL", api.GetEndpoint("http"))
                .WaitFor(api);
        }
        else 
        {
            var frontendApiUrl = builder.Configuration["AppHost:Services:Frontend:ApiUrl"];
            if (string.IsNullOrWhiteSpace(frontendApiUrl))
            {
                throw new InvalidOperationException(
                    "AppHost:Services:Frontend:ApiUrl must be configured when AppHost:Services:Frontend:Enabled is true and AppHost:Services:Api:Enabled is false.");
            }

            frontend.WithEnvironment("API_URL", frontendApiUrl);
        }


        return frontend;

    }
}
