using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Model;
using System.Text.Json;

namespace StrDss.Service.HttpClients
{
    public interface IChesTokenApi
    {
        Task<TokenResponse?> GetTokenAsync();
    }
    public class ChesTokenApi : IChesTokenApi
    {
        public HttpClient _client { get; }
        public IApi _api { get; }
        public IConfiguration _config { get; }

        private ILogger<StrDssLogger> _logger;

        public ChesTokenApi(HttpClient client, IApi api, IConfiguration config, ILogger<StrDssLogger> logger)
        {
            _client = client;
            _api = api;
            _config = config;
            _logger = logger;
        }

        public async Task<TokenResponse?> GetTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });

            _logger.LogInformation($"[Egress] Calling CHES API to send email: {_client.BaseAddress}");

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TokenResponse>(responseContent);
        }
    }
}
