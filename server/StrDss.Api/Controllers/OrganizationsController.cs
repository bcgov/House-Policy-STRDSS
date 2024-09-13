using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using StrDss.Service;

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

        [ApiAuthorize]
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
    }
}
