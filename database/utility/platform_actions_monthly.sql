-- Platform actions monthly report (Sprint 22+)
-- Source: dss_rental_listing_action (NonComplianceNotice, TakedownRequest, PlatformTakedown)
-- Set report_date to the first day of the month AFTER the reporting period
-- (e.g. report_date = 2026-06-01 reports on May 2026 activity).

WITH report_param AS (
    SELECT CAST('2026-06-01' AS date) AS report_date
),
report_range AS (
    SELECT
        report_param.report_date - INTERVAL '1 month' AS range_start,
        report_param.report_date AS range_end
    FROM report_param
),
platforms AS (
    SELECT
        org.organization_id,
        org.organization_nm AS platform_name,
        org.organization_cd AS platform_code
    FROM dss_organization org
    WHERE org.organization_type = 'Platform'
      AND org.is_active IS TRUE
),
action_counts AS (
    SELECT
        drl.offering_organization_id AS organization_id,
        SUM(CASE WHEN drla.listing_action_type = 'NonComplianceNotice' THEN 1 ELSE 0 END) AS notice_count,
        SUM(CASE WHEN drla.listing_action_type = 'TakedownRequest' THEN 1 ELSE 0 END) AS request_count,
        SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' THEN 1 ELSE 0 END) AS total_takedowns,
        COUNT(DISTINCT CASE WHEN drla.listing_action_type = 'PlatformTakedown' THEN drla.rental_listing_id END) AS unique_listings,
        SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' AND drla.takedown_reason = 'LG Request' THEN 1 ELSE 0 END) AS lg_request_takedowns,
        SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' AND drla.takedown_reason = 'Invalid Registration' THEN 1 ELSE 0 END) AS invalid_registration_takedowns,
        SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' AND drla.takedown_reason IS NULL THEN 1 ELSE 0 END) AS other_takedowns
    FROM report_range rr
    INNER JOIN dss_rental_listing_action drla
        ON drla.action_dtm >= rr.range_start
       AND drla.action_dtm < rr.range_end
    INNER JOIN dss_rental_listing drl
        ON drl.rental_listing_id = drla.rental_listing_id
    WHERE drla.listing_action_type IN ('NonComplianceNotice', 'TakedownRequest', 'PlatformTakedown')
    GROUP BY drl.offering_organization_id
)
SELECT
    TO_CHAR(rr.range_start, 'YYYY-MM') AS reporting_month,
    plat.platform_name,
    plat.platform_code,
    COALESCE(ac.notice_count, 0) AS notice_count,
    COALESCE(ac.request_count, 0) AS request_count,
    COALESCE(ac.total_takedowns, 0) AS total_takedowns,
    COALESCE(ac.unique_listings, 0) AS unique_listings,
    COALESCE(ac.lg_request_takedowns, 0) AS lg_request_takedowns,
    COALESCE(ac.invalid_registration_takedowns, 0) AS invalid_registration_takedowns,
    COALESCE(ac.other_takedowns, 0) AS other_takedowns
FROM report_range rr
CROSS JOIN platforms plat
LEFT JOIN action_counts ac
    ON ac.organization_id = plat.organization_id
WHERE COALESCE(ac.notice_count, 0) > 0
   OR COALESCE(ac.request_count, 0) > 0
   OR COALESCE(ac.total_takedowns, 0) > 0
ORDER BY plat.platform_name;
