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
        Task<List<OrganizationDto>> GetOrganizationsAsync(string? type);
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

        public async Task<List<OrganizationDto>> GetOrganizationsAsync(string? type)
        {
            var query = _dbSet.AsNoTracking();

            if (type != null && type != "All") 
            { 
                query = query.Where(x => x.OrganizationType == type);
            }

            query = query.Include(x => x.DssOrganizationContactPeople);

            return _mapper.Map<List<OrganizationDto>>(await query.ToListAsync());
        }
    }
}
