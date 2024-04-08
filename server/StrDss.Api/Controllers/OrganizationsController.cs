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
    }
}
