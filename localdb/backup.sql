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

ALTER TABLE ONLY public.dss_user_role_privilege DROP CONSTRAINT dss_user_role_privilege_fk_conferring;
ALTER TABLE ONLY public.dss_user_role_privilege DROP CONSTRAINT dss_user_role_privilege_fk_conferred_by;
ALTER TABLE ONLY public.dss_user_role_assignment DROP CONSTRAINT dss_user_role_assignment_fk_granted_to;
ALTER TABLE ONLY public.dss_user_role_assignment DROP CONSTRAINT dss_user_role_assignment_fk_granted;
ALTER TABLE ONLY public.dss_user_identity DROP CONSTRAINT dss_user_identity_fk_representing;
ALTER TABLE ONLY public.dss_user_identity DROP CONSTRAINT dss_user_identity_fk_given;
ALTER TABLE ONLY public.dss_organization DROP CONSTRAINT dss_organization_fk_treated_as;
ALTER TABLE ONLY public.dss_organization DROP CONSTRAINT dss_organization_fk_managed_by;
ALTER TABLE ONLY public.dss_organization_contact_person DROP CONSTRAINT dss_organization_contact_person_fk_contacted_for;
ALTER TABLE ONLY public.dss_message_reason DROP CONSTRAINT dss_message_reason_fk_justifying;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_justified_by;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_involving;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_initiated_by;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_communicating;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_affecting;
ALTER TABLE ONLY hangfire.state DROP CONSTRAINT state_jobid_fkey;
ALTER TABLE ONLY hangfire.jobparameter DROP CONSTRAINT jobparameter_jobid_fkey;
DROP TRIGGER dss_user_identity_br_iu_tr ON public.dss_user_identity;
DROP TRIGGER dss_organization_contact_person_br_iu_tr ON public.dss_organization_contact_person;
DROP TRIGGER dss_organization_br_iu_tr ON public.dss_organization;
DROP INDEX hangfire.jobqueue_queue_fetchat_jobid;
DROP INDEX hangfire.ix_hangfire_state_jobid;
DROP INDEX hangfire.ix_hangfire_set_key_score;
DROP INDEX hangfire.ix_hangfire_set_expireat;
DROP INDEX hangfire.ix_hangfire_list_expireat;
DROP INDEX hangfire.ix_hangfire_jobqueue_queueandfetchedat;
DROP INDEX hangfire.ix_hangfire_jobqueue_jobidandqueue;
DROP INDEX hangfire.ix_hangfire_jobparameter_jobidandname;
DROP INDEX hangfire.ix_hangfire_job_statename;
DROP INDEX hangfire.ix_hangfire_job_expireat;
DROP INDEX hangfire.ix_hangfire_hash_expireat;
DROP INDEX hangfire.ix_hangfire_counter_key;
DROP INDEX hangfire.ix_hangfire_counter_expireat;
ALTER TABLE ONLY public.dss_user_role_privilege DROP CONSTRAINT dss_user_role_privilege_pk;
ALTER TABLE ONLY public.dss_user_role DROP CONSTRAINT dss_user_role_pk;
ALTER TABLE ONLY public.dss_user_role_assignment DROP CONSTRAINT dss_user_role_assignment_pk;
ALTER TABLE ONLY public.dss_user_privilege DROP CONSTRAINT dss_user_privilege_pk;
ALTER TABLE ONLY public.dss_user_identity DROP CONSTRAINT dss_user_identity_pk;
ALTER TABLE ONLY public.dss_organization_type DROP CONSTRAINT dss_organization_type_pk;
ALTER TABLE ONLY public.dss_organization DROP CONSTRAINT dss_organization_pk;
ALTER TABLE ONLY public.dss_organization_contact_person DROP CONSTRAINT dss_organization_contact_person_pk;
ALTER TABLE ONLY public.dss_message_reason DROP CONSTRAINT dss_message_reason_pk;
ALTER TABLE ONLY public.dss_email_message_type DROP CONSTRAINT dss_email_message_type_pk;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_pk;
ALTER TABLE ONLY public.dss_access_request_status DROP CONSTRAINT dss_access_request_status_pk;
ALTER TABLE ONLY hangfire.state DROP CONSTRAINT state_pkey;
ALTER TABLE ONLY hangfire.set DROP CONSTRAINT set_pkey;
ALTER TABLE ONLY hangfire.set DROP CONSTRAINT set_key_value_key;
ALTER TABLE ONLY hangfire.server DROP CONSTRAINT server_pkey;
ALTER TABLE ONLY hangfire.schema DROP CONSTRAINT schema_pkey;
ALTER TABLE ONLY hangfire.lock DROP CONSTRAINT lock_resource_key;
ALTER TABLE ONLY hangfire.list DROP CONSTRAINT list_pkey;
ALTER TABLE ONLY hangfire.jobqueue DROP CONSTRAINT jobqueue_pkey;
ALTER TABLE ONLY hangfire.jobparameter DROP CONSTRAINT jobparameter_pkey;
ALTER TABLE ONLY hangfire.job DROP CONSTRAINT job_pkey;
ALTER TABLE ONLY hangfire.hash DROP CONSTRAINT hash_pkey;
ALTER TABLE ONLY hangfire.hash DROP CONSTRAINT hash_key_field_key;
ALTER TABLE ONLY hangfire.counter DROP CONSTRAINT counter_pkey;
ALTER TABLE ONLY hangfire.aggregatedcounter DROP CONSTRAINT aggregatedcounter_pkey;
ALTER TABLE ONLY hangfire.aggregatedcounter DROP CONSTRAINT aggregatedcounter_key_key;
ALTER TABLE hangfire.state ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.set ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.list ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.jobqueue ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.jobparameter ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.job ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.hash ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.counter ALTER COLUMN id DROP DEFAULT;
ALTER TABLE hangfire.aggregatedcounter ALTER COLUMN id DROP DEFAULT;
DROP TABLE public.dss_user_role_privilege;
DROP TABLE public.dss_user_role_assignment;
DROP TABLE public.dss_user_role;
DROP TABLE public.dss_user_privilege;
DROP VIEW public.dss_user_identity_view;
DROP TABLE public.dss_user_identity;
DROP TABLE public.dss_organization_type;
DROP TABLE public.dss_organization_contact_person;
DROP TABLE public.dss_organization;
DROP TABLE public.dss_message_reason;
DROP TABLE public.dss_email_message_type;
DROP TABLE public.dss_email_message;
DROP TABLE public.dss_access_request_status;
DROP SEQUENCE hangfire.state_id_seq;
DROP TABLE hangfire.state;
DROP SEQUENCE hangfire.set_id_seq;
DROP TABLE hangfire.set;
DROP TABLE hangfire.server;
DROP TABLE hangfire.schema;
DROP TABLE hangfire.lock;
DROP SEQUENCE hangfire.list_id_seq;
DROP TABLE hangfire.list;
DROP SEQUENCE hangfire.jobqueue_id_seq;
DROP TABLE hangfire.jobqueue;
DROP SEQUENCE hangfire.jobparameter_id_seq;
DROP TABLE hangfire.jobparameter;
DROP SEQUENCE hangfire.job_id_seq;
DROP TABLE hangfire.job;
DROP SEQUENCE hangfire.hash_id_seq;
DROP TABLE hangfire.hash;
DROP SEQUENCE hangfire.counter_id_seq;
DROP TABLE hangfire.counter;
DROP SEQUENCE hangfire.aggregatedcounter_id_seq;
DROP TABLE hangfire.aggregatedcounter;
DROP FUNCTION public.dss_update_audit_columns();
DROP EXTENSION postgis;
DROP SCHEMA hangfire;
--
-- Name: hangfire; Type: SCHEMA; Schema: -; Owner: strdssdev
--

CREATE SCHEMA hangfire;


ALTER SCHEMA hangfire OWNER TO strdssdev;

--
-- Name: postgis; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;


--
-- Name: EXTENSION postgis; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION postgis IS 'PostGIS geometry and geography spatial types and functions';


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
-- Name: aggregatedcounter; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.aggregatedcounter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp with time zone
);


ALTER TABLE hangfire.aggregatedcounter OWNER TO strdssdev;

--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.aggregatedcounter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.aggregatedcounter_id_seq OWNER TO strdssdev;

--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.aggregatedcounter_id_seq OWNED BY hangfire.aggregatedcounter.id;


--
-- Name: counter; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.counter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp with time zone
);


ALTER TABLE hangfire.counter OWNER TO strdssdev;

--
-- Name: counter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.counter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.counter_id_seq OWNER TO strdssdev;

--
-- Name: counter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.counter_id_seq OWNED BY hangfire.counter.id;


--
-- Name: hash; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.hash (
    id bigint NOT NULL,
    key text NOT NULL,
    field text NOT NULL,
    value text,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.hash OWNER TO strdssdev;

--
-- Name: hash_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.hash_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.hash_id_seq OWNER TO strdssdev;

--
-- Name: hash_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.hash_id_seq OWNED BY hangfire.hash.id;


--
-- Name: job; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.job (
    id bigint NOT NULL,
    stateid bigint,
    statename text,
    invocationdata jsonb NOT NULL,
    arguments jsonb NOT NULL,
    createdat timestamp with time zone NOT NULL,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.job OWNER TO strdssdev;

--
-- Name: job_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.job_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.job_id_seq OWNER TO strdssdev;

--
-- Name: job_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.job_id_seq OWNED BY hangfire.job.id;


--
-- Name: jobparameter; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.jobparameter (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    value text,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobparameter OWNER TO strdssdev;

--
-- Name: jobparameter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.jobparameter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.jobparameter_id_seq OWNER TO strdssdev;

--
-- Name: jobparameter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.jobparameter_id_seq OWNED BY hangfire.jobparameter.id;


--
-- Name: jobqueue; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.jobqueue (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    queue text NOT NULL,
    fetchedat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobqueue OWNER TO strdssdev;

--
-- Name: jobqueue_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.jobqueue_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.jobqueue_id_seq OWNER TO strdssdev;

--
-- Name: jobqueue_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.jobqueue_id_seq OWNED BY hangfire.jobqueue.id;


--
-- Name: list; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.list (
    id bigint NOT NULL,
    key text NOT NULL,
    value text,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.list OWNER TO strdssdev;

--
-- Name: list_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.list_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.list_id_seq OWNER TO strdssdev;

--
-- Name: list_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.list_id_seq OWNED BY hangfire.list.id;


--
-- Name: lock; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.lock (
    resource text NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL,
    acquired timestamp with time zone
);


ALTER TABLE hangfire.lock OWNER TO strdssdev;

--
-- Name: schema; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.schema (
    version integer NOT NULL
);


ALTER TABLE hangfire.schema OWNER TO strdssdev;

--
-- Name: server; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.server (
    id text NOT NULL,
    data jsonb,
    lastheartbeat timestamp with time zone NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.server OWNER TO strdssdev;

--
-- Name: set; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.set (
    id bigint NOT NULL,
    key text NOT NULL,
    score double precision NOT NULL,
    value text NOT NULL,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.set OWNER TO strdssdev;

--
-- Name: set_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.set_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.set_id_seq OWNER TO strdssdev;

--
-- Name: set_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.set_id_seq OWNED BY hangfire.set.id;


--
-- Name: state; Type: TABLE; Schema: hangfire; Owner: strdssdev
--

CREATE TABLE hangfire.state (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    reason text,
    createdat timestamp with time zone NOT NULL,
    data jsonb,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.state OWNER TO strdssdev;

--
-- Name: state_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: strdssdev
--

CREATE SEQUENCE hangfire.state_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.state_id_seq OWNER TO strdssdev;

--
-- Name: state_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: strdssdev
--

ALTER SEQUENCE hangfire.state_id_seq OWNED BY hangfire.state.id;


--
-- Name: dss_access_request_status; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_access_request_status (
    access_request_status_cd character varying(25) NOT NULL,
    access_request_status_nm character varying(250) NOT NULL
);


ALTER TABLE public.dss_access_request_status OWNER TO strdssdev;

--
-- Name: dss_email_message; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_email_message (
    email_message_id bigint NOT NULL,
    email_message_type character varying(50) NOT NULL,
    message_delivery_dtm timestamp with time zone NOT NULL,
    message_template_dsc character varying(4000) NOT NULL,
    is_host_contacted_externally boolean NOT NULL,
    is_submitter_cc_required boolean NOT NULL,
    message_reason_id bigint,
    lg_phone_no character varying(13),
    unreported_listing_no character varying(25),
    host_email_address_dsc character varying(320),
    lg_email_address_dsc character varying(320),
    cc_email_address_dsc character varying(4000),
    unreported_listing_url character varying(4000),
    lg_str_bylaw_url character varying(4000),
    initiating_user_identity_id bigint NOT NULL,
    affected_by_user_identity_id bigint,
    involved_in_organization_id bigint
);


ALTER TABLE public.dss_email_message OWNER TO strdssdev;

--
-- Name: TABLE dss_email_message; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_email_message IS 'A message that is sent to one or more recipients via email';


--
-- Name: COLUMN dss_email_message.email_message_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.email_message_id IS 'Unique generated key';


--
-- Name: COLUMN dss_email_message.email_message_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.email_message_type IS 'Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';


--
-- Name: COLUMN dss_email_message.message_delivery_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.message_delivery_dtm IS 'A timestamp indicating when the message delivery was initiated';


--
-- Name: COLUMN dss_email_message.message_template_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.message_template_dsc IS 'The full text or template for the message that is sent';


--
-- Name: COLUMN dss_email_message.host_email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.host_email_address_dsc IS 'E-mail address of a short term rental host (directly entered by the user as a message recipient)';


--
-- Name: COLUMN dss_email_message.cc_email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.cc_email_address_dsc IS 'E-mail address of a secondary message recipient (directly entered by the user)';


--
-- Name: COLUMN dss_email_message.unreported_listing_url; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.unreported_listing_url IS 'User-provided URL for a short-term rental platform listing that is the subject of the message';


--
-- Name: COLUMN dss_email_message.initiating_user_identity_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.initiating_user_identity_id IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.affected_by_user_identity_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.affected_by_user_identity_id IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.involved_in_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.involved_in_organization_id IS 'Foreign key';


--
-- Name: dss_email_message_email_message_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_email_message ALTER COLUMN email_message_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_email_message_email_message_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_email_message_type; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_email_message_type (
    email_message_type character varying(50) NOT NULL,
    email_message_type_nm character varying(250) NOT NULL
);


ALTER TABLE public.dss_email_message_type OWNER TO strdssdev;

--
-- Name: dss_message_reason; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_message_reason (
    message_reason_id bigint NOT NULL,
    email_message_type character varying(50) NOT NULL,
    message_reason_dsc character varying(250) NOT NULL
);


ALTER TABLE public.dss_message_reason OWNER TO strdssdev;

--
-- Name: COLUMN dss_message_reason.message_reason_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_message_reason.message_reason_dsc IS 'A description of the justification for initiating a message';


--
-- Name: dss_message_reason_message_reason_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_message_reason ALTER COLUMN message_reason_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_message_reason_message_reason_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_organization; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_organization (
    organization_id bigint NOT NULL,
    organization_type character varying(25) NOT NULL,
    organization_cd character varying(25) NOT NULL,
    organization_nm character varying(250) NOT NULL,
    local_government_geometry public.geometry,
    managing_organization_id bigint,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE public.dss_organization OWNER TO strdssdev;

--
-- Name: TABLE dss_organization; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_organization IS 'A private company or governing body that plays a role in short term rental reporting or enforcement';


--
-- Name: COLUMN dss_organization.organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.organization_id IS 'Unique generated key';


--
-- Name: COLUMN dss_organization.organization_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.organization_type IS 'a level of government or business category';


--
-- Name: COLUMN dss_organization.organization_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.organization_cd IS 'An immutable system code that identifies the organization (e.g. CEU, AIRBNB)';


--
-- Name: COLUMN dss_organization.organization_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.organization_nm IS 'A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)';


--
-- Name: COLUMN dss_organization.local_government_geometry; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.local_government_geometry IS 'the shape identifying the boundaries of a local government';


--
-- Name: COLUMN dss_organization.managing_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.managing_organization_id IS 'Self-referential hierarchical foreign key';


--
-- Name: COLUMN dss_organization.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_organization.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_organization_contact_person; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_organization_contact_person (
    organization_contact_person_id bigint NOT NULL,
    is_primary boolean NOT NULL,
    given_nm character varying(25) NOT NULL,
    family_nm character varying(25) NOT NULL,
    phone_no character varying(13) NOT NULL,
    email_address_dsc character varying(320) NOT NULL,
    contacted_through_organization_id bigint NOT NULL,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE public.dss_organization_contact_person OWNER TO strdssdev;

--
-- Name: TABLE dss_organization_contact_person; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_organization_contact_person IS 'A person who has been identified as a notable contact for a particular organization';


--
-- Name: COLUMN dss_organization_contact_person.organization_contact_person_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.organization_contact_person_id IS 'Unique generated key';


--
-- Name: COLUMN dss_organization_contact_person.is_primary; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.is_primary IS 'Indicates whether the contact should receive all communications directed at the organization';


--
-- Name: COLUMN dss_organization_contact_person.given_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';


--
-- Name: COLUMN dss_organization_contact_person.family_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';


--
-- Name: COLUMN dss_organization_contact_person.phone_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.phone_no IS 'Phone number given for the contact by the organization (contains only digits)';


--
-- Name: COLUMN dss_organization_contact_person.email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.email_address_dsc IS 'E-mail address given for the contact by the organization';


--
-- Name: COLUMN dss_organization_contact_person.contacted_through_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.contacted_through_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_organization_contact_person.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_organization_contact_person.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_organization_contact_pers_organization_contact_person_i_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_organization_contact_person ALTER COLUMN organization_contact_person_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_organization_contact_pers_organization_contact_person_i_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_organization_organization_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_organization ALTER COLUMN organization_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_organization_organization_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_organization_type; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_organization_type (
    organization_type character varying(25) NOT NULL,
    organization_type_nm character varying(250) NOT NULL
);


ALTER TABLE public.dss_organization_type OWNER TO strdssdev;

--
-- Name: dss_user_identity; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_user_identity (
    user_identity_id bigint NOT NULL,
    user_guid uuid NOT NULL,
    display_nm character varying(250) NOT NULL,
    identity_provider_nm character varying(25) NOT NULL,
    is_enabled boolean NOT NULL,
    access_request_status_cd character varying(25) NOT NULL,
    access_request_dtm timestamp with time zone,
    access_request_justification_txt character varying(250),
    given_nm character varying(25),
    family_nm character varying(25),
    email_address_dsc character varying(320),
    business_nm character varying(250),
    terms_acceptance_dtm timestamp with time zone,
    represented_by_organization_id bigint,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE public.dss_user_identity OWNER TO strdssdev;

--
-- Name: TABLE dss_user_identity; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_user_identity IS 'An externally defined domain directory object representing a potential application user or group';


--
-- Name: COLUMN dss_user_identity.user_identity_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.user_identity_id IS 'Unique generated key';


--
-- Name: COLUMN dss_user_identity.user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.user_guid IS 'An immutable unique identifier assigned by the identity provider';


--
-- Name: COLUMN dss_user_identity.display_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.display_nm IS 'A human-readable full name that is assigned by the identity provider (this may include a preferred name and/or business unit name)';


--
-- Name: COLUMN dss_user_identity.identity_provider_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.identity_provider_nm IS 'A directory or domain that authenticates system users to allow access to the application or its API  (e.g. idir, bceidbusiness)';


--
-- Name: COLUMN dss_user_identity.is_enabled; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.is_enabled IS 'Indicates whether access is currently permitted using this identity';


--
-- Name: COLUMN dss_user_identity.access_request_status_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.access_request_status_cd IS 'The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied)';


--
-- Name: COLUMN dss_user_identity.access_request_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.access_request_dtm IS 'A timestamp indicating when the most recent access request was made by the user';


--
-- Name: COLUMN dss_user_identity.access_request_justification_txt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.access_request_justification_txt IS 'The most recent user-provided reason for requesting application access';


--
-- Name: COLUMN dss_user_identity.given_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.given_nm IS 'A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)';


--
-- Name: COLUMN dss_user_identity.family_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.family_nm IS 'A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)';


--
-- Name: COLUMN dss_user_identity.email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.email_address_dsc IS 'E-mail address associated with the user by the identity provider';


--
-- Name: COLUMN dss_user_identity.business_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.business_nm IS 'A human-readable organization name that is associated with the user by the identity provider';


--
-- Name: COLUMN dss_user_identity.terms_acceptance_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.terms_acceptance_dtm IS 'A timestamp indicating when the user most recently accepted the published Terms and Conditions of application access';


--
-- Name: COLUMN dss_user_identity.represented_by_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.represented_by_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_user_identity.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_user_identity.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_identity.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_user_identity_user_identity_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_user_identity ALTER COLUMN user_identity_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_user_identity_user_identity_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_user_identity_view; Type: VIEW; Schema: public; Owner: strdssdev
--

CREATE VIEW public.dss_user_identity_view AS
 SELECT u.user_identity_id,
    u.is_enabled,
    u.access_request_status_cd,
    u.access_request_dtm,
    u.access_request_justification_txt,
    u.identity_provider_nm,
    u.given_nm,
    u.family_nm,
    u.email_address_dsc,
    u.business_nm,
    u.terms_acceptance_dtm,
    u.represented_by_organization_id,
    o.organization_type,
    o.organization_cd,
    o.organization_nm,
    u.upd_dtm
   FROM (public.dss_user_identity u
     LEFT JOIN public.dss_organization o ON ((u.represented_by_organization_id = o.organization_id)));


ALTER VIEW public.dss_user_identity_view OWNER TO strdssdev;

--
-- Name: dss_user_privilege; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_user_privilege (
    user_privilege_cd character varying(25) NOT NULL,
    user_privilege_nm character varying(250) NOT NULL
);


ALTER TABLE public.dss_user_privilege OWNER TO strdssdev;

--
-- Name: TABLE dss_user_privilege; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_user_privilege IS 'A granular access right or privilege within the application that may be granted to a role';


--
-- Name: COLUMN dss_user_privilege.user_privilege_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_privilege.user_privilege_cd IS 'The immutable system code that identifies the privilege';


--
-- Name: COLUMN dss_user_privilege.user_privilege_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_privilege.user_privilege_nm IS 'The human-readable name that is given for the role';


--
-- Name: dss_user_role; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_user_role (
    user_role_cd character varying(25) NOT NULL,
    user_role_nm character varying(250) NOT NULL
);


ALTER TABLE public.dss_user_role OWNER TO strdssdev;

--
-- Name: TABLE dss_user_role; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_user_role IS 'A set of access rights and privileges within the application that may be granted to users';


--
-- Name: COLUMN dss_user_role.user_role_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_role.user_role_cd IS 'The immutable system code that identifies the role';


--
-- Name: COLUMN dss_user_role.user_role_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_role.user_role_nm IS 'The human-readable name that is given for the role';


--
-- Name: dss_user_role_assignment; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_user_role_assignment (
    user_identity_id bigint NOT NULL,
    user_role_cd character varying(25) NOT NULL
);


ALTER TABLE public.dss_user_role_assignment OWNER TO strdssdev;

--
-- Name: TABLE dss_user_role_assignment; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_user_role_assignment IS 'The association of a grantee credential to a role for the purpose of conveying application privileges';


--
-- Name: COLUMN dss_user_role_assignment.user_identity_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_role_assignment.user_identity_id IS 'Foreign key';


--
-- Name: COLUMN dss_user_role_assignment.user_role_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_role_assignment.user_role_cd IS 'Foreign key';


--
-- Name: dss_user_role_privilege; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_user_role_privilege (
    user_privilege_cd character varying(25) NOT NULL,
    user_role_cd character varying(25) NOT NULL
);


ALTER TABLE public.dss_user_role_privilege OWNER TO strdssdev;

--
-- Name: TABLE dss_user_role_privilege; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_user_role_privilege IS 'The association of a granular application privilege to a role';


--
-- Name: COLUMN dss_user_role_privilege.user_privilege_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_role_privilege.user_privilege_cd IS 'Foreign key';


--
-- Name: COLUMN dss_user_role_privilege.user_role_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_user_role_privilege.user_role_cd IS 'Foreign key';


--
-- Name: aggregatedcounter id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.aggregatedcounter ALTER COLUMN id SET DEFAULT nextval('hangfire.aggregatedcounter_id_seq'::regclass);


--
-- Name: counter id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.counter ALTER COLUMN id SET DEFAULT nextval('hangfire.counter_id_seq'::regclass);


--
-- Name: hash id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.hash ALTER COLUMN id SET DEFAULT nextval('hangfire.hash_id_seq'::regclass);


--
-- Name: job id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.job ALTER COLUMN id SET DEFAULT nextval('hangfire.job_id_seq'::regclass);


--
-- Name: jobparameter id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.jobparameter ALTER COLUMN id SET DEFAULT nextval('hangfire.jobparameter_id_seq'::regclass);


--
-- Name: jobqueue id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.jobqueue ALTER COLUMN id SET DEFAULT nextval('hangfire.jobqueue_id_seq'::regclass);


--
-- Name: list id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.list ALTER COLUMN id SET DEFAULT nextval('hangfire.list_id_seq'::regclass);


--
-- Name: set id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.set ALTER COLUMN id SET DEFAULT nextval('hangfire.set_id_seq'::regclass);


--
-- Name: state id; Type: DEFAULT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.state ALTER COLUMN id SET DEFAULT nextval('hangfire.state_id_seq'::regclass);


--
-- Data for Name: aggregatedcounter; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.aggregatedcounter (id, key, value, expireat) FROM stdin;
\.


--
-- Data for Name: counter; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.counter (id, key, value, expireat) FROM stdin;
\.


--
-- Data for Name: hash; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.hash (id, key, field, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
\.


--
-- Data for Name: jobqueue; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.jobqueue (id, jobid, queue, fetchedat, updatecount) FROM stdin;
\.


--
-- Data for Name: list; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.list (id, key, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: lock; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.lock (resource, updatecount, acquired) FROM stdin;
\.


--
-- Data for Name: schema; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.schema (version) FROM stdin;
21
\.


--
-- Data for Name: server; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.server (id, data, lastheartbeat, updatecount) FROM stdin;
4d2380d0d7d1:1:defcd173-8102-45c2-829c-b36b67f46aee	{"Queues": ["default"], "StartedAt": "2024-04-05T14:36:11.1766488Z", "WorkerCount": 1}	2024-04-05 15:12:11.458717+00	0
\.


--
-- Data for Name: set; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.set (id, key, score, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
\.


--
-- Data for Name: dss_access_request_status; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_access_request_status (access_request_status_cd, access_request_status_nm) FROM stdin;
Denied	Denied
Requested	Requested
Approved	Approved
\.


--
-- Data for Name: dss_email_message; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_email_message (email_message_id, email_message_type, message_delivery_dtm, message_template_dsc, is_host_contacted_externally, is_submitter_cc_required, message_reason_id, lg_phone_no, unreported_listing_no, host_email_address_dsc, lg_email_address_dsc, cc_email_address_dsc, unreported_listing_url, lg_str_bylaw_url, initiating_user_identity_id, affected_by_user_identity_id, involved_in_organization_id) FROM stdin;
3	Notice of Takedown	2024-04-02 15:44:58.078067+00	Dear Short-term Rental Host,<br/><br/>Short-term rental accommodations in your community must obtain a short-term rental (STR) business licence from the local government in order to operate.<br/><br/>Short-term rental accommodations are also regulated by the Province of B.C. Under the Short-term Rental Accommodations Act, short-term rental hosts in communities with a short-term rental business licence requirement must include a valid business licence number on any short-term rental listings advertised on an online platform. Short-term rental platforms are required to remove listings that do not meet this requirement if requested by the local government.<br/><br/>The short-term rental listing below is not in compliance with an applicable local government business licence requirement for the following reason:<b> Business Licence Denied</b><br/><br/>http://127.0.0.1:4200/compliance-notice<br/><br/>Listing ID Number: 1<br/><br/>Unless you are able to demonstrate compliance with the business licence requirement, this listing may be removed from the short-term rental platform after 5 days. The local government has 90 days to submit a request to takedown the listing to the platform. For more information, contact:<br/><br/>Email: young-jin.chung@gov.bc.ca<br/>Phone: 999-999-9999	f	t	3	999-999-9999	1	young-jin.chung@dxc.com	young-jin.chung@gov.bc.ca	young-jin.chung@gov.bc.ca	http://127.0.0.1:4200/compliance-notice		22	\N	2
4	Takedown Request	2024-04-02 15:45:07.942663+00	<br/><br/>Request to platform service provider for takedown of non-compliant platform offering.<br/><br/>The following short-term rental listing is not in compliance with an applicable local government business licence requirement:<br/><br/>http://127.0.0.1:4200/compliance-notice<br/><br/>Listing ID Number: 1<br/><br/>In accordance, with 17(2) of the Short-term Rental Accommodations Act, please cease providing platform services in respect of the above platform offer within 3 days.<br/><br/>[Name]<br/>[Title]<br/>[Local government]<br/>[Contact Information]	f	t	\N	\N	1	\N	\N	young-jin.chung@gov.bc.ca	http://127.0.0.1:4200/compliance-notice	\N	22	\N	2
5	Access Denied	2024-04-02 15:45:56.384181+00	Access to the STR Data Portal is restricted to authorized provincial and local government staff and short-term rental platforms. Please contact young-jin.chung@gov.bc.ca for more information.	f	f	\N	\N	\N	\N	\N	\N	\N	\N	22	14	\N
6	Access Granted	2024-04-02 15:46:28.180801+00	You have been granted access to the Short Term Rental Data Portal. Please access the portal here: http://127.0.0.1:4200. If you have any issues accessing this link, please contact young-jin.chung@gov.bc.ca.	f	f	\N	\N	\N	\N	\N	\N	\N	\N	22	12	\N
7	Notice of Takedown	2024-04-03 14:36:35.711403+00	\r\nDear Short-term Rental Host,<br/><br/>\r\nShort-term rental accommodations in your community must obtain a short-term rental (STR) business licence from the local government in order to operate.<br/><br/>\r\nShort-term rental accommodations are also regulated by the Province of B.C. Under the Short-term Rental Accommodations Act, short-term rental hosts in communities with a short-term rental business licence requirement must include a valid business licence number on any short-term rental listings advertised on an online platform. Short-term rental platforms are required to remove listings that do not meet this requirement if requested by the local government.<br/><br/>\r\nThe short-term rental listing below is not in compliance with an applicable local government business licence requirement for the following reason: <b>Expired Business Licence</b><br/><br/>\r\nhttps://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab<br/><br/>\r\nListing ID Number: 1<br/><br/>\r\nUnless you are able to demonstrate compliance with the business licence requirement, this listing may be removed from the short-term rental platform after 5 days. The local government has 90 days to submit a request to takedown the listing to the platform.<br/><br/>\r\nFor more information, contact:<br/><br/>\r\n<b>lgcontact@lg.ca<br/><br/></b>\r\n<b>https://bylaw.ca<br/><br/></b>\r\n	f	t	1	123-456-7890	1	young-jin.chung@gov.bc.ca	lgcontact@lg.ca	young-jin.chung@dxcas.com; young-jin.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	https://bylaw.ca	22	\N	2
8	Notice of Takedown	2024-04-03 14:55:44.333146+00	\r\nDear Short-term Rental Host,<br/><br/>\r\nShort-term rental accommodations in your community must obtain a short-term rental (STR) business licence from the local government in order to operate.<br/><br/>\r\nShort-term rental accommodations are also regulated by the Province of B.C. Under the Short-term Rental Accommodations Act, short-term rental hosts in communities with a short-term rental business licence requirement must include a valid business licence number on any short-term rental listings advertised on an online platform. Short-term rental platforms are required to remove listings that do not meet this requirement if requested by the local government.<br/><br/>\r\nThe short-term rental listing below is not in compliance with an applicable local government business licence requirement for the following reason: <b>Expired Business Licence</b><br/><br/>\r\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nUnless you are able to demonstrate compliance with the business licence requirement, this listing may be removed from the short-term rental platform after 5 days. The local government has 90 days to submit a request to takedown the listing to the platform.<br/><br/>\r\nFor more information, contact:<br/><br/>\r\n<b>lgcontact@lg.ca<br/><br/></b>\r\n<b>https://bylaw.ca<br/><br/></b>\r\nTest comment<br/>\r\n	f	t	1	123-456-7890	1	young-jin.chung@gov.bc.ca	lgcontact@lg.ca	young-jin.chung@dxcas.com; young-jin.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	https://bylaw.ca	22	\N	2
9	Notice of Takedown	2024-04-03 15:15:08.584425+00	\r\nDear Short-term Rental Host,<br/><br/>\r\nShort-term rental accommodations in your community must obtain a short-term rental (STR) business licence from the local government in order to operate.<br/><br/>\r\nShort-term rental accommodations are also regulated by the Province of B.C. Under the Short-term Rental Accommodations Act, short-term rental hosts in communities with a short-term rental business licence requirement must include a valid business licence number on any short-term rental listings advertised on an online platform. Short-term rental platforms are required to remove listings that do not meet this requirement if requested by the local government.<br/><br/>\r\nThe short-term rental listing below is not in compliance with an applicable local government business licence requirement for the following reason: <b>Expired Business Licence</b><br/><br/>\r\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nUnless you are able to demonstrate compliance with the business licence requirement, this listing may be removed from the short-term rental platform after 5 days. The local government has 90 days to submit a request to takedown the listing to the platform.<br/><br/>\r\nFor more information, contact:<br/><br/>\r\n<b>lgcontact@lg.ca<br/><br/></b>\r\n<b>https://bylaw.ca<br/><br/></b>\r\nTest comment<br/>\r\n	f	t	1	123-456-7890	1	young-jin.chung@gov.bc.ca	lgcontact@lg.ca	young-jin.chung@dxcas.com; young-jin.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	https://bylaw.ca	22	\N	2
10	Takedown Request	2024-04-03 16:54:07.650935+00	\r\n<b>Request to platform service provider for takedown of non-compliant platform offering.</b><br/><br/>\r\nThe following short-term rental listing is not in compliance with an applicable local government business licence requirement:<br/><br/>\r\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nIn accordance, with 17(2) of the Short-term Rental Accommodations Act, please cease providing platform services in respect of the above platform offer within 3 days.<br/><br/>\r\n<b>Test Town</b><br/><br/>\r\n<b>fiona.zhou@gov.bc.ca</b>\r\n	f	t	\N	\N	1	\N	\N	young-jin.1.chung@gov.bc.ca; young-jin.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	\N	22	\N	2
\.


--
-- Data for Name: dss_email_message_type; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_email_message_type (email_message_type, email_message_type_nm) FROM stdin;
Compliance Order	Provincial Compliance Order
Access Granted	Access Granted Notification
Escalation Request	STR Escalation Request
Takedown Request	Takedown Request
Access Denied	Access Denied Notification
Notice of Takedown	Notice of Takedown of Short Term Rental Platform Offer
Access Requested	Access Requested Notification
\.


--
-- Data for Name: dss_message_reason; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_message_reason (message_reason_id, email_message_type, message_reason_dsc) FROM stdin;
1	Notice of Takedown	Expired Business Licence
2	Notice of Takedown	Suspended Business Licence
3	Notice of Takedown	Business Licence Denied
4	Notice of Takedown	Invalid Business Licence Number
5	Notice of Takedown	No Business Licence Number on Listing
6	Notice of Takedown	Revoked Business Licence
\.


--
-- Data for Name: dss_organization; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_organization (organization_id, organization_type, organization_cd, organization_nm, local_government_geometry, managing_organization_id, upd_dtm, upd_user_guid) FROM stdin;
1	LG	LGTEST	Test Town	\N	\N	2024-04-02 15:27:10.684045+00	\N
2	Platform	PLATFORMTEST	Test Platform	\N	\N	2024-04-02 15:27:10.684045+00	\N
3	BCGov	BC	Other BC Government Components	\N	\N	2024-04-02 15:27:10.684045+00	\N
4	BCGov	CEU	Compliance Enforcement Unit	\N	\N	2024-04-02 15:27:10.684045+00	\N
\.


--
-- Data for Name: dss_organization_contact_person; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_organization_contact_person (organization_contact_person_id, is_primary, given_nm, family_nm, phone_no, email_address_dsc, contacted_through_organization_id, upd_dtm, upd_user_guid) FROM stdin;
1	t	John	Doe		young-jin.chung@dxcas.com	2	2024-03-25 16:25:03.665664+00	550e8400-e29b-41d4-a716-446655440014
2	t	Test	Contact	123-456-7890	fiona.zhou@gov.bc.ca	1	2024-04-02 15:25:18.495859+00	\N
3	t	Test	Contact	123-456-7890	fiona.zhou@gov.bc.ca	3	2024-04-02 15:25:18.495859+00	\N
4	t	Test	Contact	123-456-7890	fiona.zhou@gov.bc.ca	4	2024-04-02 15:25:18.495859+00	\N
\.


--
-- Data for Name: dss_organization_type; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_organization_type (organization_type, organization_type_nm) FROM stdin;
BCGov	BC Government Component
Platform	Short Term Rental Platform
LG	Local Government
\.


--
-- Data for Name: dss_user_identity; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_user_identity (user_identity_id, user_guid, display_nm, identity_provider_nm, is_enabled, access_request_status_cd, access_request_dtm, access_request_justification_txt, given_nm, family_nm, email_address_dsc, business_nm, terms_acceptance_dtm, represented_by_organization_id, upd_dtm, upd_user_guid) FROM stdin;
22	bc3577d3-f3f8-4687-a093-4594fa43f679	Chung, Young-Jin MOTI:EX	idir	t	Approved	2024-03-26 17:22:24.042276+00	BCGov, Ministry of Housing	Young-Jin	Chung	young-jin.chung@gov.bc.ca		2024-03-26 21:01:57.249773+00	4	2024-04-05 15:03:55.155152+00	bc3577d3-f3f8-4687-a093-4594fa43f679
18	8494b7d6-1ccf-48ff-9004-eac34ea99b63	Chung, Young-Jin 1 HOUS:EX	idir	t	Approved	2024-03-25 21:06:25.133679+00	BCGov, Ministry of Housing	Young-Jin	Chung	young-jin.1.chung@gov.bc.ca		\N	4	2024-04-05 15:03:55.155152+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
24	cce5179c-35e1-493c-8599-726b1e442616	Bet, Jeroen 1 HOUS:EX	idir	t	Approved	\N	\N	Jeroen	Bet	jeroen.1.bet@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
25	27b60088-dbed-4bc5-90cc-29b6573083f6	Bohuslavskyi, Oleksandr HOUS:EX	idir	t	Approved	\N	\N	Oleksandr	Bohuslavskyi	oleksandr.bohuslavskyi@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
26	90183f0d-a629-4529-8e52-16a42bc655ca	Dudin, Oleksii HOUS:EX	idir	t	Approved	\N	\N	Oleksii	Dudin	oleksii.dudin@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
14	550e8400-e29b-41d4-a716-446655440013	User14 Display Name	idir	f	Denied	\N	BCGov, Ministry of Housing	User14 Given Name	User14 Family Name	user14@example.com	User14 Business Name	\N	\N	2024-04-02 15:45:56.165546+00	bc3577d3-f3f8-4687-a093-4594fa43f679
12	550e8400-e29b-41d4-a716-446655440011	User12 Display Name	idir	t	Approved	\N	BCGov, Ministry of Housing	User12 Given Name	User12 Family Name	user12@example.com	User12 Business Name	\N	4	2024-04-02 15:46:27.931309+00	bc3577d3-f3f8-4687-a093-4594fa43f679
27	a793bfc0-c461-4604-b1aa-0ee01e90537f	Larsen, Leif 1 HOUS:EX	idir	t	Approved	\N	\N	Leif	Larsen	leif.1.larsen@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
28	cc8f41b9-619a-4c6b-a7e3-89740c2abd8c	Zhou, Fiona HOUS:EX	idir	t	Approved	\N	\N	Fiona	Zhou	fiona.zhou@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
29	9a132c8d-aa0c-4410-8fb0-7cc753db071c	Anderson, Richard HOUS:EX	idir	t	Approved	\N	\N	Richard	Anderson	richard.anderson@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
30	688661bc-a51a-4c3a-8bda-2d68e0b4d9dd	Forsyth, Lisa HOUS:EX	idir	t	Approved	\N	\N	Lisa	Forsyth	lisa.forsyth@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
\.


--
-- Data for Name: dss_user_privilege; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_user_privilege (user_privilege_cd, user_privilege_nm) FROM stdin;
user_read	View users
listing_read	View listings
ceu_action	Create CEU Action
takedown_action	Create Takedown Action
user_write	Manage users
listing_file_upload	Upload platform listing files
licence_file_upload	Upload business licence files
audit_read	View audit logs
\.


--
-- Data for Name: dss_user_role; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_user_role (user_role_cd, user_role_nm) FROM stdin;
ceu_staff	CEU Staff
ceu_admin	CEU Admin
bc_staff	Other Provincial Government
lg_staff	Local Government
platform_staff	Short Term Rental Platform
\.


--
-- Data for Name: dss_user_role_assignment; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_user_role_assignment (user_identity_id, user_role_cd) FROM stdin;
22	ceu_staff
12	ceu_staff
24	ceu_admin
25	ceu_admin
26	ceu_admin
24	lg_staff
25	lg_staff
26	lg_staff
24	platform_staff
25	platform_staff
26	platform_staff
18	ceu_staff
24	ceu_staff
25	ceu_staff
26	ceu_staff
27	lg_staff
27	ceu_admin
27	ceu_staff
27	platform_staff
18	ceu_admin
18	lg_staff
18	platform_staff
28	ceu_admin
29	ceu_admin
22	ceu_admin
28	ceu_staff
29	ceu_staff
28	lg_staff
29	lg_staff
22	lg_staff
28	platform_staff
29	platform_staff
22	platform_staff
30	ceu_admin
30	ceu_staff
30	lg_staff
30	platform_staff
\.


--
-- Data for Name: dss_user_role_privilege; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_user_role_privilege (user_privilege_cd, user_role_cd) FROM stdin;
ceu_action	ceu_staff
audit_read	bc_staff
ceu_action	ceu_admin
listing_read	lg_staff
takedown_action	lg_staff
listing_read	ceu_admin
user_read	ceu_admin
listing_read	ceu_staff
licence_file_upload	ceu_admin
audit_read	ceu_staff
listing_file_upload	ceu_admin
listing_read	bc_staff
user_write	ceu_admin
licence_file_upload	lg_staff
listing_file_upload	platform_staff
audit_read	lg_staff
\.


--
-- Data for Name: spatial_ref_sys; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.spatial_ref_sys (srid, auth_name, auth_srid, srtext, proj4text) FROM stdin;
\.


--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.aggregatedcounter_id_seq', 1, false);


--
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 1, false);


--
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 1, false);


--
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 1, false);


--
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 1, false);


--
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 1, false);


--
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 1, false);


--
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 1, false);


--
-- Name: dss_email_message_email_message_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_email_message_email_message_id_seq', 10, true);


--
-- Name: dss_message_reason_message_reason_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_message_reason_message_reason_id_seq', 6, true);


--
-- Name: dss_organization_contact_pers_organization_contact_person_i_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_organization_contact_pers_organization_contact_person_i_seq', 4, true);


--
-- Name: dss_organization_organization_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_organization_organization_id_seq', 4, true);


--
-- Name: dss_user_identity_user_identity_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_user_identity_user_identity_id_seq', 30, true);


--
-- Name: aggregatedcounter aggregatedcounter_key_key; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_key_key UNIQUE (key);


--
-- Name: aggregatedcounter aggregatedcounter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_pkey PRIMARY KEY (id);


--
-- Name: counter counter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);


--
-- Name: hash hash_key_field_key; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);


--
-- Name: hash hash_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);


--
-- Name: job job_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- Name: jobparameter jobparameter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);


--
-- Name: jobqueue jobqueue_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);


--
-- Name: list list_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);


--
-- Name: lock lock_resource_key; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);

ALTER TABLE ONLY hangfire.lock REPLICA IDENTITY USING INDEX lock_resource_key;


--
-- Name: schema schema_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);


--
-- Name: server server_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);


--
-- Name: set set_key_value_key; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);


--
-- Name: set set_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);


--
-- Name: state state_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);


--
-- Name: dss_access_request_status dss_access_request_status_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_access_request_status
    ADD CONSTRAINT dss_access_request_status_pk PRIMARY KEY (access_request_status_cd);


--
-- Name: dss_email_message dss_email_message_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_pk PRIMARY KEY (email_message_id);


--
-- Name: dss_email_message_type dss_email_message_type_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message_type
    ADD CONSTRAINT dss_email_message_type_pk PRIMARY KEY (email_message_type);


--
-- Name: dss_message_reason dss_message_reason_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_message_reason
    ADD CONSTRAINT dss_message_reason_pk PRIMARY KEY (message_reason_id);


--
-- Name: dss_organization_contact_person dss_organization_contact_person_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_organization_contact_person
    ADD CONSTRAINT dss_organization_contact_person_pk PRIMARY KEY (organization_contact_person_id);


--
-- Name: dss_organization dss_organization_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_organization
    ADD CONSTRAINT dss_organization_pk PRIMARY KEY (organization_id);


--
-- Name: dss_organization_type dss_organization_type_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_organization_type
    ADD CONSTRAINT dss_organization_type_pk PRIMARY KEY (organization_type);


--
-- Name: dss_user_identity dss_user_identity_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_identity
    ADD CONSTRAINT dss_user_identity_pk PRIMARY KEY (user_identity_id);


--
-- Name: dss_user_privilege dss_user_privilege_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_privilege
    ADD CONSTRAINT dss_user_privilege_pk PRIMARY KEY (user_privilege_cd);


--
-- Name: dss_user_role_assignment dss_user_role_assignment_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_role_assignment
    ADD CONSTRAINT dss_user_role_assignment_pk PRIMARY KEY (user_identity_id, user_role_cd);


--
-- Name: dss_user_role dss_user_role_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_role
    ADD CONSTRAINT dss_user_role_pk PRIMARY KEY (user_role_cd);


--
-- Name: dss_user_role_privilege dss_user_role_privilege_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_role_privilege
    ADD CONSTRAINT dss_user_role_privilege_pk PRIMARY KEY (user_privilege_cd, user_role_cd);


--
-- Name: ix_hangfire_counter_expireat; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_counter_expireat ON hangfire.counter USING btree (expireat);


--
-- Name: ix_hangfire_counter_key; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_counter_key ON hangfire.counter USING btree (key);


--
-- Name: ix_hangfire_hash_expireat; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_hash_expireat ON hangfire.hash USING btree (expireat);


--
-- Name: ix_hangfire_job_expireat; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_job_expireat ON hangfire.job USING btree (expireat);


--
-- Name: ix_hangfire_job_statename; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_job_statename ON hangfire.job USING btree (statename);


--
-- Name: ix_hangfire_jobparameter_jobidandname; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON hangfire.jobparameter USING btree (jobid, name);


--
-- Name: ix_hangfire_jobqueue_jobidandqueue; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON hangfire.jobqueue USING btree (jobid, queue);


--
-- Name: ix_hangfire_jobqueue_queueandfetchedat; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON hangfire.jobqueue USING btree (queue, fetchedat);


--
-- Name: ix_hangfire_list_expireat; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_list_expireat ON hangfire.list USING btree (expireat);


--
-- Name: ix_hangfire_set_expireat; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_set_expireat ON hangfire.set USING btree (expireat);


--
-- Name: ix_hangfire_set_key_score; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_set_key_score ON hangfire.set USING btree (key, score);


--
-- Name: ix_hangfire_state_jobid; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX ix_hangfire_state_jobid ON hangfire.state USING btree (jobid);


--
-- Name: jobqueue_queue_fetchat_jobid; Type: INDEX; Schema: hangfire; Owner: strdssdev
--

CREATE INDEX jobqueue_queue_fetchat_jobid ON hangfire.jobqueue USING btree (queue, fetchedat, jobid);


--
-- Name: dss_organization dss_organization_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_organization_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_organization FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_organization_contact_person dss_organization_contact_person_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_organization_contact_person_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_organization_contact_person FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_user_identity dss_user_identity_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_user_identity_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_user_identity FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: jobparameter jobparameter_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: state state_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: strdssdev
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: dss_email_message dss_email_message_fk_affecting; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_affecting FOREIGN KEY (affected_by_user_identity_id) REFERENCES public.dss_user_identity(user_identity_id);


--
-- Name: dss_email_message dss_email_message_fk_communicating; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_communicating FOREIGN KEY (email_message_type) REFERENCES public.dss_email_message_type(email_message_type);


--
-- Name: dss_email_message dss_email_message_fk_initiated_by; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_initiated_by FOREIGN KEY (initiating_user_identity_id) REFERENCES public.dss_user_identity(user_identity_id);


--
-- Name: dss_email_message dss_email_message_fk_involving; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_involving FOREIGN KEY (involved_in_organization_id) REFERENCES public.dss_organization(organization_id);


--
-- Name: dss_email_message dss_email_message_fk_justified_by; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_justified_by FOREIGN KEY (message_reason_id) REFERENCES public.dss_message_reason(message_reason_id);


--
-- Name: dss_message_reason dss_message_reason_fk_justifying; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_message_reason
    ADD CONSTRAINT dss_message_reason_fk_justifying FOREIGN KEY (email_message_type) REFERENCES public.dss_email_message_type(email_message_type);


--
-- Name: dss_organization_contact_person dss_organization_contact_person_fk_contacted_for; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_organization_contact_person
    ADD CONSTRAINT dss_organization_contact_person_fk_contacted_for FOREIGN KEY (contacted_through_organization_id) REFERENCES public.dss_organization(organization_id);


--
-- Name: dss_organization dss_organization_fk_managed_by; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_organization
    ADD CONSTRAINT dss_organization_fk_managed_by FOREIGN KEY (managing_organization_id) REFERENCES public.dss_organization(organization_id);


--
-- Name: dss_organization dss_organization_fk_treated_as; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_organization
    ADD CONSTRAINT dss_organization_fk_treated_as FOREIGN KEY (organization_type) REFERENCES public.dss_organization_type(organization_type);


--
-- Name: dss_user_identity dss_user_identity_fk_given; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_identity
    ADD CONSTRAINT dss_user_identity_fk_given FOREIGN KEY (access_request_status_cd) REFERENCES public.dss_access_request_status(access_request_status_cd);


--
-- Name: dss_user_identity dss_user_identity_fk_representing; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_identity
    ADD CONSTRAINT dss_user_identity_fk_representing FOREIGN KEY (represented_by_organization_id) REFERENCES public.dss_organization(organization_id);


--
-- Name: dss_user_role_assignment dss_user_role_assignment_fk_granted; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_role_assignment
    ADD CONSTRAINT dss_user_role_assignment_fk_granted FOREIGN KEY (user_role_cd) REFERENCES public.dss_user_role(user_role_cd);


--
-- Name: dss_user_role_assignment dss_user_role_assignment_fk_granted_to; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_role_assignment
    ADD CONSTRAINT dss_user_role_assignment_fk_granted_to FOREIGN KEY (user_identity_id) REFERENCES public.dss_user_identity(user_identity_id);


--
-- Name: dss_user_role_privilege dss_user_role_privilege_fk_conferred_by; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_role_privilege
    ADD CONSTRAINT dss_user_role_privilege_fk_conferred_by FOREIGN KEY (user_role_cd) REFERENCES public.dss_user_role(user_role_cd);


--
-- Name: dss_user_role_privilege dss_user_role_privilege_fk_conferring; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_user_role_privilege
    ADD CONSTRAINT dss_user_role_privilege_fk_conferring FOREIGN KEY (user_privilege_cd) REFERENCES public.dss_user_privilege(user_privilege_cd);


--
-- PostgreSQL database dump complete
--

