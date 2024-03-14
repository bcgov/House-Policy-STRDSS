using System;
using Microsoft.Extensions.Configuration;

namespace Configuration
{
    public class AppSettings
    {
        IConfiguration _Configuration;
        string environment;

        public AppSettings()
        {
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
            _Configuration = new ConfigurationBuilder().AddJsonFile($"Appsettings.{environment}.json", optional: true, reloadOnChange: true).Build();
        }

        public string GetValue(string Variable)
        {
            string variableValue = _Configuration[Variable];

            if (variableValue != null)
            {
                return (variableValue);
            }
            else
            {
                return (null);
            }
        }

    }

}

