using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Api.Models;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class BizLicencesController : BaseApiController
    {
        private IUploadDeliveryService _uploadService;
        private IBizLicenceService _bizLicenceService;

        public BizLicencesController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IUploadDeliveryService uploadService, IBizLicenceService bizLicenceService) 
            : base(currentUser, mapper, config, logger)
        {
            _uploadService = uploadService;
            _bizLicenceService = bizLicenceService;
        }

        [ApiAuthorize(Permissions.LicenceFileUpload)]
        [HttpPost]
        public async Task<ActionResult> UploadBizLicences([FromForm] PlatformDataUploadDto dto)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (dto.File == null || dto.File.Length == 0)
            {
                errors.AddItem("File", $"File is null or empty.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var maxSizeInMb = _config.GetValue<int>("BUISINESS_LICENCE_MAX_SIZE");
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

            errors = await _uploadService.UploadPlatformData(UploadDeliveryTypes.LicenceData, dto.ReportPeriod, dto.OrganizationId, stream);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            return Ok();
        }

        [ApiAuthorize(Permissions.UploadHistoryRead)]
        [HttpGet("uploadhistory")]
        public async Task<ActionResult> GetUploadHistory(long? orgId, int pageSize, int pageNumber, string orderBy = "UpdDtm", string direction = "desc")
        {
            if (_currentUser.OrganizationType != OrganizationTypes.BCGov)
            {
                orgId = _currentUser.OrganizationId;
            }

            var history = await _uploadService.GetUploadHistory(orgId, pageSize, pageNumber, orderBy, direction, new string[] { UploadDeliveryTypes.LicenceData });

            return Ok(history);
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("{orgId}/{licenceNo}")]
        public async Task<ActionResult> SearchBizLicence(long orgId, string licenceNo)
        {
            if (_currentUser.OrganizationType != OrganizationTypes.BCGov && _currentUser.OrganizationId != orgId)
            {
                var authError = new Dictionary<string, List<string>>();
                authError.AddItem("OrganizationId", $"The user is not authorized to read the licence data. The user's organization ({_currentUser.OrganizationId}) is not {orgId}.");
                return ValidationUtils.GetValidationErrorResult(authError, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            var licenceNos = await _bizLicenceService.SearchBizLicence(orgId, licenceNo);

            return Ok(licenceNos);
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpPut("{licenceId}/{rentalListingId}")]
        public async Task<ActionResult> LinkBizLicence(long rentalListingId, long licenceId)
        {
            var (errors, listing) = await _bizLicenceService.LinkBizLicence(rentalListingId, licenceId);

            if (errors.Any())
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            return Ok(listing);
        }
    }
}
