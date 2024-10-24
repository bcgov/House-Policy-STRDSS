/* Sprint 17 Incremental DB Changes to STR DSS (before seeding) */

CREATE  TABLE dss_local_government_type ( 
	local_government_type varchar(50)  NOT NULL  ,
	local_government_type_nm varchar(250)  NOT NULL  ,
	CONSTRAINT dss_local_government_type_pk PRIMARY KEY ( local_government_type )
 ) ;

ALTER TABLE dss_organization ADD business_licence_format_txt varchar(50)    ;

ALTER TABLE dss_organization ADD regional_district_dsc varchar(100)    ;

COMMENT ON COLUMN dss_organization.is_str_prohibited IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION entirely prohibits short term housing rentals';

COMMENT ON COLUMN dss_organization.business_licence_format_txt IS 'A free form indication of how BUSINESS NUMBER is laid out for a LOCAL GOVERNMENT ORGANIZATION';

COMMENT ON COLUMN dss_organization.regional_district_dsc IS 'a free form description of the regional district to which the Local Government Subdivision belongs';

COMMENT ON COLUMN dss_organization.platform_type IS 'Foreign key for a RENTAL PLATFORM';

COMMENT ON COLUMN dss_organization.local_government_type IS 'Foreign key for a LOCAL GOVERNMENT';

COMMENT ON TABLE dss_local_government_type IS 'A sub-type of local government organization used for sorting and grouping of members';

COMMENT ON COLUMN dss_local_government_type.local_government_type IS 'System-consistent code (e.g. Municipality, First Nations Community)';

COMMENT ON COLUMN dss_local_government_type.local_government_type_nm IS 'Business term for for the local government type (e.g. Municipality, First Nations Community)';
