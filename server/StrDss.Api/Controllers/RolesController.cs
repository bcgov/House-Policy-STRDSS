﻿using Asp.Versioning;
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

        [ApiAuthorize]
        [HttpGet("", Name = "GetRoles")]
        public async Task<ActionResult<List<RoleDto>>> GetRoles()
        {
            return Ok(await _roleService.GetRolesAync());
        }

        [ApiAuthorize]
        [HttpGet("permissions", Name = "GetPermissions")]
        public async Task<ActionResult<List<RoleDto>>> GetPermissions()
        {
            return Ok(await _roleService.GetPermissionsAync());
        }
    }
}