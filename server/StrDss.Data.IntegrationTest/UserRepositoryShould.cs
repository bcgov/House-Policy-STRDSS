using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model.UserDtos;
using Xunit;

namespace StrDss.Data.IntegrationTest;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Integration")]
public class UserRepositoryShould : DatabaseTestBase
{
    public UserRepositoryShould(PostgresFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData("Guid", true, true)]
    [InlineData("DisplayName", true, true)]
    [InlineData("CurrentUserGuid", true, true)]
    [InlineData("CurrentUserExternalIdentity", true, true)]
    [InlineData("Guid", false, true)]
    [InlineData("DisplayName", false, true)]
    [InlineData("CurrentUserGuid", false, true)]
    [InlineData("CurrentUserExternalIdentity", false, true)]
    [InlineData("Guid", true, false)]
    [InlineData("DisplayName", true, false)]
    [InlineData("CurrentUserGuid", true, false)]
    [InlineData("CurrentUserExternalIdentity", true, false)]
    public async Task GetUserAndPermissions_ProjectOnlyRequiredFields(string lookup, bool hasOrganization, bool hasRoles)
    {
        var userId = await DbContext.DssUserIdentities.AsNoTracking()
            .Where(x => x.RepresentedByOrganizationId != null
                && x.DssUserRoleAssignments.Any(role => role.UserRoleCdNavigation.DssUserRolePrivileges.Any()))
            .OrderBy(x => x.UserIdentityId)
            .Select(x => x.UserIdentityId)
            .FirstAsync();

        var externalIdentity = $"projection_test_{Guid.NewGuid():N}";
        await DbContext.DssUserIdentities.Where(x => x.UserIdentityId == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ExternalIdentityCd, externalIdentity));

        if (!hasOrganization)
        {
            await DbContext.DssUserIdentities.Where(x => x.UserIdentityId == userId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.RepresentedByOrganizationId, (long?)null));
        }

        if (!hasRoles)
        {
            await DbContext.DssUserRoleAssignments.Where(x => x.UserIdentityId == userId).ExecuteDeleteAsync();
        }

        var entity = await DbContext.DssUserIdentities.AsNoTracking()
            .Include(x => x.RepresentedByOrganization)
            .FirstAsync(x => x.UserIdentityId == userId);
        var expectedUser = Mapper.Map<UserDto>(entity);
        var expectedRoleCds = await DbContext.DssUserRoleAssignments.AsNoTracking()
            .Where(x => x.UserIdentityId == userId)
            .Select(x => x.UserRoleCd)
            .ToListAsync();
        var expectedPermissions = await DbContext.DssUserRoleAssignments.AsNoTracking()
            .Where(x => x.UserIdentityId == userId)
            .SelectMany(x => x.UserRoleCdNavigation.DssUserRolePrivileges)
            .Select(x => x.UserPrivilegeCd)
            .Distinct()
            .ToListAsync();

        CurrentUser.IsBcServicesCard = lookup == "CurrentUserExternalIdentity";
        CurrentUser.UserGuid = CurrentUser.IsBcServicesCard ? Guid.NewGuid() : entity.UserGuid;
        CurrentUser.ExternalIdentityCd = externalIdentity;

        var interceptor = new QueryCaptureInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = new UserRepository(context, Mapper, CurrentUser, NullLogger<StrDssLogger>.Instance);

        var (user, permissions) = await LookupAsync(repository, lookup, entity.UserGuid, entity.DisplayNm);

        Assert.NotNull(user);
        AssertScalarPropertiesEqual(expectedUser, user);
        Assert.Equal(expectedRoleCds.OrderBy(x => x), user.RoleCds.OrderBy(x => x));
        Assert.Equal(expectedPermissions.OrderBy(x => x), permissions.OrderBy(x => x));
        Assert.Equal(permissions.Count, permissions.Distinct().Count());
        if (hasRoles)
        {
            Assert.NotEmpty(user.RoleCds);
            Assert.NotEmpty(permissions);
        }
        else
        {
            Assert.Empty(user.RoleCds);
            Assert.Empty(permissions);
        }

        if (hasOrganization)
        {
            Assert.NotNull(user.RepresentedByOrganization);
            AssertScalarPropertiesEqual(expectedUser.RepresentedByOrganization!, user.RepresentedByOrganization);
            Assert.Null(user.RepresentedByOrganization.AreaGeometry);
            Assert.Empty(user.RepresentedByOrganization.ContactPeople);
        }
        else
        {
            Assert.Null(user.RepresentedByOrganization);
        }

        Assert.Equal(2, interceptor.Commands.Count);
        var userSql = interceptor.Commands[0].ToLowerInvariant();
        Assert.DoesNotContain("area_geometry", userSql);
        Assert.DoesNotContain("source_attributes_json", userSql);
        Assert.Contains("dss_user_role_assignment", userSql);
        Assert.Contains("user_role_cd", userSql);
        Assert.DoesNotContain("dss_user_role_privilege", userSql);
        Assert.DoesNotContain("dss_organization_contact_person", userSql);
        Assert.Contains("dss_user_role_assignment", interceptor.Commands[1].ToLowerInvariant());
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData("Guid")]
    [InlineData("DisplayName")]
    [InlineData("CurrentUserGuid")]
    [InlineData("CurrentUserExternalIdentity")]
    public async Task GetUserAndPermissions_ReturnNullWithoutQueryingPermissionsWhenUserMissing(string lookup)
    {
        var guid = Guid.NewGuid();
        CurrentUser.UserGuid = guid;
        CurrentUser.IsBcServicesCard = lookup == "CurrentUserExternalIdentity";
        CurrentUser.ExternalIdentityCd = $"missing_{guid:N}";

        var interceptor = new QueryCaptureInterceptor();
        await using var context = CreateDbContext(interceptor);
        var repository = new UserRepository(context, Mapper, CurrentUser, NullLogger<StrDssLogger>.Instance);

        var (user, permissions) = await LookupAsync(repository, lookup, guid, $"missing_{guid:N}");

        Assert.Null(user);
        Assert.Empty(permissions);
        Assert.Single(interceptor.Commands);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    private static Task<(UserDto? user, List<string> permissions)> LookupAsync(
        UserRepository repository, string lookup, Guid guid, string displayName)
    {
        return lookup switch
        {
            "Guid" => repository.GetUserAndPermissionsByGuidAsync(guid),
            "DisplayName" => repository.GetUserAndPermissionsByDisplayNameAsync(displayName),
            "CurrentUserGuid" or "CurrentUserExternalIdentity" => repository.GetUserAndPermissionsByCurrentUserAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(lookup))
        };
    }

    private static void AssertScalarPropertiesEqual<T>(T expected, T actual)
    {
        foreach (var property in typeof(T).GetProperties()
            .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string)))
        {
            Assert.Equal(property.GetValue(expected), property.GetValue(actual));
        }
    }

    private sealed class QueryCaptureInterceptor : DbCommandInterceptor
    {
        public List<string> Commands { get; } = new();

        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            Commands.Add(command.CommandText);
            return ValueTask.FromResult(result);
        }
    }
}
