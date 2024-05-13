using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;

namespace StrDss.Service
{
    public interface IOrganizationService
    {
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc();
        Task<List<OrganizationDto>> GetOrganizationsAsync(string? type);
        Task<List<DropdownNumDto>> GetOrganizationsDropdownAsync(string? type);
        Task<OrganizationDto?> GetOrganizationByIdAsync(long id);
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

    }
}
