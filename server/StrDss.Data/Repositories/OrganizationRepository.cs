using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;

namespace StrDss.Data.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc();
        Task<List<OrganizationDto>> GetOrganizationsAsync(string? type);
        Task<OrganizationDto?> GetOrganizationByIdAsync(long orgId);
        Task<List<string>> GetManagingOrgCdsAsync(long orgId);
        Task<OrganizationDto> GetOrganizationByOrgCdAsync(string orgCd);
    }
    public class OrganizationRepository : RepositoryBase<DssOrganization>, IOrganizationRepository
    {
        public OrganizationRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger)
            : base(dbContext, mapper, currentUser, logger)
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

        public async Task<OrganizationDto?> GetOrganizationByIdAsync(long orgId)
        {
            var org = await _dbSet.AsNoTracking()
                .Include(x => x.DssOrganizationContactPeople)
                .FirstOrDefaultAsync(x => x.OrganizationId == orgId);

            return _mapper.Map<OrganizationDto>(org);
        }

        public async Task<List<string>> GetManagingOrgCdsAsync(long orgId)
        {
            var orgCds = await _dbSet.AsNoTracking()
                .Where(x => x.OrganizationId == orgId || x.ManagingOrganizationId == orgId)
                .Select(x => x.OrganizationCd)
                .ToListAsync();

            return orgCds;
        }

        public async Task<OrganizationDto> GetOrganizationByOrgCdAsync(string orgCd)
        {
            var org = await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganizationCd == orgCd);

            return _mapper.Map<OrganizationDto>(org);
        }
    }
}
