using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.UserDtos;

namespace StrDss.Data.Repositories
{
    public interface IUserRepository
    {
        Task<PagedDto<UserListtDto>> GetUserListAsync(string status, string search, long? orgranizationId, int pageSize, int pageNumber, string orderBy, string direction);
        Task CreateUserAsync(UserCreateDto dto);
        Task<(UserDto? user, List<string> permissions)> GetUserAndPermissionsByGuidAsync(Guid guid);
        Task<UserDto?> GetUserById(long id);
        Task<UserDto?> GetUserByGuid(Guid guid);
        Task UpdateUserAsync(UserDto dto);
        Task DenyAccessRequest(AccessRequestDenyDto dto);
        Task ApproveAccessRequest(AccessRequestApproveDto dto, string role);
        Task<List<UserDto>> GetAdminUsers();
        Task UpdateIsEnabled(UpdateIsEnabledDto dto);
        Task<List<DropdownStrDto>> GetAccessRequestStatuses();
        Task AcceptTermsConditions();
    }
    public class UserRepository : RepositoryBase<DssUserIdentity>, IUserRepository
    {
        public UserRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<PagedDto<UserListtDto>> GetUserListAsync(string status, string search, long? orgranizationId, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbContext.DssUserIdentityViews.AsNoTracking();

            if (status.IsNotEmpty() && status != "All")
            {
                query = query.Where(x => x.AccessRequestStatusCd == status);
            }

            if (orgranizationId != null)
            {
                query = query.Where(x => x.RepresentedByOrganizationId == orgranizationId);
            }

            if (search.IsNotEmpty())
            {
                query = query.Where(x => x.GivenNm != null && x.GivenNm.Contains(search)
                    || x.FamilyNm != null && x.FamilyNm.Contains(search)
                    || x.OrganizationNm != null && x.OrganizationNm.Contains(search)
                    || x.OrganizationType != null && x.OrganizationType.Contains(search)
                    || x.EmailAddressDsc != null && x.EmailAddressDsc.Contains(search)
                );
            }

            var results = await Page<DssUserIdentityView, UserListtDto>(query, pageSize, pageNumber, orderBy, direction);

            return results;
        }

        public async Task CreateUserAsync(UserCreateDto dto)
        {
            await _dbContext.AddAsync(_mapper.Map<DssUserIdentity>(dto));
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserAndPermissionsByGuidAsync(Guid guid)
        {
            var query = await _dbSet.AsNoTracking()
                .Include(x => x.UserRoleCds)
                .Include(x => x.RepresentedByOrganization)
                .FirstOrDefaultAsync(x => x.UserGuid == guid);

            if (query == null)
                return (null, new List<string>());

            var user = _mapper.Map<UserDto>(query); 

            var roles = user.UserRoleCds.Select(x => x.UserRoleCd).ToList();

            var permssions = _dbContext.DssUserRoles
                .Where(x => roles.Contains(x.UserRoleCd))
                .SelectMany(x => x.UserPrivilegeCds)
                .ToLookup(x => x.UserPrivilegeCd)
                .Select(x => x.First())
                .Select(x => x.UserPrivilegeCd)
                .ToList();

            return (user, permssions);
        }

        public async Task<UserDto?> GetUserById(long id)
        {
            var entity = await _dbSet.AsNoTracking()
                .Include(x => x.RepresentedByOrganization)
                .FirstOrDefaultAsync(x => x.UserIdentityId == id);
            return _mapper.Map<UserDto>(entity);
        }

        public async Task<UserDto?> GetUserByGuid(Guid guid)
        {
            var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.UserGuid == guid);
            return _mapper.Map<UserDto>(entity);
        }

        public async Task UpdateUserAsync(UserDto dto)
        {
            var entity = await _dbSet.FirstAsync(x => x.UserIdentityId == dto.UserIdentityId);
            _mapper.Map(dto, entity);
        }

        public async Task DenyAccessRequest(AccessRequestDenyDto dto)
        {
            var entity = await _dbSet.FirstAsync(x => x.UserIdentityId == dto.UserIdentityId);
            _mapper.Map(dto, entity);
        }

        public async Task ApproveAccessRequest(AccessRequestApproveDto dto, string role)
        {
            var entity = await _dbSet.FirstAsync(x => x.UserIdentityId == dto.UserIdentityId);
            _mapper.Map(dto, entity);

            var roleEntity = await _dbContext.DssUserRoles.FirstAsync(x => x.UserRoleCd == role);
            entity.UserRoleCds.Add(roleEntity);
        }

        public async Task<List<UserDto>> GetAdminUsers()
        {
            var adminUsers = await _dbContext.DssUserRoles
                .Where(x => x.UserRoleCd == Roles.CeuAdmin)
                .SelectMany(x => x.UserIdentities)
                .Where(x => x.IsEnabled == true)
                .ToListAsync();

            return _mapper.Map<List<UserDto>>(adminUsers);
        }

        public async Task UpdateIsEnabled(UpdateIsEnabledDto dto)
        {
            var entity = await _dbSet.FirstAsync(x => x.UserIdentityId == dto.UserIdentityId);
            _mapper.Map(dto, entity);
        }

        public async Task<List<DropdownStrDto>> GetAccessRequestStatuses()
        {
            var statuses = await _dbContext.DssAccessRequestStatuses
                .AsNoTracking()
                .Select(x => new DropdownStrDto { Id = x.AccessRequestStatusCd, Description = x.AccessRequestStatusNm })
                .ToListAsync();

            return statuses;
        }

        public async Task AcceptTermsConditions()
        {
            var entity = await _dbSet.FirstAsync(x => x.UserIdentityId == _currentUser.Id);

            if(entity != null)
                entity.TermsAcceptanceDtm = DateTime.UtcNow;
        }
    }
}
