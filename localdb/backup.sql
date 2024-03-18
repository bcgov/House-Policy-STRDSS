--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1 (Debian 16.1-1.pgdg110+1)
-- Dumped by pg_dump version 16.1 (Debian 16.1-1.pgdg110+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE ONLY dss.dss_user_role_privilege DROP CONSTRAINT dss_user_role_privilege_fk_conferring;
ALTER TABLE ONLY dss.dss_user_role_privilege DROP CONSTRAINT dss_user_role_privilege_fk_conferred_by;
ALTER TABLE ONLY dss.dss_user_role_assignment DROP CONSTRAINT dss_user_role_assignment_fk_granted_to;
ALTER TABLE ONLY dss.dss_user_role_assignment DROP CONSTRAINT dss_user_role_assignment_fk_granted;
ALTER TABLE ONLY dss.dss_user_identity DROP CONSTRAINT dss_user_identity_fk_representing;
ALTER TABLE ONLY dss.dss_organization DROP CONSTRAINT dss_organization_fk_managed_by;
ALTER TABLE ONLY dss.dss_organization_contact_person DROP CONSTRAINT dss_organization_contact_person_fk_contacted_for;
ALTER TABLE ONLY dss.dss_email_message DROP CONSTRAINT dss_email_message_fk_involving;
ALTER TABLE ONLY dss.dss_email_message DROP CONSTRAINT dss_email_message_fk_initiated_by;
ALTER TABLE ONLY dss.dss_email_message DROP CONSTRAINT dss_email_message_fk_affecting;
DROP TRIGGER dss_user_identity_br_iu_tr ON dss.dss_user_identity;
DROP TRIGGER dss_organization_contact_person_br_iu_tr ON dss.dss_organization_contact_person;
DROP TRIGGER dss_organization_br_iu_tr ON dss.dss_organization;
ALTER TABLE ONLY dss.dss_user_role_privilege DROP CONSTRAINT dss_user_role_privilege_pk;
ALTER TABLE ONLY dss.dss_user_role DROP CONSTRAINT dss_user_role_pk;
ALTER TABLE ONLY dss.dss_user_role_assignment DROP CONSTRAINT dss_user_role_assignment_pk;
ALTER TABLE ONLY dss.dss_user_privilege DROP CONSTRAINT dss_user_privilege_pk;
ALTER TABLE ONLY dss.dss_user_identity DROP CONSTRAINT dss_user_identity_pk;
ALTER TABLE ONLY dss.dss_organization DROP CONSTRAINT dss_organization_pk;
ALTER TABLE ONLY dss.dss_organization_contact_person DROP CONSTRAINT dss_organization_contact_person_pk;
ALTER TABLE ONLY dss.dss_email_message DROP CONSTRAINT dss_email_message_pk;
DROP TABLE dss.dss_user_role_privilege;
DROP TABLE dss.dss_user_role_assignment;
DROP TABLE dss.dss_user_role;
DROP TABLE dss.dss_user_privilege;
DROP TABLE dss.dss_user_identity;
DROP TABLE dss.dss_organization_contact_person;
DROP TABLE dss.dss_organization;
DROP TABLE dss.dss_email_message;
DROP FUNCTION public.dss_update_audit_columns();
DROP FUNCTION dss.dss_update_audit_columns();
DROP EXTENSION postgis;
DROP SCHEMA dss;
--
-- Name: dss; Type: SCHEMA; Schema: -; Owner: strdssdev
--

CREATE SCHEMA dss;


ALTER SCHEMA dss OWNER TO strdssdev;

--
-- Name: postgis; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;


--
-- Name: EXTENSION postgis; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION postgis IS 'PostGIS geometry and geography spatial types and functions';


--
-- Name: dss_update_audit_columns(); Type: FUNCTION; Schema: dss; Owner: strdssdev
--

CREATE FUNCTION dss.dss_update_audit_columns() RETURNS trigger
    LANGUAGE plpgsql
    AS $$BEGIN
    NEW.upd_dtm := current_timestamp;
    RETURN NEW;
END;$$;


ALTER FUNCTION dss.dss_update_audit_columns() OWNER TO strdssdev;

--
-- Name: dss_update_audit_columns(); Type: FUNCTION; Schema: public; Owner: strdssdev
--

CREATE FUNCTION public.dss_update_audit_columns() RETURNS trigger
    LANGUAGE plpgsql
    AS $$BEGIN
    NEW.upd_dtm := current_timestamp;
    RETURN NEW;
END;$$;


ALTER FUNCTION public.dss_update_audit_columns() OWNER TO strdssdev;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: dss_email_message; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_email_message (
    email_message_id bigint NOT NULL,
    email_message_type character varying(50) NOT NULL,
    message_delivery_dtm timestamp with time zone NOT NULL,
    message_template_dsc character varying(4000) NOT NULL,
    message_reason_dsc character varying(250),
    unreported_listing_url character varying(250),
    host_email_address_dsc character varying(250),
    cc_email_address_dsc character varying(250),
    initiating_user_identity_id bigint NOT NULL,
    affected_by_user_identity_id bigint,
    involved_in_organization_id bigint,
    CONSTRAINT dss_email_message_ck CHECK (((email_message_type)::text = ANY ((ARRAY['Notice of Takedown'::character varying, 'Takedown Request'::character varying, 'Delisting Warning'::character varying, 'Delisting Request'::character varying, 'Access Granted Notification'::character varying, 'Access Denied Notification'::character varying])::text[])))
);


ALTER TABLE dss.dss_email_message OWNER TO strdssdev;

--
-- Name: TABLE dss_email_message; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_email_message IS 'A message that is sent to one or more recipients via email';


--
-- Name: COLUMN dss_email_message.email_message_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.email_message_id IS 'Unique generated key';


--
-- Name: COLUMN dss_email_message.email_message_type; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.email_message_type IS 'Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';


--
-- Name: COLUMN dss_email_message.message_delivery_dtm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.message_delivery_dtm IS 'A timestamp indicating when the message delivery was initiated';


--
-- Name: COLUMN dss_email_message.message_template_dsc; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.message_template_dsc IS 'The full text or template for the message that is sent';


--
-- Name: COLUMN dss_email_message.message_reason_dsc; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.message_reason_dsc IS 'A description of the justification for initiating the message';


--
-- Name: COLUMN dss_email_message.unreported_listing_url; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.unreported_listing_url IS 'User-provided URL for a short-term rental platform listing that is the subject of the message';


--
-- Name: COLUMN dss_email_message.host_email_address_dsc; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.host_email_address_dsc IS 'E-mail address of a short term rental host (directly entered by the user as a message recipient)';


--
-- Name: COLUMN dss_email_message.cc_email_address_dsc; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.cc_email_address_dsc IS 'E-mail address of a secondary message recipient (directly entered by the user)';


--
-- Name: COLUMN dss_email_message.initiating_user_identity_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.initiating_user_identity_id IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.affected_by_user_identity_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.affected_by_user_identity_id IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.involved_in_organization_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_email_message.involved_in_organization_id IS 'Foreign key';


--
-- Name: dss_email_message_email_message_id_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_email_message ALTER COLUMN email_message_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_email_message_email_message_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_organization; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_organization (
    organization_id bigint NOT NULL,
    organization_type character varying(25) NOT NULL,
    organization_cd character varying(25) NOT NULL,
    organization_nm character varying(250) NOT NULL,
    local_government_geometry public.geometry,
    managing_organization_id bigint,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid,
    CONSTRAINT dss_organization_ck CHECK (((organization_type)::text = ANY ((ARRAY['BCGov'::character varying, 'LG'::character varying, 'Platform'::character varying])::text[])))
);


ALTER TABLE dss.dss_organization OWNER TO strdssdev;

--
-- Name: TABLE dss_organization; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_organization IS 'A private company or governing body that plays a role in short term rental reporting or enforcement';


--
-- Name: COLUMN dss_organization.organization_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.organization_id IS 'Unique generated key';


--
-- Name: COLUMN dss_organization.organization_type; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.organization_type IS 'a level of government or business category';


--
-- Name: COLUMN dss_organization.organization_cd; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.organization_cd IS 'An immutable system code that identifies the organization (e.g. CEU, AIRBNB)';


--
-- Name: COLUMN dss_organization.organization_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.organization_nm IS 'A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)';


--
-- Name: COLUMN dss_organization.local_government_geometry; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.local_government_geometry IS 'the shape identifying the boundaries of a local government';


--
-- Name: COLUMN dss_organization.managing_organization_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.managing_organization_id IS 'Self-referential hierarchical foreign key';


--
-- Name: COLUMN dss_organization.upd_dtm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_organization.upd_user_guid; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_organization_contact_person; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_organization_contact_person (
    organization_contact_person_id bigint NOT NULL,
    is_primary boolean NOT NULL,
    given_nm character varying(25) NOT NULL,
    family_nm character varying(25) NOT NULL,
    phone_no character varying(13) NOT NULL,
    email_address_dsc character varying(250) NOT NULL,
    contacted_through_organization_id bigint NOT NULL,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE dss.dss_organization_contact_person OWNER TO strdssdev;

--
-- Name: TABLE dss_organization_contact_person; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_organization_contact_person IS 'A person who has been identified as a notable contact for a particular organization';


--
-- Name: COLUMN dss_organization_contact_person.organization_contact_person_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.organization_contact_person_id IS 'Unique generated key';


--
-- Name: COLUMN dss_organization_contact_person.is_primary; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.is_primary IS 'Indicates whether the contact should receive all communications directed at the organization';


--
-- Name: COLUMN dss_organization_contact_person.given_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';


--
-- Name: COLUMN dss_organization_contact_person.family_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';


--
-- Name: COLUMN dss_organization_contact_person.phone_no; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.phone_no IS 'Phone number given for the contact by the organization (contains only digits)';


--
-- Name: COLUMN dss_organization_contact_person.email_address_dsc; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.email_address_dsc IS 'E-mail address given for the contact by the organization';


--
-- Name: COLUMN dss_organization_contact_person.contacted_through_organization_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.contacted_through_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_organization_contact_person.upd_dtm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_organization_contact_person.upd_user_guid; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_organization_contact_person.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_organization_contact_pers_organization_contact_person_i_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_organization_contact_person ALTER COLUMN organization_contact_person_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_organization_contact_pers_organization_contact_person_i_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_organization_organization_id_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_organization ALTER COLUMN organization_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_organization_organization_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_user_identity; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_user_identity (
    user_identity_id bigint NOT NULL,
    user_guid uuid NOT NULL,
    display_nm character varying(250) NOT NULL,
    identity_provider_nm character varying(25) NOT NULL,
    is_enabled boolean NOT NULL,
    access_request_status_dsc character varying(25) NOT NULL,
    access_request_dtm timestamp with time zone,
    access_request_justification_txt character varying(250),
    given_nm character varying(25),
    family_nm character varying(25),
    email_address_dsc character varying(320),
    business_nm character varying(250),
    terms_acceptance_dtm timestamp with time zone,
    represented_by_organization_id bigint,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid,
    CONSTRAINT dss_user_identity_ck CHECK (((access_request_status_dsc)::text = ANY ((ARRAY['Requested'::character varying, 'Approved'::character varying, 'Denied'::character varying])::text[])))
);


ALTER TABLE dss.dss_user_identity OWNER TO strdssdev;

--
-- Name: TABLE dss_user_identity; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_user_identity IS 'An externally defined domain directory object representing a potential application user or group';


--
-- Name: COLUMN dss_user_identity.user_identity_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.user_identity_id IS 'Unique generated key';


--
-- Name: COLUMN dss_user_identity.user_guid; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.user_guid IS 'An immutable unique identifier assigned by the identity provider';


--
-- Name: COLUMN dss_user_identity.display_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.display_nm IS 'A human-readable full name that is assigned by the identity provider (this may include a preferred name and/or business unit name)';


--
-- Name: COLUMN dss_user_identity.identity_provider_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.identity_provider_nm IS 'A directory or domain that authenticates system users to allow access to the application or its API  (e.g. idir, bceidbusiness)';


--
-- Name: COLUMN dss_user_identity.is_enabled; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.is_enabled IS 'Indicates whether access is currently permitted using this identity';


--
-- Name: COLUMN dss_user_identity.access_request_status_dsc; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.access_request_status_dsc IS 'The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied)';


--
-- Name: COLUMN dss_user_identity.access_request_dtm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.access_request_dtm IS 'A timestamp indicating when the most recent access request was made by the user';


--
-- Name: COLUMN dss_user_identity.access_request_justification_txt; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.access_request_justification_txt IS 'The most recent user-provided reason for requesting application access';


--
-- Name: COLUMN dss_user_identity.given_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';


--
-- Name: COLUMN dss_user_identity.family_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';


--
-- Name: COLUMN dss_user_identity.email_address_dsc; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.email_address_dsc IS 'E-mail address associated with the user by the identity provider';


--
-- Name: COLUMN dss_user_identity.business_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.business_nm IS 'A human-readable organization name that is associated with the user by the identity provider';


--
-- Name: COLUMN dss_user_identity.terms_acceptance_dtm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.terms_acceptance_dtm IS 'A timestamp indicating when the user most recently accepted the published Terms and Conditions of application access';


--
-- Name: COLUMN dss_user_identity.represented_by_organization_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.represented_by_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_user_identity.upd_dtm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_user_identity.upd_user_guid; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_identity.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_user_identity_user_identity_id_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_user_identity ALTER COLUMN user_identity_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_user_identity_user_identity_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_user_privilege; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_user_privilege (
    user_privilege_id bigint NOT NULL,
    privilege_cd character varying(25) NOT NULL,
    privilege_nm character varying(250) NOT NULL
);


ALTER TABLE dss.dss_user_privilege OWNER TO strdssdev;

--
-- Name: TABLE dss_user_privilege; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_user_privilege IS 'A granular access right or privilege within the application that may be granted to a role';


--
-- Name: COLUMN dss_user_privilege.user_privilege_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_privilege.user_privilege_id IS 'Unique generated key';


--
-- Name: COLUMN dss_user_privilege.privilege_cd; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_privilege.privilege_cd IS 'The immutable system code that identifies the privilege';


--
-- Name: COLUMN dss_user_privilege.privilege_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_privilege.privilege_nm IS 'The human-readable name that is given for the role';


--
-- Name: dss_user_privilege_user_privilege_id_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_user_privilege ALTER COLUMN user_privilege_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_user_privilege_user_privilege_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_user_role; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_user_role (
    user_role_id bigint NOT NULL,
    user_role_cd character varying(25) NOT NULL,
    user_role_nm character varying(250) NOT NULL
);


ALTER TABLE dss.dss_user_role OWNER TO strdssdev;

--
-- Name: TABLE dss_user_role; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_user_role IS 'A set of access rights and privileges within the application that may be granted to users';


--
-- Name: COLUMN dss_user_role.user_role_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role.user_role_id IS 'Unique generated key';


--
-- Name: COLUMN dss_user_role.user_role_cd; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role.user_role_cd IS 'The immutable system code that identifies the role';


--
-- Name: COLUMN dss_user_role.user_role_nm; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role.user_role_nm IS 'The human-readable name that is given for the role';


--
-- Name: dss_user_role_assignment; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_user_role_assignment (
    user_role_assignment_id bigint NOT NULL,
    user_identity_id bigint NOT NULL,
    user_role_id bigint NOT NULL
);


ALTER TABLE dss.dss_user_role_assignment OWNER TO strdssdev;

--
-- Name: TABLE dss_user_role_assignment; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_user_role_assignment IS 'The association of a grantee credential to a role for the purpose of conveying application privileges';


--
-- Name: COLUMN dss_user_role_assignment.user_role_assignment_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role_assignment.user_role_assignment_id IS 'Unique generated key';


--
-- Name: COLUMN dss_user_role_assignment.user_identity_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role_assignment.user_identity_id IS 'Foreign key';


--
-- Name: COLUMN dss_user_role_assignment.user_role_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role_assignment.user_role_id IS 'Foreign key';


--
-- Name: dss_user_role_assignment_user_role_assignment_id_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_user_role_assignment ALTER COLUMN user_role_assignment_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_user_role_assignment_user_role_assignment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_user_role_privilege; Type: TABLE; Schema: dss; Owner: strdssdev
--

CREATE TABLE dss.dss_user_role_privilege (
    user_role_privilege_id bigint NOT NULL,
    user_privilege_id bigint NOT NULL,
    user_role_id bigint NOT NULL
);


ALTER TABLE dss.dss_user_role_privilege OWNER TO strdssdev;

--
-- Name: TABLE dss_user_role_privilege; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON TABLE dss.dss_user_role_privilege IS 'The association of a granular application privilege to a role';


--
-- Name: COLUMN dss_user_role_privilege.user_role_privilege_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role_privilege.user_role_privilege_id IS 'Unique generated key';


--
-- Name: COLUMN dss_user_role_privilege.user_privilege_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role_privilege.user_privilege_id IS 'Foreign key';


--
-- Name: COLUMN dss_user_role_privilege.user_role_id; Type: COMMENT; Schema: dss; Owner: strdssdev
--

COMMENT ON COLUMN dss.dss_user_role_privilege.user_role_id IS 'Foreign key';


--
-- Name: dss_user_role_privilege_user_role_privilege_id_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_user_role_privilege ALTER COLUMN user_role_privilege_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_user_role_privilege_user_role_privilege_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_user_role_user_role_id_seq; Type: SEQUENCE; Schema: dss; Owner: strdssdev
--

ALTER TABLE dss.dss_user_role ALTER COLUMN user_role_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME dss.dss_user_role_user_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: dss_email_message; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_email_message (email_message_id, email_message_type, message_delivery_dtm, message_template_dsc, message_reason_dsc, unreported_listing_url, host_email_address_dsc, cc_email_address_dsc, initiating_user_identity_id, affected_by_user_identity_id, involved_in_organization_id) FROM stdin;
\.


--
-- Data for Name: dss_organization; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_organization (organization_id, organization_type, organization_cd, organization_nm, local_government_geometry, managing_organization_id, upd_dtm, upd_user_guid) FROM stdin;
1	BCGov	CEU	Compliance Enforcement Unit	\N	\N	2024-03-18 14:46:54.868642+00	\N
\.


--
-- Data for Name: dss_organization_contact_person; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_organization_contact_person (organization_contact_person_id, is_primary, given_nm, family_nm, phone_no, email_address_dsc, contacted_through_organization_id, upd_dtm, upd_user_guid) FROM stdin;
\.


--
-- Data for Name: dss_user_identity; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_user_identity (user_identity_id, user_guid, display_nm, identity_provider_nm, is_enabled, access_request_status_dsc, access_request_dtm, access_request_justification_txt, given_nm, family_nm, email_address_dsc, business_nm, terms_acceptance_dtm, represented_by_organization_id, upd_dtm, upd_user_guid) FROM stdin;
\.


--
-- Data for Name: dss_user_privilege; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_user_privilege (user_privilege_id, privilege_cd, privilege_nm) FROM stdin;
1	takedown_action_write	Create Takedown Action
\.


--
-- Data for Name: dss_user_role; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_user_role (user_role_id, user_role_cd, user_role_nm) FROM stdin;
1	ceu_admin	CEU Admin
\.


--
-- Data for Name: dss_user_role_assignment; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_user_role_assignment (user_role_assignment_id, user_identity_id, user_role_id) FROM stdin;
\.


--
-- Data for Name: dss_user_role_privilege; Type: TABLE DATA; Schema: dss; Owner: strdssdev
--

COPY dss.dss_user_role_privilege (user_role_privilege_id, user_privilege_id, user_role_id) FROM stdin;
\.


--
-- Data for Name: spatial_ref_sys; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.spatial_ref_sys (srid, auth_name, auth_srid, srtext, proj4text) FROM stdin;
\.


--
-- Name: dss_email_message_email_message_id_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_email_message_email_message_id_seq', 1, false);


--
-- Name: dss_organization_contact_pers_organization_contact_person_i_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_organization_contact_pers_organization_contact_person_i_seq', 1, false);


--
-- Name: dss_organization_organization_id_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_organization_organization_id_seq', 1, true);


--
-- Name: dss_user_identity_user_identity_id_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_user_identity_user_identity_id_seq', 1, false);


--
-- Name: dss_user_privilege_user_privilege_id_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_user_privilege_user_privilege_id_seq', 1, true);


--
-- Name: dss_user_role_assignment_user_role_assignment_id_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_user_role_assignment_user_role_assignment_id_seq', 1, false);


--
-- Name: dss_user_role_privilege_user_role_privilege_id_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_user_role_privilege_user_role_privilege_id_seq', 1, false);


--
-- Name: dss_user_role_user_role_id_seq; Type: SEQUENCE SET; Schema: dss; Owner: strdssdev
--

SELECT pg_catalog.setval('dss.dss_user_role_user_role_id_seq', 1, true);


--
-- Name: dss_email_message dss_email_message_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_email_message
    ADD CONSTRAINT dss_email_message_pk PRIMARY KEY (email_message_id);


--
-- Name: dss_organization_contact_person dss_organization_contact_person_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_organization_contact_person
    ADD CONSTRAINT dss_organization_contact_person_pk PRIMARY KEY (organization_contact_person_id);


--
-- Name: dss_organization dss_organization_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_organization
    ADD CONSTRAINT dss_organization_pk PRIMARY KEY (organization_id);


--
-- Name: dss_user_identity dss_user_identity_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_identity
    ADD CONSTRAINT dss_user_identity_pk PRIMARY KEY (user_identity_id);


--
-- Name: dss_user_privilege dss_user_privilege_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_privilege
    ADD CONSTRAINT dss_user_privilege_pk PRIMARY KEY (user_privilege_id);


--
-- Name: dss_user_role_assignment dss_user_role_assignment_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_role_assignment
    ADD CONSTRAINT dss_user_role_assignment_pk PRIMARY KEY (user_role_assignment_id);


--
-- Name: dss_user_role dss_user_role_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_role
    ADD CONSTRAINT dss_user_role_pk PRIMARY KEY (user_role_id);


--
-- Name: dss_user_role_privilege dss_user_role_privilege_pk; Type: CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_role_privilege
    ADD CONSTRAINT dss_user_role_privilege_pk PRIMARY KEY (user_role_privilege_id);


--
-- Name: dss_organization dss_organization_br_iu_tr; Type: TRIGGER; Schema: dss; Owner: strdssdev
--

CREATE TRIGGER dss_organization_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_organization FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();


--
-- Name: dss_organization_contact_person dss_organization_contact_person_br_iu_tr; Type: TRIGGER; Schema: dss; Owner: strdssdev
--

CREATE TRIGGER dss_organization_contact_person_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_organization_contact_person FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();


--
-- Name: dss_user_identity dss_user_identity_br_iu_tr; Type: TRIGGER; Schema: dss; Owner: strdssdev
--

CREATE TRIGGER dss_user_identity_br_iu_tr BEFORE INSERT OR UPDATE ON dss.dss_user_identity FOR EACH ROW EXECUTE FUNCTION dss.dss_update_audit_columns();


--
-- Name: dss_email_message dss_email_message_fk_affecting; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_affecting FOREIGN KEY (affected_by_user_identity_id) REFERENCES dss.dss_user_identity(user_identity_id);


--
-- Name: dss_email_message dss_email_message_fk_initiated_by; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_initiated_by FOREIGN KEY (initiating_user_identity_id) REFERENCES dss.dss_user_identity(user_identity_id);


--
-- Name: dss_email_message dss_email_message_fk_involving; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_involving FOREIGN KEY (involved_in_organization_id) REFERENCES dss.dss_organization(organization_id);


--
-- Name: dss_organization_contact_person dss_organization_contact_person_fk_contacted_for; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_organization_contact_person
    ADD CONSTRAINT dss_organization_contact_person_fk_contacted_for FOREIGN KEY (contacted_through_organization_id) REFERENCES dss.dss_organization(organization_id);


--
-- Name: dss_organization dss_organization_fk_managed_by; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_organization
    ADD CONSTRAINT dss_organization_fk_managed_by FOREIGN KEY (managing_organization_id) REFERENCES dss.dss_organization(organization_id);


--
-- Name: dss_user_identity dss_user_identity_fk_representing; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_identity
    ADD CONSTRAINT dss_user_identity_fk_representing FOREIGN KEY (represented_by_organization_id) REFERENCES dss.dss_organization(organization_id);


--
-- Name: dss_user_role_assignment dss_user_role_assignment_fk_granted; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_role_assignment
    ADD CONSTRAINT dss_user_role_assignment_fk_granted FOREIGN KEY (user_role_id) REFERENCES dss.dss_user_role(user_role_id);


--
-- Name: dss_user_role_assignment dss_user_role_assignment_fk_granted_to; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_role_assignment
    ADD CONSTRAINT dss_user_role_assignment_fk_granted_to FOREIGN KEY (user_identity_id) REFERENCES dss.dss_user_identity(user_identity_id);


--
-- Name: dss_user_role_privilege dss_user_role_privilege_fk_conferred_by; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_role_privilege
    ADD CONSTRAINT dss_user_role_privilege_fk_conferred_by FOREIGN KEY (user_role_id) REFERENCES dss.dss_user_role(user_role_id);


--
-- Name: dss_user_role_privilege dss_user_role_privilege_fk_conferring; Type: FK CONSTRAINT; Schema: dss; Owner: strdssdev
--

ALTER TABLE ONLY dss.dss_user_role_privilege
    ADD CONSTRAINT dss_user_role_privilege_fk_conferring FOREIGN KEY (user_privilege_id) REFERENCES dss.dss_user_privilege(user_privilege_id);


--
-- PostgreSQL database dump complete
--

