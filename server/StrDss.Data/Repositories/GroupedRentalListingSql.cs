namespace StrDss.Data.Repositories
{
    internal static class GroupedRentalListingSql
    {
        internal const string ExcludingIncludedReport = "drl.including_rental_listing_report_id IS NULL";
        internal const string CurrentLocalGovernment = "lg.organization_id = @current_lg_id";
        internal const string RecentWithFallback = @"((drl.offering_organization_id IN (SELECT providing_organization_id FROM dss_rental_listing_report WHERE report_period_ym = @target_report_month) AND sm.latest_report_period_ym = @target_report_month)
OR (drl.offering_organization_id NOT IN (SELECT providing_organization_id FROM dss_rental_listing_report WHERE report_period_ym = @target_report_month) AND sm.latest_report_period_ym = @fallback_report_month))";
        internal const string RecentTargetMonth = "sm.latest_report_period_ym = @target_report_month";
        internal const string AllSearch = "(dpa.match_address_txt ILIKE @all_pattern OR drl.platform_listing_url ILIKE @all_pattern OR drl.platform_listing_no ILIKE @all_pattern OR drl.effective_business_licence_no LIKE @sanitized_all || '%' OR drl.bc_registry_no ILIKE @all_pattern OR drl.effective_host_nm ILIKE '%' || @sanitized_all || '%')";
        internal const string Address = "dpa.match_address_txt ILIKE @address_pattern";
        internal const string Url = "drl.platform_listing_url ILIKE @url_pattern";
        internal const string ListingId = "drl.platform_listing_no ILIKE @listing_id";
        internal const string HostName = "drl.effective_host_nm ILIKE @host_pattern";
        internal const string BusinessLicence = "drl.effective_business_licence_no LIKE @business_licence || '%'";
        internal const string RegistrationNumber = "drl.bc_registry_no ILIKE @registration_pattern";
        internal const string PrincipalResidenceRequired = "lgs.is_principal_residence_required IS TRUE";
        internal const string PrincipalResidenceNotRequired = "(lgs.is_principal_residence_required IS NULL OR lgs.is_principal_residence_required IS FALSE)";
        internal const string BusinessLicenceRequired = "lgs.is_business_licence_required IS TRUE";
        internal const string BusinessLicenceNotRequired = "(lgs.is_business_licence_required IS NULL OR lgs.is_business_licence_required IS FALSE)";
        internal const string LocalGovernment = "lg.organization_id = @lg_id";
        internal const string CountQuery = " SELECT count(*) FROM grouped";

        internal static string FilteredNormalized(IEnumerable<string> filters) => $@"
WITH filtered AS (
 SELECT drl.rental_listing_id, sm.bc_registry_no, sm.match_address_txt, sm.match_unit_no,
        sm.effective_host_nm, sm.listing_business_licence_no, drl.nights_booked_qty, sm.latest_report_period_ym,
        dbl.business_licence_no matched_business_licence_no, dbl.business_licence_id, dbl.expiry_dt business_licence_expiry_dt, dbl.licence_status_type, sm.last_action_nm, sm.last_action_dtm,
        dpa.match_score_amt, dpa.is_match_verified, dpa.is_match_corrected, drl.is_changed_address, drl.is_changed_business_licence,
        drl.platform_listing_no, drl.platform_listing_url, offering.organization_nm offering_organization_nm, drl.listing_status_type,
        drl.takedown_reason, drl.is_lg_transferred, drl.is_taken_down,
        sm.business_licence_digits, sm.primary_host_full_nm
 FROM dss_rental_listing drl
 LEFT JOIN dss_rental_listing_summary sm ON sm.rental_listing_id = drl.rental_listing_id
 LEFT JOIN dss_physical_address dpa ON dpa.physical_address_id = drl.locating_physical_address_id
 LEFT JOIN dss_organization lgs ON lgs.organization_id = dpa.containing_organization_id AND dpa.match_score_amt > 1
 LEFT JOIN dss_organization lg ON lg.organization_id = lgs.managing_organization_id
 LEFT JOIN dss_organization offering ON offering.organization_id = drl.offering_organization_id
 LEFT JOIN dss_business_licence dbl ON dbl.business_licence_id = drl.governing_business_licence_id
 WHERE {string.Join("\n   AND ", filters)}
), normalized AS (
 SELECT * FROM filtered
)";

        internal static string Grouped(string filteredNormalized) =>
            filteredNormalized + ", grouped AS (\n" + Aggregation;

        internal static string Children(string filteredNormalized, bool hasRegistration) =>
            filteredNormalized + ChildSelect + (hasRegistration ? RegistrationChildPredicate : NoRegistrationChildPredicate) + "\n " + ChildOrderBy;

        internal static string PageQuery(string orderBy, string direction, int pageSize)
        {
            var column = orderBy switch
            {
                "effectiveHostNm" => "effective_host_nm", "matchAddressTxt" => "match_address_txt",
                "effectiveBusinessLicenceNo" => "effective_business_licence_no", "lastActionNm" => "last_action_nm",
                "lastActionDtm" => "last_action_dtm", _ => "latest_report_period_ym"
            };
            // ASC NULLS FIRST / DESC NULLS LAST. `{column} IS NULL` sorted ascending forced nulls last in both directions.
            var sort = string.Equals(direction, "asc", StringComparison.OrdinalIgnoreCase) ? "ASC NULLS FIRST" : "DESC NULLS LAST";
            return $" SELECT * FROM grouped ORDER BY {column} {sort}, bc_registry_no NULLS LAST, match_address_txt NULLS LAST, anchor_rental_listing_id" +
                (pageSize > 0 ? " LIMIT @page_size OFFSET @page_offset" : "");
        }

        internal static string HostMultipleProperties(bool limitToCurrentLocalGovernment)
        {
            var localGovernment = limitToCurrentLocalGovernment
                ? @"
 LEFT JOIN dss_organization lgs ON lgs.organization_id = dpa.containing_organization_id AND dpa.match_score_amt > 1
 LEFT JOIN dss_organization lg ON lg.organization_id = lgs.managing_organization_id"
                : "";
            var localGovernmentPredicate = limitToCurrentLocalGovernment ? $"\n   AND {CurrentLocalGovernment}" : "";
            return $@"
SELECT effective_host_nm
FROM (
 SELECT DISTINCT drl.effective_host_nm, drl.effective_business_licence_no, dpa.match_address_txt
 FROM dss_rental_listing drl
 LEFT JOIN dss_physical_address dpa ON dpa.physical_address_id = drl.locating_physical_address_id{localGovernment}
 WHERE {ExcludingIncludedReport}
   AND drl.effective_host_nm = ANY(@hosts){localGovernmentPredicate}
) pairs
GROUP BY effective_host_nm
HAVING count(*) > 1";
        }

        private const string Aggregation = @" SELECT count(*)::integer listing_count, bc_registry_no,
        (array_agg(effective_host_nm ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] effective_host_nm,
        (array_agg(primary_host_full_nm ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] primary_host_full_nm,
        (array_agg(match_address_txt ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] match_address_txt,
        (array_agg(match_unit_no ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] match_unit_no,
        sum(coalesce(nights_booked_qty, 0))::integer nights_booked_ytd_qty, max(latest_report_period_ym) latest_report_period_ym,
        (array_agg(rental_listing_id ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] anchor_rental_listing_id,
        (array_agg(listing_business_licence_no ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] effective_business_licence_no,
        (array_agg(coalesce(matched_business_licence_no, listing_business_licence_no) ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] business_licence_no,
        (array_agg(last_action_nm ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] last_action_nm, max(last_action_dtm) last_action_dtm,
        (array_agg(business_licence_id ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] business_licence_id,
        (array_agg(business_licence_expiry_dt ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] business_licence_expiry_dt,
        (array_agg(licence_status_type ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1] licence_status_type
 FROM normalized WHERE bc_registry_no IS NOT NULL GROUP BY bc_registry_no
 UNION ALL
 SELECT count(*)::integer, NULL::text, (array_agg(effective_host_nm ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1],
        (array_agg(primary_host_full_nm ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1], match_address_txt, match_unit_no,
        sum(coalesce(nights_booked_qty, 0))::integer, max(latest_report_period_ym),
        (array_agg(rental_listing_id ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1],
        (array_agg(listing_business_licence_no ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1],
        (array_agg(coalesce(matched_business_licence_no, listing_business_licence_no) ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1],
        (array_agg(last_action_nm ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1], max(last_action_dtm),
        (array_agg(business_licence_id ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1],
        (array_agg(business_licence_expiry_dt ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1],
        (array_agg(licence_status_type ORDER BY last_action_dtm DESC NULLS LAST, latest_report_period_ym DESC NULLS LAST, rental_listing_id))[1]
 FROM normalized WHERE bc_registry_no IS NULL GROUP BY match_address_txt, match_unit_no, effective_host_nm, business_licence_digits
)";

        private const string RegistrationChildPredicate = "bc_registry_no IS NOT DISTINCT FROM NULLIF(btrim(@bc_registry_no), '')";

        private const string NoRegistrationChildPredicate = @"bc_registry_no IS NULL
   AND match_address_txt IS NOT DISTINCT FROM NULLIF(btrim(@match_address_txt), '')
   AND match_unit_no IS NOT DISTINCT FROM NULLIF(btrim(@match_unit_no), '')
   AND effective_host_nm IS NOT DISTINCT FROM NULLIF(btrim(@effective_host_nm), '')
   AND business_licence_digits IS NOT DISTINCT FROM NULLIF(regexp_replace(@effective_business_licence_no, '[^0-9]', '', 'g'), '')";

        private const string ChildOrderBy = "ORDER BY latest_report_period_ym DESC NULLS LAST, rental_listing_id";

        private const string ChildSelect = @"
 SELECT rental_listing_id, latest_report_period_ym, bc_registry_no, match_address_txt, match_score_amt,
        is_match_verified, is_match_corrected, is_changed_address, nights_booked_qty, listing_business_licence_no,
        matched_business_licence_no, is_changed_business_licence, business_licence_id, licence_status_type,
        last_action_nm, last_action_dtm, platform_listing_no, platform_listing_url, offering_organization_nm,
        listing_status_type, takedown_reason, is_lg_transferred, is_taken_down
 FROM normalized
 WHERE ";
    }
}
