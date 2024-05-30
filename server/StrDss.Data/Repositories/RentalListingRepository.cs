using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingRepository
    {
        Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicense, int pageSize, int pageNumber, string orderBy, string direction);
        Task<RentalListingViewDto?> GetRentalListing(long listingId);
    }
    public class RentalListingRepository : RepositoryBase<DssRentalListingVw>, IRentalListingRepository
    {
        public RentalListingRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }
        public async Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicense, 
            int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbSet.AsNoTracking();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            if (all != null && all.IsNotEmpty())
            {
                var allLower = all.ToLower();
                query = query.Where(x => (x.MatchAddressTxt != null && x.MatchAddressTxt.ToLower().Contains(allLower)) ||
                                         (x.PlatformListingUrl != null && x.PlatformListingUrl.ToLower().Contains(allLower)) ||
                                         (x.PlatformListingNo != null && x.PlatformListingNo.ToLower().Contains(allLower)) ||
                                         (x.ListingContactNamesTxt != null && x.ListingContactNamesTxt.ToLower().Contains(allLower)) ||
                                         (x.BusinessLicenceNo != null && x.BusinessLicenceNo.ToLower().Contains(allLower)));
            }

            if (address != null && address.IsNotEmpty())
            {
                var addressLower = address.ToLower();
                query = query.Where(x => x.MatchAddressTxt != null && x.MatchAddressTxt.ToLower().Contains(addressLower));
            }

            if (url != null && url.IsNotEmpty())
            {
                var urlLower = url.ToLower();
                query = query.Where(x => x.PlatformListingUrl != null && x.PlatformListingUrl.ToLower().Contains(urlLower));
            }

            if (listingId != null && listingId.IsNotEmpty())
            {
                var listingIdLower = listingId.ToLower();
                query = query.Where(x => x.PlatformListingNo != null && x.PlatformListingNo.ToLower().Contains(listingIdLower));
            }

            if (hostName != null && hostName.IsNotEmpty())
            {
                var hostNameLower = hostName.ToLower();
                query = query.Where(x => x.ListingContactNamesTxt != null && x.ListingContactNamesTxt.ToLower().Contains(hostNameLower));
            }

            if (businessLicense != null && businessLicense.IsNotEmpty())
            {
                var businessLicenseLower = businessLicense.ToLower();
                query = query.Where(x => x.BusinessLicenceNo != null && x.BusinessLicenceNo.ToLower().Contains(businessLicenseLower));
            }

            var extraSort 
                = "AddressSort1ProvinceCd asc, AddressSort2LocalityNm asc, AddressSort3LocalityTypeDsc asc, AddressSort4StreetNm asc, AddressSort5StreetTypeDsc asc, AddressSort6StreetDirectionDsc asc, AddressSort7CivicNo asc, AddressSort8UnitNo asc";

            var listings = await Page<DssRentalListingVw, RentalListingViewDto>(query, pageSize, pageNumber, orderBy, direction, extraSort);

            //foreach (var listing in listings.SourceList)
            //{
            //    listing.Hosts =
            //        _mapper.Map<List<RentalListingContactDto>>(await
            //            _dbContext.DssRentalListingContacts
            //                .Where(x => x.ContactedThroughRentalListingId == listing.RentalListingId)
            //                .ToListAsync());
            //}

            return listings;
        }

        public async Task<RentalListingViewDto?> GetRentalListing(long listingId)
        {
            var listing = _mapper.Map<RentalListingViewDto>(await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.RentalListingId == listingId));

            if (listing == null) return listing;

            if (_currentUser.OrganizationType == OrganizationTypes.LG && listing.ManagingOrganizationId != _currentUser.OrganizationId)
            {
                return null;
            }

            listing.Hosts = _mapper.Map<List<RentalListingContactDto>>(await
                _dbContext.DssRentalListingContacts.AsNoTracking()
                .Where(x => x.ContactedThroughRentalListingId == listing.RentalListingId)
                .ToListAsync());

            listing.ListingHistory = (await
                _dbContext.DssRentalListings.AsNoTracking()
                .Include(x => x.IncludingRentalListingReport)
                .Where(x => x.PlatformListingNo == listing.PlatformListingNo && x.OfferingOrganizationId == listing.OfferingOrganizationId && x.DerivedFromRentalListingId == null)
                .Select(x => new ListingHistoryDto {
                    ReportPeriodYM = x.IncludingRentalListingReport!.ReportPeriodYm!.ToString("yyyy-MM"),
                    NightsBookedQty = x.NightsBookedQty,
                    SeparateReservationsQty = x.SeparateReservationsQty
                }).ToListAsync())
                .OrderByDescending(x => x.ReportPeriodYM)
                .ToList();

            listing.ActionHistory = (await
                _dbContext.DssEmailMessages.AsNoTracking()
                    .Where(x => x.ConcernedWithRentalListingId == listing.RentalListingId)
                    .OrderByDescending(x => x.UpdDtm)
                    .Select(x => new ActionHistoryDto
                    {
                        Action = x.EmailMessageType,
                        Date = DateUtils.ConvertUtcToPacificTime((DateTime)x.UpdDtm!),
                        UserGuid = x.UpdUserGuid
                    })
                    .ToListAsync());

            foreach(var action in listing.ActionHistory)
            {
                var user = await _dbContext.DssUserIdentities.FirstOrDefaultAsync(x => x.UserGuid == x.UpdUserGuid);

                if (user == null) continue;

                action.User = CommonUtils.GetFullName(user!.GivenNm ?? "", user!.FamilyNm ?? "");
            }

            return listing;
        }
    }
}
