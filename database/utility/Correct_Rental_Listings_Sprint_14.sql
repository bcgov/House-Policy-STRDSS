-- Populate lg_transfer_dtm if null when is_lg_transferred is true
UPDATE dss_rental_listing
SET lg_transfer_dtm = upd_dtm
WHERE including_rental_listing_report_id IS NULL AND lg_transfer_dtm IS NULL AND is_lg_transferred = true;

-- Correct 12-month nights stayed and separate reservations if differing match found
MERGE INTO dss_rental_listing AS tgt
USING (
	SELECT
		drl1.rental_listing_id,
		SUM(drl3.nights_booked_qty) AS nights_booked_qty,
		SUM(drl3.separate_reservations_qty) AS separate_reservations_qty
	FROM dss_rental_listing drl1
	JOIN dss_rental_listing drl2 ON drl2.rental_listing_id = drl1.derived_from_rental_listing_id
	JOIN dss_rental_listing drl3 ON drl2.platform_listing_no = drl3.platform_listing_no AND drl2.offering_organization_id = drl3.offering_organization_id
	JOIN dss_rental_listing_report drlr2 ON drlr2.rental_listing_report_id = drl2.including_rental_listing_report_id
	JOIN dss_rental_listing_report drlr3 ON drlr3.rental_listing_report_id = drl3.including_rental_listing_report_id
	WHERE drl1.including_rental_listing_report_id IS NULL
	  AND drlr2.report_period_ym < drlr3.report_period_ym + interval '12 months'
	GROUP BY drl1.rental_listing_id
	HAVING MIN(drl1.nights_booked_qty) != SUM(drl3.nights_booked_qty)
		OR MIN(drl1.separate_reservations_qty) != SUM(drl3.separate_reservations_qty)
) AS src
ON (tgt.rental_listing_id = src.rental_listing_id)
WHEN MATCHED
THEN
	UPDATE SET
		nights_booked_qty = src.nights_booked_qty,
		separate_reservations_qty = src.separate_reservations_qty;
