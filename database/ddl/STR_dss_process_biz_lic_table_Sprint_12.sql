CREATE OR REPLACE PROCEDURE dss_process_biz_lic_table(lg_id BIGINT)
LANGUAGE plpgsql
AS $$
BEGIN

    IF NOT EXISTS (
        SELECT 1
        FROM pg_tables
        WHERE schemaname = 'public'
        AND tablename = 'biz_lic_table'
    ) THEN
        RAISE NOTICE 'biz_lic_table does not exist. Exiting procedure.';
        RETURN;
    END IF;
	
    -- Unlink before Deletion
	MERGE INTO dss_rental_listing AS tgt
	USING (
		SELECT business_licence_id
		FROM dss_business_licence
		WHERE providing_organization_id IN (SELECT providing_organization_id FROM biz_lic_table)
		  AND business_licence_no NOT IN (SELECT business_licence_no FROM biz_lic_table)
	) AS src
	ON (tgt.governing_business_licence_id = src.business_licence_id)
    WHEN MATCHED THEN
		UPDATE SET governing_business_licence_id = NULL;

    -- Deletion 
	DELETE FROM dss_business_licence
	WHERE providing_organization_id IN (SELECT providing_organization_id FROM biz_lic_table)
	  AND business_licence_no NOT IN (SELECT business_licence_no FROM biz_lic_table);

    -- Insert into dss_business_licence from biz_lic_table or update if exists
	MERGE INTO dss_business_licence AS tgt
	USING (SELECT * FROM biz_lic_table) AS src
	ON (tgt.providing_organization_id = src.providing_organization_id AND
		tgt.business_licence_no = src.business_licence_no)
    WHEN MATCHED THEN UPDATE SET 
        expiry_dt = src.expiry_dt,
        physical_rental_address_txt = src.physical_rental_address_txt,
        licence_type_txt = src.licence_type_txt,
        restriction_txt = src.restriction_txt,
        business_nm = src.business_nm,
        mailing_street_address_txt = src.mailing_street_address_txt,
        mailing_city_nm = src.mailing_city_nm,
        mailing_province_cd = src.mailing_province_cd,
        mailing_postal_cd = src.mailing_postal_cd,
        business_owner_nm = src.business_owner_nm,
        business_owner_phone_no = src.business_owner_phone_no,
        business_owner_email_address_dsc = src.business_owner_email_address_dsc,
        business_operator_nm = src.business_operator_nm,
        business_operator_phone_no = src.business_operator_phone_no,
        business_operator_email_address_dsc = src.business_operator_email_address_dsc,
        infraction_txt = src.infraction_txt,
        infraction_dt = src.infraction_dt,
        property_zone_txt = src.property_zone_txt,
        available_bedrooms_qty = src.available_bedrooms_qty,
        max_guests_allowed_qty = src.max_guests_allowed_qty,
        is_principal_residence = src.is_principal_residence,
        is_owner_living_onsite = src.is_owner_living_onsite,
        is_owner_property_tenant = src.is_owner_property_tenant,
        property_folio_no = src.property_folio_no,
        property_parcel_identifier_no = src.property_parcel_identifier_no,
        property_legal_description_txt = src.property_legal_description_txt,
        licence_status_type = src.licence_status_type
	WHEN NOT MATCHED THEN INSERT (
        business_licence_no, expiry_dt, physical_rental_address_txt, licence_type_txt, restriction_txt, 
        business_nm, mailing_street_address_txt, mailing_city_nm, mailing_province_cd, mailing_postal_cd, 
        business_owner_nm, business_owner_phone_no, business_owner_email_address_dsc, business_operator_nm, 
        business_operator_phone_no, business_operator_email_address_dsc, infraction_txt, infraction_dt, 
        property_zone_txt, available_bedrooms_qty, max_guests_allowed_qty, is_principal_residence, 
        is_owner_living_onsite, is_owner_property_tenant, property_folio_no, property_parcel_identifier_no, 
        property_legal_description_txt, licence_status_type, providing_organization_id)
    VALUES (
        src.business_licence_no, src.expiry_dt, src.physical_rental_address_txt, src.licence_type_txt, src.restriction_txt,
        src.business_nm, src.mailing_street_address_txt, src.mailing_city_nm, src.mailing_province_cd, src.mailing_postal_cd,
        src.business_owner_nm, src.business_owner_phone_no, src.business_owner_email_address_dsc, src.business_operator_nm,
        src.business_operator_phone_no, src.business_operator_email_address_dsc, src.infraction_txt, src.infraction_dt,
        src.property_zone_txt, src.available_bedrooms_qty, src.max_guests_allowed_qty, src.is_principal_residence,
        src.is_owner_living_onsite, src.is_owner_property_tenant, src.property_folio_no, src.property_parcel_identifier_no,
        src.property_legal_description_txt, src.licence_status_type, src.providing_organization_id);

    -- Update dss_rental_listing if differing match found
	MERGE INTO dss_rental_listing AS tgt
	USING (
		SELECT drl.rental_listing_id, dbl.business_licence_id
		FROM dss_rental_listing drl
		JOIN dss_physical_address dpa ON drl.locating_physical_address_id = dpa.physical_address_id
		JOIN dss_organization lgs ON lgs.organization_id = dpa.containing_organization_id AND dpa.match_score_amt > 1
		LEFT JOIN dss_business_licence dbl ON (
            regexp_replace(UPPER(drl.business_licence_no), '[^A-Z0-9]+', '', 'g') = regexp_replace(UPPER(dbl.business_licence_no), '[^A-Z0-9]+', '', 'g') 
            AND lgs.managing_organization_id = dbl.providing_organization_id)
		WHERE drl.including_rental_listing_report_id IS NULL 
		  AND COALESCE(drl.governing_business_licence_id, -1) != COALESCE(dbl.business_licence_id, -1)
		  AND dbl.providing_organization_id = lg_id
	) AS src
	ON (tgt.rental_listing_id = src.rental_listing_id)
    WHEN MATCHED THEN
		UPDATE SET governing_business_licence_id = src.business_licence_id;

    -- Optional: Truncate the temporary table after processing
    TRUNCATE TABLE biz_lic_table;

    -- Notify of completion
    RAISE NOTICE 'Data has been processed from biz_lic_table to dss_business_licence for lg_id %', lg_id;
END $$;
