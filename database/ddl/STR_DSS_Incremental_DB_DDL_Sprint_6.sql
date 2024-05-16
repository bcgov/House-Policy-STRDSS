/* Sprint 6 Incremental DB Changes to STR DSS */

ALTER TABLE dss_email_message DROP COLUMN message_reason_id CASCADE;

DROP TABLE dss_message_reason;

ALTER TABLE dss_organization RENAME COLUMN local_government_geometry TO area_geometry;

ALTER TABLE dss_email_message ADD is_with_standard_detail boolean    ;

ALTER TABLE dss_email_message ADD custom_detail_txt varchar(4000)    ;

ALTER TABLE dss_organization ADD is_lg_participating boolean    ;

ALTER TABLE dss_organization ADD is_principal_residence_required boolean    ;

ALTER TABLE dss_organization ADD is_business_licence_required boolean    ;

ALTER TABLE dss_rental_listing ADD is_active boolean    ;

ALTER TABLE dss_rental_listing ADD is_new boolean    ;

COMMENT ON COLUMN dss_email_message.is_with_standard_detail IS 'Indicates whether message body should include text a block of detail text that is standard for the message type';

COMMENT ON COLUMN dss_email_message.custom_detail_txt IS 'Free form text that should be included in the message body';

COMMENT ON TABLE dss_organization IS 'A private company or governing body component that plays a role in short term rental reporting or enforcement';

COMMENT ON COLUMN dss_organization.is_lg_participating IS 'Indicates whether a LOCAL GOVERNMENT ORGANIZATION participates in Short Term Rental Data Sharing';

COMMENT ON COLUMN dss_organization.is_principal_residence_required IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION is subject to Provincial Principal Residence Short Term Rental restrictions';

COMMENT ON COLUMN dss_organization.is_business_licence_required IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION requires a business licence for Short Term Rental operation';

COMMENT ON COLUMN dss_organization.area_geometry IS 'the multipolygon shape identifying the boundaries of a local government subdivision';

COMMENT ON COLUMN dss_rental_listing.is_current IS 'Indicates whether the RENTAL LISTING VERSION is a CURRENT RENTAL LISTING (if it is a copy of the most current REPORTED RENTAL LISTING (having the same listing number for the same offering platform)';

COMMENT ON COLUMN dss_rental_listing.is_active IS 'Indicates whether a CURRENT RENTAL LISTING was included in the most recent RENTAL LISTING REPORT';

COMMENT ON COLUMN dss_rental_listing.is_new IS 'Indicates whether a CURRENT RENTAL LISTING appeared for the first time in the last reporting period';

COMMENT ON COLUMN dss_rental_listing.is_taken_down IS 'Indicates whether a CURRENT RENTAL LISTING has been reported as taken down by the offering platform';

