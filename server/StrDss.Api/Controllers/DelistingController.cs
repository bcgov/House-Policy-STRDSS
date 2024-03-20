using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.LocalGovernmentDtos;
using StrDss.Model.PlatformDtos;
using StrDss.Model.WarningReasonDtos;
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

            dto.ToList.Add(platform?.Email ?? "");
            if (dto.HostEmail.IsNotEmpty())
            {
                dto.ToList.Add(dto.HostEmail);
            }

            return new EmailPreview { Content = _delistingService.FormatDelistingWarningEmailContent(dto, false).HtmlToPlainText() };
        }

        [HttpPost("requests", Name = "CreateDelistingRequest")]
        [ApiAuthorize]
        public async Task<ActionResult> CreateDelistingRequest(DelistingRequestCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var lg = LocalGovernmentDto.localGovernments.FirstOrDefault(x => x.LocalGovernmentId == dto.LgId);

            var errors = await _delistingService.ValidateDelistingRequest(dto, platform, lg);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var error = await _delistingService.SendDelistingRequestAsync(dto, platform);

            if (!error.IsEmpty())
            {
                return Problem($"There were some errors in sending email.");
            }

            return NoContent();
        }

        [HttpPost("requests/preview", Name = "GetDelistingRequestPreview")]
        [ApiAuthorize]
        public async Task<ActionResult<EmailPreview>> GetDelistingRequestPreview(DelistingRequestCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var lg = LocalGovernmentDto.localGovernments.FirstOrDefault(x => x.LocalGovernmentId == dto.LgId);

            var errors = await _delistingService.ValidateDelistingRequest(dto, platform, lg);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            dto.ToList.Add(platform?.Email ?? "");

            return new EmailPreview { Content = _delistingService.FormatDelistingRequestEmailContent(dto, false).HtmlToPlainText() };
        }
    }
}
