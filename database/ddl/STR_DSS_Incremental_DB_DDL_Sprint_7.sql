/* Sprint 7 Incremental DB Changes to STR DSS */

CREATE INDEX dss_email_message_i1 ON dss_email_message  ( concerned_with_rental_listing_id, message_delivery_dtm );

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_uk UNIQUE ( organization_cd );

CREATE INDEX dss_organization_i1 ON dss_organization  ( organization_type );

CREATE INDEX dss_organization_i2 ON dss_organization  ( managing_organization_id );

CREATE INDEX dss_physical_address_i1 ON dss_physical_address  ( original_address_txt );

CREATE INDEX dss_physical_address_i2 ON dss_physical_address  ( match_address_txt );

CREATE INDEX dss_physical_address_i3 ON dss_physical_address  ( containing_organization_id );

CREATE INDEX dss_rental_listing_i1 ON dss_rental_listing  ( offering_organization_id, platform_listing_no ) ;

CREATE INDEX dss_rental_listing_i2 ON dss_rental_listing  ( including_rental_listing_report_id );

CREATE INDEX dss_rental_listing_i3 ON dss_rental_listing  ( derived_from_rental_listing_id );

CREATE INDEX dss_rental_listing_i4 ON dss_rental_listing  ( locating_physical_address_id );

CREATE INDEX dss_rental_listing_contact_i1 ON dss_rental_listing_contact  ( contacted_through_rental_listing_id );

ALTER TABLE dss_rental_listing_report ADD CONSTRAINT dss_rental_listing_report_uk UNIQUE ( providing_organization_id, report_period_ym );

ALTER TABLE dss_upload_line ADD CONSTRAINT dss_upload_line_uk UNIQUE ( including_upload_delivery_id, source_record_no );
