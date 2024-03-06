using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.ComplianceNoticeDtos;
using StrDss.Model.ComplianceNoticeReasonDtos;
using StrDss.Model.PlatformDtos;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComplianceNoticesController : BaseApiController
    {
        private ILogger<ComplianceNoticesController> _logger { get; }

        public ComplianceNoticesController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<ComplianceNoticesController> logger)
            : base(currentUser, mapper, config)
        {
            _logger = logger;
        }

        [HttpGet("reasons/dropdown", Name = "GetComplianceNoticeReasonDrowdown")]
        [ApiAuthorize]
        public ActionResult<DropdownDto> GetComplianceNoticeReasonDrowdown()
        {
            return Ok(ComplianceNoticeReasonDto.ComplianceNoticeReasons.Select(x => new DropdownDto { Id = x.ComplianceNoticeReasonId, Description = x.Reason }));
        }


        [HttpPost("", Name = "CreateComplianceNotice")]
        [ApiAuthorize]
        public ActionResult CreateComplianceNotice(ComplianceNoticeCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }

            var reason = ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason;

            if (reason == null)
            {
                errors.AddItem("reasonId", $"Reason ID ({dto.ReasonId}) does not exist.");
            }

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            _logger.LogInformation($"Compliance Notice for {dto.ListingUrl}");

            return NoContent();
        }

        [HttpPost("preview", Name = "GetComplianceNoticePreview")]
        [ApiAuthorize]
        public ActionResult<string> GetComplianceNoticePreview(ComplianceNoticeCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }

            var reason = ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason;

            if (reason == null)
            {
                errors.AddItem("reasonId", $"Reason ID ({dto.ReasonId}) does not exist.");
            }

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return 
                $@"
                To: {platform?.Name}; {dto.HostEmail}
                cc: {string.Join(";", dto.CcList)}

                Dear Sir/Madam,

                The following listing has been found non-compliance to current STR regulations:

                {dto.ListingUrl}

                We have identified the following issue: {ComplianceNoticeReasonDto.ComplianceNoticeReasons.FirstOrDefault(x => x.ComplianceNoticeReasonId == dto.ReasonId)?.Reason ?? ""}
                
                Please remedy this situation immediately to mitigate the need for any further actions.

                {dto.Comment}
                
                Kind Regards,   
                ";           
        }
    }
}
