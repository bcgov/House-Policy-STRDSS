/* Sprint 13 Incremental DB Changes to STR DSS */

CREATE INDEX IF NOT EXISTS dss_business_licence_i4 ON dss_business_licence  ( regexp_replace(UPPER(business_licence_no), '[^A-Z0-9]+', '', 'g') ) ;

CREATE INDEX IF NOT EXISTS dss_rental_listing_i10 ON dss_rental_listing  ( regexp_replace(UPPER(business_licence_no), '[^A-Z0-9]+', '', 'g') ) ;
