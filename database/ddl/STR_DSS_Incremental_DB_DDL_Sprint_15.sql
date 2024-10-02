/* Sprint 15 Incremental DB Changes to STR DSS */

CREATE  TABLE dss_platform_type ( 
	platform_type        varchar(25)  NOT NULL  ,
	platform_type_nm     varchar(250)  NOT NULL  ,
	CONSTRAINT dss_platform_type_pk PRIMARY KEY ( platform_type )
 ) ;

ALTER TABLE dss_organization ADD is_active boolean    ;

ALTER TABLE dss_organization ADD platform_type varchar(25)    ;

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_fk_classified_as FOREIGN KEY ( platform_type ) REFERENCES dss_platform_type( platform_type )   ;

COMMENT ON COLUMN dss_organization.local_government_type IS 'A sub-type of local government organization used for sorting and grouping of members';

COMMENT ON COLUMN dss_organization.is_active IS 'Indicates whether the ORGANIZATION is currently available for new associations';

COMMENT ON COLUMN dss_organization.platform_type IS 'Foreign key';

COMMENT ON TABLE dss_platform_type IS 'A sub-type of rental platform organization used for sorting and grouping of members';

COMMENT ON COLUMN dss_platform_type.platform_type IS 'System-consistent code (e.g. Major/Minor)';

COMMENT ON COLUMN dss_platform_type.platform_type_nm IS 'Business term for the platform type (e.g. Major/Minor)';

