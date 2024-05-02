using System;
using Microsoft.Extensions.Configuration;

namespace Configuration
{
    public class AppSettings
    {
        IConfiguration configuration;
        string environment;
        IConfigurationSection usersSection;
        IConfigurationSection serversSection;

        public AppSettings()
        {
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
            configuration = new ConfigurationBuilder().AddJsonFile($"Appsettings.{environment}.json", optional: true, reloadOnChange: true).Build();
            usersSection = configuration.GetSection("Users");
            serversSection = configuration.GetSection("Servers");
        }

        public string GetUser(string Key)
        {
            return (usersSection[Key]);
        }

        public string GetServer(string Key)
        {
            return (serversSection[Key]);
        }

    }
}

