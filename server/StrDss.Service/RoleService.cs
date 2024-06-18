using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.UserDtos;

namespace StrDss.Service
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetRolesAync();
        Task<RoleDto?> GetRoleAync(string roleCd);
        Task<List<PermissionDto>> GetPermissionsAync();
    }
    public class RoleService : ServiceBase, IRoleService
    {
        private IRoleRepository _roleRepo;

        public RoleService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IRoleRepository roleRepo) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _roleRepo = roleRepo;
        }
        public async Task<List<RoleDto>> GetRolesAync()
        {
            return await _roleRepo.GetRolesAync();
        }

        public async Task<RoleDto?> GetRoleAync(string roleCd)
        {
            return await _roleRepo.GetRoleAync(roleCd);
        }
        public async Task<List<PermissionDto>> GetPermissionsAync()
        {
            return await _roleRepo.GetPermissionsAync();
        }
    }
}
