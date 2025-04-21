using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Api.Models;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : BaseApiController
    {
        private IUploadDeliveryService _uploadService;
        public RegistrationController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IUploadDeliveryService uploadService) : base(currentUser, mapper, config, logger)
        {
            _uploadService = uploadService;
        }

        [ApiAuthorize(Permissions.ValidateRegistration)]
        [HttpPost]
        public async Task<ActionResult> UploadValidateRegistration([FromForm] ValidateRegistrationUploadDto dto)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (dto.File == null || dto.File.Length == 0)
            {
                errors.AddItem("File", $"File is null or empty.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var maxSizeInMb = _config.GetValue<int>("VALIDATE_REGISTRATION_MAX_SIZE");
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

            errors = await _uploadService.UploadData(UploadDeliveryTypes.RegistrationData, string.Empty, dto.OrganizationId, stream);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            return Ok();
        }

        [ApiAuthorize(Permissions.ValidateRegistration)]
        [HttpGet("registrationvalidationhistory")]
        public async Task<ActionResult> GetRegistrationValidationHistory(long? platformId, int pageSize, int pageNumber, string orderBy = "UpdDtm", string direction = "desc")
        {
            var history = await _uploadService.GetUploadHistory(platformId, pageSize, pageNumber, orderBy, direction,
                [UploadDeliveryTypes.RegistrationData, UploadDeliveryTypes.ListingData]);

            return Ok(history);
        }

        [ApiAuthorize(Permissions.ValidateRegistration)]
        [HttpGet("downloadvalidationreport/{uploadId}")]
        public async Task<ActionResult> DownloadValidationReport(long uploadId)
        {
            var bytes = await _uploadService.DownloadValidationReportAsync(uploadId);

            if (bytes == null)
                return NotFound();

            return File(bytes!, "text/csv", $"validation-report-{uploadId}.csv");
        }
    }
}
