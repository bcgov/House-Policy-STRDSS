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

            if (listing.LocatingPhysicalAddress!.OriginalAddressTxt.ToLower().Trim() != address.ToLower().Trim()) 
                return null;

            return listing.LocatingPhysicalAddress;
        }

        public async Task<List<DssPhysicalAddress>> GetPhysicalAddressesToCleanUpAsync()
        {
            return await _dbSet
                .Where(x => x.IsSystemProcessing == null && x.MatchScoreAmt < 90)
                .Take(300)
                .ToListAsync();
        }
    }
}
