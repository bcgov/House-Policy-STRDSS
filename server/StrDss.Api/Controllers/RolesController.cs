using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.UserDtos;
using StrDss.Service;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseApiController
    {
        private IRoleService _roleService;

        public RolesController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IRoleService roleService) 
            : base(currentUser, mapper, config, logger)
        {
            _roleService = roleService;
        }

        [ApiAuthorize(Permissions.RoleRead)]
        [HttpGet("", Name = "GetRoles")]
        public async Task<ActionResult<List<RoleDto>>> GetRoles()
        {
            return Ok(await _roleService.GetRolesAync());
        }

        [ApiAuthorize(Permissions.RoleRead)]
        [HttpGet("{roleCd}", Name = "GetRole")]
        public async Task<ActionResult<RoleDto>> GetRole(string roleCd)
        {
            return Ok(await _roleService.GetRoleAync(roleCd));
        }

        [ApiAuthorize(Permissions.RoleRead)]
        [HttpGet("permissions", Name = "GetPermissions")]
        public async Task<ActionResult<List<RoleDto>>> GetPermissions()
        {
            return Ok(await _roleService.GetPermissionsAync());
        }

        [ApiAuthorize(Permissions.RoleWrite)]
        [HttpPost("", Name = "CreateRole")]
        public async Task<ActionResult> CreateRole(RoleUpdateDto dto)
        {
            var errors = await _roleService.CreateRoleAsync(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [ApiAuthorize(Permissions.RoleWrite)]
        [HttpPut("{roleCd}", Name = "UpdateRole")]
        public async Task<ActionResult> UpdateRole(string roleCd, RoleUpdateDto dto)
        {
            dto.UserRoleCd = roleCd;

            var errors = await _roleService.UpdateRoleAsync(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

        [ApiAuthorize(Permissions.RoleWrite)]
        [HttpDelete("{roleCd}", Name = "DeleteRole")]
        public async Task<ActionResult> DeleteRole(string roleCd)
        {
            var errors = await _roleService.DeleteRoleAsync(roleCd);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return NoContent();
        }

    }
}
