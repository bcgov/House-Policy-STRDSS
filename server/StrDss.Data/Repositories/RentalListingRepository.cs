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
        Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicense, int pageSize, int pageNumber, string orderBy, string direction);
        Task<RentalListingViewDto?> GetRentalListing(long listingId);
        Task<RentalListingForTakedownNotice?> GetRentalLisgingForTakedownNotice(long listingId);
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

            if (listing.LastActionDtm != null)
            {
                listing.LastActionDtm = DateUtils.ConvertUtcToPacificTime((DateTime)listing.LastActionDtm!);
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
                    .Include(x => x.EmailMessageTypeNavigation)
                    .Where(x => x.ConcernedWithRentalListingId == listing.RentalListingId)
                    .OrderByDescending(x => x.MessageDeliveryDtm)
                    .Select(x => new ActionHistoryDto
                    {
                        Action = x.EmailMessageTypeNavigation.EmailMessageTypeNm,
                        Date = DateUtils.ConvertUtcToPacificTime((DateTime)x.MessageDeliveryDtm!),
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

        public async Task<RentalListingForTakedownNotice?> GetRentalLisgingForTakedownNotice(long listingId)
        {
            var listing = await GetRentalListingAsync(listingId);
            if (listing == null) return null;

            listing.HostEmails = await GetHostEmailsAsync(listingId);
            (listing.PlatformEmails, listing.ProvidingPlatformId) = await GetPlatformEmailsAsync(listing.OfferingPlatformId);

            return listing;
        }

        private async Task<RentalListingForTakedownNotice?> GetRentalListingAsync(long listingId)
        {
            return await _dbContext.DssRentalListings
                .Select(x => new RentalListingForTakedownNotice
                {
                    RentalListingId = listingId,
                    PlatformListingNo = x.PlatformListingNo,
                    PlatformListingUrl = x.PlatformListingUrl,
                    OrganizationCd = x.OfferingOrganization.OrganizationCd,
                    OfferingPlatformId = x.OfferingOrganizationId
                })
                .FirstOrDefaultAsync(x => x.RentalListingId == listingId);
        }

        private async Task<List<string>> GetHostEmailsAsync(long listingId)
        {
            var emails = await _dbContext.DssRentalListingContacts
                .Where(x => x.ContactedThroughRentalListingId == listingId && x.EmailAddressDsc != null)
                .Select(x => x.EmailAddressDsc)
                .ToListAsync();

            var hostEmails = new List<string>();
            
            foreach (var email in emails)
            {
                if (email != null)
                {
                    hostEmails.Add(email);
                }
            }

            return hostEmails;
        }

        private async Task<(List<string>, long)> GetPlatformEmailsAsync(long platformId)
        {
            var platform = await _dbContext.DssOrganizations
                .Include(x => x.DssOrganizationContactPeople)
                .Include(x => x.ManagingOrganization)
                    .ThenInclude(x => x.DssOrganizationContactPeople)
                .FirstAsync(x => x.OrganizationId == platformId);

            var platformEmails = platform.ManagingOrganization == null
                ? platform.DssOrganizationContactPeople
                    .Where(x => x.IsPrimary == true && x.EmailMessageType == EmailMessageTypes.NoticeOfTakedown && x.EmailAddressDsc != null)
                    .Select(x => x.EmailAddressDsc)
                : platform.ManagingOrganization.DssOrganizationContactPeople
                    .Where(x => x.IsPrimary == true && x.EmailMessageType == EmailMessageTypes.NoticeOfTakedown && x.EmailAddressDsc != null)
                    .Select(x => x.EmailAddressDsc);

            var providingPlatformId = platform.ManagingOrganization == null ? platform.OrganizationId : platform.ManagingOrganizationId ?? 0;

            return (platformEmails.ToList(), providingPlatformId);
        }

    }
}
