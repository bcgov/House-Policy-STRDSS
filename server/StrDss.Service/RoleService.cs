using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
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
        Task<Dictionary<string, List<string>>> CreateRoleAsync(RoleUpdateDto role);
        Task<Dictionary<string, List<string>>> UpdateRoleAsync(RoleUpdateDto role);
        Task<Dictionary<string, List<string>>> DeleteRoleAsync(string roleCd);
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
        public async Task<Dictionary<string, List<string>>> CreateRoleAsync(RoleUpdateDto role)
        {
            var errors = new Dictionary<string, List<string>>();

            if (await _roleRepo.DoesRoleCdExist(role.UserRoleCd))
            {
                errors.AddItem("userRoleCd", $"Role Code [{role.UserRoleCd}] already exists.");
                return errors;
            }

            errors = await ValidateRoleDtoAsync(role);

            if (errors.Count > 0)
            {
                return errors;
            }

            await _roleRepo.CreateRoleAsync(role);

            _unitOfWork.Commit();

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateRoleDtoAsync(RoleUpdateDto role)
        {
            var errors = new Dictionary<string, List<string>>();

            errors = _validator.Validate(Entities.Role, role, errors);

            var permissionCount = await _roleRepo.CountActivePermissionIdsAsnyc(role.Permissions);
            if (permissionCount != role.Permissions.Count)
            {
                errors.AddItem("permission", $"Some of the permissions are invalid.");
            }

            if (await _roleRepo.DoseRoleWithSamePermissionsExist(role.UserRoleCd, role.Permissions))
            {
                errors.AddItem("permissions", $"A role with these permissions already exists. Duplicate roles are not permitted. Please modify the permissions and try again.");
            }

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> UpdateRoleAsync(RoleUpdateDto role)
        {
            var errors = new Dictionary<string, List<string>>();

            if (!await _roleRepo.DoesRoleCdExist(role.UserRoleCd))
            {
                errors.AddItem("userRoleCd", $"Role Code [{role.UserRoleCd}] not found.");
                return errors;
            }

            errors = await ValidateRoleDtoAsync(role);

            if (errors.Count > 0)
            {
                return errors;
            }

            await _roleRepo.UpdateRoleAsync(role);

            _unitOfWork.Commit();

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> DeleteRoleAsync(string roleCd)
        {
            var errors = new Dictionary<string, List<string>>();

            var role = await _roleRepo.GetRoleAync(roleCd);

            if (role == null)
            {
                errors.AddItem("userRoleCd", $"Role Code [{roleCd}] not found.");
                return errors;
            }

            if (role.IsReferenced)
            {
                errors.AddItem("userRoleCd", $"Role Code [{roleCd}] is assigned to users and cannot be deleted.");
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            await _roleRepo.DeleteRoleAsync(roleCd);

            _unitOfWork.Commit();

            return errors;
        }
    }
}
