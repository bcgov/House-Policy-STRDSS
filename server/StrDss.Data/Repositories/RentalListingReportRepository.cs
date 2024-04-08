using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingReportRepository
    {
        Task AddRentalLisitngReportAsync(DssRentalListingReport report);
        Task<DssRentalListingReport?> GetReportToProcessAsync();
        Task<DssRentalListingLine?> GetListingLineAsync(long reportId, string orgCd, string listingNo);
    }
    public class RentalListingReportRepository : RepositoryBase<DssRentalListingReport>, IRentalListingReportRepository
    {
        public RentalListingReportRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddRentalLisitngReportAsync(DssRentalListingReport report)
        {
            await _dbSet.AddAsync(report);
        }

        public async Task<DssRentalListingReport?> GetReportToProcessAsync()
        {
            return await _dbSet
                .Include(x => x.ProvidingOrganization)
                .FirstOrDefaultAsync(x => x.IsProcessed == false);                
        }

        public async Task<DssRentalListingLine?> GetListingLineAsync(long reportId, string orgCd, string listingNo)
        {
            return await _dbContext
                .DssRentalListingLines
                .FirstOrDefaultAsync(x => x.IncludingRentalListingReportId == reportId && x.OrganizationCd == orgCd && x.PlatformListingNo == listingNo);
        }
    }
}
