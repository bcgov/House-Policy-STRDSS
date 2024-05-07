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
        Task<DssRentalListingReport?> GetRentalListingReportAsync(long orgId, DateOnly reportPeriodYm);
        Task AddRentalListingReportAsync(DssRentalListingReport report);
        Task<DssRentalListing?> GetRentalListingAsync(long reportId, long offeringOrgId, string listingId);
        Task AddRentalListingAsync(DssRentalListing listing);
    }
    public class RentalListingReportRepository : RepositoryBase<DssRentalListingReport>, IRentalListingReportRepository
    {
        public RentalListingReportRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<DssRentalListingReport?> GetRentalListingReportAsync(long orgId, DateOnly reportPeriodYm)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.ProvidingOrganizationId == orgId && x.ReportPeriodYm == reportPeriodYm);
        }

        public async Task AddRentalListingReportAsync(DssRentalListingReport report)
        {
            await _dbSet.AddAsync(report);
        }

        public async Task<DssRentalListing?> GetRentalListingAsync(long reportId, long offeringOrgId, string listingId)
        {
            return await _dbContext.DssRentalListings
                .FirstOrDefaultAsync(x => x.IncludingRentalListingReportId == reportId && x.OfferingOrganizationId == offeringOrgId && x.PlatformListingNo == listingId);
        }
        public async Task AddRentalListingAsync(DssRentalListing listing)
        {
            await _dbContext.DssRentalListings.AddAsync(listing);
        }
    }
}
