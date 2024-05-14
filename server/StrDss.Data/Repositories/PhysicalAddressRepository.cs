using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IPhysicalAddressRepository
    {
        Task AddPhysicalAddressAsync(DssPhysicalAddress address);
        Task<DssPhysicalAddress?> GetPhysicalAdderssFromMasterListingAsync(long offeringOrgId, string listingId, string address);
        Task InsertTestAddress();
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

        public async Task InsertTestAddress()
        {
            var address = new DssPhysicalAddress
            {
                OriginalAddressTxt = "Test",
                MatchAddressTxt = "Test",
                MatchScoreAmt = 30,
                UpdDtm = DateTime.UtcNow,
                LocationGeometry = new Point(-123.3709161, 48.4177006) { SRID = 4326 }
            };

            await _dbContext.DssPhysicalAddresses.AddAsync(address);
            _dbContext.SaveChanges();
        }
    }
}
