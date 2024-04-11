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
ALTER TABLE ONLY public.dss_rental_listing_report DROP CONSTRAINT dss_rental_listing_report_fk_provided_by;
ALTER TABLE ONLY public.dss_rental_listing_line DROP CONSTRAINT dss_rental_listing_line_fk_included_in;
ALTER TABLE ONLY public.dss_rental_listing DROP CONSTRAINT dss_rental_listing_fk_offered_by;
ALTER TABLE ONLY public.dss_rental_listing DROP CONSTRAINT dss_rental_listing_fk_located_at;
ALTER TABLE ONLY public.dss_rental_listing DROP CONSTRAINT dss_rental_listing_fk_included_in;
ALTER TABLE ONLY public.dss_rental_listing_contact DROP CONSTRAINT dss_rental_listing_contact_fk_contacted_for;
ALTER TABLE ONLY public.dss_physical_address DROP CONSTRAINT dss_physical_address_fk_contained_in;
ALTER TABLE ONLY public.dss_organization DROP CONSTRAINT dss_organization_fk_treated_as;
ALTER TABLE ONLY public.dss_organization DROP CONSTRAINT dss_organization_fk_managed_by;
ALTER TABLE ONLY public.dss_organization_contact_person DROP CONSTRAINT dss_organization_contact_person_fk_subscribed_to;
ALTER TABLE ONLY public.dss_organization_contact_person DROP CONSTRAINT dss_organization_contact_person_fk_contacted_for;
ALTER TABLE ONLY public.dss_message_reason DROP CONSTRAINT dss_message_reason_fk_justifying;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_requested_by;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_justified_by;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_involving;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_initiated_by;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_communicating;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_batched_in;
ALTER TABLE ONLY public.dss_email_message DROP CONSTRAINT dss_email_message_fk_affecting;
ALTER TABLE ONLY hangfire.state DROP CONSTRAINT state_jobid_fkey;
ALTER TABLE ONLY hangfire.jobparameter DROP CONSTRAINT jobparameter_jobid_fkey;
DROP TRIGGER dss_user_identity_br_iu_tr ON public.dss_user_identity;
DROP TRIGGER dss_rental_listing_report_br_iu_tr ON public.dss_rental_listing_report;
DROP TRIGGER dss_rental_listing_contact_br_iu_tr ON public.dss_rental_listing_contact;
DROP TRIGGER dss_rental_listing_br_iu_tr ON public.dss_rental_listing;
DROP TRIGGER dss_physical_address_br_iu_tr ON public.dss_physical_address;
DROP TRIGGER dss_organization_contact_person_br_iu_tr ON public.dss_organization_contact_person;
DROP TRIGGER dss_organization_br_iu_tr ON public.dss_organization;
DROP TRIGGER dss_email_message_br_iu_tr ON public.dss_email_message;
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
ALTER TABLE ONLY public.dss_rental_listing_report DROP CONSTRAINT dss_rental_listing_report_pk;
ALTER TABLE ONLY public.dss_rental_listing DROP CONSTRAINT dss_rental_listing_pk;
ALTER TABLE ONLY public.dss_rental_listing_line DROP CONSTRAINT dss_rental_listing_line_pk;
ALTER TABLE ONLY public.dss_rental_listing_contact DROP CONSTRAINT dss_rental_listing_contact_pk;
ALTER TABLE ONLY public.dss_physical_address DROP CONSTRAINT dss_physical_address_pk;
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
DROP TABLE public.dss_rental_listing_report;
DROP TABLE public.dss_rental_listing_line;
DROP TABLE public.dss_rental_listing_contact;
DROP TABLE public.dss_rental_listing;
DROP TABLE public.dss_physical_address;
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
-- Name: TABLE dss_access_request_status; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_access_request_status IS 'A potential status for a user access request (e.g. Requested, Approved, or Denied)';


--
-- Name: COLUMN dss_access_request_status.access_request_status_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_access_request_status.access_request_status_cd IS 'System-consistent code for the request status';


--
-- Name: COLUMN dss_access_request_status.access_request_status_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_access_request_status.access_request_status_nm IS 'Business term for the request status';


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
    lg_phone_no character varying(30),
    unreported_listing_no character varying(50),
    host_email_address_dsc character varying(320),
    lg_email_address_dsc character varying(320),
    cc_email_address_dsc character varying(4000),
    unreported_listing_url character varying(4000),
    lg_str_bylaw_url character varying(4000),
    initiating_user_identity_id bigint,
    affected_by_user_identity_id bigint,
    involved_in_organization_id bigint,
    batching_email_message_id bigint,
    requesting_organization_id bigint,
    external_message_no character varying(50),
    upd_dtm timestamp with time zone,
    upd_user_guid uuid
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

COMMENT ON COLUMN public.dss_email_message.email_message_type IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.message_delivery_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.message_delivery_dtm IS 'A timestamp indicating when the message delivery was initiated';


--
-- Name: COLUMN dss_email_message.message_template_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.message_template_dsc IS 'The full text or template for the message that is sent';


--
-- Name: COLUMN dss_email_message.is_host_contacted_externally; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.is_host_contacted_externally IS 'Indicates whether the the property host has already been contacted by external means';


--
-- Name: COLUMN dss_email_message.is_submitter_cc_required; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.is_submitter_cc_required IS 'Indicates whether the user initiating the message should receive a copy of the email';


--
-- Name: COLUMN dss_email_message.message_reason_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.message_reason_id IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.lg_phone_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.lg_phone_no IS 'A phone number associated with a Local Government contact';


--
-- Name: COLUMN dss_email_message.unreported_listing_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.unreported_listing_no IS 'The platform issued identification number for the listing (if not included in a rental listing report)';


--
-- Name: COLUMN dss_email_message.host_email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.host_email_address_dsc IS 'E-mail address of a short term rental host (directly entered by the user as a message recipient)';


--
-- Name: COLUMN dss_email_message.lg_email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.lg_email_address_dsc IS 'E-mail address of a local government contact (directly entered by the user as a message recipient)';


--
-- Name: COLUMN dss_email_message.cc_email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.cc_email_address_dsc IS 'E-mail address of a secondary message recipient (directly entered by the user)';


--
-- Name: COLUMN dss_email_message.unreported_listing_url; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.unreported_listing_url IS 'User-provided URL for a short-term rental platform listing that is the subject of the message';


--
-- Name: COLUMN dss_email_message.lg_str_bylaw_url; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.lg_str_bylaw_url IS 'User-provided URL for a local government bylaw that is the subject of the message';


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
-- Name: COLUMN dss_email_message.batching_email_message_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.batching_email_message_id IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.requesting_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.requesting_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_email_message.external_message_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.external_message_no IS 'External identifier for tracking the message delivery progress';


--
-- Name: COLUMN dss_email_message.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_email_message.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


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
-- Name: TABLE dss_email_message_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_email_message_type IS 'The type or purpose of a system generated message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';


--
-- Name: COLUMN dss_email_message_type.email_message_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message_type.email_message_type IS 'System-consistent code for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';


--
-- Name: COLUMN dss_email_message_type.email_message_type_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_email_message_type.email_message_type_nm IS 'Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)';


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
-- Name: TABLE dss_message_reason; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_message_reason IS 'A description of the justification for initiating a message';


--
-- Name: COLUMN dss_message_reason.message_reason_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_message_reason.message_reason_id IS 'Unique generated key';


--
-- Name: COLUMN dss_message_reason.email_message_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_message_reason.email_message_type IS 'Foreign key';


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

COMMENT ON COLUMN public.dss_organization.organization_type IS 'Foreign key';


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
    is_primary boolean,
    given_nm character varying(25),
    family_nm character varying(25),
    phone_no character varying(30),
    email_address_dsc character varying(320) NOT NULL,
    contacted_through_organization_id bigint NOT NULL,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid,
    email_message_type character varying(50)
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
-- Name: COLUMN dss_organization_contact_person.email_message_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_contact_person.email_message_type IS 'Foreign key';


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
-- Name: TABLE dss_organization_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_organization_type IS 'A level of government or business category';


--
-- Name: COLUMN dss_organization_type.organization_type; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_type.organization_type IS 'System-consistent code for a level of government or business category';


--
-- Name: COLUMN dss_organization_type.organization_type_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_organization_type.organization_type_nm IS 'Business term for a level of government or business category';


--
-- Name: dss_physical_address; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_physical_address (
    physical_address_id bigint NOT NULL,
    original_address_txt character varying(250) NOT NULL,
    match_result_json json,
    match_address_txt character varying(250),
    match_score_amt smallint,
    site_no character varying(50),
    block_no character varying(50),
    location_geometry public.geometry,
    is_exempt boolean,
    containing_organization_id bigint,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE public.dss_physical_address OWNER TO strdssdev;

--
-- Name: TABLE dss_physical_address; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_physical_address IS 'A property address that includes any verifiable BC attributes';


--
-- Name: COLUMN dss_physical_address.physical_address_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.physical_address_id IS 'Unique generated key';


--
-- Name: COLUMN dss_physical_address.original_address_txt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.original_address_txt IS 'The source-provided address of a short-term rental offering';


--
-- Name: COLUMN dss_physical_address.match_result_json; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.match_result_json IS 'Full JSON result of the source address matching attempt';


--
-- Name: COLUMN dss_physical_address.match_address_txt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.match_address_txt IS 'The sanitized physical address that has been derived from the original';


--
-- Name: COLUMN dss_physical_address.match_score_amt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.match_score_amt IS 'The relative score returned from the address matching attempt';


--
-- Name: COLUMN dss_physical_address.site_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.site_no IS 'The siteID returned by the address match';


--
-- Name: COLUMN dss_physical_address.block_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.block_no IS 'The blockID returned by the address match';


--
-- Name: COLUMN dss_physical_address.location_geometry; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.location_geometry IS 'The computed location point of the matched address';


--
-- Name: COLUMN dss_physical_address.is_exempt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.is_exempt IS 'Indicates whether the address has been identified as exempt from Short Term Rental regulations';


--
-- Name: COLUMN dss_physical_address.containing_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.containing_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_physical_address.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_physical_address.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_physical_address.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_physical_address_physical_address_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_physical_address ALTER COLUMN physical_address_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_physical_address_physical_address_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_rental_listing; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_rental_listing (
    rental_listing_id bigint NOT NULL,
    platform_listing_no character varying(25) NOT NULL,
    platform_listing_url character varying(4000),
    business_licence_no character varying(25),
    bc_registry_no character varying(25),
    is_entire_unit boolean,
    available_bedrooms_qty smallint,
    nights_booked_qty smallint,
    separate_reservations_qty smallint,
    including_rental_listing_report_id bigint NOT NULL,
    offering_organization_id bigint NOT NULL,
    locating_physical_address_id bigint,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE public.dss_rental_listing OWNER TO strdssdev;

--
-- Name: TABLE dss_rental_listing; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_rental_listing IS 'A rental listing snapshot that is relevant to a specific month';


--
-- Name: COLUMN dss_rental_listing.rental_listing_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.rental_listing_id IS 'Unique generated key';


--
-- Name: COLUMN dss_rental_listing.platform_listing_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.platform_listing_no IS 'The platform issued identification number for the listing';


--
-- Name: COLUMN dss_rental_listing.platform_listing_url; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.platform_listing_url IS 'URL for the short-term rental platform listing';


--
-- Name: COLUMN dss_rental_listing.business_licence_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.business_licence_no IS 'The local government issued licence number that applies to the rental offering';


--
-- Name: COLUMN dss_rental_listing.bc_registry_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.bc_registry_no IS 'The Short Term Registry issued permit number';


--
-- Name: COLUMN dss_rental_listing.is_entire_unit; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.is_entire_unit IS 'Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom)';


--
-- Name: COLUMN dss_rental_listing.available_bedrooms_qty; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.available_bedrooms_qty IS 'The number of bedrooms in the dwelling unit that are available for short term rental';


--
-- Name: COLUMN dss_rental_listing.nights_booked_qty; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.nights_booked_qty IS 'The number of nights that short term rental accommodation services were provided during the reporting period';


--
-- Name: COLUMN dss_rental_listing.separate_reservations_qty; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.separate_reservations_qty IS 'The number of separate reservations that were taken during the reporting period';


--
-- Name: COLUMN dss_rental_listing.including_rental_listing_report_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.including_rental_listing_report_id IS 'Foreign key';


--
-- Name: COLUMN dss_rental_listing.offering_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.offering_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_rental_listing.locating_physical_address_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.locating_physical_address_id IS 'Foreign key';


--
-- Name: COLUMN dss_rental_listing.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_rental_listing.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_rental_listing_contact; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_rental_listing_contact (
    rental_listing_contact_id bigint NOT NULL,
    is_property_owner boolean NOT NULL,
    listing_contact_nbr smallint,
    supplier_host_no character varying(25),
    full_nm character varying(50),
    phone_no character varying(30),
    fax_no character varying(30),
    full_address_txt character varying(250),
    email_address_dsc character varying(320),
    contacted_through_rental_listing_id bigint NOT NULL,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE public.dss_rental_listing_contact OWNER TO strdssdev;

--
-- Name: TABLE dss_rental_listing_contact; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_rental_listing_contact IS 'A person who has been identified as a notable contact for a particular rental listing';


--
-- Name: COLUMN dss_rental_listing_contact.rental_listing_contact_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.rental_listing_contact_id IS 'Unique generated key';


--
-- Name: COLUMN dss_rental_listing_contact.is_property_owner; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.is_property_owner IS 'Indicates a person with the legal right to the unit being short-term rental';


--
-- Name: COLUMN dss_rental_listing_contact.listing_contact_nbr; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.listing_contact_nbr IS 'Indicates which of the five possible supplier hosts is represented by this contact';


--
-- Name: COLUMN dss_rental_listing_contact.supplier_host_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.supplier_host_no IS 'The platform identifier for the supplier host';


--
-- Name: COLUMN dss_rental_listing_contact.full_nm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.full_nm IS 'The full name of the contact person as inluded in the listing';


--
-- Name: COLUMN dss_rental_listing_contact.phone_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.phone_no IS 'Phone number given for the contact';


--
-- Name: COLUMN dss_rental_listing_contact.fax_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.fax_no IS 'Facsimile numbrer given for the contact';


--
-- Name: COLUMN dss_rental_listing_contact.full_address_txt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.full_address_txt IS 'Mailing address given for the contact';


--
-- Name: COLUMN dss_rental_listing_contact.email_address_dsc; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.email_address_dsc IS 'E-mail address given for the contact';


--
-- Name: COLUMN dss_rental_listing_contact.contacted_through_rental_listing_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.contacted_through_rental_listing_id IS 'Foreign key';


--
-- Name: COLUMN dss_rental_listing_contact.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_rental_listing_contact.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_contact.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_rental_listing_contact_rental_listing_contact_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_rental_listing_contact ALTER COLUMN rental_listing_contact_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_rental_listing_contact_rental_listing_contact_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_rental_listing_line; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_rental_listing_line (
    rental_listing_line_id bigint NOT NULL,
    is_validation_failure boolean NOT NULL,
    is_system_failure boolean NOT NULL,
    organization_cd character varying(25) NOT NULL,
    platform_listing_no character varying(25) NOT NULL,
    source_line_txt character varying(32000) NOT NULL,
    error_txt character varying(32000),
    including_rental_listing_report_id bigint NOT NULL
);


ALTER TABLE public.dss_rental_listing_line OWNER TO strdssdev;

--
-- Name: TABLE dss_rental_listing_line; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_rental_listing_line IS 'A rental listing report line that has been extracted from the source';


--
-- Name: COLUMN dss_rental_listing_line.rental_listing_line_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.rental_listing_line_id IS 'Unique generated key';


--
-- Name: COLUMN dss_rental_listing_line.is_validation_failure; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.is_validation_failure IS 'Indicates that there has been a validation problem that prevents successful ingestion of the rental listing';


--
-- Name: COLUMN dss_rental_listing_line.is_system_failure; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.is_system_failure IS 'Indicates that a system fault has prevented complete ingestion of the rental listing';


--
-- Name: COLUMN dss_rental_listing_line.organization_cd; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.organization_cd IS 'An immutable system code that identifies the listing organization (e.g. AIRBNB)';


--
-- Name: COLUMN dss_rental_listing_line.platform_listing_no; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.platform_listing_no IS 'The platform issued identification number for the listing';


--
-- Name: COLUMN dss_rental_listing_line.source_line_txt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.source_line_txt IS 'Full text of the report line that could not be interpreted';


--
-- Name: COLUMN dss_rental_listing_line.error_txt; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.error_txt IS 'Freeform description of the problem found while attempting to interpret the report line';


--
-- Name: COLUMN dss_rental_listing_line.including_rental_listing_report_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_line.including_rental_listing_report_id IS 'Foreign key';


--
-- Name: dss_rental_listing_line_rental_listing_line_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_rental_listing_line ALTER COLUMN rental_listing_line_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_rental_listing_line_rental_listing_line_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_rental_listing_rental_listing_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_rental_listing ALTER COLUMN rental_listing_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_rental_listing_rental_listing_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: dss_rental_listing_report; Type: TABLE; Schema: public; Owner: strdssdev
--

CREATE TABLE public.dss_rental_listing_report (
    rental_listing_report_id bigint NOT NULL,
    is_processed boolean NOT NULL,
    report_period_ym date NOT NULL,
    source_bin bytea,
    providing_organization_id bigint NOT NULL,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


ALTER TABLE public.dss_rental_listing_report OWNER TO strdssdev;

--
-- Name: TABLE dss_rental_listing_report; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON TABLE public.dss_rental_listing_report IS 'A delivery of rental listing information that is relevant to a specific month';


--
-- Name: COLUMN dss_rental_listing_report.rental_listing_report_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_report.rental_listing_report_id IS 'Unique generated key';


--
-- Name: COLUMN dss_rental_listing_report.report_period_ym; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_report.report_period_ym IS 'The month to which the listing information is relevant (always set to the first day of the month)';


--
-- Name: COLUMN dss_rental_listing_report.source_bin; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_report.source_bin IS 'The binary image of the information that was uploaded';


--
-- Name: COLUMN dss_rental_listing_report.providing_organization_id; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_report.providing_organization_id IS 'Foreign key';


--
-- Name: COLUMN dss_rental_listing_report.upd_dtm; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_report.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_rental_listing_report.upd_user_guid; Type: COMMENT; Schema: public; Owner: strdssdev
--

COMMENT ON COLUMN public.dss_rental_listing_report.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_rental_listing_report_rental_listing_report_id_seq; Type: SEQUENCE; Schema: public; Owner: strdssdev
--

ALTER TABLE public.dss_rental_listing_report ALTER COLUMN rental_listing_report_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.dss_rental_listing_report_rental_listing_report_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


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
3	stats:deleted:2024-04-11-17	1	2024-04-12 17:55:13.604416+00
1	stats:succeeded:2024-04-11-17	3	2024-04-12 17:58:44.623891+00
15	stats:deleted:2024-04-11-18	2	2024-04-12 18:34:56.520382+00
4	stats:deleted:2024-04-11	3	2024-05-11 18:34:55.520382+00
10	stats:succeeded:2024-04-11-18	9	2024-04-12 18:36:16.0147+00
2	stats:deleted	3	\N
5	stats:succeeded	12	\N
6	stats:succeeded:2024-04-11	12	2024-05-11 18:36:15.0147+00
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
1	recurring-job:Process Takedown Request Batch Emails	Queue	default	\N	0
3	recurring-job:Process Takedown Request Batch Emails	TimeZoneId	UTC	\N	0
4	recurring-job:Process Takedown Request Batch Emails	Job	{"t":"StrDss.Service.Hangfire.HangfireJobs, StrDss.Service","m":"ProcessTakedownRequestBatchEmails"}	\N	0
5	recurring-job:Process Takedown Request Batch Emails	CreatedAt	1712857641457	\N	0
7	recurring-job:Process Takedown Request Batch Emails	V	2	\N	0
8	recurring-job:Process Takedown Request Batch Emails	LastExecution	1712860571700	\N	0
9	recurring-job:Process Takedown Request Batch Emails	LastJobId	15	\N	0
2	recurring-job:Process Takedown Request Batch Emails	Cron	50 15 * * *	\N	0
6	recurring-job:Process Takedown Request Batch Emails	NextExecution	1712937000000	\N	0
\.


--
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
1	3	Deleted	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 17:47:54.028454+00	2024-04-12 17:55:13.604416+00	0
12	36	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:22:43.45081+00	2024-04-12 18:23:06.485136+00	0
14	42	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:35:11.598545+00	2024-04-12 18:35:26.301796+00	0
2	6	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 17:55:15.59473+00	2024-04-12 17:55:17.338254+00	0
13	39	Deleted	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:24:06.512758+00	2024-04-12 18:34:56.520382+00	0
3	9	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 17:56:44.995052+00	2024-04-12 17:57:38.865219+00	0
5	15	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:11:32.102007+00	2024-04-12 18:12:07.524536+00	0
4	12	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 17:58:08.899444+00	2024-04-12 17:58:44.623891+00	0
15	45	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:36:11.708779+00	2024-04-12 18:36:16.0147+00	0
6	18	Deleted	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:12:14.720048+00	2024-04-12 18:18:06.681684+00	0
7	21	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:18:46.365165+00	2024-04-12 18:18:51.404847+00	0
8	24	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:19:08.495172+00	2024-04-12 18:19:10.194231+00	0
9	27	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:21:17.663211+00	2024-04-12 18:21:18.780659+00	0
10	30	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:21:34.679216+00	2024-04-12 18:21:35.03162+00	0
11	33	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-11 18:22:02.779692+00	2024-04-12 18:22:02.978571+00	0
\.


--
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
1	1	RecurringJobId	"Process Takedown Request Batch Emails"	0
2	1	Time	1712857656	0
3	1	CurrentCulture	"en-CA"	0
4	1	CurrentUICulture	"en-US"	0
5	2	RecurringJobId	"Process Takedown Request Batch Emails"	0
6	2	Time	1712858115	0
7	2	CurrentCulture	"en-CA"	0
8	2	CurrentUICulture	"en-US"	0
9	3	RecurringJobId	"Process Takedown Request Batch Emails"	0
10	3	Time	1712858204	0
11	3	CurrentCulture	"en-CA"	0
12	3	CurrentUICulture	"en-US"	0
13	4	RecurringJobId	"Process Takedown Request Batch Emails"	0
14	4	Time	1712858288	0
15	4	CurrentCulture	"en-CA"	0
16	4	CurrentUICulture	"en-US"	0
17	5	RecurringJobId	"Process Takedown Request Batch Emails"	0
18	5	Time	1712859092	0
19	5	CurrentCulture	"en-CA"	0
20	5	CurrentUICulture	"en-US"	0
21	6	RecurringJobId	"Process Takedown Request Batch Emails"	0
22	6	Time	1712859127	0
23	6	CurrentCulture	"en-CA"	0
24	6	CurrentUICulture	"en-US"	0
25	7	RecurringJobId	"Process Takedown Request Batch Emails"	0
26	7	Time	1712859526	0
27	7	CurrentCulture	"en-CA"	0
28	7	CurrentUICulture	"en-US"	0
29	8	RecurringJobId	"Process Takedown Request Batch Emails"	0
30	8	Time	1712859548	0
31	8	CurrentCulture	"en-CA"	0
32	8	CurrentUICulture	"en-US"	0
33	9	RecurringJobId	"Process Takedown Request Batch Emails"	0
34	9	Time	1712859677	0
35	9	CurrentCulture	"en-CA"	0
36	9	CurrentUICulture	"en-US"	0
37	10	RecurringJobId	"Process Takedown Request Batch Emails"	0
38	10	Time	1712859694	0
39	10	CurrentCulture	"en-CA"	0
40	10	CurrentUICulture	"en-US"	0
41	11	RecurringJobId	"Process Takedown Request Batch Emails"	0
42	11	Time	1712859722	0
43	11	CurrentCulture	"en-CA"	0
44	11	CurrentUICulture	"en-US"	0
45	12	RecurringJobId	"Process Takedown Request Batch Emails"	0
46	12	Time	1712859763	0
47	12	CurrentCulture	"en-CA"	0
48	12	CurrentUICulture	"en-US"	0
49	13	RecurringJobId	"Process Takedown Request Batch Emails"	0
50	13	Time	1712859846	0
51	13	CurrentCulture	"en-CA"	0
52	13	CurrentUICulture	"en-US"	0
53	14	RecurringJobId	"Process Takedown Request Batch Emails"	0
54	14	Time	1712860511	0
55	14	CurrentCulture	"en-CA"	0
56	14	CurrentUICulture	"en-US"	0
57	15	RecurringJobId	"Process Takedown Request Batch Emails"	0
58	15	Time	1712860571	0
59	15	CurrentCulture	"en-CA"	0
60	15	CurrentUICulture	"en-US"	0
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
cnd2214638-n:54284:9109c3bb-a687-439a-9b64-614ed596711e	{"Queues": ["default"], "StartedAt": "2024-04-11T20:41:53.9151808Z", "WorkerCount": 1}	2024-04-11 20:45:26.655693+00	0
cnd2214638-n:47476:0b807860-a2fd-4d2f-b917-91d0a39b20d6	{"Queues": ["default"], "StartedAt": "2024-04-11T20:47:00.5248304Z", "WorkerCount": 1}	2024-04-11 20:47:30.59592+00	0
cnd2214638-n:19496:47e5f672-0c15-43a0-9824-70411d102158	{"Queues": ["default"], "StartedAt": "2024-04-11T20:48:06.2498245Z", "WorkerCount": 1}	2024-04-11 20:48:36.293226+00	0
cnd2214638-n:13724:2681d550-5bef-4987-826b-ad61b8ddca74	{"Queues": ["default"], "StartedAt": "2024-04-11T20:50:08.7500549Z", "WorkerCount": 1}	2024-04-11 20:51:38.80277+00	0
\.


--
-- Data for Name: set; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.set (id, key, score, value, expireat, updatecount) FROM stdin;
1	recurring-jobs	1712937000	Process Takedown Request Batch Emails	\N	0
\.


--
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
1	1	Enqueued	Triggered using recurring job manager	2024-04-11 17:47:54.053058+00	{"Queue": "default", "EnqueuedAt": "1712857674046"}	0
2	1	Processing	\N	2024-04-11 17:47:54.084465+00	{"ServerId": "cnd2214638-n:45932:c222be9f-867d-441f-85c8-eb700c2096bd", "WorkerId": "bebdee8b-8642-446f-8854-6f0d6f6b397a", "StartedAt": "1712857674069"}	0
3	1	Deleted	\N	2024-04-11 17:55:13.60659+00	{"DeletedAt": "1712858113577"}	0
4	2	Enqueued	Triggered by recurring job scheduler	2024-04-11 17:55:15.605703+00	{"Queue": "default", "EnqueuedAt": "1712858115604"}	0
5	2	Processing	\N	2024-04-11 17:55:15.622546+00	{"ServerId": "cnd2214638-n:58272:012e7565-ce00-42b7-9753-6a2c53afd42d", "WorkerId": "a0581633-2519-4429-a216-e8b3bb9144e2", "StartedAt": "1712858115617"}	0
6	2	Succeeded	\N	2024-04-11 17:55:17.339288+00	{"Latency": "34", "SucceededAt": "1712858117331", "PerformanceDuration": "1702"}	0
7	3	Enqueued	Triggered by recurring job scheduler	2024-04-11 17:56:45.025508+00	{"Queue": "default", "EnqueuedAt": "1712858205018"}	0
8	3	Processing	\N	2024-04-11 17:56:45.057353+00	{"ServerId": "cnd2214638-n:14340:0a897b2d-5acd-4fa0-9a9a-a19308405328", "WorkerId": "1768f50b-9979-48f5-97dc-b4fa05d55e2b", "StartedAt": "1712858205045"}	0
9	3	Succeeded	\N	2024-04-11 17:57:38.866774+00	{"Latency": "68", "SucceededAt": "1712858258853", "PerformanceDuration": "53790"}	0
10	4	Enqueued	Triggered by recurring job scheduler	2024-04-11 17:58:08.904988+00	{"Queue": "default", "EnqueuedAt": "1712858288904"}	0
11	4	Processing	\N	2024-04-11 17:58:08.917531+00	{"ServerId": "cnd2214638-n:14340:0a897b2d-5acd-4fa0-9a9a-a19308405328", "WorkerId": "1768f50b-9979-48f5-97dc-b4fa05d55e2b", "StartedAt": "1712858288913"}	0
12	4	Succeeded	\N	2024-04-11 17:58:44.625015+00	{"Latency": "23", "SucceededAt": "1712858324616", "PerformanceDuration": "35694"}	0
13	5	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:11:32.134153+00	{"Queue": "default", "EnqueuedAt": "1712859092123"}	0
14	5	Processing	\N	2024-04-11 18:11:32.16724+00	{"ServerId": "cnd2214638-n:31608:2a8ed5bc-0846-48cc-965c-fe2309d42a52", "WorkerId": "2a59c132-78a3-456f-a201-3f790ceee400", "StartedAt": "1712859092155"}	0
15	5	Succeeded	\N	2024-04-11 18:12:07.52609+00	{"Latency": "71", "SucceededAt": "1712859127514", "PerformanceDuration": "35341"}	0
16	6	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:12:14.725614+00	{"Queue": "default", "EnqueuedAt": "1712859134725"}	0
17	6	Processing	\N	2024-04-11 18:12:14.739801+00	{"ServerId": "cnd2214638-n:31608:2a8ed5bc-0846-48cc-965c-fe2309d42a52", "WorkerId": "2a59c132-78a3-456f-a201-3f790ceee400", "StartedAt": "1712859134734"}	0
18	6	Deleted	\N	2024-04-11 18:18:06.68313+00	{"DeletedAt": "1712859486666"}	0
19	7	Enqueued	Triggered using recurring job manager	2024-04-11 18:18:46.375938+00	{"Queue": "default", "EnqueuedAt": "1712859526374"}	0
20	7	Processing	\N	2024-04-11 18:18:46.390946+00	{"ServerId": "cnd2214638-n:45816:86076005-2808-45b6-bc53-ed3761ebdeab", "WorkerId": "84736896-ebcd-435b-9f91-c2cb5800452e", "StartedAt": "1712859526385"}	0
21	7	Succeeded	\N	2024-04-11 18:18:51.405826+00	{"Latency": "32", "SucceededAt": "1712859531398", "PerformanceDuration": "5001"}	0
22	8	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:19:08.501165+00	{"Queue": "default", "EnqueuedAt": "1712859548500"}	0
23	8	Processing	\N	2024-04-11 18:19:08.514392+00	{"ServerId": "cnd2214638-n:45816:86076005-2808-45b6-bc53-ed3761ebdeab", "WorkerId": "84736896-ebcd-435b-9f91-c2cb5800452e", "StartedAt": "1712859548510"}	0
24	8	Succeeded	\N	2024-04-11 18:19:10.195233+00	{"Latency": "25", "SucceededAt": "1712859550188", "PerformanceDuration": "1668"}	0
25	9	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:21:17.697441+00	{"Queue": "default", "EnqueuedAt": "1712859677685"}	0
26	9	Processing	\N	2024-04-11 18:21:17.73223+00	{"ServerId": "cnd2214638-n:56140:4017c3a8-3557-48d8-8cd4-942ca2f1caaa", "WorkerId": "79c5e716-4b0d-4fd5-b626-0ced4024183f", "StartedAt": "1712859677719"}	0
27	9	Succeeded	\N	2024-04-11 18:21:18.78211+00	{"Latency": "75", "SucceededAt": "1712859678768", "PerformanceDuration": "1029"}	0
28	10	Enqueued	Triggered using recurring job manager	2024-04-11 18:21:34.684951+00	{"Queue": "default", "EnqueuedAt": "1712859694684"}	0
29	10	Processing	\N	2024-04-11 18:21:34.69889+00	{"ServerId": "cnd2214638-n:56140:4017c3a8-3557-48d8-8cd4-942ca2f1caaa", "WorkerId": "79c5e716-4b0d-4fd5-b626-0ced4024183f", "StartedAt": "1712859694694"}	0
30	10	Succeeded	\N	2024-04-11 18:21:35.032614+00	{"Latency": "24", "SucceededAt": "1712859695025", "PerformanceDuration": "321"}	0
31	11	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:22:02.784144+00	{"Queue": "default", "EnqueuedAt": "1712859722783"}	0
32	11	Processing	\N	2024-04-11 18:22:02.797339+00	{"ServerId": "cnd2214638-n:56140:4017c3a8-3557-48d8-8cd4-942ca2f1caaa", "WorkerId": "79c5e716-4b0d-4fd5-b626-0ced4024183f", "StartedAt": "1712859722793"}	0
33	11	Succeeded	\N	2024-04-11 18:22:02.979277+00	{"Latency": "23", "SucceededAt": "1712859722973", "PerformanceDuration": "170"}	0
34	12	Enqueued	Triggered using recurring job manager	2024-04-11 18:22:43.455547+00	{"Queue": "default", "EnqueuedAt": "1712859763455"}	0
35	12	Processing	\N	2024-04-11 18:22:43.468876+00	{"ServerId": "cnd2214638-n:56140:4017c3a8-3557-48d8-8cd4-942ca2f1caaa", "WorkerId": "79c5e716-4b0d-4fd5-b626-0ced4024183f", "StartedAt": "1712859763463"}	0
36	12	Succeeded	\N	2024-04-11 18:23:06.486009+00	{"Latency": "22", "SucceededAt": "1712859786465", "PerformanceDuration": "22991"}	0
37	13	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:24:06.517294+00	{"Queue": "default", "EnqueuedAt": "1712859846517"}	0
38	13	Processing	\N	2024-04-11 18:24:06.529696+00	{"ServerId": "cnd2214638-n:56140:4017c3a8-3557-48d8-8cd4-942ca2f1caaa", "WorkerId": "79c5e716-4b0d-4fd5-b626-0ced4024183f", "StartedAt": "1712859846525"}	0
39	13	Deleted	\N	2024-04-11 18:34:56.523032+00	{"DeletedAt": "1712860496488"}	0
40	14	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:35:11.609508+00	{"Queue": "default", "EnqueuedAt": "1712860511608"}	0
41	14	Processing	\N	2024-04-11 18:35:11.62543+00	{"ServerId": "cnd2214638-n:14400:233720db-6c19-43a4-a1a5-09462d923257", "WorkerId": "569863a7-9c84-41ea-8b78-d5cc513b5e0a", "StartedAt": "1712860511620"}	0
42	14	Succeeded	\N	2024-04-11 18:35:26.30269+00	{"Latency": "32", "SucceededAt": "1712860526295", "PerformanceDuration": "14663"}	0
43	15	Enqueued	Triggered by recurring job scheduler	2024-04-11 18:36:11.715366+00	{"Queue": "default", "EnqueuedAt": "1712860571714"}	0
44	15	Processing	\N	2024-04-11 18:36:11.731649+00	{"ServerId": "cnd2214638-n:14400:233720db-6c19-43a4-a1a5-09462d923257", "WorkerId": "569863a7-9c84-41ea-8b78-d5cc513b5e0a", "StartedAt": "1712860571726"}	0
45	15	Succeeded	\N	2024-04-11 18:36:16.015644+00	{"Latency": "29", "SucceededAt": "1712860576008", "PerformanceDuration": "4270"}	0
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

COPY public.dss_email_message (email_message_id, email_message_type, message_delivery_dtm, message_template_dsc, is_host_contacted_externally, is_submitter_cc_required, message_reason_id, lg_phone_no, unreported_listing_no, host_email_address_dsc, lg_email_address_dsc, cc_email_address_dsc, unreported_listing_url, lg_str_bylaw_url, initiating_user_identity_id, affected_by_user_identity_id, involved_in_organization_id, batching_email_message_id, requesting_organization_id, external_message_no, upd_dtm, upd_user_guid) FROM stdin;
33	Access Requested	2024-04-11 20:51:36.645789+00	New access request has been raised and requires review. http://127.0.0.1:5174	f	f	\N	\N	\N	\N	\N	\N	\N	\N	41	41	\N	\N	\N	341a994e-8748-46ed-9dfe-6a781cfa9d5a	2024-04-11 20:51:36.583229+00	bc3577d3-f3f8-4687-a093-4594fa43f679
\.


--
-- Data for Name: dss_email_message_type; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_email_message_type (email_message_type, email_message_type_nm) FROM stdin;
Compliance Order	Provincial Compliance Order
Access Granted	Access Granted Notification
Escalation Request	STR Escalation Request
Takedown Request	Takedown Request Confirmation
Access Denied	Access Denied Notification
Notice of Takedown	Notice of Takedown of Short Term Rental Platform Offer
Access Requested	Access Requested Notification
Batch Takedown Request	Batch Takedown Request
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
1	LG	LGTEST	Test Town	\N	\N	2024-04-10 21:59:08.797067+00	\N
2	Platform	PLATFORMTEST	Test Platform	\N	\N	2024-04-10 21:59:08.797067+00	\N
3	BCGov	BC	Other BC Government Components	\N	\N	2024-04-10 21:59:08.797067+00	\N
4	BCGov	CEU	Compliance Enforcement Unit	\N	\N	2024-04-10 21:59:08.797067+00	\N
\.


--
-- Data for Name: dss_organization_contact_person; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_organization_contact_person (organization_contact_person_id, is_primary, given_nm, family_nm, phone_no, email_address_dsc, contacted_through_organization_id, upd_dtm, upd_user_guid, email_message_type) FROM stdin;
1	t	John	Doe		young-jin.chung@dxcas.com	2	2024-03-25 16:25:03.665664+00	550e8400-e29b-41d4-a716-446655440014	\N
2	t	Test	Contact	123-456-7890	fiona.zhou@gov.bc.ca	1	2024-04-02 15:25:18.495859+00	\N	\N
3	t	Test	Contact	123-456-7890	fiona.zhou@gov.bc.ca	3	2024-04-02 15:25:18.495859+00	\N	\N
4	t	Test	Contact	123-456-7890	fiona.zhou@gov.bc.ca	4	2024-04-02 15:25:18.495859+00	\N	\N
5	t	\N	\N	\N	fiona.zhou@gov.bc.ca	2	2024-04-10 21:59:08.803093+00	\N	Compliance Order
6	t	\N	\N	\N	fiona.zhou@gov.bc.ca	2	2024-04-10 21:59:08.803093+00	\N	Access Granted
7	t	\N	\N	\N	fiona.zhou@gov.bc.ca	2	2024-04-10 21:59:08.803093+00	\N	Escalation Request
8	t	\N	\N	\N	young-jin.chung@dxcas.com	2	2024-04-11 16:17:04.625729+00	\N	Takedown Request
9	t	\N	\N	\N	young-jin.chung@dxcas.com	2	2024-04-11 16:17:21.581579+00	\N	Access Denied
10	t	\N	\N	\N	young-jin.chung@dxcas.com	2	2024-04-11 16:17:21.584286+00	\N	Notice of Takedown
11	t	\N	\N	\N	young-jin.chung@dxcas.com	2	2024-04-11 16:17:21.586434+00	\N	Access Requested
12	t	\N	\N	\N	young-jin.chung@dxcas.com	2	2024-04-11 16:17:21.588613+00	\N	Batch Takedown Request
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
-- Data for Name: dss_physical_address; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_physical_address (physical_address_id, original_address_txt, match_result_json, match_address_txt, match_score_amt, site_no, block_no, location_geometry, is_exempt, containing_organization_id, upd_dtm, upd_user_guid) FROM stdin;
\.


--
-- Data for Name: dss_rental_listing; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_rental_listing (rental_listing_id, platform_listing_no, platform_listing_url, business_licence_no, bc_registry_no, is_entire_unit, available_bedrooms_qty, nights_booked_qty, separate_reservations_qty, including_rental_listing_report_id, offering_organization_id, locating_physical_address_id, upd_dtm, upd_user_guid) FROM stdin;
\.


--
-- Data for Name: dss_rental_listing_contact; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_rental_listing_contact (rental_listing_contact_id, is_property_owner, listing_contact_nbr, supplier_host_no, full_nm, phone_no, fax_no, full_address_txt, email_address_dsc, contacted_through_rental_listing_id, upd_dtm, upd_user_guid) FROM stdin;
\.


--
-- Data for Name: dss_rental_listing_line; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_rental_listing_line (rental_listing_line_id, is_validation_failure, is_system_failure, organization_cd, platform_listing_no, source_line_txt, error_txt, including_rental_listing_report_id) FROM stdin;
\.


--
-- Data for Name: dss_rental_listing_report; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_rental_listing_report (rental_listing_report_id, is_processed, report_period_ym, source_bin, providing_organization_id, upd_dtm, upd_user_guid) FROM stdin;
5	f	2024-03-01	\\xefbbbf7270745f706572696f642c6f72675f63642c6c697374696e675f69642c6c697374696e675f75726c2c72656e74616c5f616464726573732c6275735f6c69635f6e6f2c62635f7265675f6e6f2c69735f656e746972655f756e69742c626564726f6f6d735f7174792c6e69676874735f626f6f6b65645f7174792c7265736572766174696f6e735f7174792c70726f70657274795f686f73745f6e6d2c70726f70657274795f686f73745f656d61696c2c70726f70657274795f686f73745f70686f6e652c70726f70657274795f686f73745f6661782c70726f70657274795f686f73745f616464726573732c737570706c6965725f686f73745f315f6e6d2c737570706c6965725f686f73745f315f656d61696c2c737570706c6965725f686f73745f315f70686f6e652c737570706c6965725f686f73745f315f6661782c737570706c6965725f686f73745f315f616464726573732c737570706c6965725f686f73745f315f69642c737570706c6965725f686f73745f325f6e6d2c737570706c6965725f686f73745f325f656d61696c2c737570706c6965725f686f73745f325f70686f6e652c737570706c6965725f686f73745f325f6661782c737570706c6965725f686f73745f325f616464726573732c737570706c6965725f686f73745f325f69642c737570706c6965725f686f73745f335f6e6d2c737570706c6965725f686f73745f335f656d61696c2c737570706c6965725f686f73745f335f70686f6e652c737570706c6965725f686f73745f335f6661782c737570706c6965725f686f73745f335f616464726573732c737570706c6965725f686f73745f335f69642c737570706c6965725f686f73745f345f6e6d2c737570706c6965725f686f73745f345f656d61696c2c737570706c6965725f686f73745f345f70686f6e652c737570706c6965725f686f73745f345f6661782c737570706c6965725f686f73745f345f616464726573732c737570706c6965725f686f73745f345f69642c737570706c6965725f686f73745f355f6e6d2c737570706c6965725f686f73745f355f656d61696c2c737570706c6965725f686f73745f355f70686f6e652c737570706c6965725f686f73745f355f6661782c737570706c6965725f686f73745f355f616464726573732c737570706c6965725f686f73745f355f69640d0a323032342d30332c504c4154464f524d544553542c313030303030312c68747470733a2f2f6578616d706c652e636f6d2f313030303030312f2c22556e697420310d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420320d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223536373820416e6f74686572205761790d0a4d7920436974792c2042432022227465737422220d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030322c68747470733a2f2f6578616d706c652e636f6d2f313030303030322f2c22556e697420320d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420330d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223637383920416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030332c68747470733a2f2f6578616d706c652e636f6d2f313030303030332f2c22556e697420330d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420340d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223738393020416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030342c68747470733a2f2f6578616d706c652e636f6d2f313030303030342f2c22556e697420340d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420350d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223839303120416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030352c68747470733a2f2f6578616d706c652e636f6d2f313030303030352f2c22556e697420350d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420360d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223930313220416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030362c68747470733a2f2f6578616d706c652e636f6d2f313030303030362f2c22556e697420360d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420370d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223031323320416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030372c68747470733a2f2f6578616d706c652e636f6d2f313030303030372f2c22556e697420370d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420380d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223132333420416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030382c68747470733a2f2f6578616d706c652e636f6d2f313030303030382f2c22556e697420380d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e697420390d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223233343520416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303030392c68747470733a2f2f6578616d706c652e636f6d2f313030303030392f2c22556e697420390d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e69742031300d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223334353620416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303031302c68747470733a2f2f6578616d706c652e636f6d2f313030303031302f2c22556e69742031300d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e69742031310d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938372d3635342d333231312c223435363720416e6f74686572205761790d0a4d7920436974792c2042430d0a56305620305630222c3635343938373332312c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c0d0a323032342d30332c504c4154464f524d544553542c313030303031312c68747470733a2f2f6578616d706c652e636f6d2f313030303031312f2c22556e69742031310d0a343536204578616d706c652053742e0d0a4d7920436974792c204243222c35343332312c3938373635342c7965732c322c32342c382c54657265736120486f6d656f776e65722c7465726573612e686f6d656f776e6572406d792e656d61696c2c3132332d3435362d373839302c2c22556e69742031320d0a343536204578616d706c652053742e0d0a4d7920436974792c2042430d0a56305620305630222c426f62204c69737465722c626f622e6c6973746572406d792e656d61696c2c3938372d3635342d333231302c3938370d0a	2	2024-04-10 17:55:59.342029+00	bc3577d3-f3f8-4687-a093-4594fa43f679
\.


--
-- Data for Name: dss_user_identity; Type: TABLE DATA; Schema: public; Owner: strdssdev
--

COPY public.dss_user_identity (user_identity_id, user_guid, display_nm, identity_provider_nm, is_enabled, access_request_status_cd, access_request_dtm, access_request_justification_txt, given_nm, family_nm, email_address_dsc, business_nm, terms_acceptance_dtm, represented_by_organization_id, upd_dtm, upd_user_guid) FROM stdin;
18	8494b7d6-1ccf-48ff-9004-eac34ea99b63	Chung, Young-Jin 1 HOUS:EX	idir	t	Approved	2024-03-25 21:06:25.133679+00	BCGov, Ministry of Housing	Young-Jin	Chung	young-jin.1.chung@gov.bc.ca		\N	4	2024-04-05 15:03:55.155152+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
24	cce5179c-35e1-493c-8599-726b1e442616	Bet, Jeroen 1 HOUS:EX	idir	t	Approved	\N	\N	Jeroen	Bet	jeroen.1.bet@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
25	27b60088-dbed-4bc5-90cc-29b6573083f6	Bohuslavskyi, Oleksandr HOUS:EX	idir	t	Approved	\N	\N	Oleksandr	Bohuslavskyi	oleksandr.bohuslavskyi@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
26	90183f0d-a629-4529-8e52-16a42bc655ca	Dudin, Oleksii HOUS:EX	idir	t	Approved	\N	\N	Oleksii	Dudin	oleksii.dudin@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
27	a793bfc0-c461-4604-b1aa-0ee01e90537f	Larsen, Leif 1 HOUS:EX	idir	t	Approved	\N	\N	Leif	Larsen	leif.1.larsen@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
28	cc8f41b9-619a-4c6b-a7e3-89740c2abd8c	Zhou, Fiona HOUS:EX	idir	t	Approved	\N	\N	Fiona	Zhou	fiona.zhou@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
29	9a132c8d-aa0c-4410-8fb0-7cc753db071c	Anderson, Richard HOUS:EX	idir	t	Approved	\N	\N	Richard	Anderson	richard.anderson@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
30	688661bc-a51a-4c3a-8bda-2d68e0b4d9dd	Forsyth, Lisa HOUS:EX	idir	t	Approved	\N	\N	Lisa	Forsyth	lisa.forsyth@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
41	bc3577d3-f3f8-4687-a093-4594fa43f679	Chung, Young-Jin MOTI:EX	idir	f	Requested	2024-04-11 20:51:36.442079+00	BCGov, Ministry of Housing	Young-Jin	Chung	young-jin.chung@gov.bc.ca		2024-04-11 20:51:36.442386+00	\N	2024-04-11 20:51:36.583229+00	bc3577d3-f3f8-4687-a093-4594fa43f679
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
28	ceu_staff
29	ceu_staff
28	lg_staff
29	lg_staff
28	platform_staff
29	platform_staff
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

SELECT pg_catalog.setval('hangfire.aggregatedcounter_id_seq', 27, true);


--
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 48, true);


--
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 9, true);


--
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 15, true);


--
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 60, true);


--
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 15, true);


--
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 28, true);


--
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 45, true);


--
-- Name: dss_email_message_email_message_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_email_message_email_message_id_seq', 33, true);


--
-- Name: dss_message_reason_message_reason_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_message_reason_message_reason_id_seq', 6, true);


--
-- Name: dss_organization_contact_pers_organization_contact_person_i_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_organization_contact_pers_organization_contact_person_i_seq', 12, true);


--
-- Name: dss_organization_organization_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_organization_organization_id_seq', 4, true);


--
-- Name: dss_physical_address_physical_address_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_physical_address_physical_address_id_seq', 1, false);


--
-- Name: dss_rental_listing_contact_rental_listing_contact_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_rental_listing_contact_rental_listing_contact_id_seq', 1, false);


--
-- Name: dss_rental_listing_line_rental_listing_line_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_rental_listing_line_rental_listing_line_id_seq', 1, false);


--
-- Name: dss_rental_listing_rental_listing_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_rental_listing_rental_listing_id_seq', 1, false);


--
-- Name: dss_rental_listing_report_rental_listing_report_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_rental_listing_report_rental_listing_report_id_seq', 5, true);


--
-- Name: dss_user_identity_user_identity_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_user_identity_user_identity_id_seq', 41, true);


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
-- Name: dss_physical_address dss_physical_address_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_physical_address
    ADD CONSTRAINT dss_physical_address_pk PRIMARY KEY (physical_address_id);


--
-- Name: dss_rental_listing_contact dss_rental_listing_contact_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing_contact
    ADD CONSTRAINT dss_rental_listing_contact_pk PRIMARY KEY (rental_listing_contact_id);


--
-- Name: dss_rental_listing_line dss_rental_listing_line_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing_line
    ADD CONSTRAINT dss_rental_listing_line_pk PRIMARY KEY (rental_listing_line_id);


--
-- Name: dss_rental_listing dss_rental_listing_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing
    ADD CONSTRAINT dss_rental_listing_pk PRIMARY KEY (rental_listing_id);


--
-- Name: dss_rental_listing_report dss_rental_listing_report_pk; Type: CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing_report
    ADD CONSTRAINT dss_rental_listing_report_pk PRIMARY KEY (rental_listing_report_id);


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
-- Name: dss_email_message dss_email_message_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_email_message_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_email_message FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_organization dss_organization_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_organization_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_organization FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_organization_contact_person dss_organization_contact_person_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_organization_contact_person_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_organization_contact_person FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_physical_address dss_physical_address_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_physical_address_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_physical_address FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_rental_listing dss_rental_listing_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_rental_listing_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_rental_listing FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_rental_listing_contact dss_rental_listing_contact_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_rental_listing_contact_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_rental_listing_contact FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: dss_rental_listing_report dss_rental_listing_report_br_iu_tr; Type: TRIGGER; Schema: public; Owner: strdssdev
--

CREATE TRIGGER dss_rental_listing_report_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_rental_listing_report FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


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
-- Name: dss_email_message dss_email_message_fk_batched_in; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_batched_in FOREIGN KEY (batching_email_message_id) REFERENCES public.dss_email_message(email_message_id);


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
-- Name: dss_email_message dss_email_message_fk_requested_by; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_email_message
    ADD CONSTRAINT dss_email_message_fk_requested_by FOREIGN KEY (requesting_organization_id) REFERENCES public.dss_organization(organization_id);


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
-- Name: dss_organization_contact_person dss_organization_contact_person_fk_subscribed_to; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_organization_contact_person
    ADD CONSTRAINT dss_organization_contact_person_fk_subscribed_to FOREIGN KEY (email_message_type) REFERENCES public.dss_email_message_type(email_message_type);


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
-- Name: dss_physical_address dss_physical_address_fk_contained_in; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_physical_address
    ADD CONSTRAINT dss_physical_address_fk_contained_in FOREIGN KEY (containing_organization_id) REFERENCES public.dss_organization(organization_id);


--
-- Name: dss_rental_listing_contact dss_rental_listing_contact_fk_contacted_for; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing_contact
    ADD CONSTRAINT dss_rental_listing_contact_fk_contacted_for FOREIGN KEY (contacted_through_rental_listing_id) REFERENCES public.dss_rental_listing(rental_listing_id);


--
-- Name: dss_rental_listing dss_rental_listing_fk_included_in; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing
    ADD CONSTRAINT dss_rental_listing_fk_included_in FOREIGN KEY (including_rental_listing_report_id) REFERENCES public.dss_rental_listing_report(rental_listing_report_id);


--
-- Name: dss_rental_listing dss_rental_listing_fk_located_at; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing
    ADD CONSTRAINT dss_rental_listing_fk_located_at FOREIGN KEY (locating_physical_address_id) REFERENCES public.dss_physical_address(physical_address_id);


--
-- Name: dss_rental_listing dss_rental_listing_fk_offered_by; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing
    ADD CONSTRAINT dss_rental_listing_fk_offered_by FOREIGN KEY (offering_organization_id) REFERENCES public.dss_organization(organization_id);


--
-- Name: dss_rental_listing_line dss_rental_listing_line_fk_included_in; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing_line
    ADD CONSTRAINT dss_rental_listing_line_fk_included_in FOREIGN KEY (including_rental_listing_report_id) REFERENCES public.dss_rental_listing_report(rental_listing_report_id);


--
-- Name: dss_rental_listing_report dss_rental_listing_report_fk_provided_by; Type: FK CONSTRAINT; Schema: public; Owner: strdssdev
--

ALTER TABLE ONLY public.dss_rental_listing_report
    ADD CONSTRAINT dss_rental_listing_report_fk_provided_by FOREIGN KEY (providing_organization_id) REFERENCES public.dss_organization(organization_id);


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

