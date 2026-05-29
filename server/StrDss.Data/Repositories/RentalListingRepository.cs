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
        Task<PagedDto<RentalListingTableRowDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction, bool includeTotalCount = true);
        Task<int> GetRentalListingsCountAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent);
        Task<int> GetGroupedRentalListingsCountAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent);
        Task<PagedDto<RentalListingGroupSummaryDto>> GetGroupedRentalListingsPagedAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction, bool includeTotalCount = true);
        Task<List<RentalListingTableRowDto>> GetGroupedListingChildrenAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, string? bcRegistryNo, string? matchAddressTxt, string? matchUnitNo, string? effectiveHostNm, string? effectiveBusinessLicenceNo);
        Task<Dictionary<string, bool>> BatchEffectiveHostHasMultiplePropertiesAsync(IReadOnlyList<string?> effectiveHostNms);
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
        Task<bool> DismissIdUpdatedStatusAsync(long rentalListingId);
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
        private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "latestReportPeriodYm", "registrationNumber", "matchAddressTxt", "nightsBookedYtdQty",
            "businessLicenceNo", "businessLicenceNoMatched", "platformListingNo"
        };

        private static readonly HashSet<string> GroupedAllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "latestReportPeriodYm", "effectiveHostNm", "matchAddressTxt", "effectiveBusinessLicenceNo"
        };

        /// <summary>
        /// Builds the base IQueryable for GetRentalListings from base tables (no view).
        /// Uses set-based LEFT JOINs to pre-aggregated subqueries for LatestReportPeriodYm and last email action
        /// instead of correlated scalar subqueries. Projects directly to <see cref="RentalListingTableRowDto"/>.
        /// For performance tuning: run EXPLAIN (ANALYZE, BUFFERS) on the generated SQL; indexes are defined in
        /// STR_DSS_Physical_DB_DDL (R14) and STR_DSS_Incremental_DB_Sprint_R15 (partial indexes i12, i13 on dss_rental_listing).
        /// </summary>
        private IQueryable<RentalListingTableRowDto> GetRentalListingsTableBaseQuery()
        {
            var currentListings = _dbContext.DssRentalListings
                .AsNoTracking()
                .Where(drl => drl.IncludingRentalListingReportId == null);

            // Pre-aggregated: max report period per (OfferingOrganizationId, PlatformListingNo) from reported listings
            var latestReportPeriod = from rl in _dbContext.DssRentalListings
                                    where rl.IncludingRentalListingReportId != null
                                    join rpt in _dbContext.DssRentalListingReports on rl.IncludingRentalListingReportId equals rpt.RentalListingReportId
                                    group rpt by new { rl.OfferingOrganizationId, rl.PlatformListingNo } into g
                                    select new { g.Key.OfferingOrganizationId, g.Key.PlatformListingNo, MaxReportPeriodYm = g.Max(x => x.ReportPeriodYm) };

            return from drl in currentListings
                   join org in _dbContext.DssOrganizations on drl.OfferingOrganizationId equals org.OrganizationId
                   join dlst in _dbContext.DssListingStatusTypes on drl.ListingStatusType equals dlst.ListingStatusType into dlstJ
                   from dlst in dlstJ.DefaultIfEmpty()
                   join dpa in _dbContext.DssPhysicalAddresses on drl.LocatingPhysicalAddressId equals dpa.PhysicalAddressId into dpaJ
                   from dpa in dpaJ.DefaultIfEmpty()
                   join lgs in _dbContext.DssOrganizations on (dpa != null && dpa.MatchScoreAmt > 1 && dpa.ContainingOrganizationId != null ? dpa.ContainingOrganizationId : (long?)null) equals (long?)lgs.OrganizationId into lgsJ
                   from lgs in lgsJ.DefaultIfEmpty()
                   join lg in _dbContext.DssOrganizations on (lgs != null ? lgs.ManagingOrganizationId : (long?)null) equals (long?)lg.OrganizationId into lgJ
                   from lg in lgJ.DefaultIfEmpty()
                   join dbl in _dbContext.DssBusinessLicences on drl.GoverningBusinessLicenceId equals (long?)dbl.BusinessLicenceId into dblJ
                   from dbl in dblJ.DefaultIfEmpty()
                   join lrp in latestReportPeriod on new { drl.OfferingOrganizationId, drl.PlatformListingNo } equals new { lrp.OfferingOrganizationId, lrp.PlatformListingNo } into lrpJ
                   from lrp in lrpJ.DefaultIfEmpty()
                   select new RentalListingTableRowDto
                   {
                       RentalListingId = drl.RentalListingId,
                       LatestReportPeriodYm = lrp != null ? (DateOnly?)lrp.MaxReportPeriodYm : null,
                       OfferingOrganizationId = drl.OfferingOrganizationId,
                       PlatformListingNo = drl.PlatformListingNo,
                       EffectiveBusinessLicenceNo = drl.EffectiveBusinessLicenceNo,
                       EffectiveHostNm = drl.EffectiveHostNm,
                       IsLgTransferred = drl.IsLgTransferred,
                       IsTakenDown = drl.IsTakenDown,
                       BcRegistryNo = drl.BcRegistryNo,
                       MatchAddressTxt = dpa != null ? dpa.MatchAddressTxt : null,
                       MatchUnitNo = dpa != null ? dpa.UnitNo : null,
                       MatchScoreAmt = dpa != null ? dpa.MatchScoreAmt : null,
                       IsMatchVerified = dpa != null ? dpa.IsMatchVerified : null,
                       IsMatchCorrected = dpa != null ? dpa.IsMatchCorrected : null,
                       IsChangedAddress = drl.IsChangedAddress,
                       NightsBookedYtdQty = drl.NightsBookedQty,
                       BusinessLicenceNo = drl.BusinessLicenceNo,
                       BusinessLicenceNoMatched = dbl != null ? dbl.BusinessLicenceNo : null,
                       IsChangedBusinessLicence = drl.IsChangedBusinessLicence,
                       LastActionDtm = null,
                       LastActionNm = null,
                       PlatformListingUrl = drl.PlatformListingUrl,
                       OfferingOrganizationNm = org.OrganizationNm,
                       TakeDownReason = drl.TakeDownReason,
                       ManagingOrganizationId = (dpa != null && dpa.MatchScoreAmt > 1 && lg != null) ? lg.OrganizationId : (long?)null,
                       ListingStatusType = dlst != null ? dlst.ListingStatusType : drl.ListingStatusType,
                       IsPrincipalResidenceRequired = (dpa != null && dpa.MatchScoreAmt > 1 && lgs != null) ? lgs.IsPrincipalResidenceRequired : null,
                       IsBusinessLicenceRequired = (dpa != null && dpa.MatchScoreAmt > 1 && lgs != null) ? lgs.IsBusinessLicenceRequired : null,
                       BusinessLicenceId = drl.GoverningBusinessLicenceId,
                       BusinessLicenceExpiryDt = dbl != null ? (DateOnly?)dbl.ExpiryDt : null,
                       LicenceStatusType = dbl != null ? dbl.LicenceStatusType : null
                   };
        }

        /// <summary>
        /// Hydrates LastActionDtm and LastActionNm for a batch of listing rows by querying
        /// the email messages table only for the given listing IDs (typically one page worth).
        ///
        /// Implemented as a single grouped query: for each ConcernedWithRentalListingId we ask
        /// the DB for both the max MessageDeliveryDtm AND the type name from the row that owns
        /// that max (ties broken deterministically by EmailMessageId desc). EF Core 8 + Npgsql
        /// translate the inner OrderByDescending(...).Select(...).FirstOrDefault() to a LATERAL
        /// subquery on PostgreSQL, so this is one round trip instead of two.
        /// </summary>
        private async Task HydrateLastActionFieldsAsync(List<RentalListingTableRowDto> rows)
        {
            if (rows.Count == 0) return;

            var listingIds = rows
                .Where(r => r.RentalListingId != null)
                .Select(r => r.RentalListingId!.Value)
                .ToList();
            Dictionary<long, (DateTime Dtm, string? TypeNm)>? byId = null;
            if (listingIds.Count > 0)
            {
                var latest = await _dbContext.DssEmailMessages
                    .AsNoTracking()
                    .Where(em => em.ConcernedWithRentalListingId != null
                              && listingIds.Contains(em.ConcernedWithRentalListingId!.Value))
                    .GroupBy(em => em.ConcernedWithRentalListingId!.Value)
                    .Select(g => new
                    {
                        RentalListingId = g.Key,
                        Dtm = g.Max(x => x.MessageDeliveryDtm),
                        TypeNm = g.OrderByDescending(x => x.MessageDeliveryDtm)
                                  .ThenByDescending(x => x.EmailMessageId)
                                  .Select(x => x.EmailMessageTypeNavigation.EmailMessageTypeNm)
                                  .FirstOrDefault()
                    })
                    .ToListAsync();

                if (latest.Count > 0)
                {
                    byId = latest.ToDictionary(x => x.RentalListingId, x => (x.Dtm, x.TypeNm));
                }
            }

            foreach (var row in rows)
            {
                if (row.RentalListingId != null
                    && byId != null
                    && byId.TryGetValue(row.RentalListingId.Value, out var info))
                {
                    row.LastActionDtm = info.Dtm;
                    row.LastActionNm = info.TypeNm;
                }

                row.LastActionDtm = row.LastActionDtm == null ? null : DateUtils.ConvertUtcToPacificTime(row.LastActionDtm.Value);
                if (row.TakeDownReason == TakeDownReasonStatus.InvalidRegistration)
                {
                    row.LastActionNm = "Reg Check Failed";
                }
            }
        }

        /// <summary>
        /// Post-paging hydrator for <see cref="RentalListingGroupSummaryDto.PrimaryHostNm"/>.
        /// Loads the original-case property-owner contact name from dss_rental_listing_contact
        /// for the anchor listing of each summary on the current page only (~10-50 rows). This
        /// avoids the DSS-1311 regression of fetching contacts for the entire unpaged set while
        /// still showing a human-readable host name instead of the sanitized/uppercased
        /// EffectiveHostNm. Uses the indexed FK dss_rental_listing_contact_i1 (defined in
        /// STR_DSS_Physical_DB_DDL_Sprint_14.sql) and filters to IsPropertyOwner = true.
        /// </summary>
        private async Task HydratePrimaryHostNmAsync(List<RentalListingGroupSummaryDto> page)
        {
            if (page.Count == 0) return;

            var anchorIds = page
                .Where(s => s.AnchorRentalListingId != null)
                .Select(s => s.AnchorRentalListingId!.Value)
                .Distinct()
                .ToList();
            if (anchorIds.Count == 0) return;

            // One row per anchor listing, deterministically picking the first property-owner
            // contact by RentalListingContactId when more than one exists for that listing.
            var nameByListing = await _dbContext.DssRentalListingContacts
                .AsNoTracking()
                .Where(c => c.IsPropertyOwner && anchorIds.Contains(c.ContactedThroughRentalListingId))
                .GroupBy(c => c.ContactedThroughRentalListingId)
                .Select(g => new
                {
                    Id = g.Key,
                    FullNm = g.OrderBy(x => x.RentalListingContactId)
                              .Select(x => x.FullNm)
                              .FirstOrDefault()
                })
                .ToListAsync();

            if (nameByListing.Count == 0) return;

            var lookup = nameByListing.ToDictionary(x => x.Id, x => x.FullNm);

            foreach (var s in page)
            {
                if (s.AnchorRentalListingId == null) continue;
                if (lookup.TryGetValue(s.AnchorRentalListingId.Value, out var nm)
                    && !string.IsNullOrWhiteSpace(nm))
                {
                    s.PrimaryHostNm = nm.Trim();
                }
            }
        }

        /// <summary>
        /// Applies the "recent" filter in a single SQL round trip by using a subquery for platforms
        /// that reported for the target month instead of materializing a list with .ToList().
        /// </summary>
        private IQueryable<RentalListingTableRowDto> ApplyRecentFilterTable(IQueryable<RentalListingTableRowDto> query)
        {
            var currentDate = DateUtils.ConvertUtcToPacificTime(DateTime.UtcNow);
            var currentDayOfMonth = currentDate.Day;
            var currentMonth = new DateOnly(currentDate.Year, currentDate.Month, 1);
            var targetReportMonth = currentMonth.AddMonths(-1);
            var fallbackReportMonth = targetReportMonth.AddMonths(-1);

            // Subquery: organizations that reported for the target month (translated to SQL IN (subquery), no extra round trip)
            var platformsReportedTargetMonth = _dbContext.DssRentalListingReports
                .AsNoTracking()
                .Where(r => r.ReportPeriodYm == targetReportMonth)
                .Select(r => r.ProvidingOrganizationId)
                .Distinct();

            if (currentDayOfMonth <= 19)
            {
                query = query.Where(x =>
                    (platformsReportedTargetMonth.Contains(x.OfferingOrganizationId ?? 0) && x.LatestReportPeriodYm == targetReportMonth) ||
                    (!platformsReportedTargetMonth.Contains(x.OfferingOrganizationId ?? 0) && x.LatestReportPeriodYm == fallbackReportMonth));
            }
            else
            {
                query = query.Where(x => x.LatestReportPeriodYm == targetReportMonth);
            }

            return query;
        }

        private void ApplyFiltersTable(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, ref IQueryable<RentalListingTableRowDto> query)
        {
            if (all != null && all.IsNotEmpty())
            {
                var allPattern = $"%{all}%";
                var sanitizedAll = CommonUtils.SanitizeAndUppercaseString(all);
                var sanitizedAllPattern = $"%{sanitizedAll}%";
                query = query.Where(x =>
                    (x.MatchAddressTxt != null && EF.Functions.ILike(x.MatchAddressTxt, allPattern)) ||
                    (x.PlatformListingUrl != null && EF.Functions.ILike(x.PlatformListingUrl, allPattern)) ||
                    (x.PlatformListingNo != null && EF.Functions.ILike(x.PlatformListingNo, allPattern)) ||
                    (x.EffectiveBusinessLicenceNo != null && x.EffectiveBusinessLicenceNo.StartsWith(sanitizedAll)) ||
                    (x.BcRegistryNo != null && EF.Functions.ILike(x.BcRegistryNo, allPattern)) ||
                    (x.EffectiveHostNm != null && EF.Functions.ILike(x.EffectiveHostNm, sanitizedAllPattern)));
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
                var hostNamePattern = $"%{CommonUtils.SanitizeAndUppercaseString(hostName)}%";
                query = query.Where(x => x.EffectiveHostNm != null && EF.Functions.ILike(x.EffectiveHostNm, hostNamePattern));
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
                reassigned = null;
            if (takedownComplete != null && takedownComplete.Value == false)
                takedownComplete = null;

            if (reassigned != null && takedownComplete != null)
                query = query.Where(x => x.IsTakenDown == true || x.IsLgTransferred == true);
            else if (reassigned != null)
                query = query.Where(x => x.IsLgTransferred == true);
            else if (takedownComplete != null)
                query = query.Where(x => x.IsTakenDown == true);

            if (lgId != null)
                query = query.Where(x => x.ManagingOrganizationId == lgId);

            if (statusArray.Length > 0)
                query = query.Where(x => x.ListingStatusType != null && statusArray.Contains(x.ListingStatusType));
        }

        /// <summary>
        /// Applies server-side sort for the listings table using explicit OrderBy/OrderByDescending
        /// so EF translates to SQL ORDER BY (avoids client evaluation from DynamicOrderBy).
        /// </summary>
        private static IQueryable<RentalListingTableRowDto> ApplyOrderByTable(IQueryable<RentalListingTableRowDto> query, string orderBy, string direction)
        {
            var isDesc = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);
            var key = orderBy ?? "latestReportPeriodYm";

            return key switch
            {
                "businessLicenceNo" => isDesc
                    ? query.OrderByDescending(x => x.BusinessLicenceNo ?? "ZZZZ")
                    : query.OrderBy(x => x.BusinessLicenceNo ?? "ZZZZ"),
                "businessLicenceNoMatched" => isDesc
                    ? query.OrderByDescending(x => x.BusinessLicenceNoMatched ?? "ZZZZ")
                    : query.OrderBy(x => x.BusinessLicenceNoMatched ?? "ZZZZ"),
                "nightsBookedYtdQty" => isDesc
                    ? query.OrderByDescending(x => x.NightsBookedYtdQty ?? -1)
                    : query.OrderBy(x => x.NightsBookedYtdQty ?? -1),
                "registrationNumber" => isDesc
                    ? query.OrderByDescending(x => x.BcRegistryNo ?? "ZZZZ")
                    : query.OrderBy(x => x.BcRegistryNo ?? "ZZZZ"),
                "latestReportPeriodYm" => isDesc
                    ? query.OrderByDescending(x => x.LatestReportPeriodYm ?? DateOnly.MinValue)
                    : query.OrderBy(x => x.LatestReportPeriodYm ?? DateOnly.MinValue),
                "matchAddressTxt" => isDesc
                    ? query.OrderByDescending(x => x.MatchAddressTxt ?? "ZZZZ")
                    : query.OrderBy(x => x.MatchAddressTxt ?? "ZZZZ"),
                "platformListingNo" => isDesc
                    ? query.OrderByDescending(x => x.PlatformListingNo ?? "ZZZZ")
                    : query.OrderBy(x => x.PlatformListingNo ?? "ZZZZ"),
                _ => query.OrderByDescending(x => x.LatestReportPeriodYm ?? DateOnly.MinValue)
            };
        }

        public async Task<int> GetRentalListingsCountAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent)
        {
            var query = GetRentalListingsTableBaseQuery();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            if (recent)
            {
                query = ApplyRecentFilterTable(query);
            }

            ApplyFiltersTable(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, ref query);

            return await query.CountAsync();
        }

        public async Task<PagedDto<RentalListingTableRowDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction, bool includeTotalCount = true)
        {
            var stopwatch = Stopwatch.StartNew();

            var query = GetRentalListingsTableBaseQuery();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            // Apply recent filter if requested for data retrieval
            if (recent)
            {
                query = ApplyRecentFilterTable(query);
            }
            _logger.LogInformation($"Get Rental Listings - Total Listings Fetched, Recent: {recent}, Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            // Apply all other filters for data retrieval
            ApplyFiltersTable(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, ref query);

            int? countAfterFilters = null;
            if (includeTotalCount)
            {
                countAfterFilters = await query.CountAsync();
                _logger.LogInformation($"Get Rental Listings - Total Listings After Filter: {countAfterFilters}, Time: {stopwatch.Elapsed.TotalSeconds} seconds");
            }

            // Whitelist sort: only columns used by the frontend table
            if (string.IsNullOrEmpty(orderBy) || !AllowedSortColumns.Contains(orderBy))
            {
                orderBy = "latestReportPeriodYm";
                direction = "desc";
            }

            // Sort and page at DB with explicit OrderBy (server-side; no DynamicOrderBy/client eval)
            var ordered = ApplyOrderByTable(query, orderBy, direction);
            if (pageNumber <= 0) pageNumber = 1;
            var skipRecordCount = pageSize > 0 ? (pageNumber - 1) * pageSize : 0;
            var pagedQuery = pageSize > 0
                ? ordered.Skip(skipRecordCount).Take(pageSize)
                : ordered;
            var pagedList = await pagedQuery.ToListAsync();

            _logger.LogInformation($"Get Rental Listings - Total Listings After Paging: {pagedList.Count}, Page: {pageNumber}, PageSize: {pageSize}, Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            // Hydrate email action fields for the paged results only (avoids expensive full-table aggregation)
            await HydrateLastActionFieldsAsync(pagedList);

            stopwatch.Stop();
            _logger.LogInformation($"Get Rental Listings - Final Mapping and Processing Completed, Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            return new PagedDto<RentalListingTableRowDto>
            {
                SourceList = pagedList,
                PageInfo = new PageInfo
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = countAfterFilters ?? 0,
                    OrderBy = orderBy,
                    Direction = direction,
                    ItemCount = pagedList.Count
                }
            };
        }

        private async Task<List<RentalListingTableRowDto>> GetFilteredListingTableRowsAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, bool hydrateLastAction = true, bool normalizeGroupingKeys = true)
        {
            var query = GetRentalListingsTableBaseQuery();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            if (recent)
            {
                query = ApplyRecentFilterTable(query);
            }

            ApplyFiltersTable(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, ref query);

            var list = await query.ToListAsync();
            if (hydrateLastAction)
            {
                await HydrateLastActionFieldsAsync(list);
            }
            if (normalizeGroupingKeys)
            {
                NormalizeGroupingKeyFields(list);
            }
            return list;
        }

        /// <summary>
        /// Align grouped parent keys with expand (grouped/listings): trim whitespace and treat blank as null
        /// so listing counts and child fetch use the same identity (avoids empty expand when DB has padded values).
        /// </summary>
        private static void NormalizeGroupingKeyFields(List<RentalListingTableRowDto> rows)
        {
            foreach (var row in rows)
            {
                row.BcRegistryNo = string.IsNullOrWhiteSpace(row.BcRegistryNo) ? null : row.BcRegistryNo.Trim();
                row.MatchAddressTxt = string.IsNullOrWhiteSpace(row.MatchAddressTxt) ? null : row.MatchAddressTxt.Trim();
                row.MatchUnitNo = string.IsNullOrWhiteSpace(row.MatchUnitNo) ? null : row.MatchUnitNo.Trim();
                row.EffectiveHostNm = string.IsNullOrWhiteSpace(row.EffectiveHostNm) ? null : row.EffectiveHostNm.Trim();
                row.EffectiveBusinessLicenceNo = string.IsNullOrWhiteSpace(row.EffectiveBusinessLicenceNo) ? null : row.EffectiveBusinessLicenceNo.Trim();
            }
        }

        private const string NoRegGroupKeySeparator = "\u001e";

        /// <summary>No-reg groups: platform best match address + unit + effective host (listing) + normalized listing BL.</summary>
        private static string EncodeNoRegGroupKey(string? matchAddress, string? unitNo, string? effectiveHost, string? blNormalized)
        {
            static string Cell(string? s) => s ?? "";
            return string.Join(NoRegGroupKeySeparator, Cell(matchAddress), Cell(unitNo), Cell(effectiveHost?.ToUpperInvariant()), Cell(blNormalized));
        }

        private static string BuildNoRegCanonicalGroupingKey(RentalListingTableRowDto row)
        {
            var addr = string.IsNullOrWhiteSpace(row.MatchAddressTxt) ? null : row.MatchAddressTxt.Trim();
            var unit = string.IsNullOrWhiteSpace(row.MatchUnitNo) ? null : row.MatchUnitNo.Trim();
            var host = string.IsNullOrWhiteSpace(row.EffectiveHostNm) ? null : row.EffectiveHostNm.Trim();
            var bl = CommonUtils.NormalizeBusinessLicenceForAggregation(row.BusinessLicenceNo);
            return EncodeNoRegGroupKey(addr, unit, host, bl);
        }

        private static string BuildNoRegCanonicalKeyFromExpandParams(string? matchAddressTxt, string? matchUnitNo, string? effectiveHostNm, string? listingBusinessLicenceNo)
        {
            var addr = NormalizeExpandKeyPart(matchAddressTxt);
            var unit = NormalizeExpandKeyPart(matchUnitNo);
            var hostNorm = NormalizeExpandKeyPart(effectiveHostNm);
            var bl = CommonUtils.NormalizeBusinessLicenceForAggregation(listingBusinessLicenceNo);
            return EncodeNoRegGroupKey(addr, unit, hostNorm, bl);
        }

        private static string? NormalizeExpandKeyPart(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static List<RentalListingGroupSummaryDto> BuildGroupedSummaries(List<RentalListingTableRowDto> rows)
        {
            var withReg = rows.Where(r => !string.IsNullOrWhiteSpace(r.BcRegistryNo)).ToList();
            var withoutReg = rows.Where(r => string.IsNullOrWhiteSpace(r.BcRegistryNo)).ToList();

            var summaries = new List<RentalListingGroupSummaryDto>(withReg.Count / 2 + withoutReg.Count / 2 + 4);

            foreach (var g in withReg.GroupBy(r => r.BcRegistryNo!.Trim()))
            {
                summaries.Add(BuildOneGroupSummary(g.ToList(), bcRegistryNo: g.Key));
            }

            foreach (var g in withoutReg.GroupBy(BuildNoRegCanonicalGroupingKey))
            {
                summaries.Add(BuildOneGroupSummary(g.ToList(), bcRegistryNo: null));
            }

            return summaries;
        }

        private static RentalListingGroupSummaryDto BuildOneGroupSummary(List<RentalListingTableRowDto> groupList, string? bcRegistryNo)
        {
            var first = groupList[0];
            var anchor = groupList
                .OrderByDescending(r => r.LastActionDtm ?? DateTime.MinValue)
                .ThenByDescending(r => r.LatestReportPeriodYm ?? DateOnly.MinValue)
                .First();

            DateOnly? maxReport = null;
            foreach (var r in groupList)
            {
                if (r.LatestReportPeriodYm > maxReport || maxReport == null)
                {
                    maxReport = r.LatestReportPeriodYm;
                }
            }

            return new RentalListingGroupSummaryDto
            {
                ListingCount = groupList.Count,
                BcRegistryNo = bcRegistryNo,
                EffectiveBusinessLicenceNo = first.BusinessLicenceNo,
                EffectiveHostNm = first.EffectiveHostNm,
                MatchAddressTxt = first.MatchAddressTxt,
                MatchUnitNo = first.MatchUnitNo,
                PrimaryHostNm = first.EffectiveHostNm,
                NightsBookedYtdQty = groupList.Sum(r => r.NightsBookedYtdQty ?? 0),
                BusinessLicenceNo = anchor.BusinessLicenceNoMatched ?? anchor.BusinessLicenceNo,
                LastActionNm = anchor.LastActionNm,
                LastActionDtm = anchor.LastActionDtm,
                LatestReportPeriodYm = maxReport,
                BusinessLicenceId = anchor.BusinessLicenceId,
                BusinessLicenceExpiryDt = anchor.BusinessLicenceExpiryDt,
                LicenceStatusType = anchor.LicenceStatusType,
                HasMultipleProperties = false,
                AnchorRentalListingId = anchor.RentalListingId
            };
        }

        private static List<RentalListingGroupSummaryDto> OrderGroupedSummaries(List<RentalListingGroupSummaryDto> summaries, string orderBy, string direction)
        {
            var desc = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(orderBy) || !GroupedAllowedSortColumns.Contains(orderBy))
            {
                orderBy = "latestReportPeriodYm";
                desc = true;
            }

            // NULLS LAST in BOTH directions so toggling asc/desc never anchors null-host groups to the top of
            // page 1. The previous "?? \"\"" / "?? \"\\uFFFF\"" fallbacks pinned nulls to the top of asc AND
            // desc — combined with the BuildOneGroupSummary issue that produced spurious-null EffectiveHostNm,
            // the user saw a block of rows whose displayed PrimaryHostNm (hydrated from contacts) was real but
            // whose sort key was null, and toggling direction didn't reorder them. Pinned to StringComparer.Ordinal
            // so the result is byte-deterministic across server cultures.
            return orderBy switch
            {
                "effectiveHostNm" => desc
                    ? summaries.OrderBy(s => s.EffectiveHostNm == null)
                               .ThenByDescending(s => s.EffectiveHostNm, StringComparer.Ordinal).ToList()
                    : summaries.OrderBy(s => s.EffectiveHostNm == null)
                               .ThenBy(s => s.EffectiveHostNm, StringComparer.Ordinal).ToList(),
                "matchAddressTxt" => desc
                    ? summaries.OrderBy(s => s.MatchAddressTxt == null)
                               .ThenByDescending(s => s.MatchAddressTxt, StringComparer.Ordinal).ToList()
                    : summaries.OrderBy(s => s.MatchAddressTxt == null)
                               .ThenBy(s => s.MatchAddressTxt, StringComparer.Ordinal).ToList(),
                "effectiveBusinessLicenceNo" => desc
                    ? summaries.OrderBy(s => s.EffectiveBusinessLicenceNo == null)
                               .ThenByDescending(s => s.EffectiveBusinessLicenceNo, StringComparer.Ordinal).ToList()
                    : summaries.OrderBy(s => s.EffectiveBusinessLicenceNo == null)
                               .ThenBy(s => s.EffectiveBusinessLicenceNo, StringComparer.Ordinal).ToList(),
                _ => desc
                    ? summaries.OrderBy(s => s.LatestReportPeriodYm == null)
                               .ThenByDescending(s => s.LatestReportPeriodYm).ToList()
                    : summaries.OrderBy(s => s.LatestReportPeriodYm == null)
                               .ThenBy(s => s.LatestReportPeriodYm).ToList()
            };
        }

        public async Task<int> GetGroupedRentalListingsCountAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent)
        {
            var rows = await GetFilteredListingTableRowsAsync(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, recent, hydrateLastAction: false, normalizeGroupingKeys: false);
            var summaries = BuildGroupedSummaries(rows);
            return summaries.Count;
        }

        public async Task<PagedDto<RentalListingGroupSummaryDto>> GetGroupedRentalListingsPagedAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction, bool includeTotalCount = true)
        {
            var rows = await GetFilteredListingTableRowsAsync(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, recent);
            var summaries = BuildGroupedSummaries(rows);

            if (string.IsNullOrEmpty(orderBy) || !GroupedAllowedSortColumns.Contains(orderBy))
            {
                orderBy = "latestReportPeriodYm";
                direction = "desc";
            }

            var ordered = OrderGroupedSummaries(summaries, orderBy, direction);
            var total = ordered.Count;

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            var skip = pageSize > 0 ? (pageNumber - 1) * pageSize : 0;
            var page = pageSize > 0 ? ordered.Skip(skip).Take(pageSize).ToList() : ordered;

            // Replace sanitized/uppercased EffectiveHostNm fallback with original-case
            // property-owner contact name. Runs against the paged anchors only (~pageSize rows).
            await HydratePrimaryHostNmAsync(page);

            return new PagedDto<RentalListingGroupSummaryDto>
            {
                SourceList = page,
                PageInfo = new PageInfo
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = includeTotalCount ? total : 0,
                    OrderBy = orderBy,
                    Direction = direction,
                    ItemCount = page.Count
                }
            };
        }

        /// <summary>
        /// Expand: uncached. Mode A: bcRegistryNo (SQL filter). Mode B: same in-memory key as no-reg grouping (address + unit + effective host + normalized listing BL).
        /// </summary>
        public async Task<List<RentalListingTableRowDto>> GetGroupedListingChildrenAsync(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, string? bcRegistryNo, string? matchAddressTxt, string? matchUnitNo, string? effectiveHostNm, string? effectiveBusinessLicenceNo)
        {
            if (!string.IsNullOrWhiteSpace(bcRegistryNo))
            {
                var query = GetRentalListingsTableBaseQuery();

                if (_currentUser.OrganizationType == OrganizationTypes.LG)
                {
                    query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
                }

                if (recent)
                {
                    query = ApplyRecentFilterTable(query);
                }

                ApplyFiltersTable(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, ref query);

                var reg = bcRegistryNo.Trim();
                query = query.Where(x => x.BcRegistryNo != null && x.BcRegistryNo.Trim() == reg);
                query = query.OrderByDescending(x => x.LatestReportPeriodYm ?? DateOnly.MinValue);

                var list = await query.ToListAsync();
                await HydrateLastActionFieldsAsync(list);
                return list;
            }

            var targetKey = BuildNoRegCanonicalKeyFromExpandParams(matchAddressTxt, matchUnitNo, effectiveHostNm, effectiveBusinessLicenceNo);
            var rows = await GetFilteredListingTableRowsAsync(all, address, url, listingId, hostName, businessLicence, registrationNumber, prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, recent);

            return rows
                .Where(r => string.IsNullOrWhiteSpace(r.BcRegistryNo))
                .Where(r => BuildNoRegCanonicalGroupingKey(r) == targetKey)
                .OrderByDescending(x => x.LatestReportPeriodYm ?? DateOnly.MinValue)
                .ToList();
        }

        public async Task<Dictionary<string, bool>> BatchEffectiveHostHasMultiplePropertiesAsync(IReadOnlyList<string?> effectiveHostNms)
        {
            var normalized = effectiveHostNms
                .Where(h => !string.IsNullOrEmpty(h))
                .Select(h => h!)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (normalized.Count == 0)
            {
                return new Dictionary<string, bool>(StringComparer.Ordinal);
            }

            var query = GetRentalListingsTableBaseQuery();

            if (_currentUser.OrganizationType == OrganizationTypes.LG)
            {
                query = query.Where(x => x.ManagingOrganizationId == _currentUser.OrganizationId);
            }

            var tuples = await query
                .Where(x => x.EffectiveHostNm != null && normalized.Contains(x.EffectiveHostNm))
                .Select(x => new { x.EffectiveHostNm, x.EffectiveBusinessLicenceNo, x.MatchAddressTxt })
                .ToListAsync();

            var result = normalized.ToDictionary(h => h, _ => false, StringComparer.Ordinal);
            foreach (var g in tuples.GroupBy(t => t.EffectiveHostNm!, StringComparer.Ordinal))
            {
                var distinct = g.Select(t => (t.EffectiveBusinessLicenceNo, t.MatchAddressTxt)).Distinct().Count();
                result[g.Key] = distinct > 1;
            }

            return result;
        }

        public async Task<int> CountHostListingsAsync(string hostName)
        {
            var query = GetRentalListingsTableBaseQuery();

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
                    IsTakedownActionSuspended = x.IsTakedownActionSuspended
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

        public async Task<bool> DismissIdUpdatedStatusAsync(long rentalListingId)
        {
            var listing = await _dbContext.DssRentalListings
                .FirstOrDefaultAsync(x => x.RentalListingId == rentalListingId);

            if (listing == null || listing.ListingStatusType != "U")
                return false;

            listing.ListingStatusType = "A";
            return true;
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
