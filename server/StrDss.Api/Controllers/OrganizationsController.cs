using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using StrDss.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : BaseApiController
    {
        private IOrganizationService _orgService;

        public OrganizationsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IOrganizationService orgService) 
            : base(currentUser, mapper, config, logger)
        {
            _orgService = orgService;
        }

        [ApiAuthorize]
        [HttpGet("types", Name = "GetOrganizationTypes")]
        public async Task<ActionResult<List<OrganizationTypeDto>>> GetOrganizationTypes()
        {
            return Ok(await _orgService.GetOrganizationTypesAsnc());
        }

        [ApiAuthorize]
        [HttpGet("", Name = "GetOrganizations")]
        public async Task<ActionResult<List<OrganizationDto>>> GetOrganizations(string? type)
        {
            return Ok(await _orgService.GetOrganizationsAsync(type));
        }

        [ApiAuthorize]
        [HttpGet("dropdown", Name = "GetOrganizationsDropdown")]
        public async Task<ActionResult<List<DropdownNumDto>>> GetOrganizationsDropdown(string? type)
        {
            return Ok(await _orgService.GetOrganizationsDropdownAsync(type));
        }

        /// <summary>
        /// Retrieves the Short-Term Rental (STR) requirements for a specified location based on longitude and latitude coordinates.
        /// Validates the geographical boundaries of the input values to ensure they fall within acceptable ranges.
        /// </summary>
        /// <param name="longitude">The longitude of the location, must be between -180 and 180 degrees.</param>
        /// <param name="latitude">The latitude of the location, must be between -90 and 90 degrees.</param>
        /// <returns>An object containing STR requirements for the given location, or a validation error if the input is invalid.</returns>
        [ApiAuthorize] //todo: specify permission
        [SwaggerOperation(Tags = new string[] { Common.ApiTags.Aps })]
        [HttpGet("strrequirements", Name = "GetStrRequirements")]
        public async Task<ActionResult<StrRequirementsDto>> GetStrRequirements(double longitude, double latitude)
        {
            var errors = new Dictionary<string, List<string>>();

            if (longitude < -180 || longitude > 180)
                errors.AddItem("logitude", "Longitude must be between -180 and 180 degrees.");

            if (latitude < -90 || latitude > 90)
                errors.AddItem("latitude", "Longitude must be between -180 and 180 degrees.");

            if (errors.Any())
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred.");

            var strRequirement = await _orgService.GetStrRequirements(longitude, latitude);

            if (strRequirement == null)
            {
                return NotFound();
            }

            return Ok(await _orgService.GetStrRequirements(longitude, latitude));
        }


        [ApiAuthorize(Permissions.PlatformRead)]
        [HttpGet("platforms")]
        public async Task<ActionResult<List<PlatformViewDto>>> GetPlatforms(int pageSize = 10, int pageNumber = 1, string orderBy = "OrganizationNm", string direction = "asc")
        {
            var platforms = await _orgService.GetPlatforms(pageSize, pageNumber, orderBy, direction);
            return Ok(platforms);
        }

        [ApiAuthorize(Permissions.PlatformRead)]
        [HttpGet("platforms/{id}")]
        public async Task<ActionResult<PlatformViewDto>> GetPlatform(long id)
        {
            var platform = await _orgService.GetPlatform(id);

            if (platform == null)
            {
                return NotFound();
            }

            return Ok(platform);
        }

        [ApiAuthorize(Permissions.PlatformWrite)]
        [HttpPost("platforms", Name = "CreatePlatform")]
        public async Task<ActionResult> CreatePlatform(PlatformCreateDto dto)
        {
            var (errors, id) = await _orgService.CreatePlatformAsync(dto);

            if (errors.Any())
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok(id);
        }

        [ApiAuthorize(Permissions.PlatformWrite)]
        [HttpPut("platforms/{id}", Name = "UpdatePlatform")]
        public async Task<ActionResult> UpdatePlatform(PlatformUpdateDto dto, long id)
        {
            dto.OrganizationId = id;

            var errors = await _orgService.UpdatePlatformAsync(dto);

            if (errors.Any())
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        [ApiAuthorize(Permissions.PlatformWrite)]
        [HttpPost("platforms/subsidiaries", Name = "CreatePlatformSub")]
        public async Task<ActionResult> CreatePlatformSub(PlatformSubCreateDto dto)
        {
            var (errors, id) = await _orgService.CreatePlatformSubAsync(dto);

            if (errors.Any())
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok(id);
        }

        [ApiAuthorize(Permissions.PlatformWrite)]
        [HttpPut("platforms/subsidiaries/{id}", Name = "UpdatePlatformSub")]
        public async Task<ActionResult> UpdatePlatformSub(PlatformSubUpdateDto dto, long id)
        {
            dto.OrganizationId = id;

            var errors = await _orgService.UpdatePlatformSubAsync(dto);

            if (errors.Any())
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }
    }
}
