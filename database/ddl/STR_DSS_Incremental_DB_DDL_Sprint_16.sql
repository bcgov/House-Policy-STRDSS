/* Sprint 16 Incremental DB Changes to STR DSS */

ALTER TABLE dss_user_identity ADD external_identity_cd varchar(100)    ;

COMMENT ON COLUMN dss_user_identity.external_identity_cd IS 'A non-guid unique identifier assigned by the identity provider';
