using Microsoft.Extensions.Configuration;
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

        public ChesTokenApi(HttpClient client, IApi api, IConfiguration config)
        {
            _client = client;
            _api = api;
            _config = config;
        }

        public async Task<TokenResponse?> GetTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TokenResponse>(responseContent);
        }
    }
}
