using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.UserDtos;
using System.Security;

namespace StrDss.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<List<RoleDto>> GetRolesAync();
        Task<List<PermissionDto>> GetPermissionsAync();
        Task<RoleDto?> GetRoleAync(string roleCd);
        Task CreateRoleAsync(RoleUpdateDto role);
        Task<int> CountActivePermissionIdsAsnyc(IEnumerable<string> permissions);
        Task<bool> DoesRoleCdExist(string roleCd);
        Task UpdateRoleAsync(RoleUpdateDto role);
    }
    public class RoleRepository : RepositoryBase<DssUserRole>, IRoleRepository
    {
        public RoleRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<List<RoleDto>> GetRolesAync()
        {
            var roles = _mapper.Map<List<RoleDto>>(await _dbSet.AsNoTracking().Include(x => x.UserPrivilegeCds).ToListAsync());

            foreach (var role in roles)
            {
                role.IsReferenced = await _dbSet.AsNoTracking().Where(x => x.UserRoleCd == role.UserRoleCd).Select(x => x.UserIdentities.Any()).FirstAsync();
            }

            return roles;
        }

        public async Task<RoleDto?> GetRoleAync(string roleCd)
        {
            var role = _mapper.Map<RoleDto>(await _dbSet.AsNoTracking().Include(x => x.UserPrivilegeCds).Where(x => x.UserRoleCd == roleCd).FirstOrDefaultAsync());

            if (role == null) return null;

            role.IsReferenced = await _dbSet.AsNoTracking().Where(x => x.UserRoleCd == roleCd).Select(x => x.UserIdentities.Any()).FirstAsync();

            return role;
        }

        public async Task<List<PermissionDto>> GetPermissionsAync()
        {
            return _mapper.Map<List<PermissionDto>>(await _dbContext.DssUserPrivileges.AsNoTracking().ToListAsync());
        }

        public async Task<int> CountActivePermissionIdsAsnyc(IEnumerable<string> permissions)
        {
            return await _dbContext.DssUserPrivileges.CountAsync(x => permissions.Contains(x.UserPrivilegeCd));
        }

        public async Task CreateRoleAsync(RoleUpdateDto role)
        {
            var roleEntity = _mapper.Map<DssUserRole>(role);

            await _dbSet.AddAsync(roleEntity);

            foreach (var permission in role.Permissions)
            {
                var privilege = await _dbContext.DssUserPrivileges
                    .FirstAsync(x => x.UserPrivilegeCd == permission);

                roleEntity.UserPrivilegeCds.Add(privilege);
            }
        }

        public async Task<bool> DoesRoleCdExist(string roleCd)
        {
            return await _dbSet.AnyAsync(x => x.UserRoleCd == roleCd);
        }

        public async Task UpdateRoleAsync(RoleUpdateDto role)
        {
            var roleEntity = await _dbSet.Include(x => x.UserPrivilegeCds).FirstAsync(x => x.UserRoleCd == role.UserRoleCd);

            _mapper.Map(role, roleEntity);

            roleEntity.UserPrivilegeCds.Clear();

            var permissions = role.Permissions.Distinct();
            
            foreach (var permission in role.Permissions)
            {
                var privilege = await _dbContext.DssUserPrivileges
                    .FirstAsync(x => x.UserPrivilegeCd == permission);

                roleEntity.UserPrivilegeCds.Add(privilege);
            }
        }

        private async Task SyncPermissionsAsync(RoleUpdateDto role, DssUserRole roleEntity)
        {
            var permissionsToDelete =
                roleEntity.UserPrivilegeCds.Where(x => !role.Permissions.Contains(x.UserPrivilegeCd)).ToList();

            for (var i = permissionsToDelete.Count() - 1; i >= 0; i--)
            {
                _dbContext.Remove(permissionsToDelete[i]);
            }

            var existingPermissions = roleEntity.UserPrivilegeCds.Select(x => x.UserPrivilegeCd);

            var newPermissions = role.Permissions.Where(x => !existingPermissions.Contains(x));

            foreach (var permission in newPermissions)
            {
                var privilege = await _dbContext.DssUserPrivileges
                    .FirstAsync(x => x.UserPrivilegeCd == permission);

                roleEntity.UserPrivilegeCds.Add(privilege);
            }
        }
    }
}
