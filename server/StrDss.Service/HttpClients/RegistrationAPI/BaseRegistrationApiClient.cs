using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrDss.Service.HttpClients
{
    public abstract class BaseRegistrationApiClient
    {
        private string _apiKey;

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }

        protected HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string requestUri)
        {
            var request = new HttpRequestMessage(method, requestUri);
            if (!string.IsNullOrEmpty(_apiKey))
            {
                request.Headers.Add("x-apikey", _apiKey);
            }
            return request;
        }
    }
}
