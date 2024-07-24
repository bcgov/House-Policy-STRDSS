using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IPhysicalAddressRepository
    {
        Task AddPhysicalAddressAsync(DssPhysicalAddress address);
        Task<DssPhysicalAddress?> GetPhysicalAdderssFromMasterListingAsync(long offeringOrgId, string listingId, string address);
        Task<List<DssPhysicalAddress>> GetPhysicalAddressesToCleanUpAsync();
        Task ReloadAddressAsync(DssPhysicalAddress address);
        void ReplaceAddress(DssRentalListing listing, DssPhysicalAddress newAddress);
    }
    public class PhysicalAddressRepository : RepositoryBase<DssPhysicalAddress>, IPhysicalAddressRepository
    {
        public PhysicalAddressRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddPhysicalAddressAsync(DssPhysicalAddress address)
        {
            await _dbSet.AddAsync(address);
        }
        
        public async Task<DssPhysicalAddress?> GetPhysicalAdderssFromMasterListingAsync(long offeringOrgId, string listingId, string address)
        {
            var listing = await _dbContext.DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .FirstOrDefaultAsync(x => x.IncludingRentalListingReportId == null
                    && x.OfferingOrganizationId == offeringOrgId
                    && x.PlatformListingNo == listingId);

            if (listing == null) 
                return null;

            return listing.LocatingPhysicalAddress;
        }

        public async Task<List<DssPhysicalAddress>> GetPhysicalAddressesToCleanUpAsync()
        {
            return await _dbSet
                .Where(x => x.IsSystemProcessing == null 
                    && x.MatchScoreAmt < 90
                    && (x.IsMatchCorrected == null || (x.IsMatchCorrected.HasValue && x.IsMatchCorrected.Value == false))
                    && (x.IsMatchVerified == null || (x.IsMatchVerified.HasValue && x.IsMatchVerified.Value == false))
                 )
                .OrderBy(x => x.PhysicalAddressId)
                .Take(300)
                .ToListAsync();
        }
        public async Task ReloadAddressAsync(DssPhysicalAddress address)
        {
            await _dbContext.Entry(address).ReloadAsync();
        }

        public void ReplaceAddress(DssRentalListing listing, DssPhysicalAddress newAddress)
        {
            listing.LocatingPhysicalAddress = newAddress;

            _dbContext.Entry(listing.LocatingPhysicalAddress).State = EntityState.Detached;
            _dbContext.Entry(newAddress).State = EntityState.Added;
        }
    }
}
