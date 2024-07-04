using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.RentalReportDtos;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingRepository
    {
        Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? rentalListingId, string? hostName, string? businessLicense, int pageSize, int pageNumber, string orderBy, string direction);
        Task<RentalListingViewDto?> GetRentalListing(long rentaListingId);
        Task<RentalListingForTakedownDto?> GetRentalListingForTakedownAction(long rentlListingId, bool includeHostEmails);
        Task<List<long>> GetRentalListingIdsToExport();
        Task<RentalListingExportDto?> GetRentalListingToExport(long rentalListingId);
        Task<DssRentalListingExtract> GetOrCreateRentalListingExtractByOrgId(long organizationId);
        Task<DssRentalListingExtract> GetOrCreateRentalListingExtractByExtractNm(string name);
        Task<List<RentalListingExtractDto>> GetRetalListingExportsAsync();
        Task<RentalListingExtractDto?> GetRetalListingExportAsync(long extractId);
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

            foreach (var listing in listings.SourceList)
            {
                listing.Hosts =
                    _mapper.Map<List<RentalListingContactDto>>(await
                        _dbContext.DssRentalListingContacts
                            .Where(x => x.ContactedThroughRentalListingId == listing.RentalListingId)
                            .ToListAsync());
            }

            return listings;
        }

        public async Task<RentalListingViewDto?> GetRentalListing(long rentalListingId)
        {
            var listing = _mapper.Map<RentalListingViewDto>(await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.RentalListingId == rentalListingId));

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
                    .Include(x => x.InitiatingUserIdentity)
                    .Where(x => x.ConcernedWithRentalListingId == listing.RentalListingId)
                    .OrderByDescending(x => x.MessageDeliveryDtm)
                    .Select(x => new ActionHistoryDto
                    {
                        Action = x.EmailMessageTypeNavigation.EmailMessageTypeNm,
                        Date = DateUtils.ConvertUtcToPacificTime((DateTime)x.MessageDeliveryDtm!),
                        FirstName = x.InitiatingUserIdentity == null ? "" : x.InitiatingUserIdentity.GivenNm,
                        LastName = x.InitiatingUserIdentity == null ? "" : x.InitiatingUserIdentity.FamilyNm
                    })
                    .ToListAsync());

            foreach (var action in listing.ActionHistory)
            {
                action.User = CommonUtils.GetFullName(action.FirstName ?? "", action.LastName ?? "");
            }

            return listing;
        }

        public async Task<RentalListingForTakedownDto?> GetRentalListingForTakedownAction(long rentalListingId, bool includeHostEmail)
        {
            var listing = await GetRentalListingAsync(rentalListingId);
            if (listing == null) return null;

            if (includeHostEmail)
            {
                listing.HostEmails = await GetHostEmailsAsync(rentalListingId);
            }

            (listing.PlatformEmails, listing.ProvidingPlatformId) = await GetPlatformEmailsAsync(listing.OfferingPlatformId);

            return listing;
        }

        private async Task<RentalListingForTakedownDto?> GetRentalListingAsync(long rentalListingId)
        {
            return await _dbContext.DssRentalListingVws
                .Where(x => x.RentalListingId == rentalListingId)
                .Select(x => new RentalListingForTakedownDto
                {
                    RentalListingId = rentalListingId,
                    PlatformListingNo = x.PlatformListingNo ?? "",
                    PlatformListingUrl = x.PlatformListingUrl,
                    OrganizationCd = x.OfferingOrganizationCd ?? "",
                    OfferingPlatformId = x.OfferingOrganizationId ?? 0,
                    LocalGovernmentId = x.ManagingOrganizationId ?? 0,
                })
                .FirstOrDefaultAsync();
        }

        private async Task<List<string>> GetHostEmailsAsync(long rentalListingId)
        {
            var emails = await _dbContext.DssRentalListingContacts
                .Where(x => x.ContactedThroughRentalListingId == rentalListingId && x.EmailAddressDsc != null)
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

        public async Task<List<long>> GetRentalListingIdsToExport()
        {
            return await _dbSet
                .OrderBy(x => x.ManagingOrganizationId)
                .ThenBy(x => x.IsPrincipalResidenceRequired)
                .Select(x => x.RentalListingId ?? 0)
                .ToListAsync();
        }

        public async Task<RentalListingExportDto?> GetRentalListingToExport(long rentalListingId)
        {
            var listing = _mapper.Map<RentalListingExportDto>(await _dbSet.AsNoTracking().FirstAsync(x => x.RentalListingId == rentalListingId));

            await LoadHistoryFields(listing);

            await LoadPropertyHostsFields(listing);

            await LoadActionsFields(listing);

            return listing;
        }

        private async Task LoadActionsFields(RentalListingExportDto listing)
        {
            var actions = await _dbContext.DssEmailMessages
                .AsNoTracking()
                .Where(x => x.ConcernedWithRentalListingId == listing.RentalListingId)
                .OrderByDescending(x => x.MessageDeliveryDtm)
                .Skip(1)
                .Take(2)
                .Select(x => new
                {
                    x.MessageDeliveryDtm,
                    x.EmailMessageTypeNavigation.EmailMessageTypeNm
                })
                .ToListAsync();

            switch (actions.Count)
            {
                case 2:
                    listing.LastActionDtm1 = actions[0].MessageDeliveryDtm;
                    listing.LastActionNm1 = actions[0].EmailMessageTypeNm;
                    listing.LastActionDtm2 = actions[1].MessageDeliveryDtm;
                    listing.LastActionNm2 = actions[1].EmailMessageTypeNm;
                    break;
                case 1:
                    listing.LastActionDtm1 = actions[0].MessageDeliveryDtm;
                    listing.LastActionNm1 = actions[0].EmailMessageTypeNm;
                    break;
            }
        }

        private async Task LoadHistoryFields(RentalListingExportDto listing)
        {
            var reportMonths = GetLast12ReportMonths();
            var listingHistory = await _dbContext.DssRentalListings
                .AsNoTracking()
                .Where(x => x.OfferingOrganizationId == listing.OfferingOrganizationId
                    && x.PlatformListingNo == listing.PlatformListingNo
                    && x.IncludingRentalListingReportId != null
                    && reportMonths.Contains(x.IncludingRentalListingReport.ReportPeriodYm))
                .Select(x => new
                {
                    x.IncludingRentalListingReport.ReportPeriodYm,
                    x.NightsBookedQty,
                    x.SeparateReservationsQty
                })
                .ToListAsync();

            var historyDict = listingHistory.ToDictionary(x => x.ReportPeriodYm);

            for (int i = 0; i < reportMonths.Length; i++)
            {
                if (historyDict.TryGetValue(reportMonths[i], out var history))
                {
                    RentalListingExportDto.NightsBookedSetters[i](listing, history.NightsBookedQty);
                    RentalListingExportDto.SeparateReservationsSetters[i](listing, history.SeparateReservationsQty);
                }
            }
        }

        private async Task LoadPropertyHostsFields(RentalListingExportDto listing)
        {
            var listingContacts = await _dbContext.DssRentalListingContacts
                .AsNoTracking()
                .Where(x => x.ContactedThroughRentalListingId == listing.RentalListingId)
                .Select(x => new
                {
                    x.IsPropertyOwner,
                    x.ListingContactNbr,
                    x.FullNm,
                    x.EmailAddressDsc,
                    x.PhoneNo,
                    x.FullAddressTxt
                })
                .ToListAsync();

            var propertyHosts = new[]
            {
                listingContacts.FirstOrDefault(x => x.IsPropertyOwner),
                listingContacts.FirstOrDefault(x => !x.IsPropertyOwner && x.ListingContactNbr == 1),
                listingContacts.FirstOrDefault(x => !x.IsPropertyOwner && x.ListingContactNbr == 2),
                listingContacts.FirstOrDefault(x => !x.IsPropertyOwner && x.ListingContactNbr == 3),
                listingContacts.FirstOrDefault(x => !x.IsPropertyOwner && x.ListingContactNbr == 4),
                listingContacts.FirstOrDefault(x => !x.IsPropertyOwner && x.ListingContactNbr == 5)
            };

            for (int i = 0; i < propertyHosts.Length; i++)
            {
                var host = propertyHosts[i];
                if (host != null)
                {
                    RentalListingExportDto.PropertyHostNameSetters[i](listing, host.FullNm);
                    RentalListingExportDto.PropertyHostEmailSetters[i](listing, host.EmailAddressDsc);
                    RentalListingExportDto.PropertyHostPhoneNumberSetters[i](listing, host.PhoneNo);
                    RentalListingExportDto.PropertyHostMailingAddressSetters[i](listing, host.FullAddressTxt);
                }
            }
        }

        private DateOnly[] GetLast12ReportMonths()
        {
            var today = DateTime.UtcNow;
            var currentMonth = new DateOnly(today.Year, today.Month, 1);
            var reportMonths = new DateOnly[12];

            for (var i = 0; i < 12; i++)
            {
                reportMonths[i] = currentMonth.AddMonths(-1 * (i + 1));
            }

            return reportMonths;
        }


        public async Task<DssRentalListingExtract> GetOrCreateRentalListingExtractByOrgId(long organizationId)
        {
            var extract = await _dbContext.DssRentalListingExtracts.FirstOrDefaultAsync(x => x.FilteringOrganizationId == organizationId);

            if (extract == null)
            {
                extract = new DssRentalListingExtract
                {
                    FilteringOrganizationId = organizationId,
                    IsPrRequirementFiltered = false,
                    UpdUserGuid = Guid.Empty,
                };

                _dbContext.DssRentalListingExtracts.Add(extract);
            }

            return extract;
        }

        public async Task<DssRentalListingExtract> GetOrCreateRentalListingExtractByExtractNm(string name)
        {
            var extract = await _dbContext.DssRentalListingExtracts.FirstOrDefaultAsync(x => x.RentalListingExtractNm == name);

            if (extract == null)
            {
                extract = new DssRentalListingExtract
                {
                    RentalListingExtractNm = name,
                    IsPrRequirementFiltered = false,
                    UpdUserGuid = Guid.Empty,
                };

                _dbContext.DssRentalListingExtracts.Add(extract);
            }

            return extract;
        }
        public async Task<RentalListingExtractDto?> GetRetalListingExportAsync(long extractId)
        {
            var extract = await _dbContext
                .DssRentalListingExtracts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RentalListingExtractId == extractId);

            if (extract == null) return null;

            return _mapper.Map<RentalListingExtractDto>(extract);
        }

        public async Task<List<RentalListingExtractDto>> GetRetalListingExportsAsync()
        {
            var datasets = _mapper.Map<List<RentalListingExtractDto>>(await _dbContext.DssRentalListingExtracts.AsNoTracking().ToListAsync());

            return datasets.Where(x => _currentUser.OrganizationType == OrganizationTypes.LG
                                       ? x.FilteringOrganizationId == _currentUser.OrganizationId
                                       : x.FilteringOrganizationId == null).ToList();
        }
    }
}
