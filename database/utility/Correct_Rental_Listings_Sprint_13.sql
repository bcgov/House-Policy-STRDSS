-- Correct dss_rental_listing.effective_business_licence_no if differing match found
MERGE INTO dss_rental_listing AS tgt
USING (
	SELECT
		drl.rental_listing_id,
		CASE WHEN dbl.business_licence_id IS NULL
			THEN regexp_replace(UPPER(drl.business_licence_no), '[^A-Z0-9]+', '', 'g')
			ELSE regexp_replace(UPPER(dbl.business_licence_no), '[^A-Z0-9]+', '', 'g')
		END AS normalized_business_licence_no
	FROM dss_rental_listing drl
	LEFT JOIN dss_business_licence dbl ON drl.governing_business_licence_id = dbl.business_licence_id
	WHERE drl.including_rental_listing_report_id IS NULL
) AS src
ON (tgt.rental_listing_id = src.rental_listing_id)
WHEN MATCHED
 AND COALESCE(src.normalized_business_licence_no, '') !=  COALESCE(tgt.effective_business_licence_no, '')
THEN
	UPDATE SET effective_business_licence_no = src.normalized_business_licence_no;

-- Correct dss_rental_listing.effective_host_nm if differing match found
MERGE INTO dss_rental_listing AS tgt
USING (
	SELECT
		drl.rental_listing_id,
		regexp_replace(UPPER(drlc.full_nm), '[^A-Z0-9]+', '', 'g') AS effective_host_nm
	FROM dss_rental_listing drl
	LEFT JOIN dss_rental_listing_contact drlc ON drl.rental_listing_id = drlc.contacted_through_rental_listing_id AND drlc.is_property_owner
	WHERE drl.including_rental_listing_report_id IS NULL
) AS src
ON (tgt.rental_listing_id = src.rental_listing_id)
WHEN MATCHED
 AND COALESCE(src.effective_host_nm, '') !=  COALESCE(tgt.effective_host_nm, '')
THEN
	UPDATE SET effective_host_nm = src.effective_host_nm;
