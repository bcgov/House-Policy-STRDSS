using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.RentalReportDtos;
using System.Diagnostics;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingRepository
    {
        Task<RentalListingResponseWithCountsDto> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction);
        Task<AggregatedListingResponseWithCountsDto> GetGroupedRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent);
        Task<int> CountHostListingsAsync(string hostName);
        Task<RentalListingViewDto?> GetRentalListing(long rentaListingId, bool loadHistory = true);
        Task<RentalListingForTakedownDto?> GetRentalListingForTakedownAction(long rentlListingId, bool includeHostEmails);
        Task<List<long>> GetRentalListingIdsToExport();
        Task<RentalListingExportDto?> GetRentalListingToExport(long rentalListingId);
        Task<DssRentalListingExtract> GetOrCreateRentalListingExtractByOrgId(long organizationId);
        Task<DssRentalListingExtract> GetOrCreateRentalListingExtractByExtractNm(string name);
        Task<List<RentalListingExtractDto>> GetRetalListingExportsAsync();
        Task<RentalListingExtractDto?> GetRetalListingExportAsync(long extractId);
        Task<RentalListingExtractDto?> GetRetalListingExportByNameAsync(string extractName);
        Task ConfirmAddressAsync(long rentalListingId);
        Task<DssRentalListing> UpdateAddressAsync(UpdateListingAddressDto dto);
        DateTime GetLatestRentalListingExportTime();
        Task<bool> ListingDataToProcessExists();
        Task LinkBizLicence(long rentalListingId, long licenceId);
        Task UnLinkBizLicence(long rentalListingId);
        Task ResetLgTransferFlag();
    }
    public partial class RentalListingRepository : RepositoryBase<DssRentalListingVw>, IRentalListingRepository
    {
        private IUserRepository _userRepo;

        public RentalListingRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger,
            IUserRepository userRepo) 
            : base(dbContext, mapper, currentUser, logger)
        {
            _userRepo = userRepo;
        }
        public async Task<RentalListingResponseWithCountsDto> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbSet.AsNoTracking();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            // Get allCount before applying filters (only organization filter applied)
            var allCount = await query.CountAsync();

            // Get recentCount with recent filter applied (before other filters)
            var queryForRecentCount = query;
            queryForRecentCount = ApplyRecentFilter(queryForRecentCount);
            var recentCount = await queryForRecentCount.CountAsync();

            // Apply recent filter if requested for data retrieval
            if (recent)
            {
                query = ApplyRecentFilter(query);
            }

            // Apply all other filters for data retrieval
            ApplyFilters(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, ref query);

            var extraSort
                = "AddressSort1ProvinceCd asc, AddressSort2LocalityNm asc, AddressSort3LocalityTypeDsc asc, AddressSort4StreetNm asc, AddressSort5StreetTypeDsc asc, AddressSort6StreetDirectionDsc asc, AddressSort7CivicNo asc, AddressSort8UnitNo asc";

            if (orderBy == "lastActionDtm")
            {
                orderBy = "lastActionDtm ?? System.DateTime.MinValue";
            }
            else if (orderBy == "lastActionNm")
            {
                orderBy = "lastActionNm ?? \"ZZZZ\"";
            }
            else if (orderBy == "businessLicenceNo")
            {
                orderBy = "businessLicenceNo ?? \"ZZZZ\"";
            }
            else if (orderBy == "businessLicenceNoMatched")
            {
                orderBy = "businessLicenceNoMatched ?? \"ZZZZ\"";
            }
            else if (orderBy == "nightsBookedYtdQty")
            {
                orderBy = "nightsBookedYtdQty ?? -1";
            }
            else if (orderBy == "registrationNumber")
            {
                orderBy = "bcRegistryNo ?? \"ZZZZ\"";
            }
            else if (orderBy == "latestReportPeriodYm")
            {
                orderBy = "latestReportPeriodYm ?? System.DateOnly.MinValue";
            }

            var listings = await Page<DssRentalListingVw, RentalListingViewDto>(query, pageSize, pageNumber, orderBy, direction, extraSort);

            var contacts = _mapper.Map<List<RentalListingContactDto>>(await _dbContext.DssRentalListingContacts
                .AsNoTracking()
                .Where(contact => listings.SourceList.Select(listing => listing.RentalListingId).Contains(contact.ContactedThroughRentalListingId))
                .ToListAsync());            

            foreach (var listing in listings.SourceList)            {
                listing.LastActionDtm = listing.LastActionDtm == null ? null : DateUtils.ConvertUtcToPacificTime(listing.LastActionDtm.Value);
                listing.Hosts = contacts.Where(x => x.ContactedThroughRentalListingId == listing.RentalListingId).ToList();
                
                // Override LastActionNm if takedown reason is "Invalid Registration"
                if (listing.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
                {
                    listing.LastActionNm = "Reg Check Failed";
                }
            }

            return new RentalListingResponseWithCountsDto
            {
                SourceList = listings.SourceList,
                PageInfo = listings.PageInfo,
                RecentCount = recentCount,
                AllCount = allCount
            };
        }

        public async Task<AggregatedListingResponseWithCountsDto> GetGroupedRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent)
        {
            var stopwatch = Stopwatch.StartNew();

            var query = _dbSet.AsNoTracking();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            // Get allCount before applying filters (only organization filter applied)
            var allCount = await query.CountAsync();

            // Get recentCount with recent filter applied (before other filters)
            var queryForRecentCount = query;
            queryForRecentCount = ApplyRecentFilter(queryForRecentCount);
            var recentCount = await queryForRecentCount.CountAsync();

            // Apply recent filter if requested for data retrieval
            if (recent)
            {
                query = ApplyRecentFilter(query);
            }

            // Apply all other filters for data retrieval
            ApplyFilters(all, address, url, listingId, hostName, businessLicence, registrationNumber, 
                prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, ref query);

            // Get all listings that match filters in one query
            var allListings = await query.ToListAsync();
            
            _logger.LogDebug($"Get Grouped Listings - Total Listings Fetched: {allListings.Count}, Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            stopwatch.Restart();

            // Use HashSet for faster Contains lookup
            var rentalListingIds = new HashSet<long>(allListings.Select(x => x.RentalListingId ?? 0));
            
            // Get all contacts in one query and map them all at once
            var allContactEntities = await _dbContext.DssRentalListingContacts
                .AsNoTracking()
                .Where(contact => rentalListingIds.Contains(contact.ContactedThroughRentalListingId))
                .ToListAsync();
            
            // Map all contacts at once, then group
            var allContactDtos = _mapper.Map<List<RentalListingContactDto>>(allContactEntities);
            var contactsByListing = allContactDtos
                .GroupBy(x => x.ContactedThroughRentalListingId)
                .ToDictionary(g => g.Key, g => g.ToList());

            _logger.LogDebug($"Get Grouped Listings - Contacts Fetched: {allContactEntities.Count}, Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            stopwatch.Restart();

            // Single pass partitioning using lists with pre-allocated capacity estimate
            var listingsWithRegNo = new List<DssRentalListingVw>(allListings.Count / 2);
            var listingsWithoutRegNo = new List<DssRentalListingVw>(allListings.Count / 2);
            
            foreach (var listing in allListings)
            {
                if (!string.IsNullOrWhiteSpace(listing.BcRegistryNo))
                    listingsWithRegNo.Add(listing);
                else
                    listingsWithoutRegNo.Add(listing);
            }

            // Group listings with registration numbers by BcRegistryNo
            var groupedByRegNo = listingsWithRegNo
                .GroupBy(x => x.BcRegistryNo)
                .Select(g => CreateGroup(g, contactsByListing))
                .ToList();

            // Group listings without registration numbers by existing logic (address, host, business licence)
            var groupedByOther = listingsWithoutRegNo
                .GroupBy(x => new { x.MatchAddressTxt, x.EffectiveHostNm, x.EffectiveBusinessLicenceNo })
                .Select(g => CreateGroup(g, contactsByListing))
                .ToList();

            // Combine both groups - use capacity to avoid resizing
            var grouped = new List<RentalListingGroupDto>(groupedByRegNo.Count + groupedByOther.Count);
            grouped.AddRange(groupedByRegNo);
            grouped.AddRange(groupedByOther);

            stopwatch.Stop();

            _logger.LogDebug($"Get Grouped Listings - Groups Created: {grouped.Count} (RegNo: {groupedByRegNo.Count}, Other: {groupedByOther.Count}), Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            return new AggregatedListingResponseWithCountsDto
            {
                Data = grouped,
                RecentCount = recentCount,
                AllCount = allCount
            };
        }

        private IQueryable<DssRentalListingVw> ApplyRecentFilter(IQueryable<DssRentalListingVw> query)
        {
            // Get current date in Pacific time
            var currentDate = DateUtils.ConvertUtcToPacificTime(DateTime.UtcNow);
            var currentDayOfMonth = currentDate.Day;
            var currentMonth = new DateOnly(currentDate.Year, currentDate.Month, 1);
            
            // Reports for a month are uploaded in the following month (e.g., January reports are uploaded in mid-February)
            // So we need to look at the previous month's reports, not the current month's
            var targetReportMonth = currentMonth.AddMonths(-1);  // The month we expect recent reports for
            var fallbackReportMonth = targetReportMonth.AddMonths(-1);  // The month before that for platforms that haven't reported yet

            // Get all platforms/organizations that have reported for the target month
            var platformsReportedTargetMonth = _dbContext.DssRentalListingReports
                .AsNoTracking()
                .Where(r => r.ReportPeriodYm == targetReportMonth)
                .Select(r => r.ProvidingOrganizationId)
                .Distinct()
                .ToList();

            if (currentDayOfMonth <= 19)
            {
                // Days 1-19: Show target month for platforms that reported, fallback month for those that haven't
                // Example: Feb 1-19 → Show January reports if uploaded, otherwise December reports
                query = query.Where(x => 
                    (platformsReportedTargetMonth.Contains(x.OfferingOrganizationId ?? 0) && x.LatestReportPeriodYm == targetReportMonth) ||
                    (!platformsReportedTargetMonth.Contains(x.OfferingOrganizationId ?? 0) && x.LatestReportPeriodYm == fallbackReportMonth)
                );
            }
            else
            {
                // Days 20-31: Show only target month listings (expect all platforms to have reported by now)
                // Example: Feb 20-28 → Show only January reports
                query = query.Where(x => x.LatestReportPeriodYm == targetReportMonth);
            }

            return query;
        }

        private RentalListingGroupDto CreateGroup(
            IEnumerable<DssRentalListingVw> listings, 
            Dictionary<long, List<RentalListingContactDto>> contactsByListing)
        {
            var listingsList = listings as IList<DssRentalListingVw> ?? listings.ToList();
            var firstListing = listingsList[0];
            
            var group = new RentalListingGroupDto
            {
                EffectiveBusinessLicenceNo = firstListing.EffectiveBusinessLicenceNo,
                EffectiveHostNm = firstListing.EffectiveHostNm,
                MatchAddressTxt = firstListing.MatchAddressTxt,
                NightsBookedYtdQty = 0
            };

            var listingDtos = _mapper.Map<List<RentalListingViewDto>>(listingsList);
            
            DateTime? maxLastActionDtm = null;
            RentalListingViewDto? listingWithLatestAction = null;
            DateOnly? maxReportPeriod = null;
            
            foreach (var listing in listingDtos)
            {
                listing.LastActionDtm = listing.LastActionDtm == null ? null : DateUtils.ConvertUtcToPacificTime(listing.LastActionDtm.Value);
                listing.Filtered = true; // All listings match the filter
                
                if (contactsByListing.TryGetValue(listing.RentalListingId ?? 0, out var contacts))
                {
                    listing.Hosts = contacts;
                }
                else
                {
                    listing.Hosts = [];
                }
                
                // Process hosts - set HasValidEmail on each host and HasAtLeastOneValidHostEmail on listing
                foreach (var host in listing.Hosts)
                {
                    if (host.EmailAddressDsc != null)
                    {
                        host.HasValidEmail = CommonUtils.IsValidEmailAddress(host.EmailAddressDsc);
                    }
                }
                listing.HasAtLeastOneValidHostEmail = listing.Hosts.Any(h => h.HasValidEmail);
                
                group.NightsBookedYtdQty += listing.NightsBookedYtdQty ?? 0;
                
                // Override LastActionNm if takedown reason is "Invalid Registration"
                if (listing.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
                {
                    listing.LastActionNm = "Reg Check Failed";
                }
                
                // Track max values in single pass
                if (listing.LastActionDtm > maxLastActionDtm || maxLastActionDtm == null)
                {
                    maxLastActionDtm = listing.LastActionDtm;
                    listingWithLatestAction = listing;
                }
                
                if (listing.LatestReportPeriodYm > maxReportPeriod || maxReportPeriod == null)
                {
                    maxReportPeriod = listing.LatestReportPeriodYm;
                }
            }

            if (listingWithLatestAction != null)
            {
                group.BusinessLicenceNo = listingWithLatestAction.BusinessLicenceNoMatched ?? listingWithLatestAction.BusinessLicenceNo;
                group.PrimaryHostNm = listingWithLatestAction.Hosts.Where(x => x.IsPropertyOwner).Select(x => x.FullNm).FirstOrDefault();
                group.LastActionNm = listingWithLatestAction.LastActionNm;
                group.LastActionDtm = listingWithLatestAction.LastActionDtm;
                group.BusinessLicenceId = listingWithLatestAction.BusinessLicenceId;
                group.BusinessLicenceExpiryDt = listingWithLatestAction.BusinessLicenceExpiryDt;
                group.LicenceStatusType = listingWithLatestAction.LicenceStatusType;
            }

            // Set the LatestReportPeriodYm from the most recent listing
            group.LatestReportPeriodYm = maxReportPeriod;

            // Sort listings by most recently reported date (descending), with most recent at the top
            group.Listings = listingDtos
                .OrderByDescending(x => x.LatestReportPeriodYm)
                .ToList();
            
            return group;
        }

        public async Task<int> CountHostListingsAsync(string hostName)
        {
            var query = _dbSet.AsNoTracking();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            return await query
                .Where(x => x.EffectiveHostNm == hostName)
                .Select(x => new { x.EffectiveBusinessLicenceNo, x.EffectiveHostNm, x.MatchAddressTxt })
                .Distinct() 
                .CountAsync(); 
        }

        private static void ApplyFilters(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, ref IQueryable<DssRentalListingVw> query)
        {
            if (all != null && all.IsNotEmpty())
            {
                var allPattern = $"%{all}%";
                var sanitizedAll = CommonUtils.SanitizeAndUppercaseString(all);
                query = query.Where(x => 
                    (x.MatchAddressTxt != null && EF.Functions.ILike(x.MatchAddressTxt, allPattern)) ||
                    (x.PlatformListingUrl != null && EF.Functions.ILike(x.PlatformListingUrl, allPattern)) ||
                    (x.PlatformListingNo != null && EF.Functions.ILike(x.PlatformListingNo, allPattern)) ||
                    (x.ListingContactNamesTxt != null && EF.Functions.ILike(x.ListingContactNamesTxt, allPattern)) ||
                    (x.EffectiveBusinessLicenceNo != null && x.EffectiveBusinessLicenceNo.StartsWith(sanitizedAll)) ||
                    (x.BcRegistryNo != null && EF.Functions.ILike(x.BcRegistryNo, allPattern)));
            }

            if (address != null && address.IsNotEmpty())
            {
                var addressPattern = $"%{address}%";
                query = query.Where(x => x.MatchAddressTxt != null && EF.Functions.ILike(x.MatchAddressTxt, addressPattern));
            }

            if (url != null && url.IsNotEmpty())
            {
                var urlPattern = $"%{url}%";
                query = query.Where(x => x.PlatformListingUrl != null && EF.Functions.ILike(x.PlatformListingUrl, urlPattern));
            }

            if (listingId != null && listingId.IsNotEmpty())
            {
                query = query.Where(x => x.PlatformListingNo != null && EF.Functions.ILike(x.PlatformListingNo, listingId));
            }

            if (hostName != null && hostName.IsNotEmpty())
            {
                var hostNamePattern = $"%{hostName}%";
                query = query.Where(x => x.ListingContactNamesTxt != null && EF.Functions.ILike(x.ListingContactNamesTxt, hostNamePattern));
            }

            if (businessLicence != null && businessLicence.IsNotEmpty())
            {
                var effectiveBusinessLicenceNo = CommonUtils.SanitizeAndUppercaseString(businessLicence);
                query = query.Where(x => x.EffectiveBusinessLicenceNo != null && x.EffectiveBusinessLicenceNo.StartsWith(effectiveBusinessLicenceNo));
            }

            if (registrationNumber != null && registrationNumber.IsNotEmpty())
            {
                var registrationPattern = $"%{registrationNumber}%";
                query = query.Where(x => x.BcRegistryNo != null && EF.Functions.ILike(x.BcRegistryNo, registrationPattern));
            }

            if (prRequirement != null)
            {
                query = query.Where(x => prRequirement.Value
                    ? x.IsPrincipalResidenceRequired == true
                    : x.IsPrincipalResidenceRequired == null || x.IsPrincipalResidenceRequired == false);
            }

            if (blRequirement != null)
            {
                query = query.Where(x => blRequirement.Value
                    ? x.IsBusinessLicenceRequired == true
                    : x.IsBusinessLicenceRequired == null || x.IsBusinessLicenceRequired == false);
            }

            if (reassigned != null && reassigned.Value == false)
            {
                reassigned = null;
            }

            if (takedownComplete != null && takedownComplete.Value == false)
            {
                takedownComplete = null;
            }

            if (reassigned != null && takedownComplete != null)
            {
                query = query.Where(x => x.IsTakenDown == true || x.IsLgTransferred == true);
            }
            else if (reassigned != null)
            {
                query = query.Where(x => x.IsLgTransferred == true);
            }
            else if (takedownComplete != null)
            {
                query = query.Where(x => x.IsTakenDown == true);
            }

            if (lgId != null)
            {
                query = query.Where(x => x.ManagingOrganizationId == lgId);
            }

            if (statusArray.Length > 0)
            {
                query = query.Where(x => statusArray.Contains(x.ListingStatusType));
            }
        }

        public async Task<RentalListingViewDto?> GetRentalListing(long rentalListingId, bool loadHistory = true)
        {
            var listing = _mapper.Map<RentalListingViewDto>(await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.RentalListingId == rentalListingId));

            if (listing == null) return listing;

            if (_currentUser.OrganizationType != OrganizationTypes.BCGov && listing.ManagingOrganizationId != _currentUser.OrganizationId)
            {
                return null;
            }

            listing.LastActionDtm = listing.LastActionDtm == null ? null : DateUtils.ConvertUtcToPacificTime(listing.LastActionDtm.Value);

            // Override LastActionNm if takedown reason is "Invalid Registration"
            if (listing.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
            {
                listing.LastActionNm = "Reg Check Failed";
            }

            listing.Hosts =
                _mapper.Map<List<RentalListingContactDto>>(await
                    _dbContext.DssRentalListingContacts
                        .Where(x => x.ContactedThroughRentalListingId == listing.RentalListingId)
                        .ToListAsync());

            if (!loadHistory) return listing;

            // Get the existing listing history from the database
            var existingHistory = await
                _dbContext.DssRentalListings.AsNoTracking()
                .Include(x => x.IncludingRentalListingReport)
                .Where(x => x.PlatformListingNo == listing.PlatformListingNo && x.OfferingOrganizationId == listing.OfferingOrganizationId && x.DerivedFromRentalListingId == null)
                .Select(x => new ListingHistoryDto {
                    ReportPeriodYM = x.IncludingRentalListingReport!.ReportPeriodYm!.ToString("yyyy-MM"),
                    NightsBookedQty = x.NightsBookedQty,
                    SeparateReservationsQty = x.SeparateReservationsQty
                }).ToListAsync();

            // If there's history, fill in missing months
            if (existingHistory.Any())
            {
                // Create a dictionary for quick lookup
                var historyDict = existingHistory.ToDictionary(x => x.ReportPeriodYM);

                // Get the date range
                var sortedHistory = existingHistory.OrderBy(x => x.ReportPeriodYM).ToList();
                var earliestDate = DateOnly.ParseExact(sortedHistory.First().ReportPeriodYM, "yyyy-MM", null);
                var latestDate = DateOnly.ParseExact(sortedHistory.Last().ReportPeriodYM, "yyyy-MM", null);

                // Generate all months between earliest and latest
                var allMonths = new List<ListingHistoryDto>();
                var currentMonth = earliestDate;

                while (currentMonth <= latestDate)
                {
                    var periodKey = currentMonth.ToString("yyyy-MM");
                    
                    if (historyDict.TryGetValue(periodKey, out var existingEntry))
                    {
                        allMonths.Add(existingEntry);
                    }
                    else
                    {
                        // Add missing month with -1 values
                        allMonths.Add(new ListingHistoryDto
                        {
                            ReportPeriodYM = periodKey,
                            NightsBookedQty = -1,
                            SeparateReservationsQty = -1
                        });
                    }

                    currentMonth = currentMonth.AddMonths(1);
                }

                listing.ListingHistory = allMonths.OrderByDescending(x => x.ReportPeriodYM).ToList();
            }
            else
            {
                listing.ListingHistory = existingHistory;
            }

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
                var fullName = CommonUtils.GetFullName(action.FirstName ?? "", action.LastName ?? "");
                action.User = fullName == "" ? "System" : fullName;
            }

            listing.AddressChangeHistory = await GetAddressChangeHistoryAsync(rentalListingId);

            listing.BizLicenceInfo = _mapper.Map<BizLicenceDto>(
                await _dbContext.DssBusinessLicences.AsNoTracking().Include(x => x.LicenceStatusTypeNavigation)
                    .FirstOrDefaultAsync(x => x.BusinessLicenceId == listing.BusinessLicenceId)
            );

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

            // Override LastActionNm if takedown reason is "Invalid Registration"
            if (listing.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
            {
                listing.LastActionNm = "Reg Check Failed";
            }

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
                    
                    // Override if it's a completed takedown with invalid registration reason
                    if (actions[0].EmailMessageTypeNm == EmailMessageTypes.CompletedTakedown && 
                        listing.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
                    {
                        listing.LastActionNm1 = "Reg Check Failed";
                    }
                    
                    listing.LastActionDtm2 = actions[1].MessageDeliveryDtm;
                    listing.LastActionNm2 = actions[1].EmailMessageTypeNm;
                    
                    // Override if it's a completed takedown with invalid registration reason
                    if (actions[1].EmailMessageTypeNm == EmailMessageTypes.CompletedTakedown && 
                        listing.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
                    {
                        listing.LastActionNm2 = "Reg Check Failed";
                    }
                    break;
                case 1:
                    listing.LastActionDtm1 = actions[0].MessageDeliveryDtm;
                    listing.LastActionNm1 = actions[0].EmailMessageTypeNm;
                    
                    // Override if it's a completed takedown with invalid registration reason
                    if (actions[0].EmailMessageTypeNm == EmailMessageTypes.CompletedTakedown && 
                        listing.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
                    {
                        listing.LastActionNm1 = "Reg Check Failed";
                    }
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

            var historyDict = listingHistory
                .GroupBy(x => x.ReportPeriodYm)
                .ToDictionary(g => g.Key, g => g.First()); // Use the first entry in case of duplicates

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

        public async Task<RentalListingExtractDto?> GetRetalListingExportByNameAsync(string extractName)
        {
            var extract = await _dbContext
                .DssRentalListingExtracts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RentalListingExtractNm == extractName);

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

        public async Task ConfirmAddressAsync(long rentalListingId)
        {
            var listing = await _dbContext
                .DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .FirstAsync(x => x.RentalListingId == rentalListingId);

            var newAddress = _mapper.Map<DssPhysicalAddress>(listing.LocatingPhysicalAddress);

            newAddress.PhysicalAddressId = 0;
            newAddress.IsMatchVerified = true;
            newAddress.IsMatchCorrected = null;
            newAddress.IsChangedOriginalAddress = null;
            newAddress.IsSystemProcessing = true;

            newAddress.ReplacingPhysicalAddressId = listing.LocatingPhysicalAddressId;

            listing.LocatingPhysicalAddress = newAddress;

            _dbContext.Entry(listing.LocatingPhysicalAddress).State = EntityState.Detached;
            _dbContext.Entry(newAddress).State = EntityState.Added;
        }

        public async Task<List<AddressChangeHistoryDto>> GetAddressChangeHistoryAsync(long rentalListingId)
        {
            var history = new List<AddressChangeHistoryDto>();
            var listing = await _dbContext.DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .FirstOrDefaultAsync(x => x.RentalListingId == rentalListingId);

            if (listing?.LocatingPhysicalAddress == null)
                return history;

            var addressIds = new HashSet<long>();
            await BuildAddressHistoryRecursivelyAsync(listing.LocatingPhysicalAddress, history, addressIds);
            return history.OrderByDescending(x => x.Date).ToList();
        }

        private async Task BuildAddressHistoryRecursivelyAsync(DssPhysicalAddress address, List<AddressChangeHistoryDto> history, HashSet<long> processedAddressIds)
        {
            if (address == null || !processedAddressIds.Add(address.PhysicalAddressId))
                return;

            var addressDto = new AddressChangeHistoryDto
            {
                Type = GetAddressHistoryType(address),
                PlatformAddress = address.OriginalAddressTxt,
                BestMatchAddress = address.MatchAddressTxt ?? "",
                Date = DateUtils.ConvertUtcToPacificTime(address.UpdDtm),
                User = await GetUserNameAsync(address.UpdUserGuid)
            };

            history.Add(addressDto);

            if (address.ReplacingPhysicalAddressId.HasValue)
            {
                var previousAddress = await _dbContext.DssPhysicalAddresses
                    .FirstOrDefaultAsync(a => a.PhysicalAddressId == address.ReplacingPhysicalAddressId);
                await BuildAddressHistoryRecursivelyAsync(previousAddress, history, processedAddressIds);
            }
        }

        private async Task<string> GetUserNameAsync(Guid? userGuid)
        {
            if (userGuid == null || userGuid == Guid.Empty)
                return "System";

            var user = await _userRepo.GetUserByGuid(userGuid.Value);
            return user == null ? "System" : CommonUtils.GetFullName(user.GivenNm ?? "", user.FamilyNm ?? "");
        }

        private string GetAddressHistoryType(DssPhysicalAddress address)
        {
            if (address == null)
                return "Unknown";

            if (address.IsChangedOriginalAddress != null && address.IsChangedOriginalAddress.Value)
                return "Platform Data";

            if (address.IsMatchVerified != null && address.IsMatchVerified.Value)
                return "User Confirmation";

            if (address.IsMatchCorrected != null && address.IsMatchCorrected.Value)
                return "User Edit";

            return "Platform Data";
        }

        public async Task<DssRentalListing> UpdateAddressAsync(UpdateListingAddressDto dto)
        {
            var listing = await _dbContext
                .DssRentalListings
                .Include(x => x.LocatingPhysicalAddress)
                .FirstAsync(x => x.RentalListingId == dto.RentalListingId);

            var newAddress = _mapper.Map<DssPhysicalAddress>(listing.LocatingPhysicalAddress);

            newAddress.PhysicalAddressId = 0;
            newAddress.IsMatchVerified = null;
            newAddress.IsMatchCorrected = null;
            newAddress.IsChangedOriginalAddress = null;
            newAddress.IsSystemProcessing = true;

            newAddress.ReplacingPhysicalAddressId = listing.LocatingPhysicalAddressId;

            listing.LocatingPhysicalAddress = newAddress;

            _dbContext.Entry(listing.LocatingPhysicalAddress).State = EntityState.Detached;
            _dbContext.Entry(newAddress).State = EntityState.Added;

            return listing;
        }

        public DateTime GetLatestRentalListingExportTime()
        {
            if (_dbContext.DssRentalListingExtracts.Any())
            {
                return _dbContext.DssRentalListingExtracts.Max(x => x.UpdDtm);
            }
            else
            {
                return DateTime.UtcNow.AddDays(-1);
            }
        }

        public async Task<bool> ListingDataToProcessExists()
        {
            return await _dbContext.DssUploadLines
                .AnyAsync(x => x.IncludingUploadDelivery.UploadDeliveryType == UploadDeliveryTypes.ListingData && x.IsProcessed == false);                
        }


        public async Task LinkBizLicence(long rentalListingId, long licenceId)
        {
            //assume they exist - validated already
            var licence = await _dbContext.DssBusinessLicences.FirstAsync(x => x.BusinessLicenceId == licenceId);
            var listing = await _dbContext.DssRentalListings.FirstAsync(x => x.RentalListingId == rentalListingId);

            var blFromLg = CommonUtils.SanitizeAndUppercaseString(licence.BusinessLicenceNo);
            var blFromPlatform = CommonUtils.SanitizeAndUppercaseString(listing.BusinessLicenceNo);

            listing.GoverningBusinessLicenceId = licenceId;
            listing.EffectiveBusinessLicenceNo = blFromLg;
            listing.IsChangedBusinessLicence = blFromLg != blFromPlatform;
        }

        public async Task UnLinkBizLicence(long rentalListingId)
        {
            //assume it exists - validated already
            var listing = await _dbContext.DssRentalListings
                .FirstAsync(x => x.RentalListingId == rentalListingId);

            if (listing.GoverningBusinessLicenceId == null)
                return;

            listing.GoverningBusinessLicenceId = null;
            listing.EffectiveBusinessLicenceNo = CommonUtils.SanitizeAndUppercaseString(listing.BusinessLicenceNo);
            listing.IsChangedBusinessLicence = true;
        }

        public async Task ResetLgTransferFlag()
        {
            var sql = "UPDATE dss_rental_listing SET is_lg_transferred = null, lg_transfer_dtm = null WHERE lg_transfer_dtm <= @twomonth";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@twomonth", NpgsqlTypes.NpgsqlDbType.Date)
                {
                    Value = DateTime.UtcNow.Date.AddMonths(-2)
                },
            };

            var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());

            if (rowsAffected > 0)
                _logger.LogInformation("{RowsAffected} rental listings had the lg_transfer_dtm and is_lg_transferred fields reset.", rowsAffected);
        }
    }
}
