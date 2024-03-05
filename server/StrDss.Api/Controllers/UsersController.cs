using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Model;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        public UsersController(ICurrentUser currentUser, IMapper mapper, IConfiguration config)
            : base(currentUser, mapper, config)
        {
        }

        [HttpGet("currentuser", Name = "GetCurrentUser")]
        [ApiAuthorize]
        public ActionResult<ICurrentUser> GetCurrentUser()
        {
            return Ok(_currentUser);
        }
    }
}
