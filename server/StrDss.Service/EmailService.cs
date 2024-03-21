using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        Task SendEmailAsync(EmailContent emailContent);
    }

    public class EmailService : ServiceBase, IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IChesTokenApi _chesTokenApi;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IConfiguration config, IChesTokenApi chesTokenApi, HttpClient httpClient, ILogger<EmailService> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor)
        {
            _config = config;
            _httpClient = httpClient;
            _chesTokenApi = chesTokenApi;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailContent emailContent)
        {
            var env = _config.GetValue<string>("ENV_NAME") ?? "dev";

            if (env.ToLowerInvariant() != "prod") 
            {
                var nl = Environment.NewLine;
                emailContent.Subject = $"[{env.ToUpperInvariant()}] {emailContent.Subject}";
                emailContent.Body = $"<b>Kindly be advised that this is a test email.</b><br/><br/>{emailContent.Body}";
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
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                var error = $"Exception raised when sending '{emailContent.Subject}' for {emailContent.Info}.";
                _logger.LogError($"{error} - {ex}");
                throw new Exception(error);
            }
        }
    }

}
