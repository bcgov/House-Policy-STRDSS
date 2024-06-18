using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.UserDtos;

namespace StrDss.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<List<RoleDto>> GetRolesAync();
        Task<List<PermissionDto>> GetPermissionsAync();
        Task<RoleDto?> GetRoleAync(string roleCd);
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
    }
}
