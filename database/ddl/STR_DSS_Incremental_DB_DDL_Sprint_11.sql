/* Sprint 11 Incremental DB Changes to STR DSS */

drop view if exists dss_rental_listing_vw;

ALTER TABLE dss_upload_line DROP CONSTRAINT dss_upload_line_uk;

ALTER TABLE dss_rental_listing ALTER COLUMN business_licence_no TYPE varchar(230);

ALTER TABLE dss_rental_listing_contact ALTER COLUMN full_nm TYPE varchar(230);

ALTER TABLE dss_organization ADD local_government_type varchar(50)    ;

ALTER TABLE dss_upload_delivery ADD source_header_txt varchar(32000)    ;

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_uk UNIQUE ( including_upload_delivery_id, source_organization_cd, source_record_no ) ;

CREATE INDEX dss_upload_line_i1 ON dss_upload_line  ( is_processed, including_upload_delivery_id ) ;

COMMENT ON COLUMN dss_organization.local_government_type IS 'A sub-type of local government organization used for sorting and grouping or members';

COMMENT ON COLUMN dss_upload_delivery.source_header_txt IS 'Full text of the header line';

COMMENT ON COLUMN dss_upload_line.source_line_txt IS 'Full text of the upload line';

