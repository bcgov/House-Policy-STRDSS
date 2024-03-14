using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Model;
using StrDss.Model.PlatformDtos;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : BaseApiController
    {
        public PlatformsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config)
            : base(currentUser, mapper, config)
        {
        }

        [HttpGet("dropdown", Name = "GetPlatformsDrowdown")]
        [ApiAuthorize]
        public ActionResult<DropdownDto> GetPlatformsDrowdown()
        {
            return Ok(PlatformDto.Platforms.Select(x => new DropdownDto { Id = x.PlatformId, Description = x.Name }));
        }
    }
}
