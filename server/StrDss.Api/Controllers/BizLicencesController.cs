﻿using Asp.Versioning;
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
            var history = await _uploadService.GetUploadHistory(orgId, pageSize, pageNumber, orderBy, direction, UploadDeliveryTypes.LicenceData);

            return Ok(history);
        }

        //[ApiAuthorize()]
        //[HttpGet("process")]
        //public async Task<ActionResult> ProcessUpload()
        //{
        //    await _bizLicenceService.ProcessBizLicenceUploadAsync();
        //    return Ok();
        //}
    }
}