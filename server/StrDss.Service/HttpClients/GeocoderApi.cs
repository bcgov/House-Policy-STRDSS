using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace StrDss.Service.HttpClients
{
    public interface IGeocoderApi
    {
        //Task GetAddressAsync(DssPhysicalAddress address);
        //Task GetAddressAsync(string address);
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

        //public async Task GetAddressAsync(DssPhysicalAddress address)
        //{
        //    _logger.LogInformation($"[Egress] Calling Geocoder API: {_client.BaseAddress}");

        //    var response = await _client.GetStringAsync($"addresses.geojson?addressString={address.OriginalAddressTxt}");

        //    using JsonDocument doc = JsonDocument.Parse(response);

        //    var root = doc.RootElement;

        //    if (root.ValueKind == JsonValueKind.Object)
        //    {
        //        if (root.TryGetProperty("features", out var features))
        //        {
        //            var feature = features[0];
        //            var geometry = feature.GetProperty("geometry");
        //            var coordinates = geometry.GetProperty("coordinates");

        //            var lon = coordinates[0].GetDouble();
        //            var lat = coordinates[1].GetDouble();

        //            address.LocationGeometry = 

        //            var properties = feature.GetProperty("properties");
        //            var foundFaults = false;

        //            if (properties.TryGetProperty("faults", out var faults))
        //            {
        //                if (faults.EnumerateArray().Count() > 0)
        //                {
        //                    foundFaults = true;
        //                }

        //                //foreach (var fault in faults.EnumerateArray())
        //                //{
        //                //    var value = fault.GetProperty("value").GetInt32();
        //                //    var element = fault.GetProperty("element").GetString();
        //                //    var faultType = fault.GetProperty("fault").GetString();
        //                //    var penalty = fault.GetProperty("penalty").GetInt32();

        //                //    Console.WriteLine($"Fault: {faultType}, Element: {element}, Value: {value}, Penalty: {penalty}");
        //                //}
        //            }

        //            var score = properties.GetProperty("score").GetInt32();
        //            var matchPrecision = properties.GetProperty("matchPrecision").GetString();

        //            if (score > 90 && matchPrecision == "CIVIC_NUMBER" || matchPrecision == "BLOCK")
        //            {
        //                foundFaults = false;
        //            }
        //        }
        //    }
        //}

        //public async Task GetAddressAsync(string address)
        //{
        //    try
        //    {
        //        var response = await _client.GetStringAsync($"addresses.geojson?addressString={address}");
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
    }
}
