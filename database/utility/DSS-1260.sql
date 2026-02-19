select * from dss_organization d where d.organization_nm like '%Airbnb%'

-- AirBNB id 202

select *
from dss_rental_listing d 
where d.offering_organization_id = 202
and d.is_taken_down = true
and d.including_rental_listing_report_id is null;

SELECT COUNT(DISTINCT platform_listing_no) AS unique_listing_count
FROM dss_rental_listing d 
WHERE d.offering_organization_id = 202
  AND d.is_taken_down = true
  AND d.including_rental_listing_report_id IS NULL;

select *
from dss_upload_delivery d 
where d.upload_delivery_type = 'Takedown Data'
and d.providing_organization_id = 202

SELECT SUM(upload_lines_processed) AS total_lines_processed
FROM dss_upload_delivery
WHERE upload_delivery_type = 'Takedown Data'
  AND providing_organization_id = 202;


select count(*)
from dss_email_message d 
where d.email_message_type = 'Completed Takedown'
and d.involved_in_organization_id = 202

SELECT COUNT(DISTINCT platform_listing_no) AS unique_platform_listing_count
FROM dss_rental_listing
WHERE offering_organization_id = 202
  AND including_rental_listing_report_id IS NULL;

  select *
from dss_upload_delivery d
join dss_upload_delivery d on ul.including_upload_delivery_id = d.upload_delivery_id
where d.providing_organization_id = 202

SELECT ul.*
FROM dss_upload_line ul
JOIN dss_upload_delivery d ON ul.including_upload_delivery_id = d.upload_delivery_id
WHERE d.providing_organization_id = 202
and d.upload_delivery_type = 'Takedown Data'
and ul.is_processed = true;

-- Compare the reported total lines and processed lines with the real count from the lines table
SELECT 
    d.upload_delivery_id,
    d.upd_dtm AS upload_date,
    d.upload_delivery_type,
    d.upload_status,
    -- From delivery table
    d.upload_lines_total AS reported_total_lines,
    d.upload_lines_processed AS reported_processed_lines,
    -- Actual counts from upload_line table
    COUNT(ul.upload_line_id) AS actual_total_lines,
    COUNT(*) FILTER (WHERE ul.is_processed = true) AS actual_processed_lines,
    COUNT(*) FILTER (WHERE ul.is_processed = false) AS actual_not_processed_lines,
    COUNT(*) FILTER (WHERE ul.is_system_failure = true) AS system_failure_lines,
    -- Comparison
    d.upload_lines_processed - COUNT(*) FILTER (WHERE ul.is_processed = true) AS processed_difference
FROM dss_upload_delivery d
LEFT JOIN dss_upload_line ul ON ul.including_upload_delivery_id = d.upload_delivery_id
WHERE d.providing_organization_id = 202
  AND d.upload_delivery_type = 'Takedown Data'
GROUP BY 
    d.upload_delivery_id, 
    d.upd_dtm, 
    d.upload_delivery_type,
    d.upload_status,
    d.upload_lines_total, 
    d.upload_lines_processed
ORDER BY d.upd_dtm DESC;

-- Recitified the total and processed lines to align with the reality from the upload_lines table
WITH actual_counts AS (
    SELECT 
        ul.including_upload_delivery_id,
        COUNT(*) AS actual_total_lines,
        COUNT(*) FILTER (WHERE ul.is_processed = true) AS actual_processed_lines
    FROM dss_upload_line ul
    WHERE ul.including_upload_delivery_id IN (
        SELECT upload_delivery_id 
        FROM dss_upload_delivery 
        WHERE upload_delivery_type = 'Takedown Data'
    )
    GROUP BY ul.including_upload_delivery_id
)
UPDATE dss_upload_delivery d
SET 
    upload_lines_total = ac.actual_total_lines,
    upload_lines_processed = ac.actual_processed_lines
FROM actual_counts ac
WHERE d.upload_delivery_id = ac.including_upload_delivery_id
  AND d.upload_delivery_type = 'Takedown Data'
  AND (
      d.upload_lines_total <> ac.actual_total_lines 
      OR d.upload_lines_processed <> ac.actual_processed_lines
  )
RETURNING 
    d.upload_delivery_id,
    d.upload_lines_total,
    d.upload_lines_processed;


-- Monthly Notice of Non-compliance report by platform since May 2024
SELECT 
    TO_CHAR(dem.message_delivery_dtm, 'YYYY-MM') AS month,
    DATE_TRUNC('month', dem.message_delivery_dtm) AS month_start,
    org.organization_id,
    org.organization_nm AS platform_name,
    org.organization_cd AS platform_code,
    COUNT(*) AS notice_count,
    COUNT(DISTINCT dem.concerned_with_rental_listing_id) AS unique_listings_notified
FROM dss_email_message dem
JOIN dss_organization org ON org.organization_id = dem.involved_in_organization_id
WHERE dem.email_message_type = 'Notice of Takedown'
  AND dem.message_delivery_dtm >= '2024-05-01'
GROUP BY 
    TO_CHAR(dem.message_delivery_dtm, 'YYYY-MM'),
    DATE_TRUNC('month', dem.message_delivery_dtm),
    org.organization_id,
    org.organization_nm,
    org.organization_cd
ORDER BY month_start, org.organization_nm;


-- what reasons exist
SELECT DISTINCT takedown_reason 
FROM dss_rental_listing 
WHERE takedown_reason IS NOT NULL
ORDER BY takedown_reason;

-- Monthly Takedown Request report by platform since May 2024
SELECT 
    TO_CHAR(dem.message_delivery_dtm, 'YYYY-MM') AS month,
    DATE_TRUNC('month', dem.message_delivery_dtm) AS month_start,
    org.organization_id,
    org.organization_nm AS platform_name,
    org.organization_cd AS platform_code,
    COUNT(*) AS notice_count,
    COUNT(DISTINCT dem.concerned_with_rental_listing_id) AS unique_listings_notified
FROM dss_email_message dem
JOIN dss_organization org ON org.organization_id = dem.involved_in_organization_id
WHERE dem.email_message_type = 'Takedown Request'
  AND dem.message_delivery_dtm >= '2024-05-01'
GROUP BY 
    TO_CHAR(dem.message_delivery_dtm, 'YYYY-MM'),
    DATE_TRUNC('month', dem.message_delivery_dtm),
    org.organization_id,
    org.organization_nm,
    org.organization_cd
ORDER BY month_start, org.organization_nm;