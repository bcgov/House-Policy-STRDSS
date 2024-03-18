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
        Task<Dictionary<string, List<string>>> CreateAccessRequest(AccessRequestCreateDto dto);
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
            return await _userRepo.GetUserByGuidAsync(guid);
        }

        public async Task<Dictionary<string, List<string>>> CreateAccessRequest(AccessRequestCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var orgTypes = await _orgRepo.GetOrganizationTypesAsnc();

            if (!orgTypes.Any(x => x.Value == dto.OrganiztionType))
            {
                errors.AddItem("organiztionType", "Organization type is not valid");
            }

            if (dto.OrganiztionName.IsEmpty())
            {
                errors.AddItem("organiztionName", "Organization name is mandatory");
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            var userCreateDto = new UserCreateDto
            {
                UserGuid = _currentUser.UserGuid,
                DisplayNm = _currentUser.DisplayName,
                IdentityProviderNm = _currentUser.IdentityProviderNm,
                IsEnabled = false,
                AccessRequestStatusDsc = "Pending",
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

            _unitOfWork.Commit();

            return errors;
        }
    }
}
