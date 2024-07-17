UPDATE dss_rental_listing t
SET
    nights_booked_qty = COALESCE(ytd.total_nights_booked, 0),
    separate_reservations_qty = COALESCE(ytd.total_reservations, 0)
FROM
    dss_rental_listing drl
JOIN (
    SELECT
        offering_organization_id,
        platform_listing_no,
        MAX(irr.report_period_ym) AS max_report_period_ym,
        CAST(SUM(CAST(nights_booked_qty AS SMALLINT)) AS SMALLINT) AS total_nights_booked,
        CAST(SUM(CAST(separate_reservations_qty AS SMALLINT)) AS SMALLINT) AS total_reservations
    FROM
        dss_rental_listing
    JOIN dss_rental_listing_report irr ON
        dss_rental_listing.including_rental_listing_report_id = irr.rental_listing_report_id
    WHERE
        irr.report_period_ym >= DATE_TRUNC('year', CURRENT_DATE)
        AND irr.report_period_ym <= CURRENT_DATE
    GROUP BY
        offering_organization_id,
        platform_listing_no
) AS ytd ON
    drl.offering_organization_id = ytd.offering_organization_id
    AND drl.platform_listing_no = ytd.platform_listing_no
    AND drl.including_rental_listing_report_id IS NULL
WHERE
    t.including_rental_listing_report_id IS NULL
    AND t.offering_organization_id = drl.offering_organization_id
    AND t.platform_listing_no = drl.platform_listing_no
   ;
   
update dss_rental_listing set listing_status_type = 'N' where including_rental_listing_report_id is null and is_new = true and is_active = true;
update dss_rental_listing set listing_status_type = 'A' where including_rental_listing_report_id is null and is_new = false and is_active = true;
update dss_rental_listing set listing_status_type = 'I' where including_rental_listing_report_id is null and is_active = false;
   