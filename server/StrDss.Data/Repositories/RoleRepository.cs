using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.UserDtos;
using System.Collections.Generic;
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
        Task<bool> DoseRoleWithSamePermissionsExist(string roleCd, IEnumerable<string> permissions);
        Task UpdateRoleAsync(RoleUpdateDto role);
        Task DeleteRoleAsync(string roleCd);
    }
    public class RoleRepository : RepositoryBase<DssUserRole>, IRoleRepository
    {
        public RoleRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<List<RoleDto>> GetRolesAync()
        {
            var roleEntities = await _dbSet.AsNoTracking().Include(x => x.DssUserRolePrivileges).ToListAsync();

            var roles = _mapper.Map<List<RoleDto>>(roleEntities);

            foreach (var role in roles)
            {
                var permissionEntities = await _dbContext
                    .DssUserRolePrivileges
                    .Where(x => x.UserRoleCd == role.UserRoleCd)
                    .Select(x => x.UserPrivilegeCdNavigation)
                    .ToListAsync();

                role.Permissions = _mapper.Map<List<PermissionDto>>(permissionEntities);
                role.IsReferenced = await _dbSet.AsNoTracking().Where(x => x.UserRoleCd == role.UserRoleCd).Select(x => x.DssUserRoleAssignments.Any()).FirstAsync();
            }

            return roles;
        }

        public async Task<RoleDto?> GetRoleAync(string roleCd)
        {
            var roleEntity = await _dbSet.AsNoTracking().Include(x => x.DssUserRolePrivileges).FirstOrDefaultAsync(x => x.UserRoleCd == roleCd);

            if (roleEntity == null) return null;

            var role = _mapper.Map<RoleDto>(roleEntity);

            var permissionEntities = await _dbContext
                .DssUserRolePrivileges
                .Where(x => x.UserRoleCd == role.UserRoleCd)
                .Select(x => x.UserPrivilegeCdNavigation)
                .ToListAsync();

            role.Permissions = _mapper.Map<List<PermissionDto>>(permissionEntities);
            role.IsReferenced = await _dbSet.AsNoTracking().Where(x => x.UserRoleCd == roleCd).Select(x => x.DssUserRoleAssignments.Any()).FirstAsync();

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
                var rolePermission = new DssUserRolePrivilege
                {
                    UserPrivilegeCd = permission,
                    UserRoleCd = roleEntity.UserRoleCd,
                };

                roleEntity.DssUserRolePrivileges.Add(rolePermission);
            }
        }

        public async Task<bool> DoesRoleCdExist(string roleCd)
        {
            return await _dbSet.AnyAsync(x => x.UserRoleCd == roleCd);
        }

        public async Task<bool> DoseRoleWithSamePermissionsExist(string roleCd, IEnumerable<string> permissions)
        {
            // Convert permissions to a sorted list to ensure order does not matter
            var sortedPermissions = permissions.Distinct().OrderBy(p => p).ToList();

            // Get roles and their permissions, excluding the specified roleCd
            var rolesWithPermissions = await _dbContext.DssUserRolePrivileges
                .Where(rp => rp.UserRoleCd != roleCd)
                .GroupBy(rp => rp.UserRoleCd)
                .Select(g => new
                {
                    UserRoleCd = g.Key,
                    Permissions = g.Select(rp => rp.UserPrivilegeCd).OrderBy(p => p).ToList()
                })
                .ToListAsync();

            // Check if any role has the same permissions as provided
            return rolesWithPermissions.Any(rwp => rwp.Permissions.SequenceEqual(sortedPermissions));
        }


        public async Task UpdateRoleAsync(RoleUpdateDto role)
        {
            var roleEntity = await _dbSet.Include(x => x.DssUserRolePrivileges).FirstAsync(x => x.UserRoleCd == role.UserRoleCd);

            _mapper.Map(role, roleEntity);

            roleEntity.DssUserRolePrivileges.Clear();

            var permissions = role.Permissions.Distinct();
            
            foreach (var permission in role.Permissions)
            {
                var rolePermission = new DssUserRolePrivilege
                {
                    UserPrivilegeCd = permission,
                    UserRoleCd = roleEntity.UserRoleCd,
                };

                roleEntity.DssUserRolePrivileges.Add(rolePermission);
            }
        }

        public async Task DeleteRoleAsync(string roleCd)
        {
            var roleEntity = await _dbSet
                .Include(x => x.DssUserRolePrivileges)
                .FirstAsync(x => x.UserRoleCd == roleCd);

            roleEntity.DssUserRolePrivileges.Clear();

            _dbSet.Remove(roleEntity);
        }
    }
}
