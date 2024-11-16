/* Sprint 18 Incremental DB Changes to STR DSS */

DROP INDEX IF EXISTS dss_business_licence_i4 ;

CREATE INDEX IF NOT EXISTS dss_business_licence_i4 ON dss_business_licence  ( regexp_replace(regexp_replace(UPPER(business_licence_no), '[^A-Z0-9]+', '', 'g'), '^0+', '') ) ;

DROP INDEX IF EXISTS dss_rental_listing_i10 ;

CREATE INDEX IF NOT EXISTS dss_rental_listing_i10 ON dss_rental_listing  ( regexp_replace(regexp_replace(UPPER(business_licence_no), '[^A-Z0-9]+', '', 'g'), '^0+', '') ) ;

COMMENT ON TABLE dss_business_licence_status_type IS 'A potential status for a BUSINESS LICENCE (e.g. Pending, Denied, Issued, Suspended, Revoked, Cancelled, Expired)';

COMMENT ON COLUMN dss_business_licence_status_type.licence_status_type IS 'System-consistent code for the business licence status (e.g. Pending, Denied, Issued, Suspended, Revoked, Cancelled, Expired)';

COMMENT ON COLUMN dss_business_licence_status_type.licence_status_type_nm IS 'Business term for the licence status (e.g. Pending, Denied, Issued, Suspended, Revoked, Cancelled, Expired)';
