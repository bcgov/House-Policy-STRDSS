using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StrDss.Common;

namespace StrDss.Service
{
    public interface IMemoryCacheService
    {
        Task<JsonWebKey> GetPublicKeyAsync(bool refreshCache);
    }

    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _config;
        private readonly ILogger<StrDssLogger> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24);

        public MemoryCacheService(IMemoryCache memoryCache, IConfiguration config, ILogger<StrDssLogger> logger)
        {
            _memoryCache = memoryCache;
            _config = config;
            _logger = logger;
        }

        public async Task<JsonWebKey> GetPublicKeyAsync(bool refreshCache)
        {
            var jwksUrl = _config.GetValue<string>("APS_GW_JWT_JWKS_URL");

            if (!refreshCache && _memoryCache.TryGetValue<JsonWebKey>(jwksUrl, out var cachedResult))
            {
                return cachedResult;
            }

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(jwksUrl);
                var jwks = JsonWebKeySet.Create(response);
                var publicKey = jwks.Keys[0];

                _memoryCache.Set(jwksUrl, publicKey, _cacheDuration);

                return publicKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
