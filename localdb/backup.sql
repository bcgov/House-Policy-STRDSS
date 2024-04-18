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
2	stats:deleted	11	\N
30	stats:succeeded:2024-04-15	8	2024-05-15 17:44:09.884482+00
42	stats:deleted:2024-04-15	8	2024-05-15 17:43:26.576377+00
73	stats:succeeded:2024-04-16-21	1	2024-04-17 21:11:00.239769+00
75	stats:succeeded:2024-04-16	1	2024-05-16 21:10:59.239769+00
76	stats:succeeded:2024-04-17-16	1	2024-04-18 16:29:15.303124+00
77	stats:succeeded:2024-04-17	1	2024-05-17 16:29:14.303124+00
5	stats:succeeded	22	\N
4	stats:deleted:2024-04-11	3	2024-05-11 18:34:55.520382+00
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
8	recurring-job:Process Takedown Request Batch Emails	LastExecution	1713371353242	\N	0
6	recurring-job:Process Takedown Request Batch Emails	NextExecution	1713455400000	\N	0
9	recurring-job:Process Takedown Request Batch Emails	LastJobId	34	\N	0
2	recurring-job:Process Takedown Request Batch Emails	Cron	50 15 * * *	\N	0
\.


--
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
33	98	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-16 21:10:59.1403+00	2024-04-17 21:11:00.239769+00	0
34	101	Succeeded	{"Type": "StrDss.Service.Hangfire.HangfireJobs, StrDss.Service", "Method": "ProcessTakedownRequestBatchEmails", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2024-04-17 16:29:13.315985+00	2024-04-18 16:29:15.303124+00	0
\.


--
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
133	34	RecurringJobId	"Process Takedown Request Batch Emails"	0
134	34	Time	1713371353	0
135	34	CurrentCulture	"en-CA"	0
136	34	CurrentUICulture	"en-US"	0
129	33	RecurringJobId	"Process Takedown Request Batch Emails"	0
130	33	Time	1713301859	0
131	33	CurrentCulture	"en-CA"	0
132	33	CurrentUICulture	"en-US"	0
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
cnd2214638-n:46624:73474e68-e344-4a2a-8246-fe0066c70d40	{"Queues": ["default"], "StartedAt": "2024-04-17T19:40:06.0344352Z", "WorkerCount": 1}	2024-04-17 19:43:06.251044+00	0
cnd2214638-n:25576:79e467ce-729c-4c68-b301-ff49baec0d54	{"Queues": ["default"], "StartedAt": "2024-04-17T19:43:36.8927863Z", "WorkerCount": 1}	2024-04-17 19:44:48.783287+00	0
cnd2214638-n:20328:2aa4afc1-f927-4f79-8fda-5dfc90f06a0b	{"Queues": ["default"], "StartedAt": "2024-04-17T19:45:30.7018359Z", "WorkerCount": 1}	2024-04-17 19:48:03.62583+00	0
\.


--
-- Data for Name: set; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.set (id, key, score, value, expireat, updatecount) FROM stdin;
1	recurring-jobs	1713455400	Process Takedown Request Batch Emails	\N	0
\.


--
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: strdssdev
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
99	34	Enqueued	Triggered by recurring job scheduler	2024-04-17 16:29:13.355148+00	{"Queue": "default", "EnqueuedAt": "1713371353344"}	0
101	34	Succeeded	\N	2024-04-17 16:29:15.306095+00	{"Latency": "80", "SucceededAt": "1713371355291", "PerformanceDuration": "1894"}	0
100	34	Processing	\N	2024-04-17 16:29:13.390588+00	{"ServerId": "cnd2214638-n:33668:46358936-09f2-4c9a-bbd5-4298dc646511", "WorkerId": "f526f147-990f-4258-929a-bdc57e9827d9", "StartedAt": "1713371353377"}	0
96	33	Enqueued	Triggered by recurring job scheduler	2024-04-16 21:10:59.180976+00	{"Queue": "default", "EnqueuedAt": "1713301859170"}	0
97	33	Processing	\N	2024-04-16 21:10:59.216923+00	{"ServerId": "cnd2214638-n:32996:cb347de5-45bd-40d9-a346-87ab946a4947", "WorkerId": "fbb8026e-5018-4a29-8c06-41f046eb60c4", "StartedAt": "1713301859204"}	0
98	33	Succeeded	\N	2024-04-16 21:11:00.242309+00	{"Latency": "82", "SucceededAt": "1713301860229", "PerformanceDuration": "1005"}	0
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
34	Access Requested	2024-04-11 22:26:19.391757+00	New access request has been raised and requires review. http://127.0.0.1:5174	f	f	\N	\N	\N	\N	\N	\N	\N	\N	42	42	\N	\N	\N	a1514b5c-2db1-4188-a95e-37e3ff3f3b64	2024-04-11 22:26:19.325271+00	bc3577d3-f3f8-4687-a093-4594fa43f679
53	Access Denied	2024-04-17 16:39:59.643702+00	Access to the STR Data Portal is restricted to authorized provincial and local government staff and short-term rental platforms. Please contact young-jin.1.chung@gov.bc.ca for more information.	f	f	\N	\N	\N	\N	\N	\N	\N	\N	18	47	\N	\N	\N	3837ff5b-b2bd-4fca-abce-5533e6880132	2024-04-17 16:39:59.73302+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
39	Batch Takedown Request	2024-04-15 15:27:38.96863+00	\r\nThe short-term rental listings in the attached file are the subject of a request by a local government described under s. 18(3) of the Short-Term Rental Accommodations Act and s. 16 of the Short-Term Rental Accommodations Regulation. <br/><br/>\r\nA “takedown request” (a request for a platform to cease providing platform services, e.g., removing a listing) has been submitted for each of these listings by the respective local government via the Province of British Columbia’s Short-term Rental (STR) Data Portal.<br/><br/>\r\nIn accordance with s. 18(3) of the Short-Term Rental Accommodations Act and s. 16 (3) of the Short-Term Rental Accommodations Regulation, please cease providing platform services in respect of the attached platform offers within 5 days from the date of receipt of this request.<br/><br/>\r\nFailure to comply with this request could result in enforcement actions or penalties under the Short-Term Rental Accommodations Act.<br/><br/>\r\nFor more information on these requests, or local government short-term rental business licences, please contact the local government.<br/><br/>\r\nFor more information on the Short-term Rental Accommodations Act, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a><br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n	f	f	\N	\N	\N	\N	\N	\N	\N	\N	\N	\N	2	\N	\N	43ae1eaa-5a2a-40d6-8b0e-279a08f8fb72	2024-04-15 15:27:39.537273+00	00000000-0000-0000-0000-000000000000
37	Takedown Request	2024-04-15 15:17:15.798936+00	\r\n<b>A takedown request for the following short-term rental listing was submitted to the STR Data Portal and will be delivered to the respective short-term rental platform at 11:50pm PST tonight:</b><br/><br/>\r\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nUnder the Short-Term Rental Accommodations Act and its regulations, the platform is required to comply with the request within 5 days from the date of receipt of the request. If the platform fails to comply with the request (e.g., remove the listing), local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: <a href='mailto: CEUescalations@gov.bc.ca'>CEUescalations@gov.bc.ca</a>.<br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n	f	t	\N	\N	1	\N	\N	young-jin.1.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	\N	18	\N	2	39	1	1442b233-4795-49b8-9438-4b4c4276593c	2024-04-15 15:27:39.537273+00	00000000-0000-0000-0000-000000000000
54	Access Granted	2024-04-17 16:40:19.725013+00	You have been granted access to the Short Term Rental Data Portal. Please access the portal here: http://127.0.0.1:4200. If you have any issues accessing this link, please contact young-jin.1.chung@gov.bc.ca.	f	f	\N	\N	\N	\N	\N	\N	\N	\N	18	42	\N	\N	\N	6909a946-e79b-43e6-b3d7-173faa0ce5be	2024-04-17 16:40:19.808691+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
38	Takedown Request	2024-04-15 15:21:01.965741+00	\r\nA takedown request for the following short-term rental listing was submitted to the Province of B.C.’s Short-term Rental Data Portal and will be delivered to the platform at 11:50pm PST tonight:<br/><br/>\r\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nUnder the <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'>Short-Term Rental Accommodations Act</a> and its regulations, the platform is required to comply with the request within 5 days from the date of receipt of the request. If the platform fails to comply with the request (e.g., remove the listing), local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: <a href='mailto: CEUescalations@gov.bc.ca'>CEUescalations@gov.bc.ca</a>.<br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n	f	t	\N	\N	1	\N	\N	young-jin.1.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	\N	18	\N	2	39	1	e2325ecf-cf12-4f33-bdd6-248ad2c4ac4e	2024-04-15 15:27:39.537273+00	00000000-0000-0000-0000-000000000000
41	Batch Takedown Request	2024-04-15 15:55:58.243537+00	\r\nThe short-term rental listings in the attached file are the subject of a request by a local government described under s. 18(3) of the <i>Short-Term Rental Accommodations Act</i> and s. 16 of the <i>Short-Term Rental Accommodations Regulation</i>. <br/><br/>\r\nA “takedown request” (a request for a platform to cease providing platform services, e.g., removing a listing) has been submitted for each of these listings by the respective local government via the Province of British Columbia’s Short-term Rental (STR) Data Portal.<br/><br/>\r\nIn accordance with s. 18(3) of the <i>Short-Term Rental Accommodations Act</i> and s. 16 (3) of the <i>Short-Term Rental Accommodations Regulation</i>, please cease providing platform services in respect of the attached platform offers within 5 days from the date of receipt of this request.<br/><br/>\r\nFailure to comply with this request could result in enforcement actions or penalties under the <i>Short-Term Rental Accommodations Act</i>.<br/><br/>\r\nFor more information on these requests, or local government short-term rental business licences, please contact the local government.<br/><br/>\r\nFor more information on the <i>Short-term Rental Accommodations Act</i>, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a><br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n\r\nAttachments: VXJsLExpc3RpbmdJZCxSZXF1ZXN0ZWRCeQ0KaHR0cHM6Ly93d3cuYWlyYm5iLmNvbS9yb29tcy8xNjEyMDM4OD9jaGVja19pbj0yMDI0LTA0LTI1JmFtcDtjaGVja19vdXQ9MjAyNC0wNC0yNyZhbXA7Z3Vlc3RzPTEmYW1wO2FkdWx0cz0xJmFtcDtzPTY3JmFtcDt1bmlxdWVfc2hhcmVfaWQ9ZmExMDdkMjQtYjBkZi00OWJkLTkxZTQtNTc2ZGQ3ZDM2OGFiLDEsVGVzdCBUb3duDQo=	f	f	\N	\N	\N	\N	\N	\N	\N	\N	\N	\N	2	\N	\N	e4ddde0d-c721-40a6-bb6f-3e2fe6678979	2024-04-15 15:55:58.33391+00	00000000-0000-0000-0000-000000000000
40	Takedown Request	2024-04-15 15:55:28.485897+00	\r\nA takedown request for the following short-term rental listing was submitted to the Province of B.C.’s Short-term Rental Data Portal and will be delivered to the platform at 11:50pm PST tonight:<br/><br/>\r\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nUnder the <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'>Short-Term Rental Accommodations Act</a> and its regulations, the platform is required to comply with the request within 5 days from the date of receipt of the request. If the platform fails to comply with the request (e.g., remove the listing), local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: <a href='mailto: CEUescalations@gov.bc.ca'>CEUescalations@gov.bc.ca</a>.<br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n	f	t	\N	\N	1	\N	\N	young-jin.1.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	\N	18	\N	2	41	1	be6534a0-59b9-461f-bcfa-f3944dc6b78f	2024-04-15 15:55:58.33391+00	00000000-0000-0000-0000-000000000000
43	Access Requested	2024-04-16 21:17:52.796223+00	Hello,<br/><br/>\nA new access request is waiting for you to approve.<br/><br/>\nTo approve or reject, please visit: http://127.0.0.1:5174/user-management<br/><br/>\nThank you.\n	f	f	\N	\N	\N	\N	\N	\N	\N	\N	44	44	\N	\N	\N	4369117e-28d7-4999-9b7c-39a34295df0a	2024-04-16 21:17:52.756846+00	71449ab2-0d43-48c1-94a1-2f1211ff35a4
44	Access Granted	2024-04-16 21:18:14.84659+00	You have been granted access to the Short Term Rental Data Portal. Please access the portal here: http://127.0.0.1:4200. If you have any issues accessing this link, please contact young-jin.1.chung@gov.bc.ca.	f	f	\N	\N	\N	\N	\N	\N	\N	\N	18	44	\N	\N	\N	2c38d90f-2995-430c-8bd0-26671dfaa2c4	2024-04-16 21:18:14.919887+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
49	Access Requested	2024-04-17 14:30:33.505098+00	Hello,<br/><br/>\nA new access request is waiting for you to approve.<br/><br/>\nTo approve or reject, please visit: http://127.0.0.1:5174/user-management<br/><br/>\nThank you.\n	f	f	\N	\N	\N	\N	\N	\N	\N	\N	47	47	\N	\N	\N	072047ad-2739-4d1f-8a06-f8fc9be24254	2024-04-17 14:30:25.375565+00	8cabb9b0-2381-4615-ac8f-251d26b7fe72
50	Access Denied	2024-04-17 14:31:31.268738+00	Access to the STR Data Portal is restricted to authorized provincial and local government staff and short-term rental platforms. Please contact young-jin.1.chung@gov.bc.ca for more information.	f	f	\N	\N	\N	\N	\N	\N	\N	\N	18	47	\N	\N	\N	15ba8b52-d39b-41b5-b5ef-7663bc1ed680	2024-04-17 14:31:31.450436+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
51	Batch Takedown Request	2024-04-17 16:29:14.442063+00	\r\nThe short-term rental listings in the attached file are the subject of a request by a local government described under s. 18(3) of the <i>Short-Term Rental Accommodations Act</i> and s. 16 of the <i>Short-Term Rental Accommodations Regulation</i>. <br/><br/>\r\nA “takedown request” (a request for a platform to cease providing platform services, e.g., removing a listing) has been submitted for each of these listings by the respective local government via the Province of British Columbia’s Short-term Rental (STR) Data Portal.<br/><br/>\r\nIn accordance with s. 18(3) of the <i>Short-Term Rental Accommodations Act</i> and s. 16 (3) of the <i>Short-Term Rental Accommodations Regulation</i>, please cease providing platform services in respect of the attached platform offers within 5 days from the date of receipt of this request.<br/><br/>\r\nFailure to comply with this request could result in enforcement actions or penalties under the <i>Short-Term Rental Accommodations Act</i>.<br/><br/>\r\nFor more information on these requests, or local government short-term rental business licences, please contact the local government.<br/><br/>\r\nFor more information on the <i>Short-term Rental Accommodations Act</i>, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a><br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n\r\nAttachments: VXJsLExpc3RpbmdJZCxSZXF1ZXN0ZWRCeQ0KaHR0cHM6Ly93d3cuYWlyYm5iLmNvbS9yb29tcy8xNjEyMDM4OD9jaGVja19pbj0yMDI0LTA0LTI1JmFtcDtjaGVja19vdXQ9MjAyNC0wNC0yNyZhbXA7Z3Vlc3RzPTEmYW1wO2FkdWx0cz0xJmFtcDtzPTY3JmFtcDt1bmlxdWVfc2hhcmVfaWQ9ZmExMDdkMjQtYjBkZi00OWJkLTkxZTQtNTc2ZGQ3ZDM2OGFiLDEsVGVzdCBUb3duDQpodHRwOi8vZ29vZ2xlLmNhLDEsVGVzdCBUb3duDQo=	f	f	\N	\N	\N	\N	\N	\N	\N	\N	\N	\N	2	\N	\N	eff6375c-d99e-4bad-9371-40a54fd3e75f	2024-04-17 16:29:15.22532+00	00000000-0000-0000-0000-000000000000
45	Takedown Request	2024-04-16 21:18:30.527419+00	\r\nA takedown request for the following short-term rental listing was submitted to the Province of B.C.’s Short-term Rental Data Portal and will be delivered to the platform at 11:50pm PST tonight:<br/><br/>\r\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nUnder the <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'>Short-Term Rental Accommodations Act</a> and its regulations, the platform is required to comply with the request within 5 days from the date of receipt of the request. If the platform fails to comply with the request (e.g., remove the listing), local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: <a href='mailto: CEUescalations@gov.bc.ca'>CEUescalations@gov.bc.ca</a>.<br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n	f	t	\N	\N	1	\N	\N	young-jin.1.chung@gov.bc.ca	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	\N	44	\N	2	51	1	b2eabf1d-dad1-4b62-9cf4-62f1615c11de	2024-04-17 16:29:15.22532+00	00000000-0000-0000-0000-000000000000
46	Takedown Request	2024-04-16 21:23:37.113424+00	\r\nA takedown request for the following short-term rental listing was submitted to the Province of B.C.’s Short-term Rental Data Portal and will be delivered to the platform at 11:50pm PST tonight:<br/><br/>\r\n<b>http://google.ca</b><br/><br/>\r\nListing ID Number: <b>1</b><br/><br/>\r\nUnder the <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'>Short-Term Rental Accommodations Act</a> and its regulations, the platform is required to comply with the request within 5 days from the date of receipt of the request. If the platform fails to comply with the request (e.g., remove the listing), local governments can escalate the matter to the Director of the Provincial STR Compliance and Enforcement Unit at: <a href='mailto: CEUescalations@gov.bc.ca'>CEUescalations@gov.bc.ca</a>.<br/><br/>\r\nThis email has been automatically generated. Please do not reply to this email.\r\n	f	t	\N	\N	1	\N	\N		http://google.ca	\N	44	\N	2	51	1	7b089bff-f8eb-4e60-93d2-8c1051bb4221	2024-04-17 16:29:15.22532+00	00000000-0000-0000-0000-000000000000
52	Access Requested	2024-04-17 16:39:45.207135+00	Hello,<br/><br/>\nA new access request is waiting for you to approve.<br/><br/>\nTo approve or reject, please visit: http://127.0.0.1:5174/user-management<br/><br/>\nThank you.\n	f	f	\N	\N	\N	\N	\N	\N	\N	\N	47	47	\N	\N	\N	5651010f-d1f1-4af1-88c8-f7ced304733b	2024-04-17 16:39:45.172192+00	8cabb9b0-2381-4615-ac8f-251d26b7fe72
55	Notice of Takedown	2024-04-17 16:41:33.046337+00	\nDear Host,<br/><br/>\nShort-term rental accommodations in your community are regulated by your local government. The Test Town has determined that the following short-term rental listing is not in compliance with an applicable local government business licence requirement:<br/><br/>\n<b>https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab</b><br/><br/>\nListing ID Number: <b>1</b><br/><br/>\nUnder the provincial <a href='https://www.bclaws.gov.bc.ca/civix/document/id/bills/billsprevious/4th42nd:gov35-1'>Short-Term Rental Accommodations Act</a> and its regulations, the local government may submit a request to the short-term rental platform to cease providing platform services (e.g., remove this listing from the platform and cancel all bookings) within a period of 5-90 days after the date of delivery of this Notice. Short-term rental platforms are required to comply with the local government’s request within 5 days of receiving the request.<br/><br/>\nThis Notice has been issued by Test Town.<br/><br/>\nTest comment<br/><br/>\nFor more information on this Notice, or local government short-term rental business licences, please contact your local government.<br/><br/>\nFor more information on the Short-term Rental Accommodations Act, please visit: <a href='https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals'>New rules for short-term rentals - Province of British Columbia (gov.bc.ca)</a>.<br/><br/>\r\n\nThis email has been automatically generated. Please do not reply to this email. A copy of this Notice has been sent to the short-term rental platform.<br/><br/>\n	f	t	\N	123-456-7890	1	young-jin.chung@gov.bc.ca	lgcontact@lg.ca	young-jin.chung@dxcas.com; fiona.zhou@gov.bc.ca; young-jin.chung@dxcas.com	https://www.airbnb.com/rooms/16120388?check_in=2024-04-25&amp;check_out=2024-04-27&amp;guests=1&amp;adults=1&amp;s=67&amp;unique_share_id=fa107d24-b0df-49bd-91e4-576dd7d368ab	https://bylaw.ca	44	\N	2	\N	\N	79394d7f-833c-4736-9861-11152cd01b1a	2024-04-17 16:41:33.241239+00	71449ab2-0d43-48c1-94a1-2f1211ff35a4
56	Access Requested	2024-04-17 19:41:25.776953+00	Hello,<br/><br/>\nA new access request is waiting for you to approve.<br/><br/>\nTo approve or reject, please visit: http://127.0.0.1:5174/user-management<br/><br/>\nThank you.\n	f	f	\N	\N	\N	\N	\N	\N	\N	\N	48	48	\N	\N	\N	e32d017c-f4c1-4725-9d79-24eb97f99597	2024-04-17 19:41:25.712658+00	356de3e3-43eb-46e3-a376-e265be1c054d
57	Access Denied	2024-04-17 19:42:39.854232+00	Access to the STR Data Portal is restricted to authorized provincial and local government staff and short-term rental platforms. Please contact young-jin.1.chung@gov.bc.ca for more information.	f	f	\N	\N	\N	\N	\N	\N	\N	\N	18	48	\N	\N	\N	7c60af8a-9777-4e78-b9ca-2fb9bdb01e15	2024-04-17 19:42:40.038209+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
58	Access Requested	2024-04-17 19:46:15.418077+00	Hello,<br/><br/>\nA new access request is waiting for you to approve.<br/><br/>\nTo approve or reject, please visit: http://127.0.0.1:5174/user-management<br/><br/>\nThank you.\n	f	f	\N	\N	\N	\N	\N	\N	\N	\N	48	48	\N	\N	\N	c1294821-b9c8-468f-a8b1-234482fb7c38	2024-04-17 19:46:15.379812+00	356de3e3-43eb-46e3-a376-e265be1c054d
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
24	cce5179c-35e1-493c-8599-726b1e442616	Bet, Jeroen 1 HOUS:EX	idir	t	Approved	\N	\N	Jeroen	Bet	jeroen.1.bet@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
25	27b60088-dbed-4bc5-90cc-29b6573083f6	Bohuslavskyi, Oleksandr HOUS:EX	idir	t	Approved	\N	\N	Oleksandr	Bohuslavskyi	oleksandr.bohuslavskyi@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
26	90183f0d-a629-4529-8e52-16a42bc655ca	Dudin, Oleksii HOUS:EX	idir	t	Approved	\N	\N	Oleksii	Dudin	oleksii.dudin@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
27	a793bfc0-c461-4604-b1aa-0ee01e90537f	Larsen, Leif 1 HOUS:EX	idir	t	Approved	\N	\N	Leif	Larsen	leif.1.larsen@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
28	cc8f41b9-619a-4c6b-a7e3-89740c2abd8c	Zhou, Fiona HOUS:EX	idir	t	Approved	\N	\N	Fiona	Zhou	fiona.zhou@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
47	8cabb9b0-2381-4615-ac8f-251d26b7fe72	ContactDev Airbnb	bceidbusiness	f	Denied	2024-04-17 16:39:45.058535+00	BCGov, Ministry of Housing	ContactDev	Airbnb	fiona.zhou@gov.bc.ca	Airbnb, ContactDev	\N	\N	2024-04-17 16:39:59.639963+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
42	bc3577d3-f3f8-4687-a093-4594fa43f679	Chung, Young-Jin MOTI:EX	idir	t	Approved	2024-04-11 22:26:19.167586+00	BCGov, Ministry of Housing	Young-Jin	Chung	young-jin.chung@gov.bc.ca		2024-04-11 22:26:19.167867+00	4	2024-04-17 16:40:19.718366+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
48	356de3e3-43eb-46e3-a376-e265be1c054d	ContactDev Victoria	bceidbusiness	f	Requested	2024-04-17 19:46:15.241178+00	BCGov, Ministry of Housing	ContactDev	Victoria	fiona.zhou@gov.bc.ca	City of Victoria	\N	\N	2024-04-17 19:46:15.379812+00	356de3e3-43eb-46e3-a376-e265be1c054d
29	9a132c8d-aa0c-4410-8fb0-7cc753db071c	Anderson, Richard HOUS:EX	idir	t	Approved	\N	\N	Richard	Anderson	richard.anderson@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
30	688661bc-a51a-4c3a-8bda-2d68e0b4d9dd	Forsyth, Lisa HOUS:EX	idir	t	Approved	\N	\N	Lisa	Forsyth	lisa.forsyth@gov.bc.ca	\N	\N	4	2024-04-05 15:03:55.155152+00	\N
18	8494b7d6-1ccf-48ff-9004-eac34ea99b63	Chung, Young-Jin 1 HOUS:EX	idir	t	Approved	2024-03-25 21:06:25.133679+00	BCGov, Ministry of Housing	Young-Jin	Chung	young-jin.1.chung@gov.bc.ca		2024-04-16 21:17:03.773409+00	4	2024-04-16 21:17:03.836845+00	8494b7d6-1ccf-48ff-9004-eac34ea99b63
44	71449ab2-0d43-48c1-94a1-2f1211ff35a4	ContactDev Vancouver	bceidbusiness	t	Approved	2024-04-16 21:17:52.711636+00	BCGov, Ministry of Housing	Contactdev Vancouver		fiona.zhou@gov.bc.ca	City of Vancouver Dev	2024-04-16 21:23:13.828116+00	1	2024-04-16 21:23:13.879707+00	71449ab2-0d43-48c1-94a1-2f1211ff35a4
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
44	lg_staff
42	ceu_staff
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

SELECT pg_catalog.setval('hangfire.aggregatedcounter_id_seq', 78, true);


--
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 108, true);


--
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 9, true);


--
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 34, true);


--
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 136, true);


--
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 34, true);


--
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 48, true);


--
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: strdssdev
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 101, true);


--
-- Name: dss_email_message_email_message_id_seq; Type: SEQUENCE SET; Schema: public; Owner: strdssdev
--

SELECT pg_catalog.setval('public.dss_email_message_email_message_id_seq', 58, true);


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

SELECT pg_catalog.setval('public.dss_user_identity_user_identity_id_seq', 48, true);


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

