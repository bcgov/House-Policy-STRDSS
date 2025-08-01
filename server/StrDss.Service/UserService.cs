﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.UserDtos;
using StrDss.Service.Bceid;
using StrDss.Service.EmailTemplates;

namespace StrDss.Service
{
    public interface IUserService
    {
        Task<PagedDto<UserListtDto>> GetUserListAsync(string status, string search, long? orgranizationId, int pageSize, int pageNumber, string orderBy, string direction);
        Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid);
        Task<(UserDto? user, List<string> permissions)> GetUserByCurrentUserAsync();
        Task<Dictionary<string, List<string>>> CreateAccessRequestAsync(AccessRequestCreateDto dto);
        Task<Dictionary<string, List<string>>> DenyAccessRequest(AccessRequestDenyDto dto);
        Task<Dictionary<string, List<string>>> ApproveAccessRequest(AccessRequestApproveDto dto);
        Task<Dictionary<string, List<string>>> UpdateIsEnabled(UpdateIsEnabledDto dto);
        Task<List<DropdownStrDto>> GetAccessRequestStatuses();
        Task<Dictionary<string, List<string>>> AcceptTermsConditions();
        Task UpdateBceidUserInfo(long userId, string firstName, string LastName);
        Task<UserDto?> GetUserByIdAsync(long userId);
        Task<Dictionary<string, List<string>>> UpdateUserAsync(UserUpdateDto dto);
        Task<BceidAccount?> GetBceidUserInfo();
        Task<(Dictionary<string, List<string>>, long)> CreateApsUserAsync(ApsUserCreateDto dto);
        Task<(UserDto? user, List<string> permissions)> GetUserByDisplayNameAsync(string displayName);
    }
    public class UserService : ServiceBase, IUserService
    {
        private IUserRepository _userRepo;
        private IOrganizationRepository _orgRepo;
        private IEmailMessageService _emailService;
        private IEmailMessageRepository _emailRepo;
        private IBceidApi _bceid;
        private IRoleRepository _roleRepo;

        public UserService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IUserRepository userRepo, IOrganizationRepository orgRepo, IEmailMessageService emailService, IEmailMessageRepository emailRepo, IBceidApi bceid, IRoleRepository roleRepo)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _userRepo = userRepo;
            _orgRepo = orgRepo;
            _emailService = emailService;
            _emailRepo = emailRepo;
            _bceid = bceid;
            _roleRepo = roleRepo;
        }

        public async Task<PagedDto<UserListtDto>> GetUserListAsync(string status, string search, long? orgranizationId, int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _userRepo.GetUserListAsync(status, search, orgranizationId, pageSize, pageNumber, orderBy, direction);
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid)
        {
            return await _userRepo.GetUserAndPermissionsByGuidAsync(guid);
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserByCurrentUserAsync()
        {
            return await _userRepo.GetUserAndPermissionsByCurrentUserAsync();
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserByDisplayNameAsync(string displayName)
        {
            return await _userRepo.GetUserAndPermissionsByDisplayNameAsync(displayName);
        }

        public async Task<Dictionary<string, List<string>>> CreateAccessRequestAsync(AccessRequestCreateDto dto)
        {
            var (errors, userDto) = await ValidateAccessRequestCreateDtoAsync(dto);

            if (errors.Count > 0)
            {
                return errors;
            }

            if (_currentUser.IdentityProviderNm == StrDssIdProviders.BceidBusiness)
            {
                try
                {
                    var (error, account) = await _bceid.GetBceidAccountCachedAsync(_currentUser.UserGuid, "", StrDssIdProviders.BceidBusiness, _currentUser.UserGuid, _currentUser.IdentityProviderNm);

                    if (account == null)
                    {
                        _logger.LogError($"BCeID call error: {error}");
                    }

                    if (account != null)
                    {
                        _currentUser.FirstName = account.FirstName;
                        _currentUser.LastName = account.LastName;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"BCeID Web call failed - {ex.Message}", ex);
                    _logger.LogInformation("BCeID Web call failed - Skipping UpdateBceidUserInfo ");
                }
            }

            if (userDto == null)
            {
                var userCreateDto = new UserCreateDto
                {
                    UserGuid = _currentUser.IsBcServicesCard ? Guid.NewGuid() : _currentUser.UserGuid,
                    ExternalIdentityCd = _currentUser.ExternalIdentityCd,
                    DisplayNm = _currentUser.DisplayName,
                    IdentityProviderNm = _currentUser.IdentityProviderNm,
                    IsEnabled = false,
                    AccessRequestStatusCd = AccessRequestStatuses.Requested,
                    AccessRequestDtm = DateTime.UtcNow,
                    AccessRequestJustificationTxt = $"{dto.OrganizationType}, {dto.OrganizationName}",
                    GivenNm = _currentUser.FirstName,
                    FamilyNm = _currentUser.LastName,
                    EmailAddressDsc = _currentUser.EmailAddress,
                    BusinessNm = _currentUser.BusinessNm,
                    TermsAcceptanceDtm = _currentUser.IdentityProviderNm == StrDssIdProviders.Idir ? DateTime.UtcNow : null, // no need for the idir user to accept the term
                };

                await _userRepo.CreateUserAsync(userCreateDto);
            }
            else
            {
                userDto.DisplayNm = _currentUser.DisplayName;
                userDto.IdentityProviderNm = _currentUser.IdentityProviderNm;
                userDto.IsEnabled = false;
                userDto.AccessRequestStatusCd = AccessRequestStatuses.Requested;
                userDto.AccessRequestDtm = DateTime.UtcNow;
                userDto.AccessRequestJustificationTxt = $"{dto.OrganizationType}, {dto.OrganizationName}";
                userDto.GivenNm = _currentUser.FirstName;
                userDto.FamilyNm = _currentUser.LastName;
                userDto.EmailAddressDsc = _currentUser.EmailAddress;
                userDto.BusinessNm = _currentUser.BusinessNm;
                userDto.RepresentedByOrganizationId = null;

                await _userRepo.UpdateUserAsync(userDto);
            }

            using var transaction = _unitOfWork.BeginTransaction();

            _unitOfWork.Commit();

            var user = await _userRepo.GetUserByCurrentUser();

            var adminUsers = await _userRepo.GetAdminUsers();

            if (adminUsers.Count > 0)
            {
                var emails = adminUsers.Select(x => x.EmailAddressDsc);

                var template = new NewAccessRequest(_emailService)
                {
                    Link = GetHostUrl() + "/user-management",
                    To = emails!,
                    Info = $"New Access Request email for {_currentUser.DisplayName}"
                };

                var emailEntity = new DssEmailMessage
                {
                    EmailMessageType = template.EmailMessageType,
                    MessageDeliveryDtm = DateTime.UtcNow,
                    MessageTemplateDsc = template.GetContent(),
                    IsHostContactedExternally = false,
                    IsSubmitterCcRequired = false,
                    LgPhoneNo = null,
                    UnreportedListingNo = null,
                    HostEmailAddressDsc = null,
                    LgEmailAddressDsc = null,
                    CcEmailAddressDsc = null,
                    UnreportedListingUrl = null,
                    LgStrBylawUrl = null,
                    InitiatingUserIdentityId = user!.UserIdentityId,
                    AffectedByUserIdentityId = user!.UserIdentityId,
                    InvolvedInOrganizationId = null
                };

                await _emailRepo.AddEmailMessage(emailEntity);

                emailEntity.ExternalMessageNo = await template.SendEmail();

                _unitOfWork.Commit();
            }

            _unitOfWork.CommitTransaction(transaction);

            return errors;
        }

        private async Task<(Dictionary<string, List<string>> errors, UserDto? user)> ValidateAccessRequestCreateDtoAsync(AccessRequestCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var userDto = await _userRepo.GetUserByCurrentUser();
            if (userDto != null)
            {
                if (userDto.AccessRequestStatusCd == AccessRequestStatuses.Requested)
                {
                    errors.AddItem("entity", "Your access request is pending");
                    return (errors, userDto);
                }

                if (userDto.AccessRequestStatusCd == AccessRequestStatuses.Approved && !userDto.IsEnabled)
                {
                    errors.AddItem("entity", "Your access has been disabled");
                    return (errors, userDto);
                }

                if (userDto.AccessRequestStatusCd == AccessRequestStatuses.Approved)
                {
                    errors.AddItem("entity", "Your access has been already approved");
                    return (errors, userDto);
                }
            }

            var orgTypes = await _orgRepo.GetOrganizationTypesAsnc();

            if (!orgTypes.Any(x => x.OrganizationType == dto.OrganizationType))
            {
                errors.AddItem("organizationType", "Organization type is not valid");
            }

            if (dto.OrganizationName.IsEmpty())
            {
                errors.AddItem("organizationName", "Organization name is mandatory");
            }

            return (errors, userDto);
        }

        public async Task<Dictionary<string, List<string>>> DenyAccessRequest(AccessRequestDenyDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var user = await _userRepo.GetUserById(dto.UserIdentityId);

            if (user == null)
            {
                errors.AddItem("entity", $"Access request ({dto.UserIdentityId}) doesn't exist");
                return errors;
            }
            else
            {
                if (user.AccessRequestStatusCd  != AccessRequestStatuses.Requested) 
                {
                    errors.AddItem("entity", $"Unable to deny access request. The request is currently in status '{user.AccessRequestStatusCd}', which does not allow denial.");
                    return errors;
                }
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            await _userRepo.DenyAccessRequest(dto);

            using var transaction = _unitOfWork.BeginTransaction();

            _unitOfWork.Commit();

            if (user.EmailAddressDsc!.IsEmpty())
            {
                errors.AddItem("entity", $"The user doesn't have email address.");
                return errors;
            }

            var template = new AccessRequestDenial(_emailService)
            {
                AdminEmail = _currentUser.EmailAddress,
                To = new string[] { user.EmailAddressDsc! },
                Info = $"Denial email for {user.DisplayNm}"
            };

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent(),
                IsHostContactedExternally = false,
                IsSubmitterCcRequired = false,
                LgPhoneNo = null,
                UnreportedListingNo = null,
                HostEmailAddressDsc = null,
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = null,
                UnreportedListingUrl = null,
                LgStrBylawUrl = null,
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = user.UserIdentityId,
                InvolvedInOrganizationId = null
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            _unitOfWork.Commit();

            _unitOfWork.CommitTransaction(transaction);

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> ApproveAccessRequest(AccessRequestApproveDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var user = await _userRepo.GetUserById(dto.UserIdentityId);

            if (user == null)
            {
                errors.AddItem("entity", $"Access request ({dto.UserIdentityId}) doesn't exist");
                return errors;
            }
            else
            {
                if (user.AccessRequestStatusCd != AccessRequestStatuses.Requested)
                {
                    errors.AddItem("entity", $"Unable to approve access request. The request is currently in status '{user.AccessRequestStatusCd}', which does not allow approval.");
                    return errors;
                }
            }

            var org = await _orgRepo.GetOrganizationByIdAsync(dto.RepresentedByOrganizationId);

            if (org == null)
            {
                errors.AddItem("representedByOrganizationId", $"Organization ({dto.RepresentedByOrganizationId}) doesn't exist");
                return errors;
            }

            //only IDIR account can have BCGov org type
            if (user.IdentityProviderNm != StrDssIdProviders.Idir && org.OrganizationType == OrganizationTypes.BCGov)
            {
                errors.AddItem("representedByOrganizationId", $"Non IDIR account cannot be associated with {OrganizationTypes.BCGov} type organization");
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            var role = "";
            switch (org.OrganizationType)
            {
                case OrganizationTypes.BCGov:
                    role = Roles.CeuStaff; break;
                case OrganizationTypes.LG:
                    role = Roles.LgStaff; break;
                case OrganizationTypes.Platform:
                    role = Roles.PlatformStaff; break;
                default:
                    throw new Exception($"Unknow organization type {org.OrganizationType}");
            }

            await _userRepo.ApproveAccessRequest(dto, role);

            using var transaction = _unitOfWork.BeginTransaction();

            _unitOfWork.Commit();

            if (user.EmailAddressDsc!.IsEmpty())
            {
                errors.AddItem("entity", $"The user doesn't have email address.");
                return errors;
            }

            var template = new AccessRequestApproval(_emailService)
            {
                Link = GetHostUrl(),
                AdminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "dssadmin@gov.bc.ca",
                To = new string[] { user.EmailAddressDsc! },
                Info = $"Approval email for {user.DisplayNm}"
            };

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent(),
                IsHostContactedExternally = false,
                IsSubmitterCcRequired = false,
                LgPhoneNo = null,
                UnreportedListingNo = null,
                HostEmailAddressDsc = null,
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = null,
                UnreportedListingUrl = null,
                LgStrBylawUrl = null,
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = user.UserIdentityId,
                InvolvedInOrganizationId = null
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            _unitOfWork.Commit();

            _unitOfWork.CommitTransaction(transaction);

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> UpdateIsEnabled(UpdateIsEnabledDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var user = await _userRepo.GetUserById(dto.UserIdentityId);

            if (user == null)
            {
                errors.AddItem("entity", $"User ({dto.UserIdentityId}) doesn't exist");
                return errors;
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            await _userRepo.UpdateIsEnabled(dto);

            _unitOfWork.Commit();

            return errors;
        }

        public async Task<List<DropdownStrDto>> GetAccessRequestStatuses()
        {
            return await _userRepo.GetAccessRequestStatuses();
        }

        public async Task<Dictionary<string, List<string>>> AcceptTermsConditions()
        {
            var errors = new Dictionary<string, List<string>>();

            if (_currentUser.Id == 0)
            {
                errors.AddItem("entity", $"The user doesn't exist.");
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            await _userRepo.AcceptTermsConditions();

            _unitOfWork.Commit();

            return errors;
        }

        public async Task UpdateBceidUserInfo(long userId, string firstName, string LastName)
        {
            await _userRepo.UpdateUserNamesAsync(userId, firstName, LastName);

            _unitOfWork.Commit();
        }

        public async Task<UserDto?> GetUserByIdAsync(long userId)
        {
            return await _userRepo.GetUserById(userId);
        }

        public async Task<Dictionary<string, List<string>>> UpdateUserAsync(UserUpdateDto dto)
        {
            var errors = await ValidateUserUpdateDto(dto);
            if (errors.Count > 0) return errors;

            await _userRepo.UpdateUserAsync(dto);
            _unitOfWork.Commit();

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> ValidateUserUpdateDto(UserUpdateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var user = await _userRepo.GetUserById(dto.UserIdentityId);
            if (user == null)
            {
                errors.AddItem("userIdentityId", $"User ({dto.UserIdentityId}) doesn't exist");
                return errors;
            }

            await ValidateOrgAndRoles(dto, errors);

            return errors;
        }

        private async Task ValidateOrgAndRoles(IOrgRoles dto, Dictionary<string, List<string>> errors)
        {
            var org = await _orgRepo.GetOrganizationByIdAsync(dto.RepresentedByOrganizationId);
            if (org == null)
            {
                errors.AddItem("representedByOrganizationId", $"Organization ({dto.RepresentedByOrganizationId}) doesn't exist");
            }

            foreach (var roleCd in dto.RoleCds)
            {
                var exists = await _roleRepo.DoesRoleCdExist(roleCd);
                if (!exists)
                {
                    errors.AddItem("roleCd", $"Role ({roleCd}) doesn't exist");
                }
            }
        }

        public async Task<BceidAccount?> GetBceidUserInfo()
        {
            if (_currentUser.IdentityProviderNm == StrDssIdProviders.BceidBusiness)
            {
                try
                {
                    var (error, account) = await _bceid.GetBceidAccountCachedAsync(_currentUser.UserGuid, "", StrDssIdProviders.BceidBusiness, _currentUser.UserGuid, _currentUser.IdentityProviderNm);
                    return account;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"BCeID Web call failed - {ex.Message}", ex);
                    _logger.LogInformation("BCeID Web call failed");
                }
            }

            return null;
        }

        public async Task<(Dictionary<string, List<string>>, long)> CreateApsUserAsync(ApsUserCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrEmpty(dto.DisplayNm))
            {
                errors.AddItem("client_id", "Client ID is mandatory.");
            }

            await ValidateOrgAndRoles(dto, errors);

            if (!errors.Any() && await _userRepo.ApsUserExists(dto.DisplayNm))
            {
                errors.AddItem("client_id", $"The client ID {dto.DisplayNm} already exists.");
            }

            if (errors.Any()) return (errors, 0);

            var entity = await _userRepo.CreateApsUserAsync(dto);
            
            _unitOfWork.Commit();

            return (errors, entity.UserIdentityId);
        }
    }
}
