/* Sprint 7 Incremental DB Changes to STR DSS */

CREATE  TABLE dss_rental_listing_extract ( 
	rental_listing_extract_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	rental_listing_extract_nm varchar(250)  NOT NULL  ,
	is_pr_requirement_filtered boolean  NOT NULL  ,
	source_bin           bytea    ,
	filtering_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_rental_listing_extract_pk PRIMARY KEY ( rental_listing_extract_id )
 ) ;

ALTER TABLE dss_physical_address ADD is_match_verified boolean    ;

ALTER TABLE dss_physical_address ADD is_match_corrected boolean    ;

ALTER TABLE dss_physical_address ADD is_system_processing boolean    ;

ALTER TABLE dss_rental_listing ADD is_changed_address boolean    ;

ALTER TABLE dss_rental_listing ADD is_lg_transferred boolean    ;

ALTER TABLE dss_user_role ADD user_role_dsc varchar(200)    ;

ALTER TABLE dss_user_role ADD upd_dtm timestamptz    ;

ALTER TABLE dss_user_role ADD upd_user_guid uuid    ;

ALTER TABLE dss_user_role_assignment ADD upd_dtm timestamptz    ;

ALTER TABLE dss_user_role_assignment ADD upd_user_guid uuid    ;

ALTER TABLE dss_user_role_privilege ADD upd_dtm timestamptz    ;

ALTER TABLE dss_user_role_privilege ADD upd_user_guid uuid    ;

ALTER TABLE dss_rental_listing_extract ADD CONSTRAINT dss_rental_listing_extract_fk_filtered_by FOREIGN KEY ( filtering_organization_id ) REFERENCES dss_organization( organization_id )   ;

CREATE TRIGGER dss_rental_listing_extract_br_iu_tr BEFORE INSERT OR UPDATE ON dss_rental_listing_extract FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_user_role_assignment_br_iu_tr BEFORE INSERT OR UPDATE ON dss_user_role_assignment FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_user_role_br_iu_tr BEFORE INSERT OR UPDATE ON dss_user_role FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

CREATE TRIGGER dss_user_role_privilege_br_iu_tr BEFORE INSERT OR UPDATE ON dss_user_role_privilege FOR EACH ROW EXECUTE FUNCTION dss_update_audit_columns();

COMMENT ON COLUMN dss_physical_address.is_match_verified IS 'Indicates whether the matched address has been verified as correct for the listing by the responsible authorities';

COMMENT ON COLUMN dss_physical_address.is_match_corrected IS 'Indicates whether the matched address has been manually changed to one that is verified as correct for the listing';

COMMENT ON COLUMN dss_physical_address.is_system_processing IS 'Indicates whether the physical address is being processed by the system and may not yet be in its final form';

COMMENT ON COLUMN dss_rental_listing.is_changed_address IS 'Indicates whether a CURRENT RENTAL LISTING has been subjected to address changes';

COMMENT ON COLUMN dss_rental_listing.is_lg_transferred IS 'Indicates whether a CURRENT RENTAL LISTING has been transferred to a different Local Goverment Organization as a result of address changes';

COMMENT ON COLUMN dss_user_role.user_role_dsc IS 'The human-readable description that is given for the role';

COMMENT ON COLUMN dss_user_role.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_role.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON COLUMN dss_user_role_assignment.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_role_assignment.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON COLUMN dss_user_role_privilege.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_role_privilege.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_rental_listing_extract IS 'A prebuilt report that is specific to a subset of rental listings';

COMMENT ON COLUMN dss_rental_listing_extract.rental_listing_extract_id IS 'Unique generated key';

COMMENT ON COLUMN dss_rental_listing_extract.rental_listing_extract_nm IS 'A description of the information contained in the extract';

COMMENT ON COLUMN dss_rental_listing_extract.is_pr_requirement_filtered IS 'Indicates whether the report is filtered by jurisdictional principal residence requirement';

COMMENT ON COLUMN dss_rental_listing_extract.source_bin IS 'The binary image of the information in the report';

COMMENT ON COLUMN dss_rental_listing_extract.filtering_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_rental_listing_extract.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_rental_listing_extract.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

