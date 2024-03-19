using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Model;
using StrDss.Model.UserDtos;
using StrDss.Service;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private IUserService _userService;

        public UsersController(ICurrentUser currentUser, IMapper mapper, IConfiguration config,
            IUserService userService)
            : base(currentUser, mapper, config)
        {
            _userService = userService;
        }

        [ApiAuthorize]
        [HttpGet("currentuser", Name = "GetCurrentUser")]
        public ActionResult<ICurrentUser> GetCurrentUser()
        {
            return Ok(_currentUser);
        }

        [ApiAuthorize]
        [HttpGet("accessrequestlist", Name = "GetAccessRequestList")]
        public async Task<ActionResult<PagedDto<AccessRequestDto>>> GetAccessRequestList(string? status, int pageSize, int pageNumber, string orderBy = "AccessRequestDtm", string direction = "")
        {
            var list = await _userService.GetAccessRequestListAsync(status ?? "", pageSize, pageNumber, orderBy, direction);
            return Ok(list);
        }

        [ApiAuthorize]
        [HttpPost("createaccessrequest", Name = "CreateAccessRequest")]
        public async Task<ActionResult> CreateAccessRequest(AccessRequestCreateDto dto)
        {
            var errors = await _userService.CreateAccessRequestAsync(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }
    }
}
