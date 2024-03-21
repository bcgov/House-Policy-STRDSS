CREATE  TABLE dss_organization ( 
	organization_id      bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	organization_type    varchar(25)  NOT NULL  ,
	organization_cd      varchar(25)  NOT NULL  ,
	organization_nm      varchar(250)  NOT NULL  ,
	local_government_geometry geometry    ,
	managing_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_organization_pk PRIMARY KEY ( organization_id )
 );

CREATE  TABLE dss_organization_type ( 
	organization_type         varchar(25)  NOT NULL  ,
	organization_type_nm      varchar(250)  NOT NULL  ,
	CONSTRAINT dss_organization_type_pk PRIMARY KEY ( organization_type )
 );

CREATE  TABLE dss_organization_contact_person ( 
	organization_contact_person_id bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	is_primary           boolean  NOT NULL  ,
	given_nm             varchar(25)  NOT NULL  ,
	family_nm            varchar(25)  NOT NULL  ,
	phone_no             varchar(13)  NOT NULL  ,
	email_address_dsc    varchar(320)  NOT NULL  ,
	contacted_through_organization_id bigint  NOT NULL  ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_organization_contact_person_pk PRIMARY KEY ( organization_contact_person_id )
 );

CREATE  TABLE dss_user_identity ( 
	user_identity_id     bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	user_guid            uuid  NOT NULL  ,
	display_nm           varchar(250)  NOT NULL  ,
	identity_provider_nm varchar(25)  NOT NULL  ,
	is_enabled           boolean  NOT NULL  ,
	access_request_status_cd varchar(25)  NOT NULL  ,
	access_request_dtm   timestamptz    ,
	access_request_justification_txt varchar(250)    ,
	given_nm             varchar(25)    ,
	family_nm            varchar(25)    ,
	email_address_dsc    varchar(320)    ,
	business_nm          varchar(250)    ,
	terms_acceptance_dtm timestamptz    ,
	represented_by_organization_id bigint    ,
	upd_dtm              timestamptz  NOT NULL  ,
	upd_user_guid        uuid    ,
	CONSTRAINT dss_user_identity_pk PRIMARY KEY ( user_identity_id )
 );

CREATE  TABLE dss_access_request_status ( 
	access_request_status_cd         varchar(25)  NOT NULL  ,
	access_request_status_nm         varchar(250)  NOT NULL  ,
	CONSTRAINT dss_access_request_status_pk PRIMARY KEY ( access_request_status_cd )
 );

CREATE  TABLE dss_user_privilege ( 
	user_privilege_cd         varchar(25)  NOT NULL  ,
	user_privilege_nm         varchar(250)  NOT NULL  ,
	CONSTRAINT dss_user_privilege_pk PRIMARY KEY ( user_privilege_cd )
 );

CREATE  TABLE dss_user_role ( 
	user_role_cd         varchar(25)  NOT NULL  ,
	user_role_nm         varchar(250)  NOT NULL  ,
	CONSTRAINT dss_user_role_pk PRIMARY KEY ( user_role_cd )
 );

CREATE  TABLE dss_user_role_assignment ( 
	user_identity_id     bigint  NOT NULL  ,
	user_role_cd         varchar(25)  NOT NULL  ,
	CONSTRAINT dss_user_role_assignment_pk PRIMARY KEY ( user_identity_id, user_role_cd )
 );

CREATE  TABLE dss_user_role_privilege ( 
	user_privilege_cd    varchar(25)  NOT NULL  ,
	user_role_cd         varchar(25)  NOT NULL  ,
	CONSTRAINT dss_user_role_privilege_pk PRIMARY KEY ( user_privilege_cd, user_role_cd )
 );

CREATE  TABLE dss_email_message ( 
	email_message_id     bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	email_message_type   varchar(50)  NOT NULL  ,
	message_delivery_dtm timestamptz  NOT NULL  ,
	message_template_dsc varchar(4000)  NOT NULL  ,
	is_host_contacted_externally boolean  NOT NULL  ,
	is_submitter_cc_required boolean  NOT NULL  ,
	message_reason_id     bigint   ,
	lg_phone_no varchar(13)    ,
	unreported_listing_no varchar(25)    ,
	host_email_address_dsc varchar(320)    ,
	lg_email_address_dsc varchar(320)    ,
	cc_email_address_dsc varchar(4000)    ,
	unreported_listing_url varchar(4000)    ,
	lg_str_bylaw_url varchar(4000)    ,
	initiating_user_identity_id bigint  NOT NULL  ,
	affected_by_user_identity_id bigint    ,
	involved_in_organization_id bigint    ,
	CONSTRAINT dss_email_message_pk PRIMARY KEY ( email_message_id )
 );

CREATE  TABLE dss_email_message_type ( 
	email_message_type         varchar(50)  NOT NULL  ,
	email_message_type_nm      varchar(250)  NOT NULL  ,
	CONSTRAINT dss_email_message_type_pk PRIMARY KEY ( email_message_type )
 );

CREATE  TABLE dss_message_reason ( 
	message_reason_id    bigint  NOT NULL GENERATED ALWAYS AS IDENTITY  ,
	email_message_type    varchar(50)  NOT NULL  ,
	message_reason_dsc   varchar(250)  NOT NULL  ,
	CONSTRAINT dss_message_reason_pk PRIMARY KEY ( message_reason_id )
 );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_initiated_by FOREIGN KEY ( initiating_user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_affecting FOREIGN KEY ( affected_by_user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_involving FOREIGN KEY ( involved_in_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_communicating FOREIGN KEY ( email_message_type ) REFERENCES dss_email_message_type( email_message_type );

ALTER TABLE dss_email_message ADD CONSTRAINT dss_email_message_fk_justified_by FOREIGN KEY ( message_reason_id ) REFERENCES dss_message_reason( message_reason_id );

ALTER TABLE dss_message_reason ADD CONSTRAINT dss_message_reason_fk_justifying FOREIGN KEY ( email_message_type ) REFERENCES dss_email_message_type( email_message_type );

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_fk_managed_by FOREIGN KEY ( managing_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_organization ADD CONSTRAINT dss_organization_fk_treated_as FOREIGN KEY ( organization_type ) REFERENCES dss_organization_type( organization_type );

ALTER TABLE dss_organization_contact_person ADD CONSTRAINT dss_organization_contact_person_fk_contacted_for FOREIGN KEY ( contacted_through_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_user_identity ADD CONSTRAINT dss_user_identity_fk_representing FOREIGN KEY ( represented_by_organization_id ) REFERENCES dss_organization( organization_id );

ALTER TABLE dss_user_identity ADD CONSTRAINT dss_user_identity_fk_given FOREIGN KEY ( access_request_status_cd ) REFERENCES dss_access_request_status( access_request_status_cd );

ALTER TABLE dss_user_role_assignment ADD CONSTRAINT dss_user_role_assignment_fk_granted FOREIGN KEY ( user_role_cd ) REFERENCES dss_user_role( user_role_cd );

ALTER TABLE dss_user_role_assignment ADD CONSTRAINT dss_user_role_assignment_fk_granted_to FOREIGN KEY ( user_identity_id ) REFERENCES dss_user_identity( user_identity_id );

ALTER TABLE dss_user_role_privilege ADD CONSTRAINT dss_user_role_privilege_fk_conferred_by FOREIGN KEY ( user_role_cd ) REFERENCES dss_user_role( user_role_cd );

ALTER TABLE dss_user_role_privilege ADD CONSTRAINT dss_user_role_privilege_fk_conferring FOREIGN KEY ( user_privilege_cd ) REFERENCES dss_user_privilege( user_privilege_cd );

COMMENT ON TABLE dss_organization IS 'A private company or governing body that plays a role in short term rental reporting or enforcement';

COMMENT ON COLUMN dss_organization.organization_id IS 'Unique generated key';

COMMENT ON COLUMN dss_organization.organization_type IS 'a level of government or business category';

COMMENT ON COLUMN dss_organization.organization_cd IS 'An immutable system code that identifies the organization (e.g. CEU, AIRBNB)';

COMMENT ON COLUMN dss_organization.organization_nm IS 'A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)';

COMMENT ON COLUMN dss_organization.local_government_geometry IS 'the shape identifying the boundaries of a local government';

COMMENT ON COLUMN dss_organization.managing_organization_id IS 'Self-referential hierarchical foreign key';

COMMENT ON COLUMN dss_organization.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_organization.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_organization_contact_person IS 'A person who has been identified as a notable contact for a particular organization';

COMMENT ON COLUMN dss_organization_contact_person.organization_contact_person_id IS 'Unique generated key';

COMMENT ON COLUMN dss_organization_contact_person.is_primary IS 'Indicates whether the contact should receive all communications directed at the organization';

COMMENT ON COLUMN dss_organization_contact_person.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';

COMMENT ON COLUMN dss_organization_contact_person.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';

COMMENT ON COLUMN dss_organization_contact_person.phone_no IS 'Phone number given for the contact by the organization (contains only digits)';

COMMENT ON COLUMN dss_organization_contact_person.email_address_dsc IS 'E-mail address given for the contact by the organization';

COMMENT ON COLUMN dss_organization_contact_person.contacted_through_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_organization_contact_person.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_organization_contact_person.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_user_identity IS 'An externally defined domain directory object representing a potential application user or group';

COMMENT ON COLUMN dss_user_identity.user_identity_id IS 'Unique generated key';

COMMENT ON COLUMN dss_user_identity.user_guid IS 'An immutable unique identifier assigned by the identity provider';

COMMENT ON COLUMN dss_user_identity.display_nm IS 'A human-readable full name that is assigned by the identity provider (this may include a preferred name and/or business unit name)';

COMMENT ON COLUMN dss_user_identity.identity_provider_nm IS 'A directory or domain that authenticates system users to allow access to the application or its API  (e.g. idir, bceidbusiness)';

COMMENT ON COLUMN dss_user_identity.is_enabled IS 'Indicates whether access is currently permitted using this identity';

COMMENT ON COLUMN dss_user_identity.access_request_status_cd IS 'The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied)';

COMMENT ON COLUMN dss_user_identity.access_request_dtm IS 'A timestamp indicating when the most recent access request was made by the user';

COMMENT ON COLUMN dss_user_identity.access_request_justification_txt IS 'The most recent user-provided reason for requesting application access';

COMMENT ON COLUMN dss_user_identity.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';

COMMENT ON COLUMN dss_user_identity.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';

COMMENT ON COLUMN dss_user_identity.email_address_dsc IS 'E-mail address associated with the user by the identity provider';

COMMENT ON COLUMN dss_user_identity.business_nm IS 'A human-readable organization name that is associated with the user by the identity provider';

COMMENT ON COLUMN dss_user_identity.terms_acceptance_dtm IS 'A timestamp indicating when the user most recently accepted the published Terms and Conditions of application access';

COMMENT ON COLUMN dss_user_identity.represented_by_organization_id IS 'Foreign key';

COMMENT ON COLUMN dss_user_identity.upd_dtm IS 'Trigger-updated timestamp of last change';

COMMENT ON COLUMN dss_user_identity.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';

COMMENT ON TABLE dss_user_privilege IS 'A granular access right or privilege within the application that may be granted to a role';

COMMENT ON COLUMN dss_user_privilege.user_privilege_cd IS 'The immutable system code that identifies the privilege';

COMMENT ON COLUMN dss_user_privilege.user_privilege_nm IS 'The human-readable name that is given for the role';

COMMENT ON TABLE dss_user_role IS 'A set of access rights and privileges within the application that may be granted to users';

COMMENT ON COLUMN dss_user_role.user_role_cd IS 'The immutable system code that identifies the role';

COMMENT ON COLUMN dss_user_role.user_role_nm IS 'The human-readable name that is given for the role';

COMMENT ON TABLE dss_user_role_assignment IS 'The association of a grantee credential to a role for the purpose of conveying application privileges';

COMMENT ON COLUMN dss_user_role_assignment.user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_user_role_assignment.user_role_cd IS 'Foreign key';

COMMENT ON TABLE dss_user_role_privilege IS 'The association of a granular application privilege to a role';

COMMENT ON COLUMN dss_user_role_privilege.user_privilege_cd IS 'Foreign key';

COMMENT ON COLUMN dss_user_role_privilege.user_role_cd IS 'Foreign key';

COMMENT ON TABLE dss_email_message IS 'A message that is sent to one or more recipients via email';

COMMENT ON COLUMN dss_email_message.email_message_id IS 'Unique generated key';

COMMENT ON COLUMN dss_email_message.email_message_type IS 'Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';

COMMENT ON COLUMN dss_email_message.message_delivery_dtm IS 'A timestamp indicating when the message delivery was initiated';

COMMENT ON COLUMN dss_email_message.message_template_dsc IS 'The full text or template for the message that is sent';

COMMENT ON COLUMN dss_message_reason.message_reason_dsc IS 'A description of the justification for initiating a message';

COMMENT ON COLUMN dss_email_message.unreported_listing_url IS 'User-provided URL for a short-term rental platform listing that is the subject of the message';

COMMENT ON COLUMN dss_email_message.host_email_address_dsc IS 'E-mail address of a short term rental host (directly entered by the user as a message recipient)';

COMMENT ON COLUMN dss_email_message.cc_email_address_dsc IS 'E-mail address of a secondary message recipient (directly entered by the user)';

COMMENT ON COLUMN dss_email_message.initiating_user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.affected_by_user_identity_id IS 'Foreign key';

COMMENT ON COLUMN dss_email_message.involved_in_organization_id IS 'Foreign key';

CREATE OR REPLACE FUNCTION dss_update_audit_columns() RETURNS trigger
    LANGUAGE plpgsql AS
$$BEGIN
    NEW.upd_dtm := current_timestamp;
    RETURN NEW;
END;$$;

CREATE OR REPLACE TRIGGER dss_organization_br_iu_tr
     BEFORE INSERT OR UPDATE ON dss_organization
    FOR EACH ROW
    EXECUTE PROCEDURE dss_update_audit_columns();

CREATE OR REPLACE TRIGGER dss_organization_contact_person_br_iu_tr
     BEFORE INSERT OR UPDATE ON dss_organization_contact_person
    FOR EACH ROW
    EXECUTE PROCEDURE dss_update_audit_columns();
	
CREATE OR REPLACE TRIGGER dss_user_identity_br_iu_tr
     BEFORE INSERT OR UPDATE ON dss_user_identity
    FOR EACH ROW
    EXECUTE PROCEDURE dss_update_audit_columns();
