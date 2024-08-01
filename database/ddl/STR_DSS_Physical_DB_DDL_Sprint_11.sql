/* Create all Sprint 11 DB Objects STR DSS */

CREATE  TABLE dss_access_request_status ( 
	access_request_status_cd varchar(25)  NOT NULL  ,
	access_request_status_nm varchar(250)  NOT NULL  ,
	CONSTRAINT dss_access_request_status_pk PRIMARY KEY ( access_request_status_cd )
 );

CREATE  TABLE dss_email_message_type ( 
	email_message_type   varchar(50)  NOT NULL  ,
	email_message_type_nm varchar(250)  NOT NULL  ,
	CONSTRAINT dss_email_message_type_pk PRIMARY KEY ( email_message_type )
 );

CREATE  TABLE dss_listing_status_type ( 
	listing_status_type  varchar(2)  NOT NULL  ,
	listing_status_type_nm varchar(50)  NOT NULL  ,
	listing_status_sort_no smallint  NOT NULL  ,
	CONSTRAINT dss_listing_status_type_pk PRIMARY KEY ( listing_status_type )
 );

CREATE  TABLE dss_organization_type ( 
	organization_type    varchar(25)  NOT NULL  ,
	organization_type_nm varchar(250)  NOT NULL  ,
	CONSTRAINT dss_organization_type_pk PRIMARY KEY ( organization_type )
 );

CREATE  TABLE dss_user_privilege ( 
	user_privilege_cd    varchar(25)  NOT NULL  ,
	user_privilege_nm    varchar(250)  NOT NULL  ,
	CONSTRAINT dss_user_privilege_pk PRIMARY KEY ( user_privilege_cd )
 );

CREATE  TABLE dss_user_role ( 
	user_role_cd         varchar(25)  NOT NULL  ,
	user_role_nm         varchar(250)  NOT NULL  ,
	user_role_dsc        varchar(200)    ,
	upd_dtm              timestamptz    ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_user_role_pk PRIMARY KEY ( user_role_cd )
 );

CREATE  TABLE dss_user_role_privilege ( 
	user_privilege_cd    varchar(25)  NOT NULL  ,
	user_role_cd         varchar(25)  NOT NULL  ,
	upd_dtm              timestamptz    ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_user_role_privilege_pk PRIMARY KEY ( user_privilege_cd, user_role_cd )
 );

CREATE  TABLE dss_organization ( 
	organization_id      bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	organization_type    varchar(25)  NOT NULL  ,
	organization_cd      varchar(25)  NOT NULL  ,
	organization_nm      varchar(250)  NOT NULL  ,
	local_government_type varchar(50)    ,
	economic_region_dsc  varchar(100)    ,
	is_lg_participating  boolean    ,
	is_principal_residence_required boolean    ,
	is_business_licence_required boolean    ,
	area_geometry        geometry    ,
	managing_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_organization_pk PRIMARY KEY ( organization_id ),
	CONSTRAINT dss_organization_uk UNIQUE ( organization_cd ) 
 );

CREATE INDEX dss_organization_i1 ON dss_organization  ( organization_type );

CREATE INDEX dss_organization_i2 ON dss_organization  ( managing_organization_id );

CREATE  TABLE dss_organization_contact_person ( 
	organization_contact_person_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	email_address_dsc    varchar(320)  NOT NULL  ,
	is_primary           boolean    ,
	given_nm             varchar(25)    ,
	family_nm            varchar(25)    ,
	phone_no             varchar(30)    ,
	contacted_through_organization_id bigint  NOT NULL  ,
	email_message_type   varchar(50)    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_organization_contact_person_pk PRIMARY KEY ( organization_contact_person_id )
 );

CREATE  TABLE dss_physical_address ( 
	physical_address_id  bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	original_address_txt varchar(250)  NOT NULL  ,
	match_result_json    json    ,
	match_address_txt    varchar(250)    ,
	match_score_amt      smallint    ,
	unit_no              varchar(50)    ,
	civic_no             varchar(50)    ,
	street_nm            varchar(100)    ,
	street_type_dsc      varchar(50)    ,
	street_direction_dsc varchar(50)    ,
	locality_nm          varchar(100)    ,
	locality_type_dsc    varchar(50)    ,
	province_cd          varchar(5)    ,
	site_no              varchar(50)    ,
	block_no             varchar(50)    ,
	location_geometry    geometry    ,
	is_exempt            boolean    ,
	is_match_verified    boolean    ,
	is_changed_original_address boolean    ,
	is_match_corrected   boolean    ,
	is_system_processing boolean    ,
	containing_organization_id bigint    ,
	replacing_physical_address_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_physical_address_pk PRIMARY KEY ( physical_address_id )
 );

CREATE INDEX dss_physical_address_i1 ON dss_physical_address  ( original_address_txt );

CREATE INDEX dss_physical_address_i2 ON dss_physical_address  ( match_address_txt );

CREATE INDEX dss_physical_address_i3 ON dss_physical_address  ( containing_organization_id );

CREATE  TABLE dss_rental_listing_extract ( 
	rental_listing_extract_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	rental_listing_extract_nm varchar(250)  NOT NULL  ,
	is_pr_requirement_filtered boolean  NOT NULL  ,
	source_bin           bytea    ,
	filtering_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_extract_pk PRIMARY KEY ( rental_listing_extract_id )
 );

CREATE  TABLE dss_rental_listing_report ( 
	rental_listing_report_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	report_period_ym     date  NOT NULL  ,
	is_current           boolean  NOT NULL  ,
	providing_organization_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_report_pk PRIMARY KEY ( rental_listing_report_id ),
	CONSTRAINT dss_rental_listing_report_uk UNIQUE ( providing_organization_id, report_period_ym ) 
 );

CREATE  TABLE dss_upload_delivery ( 
	upload_delivery_id   bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	upload_delivery_type varchar(25)  NOT NULL  ,
	report_period_ym     date    ,
	source_hash_dsc      varchar(256)  NOT NULL  ,
	source_bin           bytea    ,
	source_header_txt    varchar(32000)    ,
	providing_organization_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_upload_delivery_pk PRIMARY KEY ( upload_delivery_id )
 );

CREATE  TABLE dss_upload_line ( 
	upload_line_id       bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	is_validation_failure boolean  NOT NULL  ,
	is_system_failure    boolean  NOT NULL  ,
	is_processed         boolean  NOT NULL  ,
	source_organization_cd varchar(25)  NOT NULL  ,
	source_record_no     varchar(50)  NOT NULL  ,
	source_line_txt      varchar(32000)  NOT NULL  ,
	error_txt            varchar(32000)    ,
	including_upload_delivery_id bigint  NOT NULL  ,
	CONSTRAINT dss_upload_line_pk PRIMARY KEY ( upload_line_id ),
	CONSTRAINT dss_upload_line_uk UNIQUE ( including_upload_delivery_id, source_organization_cd, source_record_no ) 
 );

CREATE INDEX dss_upload_line_i1 ON dss_upload_line  ( is_processed, including_upload_delivery_id );

CREATE  TABLE dss_user_identity ( 
	user_identity_id     bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	user_guid            uuid  NOT NULL  ,
	display_nm           varchar(250)  NOT NULL  ,
	identity_provider_nm varchar(25)  NOT NULL  ,
	is_enabled           boolean  NOT NULL  ,
	access_request_status_cd varchar(25)  NOT NULL  ,
	access_request_dtm   timestamptz    ,
	access_request_justification_txt varchar(250)    ,
	given_nm             varchar(25)    ,
	family_nm            varchar(25)    ,
	email_address_dsc    varchar(320)    ,
	business_nm          varchar(250)    ,
	terms_acceptance_dtm timestamptz    ,
	represented_by_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_user_identity_pk PRIMARY KEY ( user_identity_id )
 );

CREATE  TABLE dss_user_role_assignment ( 
	user_identity_id     bigint  NOT NULL  ,
	user_role_cd         varchar(25)  NOT NULL  ,
	upd_dtm              timestamptz    ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_user_role_assignment_pk PRIMARY KEY ( user_identity_id, user_role_cd )
 );

CREATE  TABLE dss_rental_listing ( 
	rental_listing_id    bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	platform_listing_no  varchar(50)  NOT NULL  ,
	platform_listing_url varchar(4000)    ,
	business_licence_no  varchar(230)    ,
	bc_registry_no       varchar(50)    ,
	is_current           boolean  NOT NULL  ,
	is_active            boolean    ,
	is_new               boolean    ,
	is_taken_down        boolean    ,
	is_changed_original_address boolean    ,
	is_changed_address   boolean    ,
	is_lg_transferred    boolean    ,
	is_entire_unit       boolean    ,
	available_bedrooms_qty smallint    ,
	nights_booked_qty    smallint    ,
	separate_reservations_qty smallint    ,
	offering_organization_id bigint  NOT NULL  ,
	including_rental_listing_report_id bigint    ,
	derived_from_rental_listing_id bigint    ,
	locating_physical_address_id bigint    ,
	listing_status_type  varchar(2)    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_pk PRIMARY KEY ( rental_listing_id )
 );

CREATE INDEX dss_rental_listing_i1 ON dss_rental_listing  ( offering_organization_id, platform_listing_no );

CREATE INDEX dss_rental_listing_i2 ON dss_rental_listing  ( including_rental_listing_report_id );

CREATE INDEX dss_rental_listing_i3 ON dss_rental_listing  ( derived_from_rental_listing_id );

CREATE INDEX dss_rental_listing_i4 ON dss_rental_listing  ( locating_physical_address_id );

CREATE INDEX dss_rental_listing_i5 ON dss_rental_listing  ( listing_status_type, offering_organization_id );

CREATE  TABLE dss_rental_listing_contact ( 
	rental_listing_contact_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	is_property_owner    boolean  NOT NULL  ,
	listing_contact_nbr  smallint    ,
	supplier_host_no     varchar(50)    ,
	full_nm              varchar(230)    ,
	phone_no             varchar(30)    ,
	fax_no               varchar(30)    ,
	full_address_txt     varchar(250)    ,
	email_address_dsc    varchar(320)    ,
	contacted_through_rental_listing_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_contact_pk PRIMARY KEY ( rental_listing_contact_id )
 );

CREATE INDEX dss_rental_listing_contact_i1 ON dss_rental_listing_contact  ( contacted_through_rental_listing_id );

CREATE  TABLE dss_email_message ( 
	email_message_id     bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	email_message_type   varchar(50)  NOT NULL  ,
	message_delivery_dtm timestamptz  NOT NULL  ,
	message_template_dsc varchar(4000)  NOT NULL  ,
	is_submitter_cc_required boolean  NOT NULL  ,
	is_host_contacted_externally boolean    ,
	is_with_standard_detail boolean    ,
	lg_phone_no          varchar(30)    ,
	unreported_listing_no varchar(50)    ,
	host_email_address_dsc varchar(320)    ,
	lg_email_address_dsc varchar(320)    ,
	cc_email_address_dsc varchar(4000)    ,
	unreported_listing_url varchar(4000)    ,
	lg_str_bylaw_url     varchar(4000)    ,
	custom_detail_txt    varchar(4000)    ,
	concerned_with_rental_listing_id bigint    ,
	initiating_user_identity_id bigint    ,
	affected_by_user_identity_id bigint    ,
	involved_in_organization_id bigint    ,
	batching_email_message_id bigint    ,
	requesting_organization_id bigint    ,
	external_message_no  varchar(50)    ,
	upd_dtm              timestamptz    ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_email_message_pk PRIMARY KEY ( email_message_id )
 );

CREATE INDEX dss_email_message_i1 ON dss_email_message  ( concerned_with_rental_listing_id, message_delivery_dtm );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_initiated_by FOREIGN KEY ( initiating_user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_affecting FOREIGN KEY ( affected_by_user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_involving FOREIGN KEY ( involved_in_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_communicating FOREIGN KEY ( email_message_type ) REFERENCES dss_email_message_type( email_message_type );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_batched_in FOREIGN KEY ( batching_email_message_id ) REFERENCES dss_email_message( email_message_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_requested_by FOREIGN KEY ( requesting_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_included_in FOREIGN KEY ( concerned_with_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id );

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_fk_managed_by FOREIGN KEY ( managing_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_fk_treated_as FOREIGN KEY ( organization_type ) REFERENCES dss_organization_type( organization_type );

ALTER TABLE dss_organization_contact_person ADD CONSTRAINT dss_organization_contact_person_fk_contacted_for FOREIGN KEY ( contacted_through_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_organization_contact_person ADD CONSTRAINT dss_organization_contact_person_fk_subscribed_to FOREIGN KEY ( email_message_type ) REFERENCES dss_email_message_type( email_message_type );

ALTER TABLE dss_physical_address ADD CONSTRAINT dss_physical_address_fk_contained_in FOREIGN KEY ( containing_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_physical_address ADD CONSTRAINT dss_physical_address_fk_replaced_by FOREIGN KEY ( replacing_physical_address_id ) REFERENCES dss_physical_address( physical_address_id );

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_offered_by FOREIGN KEY ( offering_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_included_in FOREIGN KEY ( including_rental_listing_report_id ) REFERENCES dss_rental_listing_report( rental_listing_report_id );

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_located_at FOREIGN KEY ( locating_physical_address_id ) REFERENCES dss_physical_address( physical_address_id );

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_generating FOREIGN KEY ( derived_from_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id );

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_classified_as FOREIGN KEY ( listing_status_type ) REFERENCES dss_listing_status_type( listing_status_type );

ALTER TABLE dss_rental_listing_contact ADD CONSTRAINT dss_rental_listing_contact_fk_contacted_for FOREIGN KEY ( contacted_through_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id );

ALTER TABLE dss_rental_listing_extract ADD CONSTRAINT dss_rental_listing_extract_fk_filtered_by FOREIGN KEY ( filtering_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_rental_listing_report ADD CONSTRAINT dss_rental_listing_report_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_upload_delivery ADD CONSTRAINT dss_upload_delivery_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_fk_included_in FOREIGN KEY ( including_upload_delivery_id ) REFERENCES dss_upload_delivery( upload_delivery_id );

ALTER TABLE dss_user_identity ADD CONSTRAINT dss_user_identity_fk_representing FOREIGN KEY ( represented_by_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_user_identity ADD CONSTRAINT dss_user_identity_fk_given FOREIGN KEY ( access_request_status_cd ) REFERENCES dss_access_request_status( access_request_status_cd );

ALTER TABLE dss_user_role_assignment ADD CONSTRAINT dss_user_role_assignment_fk_granted FOREIGN KEY ( user_role_cd ) REFERENCES dss_user_role( user_role_cd );

ALTER TABLE dss_user_role_assignment ADD CONSTRAINT dss_user_role_assignment_fk_granted_to FOREIGN KEY ( user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_user_role_privilege ADD CONSTRAINT dss_user_role_privilege_fk_conferred_by FOREIGN KEY ( user_role_cd ) REFERENCES dss_user_role( user_role_cd );

ALTER TABLE dss_user_role_privilege ADD CONSTRAINT dss_user_role_privilege_fk_conferring FOREIGN KEY ( user_privilege_cd ) REFERENCES dss_user_privilege( user_privilege_cd );

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
    dem.message_delivery_dtm AS last_action_dtm
   FROM (((((((dss_rental_listing drl
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
  WHERE (drl.including_rental_listing_report_id IS NULL);

CREATE OR REPLACE VIEW dss_rental_upload_history_view AS SELECT dud.upload_delivery_id,
    dud.report_period_ym,
    dud.providing_organization_id,
    do2.organization_nm,
    dud.upd_dtm,
    dui.given_nm,
    dui.family_nm,
    count(*) AS total,
    sum(
        CASE
            WHEN (dul.is_processed = true) THEN 1
            ELSE 0
        END) AS processed,
    sum(
        CASE
            WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
            ELSE 0
        END) AS errors,
    sum(
        CASE
            WHEN ((dul.is_validation_failure = false) AND (dul.is_system_failure = false)) THEN 1
            ELSE 0
        END) AS success,
        CASE
            WHEN (count(*) = sum(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) THEN 'Processed'::text
            ELSE 'Pending'::text
        END AS status
   FROM (((dss_upload_delivery dud
     JOIN dss_upload_line dul ON ((dul.including_upload_delivery_id = dud.upload_delivery_id)))
     JOIN dss_user_identity dui ON ((dud.upd_user_guid = dui.user_guid)))
     JOIN dss_organization do2 ON ((dud.providing_organization_id = do2.organization_id)))
  GROUP BY dud.upload_delivery_id, dud.report_period_ym, dud.providing_organization_id, do2.organization_nm, dud.upd_dtm, dui.given_nm, dui.family_nm;

CREATE OR REPLACE VIEW dss_user_identity_view AS SELECT u.user_identity_id,
    u.is_enabled,
    u.access_request_status_cd,
    u.access_request_dtm,
    u.access_request_justification_txt,
    u.identity_provider_nm,
    u.given_nm,
    u.family_nm,
    u.email_address_dsc,
    u.business_nm,
    u.terms_acceptance_dtm,
    u.represented_by_organization_id,
    o.organization_type,
    o.organization_cd,
    o.organization_nm,
    u.upd_dtm
   FROM (dss_user_identity u
     LEFT JOIN dss_organization o ON ((u.represented_by_organization_id = o.organization_id)));

CREATE OR REPLACE FUNCTION dss_update_audit_columns()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$BEGIN
    NEW.upd_dtm := current_timestamp;
    RETURN NEW;
END;$function$;

CREATE TRIGGER dss_email_message_br_iu_tr BEFORE INSERT OR UPDATE ON dss_email_message FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_organization_br_iu_tr BEFORE INSERT OR UPDATE ON dss_organization FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_organization_contact_person_br_iu_tr BEFORE INSERT OR UPDATE ON dss_organization_contact_person FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_physical_address_br_iu_tr BEFORE INSERT OR UPDATE ON dss_physical_address FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_rental_listing_br_iu_tr BEFORE INSERT OR UPDATE ON dss_rental_listing FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_rental_listing_contact_br_iu_tr BEFORE INSERT OR UPDATE ON dss_rental_listing_contact FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_rental_listing_extract_br_iu_tr BEFORE INSERT OR UPDATE ON dss_rental_listing_extract FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_rental_listing_report_br_iu_tr BEFORE INSERT OR UPDATE ON dss_rental_listing_report FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_upload_delivery_br_iu_tr BEFORE INSERT OR UPDATE ON dss_upload_delivery FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_user_identity_br_iu_tr BEFORE INSERT OR UPDATE ON dss_user_identity FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_user_role_assignment_br_iu_tr BEFORE INSERT OR UPDATE ON dss_user_role_assignment FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_user_role_br_iu_tr BEFORE INSERT OR UPDATE ON dss_user_role FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_user_role_privilege_br_iu_tr BEFORE INSERT OR UPDATE ON dss_user_role_privilege FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE OR REPLACE FUNCTION dss_containing_organization_id(p_point geometry)
 RETURNS bigint
 LANGUAGE sql
 IMMUTABLE STRICT
RETURN (SELECT do1.organization_id FROM dss_organization do1 WHERE (st_intersects(dss_containing_organization_id.p_point, do1.area_geometry) AND (NOT (EXISTS (SELECT do2.organization_nm FROM dss_organization do2 WHERE (st_intersects(dss_containing_organization_id.p_point, do2.area_geometry) AND (do2.organization_id <> do1.organization_id) AND (st_area(do2.area_geometry) < st_area(do1.area_geometry))))))))
;

COMMENT ON TABLE dss_access_request_status IS 'A potential status for a user access request (e.g. Requested, Approved, or Denied)';

COMMENT ON COLUMN dss_access_request_status.access_request_status_cd IS 'System-consistent code for the request status';

COMMENT ON COLUMN dss_access_request_status.access_request_status_nm IS 'Business term for the request status';

COMMENT ON TABLE dss_email_message_type IS 'The type or purpose of a system generated message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON COLUMN dss_email_message_type.email_message_type IS 'System-consistent code for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON COLUMN dss_email_message_type.email_message_type_nm IS 'Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON TABLE dss_listing_status_type IS 'A potential status for a CURRENT RENTAL LISTING (e.g. New, Active, Inactive, Reassigned, Taken Down)';

COMMENT ON COLUMN dss_listing_status_type.listing_status_type IS 'System-consistent code for the listing status (e.g. N, A, I, R, T)';

COMMENT ON COLUMN dss_listing_status_type.listing_status_type_nm IS 'Business term for the listing status (e.g. New, Active, Inactive, Reassigned, Taken Down)';

COMMENT ON TABLE dss_organization_type IS 'A level of government or business category';

COMMENT ON COLUMN dss_organization_type.organization_type IS 'System-consistent code for a level of government or business category';

COMMENT ON COLUMN dss_organization_type.organization_type_nm IS 'Business term for a level of government or business category';

COMMENT ON TABLE dss_user_privilege IS 'A granular access right or privilege within the application that may be granted to a role';

COMMENT ON COLUMN dss_user_privilege.user_privilege_cd IS 'The immutable system code that identifies the privilege';

COMMENT ON COLUMN dss_user_privilege.user_privilege_nm IS 'The human-readable name that is given for the role';

COMMENT ON TABLE dss_user_role IS 'A set of access rights and privileges within the application that may be granted to users';

COMMENT ON COLUMN dss_user_role.user_role_cd IS 'The immutable system code that identifies the role';

COMMENT ON COLUMN dss_user_role.user_role_nm IS 'The human-readable name that is given for the role';

COMMENT ON COLUMN dss_user_role.user_role_dsc IS 'The human-readable description that is given for the role';

COMMENT ON COLUMN dss_user_role.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_role.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_user_role_privilege IS 'The association of a granular application privilege to a role';

COMMENT ON COLUMN dss_user_role_privilege.user_privilege_cd IS 'Foreign key';

COMMENT ON COLUMN dss_user_role_privilege.user_role_cd IS 'Foreign key';

COMMENT ON COLUMN dss_user_role_privilege.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_role_privilege.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_organization IS 'A private company or governing body component that plays a role in short term rental reporting or enforcement';

COMMENT ON COLUMN dss_organization.organization_id IS 'Unique generated key';

COMMENT ON COLUMN dss_organization.organization_type IS 'Foreign key';

COMMENT ON COLUMN dss_organization.organization_cd IS 'An immutable system code that identifies the organization (e.g. CEU, AIRBNB)';

COMMENT ON COLUMN dss_organization.organization_nm IS 'A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)';

COMMENT ON COLUMN dss_organization.local_government_type IS 'A sub-type of local government organization used for sorting and grouping or members';

COMMENT ON COLUMN dss_organization.economic_region_dsc IS 'A free form description of the economic region to which a Local Government Subdivision belongs';

COMMENT ON COLUMN dss_organization.is_lg_participating IS 'Indicates whether a LOCAL GOVERNMENT ORGANIZATION participates in Short Term Rental Data Sharing';

COMMENT ON COLUMN dss_organization.is_principal_residence_required IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION is subject to Provincial Principal Residence Short Term Rental restrictions';

COMMENT ON COLUMN dss_organization.is_business_licence_required IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION requires a business licence for Short Term Rental operation';

COMMENT ON COLUMN dss_organization.area_geometry IS 'the multipolygon shape identifying the boundaries of a local government subdivision';

COMMENT ON COLUMN dss_organization.managing_organization_id IS 'Self-referential hierarchical foreign key';

COMMENT ON COLUMN dss_organization.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_organization.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_organization_contact_person IS 'A person who has been identified as a notable contact for a particular organization';

COMMENT ON COLUMN dss_organization_contact_person.organization_contact_person_id IS 'Unique generated key';

COMMENT ON COLUMN dss_organization_contact_person.email_address_dsc IS 'E-mail address given for the contact by the organization';

COMMENT ON COLUMN dss_organization_contact_person.is_primary IS 'Indicates whether the contact should receive all communications directed at the organization';

COMMENT ON COLUMN dss_organization_contact_person.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';

COMMENT ON COLUMN dss_organization_contact_person.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';

COMMENT ON COLUMN dss_organization_contact_person.phone_no IS 'Phone number given for the contact by the organization (contains only digits)';

COMMENT ON COLUMN dss_organization_contact_person.contacted_through_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_organization_contact_person.email_message_type IS 'Foreign key';

COMMENT ON COLUMN dss_organization_contact_person.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_organization_contact_person.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_physical_address IS 'A property address that includes any verifiable BC attributes';

COMMENT ON COLUMN dss_physical_address.physical_address_id IS 'Unique generated key';

COMMENT ON COLUMN dss_physical_address.original_address_txt IS 'The source-provided address of a short-term rental offering';

COMMENT ON COLUMN dss_physical_address.match_result_json IS 'Full JSON result of the source address matching attempt';

COMMENT ON COLUMN dss_physical_address.match_address_txt IS 'The sanitized physical address (returned as fullAddress) that has been derived from the original';

COMMENT ON COLUMN dss_physical_address.match_score_amt IS 'The relative score returned from the address matching attempt';

COMMENT ON COLUMN dss_physical_address.unit_no IS 'The unitNumber (suite) returned by the address match (e.g. 100)';

COMMENT ON COLUMN dss_physical_address.civic_no IS 'The civicNumber (building number) returned by the address match (e.g. 1285)';

COMMENT ON COLUMN dss_physical_address.street_nm IS 'The streetName returned by the address match (e.g. Pender)';

COMMENT ON COLUMN dss_physical_address.street_type_dsc IS 'The streetType returned by the address match (e.g. St or Street)';

COMMENT ON COLUMN dss_physical_address.street_direction_dsc IS 'The streetDirection returned by the address match (e.g. W or West)';

COMMENT ON COLUMN dss_physical_address.locality_nm IS 'The localityName (community) returned by the address match (e.g. Vancouver)';

COMMENT ON COLUMN dss_physical_address.locality_type_dsc IS 'The localityType returned by the address match (e.g. City)';

COMMENT ON COLUMN dss_physical_address.province_cd IS 'The provinceCode returned by the address match';

COMMENT ON COLUMN dss_physical_address.site_no IS 'The siteID returned by the address match';

COMMENT ON COLUMN dss_physical_address.block_no IS 'The blockID returned by the address match';

COMMENT ON COLUMN dss_physical_address.location_geometry IS 'The computed location point of the matched address';

COMMENT ON COLUMN dss_physical_address.is_exempt IS 'Indicates whether the address has been identified as exempt from Short Term Rental regulations';

COMMENT ON COLUMN dss_physical_address.is_match_verified IS 'Indicates whether the matched address has been verified as correct for the listing by the responsible authorities';

COMMENT ON COLUMN dss_physical_address.is_changed_original_address IS 'Indicates whether the original address has received a different property address from the platform in the last reporting period';

COMMENT ON COLUMN dss_physical_address.is_match_corrected IS 'Indicates whether the matched address has been manually changed to one that is verified as correct for the listing';

COMMENT ON COLUMN dss_physical_address.is_system_processing IS 'Indicates whether the physical address is being processed by the system and may not yet be in its final form';

COMMENT ON COLUMN dss_physical_address.containing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_physical_address.replacing_physical_address_id IS 'Foreign key';

COMMENT ON COLUMN dss_physical_address.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_physical_address.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing_extract IS 'A prebuilt report that is specific to a subset of rental listings';

COMMENT ON COLUMN dss_rental_listing_extract.rental_listing_extract_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing_extract.rental_listing_extract_nm IS 'A description of the information contained in the extract';

COMMENT ON COLUMN dss_rental_listing_extract.is_pr_requirement_filtered IS 'Indicates whether the report is filtered by jurisdictional principal residence requirement';

COMMENT ON COLUMN dss_rental_listing_extract.source_bin IS 'The binary image of the information in the report';

COMMENT ON COLUMN dss_rental_listing_extract.filtering_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing_extract.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing_extract.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing_report IS 'A platform-specific collection of rental listing information that is relevant to a specific month';

COMMENT ON COLUMN dss_rental_listing_report.rental_listing_report_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing_report.report_period_ym IS 'The month to which the listing information is relevant (always set to the first day of the month)';

COMMENT ON COLUMN dss_rental_listing_report.is_current IS 'Indicates whether the rental listing version is the most recent one reported by the platform';

COMMENT ON COLUMN dss_rental_listing_report.providing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing_report.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing_report.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_upload_delivery IS 'A delivery of uploaded information that is relevant to a specific month';

COMMENT ON COLUMN dss_upload_delivery.upload_delivery_id IS 'Unique generated key';

COMMENT ON COLUMN dss_upload_delivery.upload_delivery_type IS 'Identifies the treatment applied to ingesting the uploaded information';

COMMENT ON COLUMN dss_upload_delivery.report_period_ym IS 'The month to which the delivery batch is relevant (always set to the first day of the month)';

COMMENT ON COLUMN dss_upload_delivery.source_hash_dsc IS 'The hash value of the information that was uploaded';

COMMENT ON COLUMN dss_upload_delivery.source_bin IS 'The binary image of the information that was uploaded';

COMMENT ON COLUMN dss_upload_delivery.source_header_txt IS 'Full text of the header line';

COMMENT ON COLUMN dss_upload_delivery.providing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_upload_delivery.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_upload_delivery.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_upload_line IS 'An upload delivery line that has been extracted from the source';

COMMENT ON COLUMN dss_upload_line.upload_line_id IS 'Unique generated key';

COMMENT ON COLUMN dss_upload_line.is_validation_failure IS 'Indicates that there has been a validation problem that prevents successful ingestion of the upload line';

COMMENT ON COLUMN dss_upload_line.is_system_failure IS 'Indicates that a system fault has prevented complete ingestion of the upload line';

COMMENT ON COLUMN dss_upload_line.is_processed IS 'Indicates that no further ingestion attempt is required for the upload line';

COMMENT ON COLUMN dss_upload_line.source_organization_cd IS 'An immutable system code identifying the organization who created the information in the upload line (e.g. AIRBNB)';

COMMENT ON COLUMN dss_upload_line.source_record_no IS 'The immutable identification number for the source record, such as a rental listing number';

COMMENT ON COLUMN dss_upload_line.source_line_txt IS 'Full text of the upload line';

COMMENT ON COLUMN dss_upload_line.error_txt IS 'Freeform description of the problem found while attempting to interpret the report line';

COMMENT ON COLUMN dss_upload_line.including_upload_delivery_id IS 'Foreign key';

COMMENT ON TABLE dss_user_identity IS 'An externally defined domain directory object representing a potential application user or group';

COMMENT ON COLUMN dss_user_identity.user_identity_id IS 'Unique generated key';

COMMENT ON COLUMN dss_user_identity.user_guid IS 'An immutable unique identifier assigned by the identity provider';

COMMENT ON COLUMN dss_user_identity.display_nm IS 'A human-readable full name that is assigned by the identity provider (this may include a preferred name and/or business unit name)';

COMMENT ON COLUMN dss_user_identity.identity_provider_nm IS 'A directory or domain that authenticates system users to allow access to the application or its API  (e.g. idir, bceidbusiness)';

COMMENT ON COLUMN dss_user_identity.is_enabled IS 'Indicates whether access is currently permitted using this identity';

COMMENT ON COLUMN dss_user_identity.access_request_status_cd IS 'The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied)';

COMMENT ON COLUMN dss_user_identity.access_request_dtm IS 'A timestamp indicating when the most recent access request was made by the user';

COMMENT ON COLUMN dss_user_identity.access_request_justification_txt IS 'The most recent user-provided reason for requesting application access';

COMMENT ON COLUMN dss_user_identity.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';

COMMENT ON COLUMN dss_user_identity.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';

COMMENT ON COLUMN dss_user_identity.email_address_dsc IS 'E-mail address associated with the user by the identity provider';

COMMENT ON COLUMN dss_user_identity.business_nm IS 'A human-readable organization name that is associated with the user by the identity provider';

COMMENT ON COLUMN dss_user_identity.terms_acceptance_dtm IS 'A timestamp indicating when the user most recently accepted the published Terms and Conditions of application access';

COMMENT ON COLUMN dss_user_identity.represented_by_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_user_identity.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_identity.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_user_role_assignment IS 'The association of a grantee credential to a role for the purpose of conveying application privileges';

COMMENT ON COLUMN dss_user_role_assignment.user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_user_role_assignment.user_role_cd IS 'Foreign key';

COMMENT ON COLUMN dss_user_role_assignment.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_role_assignment.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing IS 'A rental listing snapshot that is either relevant to a specific monthly report, or is the current, master version';

COMMENT ON COLUMN dss_rental_listing.rental_listing_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing.platform_listing_no IS 'The platform issued identification number for the listing';

COMMENT ON COLUMN dss_rental_listing.platform_listing_url IS 'URL for the short-term rental platform listing';

COMMENT ON COLUMN dss_rental_listing.business_licence_no IS 'The local government issued licence number that applies to the rental offering';

COMMENT ON COLUMN dss_rental_listing.bc_registry_no IS 'The Short Term Registry issued permit number';

COMMENT ON COLUMN dss_rental_listing.is_current IS 'Indicates whether the RENTAL LISTING VERSION is a CURRENT RENTAL LISTING (if it is a copy of the most current REPORTED RENTAL LISTING (having the same listing number for the same offering platform)';

COMMENT ON COLUMN dss_rental_listing.is_active IS 'Indicates whether a CURRENT RENTAL LISTING was included in the most recent RENTAL LISTING REPORT';

COMMENT ON COLUMN dss_rental_listing.is_new IS 'Indicates whether a CURRENT RENTAL LISTING appeared for the first time in the last reporting period';

COMMENT ON COLUMN dss_rental_listing.is_taken_down IS 'Indicates whether a CURRENT RENTAL LISTING has been reported as taken down by the offering platform';

COMMENT ON COLUMN dss_rental_listing.is_changed_original_address IS 'Indicates whether a CURRENT RENTAL LISTING has received a different property address in the last reporting period';

COMMENT ON COLUMN dss_rental_listing.is_changed_address IS 'Indicates whether a CURRENT RENTAL LISTING has been subjected to address match changes by a user';

COMMENT ON COLUMN dss_rental_listing.is_lg_transferred IS 'Indicates whether a CURRENT RENTAL LISTING has been transferred to a different Local Goverment Organization as a result of address changes';

COMMENT ON COLUMN dss_rental_listing.is_entire_unit IS 'Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom)';

COMMENT ON COLUMN dss_rental_listing.available_bedrooms_qty IS 'The number of bedrooms in the dwelling unit that are available for short term rental';

COMMENT ON COLUMN dss_rental_listing.nights_booked_qty IS 'The number of nights that short term rental accommodation services were provided during the reporting period';

COMMENT ON COLUMN dss_rental_listing.separate_reservations_qty IS 'The number of separate reservations that were taken during the reporting period';

COMMENT ON COLUMN dss_rental_listing.offering_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.including_rental_listing_report_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.derived_from_rental_listing_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.locating_physical_address_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.listing_status_type IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing_contact IS 'A person who has been identified as a notable contact for a particular rental listing';

COMMENT ON COLUMN dss_rental_listing_contact.rental_listing_contact_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing_contact.is_property_owner IS 'Indicates a person with the legal right to the unit being short-term rental';

COMMENT ON COLUMN dss_rental_listing_contact.listing_contact_nbr IS 'Indicates which of the five possible supplier hosts is represented by this contact';

COMMENT ON COLUMN dss_rental_listing_contact.supplier_host_no IS 'The platform identifier for the supplier host';

COMMENT ON COLUMN dss_rental_listing_contact.full_nm IS 'The full name of the contact person as inluded in the listing';

COMMENT ON COLUMN dss_rental_listing_contact.phone_no IS 'Phone number given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.fax_no IS 'Facsimile numbrer given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.full_address_txt IS 'Mailing address given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.email_address_dsc IS 'E-mail address given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.contacted_through_rental_listing_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing_contact.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing_contact.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_email_message IS 'A message that is sent to one or more recipients via email';

COMMENT ON COLUMN dss_email_message.email_message_id IS 'Unique generated key';

COMMENT ON COLUMN dss_email_message.email_message_type IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.message_delivery_dtm IS 'A timestamp indicating when the message delivery was initiated';

COMMENT ON COLUMN dss_email_message.message_template_dsc IS 'The full text or template for the message that is sent';

COMMENT ON COLUMN dss_email_message.is_submitter_cc_required IS 'Indicates whether the user initiating the message should receive a copy of the email';

COMMENT ON COLUMN dss_email_message.is_host_contacted_externally IS 'Indicates whether the the property host has already been contacted by external means';

COMMENT ON COLUMN dss_email_message.is_with_standard_detail IS 'Indicates whether message body should include text a block of detail text that is standard for the message type';

COMMENT ON COLUMN dss_email_message.lg_phone_no IS 'A phone number associated with a Local Government contact';

COMMENT ON COLUMN dss_email_message.unreported_listing_no IS 'The platform issued identification number for the listing (if not included in a rental listing report)';

COMMENT ON COLUMN dss_email_message.host_email_address_dsc IS 'E-mail address of a short term rental host (directly entered by the user as a message recipient)';

COMMENT ON COLUMN dss_email_message.lg_email_address_dsc IS 'E-mail address of a local government contact (directly entered by the user as a message recipient)';

COMMENT ON COLUMN dss_email_message.cc_email_address_dsc IS 'E-mail address of a secondary message recipient (directly entered by the user)';

COMMENT ON COLUMN dss_email_message.unreported_listing_url IS 'User-provided URL for a short-term rental platform listing that is the subject of the message';

COMMENT ON COLUMN dss_email_message.lg_str_bylaw_url IS 'User-provided URL for a local government bylaw that is the subject of the message';

COMMENT ON COLUMN dss_email_message.custom_detail_txt IS 'Free form text that should be included in the message body';

COMMENT ON COLUMN dss_email_message.concerned_with_rental_listing_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.initiating_user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.affected_by_user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.involved_in_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.batching_email_message_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.requesting_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.external_message_no IS 'External identifier for tracking the message delivery progress';

COMMENT ON COLUMN dss_email_message.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_email_message.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

