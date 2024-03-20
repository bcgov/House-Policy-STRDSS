using AutoMapper;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;

namespace StrDss.Service
{
    public interface IOrganizationService
    {
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc();
    }
    public class OrganizationService : ServiceBase, IOrganizationService
    {
        private IOrganizationRepository _orgRepo;

        public OrganizationService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper,
            IOrganizationRepository orgRepo) 
            : base(currentUser, validator, unitOfWork, mapper)
        {
            _orgRepo = orgRepo;
        }

        public async Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc()
        {
            return await _orgRepo.GetOrganizationTypesAsnc();
        }
    }
}
