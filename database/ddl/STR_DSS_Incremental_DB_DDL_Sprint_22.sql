-- Drop dependent view(s)
DROP VIEW IF EXISTS dss_rental_listing_vw CASCADE;

ALTER TABLE dss_physical_address
ALTER COLUMN unit_no TYPE varchar(100),
ALTER COLUMN reg_rental_unit_no TYPE varchar(100);

CREATE OR REPLACE VIEW dss_rental_listing_vw AS SELECT drl.rental_listing_id,
    dlst.listing_status_type,
    dlst.listing_status_type_nm,
    dlst.listing_status_sort_no,
    ( SELECT max(drlr.report_period_ym) AS max
           FROM (dss_rental_listing drl2
             JOIN dss_rental_listing_report drlr ON ((drlr.rental_listing_report_id = drl2.including_rental_listing_report_id)))
          WHERE ((drl2.offering_organization_id = drl.offering_organization_id) AND ((drl2.platform_listing_no)::text = (drl.platform_listing_no)::text))) AS latest_report_period_ym,
    drl.is_active,
    drl.is_new,
    drl.is_taken_down,
    drl.takedown_reason,
    drl.is_lg_transferred,
    drl.is_changed_address,
    drl.offering_organization_id,
    org.organization_cd AS offering_organization_cd,
    org.organization_nm AS offering_organization_nm,
    drl.platform_listing_no,
    drl.platform_listing_url,
    dpa.original_address_txt,
    dpa.is_match_corrected,
    dpa.is_match_verified,
    dpa.match_score_amt,
    dpa.match_address_txt,
    dpa.province_cd AS address_sort_1_province_cd,
    dpa.locality_nm AS address_sort_2_locality_nm,
    dpa.locality_type_dsc AS address_sort_3_locality_type_dsc,
    dpa.street_nm AS address_sort_4_street_nm,
    dpa.street_type_dsc AS address_sort_5_street_type_dsc,
    dpa.street_direction_dsc AS address_sort_6_street_direction_dsc,
    dpa.civic_no AS address_sort_7_civic_no,
    dpa.unit_no AS address_sort_8_unit_no,
    ( SELECT string_agg((drlc.full_nm)::text, ' ; '::text) AS string_agg
           FROM dss_rental_listing_contact drlc
          WHERE (drlc.contacted_through_rental_listing_id = drl.rental_listing_id)) AS listing_contact_names_txt,
    lg.organization_id AS managing_organization_id,
    lg.organization_nm AS managing_organization_nm,
    lgs.economic_region_dsc,
    lgs.is_principal_residence_required,
    lgs.is_business_licence_required,
    drl.is_entire_unit,
    drl.available_bedrooms_qty,
    drl.nights_booked_qty AS nights_booked_ytd_qty,
    drl.separate_reservations_qty AS separate_reservations_ytd_qty,
    drl.business_licence_no,
    drl.bc_registry_no,
    demt.email_message_type_nm AS last_action_nm,
    dem.message_delivery_dtm AS last_action_dtm,
    dbl.business_licence_id,
    dbl.business_licence_no AS business_licence_no_matched,
    dbl.expiry_dt AS business_licence_expiry_dt,
    dbl.licence_status_type,
    drl.effective_business_licence_no,
    drl.effective_host_nm,
    drl.is_changed_business_licence,
    drl.lg_transfer_dtm,
    lgs.is_str_prohibited
   FROM ((((((((dss_rental_listing drl
     JOIN dss_organization org ON ((org.organization_id = drl.offering_organization_id)))
     LEFT JOIN dss_listing_status_type dlst ON (((drl.listing_status_type)::text = (dlst.listing_status_type)::text)))
     LEFT JOIN dss_physical_address dpa ON ((drl.locating_physical_address_id = dpa.physical_address_id)))
     LEFT JOIN dss_organization lgs ON (((lgs.organization_id = dpa.containing_organization_id) AND (dpa.match_score_amt > 1))))
     LEFT JOIN dss_organization lg ON ((lgs.managing_organization_id = lg.organization_id)))
     LEFT JOIN dss_email_message dem ON ((dem.email_message_id = ( SELECT msg.email_message_id
           FROM dss_email_message msg
          WHERE (msg.concerned_with_rental_listing_id = drl.rental_listing_id)
          ORDER BY msg.message_delivery_dtm DESC
         LIMIT 1))))
     LEFT JOIN dss_email_message_type demt ON (((dem.email_message_type)::text = (demt.email_message_type)::text)))
     LEFT JOIN dss_business_licence dbl ON ((drl.governing_business_licence_id = dbl.business_licence_id)))
  WHERE (drl.including_rental_listing_report_id IS NULL);