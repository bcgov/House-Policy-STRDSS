using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
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

        public OrganizationsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config,
            IOrganizationService orgService) 
            : base(currentUser, mapper, config)
        {
            _orgService = orgService;
        }

        [HttpGet("types", Name = "GetOrganizationTypes")]
        [ApiAuthorize]
        public async Task<ActionResult<List<OrganizationTypeDto>>> GetOrganizationTypes()
        {
            return Ok(await _orgService.GetOrganizationTypesAsnc());
        }

        [HttpGet("", Name = "GetOrganizations")]
        [ApiAuthorize]
        public async Task<ActionResult<List<OrganizationDto>>> GetOrganizations(string? type)
        {
            return Ok(await _orgService.GetOrganizationsAsync(type));
        }
    }
}
