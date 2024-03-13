using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.PlatformDtos;
using StrDss.Service;
using StrDss.Service.HttpClients;
using System.Text;
using System.Text.RegularExpressions;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class DelistingController : BaseApiController
    {
        private IChesTokenApi _chesTokenApi;

        private ILogger<DelistingController> _logger { get; }
        private IDelistingService _delistingService { get; }

        public DelistingController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, IChesTokenApi chesTokenApi, ILogger<DelistingController> logger,
            IDelistingService delistingService)
            : base(currentUser, mapper, config)
        {
            _chesTokenApi = chesTokenApi;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _delistingService = delistingService;
        }

        #region Warning
        [HttpGet("reasons/dropdown", Name = "GetWarningReasonDrowdown")]
        [ApiAuthorize]
        public async Task<ActionResult<DropdownDto>> GetWarningReasonDrowdown()
        {
            await Task.CompletedTask;
            return Ok(WarningReasonDto.WarningReasons.Select(x => new DropdownDto { Id = x.WarningReasonId, Description = x.Reason }));
        }

        [HttpPost("warnings", Name = "CreateDelistingWarning")]
        [ApiAuthorize]
        public async Task<ActionResult> CreateDelistingWarning(DelistingWarningCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = WarningReasonDto.WarningReasons.FirstOrDefault(x => x.WarningReasonId == dto.ReasonId)?.Reason;

            var errors = await _delistingService.ValidateDelistingWarning(dto, platform, reason);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var error = await _delistingService.SendDelistingWarningAsync(dto, platform);

            if (!error.IsEmpty())
            {
                return Problem($"There were some errors in sending email.");
            }

            return NoContent();
        }

        [HttpPost("warnings/preview", Name = "GetDelistingWarningPreview")]
        [ApiAuthorize]
        public async Task<ActionResult<EmailPreview>> GetDelistingWarningPreview(DelistingWarningCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = WarningReasonDto.WarningReasons.FirstOrDefault(x => x.WarningReasonId == dto.ReasonId)?.Reason;

            var errors = await _delistingService.ValidateDelistingWarning(dto, platform, reason);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var toList = new List<string> { platform?.Email ?? "" };
            if (dto.HostEmail.IsNotEmpty())
            {
                toList.Add(dto.HostEmail);
            }

            return new EmailPreview { Content = _delistingService.FormatDelistingWarningEmailContent(dto, toList, false) };
        }

        #endregion

        #region Request
        [HttpPost("requests", Name = "CreateDelistingRequest")]
        [ApiAuthorize]
        public async Task<ActionResult> CreateDelistingRequest(DelistingRequestCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);

            var validationResult = ValidateDelistingRequest(dto, platform);
            if (!validationResult.IsValid)
            {
                return validationResult.Result;
            }

            var error = await SendDelistingRequestAsync(dto, platform);

            if (!error.IsEmpty())
            {
                return Problem($"There were some errors in sending email.");
            }

            return NoContent();
        }

        private async Task<string> SendDelistingRequestAsync(DelistingRequestCreateDto dto, PlatformDto? platform)
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
                    body = FormatDelistingRequestEmailContent(dto, true),
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
                    _logger.LogInformation($"Sent delisting request to {emailContent.to} for {dto.ListingUrl}");
                }
                else
                {
                    _logger.LogError($"Failed to send email. Status code: {response.StatusCode}");
                }

            }
            catch (Exception ex)
            {
                var error = $"Sending email failed with an exception";
                _logger.LogError($"{error} - {ex}");
                return error;
            }

            _logger.LogInformation($"Sent compliance notice.");
            return "";
        }

        [HttpPost("requests/preview", Name = "GetDelistingRequestPreview")]
        [ApiAuthorize]
        public async Task<ActionResult<EmailPreview>> GetDelistingRequestPreview(DelistingRequestCreateDto dto)
        {
            await Task.CompletedTask;

            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);

            var validationResult = ValidateDelistingRequest(dto, platform);
            if (!validationResult.IsValid)
            {
                return validationResult.Result;
            }

            return new EmailPreview { Content = FormatDelistingRequestEmailContent(dto, false) };
        }

        private (bool IsValid, ActionResult Result) ValidateDelistingRequest(DelistingRequestCreateDto dto, PlatformDto? platform)
        {
            var errors = new Dictionary<string, List<string>>();

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }

            var regex = RegexDefs.GetRegexInfo(RegexDefs.Email);

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

            if (dto.SendCopy)
            {
                dto.CcList.Add(_currentUser.EmailAddress);
            }

            return (true, null);
        }

        private string FormatDelistingRequestEmailContent(DelistingRequestCreateDto dto, bool contentOnly)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);

            return (contentOnly ? "" : $@"To: {platform?.Name}{Environment.NewLine}cc: {string.Join(";", dto.CcList)}{Environment.NewLine}{Environment.NewLine}")
                 + $@"Dear Sir/Madam,{Environment.NewLine}"
                 + $@"{Environment.NewLine}The following listing has been found non-compliance to current STR regulations:{Environment.NewLine}"
                 + $@"{dto.ListingUrl}{Environment.NewLine}"
                 + $@"{Environment.NewLine}Please remove this listing from your platform immediately to mitigate the need for any further actions.{Environment.NewLine}"
                 + (dto.Comment.IsEmpty() ? "" : $@"{Environment.NewLine}{dto.Comment}{Environment.NewLine}")
                 + $@"{Environment.NewLine}Kind Regards,{Environment.NewLine}";
        }
        #endregion
    }
}
