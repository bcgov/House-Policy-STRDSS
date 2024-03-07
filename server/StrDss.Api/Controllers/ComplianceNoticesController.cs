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
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComplianceNoticesController : BaseApiController
    {
        private IGcNotifyApi _gcNotifyApi;
        private ILogger<ComplianceNoticesController> _logger { get; }

        public ComplianceNoticesController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, IGcNotifyApi gcNotifyApi, ILogger<ComplianceNoticesController> logger)
            : base(currentUser, mapper, config)
        {
            _gcNotifyApi = gcNotifyApi;
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
            await Task.CompletedTask;

            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason;

            var validationResult = ValidateComplianceNotice(dto, platform, reason);
            if (!validationResult.IsValid)
            {
                return validationResult.Result;
            }

            var requestBody = new EmailRequestBody<ComplianceNoticeTemplate>
            {
                email_address = platform?.Email,
                template_id = _config.GetValue<string>("GcNotify:ComplianceNoticeTemplateId") ?? "",
                personalisation = new ComplianceNoticeTemplate
                {
                    listing_link = dto.ListingUrl,
                    comments = dto.Comment
                }
            };

            var gcNotifyId = "";
            try
            {
                var response = await _gcNotifyApi.SendNotificationTypedAsync(NotificationTypes.Eamil, requestBody);
                gcNotifyId = await _gcNotifyApi.GetGcNotifyIdAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GC Notify failed with an exception: {ex}");
                return Problem(detail: ex.Message, statusCode: 500);
            }

            if (gcNotifyId.IsEmpty())
            {
                _logger.LogError($"GC Notify failed");
                return Problem(detail: $"GC Notify failed", statusCode: 500);
            }

            _logger.LogInformation($"Sent compliance notice for {dto.ListingUrl}. GC Notify ID: {gcNotifyId}");

            return NoContent();
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

            return FormatComplianceNoticePreview(dto);
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

            if (!Regex.IsMatch(dto.HostEmail, regex.Regex))
            {
                errors.AddItem("hostEmail", $"Host email is invalid");
            }

            foreach (var email in dto.CcList)
            {
                if (!Regex.IsMatch(email, regex.Regex))
                {
                    errors.AddItem("hostEmail", $"Email ({email}) is invalid");
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

        private string FormatComplianceNoticePreview(ComplianceNoticeCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason;

            return $@"To: {platform?.Name}; {dto.HostEmail}{Environment.NewLine}cc: {string.Join(";", dto.CcList)}{Environment.NewLine}"
                 + $@"{Environment.NewLine}Dear Sir/Madam,{Environment.NewLine}"
                 + $@"{Environment.NewLine}The following listing has been found non-compliance to current STR regulations:{Environment.NewLine}"
                 + $@"{dto.ListingUrl}{Environment.NewLine}"
                 + $@"{Environment.NewLine}We have identified the following issue: {reason ?? ""}{Environment.NewLine}"
                 + $@"{Environment.NewLine}Please remedy this situation immediately to mitigate the need for any further actions.{Environment.NewLine}"
                 + (dto.Comment.IsEmpty() ? "" : $@"{Environment.NewLine}{dto.Comment}{Environment.NewLine}")
                 + $@"{Environment.NewLine}Kind Regards,{Environment.NewLine}";
        }
    }
}
