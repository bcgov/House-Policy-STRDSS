using Microsoft.EntityFrameworkCore;
using Moq;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using Xunit;

namespace StrDss.Test
{
    public class RentalListingTableQueryShould
    {
        [Fact]
        public void LatestListingAction_TranslatesToLateralJoinWithActionTieBreak()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var sql = repository.GetRentalListingsTableBaseQuery().ToQueryString();

            Assert.Contains("LEFT JOIN LATERAL", sql, StringComparison.OrdinalIgnoreCase);
            var actionLookup = ActionRegion(sql);
            Assert.Contains("ORDER BY", actionLookup, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("action_dtm DESC", actionLookup, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("rental_listing_action_id DESC", actionLookup, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("LIMIT 1", actionLookup, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("GROUP BY", actionLookup, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("ROW_NUMBER()", actionLookup, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void LastActionSortAndFilter_StayInSql()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);

            var query = repository.GetRentalListingsTableBaseQuery()
                .Where(x => x.LastActionNm != null && x.LastActionDtm != null)
                .OrderByDescending(x => x.LastActionNm ?? "ZZZZ")
                .ThenByDescending(x => x.LastActionDtm ?? DateTime.MinValue);

            var sql = query.ToQueryString();

            Assert.Contains("action_long_nm IS NOT NULL", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("action_dtm IS NOT NULL", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("ORDER BY", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("action_long_nm", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("action_dtm", sql, StringComparison.OrdinalIgnoreCase);
        }

        private static string ActionRegion(string sql)
        {
            var marker = "dss_rental_listing_action";
            var start = sql.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return sql;
            }

            var end = sql.IndexOf("LIMIT 1", start, StringComparison.OrdinalIgnoreCase);
            return end < 0 ? sql[start..] : sql[start..(end + "LIMIT 1".Length)];
        }

        private static DssDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DssDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=strdss;Username=strdss;Password=strdss",
                    npgsql => npgsql.UseNetTopologySuite())
                .Options;

            return new DssDbContext(options);
        }

        private static RentalListingRepository CreateRepository(DssDbContext context)
        {
            return new RentalListingRepository(
                context,
                Mock.Of<AutoMapper.IMapper>(),
                Mock.Of<ICurrentUser>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<StrDssLogger>>(),
                Mock.Of<IUserRepository>());
        }
    }
}
