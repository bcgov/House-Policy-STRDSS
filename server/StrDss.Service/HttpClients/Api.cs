using System.Text;

namespace StrDss.Service.HttpClients
{
    public interface IApi
    {
        Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request);
        Task<HttpResponseMessage> SendAsyncWithRetry(HttpClient client, HttpRequestMessage request);
        HttpResponseMessage SendWithRetry(HttpClient client, HttpRequestMessage request);

    }
    public class Api : IApi
    {
        const int maxAttempt = 5;

        public async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request)
        {
            var response = await client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}, Detail: {content}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> SendAsyncWithRetry(HttpClient client, HttpRequestMessage request)
        {
            var response = await client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                for (var attempt = 2; attempt <= maxAttempt; attempt++)
                {
                    await Task.Delay(100 * attempt);

                    response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }
                    else if (attempt == maxAttempt)
                    {
                        string message = "";

                        if (response.Content != null)
                        {
                            var bytes = await response.Content.ReadAsByteArrayAsync();
                            message = Encoding.UTF8.GetString(bytes);
                        }

                        throw new Exception($"Status Code: {response.StatusCode}" + Environment.NewLine + message);
                    }
                }
            }

            return response;
        }
        public HttpResponseMessage SendWithRetry(HttpClient client, HttpRequestMessage request)
        {
            var response = client.Send(request);

            //var content = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                for (var attempt = 2; attempt <= maxAttempt; attempt++)
                {
                    Task.Delay(100 * attempt);

                    response = client.Send(request);

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }
                    else if (attempt == maxAttempt)
                    {
                        string message = "";

                        if (response.Content != null)
                        {
                            message = response.Content.ReadAsStringAsync().Result;
                            //message = Encoding.UTF8.GetBytes(bytes);
                        }

                        throw new Exception($"Status Code: {response.StatusCode}" + Environment.NewLine + message);
                    }
                }
            }

            return response;
        }
    }
}
