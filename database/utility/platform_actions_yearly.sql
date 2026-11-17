-- Consolidated platform action counts by year.
-- Set report_years, range_start, and range_end. range_end is exclusive.
-- With report_years 2024, 2025, and 2026, range_start is 2024-01-01 and range_end is 2027-01-01.
WITH params AS (
    SELECT
        ARRAY[2024, 2025, 2026] AS report_years,
        DATE '2024-01-01' AS range_start,
        DATE '2027-01-01' AS range_end
),
years AS (
    SELECT unnest(report_years) AS report_year
    FROM params
),
notices AS (
    SELECT
        EXTRACT(YEAR FROM dem.message_delivery_dtm)::int AS report_year,
        COUNT(*) FILTER (WHERE dem.email_message_type = 'Notice of Takedown') AS notice_count,
        COUNT(*) FILTER (WHERE dem.email_message_type = 'Takedown Request') AS request_count
    FROM dss_email_message dem
    INNER JOIN dss_organization org
        ON org.organization_id = dem.involved_in_organization_id
    CROSS JOIN params
    WHERE dem.email_message_type IN ('Notice of Takedown', 'Takedown Request')
      AND org.organization_type = 'Platform'
      AND org.is_active IS TRUE
      AND dem.message_delivery_dtm >= params.range_start
      AND dem.message_delivery_dtm < params.range_end
    GROUP BY 1
),
takedowns AS (
    SELECT
        EXTRACT(YEAR FROM dud.report_period_ym)::int AS report_year,
        COUNT(*) AS total_takedowns,
        COUNT(DISTINCT (dul.source_organization_cd, dul.source_record_no)) AS unique_listings,
        SUM(CASE WHEN LOWER(REPLACE(reason.effective_reason, ' ', '')) = 'lgrequest' THEN 1 ELSE 0 END) AS lg_request_takedowns,
        SUM(CASE WHEN LOWER(REPLACE(reason.effective_reason, ' ', '')) = 'invalidregistration' THEN 1 ELSE 0 END) AS invalid_registration_takedowns,
        SUM(CASE WHEN LOWER(REPLACE(COALESCE(reason.effective_reason, ''), ' ', '')) NOT IN ('lgrequest', 'invalidregistration') THEN 1 ELSE 0 END) AS other_takedowns
    FROM dss_upload_delivery dud
    INNER JOIN dss_organization plat
        ON plat.organization_id = dud.providing_organization_id
       AND plat.organization_type = 'Platform'
       AND plat.is_active IS TRUE
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
    CROSS JOIN params
    WHERE dud.upload_delivery_type = 'Takedown Data'
      AND dud.report_period_ym IS NOT NULL
      AND dul.is_processed IS TRUE
      AND COALESCE(dul.is_validation_failure, FALSE) IS FALSE
      AND COALESCE(dul.is_system_failure, FALSE) IS FALSE
      AND dud.report_period_ym >= params.range_start
      AND dud.report_period_ym < params.range_end
      AND drl.rental_listing_id IS NOT NULL
    GROUP BY 1
)
SELECT
    years.report_year,
    COALESCE(notices.notice_count, 0) AS notice_count,
    COALESCE(notices.request_count, 0) AS request_count,
    COALESCE(takedowns.total_takedowns, 0) AS total_takedowns,
    COALESCE(takedowns.unique_listings, 0) AS unique_listings,
    COALESCE(takedowns.lg_request_takedowns, 0) AS lg_request_takedowns,
    COALESCE(takedowns.invalid_registration_takedowns, 0) AS invalid_registration_takedowns,
    COALESCE(takedowns.other_takedowns, 0) AS other_takedowns
FROM years
LEFT JOIN notices
    ON notices.report_year = years.report_year
LEFT JOIN takedowns
    ON takedowns.report_year = years.report_year
ORDER BY years.report_year;
