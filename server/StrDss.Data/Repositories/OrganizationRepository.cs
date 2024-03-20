using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            var types = _mapper.Map<List<OrganizationTypeDto>>(await _dbContext.DssOrganizationTypes.AsNoTracking().ToListAsync());

            return types;
        }
    }
}
