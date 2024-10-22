﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Service.HttpClients;
using System.Text;
using System.Text.Json;

namespace StrDss.Service
{
    public interface IEmailMessageService
    {
        Task<string> SendEmailAsync(EmailContent emailContent);
    }

    public class EmailMessageService : ServiceBase, IEmailMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailMessageRepository _emailRepo;
        private readonly IConfiguration _config;
        private readonly IChesTokenApi _chesTokenApi;

        public EmailMessageService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IEmailMessageRepository emailRepo, IConfiguration config, IChesTokenApi chesTokenApi, HttpClient httpClient, ILogger<StrDssLogger> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _emailRepo = emailRepo;
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
                emailContent.Body = $"<b>Kindly be advised that this is a test email.</b><br/><br/>{emailContent.Body}";
            }

            try
            {
                var token = await _chesTokenApi.GetTokenAsync();
                var chesUrl = _config.GetValue<string>("CHES_URL") ?? "";

                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token!.AccessToken}");
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token!.AccessToken}");
                }

                var jsonContent = emailContent.ToString();

                _logger.LogDebug($"CHES: {jsonContent}");

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{chesUrl}/api/v1/email", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Sent '{emailContent.Subject}' for {emailContent.Info} successfully");
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return ParseMsgIdFromJson(jsonResponse);
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
        private string ParseMsgIdFromJson(string jsonResponse)
        {
            using var document = JsonDocument.Parse(jsonResponse);

            var root = document.RootElement;

            if (!root.TryGetProperty("messages", out JsonElement messages) || !messages.EnumerateArray().Any())
            {
                return string.Empty; 
            }

            var msgId = messages.EnumerateArray().First().GetProperty("msgId").GetString();

            if (msgId == null)
            {
                return string.Empty; 
            }

            return msgId;
        }
    }
}
