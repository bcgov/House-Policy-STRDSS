using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model.OrganizationDtos;
using Xunit;

namespace StrDss.Data.IntegrationTest;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Integration")]
public class OrganizationRepositoryShould : DatabaseTestBase
{
    public OrganizationRepositoryShould(PostgresFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(null)]
    [InlineData("All")]
    [InlineData(OrganizationTypes.Platform)]
    [InlineData(OrganizationTypes.LGSub)]
    [InlineData("missing_type")]
    public async Task GetOrganizations_ProjectRequiredFieldsAndContacts(string? type)
    {
        var query = DbContext.DssOrganizations.AsNoTracking();
        query = type != null && type != "All"
            ? query.Where(x => x.OrganizationType == type)
            : query.Where(x => x.OrganizationType != OrganizationTypes.LGSub);
        var entities = await query.Include(x => x.DssOrganizationContactPeople)
            .OrderBy(x => x.OrganizationNm).ToListAsync();
        var expected = Mapper.Map<List<OrganizationDto>>(entities);

        var interceptor = new QueryCaptureInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);

        var actual = await repository.GetOrganizationsAsync(type);

        Assert.Equal(expected.Select(x => x.OrganizationId), actual.Select(x => x.OrganizationId));
        for (var i = 0; i < expected.Count; i++)
            AssertOrganizationEqual(expected[i], actual[i]);
        AssertProjectedQueries(interceptor);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData("Id", true)]
    [InlineData("Id", false)]
    [InlineData("Code", true)]
    [InlineData("Code", false)]
    public async Task OrganizationLookups_PreserveFieldsAndContactLoading(string lookup, bool hasContacts)
    {
        var query = DbContext.DssOrganizations.AsNoTracking()
            .Where(x => x.DssOrganizationContactPeople.Any() == hasContacts);
        var entity = lookup == "Id"
            ? await query.Include(x => x.DssOrganizationContactPeople).OrderBy(x => x.OrganizationId).FirstAsync()
            : await query.OrderBy(x => x.OrganizationId).FirstAsync();
        var expected = Mapper.Map<OrganizationDto>(entity);
        if (lookup == "Id" && hasContacts)
            Assert.NotEmpty(expected.ContactPeople);

        var interceptor = new QueryCaptureInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);

        var actual = lookup == "Id"
            ? await repository.GetOrganizationByIdAsync(entity.OrganizationId)
            : await repository.GetOrganizationByOrgCdAsync(entity.OrganizationCd.ToLowerInvariant());

        Assert.NotNull(actual);
        AssertOrganizationEqual(expected, actual);
        AssertProjectedQueries(interceptor);
        if (lookup == "Code")
            Assert.DoesNotContain("dss_organization_contact_person", Assert.Single(interceptor.Commands).ToLowerInvariant());
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task OrganizationLookups_ReturnNullWhenMissing()
    {
        var interceptor = new QueryCaptureInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);

        Assert.Null(await repository.GetOrganizationByIdAsync(long.MaxValue));
        Assert.Null(await repository.GetOrganizationByOrgCdAsync($"missing_{Guid.NewGuid():N}"));

        Assert.Equal(2, interceptor.Commands.Count);
        AssertProjectedQueries(interceptor);
    }

    [Theory]
    [InlineData(OrganizationTypes.Platform, true)]
    [InlineData(OrganizationTypes.Platform, false)]
    [InlineData(OrganizationTypes.LGSub, true)]
    public async Task ManagingOrganizationLookups_ProjectOnlyScalarFields(string type, bool hasManagingOrganization)
    {
        var query = DbContext.DssOrganizations.AsNoTracking()
            .Where(x => x.OrganizationType == type && x.ManagingOrganizationId.HasValue == hasManagingOrganization);
        if (type == OrganizationTypes.Platform && !hasManagingOrganization)
            query = query.Where(x => x.InverseManagingOrganization.Any());
        var entity = await query.OrderBy(x => x.OrganizationId).FirstAsync();
        var expectedCodes = type == OrganizationTypes.Platform
            ? await DbContext.DssOrganizations.AsNoTracking()
                .Where(x => x.OrganizationId == entity.OrganizationId || x.ManagingOrganizationId == entity.OrganizationId)
                .Select(x => x.OrganizationCd).ToListAsync()
            : new List<string> { entity.OrganizationCd };
        if (type == OrganizationTypes.Platform && !hasManagingOrganization)
            Assert.True(expectedCodes.Count > 1);

        var interceptor = new QueryCaptureInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);

        Assert.Equal(expectedCodes.OrderBy(x => x),
            (await repository.GetManagingOrgCdsAsync(entity.OrganizationId)).OrderBy(x => x));
        Assert.Equal(entity.ManagingOrganizationId, await repository.GetManagingOrgId(entity.OrganizationId));
        Assert.Null(await repository.GetManagingOrgId(long.MaxValue));
        Assert.Empty(await repository.GetManagingOrgCdsAsync(long.MaxValue));

        AssertProjectedQueries(interceptor);
        Assert.All(interceptor.Commands, sql => Assert.DoesNotContain("organization_nm", sql.ToLowerInvariant()));
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task JurisdictionReads_ProjectRequiredFields()
    {
        var entity = await DbContext.DssOrganizations.AsNoTracking()
            .Where(x => x.OrganizationType == OrganizationTypes.LGSub && x.AreaGeometry != null)
            .OrderBy(x => x.OrganizationId).FirstAsync();
        var expected = Mapper.Map<JurisdictionsViewDto>(entity);
        Assert.NotNull(Mapper.Map<OrganizationDto>(entity).AreaGeometry);
        var nonJurisdictionId = await DbContext.DssOrganizations
            .Where(x => x.OrganizationType != OrganizationTypes.LGSub)
            .Select(x => x.OrganizationId).FirstAsync();

        var interceptor = new QueryCaptureInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = CreateRepository(context);

        var actual = await repository.GetJurisdiction(entity.OrganizationId);
        Assert.NotNull(actual);
        AssertScalarPropertiesEqual(expected, actual);
        Assert.Null(await repository.GetJurisdiction(nonJurisdictionId));
        Assert.Null(await repository.GetJurisdiction(long.MaxValue));

        var localGovs = await repository.GetLocalGovs(0, 1, "OrganizationNm", "asc");
        Assert.NotEmpty(localGovs.SourceList);
        foreach (var localGov in localGovs.SourceList)
        {
            var expectedChildren = Mapper.Map<List<JurisdictionsViewDto>>(await DbContext.DssOrganizations.AsNoTracking()
                .Where(x => x.ManagingOrganizationId == localGov.OrganizationId)
                .OrderBy(x => x.OrganizationId).ToListAsync());
            var actualChildren = localGov.Jurisdictions.OrderBy(x => x.OrganizationId).ToList();
            Assert.Equal(expectedChildren.Count, actualChildren.Count);
            for (var i = 0; i < expectedChildren.Count; i++)
                AssertScalarPropertiesEqual(expectedChildren[i], actualChildren[i]);
        }
        Assert.Contains(localGovs.SourceList, x => x.Jurisdictions.Any());
        AssertProjectedQueries(interceptor);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    private OrganizationRepository CreateRepository(DssDbContext context) =>
        new(context, Mapper, CurrentUser, NullLogger<StrDssLogger>.Instance,
            new ConfigurationBuilder().Build());

    private static void AssertOrganizationEqual(OrganizationDto expected, OrganizationDto actual)
    {
        AssertScalarPropertiesEqual(expected, actual);
        Assert.Null(actual.AreaGeometry);
        var expectedContacts = expected.ContactPeople.OrderBy(x => x.OrganizationContactPersonId).ToList();
        var actualContacts = actual.ContactPeople.OrderBy(x => x.OrganizationContactPersonId).ToList();
        Assert.Equal(expectedContacts.Count, actualContacts.Count);
        for (var i = 0; i < expectedContacts.Count; i++)
            AssertScalarPropertiesEqual(expectedContacts[i], actualContacts[i]);
    }

    private static void AssertScalarPropertiesEqual<T>(T expected, T actual)
    {
        foreach (var property in typeof(T).GetProperties()
            .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string)))
            Assert.Equal(property.GetValue(expected), property.GetValue(actual));
    }

    private static void AssertProjectedQueries(QueryCaptureInterceptor interceptor)
    {
        Assert.NotEmpty(interceptor.Commands);
        Assert.All(interceptor.Commands, sql =>
        {
            Assert.DoesNotContain("area_geometry", sql.ToLowerInvariant());
            Assert.DoesNotContain("source_attributes_json", sql.ToLowerInvariant());
        });
    }

    private sealed class QueryCaptureInterceptor : DbCommandInterceptor
    {
        public List<string> Commands { get; } = new();

        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            Commands.Add(command.CommandText);
            return ValueTask.FromResult(result);
        }
    }
}
