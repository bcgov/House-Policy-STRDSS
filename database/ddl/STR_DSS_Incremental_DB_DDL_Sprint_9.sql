/* Sprint 9 Incremental DB Changes to STR DSS */

drop view if exists dss_rental_listing_vw;

ALTER TABLE dss_physical_address ALTER COLUMN street_nm TYPE varchar(100);

ALTER TABLE dss_physical_address ALTER COLUMN locality_nm TYPE varchar(100);

ALTER TABLE dss_physical_address ADD "is_changed_original_address" boolean    ;

ALTER TABLE dss_rental_listing ALTER COLUMN business_licence_no TYPE varchar(100);

ALTER TABLE dss_rental_listing_contact ALTER COLUMN full_nm TYPE varchar(100);

CREATE  TABLE dss_listing_status_type ( 
	listing_status_type  varchar(2)  NOT NULL  ,
	listing_status_type_nm varchar(50)  NOT NULL  ,
	listing_status_sort_no smallint  NOT NULL  ,
	CONSTRAINT dss_listing_status_type_pk PRIMARY KEY ( listing_status_type )
 ) ;

ALTER TABLE dss_organization ADD economic_region_dsc varchar(100)    ;

ALTER TABLE dss_rental_listing ADD "is_changed_original_address" boolean    ;

ALTER TABLE dss_rental_listing ADD listing_status_type varchar(2)    ;

CREATE INDEX dss_rental_listing_i5 ON dss_rental_listing  ( listing_status_type, offering_organization_id ) ;

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_classified_as FOREIGN KEY ( listing_status_type ) REFERENCES dss_listing_status_type( listing_status_type )   ;

COMMENT ON COLUMN dss_organization.economic_region_dsc IS 'A free form description of the economic region to which a Local Government Subdivision belongs';

COMMENT ON COLUMN dss_physical_address."is_changed_original_address" IS 'Indicates whether the original address has received a different property address from the platform in the last reporting period';

COMMENT ON COLUMN dss_rental_listing."is_changed_original_address" IS 'Indicates whether a CURRENT RENTAL LISTING has received a different property address in the last reporting period';

COMMENT ON COLUMN dss_rental_listing.is_changed_address IS 'Indicates whether a CURRENT RENTAL LISTING has been subjected to address match changes by a user';

COMMENT ON COLUMN dss_rental_listing.listing_status_type IS 'Foreign key';

COMMENT ON TABLE dss_listing_status_type IS 'A potential status for a CURRENT RENTAL LISTING (e.g. New, Active, Inactive, Reassigned, Taken Down)';

COMMENT ON COLUMN dss_listing_status_type.listing_status_type IS 'System-consistent code for the listing status (e.g. N, A, I, R, T)';

COMMENT ON COLUMN dss_listing_status_type.listing_status_type_nm IS 'Business term for the listing status (e.g. New, Active, Inactive, Reassigned, Taken Down)';
