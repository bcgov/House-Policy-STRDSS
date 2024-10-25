/* Sprint 17 Incremental DB Changes to STR DSS (after seeding) */

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_fk_administered_as FOREIGN KEY ( local_government_type ) REFERENCES dss_local_government_type( local_government_type )   ;

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_fk_within FOREIGN KEY ( economic_region_dsc ) REFERENCES dss_economic_region( economic_region_dsc )   ;
