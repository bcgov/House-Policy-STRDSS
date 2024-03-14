using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Data;
using StrDss.Model;
using StrDss.Service.HttpClients;
using System.Text;

namespace StrDss.Service
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(EmailContent emailContent);
    }

    public class EmailService : ServiceBase, IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IChesTokenApi _chesTokenApi;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration config, IChesTokenApi chesTokenApi, HttpClient httpClient, ILogger<EmailService> logger)
            : base(currentUser, validator, unitOfWork, mapper)
        {
            _config = config;
            _httpClient = httpClient;
            _chesTokenApi = chesTokenApi;
            _logger = logger;
        }

        public async Task<string> SendEmailAsync(EmailContent emailContent)
        {
            var env = _config.GetValue<string>("ENV_NAME") ?? "dev";

            if (env.ToLowerInvariant() != "prod") 
            {
                var nl = Environment.NewLine;
                emailContent.Subject = $"[{env.ToUpperInvariant()}] {emailContent.Subject}";
                emailContent.Body = $"Kindly be advised that this is a test email.{nl}{nl}{emailContent.Body}";
            }

            try
            {
                var token = await _chesTokenApi.GetTokenAsync();
                var chesUrl = _config.GetValue<string>("CHES_URL") ?? "";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

                var jsonContent = emailContent.ToString();
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{chesUrl}/api/v1/email", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Sent '{emailContent.Subject}' for {emailContent.Info} successfully");
                }
                else
                {
                    var error = $"Failed to send '{emailContent.Subject}' for {emailContent.Info}. Status code: {response.StatusCode}";
                    _logger.LogError(error);
                    return error;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception raised when sending '{emailContent.Subject}' for {emailContent.Info} - {ex}");
                return ex.Message;
            }

            return "";
        }
    }

}
