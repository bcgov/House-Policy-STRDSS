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
    public class RentalListingReportsController : BaseApiController
    {
        private IUploadDeliveryService _uploadService;

        public RentalListingReportsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IUploadDeliveryService uploadService) : base(currentUser, mapper, config, logger)
        {
            _uploadService = uploadService;
        }

        [ApiAuthorize(Permissions.ListingFileUpload)]
        [HttpPost]
        public async Task<ActionResult> CreateRentalLisingReport([FromForm] PlatformDataUploadDto dto)
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
                errors.AddItem("File", $"Uploaded file is not a CSV file.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            using var stream = dto.File.OpenReadStream();

            errors = await _uploadService.UploadData(UploadDeliveryTypes.ListingData, dto.ReportPeriod, dto.OrganizationId, stream);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            return Ok();
        }

        [ApiAuthorize(Permissions.UploadHistoryRead)]
        [HttpGet("rentallistinghistory")]
        public async Task<ActionResult> GetRentalListingHistory(long? platformId, int pageSize, int pageNumber, string orderBy = "UpdDtm", string direction = "desc")
        {
            var history = await _uploadService.GetUploadHistory(platformId, pageSize, pageNumber, orderBy, direction, 
                new string[] { UploadDeliveryTypes.ListingData, UploadDeliveryTypes.TakedownData });

            return Ok(history);
        }

        [ApiAuthorize(Permissions.LicenceFileUpload, Permissions.ListingFileUpload)]
        [HttpGet("uploads/{uploadId}/errorfile")]
        public async Task<ActionResult> GetRentalListingErrorFile(long uploadId)
        {
            var (bytes, hasPermission) = await _uploadService.GetErrorFile(uploadId);

            if (!hasPermission)
                return Unauthorized();

            if (bytes == null)
                return NotFound();

            return File(bytes!, "text/csv", $"errors-{uploadId}.csv");
        }
    }
}
