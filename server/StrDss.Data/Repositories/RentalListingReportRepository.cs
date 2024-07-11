using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using StrDss.Model.RentalReportDtos;
using System.Diagnostics;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingReportRepository
    {
        Task<DssRentalListingReport?> GetRentalListingReportAsync(long orgId, DateOnly reportPeriodYm);
        Task AddRentalListingReportAsync(DssRentalListingReport report);
        Task<DssRentalListing?> GetRentalListingAsync(long reportId, long offeringOrgId, string rentalListingId);
        Task AddRentalListingAsync(DssRentalListing listing);
        Task<DssRentalListing?> GetMasterListingAsync(long offeringOrgId, string rentalListingId);
        void DeleteListingContacts(long rentalListingId);
        Task<PagedDto<RentalUploadHistoryViewDto>> GetRentalListingUploadHistory(long? platformId, int pageSize, int pageNumber, string orderBy, string direction);
        Task<DssRentalUploadHistoryView?> GetRentalListingUpload(long deliveryId);
        Task UpdateInactiveListings(long providingPlatformId);
        Task UpdateListingStatus(long providingPlatformId, long rentalListingId);
        Task<int> GetTotalNumberOfUploadLines(long uploadId);
        Task<(short NightsBookedQty, short SeparateReservationsQty)> GetYtdValuesOfListingAsync(DateOnly reportPeriodYm, long offeringOrgId, string listingId);
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
            var stopwatch = Stopwatch.StartNew();

            var listing = await _dbContext.DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .FirstOrDefaultAsync(x => x.IncludingRentalListingReportId == reportId && x.OfferingOrganizationId == offeringOrgId && x.PlatformListingNo == listingId);

            stopwatch.Stop();
            _logger.LogDebug($"GetRentalListingAsync = {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

            return listing;
        }
        public async Task AddRentalListingAsync(DssRentalListing listing)
        {
            await _dbContext.DssRentalListings.AddAsync(listing);
        }

        public async Task<DssRentalListing?> GetMasterListingAsync(long offeringOrgId, string listingId)
        {
            var stopwatch = Stopwatch.StartNew();

            var listing = await _dbContext.DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .Include(x => x.DerivedFromRentalListing)
                    .ThenInclude(x => x.IncludingRentalListingReport)
                .FirstOrDefaultAsync(x => x.OfferingOrganizationId == offeringOrgId
                    && x.PlatformListingNo == listingId
                    && x.IncludingRentalListingReportId == null);

            stopwatch.Stop();
            _logger.LogDebug($"GetRentalListingAsync = {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

            return listing;
        }

        public async Task<(short NightsBookedQty, short SeparateReservationsQty)> GetYtdValuesOfListingAsync(DateOnly reportPeriodYm, long offeringOrgId, string listingId)
        {
            var startPeriodYm = new DateOnly(reportPeriodYm.Year, 1, 1);
            var ytdValues = await _dbContext.DssRentalListings.AsNoTracking()
                .Where(x => x.OfferingOrganizationId == offeringOrgId
                    && x.PlatformListingNo == listingId
                    && x.IncludingRentalListingReport.ReportPeriodYm >= startPeriodYm
                    && x.IncludingRentalListingReport.ReportPeriodYm <= reportPeriodYm)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    NightsBookedQty = (short)g.Sum(x => (short)x.NightsBookedQty),
                    SeparateReservationsQty = (short)g.Sum(x => (short)x.SeparateReservationsQty)
                })
                .FirstOrDefaultAsync();

            return ytdValues != null
                ? (ytdValues.NightsBookedQty, ytdValues.SeparateReservationsQty)
                : ((short)0, (short)0);
        }

        public void DeleteListingContacts(long rentalListingId)
        {
            var stopwatch = Stopwatch.StartNew();

            var contactsToDelete = _dbContext.DssRentalListingContacts
                .Where(c => c.ContactedThroughRentalListingId == rentalListingId);

            _dbContext.DssRentalListingContacts.RemoveRange(contactsToDelete);

            stopwatch.Stop();
            _logger.LogDebug($"GetRentalListingAsync = {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
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
            var stopwatch = Stopwatch.StartNew();

            var history = await _dbContext
                .DssRentalUploadHistoryViews
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UploadDeliveryId == deliveryId);

            stopwatch.Stop();
            _logger.LogDebug($"GetRentalListingUpload = {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

            return history;
        }

        public async Task UpdateInactiveListings(long providingPlatformId)
        {
            var stopwatch = Stopwatch.StartNew();

            var reports = await _dbSet.AsNoTracking()
                .Where(x => x.ProvidingOrganizationId == providingPlatformId && x.DssRentalListings.Any())
                .ToListAsync();

            var latestPeriodYm = reports.Max(x => x.ReportPeriodYm);
            var lastestReport = reports.First(x => x.ReportPeriodYm == latestPeriodYm);

            var inactiveListingIds = await _dbContext.DssRentalListings
                .Where(x => x.DerivedFromRentalListing != null &&
                    x.IsActive == true &&
                    x.DerivedFromRentalListing.IncludingRentalListingReport!.ProvidingOrganizationId == providingPlatformId &&
                    x.DerivedFromRentalListing.IncludingRentalListingReport!.RentalListingReportId != lastestReport.RentalListingReportId)
                .Select(x => x.RentalListingId)
                .ToArrayAsync();

            foreach (var listingId in inactiveListingIds)
            {
                await _dbContext.Database.ExecuteSqlAsync(
                    $"UPDATE dss_rental_listing SET is_active = false, is_new = false WHERE rental_listing_id = {listingId}");
            }

            stopwatch.Stop();

            _logger.LogDebug($"UpdateInactiveListings = {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
        }

        public async Task UpdateListingStatus(long providingPlatformId, long rentalListingId)
        {
            var reports = await _dbSet.AsNoTracking()
                .Where(x => x.ProvidingOrganizationId == providingPlatformId && x.DssRentalListings.Any())
                .ToListAsync();

            var listing = await _dbContext.DssRentalListings
                .Include(x => x.DerivedFromRentalListing)
                    .ThenInclude(x => x.IncludingRentalListingReport)
                .FirstAsync(x => x.RentalListingId == rentalListingId);
    
            if (reports.Count == 0)
            {
                listing.IsActive = true;
                listing.IsNew = true;
            }
            else
            {
                var latestPeriodYm = reports.Max(x => x.ReportPeriodYm);

                var isActive = listing.DerivedFromRentalListing!.IncludingRentalListingReport!.ReportPeriodYm == latestPeriodYm;
                var count = _dbContext.DssRentalListings
                    .Count(x => x.OfferingOrganizationId == listing.OfferingOrganizationId && x.PlatformListingNo == listing.PlatformListingNo && x.DerivedFromRentalListing == null);

                listing.IsActive = isActive;
                listing.IsNew = isActive && count == 1;
            }
        }

        public async Task<int> GetTotalNumberOfUploadLines(long uploadId)
        {
            return await _dbContext.DssUploadLines.Where(x => x.IncludingUploadDeliveryId == uploadId).CountAsync();
        }
    }
}
