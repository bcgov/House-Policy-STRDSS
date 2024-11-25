﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.UserDtos;
using System;

namespace StrDss.Data.Repositories
{
    public interface IUserRepository
    {
        Task<PagedDto<UserListtDto>> GetUserListAsync(string status, string search, long? orgranizationId, int pageSize, int pageNumber, string orderBy, string direction);
        Task CreateUserAsync(UserCreateDto dto);
        Task<(UserDto? user, List<string> permissions)> GetUserAndPermissionsByGuidAsync(Guid guid);
        Task<(UserDto? user, List<string> permissions)> GetUserAndPermissionsByCurrentUserAsync();
        Task<(UserDto? user, List<string> permissions)> GetUserAndPermissionsByDisplayNameAsync(string displayName);
        Task<UserDto?> GetUserById(long id);
        Task<UserDto?> GetUserByGuid(Guid guid);
        Task<UserDto?> GetUserByCurrentUser();
        Task UpdateUserAsync(UserDto dto);
        Task UpdateUserAsync(UserUpdateDto dto);
        Task DenyAccessRequest(AccessRequestDenyDto dto);
        Task ApproveAccessRequest(AccessRequestApproveDto dto, string role);
        Task<List<UserDto>> GetAdminUsers();
        Task UpdateIsEnabled(UpdateIsEnabledDto dto);
        Task<List<DropdownStrDto>> GetAccessRequestStatuses();
        Task AcceptTermsConditions();
        Task UpdateUserNamesAsync(long userId, string firstName, string lastName);
        Task<DssUserIdentity> CreateApsUserAsync(ApsUserCreateDto dto);
        Task<bool> ApsUserExists(string clientId);
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
                .Include(x => x.DssUserRoleAssignments)
                .Include(x => x.RepresentedByOrganization)
                .FirstOrDefaultAsync(x => x.UserGuid == guid);

            if (query == null)
                return (null, new List<string>());

            var user = _mapper.Map<UserDto>(query); 

            var roles = query.DssUserRoleAssignments.Select(x => x.UserRoleCd).ToList(); 

            var permssions = _dbContext.DssUserRoles
                .Where(x => roles.Contains(x.UserRoleCd))
                .SelectMany(x => x.DssUserRolePrivileges)
                .ToLookup(x => x.UserPrivilegeCd)
                .Select(x => x.First())
                .Select(x => x.UserPrivilegeCd)
                .ToList();

            return (user, permssions);
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserAndPermissionsByCurrentUserAsync()
        {
            DssUserIdentity? userEntity;

            if (_currentUser.IsBcServicesCard)
            {
                userEntity = await _dbSet.AsNoTracking()
                    .Include(x => x.DssUserRoleAssignments)
                    .Include(x => x.RepresentedByOrganization)
                    .FirstOrDefaultAsync(x => x.ExternalIdentityCd == _currentUser.ExternalIdentityCd);
            }
            else
            {
                userEntity = await _dbSet.AsNoTracking()
                    .Include(x => x.DssUserRoleAssignments)
                    .Include(x => x.RepresentedByOrganization)
                    .FirstOrDefaultAsync(x => x.UserGuid == _currentUser.UserGuid);
            }

            if (userEntity == null)
                return (null, new List<string>());

            var user = _mapper.Map<UserDto>(userEntity);

            var roles = userEntity.DssUserRoleAssignments.Select(x => x.UserRoleCd).ToList();

            var permssions = _dbContext.DssUserRoles
                .Where(x => roles.Contains(x.UserRoleCd))
                .SelectMany(x => x.DssUserRolePrivileges)
                .ToLookup(x => x.UserPrivilegeCd)
                .Select(x => x.First())
                .Select(x => x.UserPrivilegeCd)
                .ToList();

            return (user, permssions);
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserAndPermissionsByDisplayNameAsync(string displayName)
        {
            var query = await _dbSet.AsNoTracking()
                .Include(x => x.DssUserRoleAssignments)
                .Include(x => x.RepresentedByOrganization)
                .FirstOrDefaultAsync(x => x.DisplayNm == displayName);

            if (query == null)
                return (null, new List<string>());

            var user = _mapper.Map<UserDto>(query);

            var roles = query.DssUserRoleAssignments.Select(x => x.UserRoleCd).ToList();

            var permssions = _dbContext.DssUserRoles
                .Where(x => roles.Contains(x.UserRoleCd))
                .SelectMany(x => x.DssUserRolePrivileges)
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
                .Include(x => x.DssUserRoleAssignments)
                .Include(x => x.DssUserRoleAssignments)
                .FirstOrDefaultAsync(x => x.UserIdentityId == id);

            var user = _mapper.Map<UserDto>(entity);

            if (user == null) return null;

            user.RoleCds = entity!.DssUserRoleAssignments.Select(x => x.UserRoleCd).ToList();

            var grantedMessage = await _dbContext.DssEmailMessages
                .FirstOrDefaultAsync(x => x.EmailMessageType == EmailMessageTypes.AccessGranted && x.AffectedByUserIdentityId == user.UserIdentityId);

            if (grantedMessage != null)
            {
                user.AccessGrantedDtm = grantedMessage.UpdDtm;
            }

            return user;
        }

        public async Task<UserDto?> GetUserByGuid(Guid guid)
        {
            var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.UserGuid == guid);
            return _mapper.Map<UserDto>(entity);
        }

        public async Task<UserDto?> GetUserByCurrentUser()
        {
            DssUserIdentity? entity;

            if (_currentUser.IsBcServicesCard)
            {
                entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.ExternalIdentityCd == _currentUser.ExternalIdentityCd);
            }
            else
            {
                entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.UserGuid == _currentUser.UserGuid);
            }

            return _mapper.Map<UserDto>(entity);
        }

        public async Task UpdateUserAsync(UserDto dto)
        {
            var entity = await _dbSet.FirstAsync(x => x.UserIdentityId == dto.UserIdentityId);
            _mapper.Map(dto, entity);
        }

        public async Task UpdateUserAsync(UserUpdateDto dto)
        {
            var userEntity = await _dbSet
                .Include(x => x.DssUserRoleAssignments)
                .FirstAsync(x => x.UserIdentityId == dto.UserIdentityId);

            _mapper.Map(dto, userEntity);

            userEntity.DssUserRoleAssignments.Clear();

            var roleCds = dto.RoleCds.Distinct();

            foreach (var roleCd in roleCds)
            {
                var userRole = new DssUserRoleAssignment
                {
                    UserIdentityId = dto.UserIdentityId,
                    UserRoleCd = roleCd
                };

                userEntity.DssUserRoleAssignments.Add(userRole);
            }
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
            var userRole = new DssUserRoleAssignment { UserIdentityId = dto.UserIdentityId, UserRoleCd = role };
            entity.DssUserRoleAssignments.Add(userRole);
        }

        public async Task<List<UserDto>> GetAdminUsers()
        {
            var adminUsers = await _dbContext.DssUserRoles
                .Where(x => x.UserRoleCd == Roles.CeuAdmin)
                .SelectMany(x => x.DssUserRoleAssignments)
                .Select(x => x.UserIdentity)
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
        public async Task UpdateUserNamesAsync(long userId, string firstName, string lastName)
        {
            var entity = await _dbSet.FirstAsync(x => x.UserIdentityId == userId);
            entity.FamilyNm = lastName;
            entity.GivenNm = firstName;
        }

        public async Task<DssUserIdentity> CreateApsUserAsync(ApsUserCreateDto dto)
        {
            dto.FamilyNm = dto.DisplayNm;

            var userEntity = _mapper.Map<DssUserIdentity>(dto);

            userEntity.UserGuid = Guid.NewGuid();
            
            var roleCds = dto.RoleCds.Distinct();

            foreach (var roleCd in roleCds)
            {
                userEntity.DssUserRoleAssignments
                    .Add(new DssUserRoleAssignment { UserRoleCd = roleCd });
            }

            await _dbContext.AddAsync(userEntity);

            return userEntity;
        }

        public async Task<bool> ApsUserExists(string clientId)
        {
            return await _dbSet.AnyAsync(x => x.DisplayNm == clientId);
        }
    }
}
