using Microsoft.EntityFrameworkCore;
using StrDss.Common;
using StrDss.Data.IntegrationTest.Interceptors;
using StrDss.Model.RentalReportDtos;
using Xunit;

namespace StrDss.Data.IntegrationTest;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Integration")]
public class RentalListingRepositoryShould : RentalListingQueryTestBase
{
    public RentalListingRepositoryShould(PostgresFixture fixture) : base(fixture) { }

    [Theory]
    [MemberData(nameof(FilterCases))]
    public async Task ListingQueries_ExecuteEachFilterWithoutExtraReads(string scenario)
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);
        var filters = GetFilters(scenario);

        var count = await CountListings(repository, filters);
        Assert.Equal(1, interceptor.ReadersExecuted);
        var page = await Listings(repository, filters, includeTotalCount: true);
        Assert.Equal(3, interceptor.ReadersExecuted);
        Assert.Equal(count, page.PageInfo.TotalCount);
        Assert.Equal(page.SourceList.Count(), page.PageInfo.ItemCount);
        Assert.InRange(page.PageInfo.ItemCount, 0, 10);
        Assert.Empty(context.ChangeTracker.Entries());
        if (scenario is "none" or "empty") Assert.NotEmpty(page.SourceList);
        if (scenario.StartsWith("quoted-")) Assert.Empty(page.SourceList);
    }

    public static IEnumerable<object[]> SortCases()
    {
        foreach (var column in new[] { "latestReportPeriodYm", "registrationNumber", "matchAddressTxt",
            "nightsBookedYtdQty", "businessLicenceNo", "businessLicenceNoMatched", "platformListingNo",
            "lastActionNm", "lastActionDtm" })
        {
            yield return new object[] { column };
        }
    }

    [Theory]
    [MemberData(nameof(SortCases))]
    public async Task GetRentalListings_SortsInBothDirectionsAtTheDatabase(string column)
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);

        var ascending = await Listings(repository, new(), pageSize: 0, orderBy: column, direction: "ASC");
        var descending = await Listings(repository, new(), pageSize: 0, orderBy: column, direction: "DESC");

        Assert.NotEmpty(ascending.SourceList);
        Assert.Equal(2, interceptor.ReadersExecuted);
        Assert.Equal(ascending.SourceList.Select(x => SortKey(x, column)).Distinct(),
            descending.SourceList.Select(x => SortKey(x, column)).Distinct().Reverse());
        Assert.Empty(context.ChangeTracker.Entries());
    }

    private static object SortKey(RentalListingTableRowDto row, string column) => column switch
    {
        "latestReportPeriodYm" => row.LatestReportPeriodYm ?? DateOnly.MinValue,
        "registrationNumber" => row.BcRegistryNo ?? "ZZZZ",
        "matchAddressTxt" => row.MatchAddressTxt ?? "ZZZZ",
        "nightsBookedYtdQty" => row.NightsBookedYtdQty ?? -1,
        "businessLicenceNo" => row.BusinessLicenceNo ?? "ZZZZ",
        "businessLicenceNoMatched" => row.BusinessLicenceNoMatched ?? "ZZZZ",
        "platformListingNo" => row.PlatformListingNo ?? "ZZZZ",
        "lastActionNm" => row.LastActionNm ?? "ZZZZ",
        "lastActionDtm" => row.LastActionDtm ?? DateTime.MinValue,
        _ => throw new ArgumentOutOfRangeException(nameof(column))
    };

    [Theory]
    [InlineData(10, 0, 1)]
    [InlineData(10, -1, 1)]
    [InlineData(10, 2, 2)]
    [InlineData(0, 1, 1)]
    [InlineData(-1, 1, 1)]
    [InlineData(10, int.MaxValue / 10, int.MaxValue / 10)]
    public async Task GetRentalListings_HandlesPagingAndOptionalCount(int size, int number, int expectedNumber)
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);
        var page = await Listings(repository, new(), size, number, "platformListingNo", "asc");

        Assert.Equal(1, interceptor.ReadersExecuted);
        Assert.Equal(expectedNumber, page.PageInfo.PageNumber);
        Assert.Equal(size, page.PageInfo.PageSize);
        Assert.Equal(0, page.PageInfo.TotalCount);
        Assert.Equal(page.SourceList.Count(), page.PageInfo.ItemCount);
        if (size > 0) Assert.InRange(page.PageInfo.ItemCount, 0, size);
        else Assert.NotEmpty(page.SourceList);

        var all = await Listings(repository, new(), 0, orderBy: "platformListingNo", direction: "asc");
        Assert.Equal(all.SourceList.Skip(size > 0 ? (expectedNumber - 1) * size : 0)
            .Take(size > 0 ? size : int.MaxValue).Select(x => x.RentalListingId),
            page.SourceList.Select(x => x.RentalListingId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("notAColumn")]
    [InlineData("lastActionDtm; DROP TABLE dss_rental_listing; --")]
    public async Task GetRentalListings_FallsBackToSafeDefaultSort(string sort)
    {
        await using var context = CreateDbContext();
        var page = await Listings(CreateRepository(context), new(), orderBy: sort, direction: "asc");

        Assert.NotEmpty(page.SourceList);
        Assert.Equal("latestReportPeriodYm", page.PageInfo.OrderBy);
        Assert.Equal("desc", page.PageInfo.Direction);
        Assert.Equal(page.SourceList.Select(x => x.LatestReportPeriodYm ?? DateOnly.MinValue)
            .OrderByDescending(x => x), page.SourceList.Select(x => x.LatestReportPeriodYm ?? DateOnly.MinValue));
    }

    [Fact]
    public async Task ListingQueries_ApplySanitizedHostAndLicenceSearches()
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var host = await Listings(repository, new(HostName: "Teresa Homeowner"), pageSize: 0);
        var licence = await Listings(repository, new(BusinessLicence: "BL-VanLOAD"), pageSize: 0);

        Assert.NotEmpty(host.SourceList);
        Assert.NotEmpty(licence.SourceList);
        Assert.All(host.SourceList, x => Assert.Contains("TERESAHOMEOWNER", x.EffectiveHostNm!));
        Assert.All(licence.SourceList, x => Assert.StartsWith("BLVANLOAD", x.EffectiveBusinessLicenceNo!));
    }

    [Fact]
    public async Task ListingQueries_RestrictLocalGovernmentUsersAndHostCounts()
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var all = await Listings(repository, new(), pageSize: 0);
        var organization = all.SourceList.First(x => x.ManagingOrganizationId.HasValue).ManagingOrganizationId!.Value;
        CurrentUser.OrganizationType = OrganizationTypes.LG;
        CurrentUser.OrganizationId = organization;

        var scoped = await Listings(repository, new(), pageSize: 0);
        Assert.NotEmpty(scoped.SourceList);
        Assert.All(scoped.SourceList, x => Assert.Equal(organization, x.ManagingOrganizationId));
        Assert.Equal(scoped.SourceList.Count(), await CountListings(repository, new()));
        var host = scoped.SourceList.First(x => !string.IsNullOrEmpty(x.EffectiveHostNm)).EffectiveHostNm!;
        var expectedProperties = scoped.SourceList.Where(x => x.EffectiveHostNm == host)
            .Select(x => new { x.EffectiveBusinessLicenceNo, x.EffectiveHostNm, x.MatchAddressTxt }).Distinct().Count();
        Assert.Equal(expectedProperties, await repository.CountHostListingsAsync(host));

        Assert.Equal(0, await CountListings(repository, new(LgId: long.MaxValue)));
    }

    [Theory]
    [InlineData("TERESAHOMEOWNER")]
    [InlineData("missing' OR 1=1; --")]
    [InlineData("")]
    public async Task CountHostListingsAsync_ExecutesOneDatabaseRead(string host)
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var count = await CreateRepository(context).CountHostListingsAsync(host);
        Assert.Equal(1, interceptor.ReadersExecuted);
        Assert.True(count >= 0);
        Assert.Empty(context.ChangeTracker.Entries());
    }
}
