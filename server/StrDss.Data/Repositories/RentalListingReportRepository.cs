using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingReportRepository
    {
        Task<DssRentalListingReport?> GetRentalListingReportAsync(long orgId, DateOnly reportPeriodYm);
        Task AddRentalListingReportAsync(DssRentalListingReport report);
        Task<DssRentalListing?> GetRentalListingAsync(long reportId, long offeringOrgId, string listingId);
        Task AddRentalListingAsync(DssRentalListing listing);
        Task<DssRentalListing?> GetMasterListingAsync(long offeringOrgId, string listingId);
        void DeleteListingContacts(long listingId);
        Task<PagedDto<RentalUploadHistoryViewDto>> GetRentalListingUploadHistory(long? platformId, int pageSize, int pageNumber, string orderBy, string direction);
        Task<DssRentalUploadHistoryView?> GetRentalListingUpload(long deliveryId);
        Task UpdateIsCurrent(long providingPlatformId);
    }
    public class RentalListingReportRepository : RepositoryBase<DssRentalListingReport>, IRentalListingReportRepository
    {
        public RentalListingReportRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<DssRentalListingReport?> GetRentalListingReportAsync(long orgId, DateOnly reportPeriodYm)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.ProvidingOrganizationId == orgId && x.ReportPeriodYm == reportPeriodYm);
        }

        public async Task AddRentalListingReportAsync(DssRentalListingReport report)
        {
            await _dbSet.AddAsync(report);
        }

        public async Task<DssRentalListing?> GetRentalListingAsync(long reportId, long offeringOrgId, string listingId)
        {
            return await _dbContext.DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .FirstOrDefaultAsync(x => x.IncludingRentalListingReportId == reportId && x.OfferingOrganizationId == offeringOrgId && x.PlatformListingNo == listingId);
        }
        public async Task AddRentalListingAsync(DssRentalListing listing)
        {
            await _dbContext.DssRentalListings.AddAsync(listing);
        }

        public async Task<DssRentalListing?> GetMasterListingAsync(long offeringOrgId, string listingId)
        {
            return await _dbContext.DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .Include(x => x.DerivedFromRentalListing)
                    .ThenInclude(x => x.IncludingRentalListingReport)
                .FirstOrDefaultAsync(x => x.OfferingOrganizationId == offeringOrgId
                    && x.PlatformListingNo == listingId
                    && x.IncludingRentalListingReportId == null);
        }

        public void DeleteListingContacts(long listingId)
        {
            var contactsToDelete = _dbContext.DssRentalListingContacts
                .Where(c => c.ContactedThroughRentalListingId == listingId);

            _dbContext.DssRentalListingContacts.RemoveRange(contactsToDelete);
        }

        public async Task<PagedDto<RentalUploadHistoryViewDto>> GetRentalListingUploadHistory(long? platformId, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbContext.DssRentalUploadHistoryViews.AsNoTracking();

            if (_currentUser.OrganizationType == OrganizationTypes.Platform)
                query = query.Where(x => x.ProvidingOrganizationId == _currentUser.OrganizationId);

            if (platformId != null)
            {
                query = query.Where(x => x.ProvidingOrganizationId == platformId);
            }

            var history = await Page<DssRentalUploadHistoryView, RentalUploadHistoryViewDto>(query, pageSize, pageNumber, orderBy, direction);

            return history;
        }

        public async Task<DssRentalUploadHistoryView?> GetRentalListingUpload(long deliveryId)
        {
            return await _dbContext
                .DssRentalUploadHistoryViews
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UploadDeliveryId == deliveryId);
        }

        public async Task UpdateIsCurrent(long providingPlatformId)
        {
            var latestPeriodYm = _dbSet.AsNoTracking()
                .Where(x => x.ProvidingOrganizationId == providingPlatformId && x.DssRentalListings.Any())
                .Max(x => x.ReportPeriodYm);

            var masterListings = await _dbContext.DssRentalListings
                .Include(x => x.DerivedFromRentalListing)
                    .ThenInclude(x => x.IncludingRentalListingReport)
                .Where(x => x.DerivedFromRentalListing != null && x.DerivedFromRentalListing.IncludingRentalListingReport!.ProvidingOrganizationId == providingPlatformId)
                .ToArrayAsync();

            foreach (var listing in masterListings)
            {
                listing.IsCurrent = listing.DerivedFromRentalListing!.IncludingRentalListingReport!.ReportPeriodYm == latestPeriodYm;
            }
        }
    }
}
