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
        [HttpGet("", Name = "GetUserList")]
        public async Task<ActionResult<PagedDto<UserListtDto>>> GetUserList(string? status, int pageSize, int pageNumber, string orderBy = "UpdDtm", string direction = "desc")
        {
            var list = await _userService.GetUserListAsync(status ?? "", pageSize, pageNumber, orderBy, direction);
            return Ok(list);
        }

        [ApiAuthorize]
        [HttpPost("accessrequests", Name = "CreateAccessRequest")]
        public async Task<ActionResult> CreateAccessRequest(AccessRequestCreateDto dto)
        {
            var errors = await _userService.CreateAccessRequestAsync(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        [ApiAuthorize]
        [HttpPut("accessrequests/deny", Name = "DenyRequest")]
        public async Task<ActionResult> DenyRequest(AccessRequestDenyDto dto)
        {
            var errors = await _userService.DenyAccessRequest(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        [ApiAuthorize]
        [HttpPut("accessrequests/approve", Name = "ApproveRequest")]
        public async Task<ActionResult> ApproveRequest(AccessRequestApproveDto dto)
        {
            var errors = await _userService.ApproveAccessRequest(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        [ApiAuthorize]
        [HttpPut("updateisenabled", Name = "UpdateIsEnabled")]
        public async Task<ActionResult> UpdateIsEnabled(UpdateIsEnabledDto dto)
        {
            var errors = await _userService.UpdateIsEnabled(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        [ApiAuthorize]
        [HttpGet("accessrequeststatuses", Name = "GetAccessRequestStatuses")]
        public async Task<ActionResult<List<DropdownStrDto>>> GetAccessRequestStatuses()
        {
            return Ok(await _userService.GetAccessRequestStatuses());
        }

        [ApiAuthorize]
        [HttpPut("accepttermsconditions", Name = "AcceptTermsConditions")]
        public async Task<ActionResult> AcceptTermsConditions()
        {
            await _userService.AcceptTermsConditions();
            return Ok();
        }
    }
}
