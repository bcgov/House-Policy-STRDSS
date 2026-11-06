using System.Data.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using Xunit;

namespace StrDss.Test;

public class GroupedRentalListingSqlShould
{
    [Fact]
    public void Define_registration_and_no_registration_keys_before_union()
    {
        using var context = CreateContext();
        var sql = CreateRepository(context).BuildGroupedSql(null, null, null, null, null, null, null, null, null, null, false).Text;

        Assert.Contains("FROM normalized WHERE bc_registry_no IS NOT NULL GROUP BY bc_registry_no", sql);
        Assert.Contains("FROM normalized WHERE bc_registry_no IS NULL GROUP BY match_address_txt, match_unit_no, effective_host_key, business_licence_key", sql);
        Assert.Contains("NULLIF(btrim(effective_host_nm), '') effective_host_key", sql);
        Assert.Contains("regexp_replace(listing_business_licence_no, '[^0-9]', '', 'g')", sql);
        Assert.Contains("NULLIF(regexp_replace", sql);
    }

    [Fact]
    public void Apply_filters_before_parent_aggregation()
    {
        using var context = CreateContext();
        var sql = CreateRepository(context).BuildGroupedSql(null, "Main", null, null, null, null, null, null, null, null, false).Text;

        var filter = sql.IndexOf("dpa.match_address_txt ILIKE @address_pattern", StringComparison.Ordinal);
        var grouped = sql.IndexOf("), grouped AS", StringComparison.Ordinal);
        Assert.True(filter >= 0 && filter < grouped);
    }

    [Theory]
    [InlineData("latestReportPeriodYm", "latest_report_period_ym")]
    [InlineData("effectiveHostNm", "effective_host_nm")]
    [InlineData("matchAddressTxt", "match_address_txt")]
    [InlineData("effectiveBusinessLicenceNo", "effective_business_licence_no")]
    [InlineData("lastActionNm", "last_action_nm")]
    [InlineData("lastActionDtm", "last_action_dtm")]
    public void Sort_whitelisted_parent_columns_nulls_first_when_ascending_and_last_when_descending(string orderBy, string column)
    {
        var asc = GroupedRentalListingSql.PageQuery(orderBy, "asc", 25);
        var desc = GroupedRentalListingSql.PageQuery(orderBy, "desc", 0);
        const string ties = "bc_registry_no NULLS LAST, match_address_txt NULLS LAST, anchor_rental_listing_id";

        Assert.Contains($"ORDER BY {column} ASC NULLS FIRST, {ties}", asc);
        Assert.Contains($"ORDER BY {column} DESC NULLS LAST, {ties}", desc);
        Assert.DoesNotContain($"{column} IS NULL", asc);
        Assert.DoesNotContain($"{column} IS NULL", desc);

        var groupedIndex = asc.IndexOf("FROM grouped", StringComparison.Ordinal);
        var limitIndex = asc.IndexOf("LIMIT @page_size OFFSET @page_offset", StringComparison.Ordinal);
        Assert.True(groupedIndex >= 0 && limitIndex > groupedIndex);
        Assert.DoesNotContain("LIMIT", desc);
    }

    [Fact]
    public void Read_null_action_timestamp_as_null_not_datetime_min()
    {
        var reader = new Mock<DbDataReader>();
        reader.Setup(r => r.GetOrdinal("last_action_dtm")).Returns(4);
        reader.Setup(r => r.IsDBNull(4)).Returns(true);

        Assert.Null(RentalListingRepository.ReadNullableDateTime(reader.Object, "last_action_dtm"));

        var actionAt = new DateTime(2024, 6, 15, 18, 30, 0, DateTimeKind.Utc);
        reader.Setup(r => r.IsDBNull(4)).Returns(false);
        reader.Setup(r => r.GetFieldValue<DateTime>(4)).Returns(actionAt);

        Assert.Equal(actionAt, RentalListingRepository.ReadNullableDateTime(reader.Object, "last_action_dtm"));
    }

    [Fact]
    public void Read_null_licence_id_and_expiry_as_null()
    {
        var reader = new Mock<DbDataReader>();
        reader.Setup(r => r.GetOrdinal("business_licence_id")).Returns(2);
        reader.Setup(r => r.GetOrdinal("business_licence_expiry_dt")).Returns(3);
        reader.Setup(r => r.IsDBNull(2)).Returns(true);
        reader.Setup(r => r.IsDBNull(3)).Returns(true);

        Assert.Null(RentalListingRepository.ReadNullableInt64(reader.Object, "business_licence_id"));
        Assert.Null(RentalListingRepository.ReadNullableDateOnly(reader.Object, "business_licence_expiry_dt"));

        var expiry = new DateOnly(2026, 3, 1);
        reader.Setup(r => r.IsDBNull(2)).Returns(false);
        reader.Setup(r => r.IsDBNull(3)).Returns(false);
        reader.Setup(r => r.GetFieldValue<long>(2)).Returns(44L);
        reader.Setup(r => r.GetFieldValue<DateOnly>(3)).Returns(expiry);

        Assert.Equal(44L, RentalListingRepository.ReadNullableInt64(reader.Object, "business_licence_id"));
        Assert.Equal(expiry, RentalListingRepository.ReadNullableDateOnly(reader.Object, "business_licence_expiry_dt"));
    }

    [Fact]
    public void Share_the_same_cte_for_page_and_group_count()
    {
        using var context = CreateContext();
        var grouped = CreateRepository(context).BuildGroupedSql(null, null, null, null, null, null, null, null, null, null, false).Text;

        Assert.EndsWith(")", grouped.Trim());
        Assert.Contains("), grouped AS (", grouped);
        Assert.DoesNotContain("@page_size", grouped, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("OFFSET", grouped, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reuse_filtered_and_normalized_ctes_for_child_rows()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);
        var parent = repo.BuildGroupedSql("oak", "Main", "airbnb", "99", "Smith", "BL1", "REG", true, false, 42, false).Text;
        var registration = repo.BuildGroupedChildrenSql("oak", "Main", "airbnb", "99", "Smith", "BL1", "REG", true, false, 42, false, "REG9", null, null, null, null).Text;
        var noRegistration = repo.BuildGroupedChildrenSql("oak", "Main", "airbnb", "99", "Smith", "BL1", "REG", true, false, 42, false, "  ", "", null, null, "ABC").Text;
        var shared = parent[..parent.IndexOf(", grouped AS (", StringComparison.Ordinal)];

        Assert.StartsWith(shared, registration);
        Assert.StartsWith(shared, noRegistration);
        Assert.Contains("NULLIF(btrim(effective_host_nm), '') effective_host_key", shared);
        Assert.Contains("NULLIF(regexp_replace(listing_business_licence_no, '[^0-9]', '', 'g'), '') business_licence_key", shared);
        Assert.Contains("dpa.match_score_amt", shared);
        Assert.Contains("dpa.is_match_verified", shared);
        Assert.Contains("dpa.is_match_corrected", shared);
        Assert.Contains("drl.is_changed_address", shared);
        Assert.Contains("drl.is_changed_business_licence", shared);
        Assert.Contains("drl.platform_listing_no", shared);
        Assert.Contains("drl.platform_listing_url", shared);
        Assert.Contains("offering.organization_nm offering_organization_nm", shared);
        Assert.Contains("drl.listing_status_type", shared);
        Assert.Contains("drl.takedown_reason", shared);
        Assert.Contains("drl.is_lg_transferred", shared);
        Assert.Contains("drl.is_taken_down", shared);
        Assert.Contains("dpa.match_address_txt ILIKE @address_pattern", shared);
        Assert.DoesNotContain("upper(effective_host", registration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("upper(effective_host", noRegistration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("grouped AS", registration);
        Assert.DoesNotContain("array_agg", registration);
        Assert.DoesNotContain("grouped AS", noRegistration);
        Assert.DoesNotContain("array_agg", noRegistration);
    }

    [Fact]
    public void Match_registration_children_by_trimmed_registry_number()
    {
        using var context = CreateContext();
        var sql = CreateRepository(context).BuildGroupedChildrenSql(null, null, null, null, null, null, null, null, null, null, false, " 12345 ", null, null, null, null).Text;

        Assert.Contains("bc_registry_no IS NOT DISTINCT FROM NULLIF(btrim(@bc_registry_no), '')", sql);
        Assert.DoesNotContain("bc_registry_no IS NULL", sql);
        Assert.DoesNotContain("@match_address_txt", sql);
        Assert.DoesNotContain("@effective_business_licence_no", sql);
    }

    [Fact]
    public void Match_no_registration_children_by_null_safe_address_unit_host_and_digit_licence()
    {
        using var context = CreateContext();
        var sql = CreateRepository(context).BuildGroupedChildrenSql(null, null, null, null, null, null, null, null, null, null, false, null, "", " ", null, "").Text;

        Assert.Contains("bc_registry_no IS NULL", sql);
        Assert.Contains("match_address_txt IS NOT DISTINCT FROM NULLIF(btrim(@match_address_txt), '')", sql);
        Assert.Contains("match_unit_no IS NOT DISTINCT FROM NULLIF(btrim(@match_unit_no), '')", sql);
        Assert.Contains("effective_host_key IS NOT DISTINCT FROM NULLIF(btrim(@effective_host_nm), '')", sql);
        Assert.Contains("business_licence_key IS NOT DISTINCT FROM NULLIF(regexp_replace(@effective_business_licence_no, '[^0-9]', '', 'g'), '')", sql);
        Assert.DoesNotContain("upper(effective_host", sql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@bc_registry_no", sql);
    }

    [Fact]
    public void Apply_search_filters_before_the_child_predicate()
    {
        using var context = CreateContext();
        var sql = CreateRepository(context).BuildGroupedChildrenSql(null, "Main", null, null, null, null, null, null, null, null, false, null, "", "", "", "").Text;
        var filter = sql.IndexOf("dpa.match_address_txt ILIKE @address_pattern", StringComparison.Ordinal);
        var normalized = sql.IndexOf("), normalized AS (", StringComparison.Ordinal);
        var predicate = sql.IndexOf("WHERE bc_registry_no IS NULL", StringComparison.Ordinal);

        Assert.True(filter >= 0 && normalized > filter && predicate > normalized);
    }

    [Fact]
    public void Count_distinct_stored_licence_and_address_pairs_for_the_given_hosts()
    {
        var sql = GroupedRentalListingSql.HostMultipleProperties(false);
        var distinct = sql.IndexOf("SELECT DISTINCT drl.effective_host_nm, drl.effective_business_licence_no, dpa.match_address_txt", StringComparison.Ordinal);
        var having = sql.IndexOf("HAVING count(*) > 1", StringComparison.Ordinal);

        Assert.Contains("drl.effective_host_nm = ANY(@hosts)", sql);
        Assert.Contains("drl.including_rental_listing_report_id IS NULL", sql);
        Assert.True(distinct >= 0 && having > distinct);
        Assert.Contains("GROUP BY effective_host_nm", sql);
        Assert.DoesNotContain("count(DISTINCT", sql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ILIKE", sql);
        Assert.DoesNotContain("latest_report_period_ym", sql);
        Assert.DoesNotContain("dss_rental_listing_report", sql);
        Assert.DoesNotContain("dss_rental_listing_action", sql);
        Assert.DoesNotContain("@lg_id", sql);
        Assert.DoesNotContain("@current_lg_id", sql);
        Assert.DoesNotContain("btrim", sql);
        Assert.DoesNotContain("unit_no", sql);
    }

    [Fact]
    public void Limit_that_host_count_to_the_managing_local_government()
    {
        var sql = GroupedRentalListingSql.HostMultipleProperties(true);

        Assert.Contains("drl.effective_host_nm = ANY(@hosts)", sql);
        Assert.Contains("LEFT JOIN dss_organization lgs ON lgs.organization_id = dpa.containing_organization_id AND dpa.match_score_amt > 1", sql);
        Assert.Contains("LEFT JOIN dss_organization lg ON lg.organization_id = lgs.managing_organization_id", sql);
        Assert.Contains("lg.organization_id = @current_lg_id", sql);
        Assert.DoesNotContain("@lg_id", sql);
        Assert.DoesNotContain("ILIKE", sql);
        Assert.DoesNotContain("latest_report_period_ym", sql);
    }

    [Fact]
    public void Order_child_rows_by_report_month_then_listing_id()
    {
        using var context = CreateContext();
        var repo = CreateRepository(context);
        var registration = repo.BuildGroupedChildrenSql(null, null, null, null, null, null, null, null, null, null, false, "REG", null, null, null, null).Text;
        var noRegistration = repo.BuildGroupedChildrenSql(null, null, null, null, null, null, null, null, null, null, false, null, null, null, null, null).Text;
        const string order = "ORDER BY latest_report_period_ym DESC NULLS LAST, rental_listing_id";

        Assert.EndsWith(order, registration.Trim());
        Assert.EndsWith(order, noRegistration.Trim());
        Assert.DoesNotContain("@page_size", registration);
        Assert.DoesNotContain("OFFSET", registration);
        Assert.DoesNotContain("@page_size", noRegistration);
        Assert.DoesNotContain("OFFSET", noRegistration);
    }

    private static DssDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<DssDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=strdss;Username=strdss;Password=strdss",
                npgsql => npgsql.UseNetTopologySuite())
            .Options);

    private static RentalListingRepository CreateRepository(DssDbContext context) =>
        new(context, Mock.Of<IMapper>(), Mock.Of<ICurrentUser>(), Mock.Of<ILogger<StrDssLogger>>(), Mock.Of<IUserRepository>());
}
