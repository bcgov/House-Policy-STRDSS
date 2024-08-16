/* Sprint 12 Incremental DB Changes to STR DSS */

drop view if exists dss_rental_listing_vw;

ALTER TABLE dss_rental_listing ALTER COLUMN business_licence_no TYPE varchar(320);

ALTER TABLE dss_rental_listing_contact ALTER COLUMN full_nm TYPE varchar(320);

CREATE  TABLE dss_business_licence_status_type ( 
	licence_status_type  varchar(25)  NOT NULL  ,
	licence_status_type_nm varchar(50)  NOT NULL  ,
	licence_status_sort_no smallint  NOT NULL  ,
	CONSTRAINT dss_business_licence_status_type_pk PRIMARY KEY ( licence_status_type )
 ) ;

CREATE  TABLE dss_business_licence ( 
	business_licence_id  bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	business_licence_no  varchar(50)  NOT NULL  ,
	expiry_dt            date  NOT NULL  ,
	physical_rental_address_txt varchar(250)    ,
	licence_type_txt     varchar(320)    ,
	restriction_txt      varchar(320)    ,
	business_nm          varchar(320)    ,
	mailing_street_address_txt varchar(100)    ,
	mailing_city_nm      varchar(100)    ,
	mailing_province_cd  varchar(2)    ,
	mailing_postal_cd    varchar(10)    ,
	business_owner_nm    varchar(320)    ,
	business_owner_phone_no varchar(30)    ,
	business_owner_email_address_dsc varchar(320)    ,
	business_operator_nm varchar(320)    ,
	business_operator_phone_no varchar(30)    ,
	business_operator_email_address_dsc varchar(320)    ,
	infraction_txt       varchar(320)    ,
	infraction_dt        date    ,
	property_zone_txt    varchar(100)    ,
	available_bedrooms_qty smallint    ,
	max_guests_allowed_qty smallint    ,
	is_principal_residence boolean    ,
	is_owner_living_onsite boolean    ,
	is_owner_property_tenant boolean    ,
	property_folio_no    varchar(30)    ,
	property_parcel_identifier_no varchar(30)    ,
	property_legal_description_txt varchar(320)    ,
	licence_status_type  varchar(25)  NOT NULL  ,
	providing_organization_id bigint  NOT NULL  ,
	affected_by_physical_address_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_business_licence_pk PRIMARY KEY ( business_licence_id ),
	CONSTRAINT dss_business_licence_uk UNIQUE ( providing_organization_id, business_licence_no ) 
 ) ;

CREATE INDEX dss_business_licence_i2 ON dss_business_licence  ( affected_by_physical_address_id ) ;

CREATE INDEX dss_business_licence_i1 ON dss_business_licence  ( licence_status_type ) ;

CREATE INDEX dss_business_licence_i3 ON dss_business_licence  ( physical_rental_address_txt ) ;

ALTER TABLE dss_rental_listing ADD effective_business_licence_no varchar(320)    ;

ALTER TABLE dss_rental_listing ADD effective_host_nm varchar(320)    ;

ALTER TABLE dss_rental_listing ADD is_changed_business_licence boolean    ;

ALTER TABLE dss_rental_listing ADD governing_business_licence_id bigint    ;

CREATE INDEX dss_rental_listing_i6 ON dss_rental_listing  ( governing_business_licence_id ) ;

CREATE INDEX dss_rental_listing_i7 ON dss_rental_listing  ( business_licence_no ) ;

CREATE INDEX dss_rental_listing_i8 ON dss_rental_listing  ( effective_business_licence_no ) ;

CREATE INDEX dss_rental_listing_i9 ON dss_rental_listing  ( effective_host_nm ) ;

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_governed_by FOREIGN KEY ( governing_business_licence_id ) REFERENCES dss_business_licence( business_licence_id )   ;

ALTER TABLE dss_business_licence ADD CONSTRAINT dss_business_licence_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id )   ;

ALTER TABLE dss_business_licence ADD CONSTRAINT dss_business_licence_fk_affecting FOREIGN KEY ( affected_by_physical_address_id ) REFERENCES dss_physical_address( physical_address_id )   ;

ALTER TABLE dss_business_licence ADD CONSTRAINT dss_business_licence_fk_classified_as FOREIGN KEY ( licence_status_type ) REFERENCES dss_business_licence_status_type( licence_status_type )   ;

CREATE TRIGGER dss_business_licence_br_iu_tr BEFORE INSERT OR UPDATE ON dss_business_licence FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

COMMENT ON COLUMN dss_listing_status_type.listing_status_sort_no IS 'Relative order in which the business prefers to see the status listed';

COMMENT ON COLUMN dss_rental_listing.business_licence_no IS 'The business licence number that is published for the rental offering';

COMMENT ON COLUMN dss_rental_listing.effective_business_licence_no IS 'The local government issued licence number that should be used for compariing the rental offering with others';

COMMENT ON COLUMN dss_rental_listing.effective_host_nm IS 'The full name of the rental offering host that should be used for comparison with others';

COMMENT ON COLUMN dss_rental_listing.is_changed_business_licence IS 'Indicates whether a CURRENT RENTAL LISTING has been subjected to business licence linking changes by a user';

COMMENT ON COLUMN dss_rental_listing.governing_business_licence_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing_contact.full_nm IS 'The full name of the contact person as included in the listing';

COMMENT ON TABLE dss_business_licence_status_type IS 'A potential status for a BUSINESS LICENCE (e.g. Pending, Issued, Suspended, Revoked, Cancelled, Expired)';

COMMENT ON COLUMN dss_business_licence_status_type.licence_status_type IS 'System-consistent code for the business licence status (e.g. Pending, Issued, Suspended, Revoked, Cancelled, Expired)';

COMMENT ON COLUMN dss_business_licence_status_type.licence_status_type_nm IS 'Business term for the licence status (e.g. Pending, Issued, Suspended, Revoked, Cancelled, Expired)';

COMMENT ON COLUMN dss_business_licence_status_type.licence_status_sort_no IS 'Relative order in which the business prefers to see the status listed';

COMMENT ON COLUMN dss_business_licence.business_licence_id IS 'Unique generated key';

COMMENT ON COLUMN dss_business_licence.business_licence_no IS 'The local government issued licence number that applies to the rental offering';

COMMENT ON COLUMN dss_business_licence.expiry_dt IS 'The date on which the business licence expires';

COMMENT ON COLUMN dss_business_licence.physical_rental_address_txt IS 'The full physical address of the location that is licenced as a short-term rental business';

COMMENT ON COLUMN dss_business_licence.licence_type_txt IS 'Free form description of the type of business licence issued (e.g. short-term rental, bed and breakfast, boarding and lodging, tourist accommodation)';

COMMENT ON COLUMN dss_business_licence.restriction_txt IS 'Notes related to any restrictions associated with the licence';

COMMENT ON COLUMN dss_business_licence.business_nm IS 'Official name of the business';

COMMENT ON COLUMN dss_business_licence.mailing_street_address_txt IS 'Street address component of the business mailing address';

COMMENT ON COLUMN dss_business_licence.mailing_city_nm IS 'City component of the business mailing address';

COMMENT ON COLUMN dss_business_licence.mailing_province_cd IS 'Province component of the business mailing address';

COMMENT ON COLUMN dss_business_licence.mailing_postal_cd IS 'Postal code component of the business mailing address';

COMMENT ON COLUMN dss_business_licence.business_owner_nm IS 'Full name of the registered business owner';

COMMENT ON COLUMN dss_business_licence.business_owner_phone_no IS 'Phone number of the business owner';

COMMENT ON COLUMN dss_business_licence.business_owner_email_address_dsc IS 'Email address of the business owner';

COMMENT ON COLUMN dss_business_licence.business_operator_nm IS 'Full name of the business operator or property manager';

COMMENT ON COLUMN dss_business_licence.business_operator_phone_no IS 'Phone number of the business operator';

COMMENT ON COLUMN dss_business_licence.business_operator_email_address_dsc IS 'Email address of the business operator';

COMMENT ON COLUMN dss_business_licence.infraction_txt IS 'Description of an infraction';

COMMENT ON COLUMN dss_business_licence.infraction_dt IS 'The date on which the described infraction occurred';

COMMENT ON COLUMN dss_business_licence.property_zone_txt IS 'Description or name of the property zoning';

COMMENT ON COLUMN dss_business_licence.available_bedrooms_qty IS 'The number of bedrooms in the dwelling unit that are available for short term rental';

COMMENT ON COLUMN dss_business_licence.max_guests_allowed_qty IS 'The number of guests that can be accommodated';

COMMENT ON COLUMN dss_business_licence.is_principal_residence IS 'Indicates whether the short term rental property is a principal residence';

COMMENT ON COLUMN dss_business_licence.is_owner_living_onsite IS 'Indicates whether the owner lives on the property';

COMMENT ON COLUMN dss_business_licence.is_owner_property_tenant IS 'Indicates whether the business owner rents the property';

COMMENT ON COLUMN dss_business_licence.property_folio_no IS 'The number used to identify the property';

COMMENT ON COLUMN dss_business_licence.property_parcel_identifier_no IS 'The PID number assigned by the Land Title and Survey Authority that identifies the piece of land';

COMMENT ON COLUMN dss_business_licence.property_legal_description_txt IS 'The physical description of the property as it is registered with the Land Title and Survey Authority';

COMMENT ON COLUMN dss_business_licence.licence_status_type IS 'Foreign key';

COMMENT ON COLUMN dss_business_licence.providing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_business_licence.affected_by_physical_address_id IS 'Foreign key';

COMMENT ON COLUMN dss_business_licence.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_business_licence.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

