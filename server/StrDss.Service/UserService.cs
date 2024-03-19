using AutoMapper;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.UserDtos;
using System.Runtime.InteropServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StrDss.Service
{
    public interface IUserService
    {
        Task<PagedDto<AccessRequestDto>> GetAccessRequestListAsync(string status, int pageSize, int pageNumber, string orderBy, string direction);
        Task<(UserDto? user, List<string> permissions)> GetUserByGuidAsync(Guid guid);
        Task<Dictionary<string, List<string>>> CreateAccessRequestAsync(AccessRequestCreateDto dto);
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
                    AccessRequestStatusDsc = AccessRequestStatuses.Requested,
                    AccessRequestDtm = DateTime.UtcNow,
                    AccessRequestJustificationTxt = $"{dto.OrganiztionType}, {dto.OrganiztionName}",
                    GivenNm = _currentUser.FirstName,
                    FamilyNm = _currentUser.LastName,
                    EmailAddressDsc = _currentUser.EmailAddress,
                    BusinessNm = _currentUser.BusinessNm,
                    UpdDtm = DateTime.UtcNow,
                    UpdUserGuid = _currentUser.UserGuid,
                };

                await _userRepo.CreateUserAsync(userCreateDto);
            }
            else
            {
                userDto.DisplayNm = _currentUser.DisplayName;
                userDto.IdentityProviderNm = _currentUser.IdentityProviderNm;
                userDto.IsEnabled = false;
                userDto.AccessRequestStatusDsc = AccessRequestStatuses.Requested;
                userDto.AccessRequestDtm = DateTime.UtcNow;
                userDto.AccessRequestJustificationTxt = $"{dto.OrganiztionType}, {dto.OrganiztionName}";
                userDto.GivenNm = _currentUser.FirstName;
                userDto.FamilyNm = _currentUser.LastName;
                userDto.EmailAddressDsc = _currentUser.EmailAddress;
                userDto.BusinessNm = _currentUser.BusinessNm;
                userDto.UpdDtm = DateTime.UtcNow;
                userDto.UpdUserGuid = _currentUser.UserGuid;
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
                if (userDto.AccessRequestStatusDsc == AccessRequestStatuses.Requested)
                {
                    errors.AddItem("entity", "Your access request is pending");
                    return (errors, userDto);
                }

                if (userDto.AccessRequestStatusDsc == AccessRequestStatuses.Approved && !userDto.IsEnabled)
                {
                    errors.AddItem("entity", "Your access has been disabled");
                    return (errors, userDto);
                }
            }

            var orgTypes = await _orgRepo.GetOrganizationTypesAsnc();

            if (!orgTypes.Any(x => x.Value == dto.OrganiztionType))
            {
                errors.AddItem("organiztionType", "Organization type is not valid");
            }

            if (dto.OrganiztionName.IsEmpty())
            {
                errors.AddItem("organiztionName", "Organization name is mandatory");
            }

            return (errors, userDto);
        }
    }
}
