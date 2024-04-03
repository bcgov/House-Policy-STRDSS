CREATE  TABLE dss_physical_address ( 
	physical_address_id  bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	original_address_txt varchar(250)  NOT NULL  ,
	match_address_txt    varchar(250)    ,
	match_score_amt      smallint    ,
	match_precision_dsc  varchar(50)    ,
	match_precision_amt  smallint    ,
	site_nm              varchar(50)    ,
	unit_designator_txt  varchar(50)    ,
	unit_no              varchar(50)    ,
	unit_number_suffix_txt varchar(50)    ,
	civic_no             varchar(50)    ,
	civic_number_suffix_txt varchar(50)    ,
	street_nm            varchar(100)    ,
	street_type_dsc      varchar(50)    ,
	is_street_type_prefix boolean    ,
	street_direction_dsc varchar(50)    ,
	is_street_direction_prefix boolean    ,
	street_qualifier_dsc varchar(50)    ,
	locality_nm          varchar(100)    ,
	locality_type_dsc    varchar(50)    ,
	electoral_area_nm    varchar(100)    ,
	site_no              varchar(50)    ,
	block_no             varchar(50)    ,
	is_official          boolean    ,
	location_geometry    geometry    ,
	containing_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_physical_address_pk PRIMARY KEY ( physical_address_id )
 ) ;

CREATE  TABLE dss_rental_listing_report ( 
	rental_listing_report_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	is_current           boolean  NOT NULL  ,
	report_period_ym     date  NOT NULL  ,
	providing_organization_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_report_pk PRIMARY KEY ( rental_listing_report_id )
 ) ;

CREATE  TABLE dss_rental_listing ( 
	rental_listing_id    bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	platform_listing_no  varchar    ,
	business_licence_no  varchar(25)    ,
	bc_registry_no       varchar(25)    ,
	is_entire_unit       boolean    ,
	available_bedrooms_qty smallint    ,
	nights_booked_qty    smallint    ,
	separate_reservations_qty smallint    ,
	including_rental_listing_report_id bigint  NOT NULL  ,
	offering_organization_id bigint  NOT NULL  ,
	locating_physical_address_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_pk PRIMARY KEY ( rental_listing_id )
 ) ;

CREATE  TABLE dss_rental_listing_contact ( 
	rental_listing_contact_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	is_property_owner    boolean  NOT NULL  ,
	listing_contact_nbr  smallint    ,
	full_nm              varchar(50)    ,
	phone_no             varchar(13)    ,
	fax_no               varchar(13)    ,
	full_address_txt     varchar(250)    ,
	email_address_dsc    varchar(320)    ,
	contacted_through_rental_listing_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_contact_pk PRIMARY KEY ( rental_listing_contact_id )
 ) ;

ALTER TABLE dss_physical_address ADD CONSTRAINT dss_physical_address_fk_contained_in FOREIGN KEY ( containing_organization_id ) REFERENCES dss_organization( organization_id )   ;

ALTER TABLE dss_rental_listing_report ADD CONSTRAINT dss_rental_listing_report_fk_provided_by FOREIGN KEY ( providing_organization_id ) REFERENCES dss_organization( organization_id )   ;

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_offered_by FOREIGN KEY ( offering_organization_id ) REFERENCES dss_organization( organization_id )   ;

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_included_in FOREIGN KEY ( including_rental_listing_report_id ) REFERENCES dss_rental_listing_report( rental_listing_report_id )   ;

ALTER TABLE dss_rental_listing ADD CONSTRAINT dss_rental_listing_fk_located_at FOREIGN KEY ( locating_physical_address_id ) REFERENCES dss_physical_address( physical_address_id )   ;

ALTER TABLE dss_rental_listing_contact ADD CONSTRAINT dss_rental_listing_contact_fk_contacted_for FOREIGN KEY ( contacted_through_rental_listing_id ) REFERENCES dss_rental_listing( rental_listing_id )   ;

COMMENT ON TABLE dss_physical_address IS 'A property address that includes any verifiable BC attributes';

COMMENT ON COLUMN dss_physical_address.physical_address_id IS 'Unique generated key';

COMMENT ON COLUMN dss_physical_address.original_address_txt IS 'The source-provided address of a short-term rental offering';

COMMENT ON COLUMN dss_physical_address.match_address_txt IS 'The sanitized physical address that has been derived from the original';

COMMENT ON COLUMN dss_physical_address.location_geometry IS 'The computed location point of the matched address';

COMMENT ON COLUMN dss_physical_address.containing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_physical_address.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_physical_address.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing_report IS 'A delivery of rental listing information that is relevant to a specific month';

COMMENT ON COLUMN dss_rental_listing_report.rental_listing_report_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing_report.is_current IS 'Indicates whether the report is the one with the most recent reporting month for the platform (and therefore current)';

COMMENT ON COLUMN dss_rental_listing_report.report_period_ym IS 'The month to which the listing information is relevant (always set to the first day of the month)';

COMMENT ON COLUMN dss_rental_listing_report.providing_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing_report.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing_report.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing IS 'A rental listing snapshot that is relevant to a specific month';

COMMENT ON COLUMN dss_rental_listing.rental_listing_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing.platform_listing_no IS 'The platform issued identification number for the listing';

COMMENT ON COLUMN dss_rental_listing.business_licence_no IS 'The local government issued licence number that applies to the rental offering';

COMMENT ON COLUMN dss_rental_listing.bc_registry_no IS 'The Short Term Registry issued permit number';

COMMENT ON COLUMN dss_rental_listing.is_entire_unit IS 'Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom)';

COMMENT ON COLUMN dss_rental_listing.available_bedrooms_qty IS 'The number of bedrooms in the dwelling unit that are available for short term rental';

COMMENT ON COLUMN dss_rental_listing.nights_booked_qty IS 'The number of nights that short term rental accommodation services were provided during the reporting period';

COMMENT ON COLUMN dss_rental_listing.separate_reservations_qty IS 'The number of separate reservations that were taken during the reporting period';

COMMENT ON COLUMN dss_rental_listing.including_rental_listing_report_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.offering_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.locating_physical_address_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing_contact IS 'A person who has been identified as a notable contact for a particular rental listing';

COMMENT ON COLUMN dss_rental_listing_contact.rental_listing_contact_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing_contact.is_property_owner IS 'Indicates a person with the legal right to the unit being short-term rental';

COMMENT ON COLUMN dss_rental_listing_contact.listing_contact_nbr IS 'Indicates which of the five possible supplier hosts is represented by this contact';

COMMENT ON COLUMN dss_rental_listing_contact.full_nm IS 'The full name of the contact person as inluded in the listing';

COMMENT ON COLUMN dss_rental_listing_contact.phone_no IS 'Phone number given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.fax_no IS 'Facsimile numbrer given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.full_address_txt IS 'Mailing address given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.email_address_dsc IS 'E-mail address given for the contact';

COMMENT ON COLUMN dss_rental_listing_contact.contacted_through_rental_listing_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing_contact.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing_contact.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

