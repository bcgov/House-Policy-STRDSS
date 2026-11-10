using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StrDss.Common;
using StrDss.Data.IntegrationTest.Interceptors;
using StrDss.Model;
using Xunit;

namespace StrDss.Data.IntegrationTest;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Integration")]
public class GroupedRentalListingRepositoryShould : RentalListingQueryTestBase
{
    public GroupedRentalListingRepositoryShould(PostgresFixture fixture) : base(fixture) { }

    [Theory]
    [MemberData(nameof(FilterCases))]
    public async Task GroupedQueries_ExecuteDynamicFilterBranchesAndBothChildPredicates(string scenario)
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);
        var filters = GetFilters(scenario);

        var count = await CountGroups(repository, filters);
        Assert.Equal(0, interceptor.ReadersExecuted);
        var page = await Groups(repository, filters, includeTotalCount: true);
        Assert.Equal(count, page.PageInfo.TotalCount);
        Assert.Equal(page.SourceList.Count(), page.PageInfo.ItemCount);
        Assert.InRange(page.PageInfo.ItemCount, 0, 10);
        Assert.Equal(0, interceptor.ReadersExecuted);

        await Children(repository, filters, registration: "987654");
        await Children(repository, filters);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
        Assert.Empty(context.ChangeTracker.Entries());
        if (scenario is "none" or "empty") Assert.NotEmpty(page.SourceList);
        if (scenario.StartsWith("quoted-")) Assert.Empty(page.SourceList);
    }

    public static IEnumerable<object[]> SortCases()
    {
        foreach (var column in new[] { "latestReportPeriodYm", "effectiveHostNm", "matchAddressTxt",
            "effectiveBusinessLicenceNo", "lastActionNm", "lastActionDtm" })
        {
            yield return new object[] { column };
        }
    }

    private static object? SortKey(RentalListingGroupSummaryDto row, string column) => column switch
    {
        "latestReportPeriodYm" => row.LatestReportPeriodYm,
        "effectiveHostNm" => row.EffectiveHostNm,
        "matchAddressTxt" => row.MatchAddressTxt,
        "effectiveBusinessLicenceNo" => row.EffectiveBusinessLicenceNo,
        "lastActionNm" => row.LastActionNm,
        "lastActionDtm" => row.LastActionDtm,
        _ => throw new ArgumentOutOfRangeException(nameof(column))
    };

    [Theory]
    [MemberData(nameof(SortCases))]
    public async Task GetGroupedRentalListingsPagedAsync_SortsBothDirectionsIncludingNullPlacement(string column)
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var ascending = await Groups(repository, new(), pageSize: 0, orderBy: column, direction: "ASC");
        var descending = await Groups(repository, new(), pageSize: 0, orderBy: column, direction: "DESC");

        Assert.NotEmpty(ascending.SourceList);
        Assert.Equal(ascending.SourceList.Select(x => SortKey(x, column)).Distinct(),
            descending.SourceList.Select(x => SortKey(x, column)).Distinct().Reverse());
        var ascKeys = ascending.SourceList.Select(x => SortKey(x, column)).ToList();
        var descKeys = descending.SourceList.Select(x => SortKey(x, column)).ToList();
        Assert.DoesNotContain(ascKeys.SkipWhile(x => x == null), x => x == null);
        Assert.DoesNotContain(descKeys.TakeWhile(x => x != null), x => x == null);
        Assert.All(descKeys.SkipWhile(x => x != null), x => Assert.Null(x));
    }

    [Theory]
    [InlineData(10, 0, false)]
    [InlineData(10, -1, true)]
    [InlineData(10, 2, true)]
    [InlineData(0, 1, false)]
    [InlineData(-1, 1, true)]
    [InlineData(10, int.MaxValue / 10, false)]
    public async Task GetGroupedRentalListingsPagedAsync_PagesDeterministicallyAndOptionallyCounts(int size, int number, bool includeCount)
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var all = await Groups(repository, new(), pageSize: 0, includeTotalCount: true);
        var page = await Groups(repository, new(), size, number, includeTotalCount: includeCount);
        var repeat = await Groups(repository, new(), size, number, includeTotalCount: includeCount);
        var expectedNumber = Math.Max(1, number);

        Assert.NotEmpty(all.SourceList);
        Assert.Equal(expectedNumber, page.PageInfo.PageNumber);
        Assert.Equal(size, page.PageInfo.PageSize);
        Assert.Equal(includeCount ? all.PageInfo.TotalCount : 0, page.PageInfo.TotalCount);
        Assert.Equal(all.SourceList.Skip(size > 0 ? (expectedNumber - 1) * size : 0)
            .Take(size > 0 ? size : int.MaxValue).Select(x => x.AnchorRentalListingId),
            page.SourceList.Select(x => x.AnchorRentalListingId));
        Assert.Equal(page.SourceList.Select(x => x.AnchorRentalListingId), repeat.SourceList.Select(x => x.AnchorRentalListingId));
        Assert.Equal(page.SourceList.Count(), page.PageInfo.ItemCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("notAColumn")]
    [InlineData("lastActionDtm; DROP TABLE dss_rental_listing; --")]
    public async Task GetGroupedRentalListingsPagedAsync_RejectsUntrustedSortColumns(string column)
    {
        await using var context = CreateDbContext();
        var page = await Groups(CreateRepository(context), new(), orderBy: column, direction: "asc");
        Assert.NotEmpty(page.SourceList);
        Assert.Equal("latestReportPeriodYm", page.PageInfo.OrderBy);
        Assert.Equal("desc", page.PageInfo.Direction);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetGroupedListingChildrenAsync_RoundTripsNormalizedGroupKeysAndOrdersChildren(bool withRegistration)
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var groups = await Groups(repository, new(), pageSize: 0);
        var group = groups.SourceList.First(x => withRegistration
            ? x.BcRegistryNo != null
            : x.BcRegistryNo == null && x.EffectiveBusinessLicenceNo != null && x.EffectiveHostNm != null);
        var children = await Children(repository, new(), group.BcRegistryNo, group.MatchAddressTxt,
            group.MatchUnitNo, group.EffectiveHostNm, group.EffectiveBusinessLicenceNo);
        var equivalent = await Children(repository, new(), Pad(group.BcRegistryNo), Pad(group.MatchAddressTxt),
            Pad(group.MatchUnitNo), Pad(group.EffectiveHostNm), withRegistration ? "ignored" :
            "BL-" + Regex.Replace(group.EffectiveBusinessLicenceNo!, "[^0-9]", "") + "-suffix");

        Assert.NotEmpty(children);
        Assert.Equal(group.ListingCount, children.Count);
        Assert.Equal(children.Select(x => x.RentalListingId), equivalent.Select(x => x.RentalListingId));
        Assert.Equal(children.OrderByDescending(x => x.LatestReportPeriodYm).ThenBy(x => x.RentalListingId)
            .Select(x => x.RentalListingId), children.Select(x => x.RentalListingId));
        Assert.All(children, x =>
        {
            Assert.Equal(group.BcRegistryNo, x.BcRegistryNo);
            Assert.Equal(x.MatchAddressTxt?.Trim(), x.MatchAddressTxt);
            Assert.Equal(x.BusinessLicenceNo?.Trim(), x.BusinessLicenceNo);
        });
        Assert.Equal(children.Sum(x => x.NightsBookedYtdQty ?? 0), group.NightsBookedYtdQty);
        Assert.Contains(children, x => x.RentalListingId == group.AnchorRentalListingId);
    }

    private static string Pad(string? value) => value == null ? "   " : $"  {value}  ";

    [Theory]
    [InlineData("null")]
    [InlineData("empty")]
    [InlineData("whitespace")]
    [InlineData("quoted")]
    public async Task GetGroupedListingChildrenAsync_ExecutesTypedNullableAndQuotedKeyParameters(string scenario)
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var value = scenario switch { "null" => null, "empty" => "", "whitespace" => " ", _ => "missing' OR 1=1; --" };
        var rows = await Children(repository, new(), value, value, value, value, value);
        if (scenario == "quoted") Assert.Empty(rows);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Fact]
    public async Task GetGroupedRentalListingsPagedAsync_HydratesOnlyPageAnchorsAndComputesHostFlags()
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);
        var page = await Groups(repository, new(), pageSize: 20);
        Assert.NotEmpty(page.SourceList);
        Assert.Equal(0, interceptor.ReadersExecuted);
        var anchorIds = page.SourceList.Select(x => x.AnchorRentalListingId).ToList();
        var contacts = await context.DssRentalListingContacts.AsNoTracking()
            .Where(x => x.IsPropertyOwner && anchorIds.Contains(x.ContactedThroughRentalListingId))
            .OrderBy(x => x.RentalListingContactId).ToListAsync();
        Assert.NotEmpty(contacts);
        foreach (var group in page.SourceList)
        {
            var contact = contacts.FirstOrDefault(x => x.ContactedThroughRentalListingId == group.AnchorRentalListingId);
            Assert.Equal(!string.IsNullOrWhiteSpace(contact?.FullNm) ? contact.FullNm.Trim() : group.EffectiveHostNm, group.PrimaryHostNm);
            if (!string.IsNullOrEmpty(group.EffectiveHostNm))
                Assert.Equal(await repository.CountHostListingsAsync(group.EffectiveHostNm) > 1, group.HasMultipleProperties);
        }
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GroupedQueries_ScopeGroupsChildrenAndHostFlagsToTheCurrentLocalGovernment()
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var all = await Listings(repository, new(), pageSize: 0);
        CurrentUser.OrganizationType = OrganizationTypes.LG;
        CurrentUser.OrganizationId = all.SourceList.First(x => x.ManagingOrganizationId.HasValue
            && !string.IsNullOrEmpty(x.EffectiveHostNm)).ManagingOrganizationId!.Value;
        var scoped = await Listings(repository, new(), pageSize: 0);
        var ids = scoped.SourceList.Select(x => x.RentalListingId).ToHashSet();
        var page = await Groups(repository, new(), pageSize: 20, includeTotalCount: true);
        Assert.NotEmpty(page.SourceList);
        Assert.Equal(await CountGroups(repository, new()), page.PageInfo.TotalCount);
        foreach (var group in page.SourceList)
        {
            var children = await Children(repository, new(), group.BcRegistryNo, group.MatchAddressTxt,
                group.MatchUnitNo, group.EffectiveHostNm, group.EffectiveBusinessLicenceNo);
            Assert.NotEmpty(children);
            Assert.All(children, x => Assert.Contains(x.RentalListingId, ids));
            Assert.Equal(group.ListingCount, children.Count);
            if (!string.IsNullOrEmpty(group.EffectiveHostNm))
                Assert.Equal(await repository.CountHostListingsAsync(group.EffectiveHostNm) > 1, group.HasMultipleProperties);
        }
        Assert.Equal(0, await CountGroups(repository, new(LgId: long.MaxValue)));
    }

    [Theory]
    [InlineData("summary-licence-id")]
    [InlineData("summary-licence-expiry")]
    [InlineData("child-licence-id")]
    public async Task GroupedQueries_PreserveNullableLicenceColumns(string column)
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var groups = await Groups(repository, new(), pageSize: 0);
        var group = groups.SourceList.First();
        var children = await Children(repository, new(), group.BcRegistryNo, group.MatchAddressTxt,
            group.MatchUnitNo, group.EffectiveHostNm, group.EffectiveBusinessLicenceNo);
        var ids = children.Select(x => x.RentalListingId).ToList();
        var source = await context.DssRentalListings.AsNoTracking()
            .Where(x => ids.Contains(x.RentalListingId)).ToListAsync();
        Assert.NotEmpty(source);
        Assert.All(source, x => Assert.Null(x.GoverningBusinessLicenceId));

        switch (column)
        {
            case "summary-licence-id": Assert.Null(group.BusinessLicenceId); break;
            case "summary-licence-expiry": Assert.Null(group.BusinessLicenceExpiryDt); break;
            case "child-licence-id": Assert.All(children, x => Assert.Null(x.BusinessLicenceId)); break;
        }
    }

    [Fact]
    public async Task GroupedQueries_PreserveNullActionTimestampsInsteadOfMinimumDates()
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var groups = await Groups(repository, new(), pageSize: 0);
        var group = groups.SourceList.First(x => x.LastActionDtm == null);
        var children = await Children(repository, new(), group.BcRegistryNo, group.MatchAddressTxt,
            group.MatchUnitNo, group.EffectiveHostNm, group.EffectiveBusinessLicenceNo);
        Assert.NotEmpty(children);
        Assert.All(children, x => Assert.Null(x.LastActionDtm));
        Assert.Contains(groups.SourceList, x => x.LastActionDtm.HasValue);
        Assert.DoesNotContain(groups.SourceList, x => x.LastActionDtm == DateTime.MinValue);
    }
}
