using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using System.Text.Json;
using StrDss.Model;
using NetTopologySuite.Geometries;
using System.Text.Json.Serialization;

namespace StrDss.Service.HttpClients
{
    public interface IGeocoderApi
    {
        Task<string> GetAddressAsync(DssPhysicalAddress addressText);
        Task<List<AddressDto>> GetAddressCandidatesAsync(string addressText, int maxResults);
    }
    public class GeocoderApi : IGeocoderApi
    {
        public HttpClient _client { get; }
        public IConfiguration _config { get; }
        private ILogger<StrDssLogger> _logger;

        public GeocoderApi(HttpClient client, IConfiguration config, ILogger<StrDssLogger> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;
        }

        public async Task<string> GetAddressAsync(DssPhysicalAddress address)
        {
            _logger.LogInformation($"[Egress] Calling Geocoder API: {_client.BaseAddress}");

            try
            {
                var response = await _client.GetStringAsync($"addresses.geojson?addressString={SanitizeAddress(address.OriginalAddressTxt)}");

                address.MatchResultJson = response;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
                };

                var root = JsonSerializer.Deserialize<GeocoderResponse>(response, options);

                if (root?.Features != null && root.Features.Length > 0)
                {
                    var feature = root.Features[0];
                    var properties = feature.Properties;

                    address.MatchAddressTxt = properties.FullAddress;
                    address.MatchScoreAmt = (short)properties.Score;
                    address.UnitNo = properties.UnitNumber;
                    address.CivicNo = properties.CivicNumber.ToString();
                    address.StreetNm = properties.StreetName;
                    address.StreetTypeDsc = properties.StreetType;
                    address.StreetDirectionDsc = properties.StreetDirection;
                    address.LocalityNm = properties.LocalityName;
                    address.LocalityTypeDsc = properties.LocalityType;
                    address.ProvinceCd = properties.ProvinceCode;
                    address.SiteNo = properties.SiteId;
                    address.BlockNo = properties.BlockId;
                    address.LocationGeometry = new Point(feature.Geometry.Coordinates[0], feature.Geometry.Coordinates[1]) { SRID = 4326 };
                    address.IsSystemProcessing = true;
                    address.IsExempt = false;
                    address.IsMatchCorrected = false;
                    address.IsMatchVerified = false;
                }

                return "";
            }
            catch (Exception ex)
            {
                address.MatchScoreAmt = 0;
                _logger.LogError(ex.ToString());
                return ex.Message;
            }

        }

        public async Task<List<AddressDto>> GetAddressCandidatesAsync(string addressText, int maxResults)
        {
            _logger.LogInformation($"[Egress] Calling Geocoder API: {_client.BaseAddress}");

            var addresses = new List<AddressDto>();

            var response = await _client.GetStringAsync($"addresses.geojson?addressString={SanitizeAddress(addressText)}&maxResults={maxResults}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            var root = JsonSerializer.Deserialize<GeocoderResponse>(response, options);

            if (root?.Features != null && root.Features.Length > 0)
            {
                foreach (var feature in root.Features)
                {
                    addresses.Add(new AddressDto
                    {
                        LocationGeometry = new Point(feature.Geometry.Coordinates[0], feature.Geometry.Coordinates[1]) { SRID = 4326 },
                        Address = feature.Properties.FullAddress,
                        Score = feature.Properties.Score,
                    });
                }
            }

            return addresses;
        }

        private string SanitizeAddress(string address)
        {
            var toRemove = ", Canada";

            address = address.Replace("#", " ");

            if (address.EndsWith(toRemove))
            {
                address = address[..^toRemove.Length];
            }

            return address;
        }
    }
}
