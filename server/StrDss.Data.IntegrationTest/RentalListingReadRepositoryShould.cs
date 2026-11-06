using Microsoft.EntityFrameworkCore;
using StrDss.Common;
using StrDss.Data.IntegrationTest.Interceptors;
using Xunit;

namespace StrDss.Data.IntegrationTest;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Integration")]
public class RentalListingReadRepositoryShould : RentalListingQueryTestBase
{
    public RentalListingReadRepositoryShould(PostgresFixture fixture) : base(fixture) { }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetRentalListing_ExecutesOptionalHistoryAndOrdersIt(bool loadHistory)
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var result = await CreateRepository(context).GetRentalListing(401, loadHistory);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Hosts);
        if (!loadHistory)
        {
            Assert.Equal(2, interceptor.ReadersExecuted);
            return;
        }
        Assert.True(interceptor.ReadersExecuted >= 5);
        Assert.NotEmpty(result.ListingHistory);
        Assert.Equal(result.ListingHistory.OrderByDescending(x => x.ReportPeriodYM).Select(x => x.ReportPeriodYM),
            result.ListingHistory.Select(x => x.ReportPeriodYM));
        var months = result.ListingHistory.Select(x => DateOnly.ParseExact(x.ReportPeriodYM, "yyyy-MM")).ToList();
        for (var i = 1; i < months.Count; i++) Assert.Equal(months[i - 1].AddMonths(-1), months[i]);
        Assert.NotEmpty(result.ActionHistory);
        Assert.Equal(result.ActionHistory.Select(x => x.Date).OrderByDescending(x => x), result.ActionHistory.Select(x => x.Date));
        Assert.All(result.ActionHistory, x => Assert.False(string.IsNullOrWhiteSpace(x.User)));
        Assert.Equal(result.AddressChangeHistory.Select(x => x.Date).OrderByDescending(x => x),
            result.AddressChangeHistory.Select(x => x.Date));
    }

    [Fact]
    public async Task GetRentalListing_ConvertsActionTimeAndOverridesInvalidRegistrationActionName()
    {
        var source = await DbContext.DssRentalListingVws.AsNoTracking()
            .FirstAsync(x => x.TakeDownReason == TakeDownReasonStatus.InvalidRegistration && x.LastActionDtm != null);
        await using var context = CreateDbContext();
        var result = await CreateRepository(context).GetRentalListing(source.RentalListingId!.Value, false);
        Assert.NotNull(result);
        Assert.Equal("Reg Check Failed", result.LastActionNm);
        Assert.Equal(DateUtils.ConvertUtcToPacificTime(source.LastActionDtm!.Value), result.LastActionDtm);
    }

    [Fact]
    public async Task GetRentalListing_DeniesAnUnrelatedLocalGovernmentWithoutLoadingChildren()
    {
        CurrentUser.OrganizationType = OrganizationTypes.LG;
        CurrentUser.OrganizationId = long.MaxValue;
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        Assert.Null(await CreateRepository(context).GetRentalListing(401));
        Assert.Equal(1, interceptor.ReadersExecuted);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetRentalListingForTakedownAction_ExecutesOptionalHostEmailQuery(bool includeEmails)
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var result = await CreateRepository(context).GetRentalListingForTakedownAction(401, includeEmails);
        Assert.NotNull(result);
        Assert.Equal(includeEmails ? 3 : 2, interceptor.ReadersExecuted);
        if (includeEmails)
        {
            Assert.NotEmpty(result.HostEmails);
            Assert.All(result.HostEmails, x => Assert.NotNull(x));
        }
        else Assert.Empty(result.HostEmails);
        Assert.NotNull(result.PlatformEmails);
        Assert.True(result.ProvidingPlatformId > 0);
    }

    [Fact]
    public async Task GetRentalListingIdsToExport_ExecutesOneOrderedDatabaseRead()
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var ids = await CreateRepository(context).GetRentalListingIdsToExport();
        Assert.NotEmpty(ids);
        Assert.Equal(1, interceptor.ReadersExecuted);
        Assert.Empty(context.ChangeTracker.Entries());
        var source = await DbContext.DssRentalListingVws.AsNoTracking()
            .OrderBy(x => x.ManagingOrganizationId).ThenBy(x => x.IsPrincipalResidenceRequired)
            .Select(x => new { x.RentalListingId, x.ManagingOrganizationId, x.IsPrincipalResidenceRequired }).ToListAsync();
        var keys = source.ToDictionary(x => x.RentalListingId);
        Assert.Equal(source.Select(x => (x.ManagingOrganizationId, x.IsPrincipalResidenceRequired)),
            ids.Select(x => (keys[x].ManagingOrganizationId, keys[x].IsPrincipalResidenceRequired)));
    }

    [Fact]
    public async Task GetRentalListingToExport_ExecutesHistoryContactsAndActionQueries()
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var result = await CreateRepository(context).GetRentalListingToExport(401);
        Assert.NotNull(result);
        Assert.Equal(4, interceptor.ReadersExecuted);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetRentalListingToExport_OverridesInvalidRegistrationActionName()
    {
        var source = await DbContext.DssRentalListingVws.AsNoTracking()
            .FirstAsync(x => x.TakeDownReason == TakeDownReasonStatus.InvalidRegistration);
        await using var context = CreateDbContext();
        var result = await CreateRepository(context).GetRentalListingToExport(source.RentalListingId!.Value);
        Assert.NotNull(result);
        Assert.Equal("Reg Check Failed", result.LastActionNm);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExtractReads_ExecuteLookupsAndScopeTheExtractList(bool localGovernment)
    {
        var extract = await DbContext.DssRentalListingExtracts.AsNoTracking().FirstAsync();
        if (localGovernment)
        {
            CurrentUser.OrganizationType = OrganizationTypes.LG;
            CurrentUser.OrganizationId = extract.FilteringOrganizationId ?? 212;
        }
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);
        Assert.NotNull(await repository.GetRetalListingExportAsync(extract.RentalListingExtractId));
        Assert.NotNull(await repository.GetRetalListingExportByNameAsync(extract.RentalListingExtractNm));
        var list = await repository.GetRetalListingExportsAsync();
        Assert.All(list, x => Assert.Equal(localGovernment ? (long?)CurrentUser.OrganizationId : null, x.FilteringOrganizationId));
        Assert.Equal(3, interceptor.ReadersExecuted);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetAddressChangeHistoryAsync_LoadsAndOrdersAddressHistory()
    {
        await using var context = CreateDbContext();
        var history = await CreateRepository(context).GetAddressChangeHistoryAsync(401);
        Assert.NotEmpty(history);
        Assert.Equal(history.Select(x => x.Date).OrderByDescending(x => x), history.Select(x => x.Date));
        Assert.All(history, x => Assert.False(string.IsNullOrWhiteSpace(x.Type)));
    }

    [Fact]
    public async Task ReadQueries_HandleMissingRecordsWithTheirDeclaredBehavior()
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);
        Assert.Null(await repository.GetRentalListing(long.MaxValue));
        Assert.Equal(1, interceptor.ReadersExecuted);
        Assert.Null(await repository.GetRentalListingForTakedownAction(long.MaxValue, true));
        Assert.Equal(2, interceptor.ReadersExecuted);
        Assert.Empty(await repository.GetAddressChangeHistoryAsync(long.MaxValue));
        Assert.Equal(3, interceptor.ReadersExecuted);
        Assert.Null(await repository.GetRetalListingExportAsync(long.MaxValue));
        Assert.Equal(4, interceptor.ReadersExecuted);
        Assert.Null(await repository.GetRetalListingExportByNameAsync("missing' OR 1=1; --"));
        Assert.Equal(5, interceptor.ReadersExecuted);
        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetRentalListingToExport(long.MaxValue));
        Assert.Equal(6, interceptor.ReadersExecuted);
    }

    [Fact]
    public async Task GetLatestRentalListingExportTime_ExecutesSynchronousDatabaseReads()
    {
        await using var context = CreateDbContext();
        var repository = CreateRepository(context);
        var times = await DbContext.DssRentalListingExtracts.AsNoTracking().Select(x => x.UpdDtm).ToListAsync();
        var before = DateTime.UtcNow.AddDays(-1);
        var result = repository.GetLatestRentalListingExportTime();
        if (times.Count > 0) Assert.Equal(times.Max(), result);
        else Assert.InRange(result, before, DateTime.UtcNow.AddDays(-1));
    }

    [Fact]
    public async Task ListingDataToProcessExists_ExecutesOneDatabaseRead()
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();
        await using var context = CreateDbContext(interceptor);
        await CreateRepository(context).ListingDataToProcessExists();
        Assert.Equal(1, interceptor.ReadersExecuted);
        Assert.Empty(context.ChangeTracker.Entries());
    }
}
