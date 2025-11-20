
-- Attempt to retrieve bulk takedown notice details along with associated requests
-- 1.	Primary indicator: concerned_with_rental_listing_id IS NOT NULL
-- 2.	Secondary indicator: Multiple records with same initiating_user_identity_id + message_delivery_dtm (within same second)

WITH all_emails AS (
    SELECT 
        email_message_id,
        email_message_type,
        message_delivery_dtm,
        initiating_user_identity_id,
        concerned_with_rental_listing_id,
        unreported_listing_no,
        DATE_TRUNC('second', message_delivery_dtm) AS delivery_second
    FROM dss_email_message
    WHERE email_message_type IN ('Takedown Request', 'Notice of Takedown')
      AND EXTRACT(YEAR FROM message_delivery_dtm) = 2025
),
bulk_batches AS (
    -- Identify bulk submission batches (multiple from same user in same second)
    SELECT 
        email_message_type,
        initiating_user_identity_id,
        delivery_second,
        COUNT(*) AS requests_in_bulk
    FROM all_emails
    WHERE concerned_with_rental_listing_id IS NOT NULL
    GROUP BY 
        email_message_type,
        initiating_user_identity_id,
        delivery_second
    HAVING COUNT(*) > 1
),
classified_emails AS (
    SELECT 
        ae.email_message_id,
        ae.email_message_type,
        ae.message_delivery_dtm,
        TO_CHAR(ae.message_delivery_dtm, 'YYYY-MM') AS month,
        DATE_TRUNC('month', ae.message_delivery_dtm) AS month_start,
        CASE 
            WHEN bb.delivery_second IS NOT NULL THEN 'Bulk'
            ELSE 'Individual'
        END AS request_type
    FROM all_emails ae
    LEFT JOIN bulk_batches bb 
        ON ae.email_message_type = bb.email_message_type
       AND ae.initiating_user_identity_id = bb.initiating_user_identity_id
       AND ae.delivery_second = bb.delivery_second
)
SELECT 
    month,
    month_start,
    -- Takedown Requests
    COUNT(*) FILTER (WHERE email_message_type = 'Takedown Request' AND request_type = 'Bulk') AS bulk_takedown_requests,
    COUNT(*) FILTER (WHERE email_message_type = 'Takedown Request' AND request_type = 'Individual') AS individual_takedown_requests,
    COUNT(*) FILTER (WHERE email_message_type = 'Takedown Request') AS total_takedown_requests,
    -- Notice of Takedown
    COUNT(*) FILTER (WHERE email_message_type = 'Notice of Takedown' AND request_type = 'Bulk') AS bulk_notice_takedown,
    COUNT(*) FILTER (WHERE email_message_type = 'Notice of Takedown' AND request_type = 'Individual') AS individual_notice_takedown,
    COUNT(*) FILTER (WHERE email_message_type = 'Notice of Takedown') AS total_notice_takedown,
    -- Overall totals
    COUNT(*) FILTER (WHERE request_type = 'Bulk') AS total_bulk,
    COUNT(*) FILTER (WHERE request_type = 'Individual') AS total_individual,
    COUNT(*) AS grand_total,
    ROUND(100.0 * COUNT(*) FILTER (WHERE request_type = 'Bulk') / NULLIF(COUNT(*), 0), 1) AS pct_bulk
FROM classified_emails
GROUP BY month, month_start
ORDER BY month_start;
