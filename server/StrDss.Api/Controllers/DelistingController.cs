using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.PlatformDtos;
using StrDss.Model.WarningReasonDtos;
using StrDss.Service;
using StrDss.Service.HttpClients;

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
            var errors = await _delistingService.CreateDelistingWarningAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [HttpPost("warnings/preview", Name = "GetDelistingWarningPreview")]
        [ApiAuthorize]
        public async Task<ActionResult<EmailPreview>> GetDelistingWarningPreview(DelistingWarningCreateDto dto)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = WarningReasonDto.WarningReasons.FirstOrDefault(x => x.WarningReasonId == dto.ReasonId)?.Reason;

            var (errors, preview) = await _delistingService.GetDelistingWarningPreviewAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return preview;
        }

        [HttpPost("requests", Name = "CreateDelistingRequest")]
        [ApiAuthorize]
        public async Task<ActionResult> CreateDelistingRequest(DelistingRequestCreateDto dto)
        {
            var errors = await _delistingService.CreateDelistingRequestAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [HttpPost("requests/preview", Name = "GetDelistingRequestPreview")]
        [ApiAuthorize]
        public async Task<ActionResult<EmailPreview>> GetDelistingRequestPreview(DelistingRequestCreateDto dto)
        {
            var (errors, preview) = await _delistingService.GetDelistingRequestPreviewAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return preview;
        }
    }
}
