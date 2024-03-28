using AutoMapper;
using Microsoft.AspNetCore.Http;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.UserDtos;
using StrDss.Service.EmailTemplates;

namespace StrDss.Service
{
    public interface IUserService
    {
        Task<PagedDto<UserListtDto>> GetUserListAsync(string status, string search, long? orgranizationId, int pageSize, int pageNumber, string orderBy, string direction);
        Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid);
        Task<Dictionary<string, List<string>>> CreateAccessRequestAsync(AccessRequestCreateDto dto);
        Task<Dictionary<string, List<string>>> DenyAccessRequest(AccessRequestDenyDto dto);
        Task<Dictionary<string, List<string>>> ApproveAccessRequest(AccessRequestApproveDto dto);
        Task<Dictionary<string, List<string>>> UpdateIsEnabled(UpdateIsEnabledDto dto);
        Task<List<DropdownStrDto>> GetAccessRequestStatuses();
        Task<Dictionary<string, List<string>>> AcceptTermsConditions();
    }
    public class UserService : ServiceBase, IUserService
    {
        private IUserRepository _userRepo;
        private IOrganizationRepository _orgRepo;
        private IEmailMessageService _emailService;

        public UserService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepo, IOrganizationRepository orgRepo, IEmailMessageService emailService)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor)
        {
            _userRepo = userRepo;
            _orgRepo = orgRepo;
            _emailService = emailService;
        }

        public async Task<PagedDto<UserListtDto>> GetUserListAsync(string status, string search, long? orgranizationId, int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _userRepo.GetUserListAsync(status, search, orgranizationId, pageSize, pageNumber, orderBy, direction);
        }

        public async Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid)
        {
            return await _userRepo.GetUserAndPermissionsByGuidAsync(guid);
        }

        public async Task<Dictionary<string, List<string>>> CreateAccessRequestAsync(AccessRequestCreateDto dto)
        {
            var (errors, userDto) = await ValidateAccessRequestCreateDtoAsync(dto);

            if (errors.Count > 0)
            {
                return errors;
            }

            if (userDto == null)
            {
                var userCreateDto = new UserCreateDto
                {
                    UserGuid = _currentUser.UserGuid,
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

            _unitOfWork.Commit();

            var adminUsers = await _userRepo.GetAdminUsers();

            if (adminUsers.Count > 0)
            {
                var emails = adminUsers.Select(x => x.EmailAddressDsc);

                var template = new NewAccessRequest(_emailService)
                {
                    Link = GetHostUrl(),
                    To = emails,
                    Info = $"New Access Request email for {_currentUser.DisplayName}"
                };

                await template.SendEmail();
            }

            return errors;
        }

        private async Task<(Dictionary<string, List<string>> errors, UserDto user)> ValidateAccessRequestCreateDtoAsync(AccessRequestCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var userDto = await _userRepo.GetUserByGuid(_currentUser.UserGuid);
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

            _unitOfWork.Commit();

            if (user.EmailAddressDsc.IsEmpty())
            {
                errors.AddItem("entity", $"The user doesn't have email address.");
                return errors;
            }

            var template = new AccessRequestDenial(_emailService)
            {
                AdminEmail = _currentUser.EmailAddress,
                To = new string[] { user.EmailAddressDsc },
                Info = $"Denial email for {user.DisplayNm}"
            };

            await template.SendEmail();

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
                errors.AddItem("representedByOrganizationId", $"Not IDIR account cannot be associated with {OrganizationTypes.BCGov} type organization");
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

            _unitOfWork.Commit();

            if (user.EmailAddressDsc.IsEmpty())
            {
                errors.AddItem("entity", $"The user doesn't have email address.");
                return errors;
            }

            var template = new AccessRequestApproval(_emailService)
            {
                Link = GetHostUrl(),
                AdminEmail = _currentUser.EmailAddress,
                To = new string[] { user.EmailAddressDsc },
                Info = $"Approval email for {user.DisplayNm}"
            };

            await template.SendEmail();

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
    }
}
