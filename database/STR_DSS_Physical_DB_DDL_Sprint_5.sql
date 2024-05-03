/* Create all Sprint 5 DB Objects STR DSS */

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

CREATE  TABLE dss_message_reason ( 
	message_reason_id    bigint  NOT NULL GENERATED  ALWAYS AS IDENTITY ,
	email_message_type   varchar(50)  NOT NULL  ,
	message_reason_dsc   varchar(250)  NOT NULL  ,
	CONSTRAINT dss_message_reason_pk PRIMARY KEY ( message_reason_id )
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
	CONSTRAINT dss_user_role_pk PRIMARY KEY ( user_role_cd )
 );

CREATE  TABLE dss_user_role_privilege ( 
	user_privilege_cd    varchar(25)  NOT NULL  ,
	user_role_cd         varchar(25)  NOT NULL  ,
	CONSTRAINT dss_user_role_privilege_pk PRIMARY KEY ( user_privilege_cd, user_role_cd )
 );

CREATE  TABLE dss_organization ( 
	organization_id      bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	organization_type    varchar(25)  NOT NULL  ,
	organization_cd      varchar(25)  NOT NULL  ,
	organization_nm      varchar(250)  NOT NULL  ,
	local_government_geometry geometry    ,
	managing_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_organization_pk PRIMARY KEY ( organization_id )
 );

CREATE  TABLE dss_organization_contact_person ( 
	organization_contact_person_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	is_primary           boolean    ,
	given_nm             varchar(25)    ,
	family_nm            varchar(25)    ,
	phone_no             varchar(30)    ,
	email_address_dsc    varchar(320)  NOT NULL  ,
	contacted_through_organization_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	email_message_type   varchar(50)    ,
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
	street_nm            varchar(50)    ,
	street_type_dsc      varchar(50)    ,
	street_direction_dsc varchar(50)    ,
	locality_nm          varchar(50)    ,
	locality_type_dsc    varchar(50)    ,
	province_cd          varchar(5)    ,
	site_no              varchar(50)    ,
	block_no             varchar(50)    ,
	location_geometry    geometry    ,
	is_exempt            boolean    ,
	containing_organization_id bigint    ,
	replacing_physical_address_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_physical_address_pk PRIMARY KEY ( physical_address_id )
 );

CREATE  TABLE dss_rental_listing_report ( 
	rental_listing_report_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	report_period_ym     date  NOT NULL  ,
	is_current           boolean  NOT NULL  ,
	providing_organization_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_report_pk PRIMARY KEY ( rental_listing_report_id )
 );

CREATE  TABLE dss_upload_delivery ( 
	upload_delivery_id   bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	upload_delivery_type varchar(25)  NOT NULL  ,
	report_period_ym     date    ,
	source_hash_dsc      varchar(256)  NOT NULL  ,
	source_bin           bytea    ,
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
	CONSTRAINT dss_upload_line_pk PRIMARY KEY ( upload_line_id )
 );

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
	CONSTRAINT dss_user_role_assignment_pk PRIMARY KEY ( user_identity_id, user_role_cd )
 );

CREATE  TABLE dss_email_message ( 
	email_message_id     bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	email_message_type   varchar(50)  NOT NULL  ,
	message_delivery_dtm timestamptz  NOT NULL  ,
	message_template_dsc varchar(4000)  NOT NULL  ,
	is_submitter_cc_required boolean  NOT NULL  ,
	is_host_contacted_externally boolean    ,
	lg_phone_no          varchar(30)    ,
	unreported_listing_no varchar(50)    ,
	host_email_address_dsc varchar(320)    ,
	lg_email_address_dsc varchar(320)    ,
	cc_email_address_dsc varchar(4000)    ,
	unreported_listing_url varchar(4000)    ,
	lg_str_bylaw_url     varchar(4000)    ,
	concerned_with_rental_listing_id bigint    ,
	initiating_user_identity_id bigint    ,
	affected_by_user_identity_id bigint    ,
	involved_in_organization_id bigint    ,
	batching_email_message_id bigint    ,
	requesting_organization_id bigint    ,
	message_reason_id    bigint    ,
	external_message_no  varchar(50)    ,
	upd_dtm              timestamptz    ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_email_message_pk PRIMARY KEY ( email_message_id )
 );

CREATE  TABLE dss_rental_listing ( 
	rental_listing_id    bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	platform_listing_no  varchar(50)  NOT NULL  ,
	platform_listing_url varchar(4000)    ,
	business_licence_no  varchar(50)    ,
	bc_registry_no       varchar(50)    ,
	is_current           boolean  NOT NULL  ,
	is_taken_down        boolean    ,
	is_entire_unit       boolean    ,
	available_bedrooms_qty smallint    ,
	nights_booked_qty    smallint    ,
	separate_reservations_qty smallint    ,
	offering_organization_id bigint  NOT NULL  ,
	including_rental_listing_report_id bigint    ,
	derived_from_rental_listing_id bigint    ,
	locating_physical_address_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_pk PRIMARY KEY ( rental_listing_id )
 );

CREATE  TABLE dss_rental_listing_contact ( 
	rental_listing_contact_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	is_property_owner    boolean  NOT NULL  ,
	listing_contact_nbr  smallint    ,
	supplier_host_no     varchar(50)    ,
	full_nm              varchar(50)    ,
	phone_no             varchar(30)    ,
	fax_no               varchar(30)    ,
	full_address_txt     varchar(250)    ,
	email_address_dsc    varchar(320)    ,
	contacted_through_rental_listing_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_contact_pk PRIMARY KEY ( rental_listing_contact_id )
 );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_initiated_by FOREIGN KEY ( initiating_user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_affecting FOREIGN KEY ( affected_by_user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_involving FOREIGN KEY ( involved_in_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_communicating FOREIGN KEY ( email_message_type ) REFERENCES dss_email_message_type( email_message_type );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_justified_by FOREIGN KEY ( message_reason_id ) REFERENCES dss_message_reason( message_reason_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_batched_in FOREIGN KEY ( batching_email_message_id ) REFERENCES dss_email_message( email_message_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_requested_by FOREIGN KEY ( requesting_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_included_in FOREIGN KEY ( concerned_with_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id );

ALTER TABLE dss_message_reason ADD CONSTRAINT dss_message_reason_fk_justifying FOREIGN KEY ( email_message_type ) REFERENCES dss_email_message_type( email_message_type );

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

ALTER TABLE dss_rental_listing_contact ADD CONSTRAINT dss_rental_listing_contact_fk_contacted_for FOREIGN KEY ( contacted_through_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id );

ALTER TABLE dss_rental_listing_report ADD CONSTRAINT dss_rental_listing_report_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_upload_delivery ADD CONSTRAINT dss_upload_delivery_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_fk_included_in FOREIGN KEY ( including_upload_delivery_id ) REFERENCES dss_upload_delivery( upload_delivery_id );

ALTER TABLE dss_user_identity ADD CONSTRAINT dss_user_identity_fk_representing FOREIGN KEY ( represented_by_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_user_identity ADD CONSTRAINT dss_user_identity_fk_given FOREIGN KEY ( access_request_status_cd ) REFERENCES dss_access_request_status( access_request_status_cd );

ALTER TABLE dss_user_role_assignment ADD CONSTRAINT dss_user_role_assignment_fk_granted FOREIGN KEY ( user_role_cd ) REFERENCES dss_user_role( user_role_cd );

ALTER TABLE dss_user_role_assignment ADD CONSTRAINT dss_user_role_assignment_fk_granted_to FOREIGN KEY ( user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_user_role_privilege ADD CONSTRAINT dss_user_role_privilege_fk_conferred_by FOREIGN KEY ( user_role_cd ) REFERENCES dss_user_role( user_role_cd );

ALTER TABLE dss_user_role_privilege ADD CONSTRAINT dss_user_role_privilege_fk_conferring FOREIGN KEY ( user_privilege_cd ) REFERENCES dss_user_privilege( user_privilege_cd );

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
   FROM (dss.dss_user_identity u
     LEFT JOIN dss.dss_organization o ON ((u.represented_by_organization_id = o.organization_id)));

CREATE OR REPLACE FUNCTION dss.dss_update_audit_columns()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$BEGIN
    NEW.upd_dtm := current_timestamp;
    RETURN NEW;
END;$function$;

CREATE TRIGGER dss_email_message_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_email_message FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_organization_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_organization FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_organization_contact_person_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_organization_contact_person FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_physical_address_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_physical_address FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_rental_listing_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_rental_listing FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_rental_listing_contact_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_rental_listing_contact FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_rental_listing_report_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_rental_listing_report FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_upload_delivery_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_upload_delivery FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

CREATE TRIGGER dss_user_identity_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_user_identity FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();

COMMENT ON TABLE dss_access_request_status IS 'A potential status for a user access request (e.g. Requested, Approved, or Denied)';

COMMENT ON COLUMN dss_access_request_status.access_request_status_cd IS 'System-consistent code for the request status';

COMMENT ON COLUMN dss_access_request_status.access_request_status_nm IS 'Business term for the request status';

COMMENT ON TABLE dss_email_message_type IS 'The type or purpose of a system generated message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON COLUMN dss_email_message_type.email_message_type IS 'System-consistent code for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON COLUMN dss_email_message_type.email_message_type_nm IS 'Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON TABLE dss_message_reason IS 'A description of the justification for initiating a message';

COMMENT ON COLUMN dss_message_reason.message_reason_id IS 'Unique generated key';

COMMENT ON COLUMN dss_message_reason.email_message_type IS 'Foreign key';

COMMENT ON COLUMN dss_message_reason.message_reason_dsc IS 'A description of the justification for initiating a message';

COMMENT ON TABLE dss_organization_type IS 'A level of government or business category';

COMMENT ON COLUMN dss_organization_type.organization_type IS 'System-consistent code for a level of government or business category';

COMMENT ON COLUMN dss_organization_type.organization_type_nm IS 'Business term for a level of government or business category';

COMMENT ON TABLE dss_user_privilege IS 'A granular access right or privilege within the application that may be granted to a role';

COMMENT ON COLUMN dss_user_privilege.user_privilege_cd IS 'The immutable system code that identifies the privilege';

COMMENT ON COLUMN dss_user_privilege.user_privilege_nm IS 'The human-readable name that is given for the role';

COMMENT ON TABLE dss_user_role IS 'A set of access rights and privileges within the application that may be granted to users';

COMMENT ON COLUMN dss_user_role.user_role_cd IS 'The immutable system code that identifies the role';

COMMENT ON COLUMN dss_user_role.user_role_nm IS 'The human-readable name that is given for the role';

COMMENT ON TABLE dss_user_role_privilege IS 'The association of a granular application privilege to a role';

COMMENT ON COLUMN dss_user_role_privilege.user_privilege_cd IS 'Foreign key';

COMMENT ON COLUMN dss_user_role_privilege.user_role_cd IS 'Foreign key';

COMMENT ON TABLE dss_organization IS 'A private company or governing body that plays a role in short term rental reporting or enforcement';

COMMENT ON COLUMN dss_organization.organization_id IS 'Unique generated key';

COMMENT ON COLUMN dss_organization.organization_type IS 'Foreign key';

COMMENT ON COLUMN dss_organization.organization_cd IS 'An immutable system code that identifies the organization (e.g. CEU, AIRBNB)';

COMMENT ON COLUMN dss_organization.organization_nm IS 'A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)';

COMMENT ON COLUMN dss_organization.local_government_geometry IS 'the shape identifying the boundaries of a local government';

COMMENT ON COLUMN dss_organization.managing_organization_id IS 'Self-referential hierarchical foreign key';

COMMENT ON COLUMN dss_organization.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_organization.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_organization_contact_person IS 'A person who has been identified as a notable contact for a particular organization';

COMMENT ON COLUMN dss_organization_contact_person.organization_contact_person_id IS 'Unique generated key';

COMMENT ON COLUMN dss_organization_contact_person.is_primary IS 'Indicates whether the contact should receive all communications directed at the organization';

COMMENT ON COLUMN dss_organization_contact_person.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';

COMMENT ON COLUMN dss_organization_contact_person.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';

COMMENT ON COLUMN dss_organization_contact_person.phone_no IS 'Phone number given for the contact by the organization (contains only digits)';

COMMENT ON COLUMN dss_organization_contact_person.email_address_dsc IS 'E-mail address given for the contact by the organization';

COMMENT ON COLUMN dss_organization_contact_person.contacted_through_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_organization_contact_person.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_organization_contact_person.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON COLUMN dss_organization_contact_person.email_message_type IS 'Foreign key';

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

COMMENT ON COLUMN dss_physical_address.containing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_physical_address.replacing_physical_address_id IS 'Foreign key';

COMMENT ON COLUMN dss_physical_address.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_physical_address.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

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

COMMENT ON COLUMN dss_upload_line.source_line_txt IS 'Full text of the uploaod line';

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

COMMENT ON TABLE dss_rental_listing IS 'A rental listing snapshot that is either relevant to a specific monthly report, or is the current, master version';

COMMENT ON COLUMN dss_rental_listing.rental_listing_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing.platform_listing_no IS 'The platform issued identification number for the listing';

COMMENT ON COLUMN dss_rental_listing.platform_listing_url IS 'URL for the short-term rental platform listing';

COMMENT ON COLUMN dss_rental_listing.business_licence_no IS 'The local government issued licence number that applies to the rental offering';

COMMENT ON COLUMN dss_rental_listing.bc_registry_no IS 'The Short Term Registry issued permit number';

COMMENT ON COLUMN dss_rental_listing.is_current IS 'Indicates whether the listing version is the most current one (within the same listing number for the same offering platform)';

COMMENT ON COLUMN dss_rental_listing.is_taken_down IS 'Indicates whether a current listing is no longer considered active';

COMMENT ON COLUMN dss_rental_listing.is_entire_unit IS 'Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom)';

COMMENT ON COLUMN dss_rental_listing.available_bedrooms_qty IS 'The number of bedrooms in the dwelling unit that are available for short term rental';

COMMENT ON COLUMN dss_rental_listing.nights_booked_qty IS 'The number of nights that short term rental accommodation services were provided during the reporting period';

COMMENT ON COLUMN dss_rental_listing.separate_reservations_qty IS 'The number of separate reservations that were taken during the reporting period';

COMMENT ON COLUMN dss_rental_listing.offering_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.including_rental_listing_report_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.derived_from_rental_listing_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.locating_physical_address_id IS 'Foreign key';

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

COMMENT ON COLUMN dss_email_message.lg_phone_no IS 'A phone number associated with a Local Government contact';

COMMENT ON COLUMN dss_email_message.unreported_listing_no IS 'The platform issued identification number for the listing (if not included in a rental listing report)';

COMMENT ON COLUMN dss_email_message.host_email_address_dsc IS 'E-mail address of a short term rental host (directly entered by the user as a message recipient)';

COMMENT ON COLUMN dss_email_message.lg_email_address_dsc IS 'E-mail address of a local government contact (directly entered by the user as a message recipient)';

COMMENT ON COLUMN dss_email_message.cc_email_address_dsc IS 'E-mail address of a secondary message recipient (directly entered by the user)';

COMMENT ON COLUMN dss_email_message.unreported_listing_url IS 'User-provided URL for a short-term rental platform listing that is the subject of the message';

COMMENT ON COLUMN dss_email_message.lg_str_bylaw_url IS 'User-provided URL for a local government bylaw that is the subject of the message';

COMMENT ON COLUMN dss_email_message.concerned_with_rental_listing_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.initiating_user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.affected_by_user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.involved_in_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.batching_email_message_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.requesting_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.message_reason_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.external_message_no IS 'External identifier for tracking the message delivery progress';

COMMENT ON COLUMN dss_email_message.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_email_message.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';
