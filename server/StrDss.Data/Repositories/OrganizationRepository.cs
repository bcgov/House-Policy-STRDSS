using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Npgsql;
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
        Task<long?> GetContainingOrganizationId(Point point);
    }
    public class OrganizationRepository : RepositoryBase<DssOrganization>, IOrganizationRepository
    {
        private IConfiguration _config;

        public OrganizationRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger,
            IConfiguration config)
            : base(dbContext, mapper, currentUser, logger)
        {
            _config = config;
        }

        public async Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc()
        {
            var types = _mapper.Map<List<OrganizationTypeDto>>(
                await _dbContext.DssOrganizationTypes.AsNoTracking()
                .Where(x => x.OrganizationType != OrganizationTypes.LGSub)
                .ToListAsync());

            return types;
        }

        public async Task<List<OrganizationDto>> GetOrganizationsAsync(string? type)
        {
            var query = _dbSet.AsNoTracking();

            if (type != null && type != "All")
            {
                query = query.Where(x => x.OrganizationType == type);
            }
            else
            {
                query = query.Where(x => x.OrganizationType != OrganizationTypes.LGSub);
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
                .FirstOrDefaultAsync(x => x.OrganizationCd == orgCd.ToUpper());

            return _mapper.Map<OrganizationDto>(org);
        }

        public async Task<long?> GetContainingOrganizationId(Point point)
        {
            var dbHost = _config.GetValue<string>("DB_HOST");
            var dbName = _config.GetValue<string>("DB_NAME");
            var dbUser = _config.GetValue<string>("DB_USER");
            var dbPass = _config.GetValue<string>("DB_PASS");
            var dbPort = _config.GetValue<string>("DB_PORT");

            var connString = $"Host={dbHost};Username={dbUser};Password={dbPass};Database={dbName};Port={dbPort};";

            using var connection = new NpgsqlConnection(connString);

            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT dss_containing_organization_id(@p_point)", connection);

            command.Parameters.AddWithValue("@p_point", point);

            var result = await command.ExecuteScalarAsync();

            return result == DBNull.Value ? null : (long)result!;
        }
    }
}
