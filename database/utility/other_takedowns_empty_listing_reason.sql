-- Processed takedown lines whose master listing exists and takedown_reason is null.
-- Change range_start and range_end in params. range_end is exclusive.
-- '2026-01-01' through '2026-10-01' covers January to September 2026.
WITH params AS (
    SELECT
        DATE '2026-01-01' AS range_start,
        DATE '2026-10-01' AS range_end
)
SELECT
    dud.report_period_ym AS report_month,
    line_org.organization_cd AS organization_cd,
    drl.platform_listing_no,
    TRIM(BOTH '"' FROM TRIM(split_part(
        replace(dul.source_line_txt, E'\r', ''),
        ',',
        array_position(
            ARRAY(
                SELECT lower(trim(both '"' FROM trim(h)))
                FROM unnest(string_to_array(replace(dud.source_header_txt, E'\r', ''), ',')) AS h
            ),
            'reason'
        )
    ))) AS takedown_reason_by_platform_this_month,
    first_td.first_reported_takedown
FROM dss_upload_delivery dud
INNER JOIN dss_upload_line dul
    ON dul.including_upload_delivery_id = dud.upload_delivery_id
LEFT JOIN dss_organization line_org
    ON UPPER(line_org.organization_cd) = UPPER(dul.source_organization_cd)
LEFT JOIN LATERAL (
    SELECT drl.rental_listing_id, drl.takedown_reason, drl.platform_listing_no
    FROM dss_rental_listing drl
    WHERE drl.offering_organization_id = line_org.organization_id
      AND drl.platform_listing_no = dul.source_record_no
      AND drl.including_rental_listing_report_id IS NULL
    ORDER BY (drl.takedown_reason IS NULL), drl.rental_listing_id DESC
    LIMIT 1
) drl ON TRUE
LEFT JOIN LATERAL (
    SELECT MIN(fdud.report_period_ym) AS first_reported_takedown
    FROM dss_upload_line fdul
    INNER JOIN dss_upload_delivery fdud
        ON fdud.upload_delivery_id = fdul.including_upload_delivery_id
    WHERE fdud.upload_delivery_type = 'Takedown Data'
      AND fdul.source_record_no = dul.source_record_no
      AND UPPER(fdul.source_organization_cd) = UPPER(dul.source_organization_cd)
      AND COALESCE(fdul.is_validation_failure, FALSE) IS FALSE
      AND COALESCE(fdul.is_system_failure, FALSE) IS FALSE
) first_td ON TRUE
CROSS JOIN params
WHERE dud.upload_delivery_type = 'Takedown Data'
  AND dud.report_period_ym IS NOT NULL
  AND dul.is_processed IS TRUE
  AND COALESCE(dul.is_validation_failure, FALSE) IS FALSE
  AND COALESCE(dul.is_system_failure, FALSE) IS FALSE
  AND dud.report_period_ym >= params.range_start
  AND dud.report_period_ym < params.range_end
  AND drl.rental_listing_id IS NOT NULL
  AND drl.takedown_reason IS NULL
ORDER BY dud.report_period_ym;
