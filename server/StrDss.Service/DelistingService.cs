using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.PlatformDtos;
using StrDss.Service.HttpClients;
using System.Text;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IDelistingService
    {
        Task<Dictionary<string, List<string>>> ValidateDelistingWarning(DelistingWarningCreateDto dto, PlatformDto? platform, string? reason);
        Task<string> SendDelistingWarningAsync(DelistingWarningCreateDto dto, PlatformDto? platform);
        string FormatDelistingWarningEmailContent(DelistingWarningCreateDto dto, List<string> toList, bool contentOnly);
    }
    public class DelistingService : ServiceBase, IDelistingService
    {
        private IConfiguration _config;
        private IChesTokenApi _chesTokenApi;
        private ILogger<DelistingService> _logger;

        public DelistingService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration config, IChesTokenApi chesTokenApi, ILogger<DelistingService> logger)
            : base(currentUser, validator, unitOfWork, mapper)
        {
            _config = config;
            _chesTokenApi = chesTokenApi;
            _logger = logger;
        }
        public async Task<Dictionary<string, List<string>>> ValidateDelistingWarning(DelistingWarningCreateDto dto, PlatformDto? platform, string? reason)
        {
            await Task.CompletedTask;

            var errors = new Dictionary<string, List<string>>();
            RegexInfo regex;

            //Todo: Validate Platform ID
            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }


            if (dto.ListingUrl.IsEmpty())
            {
                errors.AddItem("listingUrl", "Listing URL is required");
            }
            else
            {
                regex = RegexDefs.GetRegexInfo(RegexDefs.Url);
                if (!Regex.IsMatch(dto.ListingUrl, regex.Regex))
                {
                    errors.AddItem("listingUrl", "Invalid URL");
                }
            }

            if (!dto.HostEmailSent)
            {
                if (dto.HostEmail.IsEmpty())
                {
                    errors.AddItem("hostEmail", $"Host email is required");
                }
                else
                {
                    regex = RegexDefs.GetRegexInfo(RegexDefs.Email);
                    if (!Regex.IsMatch(dto.HostEmail, regex.Regex))
                    {
                        errors.AddItem("hostEmail", $"Host email is invalid");
                    }
                }
            }

            if (reason == null)
            {
                errors.AddItem("reasonId", $"Reason ID ({dto.ReasonId}) does not exist.");
            }

            regex = RegexDefs.GetRegexInfo(RegexDefs.Email);
            foreach (var email in dto.CcList)
            {
                if (!Regex.IsMatch(email, regex.Regex))
                {
                    errors.AddItem("ccList", $"Email ({email}) is invalid");
                }
            }

            if (dto.LgContactEmail.IsEmpty())
            {
                errors.AddItem("lgContactEmail", $"Local government contact email is required");
            }
            else
            {
                regex = RegexDefs.GetRegexInfo(RegexDefs.Email);
                if (!Regex.IsMatch(dto.LgContactEmail, regex.Regex))
                {
                    errors.AddItem("lgContactEmail", $"Local government contact email is invalid");
                }
            }

            regex = RegexDefs.GetRegexInfo(RegexDefs.PhoneNumber);
            if (dto.LgContactPhone.IsNotEmpty() && !Regex.IsMatch(dto.LgContactPhone, regex.Regex))
            {
                errors.AddItem("lgContactPhone", $"Local government contact phone number is invalid");
            }

            regex = RegexDefs.GetRegexInfo(RegexDefs.Url);
            if (dto.StrBylawUrl.IsNotEmpty() && !Regex.IsMatch(dto.StrBylawUrl, regex.Regex))
            {
                errors.AddItem("strByLawUrl", "URL is required");
            }

            return errors;
        }

        public async Task<string> SendDelistingWarningAsync(DelistingWarningCreateDto dto, PlatformDto? platform)
        {
            try
            {
                var token = await _chesTokenApi.GetTokenAsync();
                var chesUrl = _config.GetValue<string>("CHES_URL") ?? "";

                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

                var toList = new List<string> { platform?.Email ?? "" };
                if (dto.HostEmail.IsNotEmpty())
                {
                    toList.Add(dto.HostEmail);
                }

                if (dto.SendCopy)
                {
                    dto.CcList.Add(_currentUser.EmailAddress);
                }


                var emailContent = new
                {
                    bcc = new string[] { },
                    bodyType = "text",
                    body = FormatDelistingWarningEmailContent(dto, toList, true),
                    cc = dto.CcList.ToArray(),
                    delayTS = 0,
                    encoding = "utf-8",
                    from = "no_reply@gov.bc.ca",
                    priority = "normal",
                    subject = "Notice of Takedown",
                    to = toList.ToArray(),
                };

                var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(emailContent);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{chesUrl}/api/v1/email", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Sent delisting warning to {emailContent.to} for {dto.ListingUrl}");
                }
                else
                {
                    _logger.LogError($"Failed to send delisting warning to {emailContent.to} for {dto.ListingUrl}. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                var error = $"$\"Exception raised when sending delisting warning to {{emailContent.to}} for {{dto.ListingUrl}}";
                _logger.LogError($"{error} - {ex}");
                return error;
            }

            return "";
        }

        public string FormatDelistingWarningEmailContent(DelistingWarningCreateDto dto, List<string> toList, bool contentOnly)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = WarningReasonDto.WarningReasons.FirstOrDefault(x => x.WarningReasonId == dto.ReasonId)?.Reason;
            var nl = Environment.NewLine;

            return (contentOnly ? "" : $@"To: {string.Join(";", toList)} {dto.HostEmail}{nl}cc: {string.Join(";", dto.CcList)}{nl}{nl}")
                 + $@"Dear Short-term Rental Host,{nl}"
                 + $@"{nl}Short-term rental accommodations in your community must obtain a short-term rental (STR) business licence from the local government in order to operate.{nl}{nl}Short-term rental accommodations are also regulated by the Province of B.C. Under the Short-term Rental Accommodations Act, short-term rental hosts in communities with a short-term rental business licence requirement must include a valid business licence number on any short-term rental listings advertised on an online platform. Short-term rental platforms are required to remove listings that do not meet this requirement if requested by the local government.{nl}{nl}The short-term rental listing below is not in compliance with an applicable local government business licence requirement for the following reason:"
                 + $@"{nl}{nl}{reason ?? ""}"
                 + $@"{nl}{nl}{dto.ListingUrl}"
                 + $@"{nl}{nl}Listing ID Number: {dto.ListingId}"
                 + $@"{nl}{nl}Unless you are able to demonstrate compliance with the business licence requirement, this listing may be removed from the short-term rental platform after 5 days. The local government has 90 days to submit a request to takedown the listing to the platform. For more information, contact:"
                 + $@"{nl}{nl}Email: {dto.LgContactEmail}"
                 + (dto.LgContactPhone.IsEmpty() ? "" : $@"{nl}Phone: {dto.LgContactPhone}")
                 + (dto.StrBylawUrl.IsEmpty() ? "" : $@"{nl}{nl}More information about our city's STR policies can be found at:{nl}{dto.StrBylawUrl}")
                 + (dto.Comment.IsEmpty() ? "" : $@"{nl}{nl}{dto.Comment}");
        }

    }
}
