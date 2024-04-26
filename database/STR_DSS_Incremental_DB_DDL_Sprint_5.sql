/* Sprint 5 Incremental DB Changes to STR DSS */

ALTER TABLE dss_rental_listing_line DROP CONSTRAINT dss_rental_listing_line_fk_included_in;

ALTER TABLE dss_rental_listing_line DROP CONSTRAINT dss_rental_listing_line_pk;

ALTER TABLE dss_rental_listing_report DROP COLUMN source_bin CASCADE;

ALTER TABLE dss_rental_listing ALTER COLUMN platform_listing_no TYPE varchar(50);

ALTER TABLE dss_rental_listing ALTER COLUMN business_licence_no TYPE varchar(50);

ALTER TABLE dss_rental_listing ALTER COLUMN bc_registry_no TYPE varchar(50);

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
	report_period_ym     date  NOT NULL  ,
	source_hash_dsc      varchar(256)  NOT NULL  ,
	source_bin           bytea    ,
	providing_organization_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_upload_delivery_pk PRIMARY KEY ( upload_delivery_id )
 ) ;

ALTER TABLE dss_upload_line ADD is_processed boolean  NOT NULL  ;

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_pk PRIMARY KEY ( upload_line_id ) ;

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_fk_included_in FOREIGN KEY ( including_upload_delivery_id ) REFERENCES dss_upload_delivery( upload_delivery_id )   ;

ALTER TABLE dss_upload_delivery ADD CONSTRAINT dss_upload_delivery_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id )   ;

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

/* Manual script additions start here */

CREATE OR REPLACE TRIGGER dss_upload_delivery_br_iu_tr
     BEFORE INSERT OR UPDATE ON dss_upload_delivery
    FOR EACH ROW
    EXECUTE PROCEDURE dss_update_audit_columns();
