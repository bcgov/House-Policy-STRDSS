using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StrDss.Service.HttpClients
{
    public static class ServiceCollectionExtension
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IApi, Api>();

            services.AddHttpClient<IChesTokenApi, ChesTokenApi>(client =>
            {
                var baseAddress = config.GetValue<string>("CHES_TOKEN_URL") ?? "";
                var clientId = config.GetValue<string>("CHES_ID") ?? "";
                var secret = config.GetValue<string>("CHES_SECRET") ?? "";

                var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{secret}"));

                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = new TimeSpan(0, 0, 10);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Credentials}");
            });
        }
    }
}
