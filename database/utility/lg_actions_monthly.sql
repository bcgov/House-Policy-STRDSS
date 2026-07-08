-- LG actions monthly report (Sprint 22+)
-- Source: dss_rental_listing_action (NonComplianceNotice, TakedownRequest)
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
)
SELECT
    TO_CHAR(rr.range_start, 'YYYY-MM') AS "Reporting Month",
    COALESCE(org.organization_nm, 'Unknown') AS "Name of Local government",
    SUM(CASE WHEN drla.listing_action_type = 'NonComplianceNotice' THEN 1 ELSE 0 END) AS "Notice of Non-compliance sent",
    SUM(CASE WHEN drla.listing_action_type = 'TakedownRequest' THEN 1 ELSE 0 END) AS "Takedown Request sent"
FROM report_range rr
INNER JOIN dss_rental_listing_action drla
    ON drla.action_dtm >= rr.range_start
   AND drla.action_dtm < rr.range_end
INNER JOIN dss_rental_listing drl
    ON drl.rental_listing_id = drla.rental_listing_id
LEFT JOIN dss_physical_address dpa
    ON dpa.physical_address_id = drl.locating_physical_address_id
LEFT JOIN dss_organization lgs
    ON lgs.organization_id = dpa.containing_organization_id
   AND dpa.match_score_amt > 1
LEFT JOIN dss_organization org
    ON org.organization_id = lgs.managing_organization_id
WHERE drla.listing_action_type IN ('NonComplianceNotice', 'TakedownRequest')
GROUP BY rr.range_start, org.organization_nm
HAVING SUM(CASE WHEN drla.listing_action_type IN ('NonComplianceNotice', 'TakedownRequest') THEN 1 ELSE 0 END) > 0
ORDER BY 2;
