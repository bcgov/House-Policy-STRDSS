using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StrDss.Service.HttpClients
{
    public enum NotificationTypes { Eamil, Sms };

    public interface IGcNotifyApi
    {
        Task<HttpResponseMessage> SendNotificationAsync(NotificationTypes type, Dictionary<string, object> data);
        Task<HttpResponseMessage> SendNotificationTypedAsync<T>(NotificationTypes type, T data);
        Task<HttpResponseMessage> GetNotificationStatusAync(string id);
        HttpResponseMessage GetNotificationStatus(string id);
        Task<string> GetGcNotifyIdAsync(HttpResponseMessage? message);
        string GetGcNotifJsonValue(string json, string memberName);
    }
    public class GcNotifyApi : IGcNotifyApi
    {
        public HttpClient _client { get; }
        public IApi _api { get; }
        public IConfiguration _config { get; }

        public GcNotifyApi(HttpClient client, IApi api, IConfiguration config)
        {
            _client = client;
            _api = api;
            _config = config;
        }

        public async Task<HttpResponseMessage> SendNotificationAsync(NotificationTypes type, Dictionary<string, object> data)
        {
            var body = JsonSerializer.Serialize(data);

            var path = type == NotificationTypes.Eamil ?
                _config.GetValue<string>("GcNotify:EmailPath") ?? "" :
                _config.GetValue<string>("GcNotify:SmsPath") ?? "";

            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await _api.SendAsync(_client, request);
        }

        public async Task<HttpResponseMessage> SendNotificationTypedAsync<T>(NotificationTypes type, T data)
        {
            var body = JsonSerializer.Serialize(data);

            var path = type == NotificationTypes.Eamil ?
                _config.GetValue<string>("GcNotify:EmailPath") ?? "" :
                _config.GetValue<string>("GcNotify:SmsPath") ?? "";

            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            try
            {
                return await _api.SendAsync(_client, request);
            }
            catch
            {
                throw;
            }
        }
        public async Task<HttpResponseMessage> GetNotificationStatusAync(string id)
        {
            var path = _config.GetValue<string>("GcNotify:NoticationPath") ?? "";
            path = $"{path}/{id}";

            var request = new HttpRequestMessage(HttpMethod.Get, path);
            return await _api.SendAsync(_client, request);
        }
        public HttpResponseMessage GetNotificationStatus(string id)
        {
            var path = _config.GetValue<string>("GcNotify:NoticationPath") ?? "";
            path = $"{path}/{id}";

            var request = new HttpRequestMessage(HttpMethod.Get, path);
            return _api.SendWithRetry(_client, request);
        }
        // GC Notify template requires at least one space to apply the template styles.
        public static string SetSpaceIfBlank(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return " ";
            }

            return value;
        }
        public static string NormalizePhoneNumber(string phoneNumber)
        {
            string norm = Regex.Replace(phoneNumber, @"[^0-9]", "");
            return norm;
        }

        public async Task<string> GetGcNotifyIdAsync(HttpResponseMessage? message)
        {
            if (message == null) return "";
            var content = await message.Content.ReadAsStringAsync();
            return GetGcNotifJsonValue(content, "id");
        }

        public string GetGcNotifJsonValue(string json, string memberName)
        {
            if (string.IsNullOrEmpty(json))
            {
                return "";
            }

            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty(memberName, out JsonElement statusProperty))
                {
                    return statusProperty.GetString() ?? "";
                }
            }
            catch (JsonException)
            {
                return "";
            }

            return "";
        }
    }


    public class EmailRequestBody<T>
    {
        public string? email_address { get; set; }
        public string? template_id { get; set; }
        public T? personalisation { get; set; }
    }
    public class EmailFile
    {
        public string file { get; set; } = "";
        public string filename { get; set; } = "";
    }

    public class EmailAttachment : EmailFile
    {
        public string sending_method { get; set; } = "attach";
    }

    public class SmsRequestBody<T>
    {
        public string? phone_number { get; set; }
        public string? template_id { get; set; }
        public T? personalisation { get; set; }
    }
}
