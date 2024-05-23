using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingRepository
    {
        Task<PagedDto<RentalListingViewDto>> GetRentalListings(int pageSize, int pageNumber, string orderBy, string direction);
    }
    public class RentalListingRepository : RepositoryBase<DssRentalListingVw>, IRentalListingRepository
    {
        public RentalListingRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }
        public async Task<PagedDto<RentalListingViewDto>> GetRentalListings(int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbSet.AsNoTracking();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            var extraSort 
                = "AddressSort1ProvinceCd asc, AddressSort2LocalityNm asc, AddressSort3LocalityTypeDsc asc, AddressSort4StreetNm asc, AddressSort5StreetTypeDsc asc, AddressSort6StreetDirectionDsc asc, AddressSort7CivicNo asc, AddressSort8UnitNo asc";

            var listings = await Page<DssRentalListingVw, RentalListingViewDto>(query, pageSize, pageNumber, orderBy, direction, extraSort);

            //foreach (var listing in listings)
            //{
            //    listing.RentalListingContacts = 
            //        _mapper.Map<List<RentalListingContactDto>>(await 
            //            _dbContext.DssRentalListingContacts
            //                .Where(x => x.ContactedThroughRentalListingId == listing.RentalListingId)
            //                .ToListAsync());
            //}

            return listings;
        }
    }
}
