/* Sprint 5 Incremental DB Changes to STR DSS */

TRUNCATE dss_rental_listing_report CASCADE;

TRUNCATE dss_physical_address CASCADE;

ALTER TABLE dss_rental_listing_line DROP CONSTRAINT dss_rental_listing_line_fk_included_in;

ALTER TABLE dss_rental_listing_line DROP CONSTRAINT dss_rental_listing_line_pk;

ALTER TABLE dss_rental_listing_report DROP COLUMN source_bin CASCADE;

ALTER TABLE dss_email_message ALTER COLUMN is_host_contacted_externally DROP NOT NULL;

ALTER TABLE dss_rental_listing ALTER COLUMN platform_listing_no TYPE varchar(50);

ALTER TABLE dss_rental_listing ALTER COLUMN business_licence_no TYPE varchar(50);

ALTER TABLE dss_rental_listing ALTER COLUMN bc_registry_no TYPE varchar(50);

ALTER TABLE dss_rental_listing ALTER COLUMN including_rental_listing_report_id DROP NOT NULL;

ALTER TABLE dss_rental_listing_contact ALTER COLUMN supplier_host_no TYPE varchar(50);

ALTER TABLE dss_rental_listing_report RENAME COLUMN is_processed TO is_current;

ALTER TABLE dss_rental_listing_line RENAME TO dss_upload_line;

ALTER TABLE dss_upload_line ALTER COLUMN platform_listing_no TYPE varchar(50);

ALTER TABLE dss_upload_line RENAME COLUMN rental_listing_line_id TO upload_line_id;

ALTER TABLE dss_upload_line RENAME COLUMN organization_cd TO source_organization_cd;

ALTER TABLE dss_upload_line RENAME COLUMN platform_listing_no TO source_record_no;

ALTER TABLE dss_upload_line RENAME COLUMN including_rental_listing_report_id TO including_upload_delivery_id;

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
 ) ;

ALTER TABLE dss_email_message ADD concerned_with_rental_listing_id bigint    ;

ALTER TABLE dss_physical_address ADD unit_no varchar(50)    ;

ALTER TABLE dss_physical_address ADD civic_no varchar(50)    ;

ALTER TABLE dss_physical_address ADD street_nm varchar(50)    ;

ALTER TABLE dss_physical_address ADD street_type_dsc varchar(50)    ;

ALTER TABLE dss_physical_address ADD street_direction_dsc varchar(50)    ;

ALTER TABLE dss_physical_address ADD locality_nm varchar(50)    ;

ALTER TABLE dss_physical_address ADD locality_type_dsc varchar(50)    ;

ALTER TABLE dss_physical_address ADD province_cd varchar(5)    ;

ALTER TABLE dss_physical_address ADD replacing_physical_address_id bigint    ;

ALTER TABLE dss_rental_listing ADD is_current boolean  NOT NULL  ;

ALTER TABLE dss_rental_listing ADD is_taken_down boolean    ;

ALTER TABLE dss_rental_listing ADD derived_from_rental_listing_id bigint    ;

ALTER TABLE dss_upload_line ADD is_processed boolean  NOT NULL  ;

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_pk PRIMARY KEY ( upload_line_id ) ;

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_included_in FOREIGN KEY ( concerned_with_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id )   ;

ALTER TABLE dss_physical_address ADD CONSTRAINT dss_physical_address_fk_replaced_by FOREIGN KEY ( replacing_physical_address_id ) REFERENCES dss_physical_address( physical_address_id )   ;

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_generating FOREIGN KEY ( derived_from_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id )   ;

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_fk_included_in FOREIGN KEY ( including_upload_delivery_id ) REFERENCES dss_upload_delivery( upload_delivery_id )   ;

ALTER TABLE dss_upload_delivery ADD CONSTRAINT dss_upload_delivery_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id )   ;

CREATE TRIGGER dss_upload_delivery_br_iu_tr BEFORE INSERT OR UPDATE ON dss_upload_delivery FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

COMMENT ON COLUMN dss_email_message.concerned_with_rental_listing_id IS 'Foreign key';

COMMENT ON COLUMN dss_physical_address.match_address_txt IS 'The sanitized physical address (returned as fullAddress) that has been derived from the original';

COMMENT ON COLUMN dss_physical_address.unit_no IS 'The unitNumber (suite) returned by the address match (e.g. 100)';

COMMENT ON COLUMN dss_physical_address.civic_no IS 'The civicNumber (building number) returned by the address match (e.g. 1285)';

COMMENT ON COLUMN dss_physical_address.street_nm IS 'The streetName returned by the address match (e.g. Pender)';

COMMENT ON COLUMN dss_physical_address.street_type_dsc IS 'The streetType returned by the address match (e.g. St or Street)';

COMMENT ON COLUMN dss_physical_address.street_direction_dsc IS 'The streetDirection returned by the address match (e.g. W or West)';

COMMENT ON COLUMN dss_physical_address.locality_nm IS 'The localityName (community) returned by the address match (e.g. Vancouver)';

COMMENT ON COLUMN dss_physical_address.locality_type_dsc IS 'The localityType returned by the address match (e.g. City)';

COMMENT ON COLUMN dss_physical_address.province_cd IS 'The provinceCode returned by the address match';

COMMENT ON COLUMN dss_physical_address.replacing_physical_address_id IS 'Foreign key';

COMMENT ON TABLE dss_rental_listing IS 'A rental listing snapshot that is either relevant to a specific monthly report, or is the current, master version';

COMMENT ON COLUMN dss_rental_listing.is_current IS 'Indicates whether the listing version is the most current one (within the same listing number for the same offering platform)';

COMMENT ON COLUMN dss_rental_listing.is_taken_down IS 'Indicates whether a current listing is no longer considered active';

COMMENT ON COLUMN dss_rental_listing.derived_from_rental_listing_id IS 'Foreign key';

COMMENT ON TABLE dss_rental_listing_report IS 'A platform-specific collection of rental listing information that is relevant to a specific month';

COMMENT ON COLUMN dss_rental_listing_report.is_current IS 'Indicates whether the rental listing version is the most recent one reported by the platform';

COMMENT ON TABLE dss_upload_line IS 'An upload delivery line that has been extracted from the source';

COMMENT ON COLUMN dss_upload_line.is_validation_failure IS 'Indicates that there has been a validation problem that prevents successful ingestion of the upload line';

COMMENT ON COLUMN dss_upload_line.is_system_failure IS 'Indicates that a system fault has prevented complete ingestion of the upload line';

COMMENT ON COLUMN dss_upload_line.is_processed IS 'Indicates that no further ingestion attempt is required for the upload line';

COMMENT ON COLUMN dss_upload_line.source_organization_cd IS 'An immutable system code identifying the organization who created the information in the upload line (e.g. AIRBNB)';

COMMENT ON COLUMN dss_upload_line.source_record_no IS 'The immutable identification number for the source record, such as a rental listing number';

COMMENT ON COLUMN dss_upload_line.source_line_txt IS 'Full text of the uploaod line';

COMMENT ON TABLE dss_upload_delivery IS 'A delivery of uploaded information that is relevant to a specific month';

COMMENT ON COLUMN dss_upload_delivery.upload_delivery_id IS 'Unique generated key';

COMMENT ON COLUMN dss_upload_delivery.upload_delivery_type IS 'Identifies the treatment applied to ingesting the uploaded information';

COMMENT ON COLUMN dss_upload_delivery.report_period_ym IS 'The month to which the delivery batch is relevant (always set to the first day of the month)';

COMMENT ON COLUMN dss_upload_delivery.source_hash_dsc IS 'The hash value of the information that was uploaded';

COMMENT ON COLUMN dss_upload_delivery.source_bin IS 'The binary image of the information that was uploaded';

COMMENT ON COLUMN dss_upload_delivery.providing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_upload_delivery.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_upload_delivery.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';
