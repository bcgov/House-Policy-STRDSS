﻿using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StrDss.Service.HttpClients
{
    public static class ServiceCollectionExtension
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient();

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

            services.AddHttpClient<IGeocoderApi, GeocoderApi>(client =>
            {
                var baseAddress = config.GetValue<string>("GEOCODER_URL") ?? "";
                var apiKey = config.GetValue<string>("GEOCODER_API_KEY") ?? "";

                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = new TimeSpan(0, 0, 10);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("apiKey", $"{apiKey}");
            });

            services.AddHttpClient<IRegistrationApiClient, RegistrationApiClient>(client =>
            {
                var baseAddress = config.GetValue<string>("REGISTRATION_API_URL") ?? "";
                var apiKey = config.GetValue<string>("REGISTRATION_API_KEY") ?? "";

                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = new TimeSpan(0, 0, 10);
                client.DefaultRequestHeaders.Add("x-apikey", apiKey);
            });

        }
    }
}
