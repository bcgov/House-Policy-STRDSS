-- DSS-1349: Platform Actions Report For Specific Month
-- Set report_date to the first day of the month AFTER the month to report.
-- Example: '2026-01-01' reports December 2025.
WITH report_param AS (
    SELECT CAST('2026-01-01' AS date) AS report_date
),
report_range AS (
    SELECT
        report_param.report_date - INTERVAL '1 month' AS range_start,
        report_param.report_date AS range_end
    FROM report_param
)
SELECT
    TO_CHAR(rr.range_start, 'YYYY-MM') AS reporting_month,
    plat.platform_name,
    plat.platform_code,
    COALESCE(nc.notice_count, 0) AS notice_count,
    COALESCE(rc.request_count, 0) AS request_count,
    COALESCE(tc.total_takedowns, 0) AS total_takedowns,
    COALESCE(tc.unique_listings, 0) AS unique_listings,
    COALESCE(tc.lg_request_takedowns, 0) AS lg_request_takedowns,
    COALESCE(tc.invalid_registration_takedowns, 0) AS invalid_registration_takedowns,
    COALESCE(tc.other_takedowns, 0) AS other_takedowns
FROM report_range rr
CROSS JOIN (
    SELECT DISTINCT CAST(DATE_TRUNC('month', dem.message_delivery_dtm) AS date) AS report_month
    FROM dss_email_message dem
    CROSS JOIN report_range rng
    WHERE (
            dem.email_message_type = 'Notice of Takedown'
         OR dem.email_message_type = 'Takedown Request'
        )
      AND dem.involved_in_organization_id IS NOT NULL
      AND dem.message_delivery_dtm >= rng.range_start
      AND dem.message_delivery_dtm < rng.range_end

    UNION

    SELECT DISTINCT dud.report_period_ym AS report_month
    FROM dss_upload_delivery dud
    CROSS JOIN report_range rng
    WHERE dud.upload_delivery_type = 'Takedown Data'
      AND dud.report_period_ym IS NOT NULL
      AND dud.report_period_ym >= rng.range_start
      AND dud.report_period_ym < rng.range_end
) m
CROSS JOIN (
    SELECT
        org.organization_id,
        org.organization_nm AS platform_name,
        org.organization_cd AS platform_code
    FROM dss_organization org
    WHERE org.organization_type = 'Platform'
      AND org.is_active IS true
) plat
LEFT JOIN (
    SELECT
        CAST(DATE_TRUNC('month', dem.message_delivery_dtm) AS date) AS report_month,
        dem.involved_in_organization_id AS organization_id,
        COUNT(*) AS notice_count
    FROM dss_email_message dem
    CROSS JOIN report_range rng
    WHERE dem.email_message_type = 'Notice of Takedown'
      AND dem.involved_in_organization_id IS NOT NULL
      AND dem.message_delivery_dtm >= rng.range_start
      AND dem.message_delivery_dtm < rng.range_end
    GROUP BY
        CAST(DATE_TRUNC('month', dem.message_delivery_dtm) AS date),
        dem.involved_in_organization_id
) nc
    ON nc.report_month = m.report_month
   AND nc.organization_id = plat.organization_id
LEFT JOIN (
    SELECT
        CAST(DATE_TRUNC('month', dem.message_delivery_dtm) AS date) AS report_month,
        dem.involved_in_organization_id AS organization_id,
        COUNT(*) AS request_count
    FROM dss_email_message dem
    CROSS JOIN report_range rng
    WHERE dem.email_message_type = 'Takedown Request'
      AND dem.involved_in_organization_id IS NOT NULL
      AND dem.message_delivery_dtm >= rng.range_start
      AND dem.message_delivery_dtm < rng.range_end
    GROUP BY
        CAST(DATE_TRUNC('month', dem.message_delivery_dtm) AS date),
        dem.involved_in_organization_id
) rc
    ON rc.report_month = m.report_month
   AND rc.organization_id = plat.organization_id
LEFT JOIN (
    SELECT
        dud.report_period_ym AS report_month,
        dud.providing_organization_id AS organization_id,
        COUNT(*) AS total_takedowns,
        COUNT(DISTINCT (dul.source_organization_cd, dul.source_record_no)) AS unique_listings,
        SUM(CASE WHEN LOWER(REPLACE(reason.effective_reason, ' ', '')) = 'lgrequest' THEN 1 ELSE 0 END) AS lg_request_takedowns,
        SUM(CASE WHEN LOWER(REPLACE(reason.effective_reason, ' ', '')) = 'invalidregistration' THEN 1 ELSE 0 END) AS invalid_registration_takedowns,
        SUM(CASE WHEN LOWER(REPLACE(COALESCE(reason.effective_reason, ''), ' ', '')) NOT IN ('lgrequest', 'invalidregistration') THEN 1 ELSE 0 END) AS other_takedowns
    FROM dss_upload_delivery dud
    INNER JOIN dss_upload_line dul
        ON dul.including_upload_delivery_id = dud.upload_delivery_id
    LEFT JOIN dss_organization line_org
        ON UPPER(line_org.organization_cd) = UPPER(dul.source_organization_cd)
    LEFT JOIN LATERAL (
        SELECT drl.rental_listing_id, drl.takedown_reason
        FROM dss_rental_listing drl
        WHERE drl.offering_organization_id = line_org.organization_id
          AND drl.platform_listing_no = dul.source_record_no
          AND drl.including_rental_listing_report_id IS NULL
        ORDER BY (drl.takedown_reason IS NULL), drl.rental_listing_id DESC
        LIMIT 1
    ) drl ON TRUE
    LEFT JOIN LATERAL (
        SELECT CASE
            WHEN COALESCE(BTRIM(drl.takedown_reason), '') <> '' THEN BTRIM(drl.takedown_reason)
            WHEN reason_pos.pos IS NULL THEN NULL
            ELSE NULLIF(BTRIM(TRIM(BOTH '"' FROM split_part(
                replace(replace(dul.source_line_txt, E'\r', ''), E'\n', ''),
                ',',
                reason_pos.pos
            ))), '')
        END AS effective_reason
        FROM (
            SELECT array_position(
                ARRAY(
                    SELECT lower(trim(both '"' FROM trim(h)))
                    FROM unnest(string_to_array(replace(replace(dud.source_header_txt, E'\r', ''), E'\n', ''), ',')) AS h
                ),
                'reason'
            ) AS pos
        ) reason_pos
    ) reason ON TRUE
    CROSS JOIN report_range rng
    WHERE dud.upload_delivery_type = 'Takedown Data'
      AND dud.report_period_ym IS NOT NULL
      AND dul.is_processed IS TRUE
      AND COALESCE(dul.is_validation_failure, FALSE) IS FALSE
      AND COALESCE(dul.is_system_failure, FALSE) IS FALSE
      AND dud.report_period_ym >= rng.range_start
      AND dud.report_period_ym < rng.range_end
      AND drl.rental_listing_id IS NOT NULL
    GROUP BY dud.report_period_ym, dud.providing_organization_id
) tc
    ON tc.report_month = m.report_month
   AND tc.organization_id = plat.organization_id
WHERE COALESCE(nc.notice_count, 0) > 0
   OR COALESCE(rc.request_count, 0) > 0
   OR COALESCE(tc.total_takedowns, 0) > 0
ORDER BY plat.platform_name;

-- Sprint 22+: Platform activity for a specific month using listing actions
WITH report_param AS (
    SELECT CAST('2026-05-20' AS date) AS report_date
),
report_range AS (
    SELECT
        report_param.report_date - INTERVAL '1 month' AS range_start,
        report_param.report_date AS range_end
    FROM report_param
)
SELECT
    TO_CHAR(rr.range_start, 'YYYY-MM') AS reporting_month,
    org.organization_nm AS platform_name,
    org.organization_cd AS platform_code,
    SUM(CASE WHEN drla.listing_action_type = 'NonComplianceNotice' THEN 1 ELSE 0 END) AS notice_count,
    SUM(CASE WHEN drla.listing_action_type = 'TakedownRequest' THEN 1 ELSE 0 END) AS request_count,
    SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' THEN 1 ELSE 0 END) AS total_takedowns,
    COUNT(DISTINCT CASE WHEN drla.listing_action_type = 'PlatformTakedown' THEN drla.rental_listing_id END) AS unique_listings,
    SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' AND drla.takedown_reason = 'LG Request' THEN 1 ELSE 0 END) AS lg_request_takedowns,
    SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' AND drla.takedown_reason = 'Invalid Registration' THEN 1 ELSE 0 END) AS invalid_registration_takedowns,
    SUM(CASE WHEN drla.listing_action_type = 'PlatformTakedown' AND drla.takedown_reason IS NULL THEN 1 ELSE 0 END) AS other_takedowns
FROM report_range rr
CROSS JOIN dss_rental_listing_action drla
INNER JOIN dss_rental_listing drl ON drl.rental_listing_id = drla.rental_listing_id
INNER JOIN dss_organization org ON org.organization_id = drl.offering_organization_id
WHERE drla.listing_action_type IN ('NonComplianceNotice', 'TakedownRequest', 'PlatformTakedown')
  AND org.organization_type = 'Platform'
  AND org.is_active IS TRUE
  AND drla.action_dtm >= rr.range_start
  AND drla.action_dtm < rr.range_end
GROUP BY rr.range_start, org.organization_nm, org.organization_cd
ORDER BY platform_name;
