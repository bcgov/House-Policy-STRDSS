using Microsoft.Extensions.Logging.Abstractions;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;

namespace StrDss.Data.IntegrationTest;

public abstract class RentalListingQueryTestBase : DatabaseTestBase
{
    protected RentalListingQueryTestBase(PostgresFixture fixture) : base(fixture)
    {
        CurrentUser.OrganizationType = OrganizationTypes.BCGov;
    }

    protected RentalListingRepository CreateRepository(DssDbContext context)
    {
        var logger = NullLogger<StrDssLogger>.Instance;
        return new RentalListingRepository(context, Mapper, CurrentUser, logger,
            new UserRepository(context, Mapper, CurrentUser, logger));
    }

    protected sealed record Filters(string? All = null, string? Address = null, string? Url = null,
        string? ListingId = null, string? HostName = null, string? BusinessLicence = null,
        string? RegistrationNumber = null, bool? PrRequirement = null, bool? BlRequirement = null,
        long? LgId = null, bool Recent = false);

    public static IEnumerable<object[]> FilterCases()
    {
        foreach (var name in new[] { "none", "empty", "whitespace", "all", "address", "url", "listing",
            "host", "licence", "registration", "pr-true", "pr-false", "bl-true", "bl-false", "lg",
            "recent", "combined", "quoted-all", "quoted-address", "quoted-url", "quoted-listing",
            "quoted-host", "quoted-licence", "quoted-registration" })
        {
            yield return new object[] { name };
        }
    }

    protected static Filters GetFilters(string name)
    {
        const string quoted = "missing' OR 1=1; --";
        return name switch
        {
            "none" => new(),
            "empty" => new("", "", "", "", "", "", ""),
            "whitespace" => new(" ", " ", " ", " ", " ", " ", " "),
            "all" => new(All: "example"),
            "address" => new(Address: "main"),
            "url" => new(Url: "example.com"),
            "listing" => new(ListingId: "100000%"),
            "host" => new(HostName: "Teresa Homeowner"),
            "licence" => new(BusinessLicence: "BL-VanLOAD"),
            "registration" => new(RegistrationNumber: "987654"),
            "pr-true" => new(PrRequirement: true),
            "pr-false" => new(PrRequirement: false),
            "bl-true" => new(BlRequirement: true),
            "bl-false" => new(BlRequirement: false),
            "lg" => new(LgId: 212),
            "recent" => new(Recent: true),
            "combined" => new("example", "main", "example.com", "100000%", "Teresa Homeowner", "54", "987", false, false, 212, true),
            "quoted-all" => new(All: quoted),
            "quoted-address" => new(Address: quoted),
            "quoted-url" => new(Url: quoted),
            "quoted-listing" => new(ListingId: quoted),
            "quoted-host" => new(HostName: quoted),
            "quoted-licence" => new(BusinessLicence: quoted),
            "quoted-registration" => new(RegistrationNumber: quoted),
            _ => throw new ArgumentOutOfRangeException(nameof(name))
        };
    }

    protected static Task<int> CountListings(RentalListingRepository repository, Filters f) =>
        repository.GetRentalListingsCountAsync(f.All, f.Address, f.Url, f.ListingId, f.HostName,
            f.BusinessLicence, f.RegistrationNumber, f.PrRequirement, f.BlRequirement, f.LgId, f.Recent);

    protected static Task<PagedDto<RentalListingTableRowDto>> Listings(RentalListingRepository repository,
        Filters f, int pageSize = 10, int pageNumber = 1, string orderBy = "latestReportPeriodYm",
        string direction = "desc", bool includeTotalCount = false) =>
        repository.GetRentalListings(f.All, f.Address, f.Url, f.ListingId, f.HostName,
            f.BusinessLicence, f.RegistrationNumber, f.PrRequirement, f.BlRequirement, f.LgId,
            f.Recent, pageSize, pageNumber, orderBy, direction, includeTotalCount);

    protected static Task<int> CountGroups(RentalListingRepository repository, Filters f) =>
        repository.GetGroupedRentalListingsCountAsync(f.All, f.Address, f.Url, f.ListingId, f.HostName,
            f.BusinessLicence, f.RegistrationNumber, f.PrRequirement, f.BlRequirement, f.LgId, f.Recent);

    protected static Task<PagedDto<RentalListingGroupSummaryDto>> Groups(RentalListingRepository repository,
        Filters f, int pageSize = 10, int pageNumber = 1, string orderBy = "latestReportPeriodYm",
        string direction = "desc", bool includeTotalCount = false) =>
        repository.GetGroupedRentalListingsPagedAsync(f.All, f.Address, f.Url, f.ListingId, f.HostName,
            f.BusinessLicence, f.RegistrationNumber, f.PrRequirement, f.BlRequirement, f.LgId,
            f.Recent, pageSize, pageNumber, orderBy, direction, includeTotalCount);

    protected static Task<List<RentalListingTableRowDto>> Children(RentalListingRepository repository,
        Filters f, string? registration = null, string? address = null, string? unit = null,
        string? host = null, string? licence = null) =>
        repository.GetGroupedListingChildrenAsync(f.All, f.Address, f.Url, f.ListingId, f.HostName,
            f.BusinessLicence, f.RegistrationNumber, f.PrRequirement, f.BlRequirement, f.LgId,
            f.Recent, registration, address, unit, host, licence);
}
