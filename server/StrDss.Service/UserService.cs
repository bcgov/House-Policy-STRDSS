using AutoMapper;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.UserDtos;

namespace StrDss.Service
{
    public interface IUserService
    {
        Task<PagedDto<AccessRequestDto>> GetAccessRequestListAsync(string status, int pageSize, int pageNumber, string orderBy, string direction);
        Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid);
        Task<Dictionary<string, List<string>>> CreateAccessRequestAsync(AccessRequestCreateDto dto);
        Task<Dictionary<string, List<string>>> DenyAccessRequest(AccessRequestDenyDto dto);
    }
    public class UserService : ServiceBase, IUserService
    {
        private IUserRepository _userRepo;
        private IOrganizationRepository _orgRepo;

        public UserService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper,
            IUserRepository userRepo, IOrganizationRepository orgRepo)
            : base(currentUser, validator, unitOfWork, mapper)
        {
            _userRepo = userRepo;
            _orgRepo = orgRepo;
        }

        public async Task<PagedDto<AccessRequestDto>> GetAccessRequestListAsync(string status, int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _userRepo.GetAccessRequestListAsync(status, pageSize, pageNumber, orderBy, direction);
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
                    AccessRequestJustificationTxt = $"{dto.OrganiztionType}, {dto.OrganiztionName}",
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
                userDto.AccessRequestJustificationTxt = $"{dto.OrganiztionType}, {dto.OrganiztionName}";
                userDto.GivenNm = _currentUser.FirstName;
                userDto.FamilyNm = _currentUser.LastName;
                userDto.EmailAddressDsc = _currentUser.EmailAddress;
                userDto.BusinessNm = _currentUser.BusinessNm;
                userDto.RepresentedByOrganizationId = null;

                await _userRepo.UpdateUserAsync(userDto);
            }

            _unitOfWork.Commit();

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

            if (!orgTypes.Any(x => x.OrganizationType == dto.OrganiztionType))
            {
                errors.AddItem("organiztionType", "Organization type is not valid");
            }

            if (dto.OrganiztionName.IsEmpty())
            {
                errors.AddItem("organiztionName", "Organization name is mandatory");
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

            await _userRepo.DenyAccessRequest(dto);

            _unitOfWork.Commit();

            return errors;
        }
    }
}
