using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.UserDtos;

namespace StrDss.Data.Repositories
{
    public interface IUserRepository
    {
        Task<PagedDto<AccessRequestDto>> GetAccessRequestListAsync(string status, int pageSize, int pageNumber, string orderBy, string direction);
        Task CreateUserAsync(UserCreateDto dto);
        Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid);
    }
    public class UserRepository : RepositoryBase<DssUserIdentity>, IUserRepository
    {
        public UserRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser) : base(dbContext, mapper, currentUser)
        {
        }

        public async Task<PagedDto<AccessRequestDto>> GetAccessRequestListAsync(string status, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbSet.AsNoTracking();

            if (status.IsNotEmpty() && status != "All")
            {
                query = query.Where(x => x.AccessRequestStatusDsc == status);
            }

            query = query.Include(x => x.RepresentedByOrganization);

            var results = await Page<DssUserIdentity, AccessRequestDto>(query, pageSize, pageNumber, orderBy, direction);

            return results;
        }

        public async Task CreateUserAsync(UserCreateDto dto)
        {
            await _dbContext.AddAsync(_mapper.Map<DssUserIdentity>(dto));
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid)
        {
            var query = await _dbSet.AsNoTracking()
                .Include(x => x.DssUserRoleAssignments)
                    .ThenInclude(x => x.UserRole)
                        .ThenInclude(x => x.DssUserRolePrivileges)
                            .ThenInclude(x => x.UserPrivilege)
                .FirstOrDefaultAsync(x => x.UserGuid == guid);

            if (query == null)
                return (null, new List<string>());

            var user = _mapper.Map<UserDto>(query);
            var permssions = query.DssUserRoleAssignments
                .Select(x => x.UserRole)
                .SelectMany(x => x.DssUserRolePrivileges.Select(y => y.UserPrivilege))
                .ToLookup(x => x.PrivilegeCd)
                .Select(x => x.First())
                .Select(x => x.PrivilegeCd)
                .ToList();

            return (user, permssions);
        }
    }
}
