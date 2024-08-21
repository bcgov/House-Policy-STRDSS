CREATE OR REPLACE PROCEDURE dss_process_biz_lic_table()
LANGUAGE plpgsql
AS $$
BEGIN

    -- Deletion 
	
	
    -- Insert into dss_business_licence from biz_lic_table or update if exists
    INSERT INTO public.dss_business_licence (
        business_licence_no, expiry_dt, physical_rental_address_txt, licence_type_txt, restriction_txt, 
        business_nm, mailing_street_address_txt, mailing_city_nm, mailing_province_cd, mailing_postal_cd, 
        business_owner_nm, business_owner_phone_no, business_owner_email_address_dsc, business_operator_nm, 
        business_operator_phone_no, business_operator_email_address_dsc, infraction_txt, infraction_dt, 
        property_zone_txt, available_bedrooms_qty, max_guests_allowed_qty, is_principal_residence, 
        is_owner_living_onsite, is_owner_property_tenant, property_folio_no, property_parcel_identifier_no, 
        property_legal_description_txt, licence_status_type, providing_organization_id, upd_dtm
    )
    SELECT 
        business_licence_no, expiry_dt, physical_rental_address_txt, licence_type_txt, restriction_txt, 
        business_nm, mailing_street_address_txt, mailing_city_nm, mailing_province_cd, mailing_postal_cd, 
        business_owner_nm, business_owner_phone_no, business_owner_email_address_dsc, business_operator_nm, 
        business_operator_phone_no, business_operator_email_address_dsc, infraction_txt, infraction_dt, 
        property_zone_txt, available_bedrooms_qty, max_guests_allowed_qty, 
        is_principal_residence, is_owner_living_onsite, is_owner_property_tenant,
        property_folio_no, property_parcel_identifier_no, property_legal_description_txt, 
        licence_status_type, providing_organization_id,
        NOW() -- Set the current timestamp for upd_dtm
    FROM biz_lic_table
    ON CONFLICT (providing_organization_id, business_licence_no)
    DO UPDATE
    SET 
        expiry_dt = EXCLUDED.expiry_dt,
        physical_rental_address_txt = EXCLUDED.physical_rental_address_txt,
        licence_type_txt = EXCLUDED.licence_type_txt,
        restriction_txt = EXCLUDED.restriction_txt,
        business_nm = EXCLUDED.business_nm,
        mailing_street_address_txt = EXCLUDED.mailing_street_address_txt,
        mailing_city_nm = EXCLUDED.mailing_city_nm,
        mailing_province_cd = EXCLUDED.mailing_province_cd,
        mailing_postal_cd = EXCLUDED.mailing_postal_cd,
        business_owner_nm = EXCLUDED.business_owner_nm,
        business_owner_phone_no = EXCLUDED.business_owner_phone_no,
        business_owner_email_address_dsc = EXCLUDED.business_owner_email_address_dsc,
        business_operator_nm = EXCLUDED.business_operator_nm,
        business_operator_phone_no = EXCLUDED.business_operator_phone_no,
        business_operator_email_address_dsc = EXCLUDED.business_operator_email_address_dsc,
        infraction_txt = EXCLUDED.infraction_txt,
        infraction_dt = EXCLUDED.infraction_dt,
        property_zone_txt = EXCLUDED.property_zone_txt,
        available_bedrooms_qty = EXCLUDED.available_bedrooms_qty,
        max_guests_allowed_qty = EXCLUDED.max_guests_allowed_qty,
        is_principal_residence = EXCLUDED.is_principal_residence,
        is_owner_living_onsite = EXCLUDED.is_owner_living_onsite,
        is_owner_property_tenant = EXCLUDED.is_owner_property_tenant,
        property_folio_no = EXCLUDED.property_folio_no,
        property_parcel_identifier_no = EXCLUDED.property_parcel_identifier_no,
        property_legal_description_txt = EXCLUDED.property_legal_description_txt,
        licence_status_type = EXCLUDED.licence_status_type,
        upd_dtm = NOW(); -- Update the update timestamp
    
    -- Optional: Truncate the temporary table after processing
    TRUNCATE TABLE biz_lic_table;
    
    -- Notify of completion
    RAISE NOTICE 'Data has been processed from biz_lic_table to dss_business_licence';
    
END $$;
