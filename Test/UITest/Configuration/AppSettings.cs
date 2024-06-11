using System;
using Microsoft.Extensions.Configuration;

namespace Configuration
{
    public class AppSettings
    {
        private IConfiguration _Configuration;
        private string _Environment;
        private IConfigurationSection _UsersSection;
        private  IConfigurationSection _ServersSection;
        private IConfiguration _ConnectionStringSection;

        public AppSettings()
        {
            _Environment = Environment.GetEnvironmentVariable("ASPNETCORE_Environment") ?? "Local";
            _Configuration = new ConfigurationBuilder().AddJsonFile($"Appsettings.{_Environment}.json", optional: true, reloadOnChange: true).Build();
            _UsersSection = _Configuration.GetSection("Users");
            _ServersSection = _Configuration.GetSection("Servers");
            _ConnectionStringSection = _Configuration.GetSection("ConnectionStrings");
        }

        public string GetUser(string Key)
        {
            return (_UsersSection[Key]);
        }

        public string GetServer(string Key)
        {
            return (_ServersSection[Key]);
        }
        public string GetConnectionString(string key)
        {
            return (_ConnectionStringSection[key]);
        }
    }
}

