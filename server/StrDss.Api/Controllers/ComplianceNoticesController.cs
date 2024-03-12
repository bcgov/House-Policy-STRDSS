using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.ComplianceNoticeDtos;
using StrDss.Model.ComplianceNoticeReasonDtos;
using StrDss.Model.GcNotifyTemplates;
using StrDss.Model.PlatformDtos;
using StrDss.Service.HttpClients;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComplianceNoticesController : BaseApiController
    {
        private IGcNotifyApi _gcNotifyApi;
        private IChesTokenApi _chesTokenApi;

        private ILogger<ComplianceNoticesController> _logger { get; }

        public ComplianceNoticesController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, IGcNotifyApi gcNotifyApi, IChesTokenApi chesTokenApi, ILogger<ComplianceNoticesController> logger)
            : base(currentUser, mapper, config)
        {
            _gcNotifyApi = gcNotifyApi;
            _chesTokenApi = chesTokenApi;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("reasons/dropdown", Name = "GetComplianceNoticeReasonDrowdown")]
        [ApiAuthorize]
        public async Task<ActionResult<DropdownDto>> GetComplianceNoticeReasonDrowdown()
        {
            await Task.CompletedTask;
            return Ok(ComplianceNoticeReasonDto.ComplianceNoticeReasons.Select(x => new DropdownDto { Id = x.ComplianceNoticeReasonId, Description = x.Reason }));
        }

        [HttpPost("", Name = "CreateComplianceNotice")]
        [ApiAuthorize]
        public async Task<ActionResult> CreateComplianceNotice(ComplianceNoticeCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason;

            var validationResult = ValidateComplianceNotice(dto, platform, reason);
            if (!validationResult.IsValid)
            {
                return validationResult.Result;
            }

            var error = await SendComplianceNoticeAsync(dto, platform);

            if (!error.IsEmpty())
            {
                return Problem($"There were some errors in sending email.");
            }

            return NoContent();
        }

        private async Task<string> SendComplianceNoticeAsync(ComplianceNoticeCreateDto dto, PlatformDto? platform)
        {
            try
            {
                var token = await _chesTokenApi.GetTokenAsync();
                var chesUrl = _config.GetValue<string>("CHES_URL") ?? "";

                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

                var emailContent = new
                {
                    bcc = new string[] { },
                    bodyType = "text",
                    body = FormatComplianceNoticeEmailContent(dto, true),
                    cc = dto.CcList.ToArray(),
                    delayTS = 0,
                    encoding = "utf-8",
                    from = "no_reply@gov.bc.ca",
                    priority = "normal",
                    subject = "Compliance Notice",
                    to = new string[] { platform?.Email ?? "" },
                };

                var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(emailContent);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{chesUrl}/api/v1/email", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Sent compliance notice to {emailContent.to} for {dto.ListingUrl}");
                }
                else
                {
                    _logger.LogError($"Failed to send email. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                var error = $"Failed to send email with an exception";
                _logger.LogError($"{error} - {ex}");
                return error;
            }

            return "";
        }

        private async Task<(string success, string error)> SendComplianceNoticeByGcNotifyAsync(ComplianceNoticeCreateDto dto, List<string> emails)
        {
            var requestBody = new EmailRequestBody<ComplianceNoticeTemplate>
            {
                template_id = _config.GetValue<string>("GcNotify:ComplianceNoticeTemplateId") ?? "",
                personalisation = new ComplianceNoticeTemplate
                {
                    listing_link = dto.ListingUrl,
                    comments = dto.Comment
                }
            };

            var errors = new List<string>();
            var success = new List<string>();

            foreach(var email in emails)
            {
                var gcNotifyId = "";
                try
                {
                    requestBody.email_address = email;
                    var response = await _gcNotifyApi.SendNotificationTypedAsync(NotificationTypes.Eamil, requestBody);
                    gcNotifyId = await _gcNotifyApi.GetGcNotifyIdAsync(response);
                    success.Add(email);
                }
                catch (Exception ex)
                {
                    var message = $"GC Notify failed with an exception: {ex}";                    
                    _logger.LogError(message);
                }

                if (gcNotifyId.IsEmpty())
                {
                    var message = $"GC Notify failed to send email to {email}";
                    _logger.LogError(message);
                    errors.Add(email);
                }                

                _logger.LogInformation($"Sent compliance notice for {dto.ListingUrl}. GC Notify ID: {gcNotifyId}, Recipient: {email}");
            }

            return (string.Join(", ", success), string.Join(", ", errors));
        }

        [HttpPost("preview", Name = "GetComplianceNoticePreview")]
        [ApiAuthorize]
        public async Task<ActionResult<string>> GetComplianceNoticePreview(ComplianceNoticeCreateDto dto)
        {
            await Task.CompletedTask;

            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason;

            var validationResult = ValidateComplianceNotice(dto, platform, reason);
            if (!validationResult.IsValid)
            {
                return validationResult.Result;
            }

            return FormatComplianceNoticeEmailContent(dto, false);
        }

        private (bool IsValid, ActionResult Result) ValidateComplianceNotice(ComplianceNoticeCreateDto dto, PlatformDto? platform, string? reason)
        {
            var errors = new Dictionary<string, List<string>>();

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }

            if (reason == null)
            {
                errors.AddItem("reasonId", $"Reason ID ({dto.ReasonId}) does not exist.");
            }

            var regex = RegexDefs.GetRegexInfo(RegexDefs.Email);

            if (dto.HostEmail.IsNotEmpty() && !Regex.IsMatch(dto.HostEmail, regex.Regex))
            {
                errors.AddItem("hostEmail", $"Host email is invalid");
            }

            foreach (var email in dto.CcList)
            {
                if (!Regex.IsMatch(email, regex.Regex))
                {
                    errors.AddItem("ccList", $"Email ({email}) is invalid");
                }
            }

            if (errors.Count > 0)
            {
                return (false, ValidationUtils.GetValidationErrorResult(errors, ControllerContext));
            }

            if (dto.HostEmail.IsNotEmpty())
            {
                dto.CcList.Add(dto.HostEmail);
            }

            if (dto.SendCopy)
            {
                dto.CcList.Add(_currentUser.EmailAddress);
            }

            return (true, null);
        }

        private string FormatComplianceNoticeEmailContent(ComplianceNoticeCreateDto dto, bool contentOnly)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason;

            return (contentOnly ? "" : $@"To: {platform?.Name}; {dto.HostEmail}{Environment.NewLine}cc: {string.Join(";", dto.CcList)}{Environment.NewLine}{Environment.NewLine}")
                 + $@"Dear Sir/Madam,{Environment.NewLine}"
                 + $@"{Environment.NewLine}The following listing has been found non-compliance to current STR regulations:{Environment.NewLine}"
                 + $@"{dto.ListingUrl}{Environment.NewLine}"
                 + $@"{Environment.NewLine}We have identified the following issue: {reason ?? ""}{Environment.NewLine}"
                 + $@"{Environment.NewLine}Please remedy this situation immediately to mitigate the need for any further actions.{Environment.NewLine}"
                 + (dto.Comment.IsEmpty() ? "" : $@"{Environment.NewLine}{dto.Comment}{Environment.NewLine}")
                 + $@"{Environment.NewLine}Kind Regards,{Environment.NewLine}";
        }
    }
}
