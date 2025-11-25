DROP VIEW IF EXISTS vw_str_portal_data CASCADE;

-- =============================================================================
-- STRR Registration Data View
-- Purpose: Provides comprehensive rental listing data using registration_number
-- =============================================================================

CREATE OR REPLACE VIEW vw_str_portal_data AS
SELECT 
    -- =========================================================================
    -- CORE REGISTRATION INFORMATION
    -- =========================================================================
    r.id AS registration_id,
    r.registration_number,
    r.registration_type,
    r.status AS registration_status,
    r.start_date AS registration_start_date,
    r.expiry_date AS registration_expiry_date,
    r.updated_date AS registration_updated_date,
    r.cancelled_date AS registration_cancelled_date,
    r.is_set_aside,
    r.noc_status,
    r.sbc_account_id,
    
    -- =========================================================================
    -- PROPERTY DETAILS
    -- =========================================================================
    rp.nickname AS property_nickname,
    rp.property_type,
    rp.ownership_type,
    rp.is_principal_residence,
    rp.pr_exempt_reason,
    rp.space_type,
    rp.host_residence,
    rp.is_unit_on_principal_residence_property,
    rp.number_of_rooms_for_rent,
    rp.parcel_identifier,
    rp.local_business_licence,
    rp.local_business_licence_expiry_date,
    rp.bl_exempt_reason,
    rp.strata_hotel_registration_number,
    rp.service_provider,
    rp.rental_act_accepted,
    
    -- =========================================================================
    -- PROPERTY ADDRESS
    -- =========================================================================
    pa.unit_number AS property_unit_number,
    pa.street_number AS property_street_number,
    pa.street_address AS property_street_address,
    pa.street_address_additional AS property_street_address_additional,
    pa.city AS property_city,
    pa.province AS property_province,
    pa.postal_code AS property_postal_code,
    pa.country AS property_country,
    
    -- =========================================================================
    -- PRIMARY CONTACT INFORMATION
    -- =========================================================================
    pc.firstname AS primary_contact_firstname,
    pc.lastname AS primary_contact_lastname,
    pc.middlename AS primary_contact_middlename,
    pc.preferredname AS primary_contact_preferredname,
    pc.email AS primary_contact_email,
    pc.phone_country_code AS primary_contact_phone_country_code,
    pc.phone_number AS primary_contact_phone,
    pc.phone_extension AS primary_contact_phone_ext,
    pc.fax_number AS primary_contact_fax,
    pc.date_of_birth AS primary_contact_dob,
    pc.business_number AS primary_contact_business_number,
    pc.job_title AS primary_contact_job_title,
    
    -- =========================================================================
    -- PRIMARY CONTACT ADDRESS
    -- =========================================================================
    pca.unit_number AS primary_contact_unit_number,
    pca.street_number AS primary_contact_street_number,
    pca.street_address AS primary_contact_street_address,
    pca.street_address_additional AS primary_contact_street_address_additional,
    pca.city AS primary_contact_city,
    pca.province AS primary_contact_province,
    pca.postal_code AS primary_contact_postal_code,
    pca.country AS primary_contact_country,
    
    -- =========================================================================
    -- SECONDARY CONTACT INFORMATION (via property_contacts)
    -- =========================================================================
    sc.firstname AS secondary_contact_firstname,
    sc.lastname AS secondary_contact_lastname,
    sc.email AS secondary_contact_email,
    sc.phone_country_code AS secondary_contact_phone_country_code,
    sc.phone_number AS secondary_contact_phone,
    sc.phone_extension AS secondary_contact_phone_ext,
    prop_cont.contact_type AS secondary_contact_type,
    
    -- =========================================================================
    -- PROPERTY MANAGER INFORMATION
    -- =========================================================================
    pm.property_manager_type,
    pm.business_legal_name AS property_manager_business_name,
    pm.business_number AS property_manager_business_number,
    
    -- Property Manager Primary Contact
    pmc.firstname AS property_manager_contact_firstname,
    pmc.lastname AS property_manager_contact_lastname,
    pmc.email AS property_manager_contact_email,
    pmc.phone_country_code AS property_manager_phone_country_code,
    pmc.phone_number AS property_manager_phone,
    pmc.phone_extension AS property_manager_phone_ext,
    
    -- Property Manager Business Address
    pmba.unit_number AS property_manager_unit_number,
    pmba.street_number AS property_manager_street_number,
    pmba.street_address AS property_manager_street_address,
    pmba.city AS property_manager_city,
    pmba.province AS property_manager_province,
    pmba.postal_code AS property_manager_postal_code,
    pmba.country AS property_manager_country,
    
    -- =========================================================================
    -- PLATFORM INFORMATION (Listing platforms like Airbnb, VRBO)
    -- =========================================================================
    plat.id AS platform_id,
    plat.legal_name AS platform_legal_name,
    pb.name AS platform_brand_name,
    
    -- =========================================================================
    -- PROPERTY LISTING URLS
    -- =========================================================================
    pl.id AS listing_id,
    pl.url AS listing_url,
    pl.type AS listing_type,
    pl.platform AS listing_platform,
    
    -- =========================================================================
    -- CONDITIONS OF APPROVAL
    -- =========================================================================
    coa.id AS conditions_of_approval_id,
    coa.preapproved_conditions,
    coa.custom_conditions,
    coa."minBookingDays" AS min_booking_days,
    
    -- =========================================================================
    -- CERTIFICATE INFORMATION
    -- =========================================================================
    cert.id AS certificate_id,
    cert.issued_date AS certificate_issued_date

FROM registrations r

-- Join to rental property
LEFT JOIN rental_properties rp ON r.id = rp.registration_id

-- Join to property address
LEFT JOIN addresses pa ON rp.address_id = pa.id

-- Join to primary contact (from property_contacts where is_primary = true)
LEFT JOIN property_contacts prop_cont ON rp.id = prop_cont.property_id AND prop_cont.is_primary = true
LEFT JOIN contacts pc ON prop_cont.contact_id = pc.id
LEFT JOIN addresses pca ON pc.address_id = pca.id

-- Join to secondary contacts (additional contacts)
LEFT JOIN property_contacts sec_prop_cont ON rp.id = sec_prop_cont.property_id AND sec_prop_cont.is_primary = false
LEFT JOIN contacts sc ON sec_prop_cont.contact_id = sc.id

-- Join to property manager
LEFT JOIN property_manager pm ON rp.property_manager_id = pm.id
LEFT JOIN contacts pmc ON pm.primary_contact_id = pmc.id
LEFT JOIN addresses pmba ON pm.business_mailing_address_id = pmba.id

-- Join to platform information
LEFT JOIN platform_registration pr ON r.id = pr.registration_id
LEFT JOIN platforms plat ON pr.platform_id = plat.id
LEFT JOIN platform_brands pb ON plat.id = pb.platform_id

-- Join to property listings (URLs)
LEFT JOIN property_listings pl ON rp.id = pl.property_id

-- Join to conditions of approval
LEFT JOIN conditions_of_approval coa ON r.id = coa.registration_id

-- Join to certificates
LEFT JOIN certificates cert ON r.id = cert.registration_id


-- =============================================================================
-- USAGE EXAMPLE: Query by registration number
-- =============================================================================
-- SELECT * FROM vw_str_portal_data 
-- WHERE registration_number = 'REG-2024-001234';

-- =============================================================================
-- NOTES:
-- - Multiple secondary contacts may exist (1:many relationship)
-- - Multiple platforms may exist per registration (1:many relationship)
-- - Multiple listing URLs may exist per property (1:many relationship)
-- - This will result in multiple rows per registration if there are multiple
--   secondary contacts, platforms, or listings
-- - Consider using JSON aggregation if a single-row result is preferred
-- =============================================================================