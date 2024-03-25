using System.Text.Json;

namespace StrDss.Api
{
    public static class HttpResponseExtensions
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static async Task WriteJsonAsync<T>(this HttpResponse response, T obj, string contentType = null)
        {
            response.ContentType = contentType ?? "application/json";
            await response.WriteAsync(JsonSerializer.Serialize<T>(obj, _jsonOptions));
        }
    }
}
