using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;

namespace StrDss.Service
{
    public interface IOrganizationService
    {
        Task<List<LocalGovTypeDto>> GetLocalGovTypesAsync();
        Task<List<EconomicRegionDto>> GetEconomicRegionsAsync();
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc();
        Task<List<OrganizationDto>> GetOrganizationsAsync(string? type);
        Task<List<DropdownNumDto>> GetOrganizationsDropdownAsync(string? type);
        Task<OrganizationDto?> GetOrganizationByIdAsync(long id);
        Task<long?> GetContainingOrganizationId(Point point);
        Task<StrRequirementsDto?> GetStrRequirements(double longitude, double latitude);
        Task<PagedDto<PlatformViewDto>> GetPlatforms(int pageSize, int pageNumber, string orderBy, string direction);
        Task<PlatformViewDto?> GetPlatform(long id);
        Task<List<PlatformTypeDto>> GetPlatformTypesAsync();
        Task<(Dictionary<string, List<string>>, long)> CreatePlatformAsync(PlatformCreateDto dto);
        Task<Dictionary<string, List<string>>> UpdatePlatformAsync(PlatformUpdateDto dto);
        Task<(Dictionary<string, List<string>>, long)> CreatePlatformSubAsync(PlatformSubCreateDto dto);
        Task<Dictionary<string, List<string>>> UpdatePlatformSubAsync(PlatformSubUpdateDto dto);
        Task<PagedDto<LocalGovViewDto>> GetJurisdictions(int pageSize, int pageNumber, string orderBy, string direction);
        Task<LocalGovViewDto?> GetLocalGov(long id);
        Task<Dictionary<string, List<string>>> UpdateLocalGovAsync(LocalGovUpdateDto dto);
    }
    public class OrganizationService : ServiceBase, IOrganizationService
    {
        private IOrganizationRepository _orgRepo;
        private ICodeSetRepository _codeSetRepo;

        public OrganizationService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IOrganizationRepository orgRepo, ICodeSetRepository codeSetRep) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _orgRepo = orgRepo;
            _codeSetRepo = codeSetRep;
        }
        public async Task<List<LocalGovTypeDto>> GetLocalGovTypesAsync()
        {
            return await _orgRepo.GetLocalGovTypesAsync();
        }
        public async Task<List<EconomicRegionDto>> GetEconomicRegionsAsync()
        {
            return await _orgRepo.GetEconomicRegionsAsync();
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

        public async Task<List<PlatformTypeDto>> GetPlatformTypesAsync()
        {
            return await _orgRepo.GetPlatformTypesAsync();
        }

        public async Task<(Dictionary<string, List<string>>, long)> CreatePlatformAsync(PlatformCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            await ValidatePlatformCreateDto<PlatformCreateDto>(dto, errors);

            if (errors.Any())
            {
                return (errors, 0);
            }

            var entity = await _orgRepo.CreatePlatformAsync(dto);

            _unitOfWork.Commit();

            return (errors, entity.OrganizationId);
        }

        private async Task<Dictionary<string, List<string>>> ValidatePlatformCreateDto<T>(IPlatformCreateDto dto, Dictionary<string, List<string>> errors)
            where T : class, IPlatformCreateDto
        {
            if (!_validator.CommonCodes.Any())
            {
                _validator.CommonCodes = await _codeSetRepo.LoadCodeSetAsync();
            }

            _validator.Validate(Entities.Platform, (T)dto, errors);

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

            await ValidatePlatformUpdateDto<PlatformUpdateDto>(dto, errors);

            if (errors.Any())
            {
                return errors;
            }

            await _orgRepo.UpdatePlatformAsync(dto);

            _unitOfWork.Commit();

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidatePlatformUpdateDto<T>(IPlatformUpdateDto dto, Dictionary<string, List<string>> errors)
            where T : class, IPlatformUpdateDto
        {
            var platformDto = await _orgRepo.GetPlatform(dto.OrganizationId);

            if (platformDto == null)
            {
                errors.AddItem("OrganizationId", $"Platform with ID {dto.OrganizationId} does not exist");
                return errors;
            }

            if (!_validator.CommonCodes.Any())
            {
                _validator.CommonCodes = await _codeSetRepo.LoadCodeSetAsync();
            }

            _validator.Validate(Entities.Platform, (T)dto, errors);

            return errors;
        }

        public async Task<(Dictionary<string, List<string>>, long)> CreatePlatformSubAsync(PlatformSubCreateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            await ValidateParentPlatform(errors, dto.ManagingOrganizationId);

            if (errors.Any())
            {
                return (errors, 0);
            }

            await ValidatePlatformCreateDto<PlatformSubCreateDto>(dto, errors);

            if (errors.Any())
            {
                return (errors, 0);
            }

            var entity = await _orgRepo.CreatePlatformSubAsync(dto);

            _unitOfWork.Commit();

            return (errors, entity.OrganizationId);
        }

        private async Task<Dictionary<string, List<string>>> ValidateParentPlatform(Dictionary<string, List<string>> errors, long parentPlatformId)
        {
            var parentPlatform = await _orgRepo.GetOrganizationByIdAsync(parentPlatformId);

            if (parentPlatform == null)
            {
                errors.AddItem("ManagingOrganizationId", $"No parent platform found with ID {parentPlatformId}.");
                return errors;
            }

            if (parentPlatform.OrganizationType != OrganizationTypes.Platform)
            {
                errors.AddItem("ManagingOrganizationId", $"The organization with ID {parentPlatformId} is not a platform (current type: {parentPlatform.OrganizationType}).");
            }

            if (parentPlatform.ManagingOrganizationId != null)
            {
                errors.AddItem("ManagingOrganizationId", $"The parent platform with ID {parentPlatformId} is a subsidiary and a subsidiary cannot be added to a subsidiary platform.");
            }

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> UpdatePlatformSubAsync(PlatformSubUpdateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            await ValidateParentPlatform(errors, dto.ManagingOrganizationId);

            if (errors.Any())
            {
                return errors;
            }

            await ValidatePlatformUpdateDto<PlatformSubUpdateDto>(dto, errors);

            if (errors.Any())
            {
                return errors;
            }

            await _orgRepo.UpdatePlatformSubAsync(dto);

            _unitOfWork.Commit();

            return errors;
        }

        public async Task<PagedDto<LocalGovViewDto>> GetJurisdictions(int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _orgRepo.GetJurisdictions(pageSize, pageNumber, orderBy, direction);
        }
        public async Task<LocalGovViewDto?> GetLocalGov(long id)
        {
            return await _orgRepo.GetLocalGov(id);
        }

        public async Task<Dictionary<string, List<string>>> UpdateLocalGovAsync(LocalGovUpdateDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            await ValidateLocalGovUpdateDto(dto, errors);

            if (errors.Any())
            {
                return errors;
            }

            await _orgRepo.UpdateLocalGovAsync(dto);

            _unitOfWork.Commit();

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateLocalGovUpdateDto(LocalGovUpdateDto dto, Dictionary<string, List<string>> errors)
        {
            var localGov = await _orgRepo.GetLocalGov(dto.OrganizationId);

            if (localGov == null)
            {
                errors.AddItem("OrganizationId", $"Local government with ID {dto.OrganizationId} does not exist");
                return errors;
            }

            if (!_validator.CommonCodes.Any())
            {
                _validator.CommonCodes = await _codeSetRepo.LoadCodeSetAsync();
            }

            _validator.Validate(Entities.LocalGov, dto, errors);

            return errors;
        }
    }
}
