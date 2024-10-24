/* Sprint 17 Incremental DB Changes to STR DSS (after seeding) */

ALTER TABLE dss_organization ADD CONSTRAINT fk_dss_organization_fk_administered_as FOREIGN KEY ( local_government_type ) REFERENCES dss_local_government_type( local_government_type )   ;
