using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StrDss.Hangfire.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkCheckerController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetNetworkStatus()
        {
            return Ok(new { status = true, server = "hangfire" });
        }
    }
}
