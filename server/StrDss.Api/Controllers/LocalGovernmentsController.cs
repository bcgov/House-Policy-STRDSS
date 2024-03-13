using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Model;
using StrDss.Model.LocalGovernmentDtos;
using StrDss.Model.PlatformDtos;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class LocalGovernmentsController : BaseApiController
    {
        public LocalGovernmentsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config)
            : base(currentUser, mapper, config)
        {
        }

        [HttpGet("dropdown", Name = "GetLocalGovernmentsDrowdown")]
        [ApiAuthorize]
        public ActionResult<DropdownDto> GetLocalGovernmentsDrowdown()
        {
            return Ok(LocalGovernmentDto.localGovernments.Select(x => new DropdownDto { Id = x.LocalGovernmentId, Description = x.Name }));
        }
    }
}
