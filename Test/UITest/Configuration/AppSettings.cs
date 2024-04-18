using System;
using Microsoft.Extensions.Configuration;

namespace Configuration
{
    public class AppSettings
    {
        IConfiguration configuration;
        string environment;
        IConfigurationSection usersSection;

        public AppSettings()
        {
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
            configuration = new ConfigurationBuilder().AddJsonFile($"Appsettings.{environment}.json", optional: true, reloadOnChange: true).Build();
            usersSection = configuration.GetSection("Users");
        }

        public string GetValue(string Key)
        {
            return (usersSection[Key]);
        }

    }
}

