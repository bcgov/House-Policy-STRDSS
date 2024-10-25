/* Sprint 17 Incremental DB Changes to STR DSS (before seeding) */

CREATE  TABLE dss_economic_region ( 
	economic_region_dsc  varchar(100)  NOT NULL  ,
	economic_region_nm   varchar(250)  NOT NULL  ,
	economic_region_sort_no smallint    ,
	CONSTRAINT dss_economic_region_pk PRIMARY KEY ( economic_region_dsc )
 ) ;

CREATE  TABLE dss_local_government_type ( 
	local_government_type varchar(50)  NOT NULL  ,
	local_government_type_nm varchar(250)  NOT NULL  ,
	local_government_type_sort_no smallint    ,
	CONSTRAINT dss_local_government_type_pk PRIMARY KEY ( local_government_type )
 ) ;

ALTER TABLE dss_organization ADD business_licence_format_txt varchar(50)    ;

COMMENT ON COLUMN dss_organization.is_str_prohibited IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION entirely prohibits short term housing rentals';

COMMENT ON COLUMN dss_organization.business_licence_format_txt IS 'A free form indication of how BUSINESS NUMBER is laid out for a LOCAL GOVERNMENT ORGANIZATION';

COMMENT ON COLUMN dss_organization.platform_type IS 'Foreign key for a RENTAL PLATFORM';

COMMENT ON COLUMN dss_organization.local_government_type IS 'Foreign key for a LOCAL GOVERNMENT';

COMMENT ON COLUMN dss_organization.economic_region_dsc IS 'Foreign key for a Local Government Subdivision';

COMMENT ON TABLE dss_economic_region IS 'A geographic classification of LOCAL GOVERNMENT SUBDIVISION used for sorting and grouping of members';

COMMENT ON COLUMN dss_economic_region.economic_region_dsc IS 'System-consistent code (e.g. Northeast, Cariboo)';

COMMENT ON COLUMN dss_economic_region.economic_region_nm IS 'Business term for the ECONOMIC REGION (e.g. Northeast, Cariboo)';

COMMENT ON COLUMN dss_economic_region.economic_region_sort_no IS 'Relative order in which the business prefers to see the ECONOMIC REGION listed';

COMMENT ON TABLE dss_local_government_type IS 'A sub-type of local government organization used for sorting and grouping of members';

COMMENT ON COLUMN dss_local_government_type.local_government_type IS 'System-consistent code (e.g. Municipality, First Nations Community)';

COMMENT ON COLUMN dss_local_government_type.local_government_type_nm IS 'Business term for for the local government type (e.g. Municipality, First Nations Community)';

COMMENT ON COLUMN dss_local_government_type.local_government_type_sort_no IS 'Relative order in which the business prefers to see the LOCAL GOVERNMENT TYPE listed';
