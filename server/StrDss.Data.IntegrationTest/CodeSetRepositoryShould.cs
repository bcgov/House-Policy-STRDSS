using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Data.IntegrationTest.Interceptors;
using StrDss.Data.Repositories;
using Xunit;

namespace StrDss.Data.IntegrationTest;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Integration")]
public class CodeSetRepositoryShould : DatabaseTestBase
{
    public CodeSetRepositoryShould(PostgresFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task LoadCodeSetAsync_ExecuteAllFourDatabaseReadsWithoutTracking()
    {
        var interceptor = new ExecuteReaderAsyncCountingInterceptor();

        await using var sourceContext = CreateDbContext();
        var options = new DbContextOptionsBuilder<DssDbContext>(
            (DbContextOptions<DssDbContext>)sourceContext.GetService<IDbContextOptions>())
            .AddInterceptors(interceptor)
            .Options;
        
        await using var context = new DssDbContext(options, CurrentUser, NullLogger<StrDssLogger>.Instance);
        var repository = new CodeSetRepository(context, Mapper, CurrentUser, NullLogger<StrDssLogger>.Instance);

        var result = await repository.LoadCodeSetAsync();

        Assert.NotNull(result);
        Assert.Equal(4, interceptor.ReadersExecuted);
        Assert.Empty(context.ChangeTracker.Entries());
    }

}
