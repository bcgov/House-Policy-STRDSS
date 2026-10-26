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
        public void ListingQueries_KeepEachJoinListAtOrBelowEight()
        {
            const int joinCollapseLimit = 8;
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var targetMonth = new DateOnly(2026, 9, 1);
            var fallbackMonth = new DateOnly(2026, 8, 1);
            var reported = context.DssRentalListingReports.AsNoTracking()
                .Where(r => r.ReportPeriodYm == targetMonth)
                .Select(r => r.ProvidingOrganizationId)
                .Distinct();

            var queries = new Dictionary<string, string>
            {
                ["base"] = repository.GetRentalListingsTableBaseQuery().ToQueryString(),
                ["filtered and sorted"] = repository.GetRentalListingsTableBaseQuery()
                    .Where(x => x.LatestReportPeriodYm == targetMonth)
                    .Where(x => x.ManagingOrganizationId == 1)
                    .Where(x => x.LastActionNm != null)
                    .OrderByDescending(x => x.LatestReportPeriodYm ?? DateOnly.MinValue)
                    .ToQueryString(),
                ["recent filter"] = repository.GetRentalListingsTableBaseQuery()
                    .Where(x =>
                        (reported.Contains(x.OfferingOrganizationId ?? 0) && x.LatestReportPeriodYm == targetMonth) ||
                        (!reported.Contains(x.OfferingOrganizationId ?? 0) && x.LatestReportPeriodYm == fallbackMonth))
                    .ToQueryString()
            };

            foreach (var (name, sql) in queries)
            {
                var joinCounts = JoinCountsByQuery(sql);
                Assert.NotEmpty(joinCounts);
                Assert.True(
                    joinCounts.All(count => count <= joinCollapseLimit),
                    $"{name} has a join list above PostgreSQL's default join_collapse_limit of {joinCollapseLimit}: {string.Join(", ", joinCounts)}");
            }
        }

        /// <summary>
        /// Counts JOIN keywords in each parenthesized query or expression.
        /// Nested subqueries are counted on their own, so two five-join subqueries do not add up to ten.
        /// The last count is the outer query.
        /// </summary>
        private static List<int> JoinCountsByQuery(string sql)
        {
            var counts = new List<int>();
            var stack = new Stack<int>();
            stack.Push(0);

            for (var i = 0; i < sql.Length; i++)
            {
                if (sql[i] == '-' && i + 1 < sql.Length && sql[i + 1] == '-')
                {
                    while (i < sql.Length && sql[i] != '\n')
                    {
                        i++;
                    }

                    continue;
                }

                if (sql[i] == '\'')
                {
                    i++;
                    while (i < sql.Length && sql[i] != '\'')
                    {
                        i++;
                    }

                    continue;
                }

                if (sql[i] == '(')
                {
                    stack.Push(0);
                    continue;
                }

                if (sql[i] == ')')
                {
                    counts.Add(stack.Pop());
                    continue;
                }

                if (!IsJoinKeyword(sql, i))
                {
                    continue;
                }

                var current = stack.Pop();
                stack.Push(current + 1);
                i += "JOIN".Length - 1;
            }

            counts.Add(stack.Pop());
            return counts;
        }

        private static bool IsJoinKeyword(string sql, int index)
        {
            const string join = "JOIN";
            if (index + join.Length > sql.Length)
            {
                return false;
            }

            if (!sql.AsSpan(index, join.Length).Equals(join, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var beforeOk = index == 0 || !char.IsLetterOrDigit(sql[index - 1]);
            var afterIndex = index + join.Length;
            var afterOk = afterIndex >= sql.Length || !char.IsLetterOrDigit(sql[afterIndex]);
            return beforeOk && afterOk;
        }

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
        public void LatestReportPeriod_JoinsMaterializedViewAndStaysInSql()
        {
            using var context = CreateContext();
            var repository = CreateRepository(context);
            var targetMonth = new DateOnly(2026, 9, 1);

            var sql = repository.GetRentalListingsTableBaseQuery()
                .Where(x => x.LatestReportPeriodYm == targetMonth)
                .OrderByDescending(x => x.LatestReportPeriodYm ?? DateOnly.MinValue)
                .ToQueryString();

            Assert.Contains("dss_rental_listing_latest_report_mv", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("latest_report_period_ym", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("ORDER BY", sql, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("GROUP BY", sql, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("dss_rental_listing_report", sql, StringComparison.OrdinalIgnoreCase);
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
