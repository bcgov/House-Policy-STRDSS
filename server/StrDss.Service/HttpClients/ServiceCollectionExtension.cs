using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StrDss.Service.HttpClients
{
    public static class ServiceCollectionExtension
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IGcNotifyApi, GcNotifyApi>(client =>
            {
                var baseAddress = config.GetValue<string>("GcNotify:Url") ?? "";
                var apiKey = config.GetValue<string>("GcNotify:ApiKey") ?? "";

                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = new TimeSpan(0, 0, 10);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", $"ApiKey-v1 {apiKey}");
            });

            services.AddScoped<IApi, Api>();
        }
    }
}
