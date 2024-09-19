/* Sprint 14 Incremental DB Changes to STR DSS */

ALTER TABLE dss_organization ADD is_str_prohibited boolean    ;

ALTER TABLE dss_rental_listing ADD lg_transfer_dtm timestamptz    ;

CREATE INDEX dss_rental_listing_i11 ON dss_rental_listing  ( lg_transfer_dtm ) ;

COMMENT ON COLUMN dss_organization.is_str_prohibited IS 'Indicates whether a LOCAL GOVERNMENT ORGANIZATION entirely prohibits short term housing rentals';

COMMENT ON COLUMN dss_rental_listing.lg_transfer_dtm IS 'Indicates when a CURRENT RENTAL LISTING was most recently transferred to a different Local Goverment Organization as a result of address changes';
