using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using StrDss.Model.UserDtos;
using System.Threading.Tasks;

namespace StrDss.Service
{
    public interface IOrganizationService
    {
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc();
        Task<List<OrganizationDto>> GetOrganizationsAsync(string? type);
        Task<List<DropdownNumDto>> GetOrganizationsDropdownAsync(string? type);
        Task<OrganizationDto?> GetOrganizationByIdAsync(long id);
        Task<long?> GetContainingOrganizationId(Point point);
        Task<StrRequirementsDto?> GetStrRequirements(double longitude, double latitude);
        Task<PagedDto<PlatformViewDto>> GetPlatforms(int pageSize, int pageNumber, string orderBy, string direction);
        Task<PlatformViewDto?> GetPlatform(long id);
        Task<(Dictionary<string, List<string>>, long)> CreatePlatformAsync(PlatformCreateDto dto);
        Task<Dictionary<string, List<string>>> UpdatePlatformAsync(PlatformUpdateDto dto);
    }
    public class OrganizationService : ServiceBase, IOrganizationService
    {
        private IOrganizationRepository _orgRepo;

        public OrganizationService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IOrganizationRepository orgRepo) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _orgRepo = orgRepo;
        }

        public async Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc()
        {
            return await _orgRepo.GetOrganizationTypesAsnc();
        }

        public async Task<List<OrganizationDto>> GetOrganizationsAsync(string? type)
        {
            return await _orgRepo.GetOrganizationsAsync(type);
        }

        public async Task<List<DropdownNumDto>> GetOrganizationsDropdownAsync(string? type)
        {
            var orgs = await _orgRepo.GetOrganizationsAsync(type);
            return orgs.Select(x => new DropdownNumDto { Id = x.OrganizationId, Description = x.OrganizationNm }).ToList();
        }

        public async Task<OrganizationDto?> GetOrganizationByIdAsync(long id)
        {
            return await _orgRepo.GetOrganizationByIdAsync(id);
        }
        public async Task<long?> GetContainingOrganizationId(Point point)
        {
            return await _orgRepo.GetContainingOrganizationId(point);
        }

        public async Task<StrRequirementsDto?> GetStrRequirements(double longitude, double latitude)
        {
            return await _orgRepo.GetStrRequirements(longitude, latitude);
        }

        public async Task<PagedDto<PlatformViewDto>> GetPlatforms(int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _orgRepo.GetPlatforms(pageSize, pageNumber, orderBy, direction);
        }

        public async Task<PlatformViewDto?> GetPlatform(long id)
        {
            return await _orgRepo.GetPlatform(id);
        }

        public async Task<(Dictionary<string, List<string>>, long)> CreatePlatformAsync(PlatformCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            await ValidatePlatformCreateDto(dto, errors);

            if (errors.Any())
            {
                return (errors, 0);
            }

            var entity = await _orgRepo.CreatePlatformAsync(dto);

            _unitOfWork.Commit();

            return (errors, entity.OrganizationId);            
        }

        private async Task<Dictionary<string, List<string>>> ValidatePlatformCreateDto(PlatformCreateDto dto, Dictionary<string, List<string>> errors)
        {
            _validator.Validate(Entities.Platform, dto, errors);

            if (errors.Any())
            {
                return errors;
            }

            if (await _orgRepo.DoesOrgCdExist(dto.OrganizationCd))
            {
                errors.AddItem("OrganizationCd", $"Organization Code {dto.OrganizationCd} already exists");
            }

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> UpdatePlatformAsync(PlatformUpdateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var platformDto = await _orgRepo.GetPlatform(dto.OrganizationId);

            if (platformDto == null)
            {
                errors.AddItem("OrganizationId", $"Platform with ID {dto.OrganizationId} does not exist");
                return errors;
            }

            await ValidatePlatformUpdateDto(dto, errors);

            if (errors.Any())
            {
                return errors;
            }

            await _orgRepo.UpdatePlatformAsync(dto);

            _unitOfWork.Commit();

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidatePlatformUpdateDto(PlatformUpdateDto dto, Dictionary<string, List<string>> errors)
        {
            await Task.CompletedTask;

            _validator.Validate(Entities.Platform, dto, errors);

            return errors;
        }
    }
}
