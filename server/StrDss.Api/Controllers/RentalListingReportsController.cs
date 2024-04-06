using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;
using System.Text;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalListingReportsController : BaseApiController
    {
        private IRentalListingReportService _listingService;

        public RentalListingReportsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config,
            IRentalListingReportService listingSeervice)
            : base(currentUser, mapper, config)
        {
            _listingService = listingSeervice;
        }

        [ApiAuthorize(Permissions.ListingFileUpload)]
        [HttpPost]
        public async Task<ActionResult> CreateRentalLisingReport([FromForm] RentalListingFileUploadDto dto)
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

            if (!IsTextFile(dto.File.ContentType))
            {
                errors.AddItem("File", $"Uploaded file is not a text file.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            using var stream = dto.File.OpenReadStream();
            using TextReader textReader = new StreamReader(stream, Encoding.UTF8);

            errors = await _listingService.ValidateAndParseUploadFileAsync(dto.ReportPeriod, dto.OrganizationId, textReader);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        private bool IsTextFile(string contentType)
        {
            return contentType == "text/plain" ||
                   contentType == "text/csv" ||
                   contentType == "application/octet-stream";
        }
    }
}
