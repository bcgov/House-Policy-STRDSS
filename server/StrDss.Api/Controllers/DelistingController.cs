using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Api.Models;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Service;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class DelistingController : BaseApiController
    {
        private IDelistingService _delistingService { get; }
        private IEmailMessageService _emailService;

        public DelistingController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IDelistingService delistingService, IEmailMessageService emailService)
            : base(currentUser, mapper, config, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _delistingService = delistingService;
            _emailService = emailService;
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
        [HttpPost("bulkwarnings", Name = "CreateTakedownNoticesFromListing")]
        public async Task<ActionResult> CreateTakedownNoticesFromListing(TakedownNoticesFromListingDto[] notices)
        {
            var errors = await _delistingService.CreateTakedownNoticesFromListingAsync(notices);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
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

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("bulkrequests", Name = "CreateTakedownRequestsFromListing")]
        public async Task<ActionResult> CreateTakedownRequestsFromListing(TakedownRequestsFromListingDto[] requests)
        {
            var errors = await _delistingService.CreateTakedownRequestsFromListingAsync(requests);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("bulkrequests/preview", Name = "GetTakedownRequestsFromListingPreview")]
        public async Task<ActionResult> GetTakedownRequestsFromListingPreview(TakedownRequestsFromListingDto[] requests)
        {
            var (errors, preview) = await _delistingService.GetTakedownRequestsFromListingPreviewAsync(requests);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok(preview);
        }

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("batchtakedownrequest", Name = "SendBatchTakedownRequest")]
        public async Task<ActionResult> SendBatchTakedownRequest([FromForm] BatchTakedownRequestCreateDto dto)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (dto.File == null || dto.File.Length == 0)
            {
                errors.AddItem("File", $"File is null or empty.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var maxSizeInMb = _config.GetValue<int>("RENTAL_LISTING_REPORT_MAX_SIZE");
            var maxSizeInB = (maxSizeInMb == 0 ? 2 : maxSizeInMb) * 1024 * 1024;

            if (dto.File.Length > maxSizeInB)
            {
                errors.AddItem("File", $"The file size exceeds the maximum size {maxSizeInMb}MB.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            if (!CommonUtils.IsTextFile(dto.File.ContentType))
            {
                errors.AddItem("File", $"Uploaded file is not a text file.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            using var stream = dto.File.OpenReadStream();

            errors = await _delistingService.SendBatchTakedownRequestAsync(dto.PlatformId, stream);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [ApiAuthorize(Permissions.TakedownAction)]
        [HttpPost("batchtakedownnotice", Name = "SendBatchTakedownNotice")]
        public async Task<ActionResult> SendBatchtakedownNotice([FromForm] BatchTakedownNoticeCreateDto dto)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (dto.File == null || dto.File.Length == 0)
            {
                errors.AddItem("File", $"File is null or empty.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var maxSizeInMb = _config.GetValue<int>("RENTAL_LISTING_REPORT_MAX_SIZE");
            var maxSizeInB = (maxSizeInMb == 0 ? 2 : maxSizeInMb) * 1024 * 1024;

            if (dto.File.Length > maxSizeInB)
            {
                errors.AddItem("File", $"The file size exceeds the maximum size {maxSizeInMb}MB.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            if (!CommonUtils.IsTextFile(dto.File.ContentType))
            {
                errors.AddItem("File", $"Uploaded file is not a text file.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            using var stream = dto.File.OpenReadStream();

            errors = await _delistingService.SendBatchTakedownNoticeAsync(dto.PlatformId, stream);
            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }


    }
}
