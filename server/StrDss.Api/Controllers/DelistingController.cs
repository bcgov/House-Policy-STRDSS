using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
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

        private ILogger<StrDssLogger> _logger { get; }
        private IDelistingService _delistingService { get; }

        private IEmailMessageService _emailService;

        public DelistingController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, IChesTokenApi chesTokenApi, ILogger<StrDssLogger> logger,
            IDelistingService delistingService, IEmailMessageService emailService)
            : base(currentUser, mapper, config, logger)
        {
            _chesTokenApi = chesTokenApi;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _delistingService = delistingService;
            _emailService = emailService;
        }

        [ApiAuthorize]
        [HttpGet("reasons/dropdown", Name = "GetReasonDropdown")]
        public async Task<ActionResult<List<DropdownNumDto>>> GetReasonDropdown()
        {
            return await _emailService.GetMessageReasons(EmailMessageTypes.NoticeOfTakedown);
        }

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("warnings", Name = "CreateTakedownNotice")]
        public async Task<ActionResult> CreateTakedownNotice(TakedownNoticeCreateDto dto)
        {
            var errors = await _delistingService.CreateTakedownNoticeAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("warnings/preview", Name = "GetTakedownNoticePreview")]
        public async Task<ActionResult<EmailPreview>> GetTakedownNoticePreview(TakedownNoticeCreateDto dto)
        {
            var (errors, preview) = await _delistingService.GetTakedownNoticePreviewAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return preview;
        }

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("requests", Name = "CreateTakedownRequest")]
        public async Task<ActionResult> CreateTakedownRequest(TakedownRequestCreateDto dto)
        {
            var errors = await _delistingService.CreateTakedownRequestAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("requests/preview", Name = "GetTakedownRequestPreview")]
        public async Task<ActionResult<EmailPreview>> GetTakedownRequestPreview(TakedownRequestCreateDto dto)
        {
            var (errors, preview) = await _delistingService.GetTakedownRequestPreviewAsync(dto);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return preview;
        }
    }
}
