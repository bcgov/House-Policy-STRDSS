/* Sprint 4 Incremental DB Changes to STR DSS */

ALTER TABLE dss_email_message ALTER COLUMN unreported_listing_no TYPE varchar(50);

ALTER TABLE dss_email_message ALTER COLUMN initiating_user_identity_id DROP NOT NULL;

ALTER TABLE dss_organization_contact_person ALTER COLUMN is_primary DROP NOT NULL;

ALTER TABLE dss_organization_contact_person ALTER COLUMN given_nm DROP NOT NULL;

ALTER TABLE dss_organization_contact_person ALTER COLUMN family_nm DROP NOT NULL;

ALTER TABLE dss_organization_contact_person ALTER COLUMN phone_no DROP NOT NULL;

ALTER TABLE dss_email_message ADD batching_email_message_id bigint    ;

ALTER TABLE dss_email_message ADD requesting_organization_id bigint    ;

ALTER TABLE dss_email_message ADD external_message_no varchar(50)    ;

ALTER TABLE dss_email_message ADD upd_dtm timestamptz    ;

ALTER TABLE dss_email_message ADD upd_user_guid uuid    ;

ALTER TABLE dss_organization_contact_person ADD email_message_type varchar(50)    ;

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_batched_in FOREIGN KEY ( batching_email_message_id ) REFERENCES dss_email_message( email_message_id )   ;

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_requested_by FOREIGN KEY ( requesting_organization_id ) REFERENCES dss_organization( organization_id )   ;

ALTER TABLE dss_organization_contact_person ADD CONSTRAINT dss_organization_contact_person_fk_subscribed_to FOREIGN KEY ( email_message_type ) REFERENCES dss_email_message_type( email_message_type )   ;

COMMENT ON TABLE dss_access_request_status IS 'A potential status for a user access request (e.g. Requested, Approved, or Denied)';

COMMENT ON COLUMN dss_access_request_status.access_request_status_cd IS 'System-consistent code for the request status';

COMMENT ON COLUMN dss_access_request_status.access_request_status_nm IS 'Business term for the request status';

COMMENT ON COLUMN dss_email_message.email_message_type IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.is_host_contacted_externally IS 'Indicates whether the the property host has already been contacted by external means';

COMMENT ON COLUMN dss_email_message.is_submitter_cc_required IS 'Indicates whether the user initiating the message should receive a copy of the email';

COMMENT ON COLUMN dss_email_message.message_reason_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.lg_phone_no IS 'A phone number associated with a Local Government contact';

COMMENT ON COLUMN dss_email_message.unreported_listing_no IS 'The platform issued identification number for the listing (if not included in a rental listing report)';

COMMENT ON COLUMN dss_email_message.lg_email_address_dsc IS 'E-mail address of a local government contact (directly entered by the user as a message recipient)';

COMMENT ON COLUMN dss_email_message.lg_str_bylaw_url IS 'User-provided URL for a local government bylaw that is the subject of the message';

COMMENT ON COLUMN dss_email_message.batching_email_message_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.requesting_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.external_message_no IS 'External identifier for tracking the message delivery progress';

COMMENT ON COLUMN dss_email_message.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_email_message.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_email_message_type IS 'The type or purpose of a system generated message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON COLUMN dss_email_message_type.email_message_type IS 'System-consistent code for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON COLUMN dss_email_message_type.email_message_type_nm IS 'Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON TABLE dss_message_reason IS 'A description of the justification for initiating a message';

COMMENT ON COLUMN dss_message_reason.message_reason_id IS 'Unique generated key';

COMMENT ON COLUMN dss_message_reason.email_message_type IS 'Foreign key';

COMMENT ON COLUMN dss_organization.organization_type IS 'Foreign key';

COMMENT ON COLUMN dss_organization_contact_person.email_message_type IS 'Foreign key';

COMMENT ON TABLE dss_organization_type IS 'A level of government or business category';

COMMENT ON COLUMN dss_organization_type.organization_type IS 'System-consistent code for a level of government or business category';

COMMENT ON COLUMN dss_organization_type.organization_type_nm IS 'Business term for a level of government or business category';

/* Manual script additions start here */

CREATE OR REPLACE TRIGGER dss_email_message_br_iu_tr
     BEFORE INSERT OR UPDATE ON dss_email_message
    FOR EACH ROW
    EXECUTE PROCEDURE dss_update_audit_columns();
