using AutoMapper;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;

namespace StrDss.Data.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc();
    }
    public class OrganizationRepository : RepositoryBase<DssOrganization>, IOrganizationRepository
    {
        public OrganizationRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser)
            : base(dbContext, mapper, currentUser)
        {
        }

        public async Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc()
        {
            await Task.CompletedTask;

            var types = new List<OrganizationTypeDto>
            {
                new OrganizationTypeDto { Value = "BCGov", Label = "Compliace Enforcement Unit Staff" },
                new OrganizationTypeDto { Value = "LocalGov", Label = "Local Government Staff" },
            };

            return types;
        }
    }
}
