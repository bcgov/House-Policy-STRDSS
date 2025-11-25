--
-- PostgreSQL database dump
--

\restrict qbFtGTIWvifiEpqPpQgcsr2YimbDaJaBEAnUabRGqhrbB0xlkpRhmM4jXDdi0EK

-- Dumped from database version 15.13
-- Dumped by pg_dump version 15.14

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

--
-- Name: postgis; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;


--
-- Name: EXTENSION postgis; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION postgis IS 'PostGIS geometry and geography spatial types and functions';


--
-- Name: contacttype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.contacttype AS ENUM (
    'INDIVIDUAL',
    'BUSINESS'
);


--
-- Name: documenttype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.documenttype AS ENUM (
    'BC_DRIVERS_LICENSE',
    'PROPERTY_ASSESSMENT_NOTICE',
    'SPEC_TAX_CONFIRMATION',
    'HOG_DECLARATION',
    'ICBC_CERTIFICATE_OF_INSURANCE',
    'HOME_INSURANCE_SUMMARY',
    'PROPERTY_TAX_NOTICE',
    'UTILITY_BILL',
    'GOVT_OR_CROWN_CORP_OFFICIAL_NOTICE',
    'TENANCY_AGREEMENT',
    'RENT_RECEIPT_OR_BANK_STATEMENT',
    'LOCAL_GOVT_BUSINESS_LICENSE',
    'OTHERS',
    'STRATA_HOTEL_DOCUMENTATION',
    'FRACTIONAL_OWNERSHIP_AGREEMENT',
    'BCSC',
    'COMBINED_BCSC_LICENSE'
);


--
-- Name: eventname; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.eventname AS ENUM (
    'APPLICATION_SUBMITTED',
    'INVOICE_GENERATED',
    'PAYMENT_COMPLETE',
    'PENDING_AUTO_APPROVAL_PROCESSING',
    'AUTO_APPROVAL_FULL_REVIEW',
    'AUTO_APPROVAL_PROVISIONAL',
    'AUTO_APPROVAL_APPROVED',
    'FULL_REVIEW_IN_PROGRESS',
    'MANUALLY_APPROVED',
    'MANUALLY_DENIED',
    'MORE_INFORMATION_REQUESTED',
    'REGISTRATION_CREATED',
    'CERTIFICATE_ISSUED',
    'REGISTRATION_EXPIRED',
    'NON_COMPLIANCE_SUSPENDED',
    'REGISTRATION_CANCELLED',
    'APPLICATION_REVIEWER_ASSIGNED',
    'APPLICATION_REVIEWER_UNASSIGNED',
    'NOC_SENT',
    'NOC_EXPIRED',
    'HOST_APPLICATION_UNIT_ADDRESS_UPDATED',
    'HOST_REGISTRATION_UNIT_ADDRESS_UPDATED',
    'APPLICATION_DECISION_SET_ASIDE',
    'REGISTRATION_REINSTATED',
    'REGISTRATION_DECISION_SET_ASIDE',
    'REGISTRATION_DOCUMENT_UPLOADED',
    'REGISTRATION_ASSIGNEE_ASSIGNED',
    'REGISTRATION_ASSIGNEE_UNASSIGNED',
    'CONDITIONS_OF_APPROVAL_UPDATED',
    'REGISTRATION_APPROVED'
);


--
-- Name: eventtype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.eventtype AS ENUM (
    'APPLICATION',
    'REGISTRATION',
    'USER'
);


--
-- Name: hostresidence; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.hostresidence AS ENUM (
    'SAME_UNIT',
    'ANOTHER_UNIT'
);


--
-- Name: hosttype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.hosttype AS ENUM (
    'OWNER',
    'FRIEND_RELATIVE',
    'LONG_TERM_TENANT'
);


--
-- Name: listingsize; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.listingsize AS ENUM (
    'LESS_THAN_250',
    'BETWEEN_250_AND_999',
    'THOUSAND_AND_ABOVE'
);


--
-- Name: ownershiptype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.ownershiptype AS ENUM (
    'OWN',
    'CO_OWN',
    'RENT',
    'OTHER'
);


--
-- Name: propertymanagertype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.propertymanagertype AS ENUM (
    'INDIVIDUAL',
    'BUSINESS'
);


--
-- Name: propertytype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.propertytype AS ENUM (
    'SINGLE_FAMILY_HOME',
    'SECONDARY_SUITE',
    'ACCESSORY_DWELLING',
    'MULTI_UNIT_HOUSING',
    'TOWN_HOME',
    'CONDO_OR_APT',
    'RECREATIONAL',
    'BED_AND_BREAKFAST',
    'STRATA_HOTEL',
    'FLOAT_HOME'
);


--
-- Name: registrationnocstatus; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.registrationnocstatus AS ENUM (
    'NOC_PENDING',
    'NOC_EXPIRED'
);


--
-- Name: registrationstatus; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.registrationstatus AS ENUM (
    'ACTIVE',
    'EXPIRED',
    'SUSPENDED',
    'CANCELLED'
);


--
-- Name: registrationtype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.registrationtype AS ENUM (
    'HOST',
    'PLATFORM',
    'STRATA_HOTEL'
);


--
-- Name: rentalspaceoption; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.rentalspaceoption AS ENUM (
    'DIFFERENT_PROPERTY',
    'SEPARATE_UNIT_SAME_PROPERTY',
    'PRIMARY_RESIDENCE_OR_SHARED_SPACE'
);


--
-- Name: rentalunitspacetype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.rentalunitspacetype AS ENUM (
    'ENTIRE_HOME',
    'SHARED_ACCOMMODATION'
);


--
-- Name: status; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.status AS ENUM (
    'NOT_PROCESSED',
    'PROCESSING',
    'ERROR',
    'COMPLETED'
);


--
-- Name: stratahotelcategory; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.stratahotelcategory AS ENUM (
    'FULL_SERVICE',
    'MULTI_UNIT_NON_PR',
    'POST_DECEMBER_2023'
);


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: dss_organization; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.dss_organization (
    organization_id bigint NOT NULL,
    organization_type character varying(25) NOT NULL,
    organization_cd character varying(25) NOT NULL,
    organization_nm character varying(250) NOT NULL,
    is_lg_participating boolean,
    is_principal_residence_required boolean,
    is_business_licence_required boolean,
    area_geometry public.geometry,
    managing_organization_id bigint,
    upd_dtm timestamp with time zone NOT NULL,
    upd_user_guid uuid
);


--
-- Name: TABLE dss_organization; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.dss_organization IS 'A private company or governing body component that plays a role in short term rental reporting or enforcement';


--
-- Name: COLUMN dss_organization.organization_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.organization_id IS 'Unique generated key';


--
-- Name: COLUMN dss_organization.organization_type; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.organization_type IS 'Foreign key';


--
-- Name: COLUMN dss_organization.organization_cd; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.organization_cd IS 'An immutable system code that identifies the organization (e.g. CEU, AIRBNB)';


--
-- Name: COLUMN dss_organization.organization_nm; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.organization_nm IS 'A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)';


--
-- Name: COLUMN dss_organization.is_lg_participating; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.is_lg_participating IS 'Indicates whether a LOCAL GOVERNMENT ORGANIZATION participates in Short Term Rental Data Sharing';


--
-- Name: COLUMN dss_organization.is_principal_residence_required; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.is_principal_residence_required IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION is subject to Provincial Principal Residence Short Term Rental restrictions';


--
-- Name: COLUMN dss_organization.is_business_licence_required; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.is_business_licence_required IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION requires a business licence for Short Term Rental operation';


--
-- Name: COLUMN dss_organization.area_geometry; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.area_geometry IS 'the multipolygon shape identifying the boundaries of a local government subdivision';


--
-- Name: COLUMN dss_organization.managing_organization_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.managing_organization_id IS 'Self-referential hierarchical foreign key';


--
-- Name: COLUMN dss_organization.upd_dtm; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.upd_dtm IS 'Trigger-updated timestamp of last change';


--
-- Name: COLUMN dss_organization.upd_user_guid; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.dss_organization.upd_user_guid IS 'The globally unique identifier (assigned by the identity provider) for the most recent user to record a change';


--
-- Name: dss_containing_organization_id(public.geometry); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.dss_containing_organization_id(p_point public.geometry) RETURNS bigint
    LANGUAGE sql IMMUTABLE STRICT
    RETURN (SELECT do1.organization_id FROM public.dss_organization do1 WHERE (public.st_intersects(dss_containing_organization_id.p_point, do1.area_geometry) AND (NOT (EXISTS (SELECT do2.organization_nm FROM public.dss_organization do2 WHERE (public.st_intersects(dss_containing_organization_id.p_point, do2.area_geometry) AND (do2.organization_id <> do1.organization_id) AND (public.st_area(do2.area_geometry) < public.st_area(do1.area_geometry))))))));


--
-- Name: dss_update_audit_columns(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.dss_update_audit_columns() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.upd_dtm := current_timestamp;
    RETURN NEW;
END;
$$;


--
-- Name: account_roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.account_roles (
    id integer NOT NULL,
    account_id integer NOT NULL,
    role character varying(25) NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer
);


--
-- Name: account_roles_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.account_roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: account_roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.account_roles_id_seq OWNED BY public.account_roles.id;


--
-- Name: addresses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.addresses (
    id integer NOT NULL,
    country character varying NOT NULL,
    street_address character varying NOT NULL,
    city character varying NOT NULL,
    province character varying NOT NULL,
    postal_code character varying NOT NULL,
    street_address_additional character varying,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    location_description character varying,
    unit_number character varying,
    street_number character varying
);


--
-- Name: addresses_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.addresses_history (
    id integer NOT NULL,
    country character varying NOT NULL,
    street_address character varying NOT NULL,
    street_address_additional character varying,
    city character varying NOT NULL,
    province character varying NOT NULL,
    postal_code character varying NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone,
    location_description character varying,
    unit_number character varying,
    street_number character varying
);


--
-- Name: addresses_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.addresses_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: addresses_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.addresses_id_seq OWNED BY public.addresses.id;


--
-- Name: alembic_version; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.alembic_version (
    version_num character varying(32) NOT NULL
);


--
-- Name: application; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.application (
    id integer NOT NULL,
    application_json jsonb NOT NULL,
    application_date timestamp with time zone DEFAULT now(),
    type character varying(50) NOT NULL,
    status character varying(50),
    decision_date timestamp with time zone,
    invoice_id integer,
    payment_status_code character varying(50),
    payment_completion_date timestamp with time zone,
    payment_account character varying(30),
    registration_id integer,
    submitter_id integer,
    reviewer_id integer,
    application_tsv tsvector GENERATED ALWAYS AS (jsonb_to_tsvector('english'::regconfig, application_json, '["string"]'::jsonb)) STORED,
    application_number character varying(14) NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    registration_type public.registrationtype,
    is_set_aside boolean,
    decider_id integer
);


--
-- Name: application_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.application_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: application_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.application_id_seq OWNED BY public.application.id;


--
-- Name: auto_approval_records; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.auto_approval_records (
    id integer NOT NULL,
    record jsonb NOT NULL,
    creation_date timestamp without time zone DEFAULT now() NOT NULL,
    application_id integer NOT NULL
);


--
-- Name: auto_approval_records_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.auto_approval_records_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: auto_approval_records_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.auto_approval_records_id_seq OWNED BY public.auto_approval_records.id;


--
-- Name: bulk_validation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.bulk_validation (
    id integer NOT NULL,
    request_file_id character varying(250),
    response_file_id character varying(250),
    request_timestamp timestamp without time zone DEFAULT now() NOT NULL,
    response_timestamp timestamp without time zone,
    status public.status NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer
);


--
-- Name: bulk_validation_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.bulk_validation_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: bulk_validation_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.bulk_validation_id_seq OWNED BY public.bulk_validation.id;


--
-- Name: certificates; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.certificates (
    id integer NOT NULL,
    registration_id integer NOT NULL,
    certificate bytea NOT NULL,
    issued_date timestamp without time zone DEFAULT now() NOT NULL,
    issuer_id integer,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer
);


--
-- Name: certificates_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.certificates_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: certificates_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.certificates_id_seq OWNED BY public.certificates.id;


--
-- Name: conditions_of_approval; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.conditions_of_approval (
    id integer NOT NULL,
    preapproved_conditions character varying[],
    custom_conditions character varying[],
    "minBookingDays" integer,
    registration_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: conditions_of_approval_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.conditions_of_approval_history (
    id integer NOT NULL,
    preapproved_conditions character varying[],
    custom_conditions character varying[],
    "minBookingDays" integer,
    registration_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: conditions_of_approval_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.conditions_of_approval_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: conditions_of_approval_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.conditions_of_approval_id_seq OWNED BY public.conditions_of_approval.id;


--
-- Name: contacts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.contacts (
    id integer NOT NULL,
    firstname character varying,
    lastname character varying NOT NULL,
    middlename character varying,
    preferredname character varying,
    email character varying,
    address_id integer,
    phone_number character varying,
    phone_extension character varying,
    fax_number character varying,
    date_of_birth date,
    social_insurance_number character varying,
    business_number character varying,
    job_title character varying,
    version integer NOT NULL,
    phone_country_code character varying
);


--
-- Name: contacts_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.contacts_history (
    id integer NOT NULL,
    firstname character varying(1000),
    lastname character varying(1000) NOT NULL,
    middlename character varying(1000),
    address_id integer,
    email character varying(255),
    preferredname character varying,
    phone_extension character varying,
    fax_number character varying,
    phone_number character varying(20),
    date_of_birth date,
    social_insurance_number character varying,
    business_number character varying,
    job_title character varying,
    version integer NOT NULL,
    changed timestamp without time zone,
    phone_country_code character varying
);


--
-- Name: contacts_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.contacts_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: contacts_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.contacts_id_seq OWNED BY public.contacts.id;


--
-- Name: documents; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.documents (
    id integer NOT NULL,
    file_name character varying NOT NULL,
    file_type character varying NOT NULL,
    path character varying NOT NULL,
    registration_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    document_type public.documenttype,
    added_on date,
    parsed_data jsonb,
    parsing_error jsonb
);


--
-- Name: documents_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.documents_history (
    id integer NOT NULL,
    file_name character varying NOT NULL,
    file_type character varying NOT NULL,
    path character varying NOT NULL,
    registration_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone,
    document_type public.documenttype,
    added_on date,
    parsed_data jsonb,
    parsing_error jsonb
);


--
-- Name: documents_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.documents_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: documents_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.documents_id_seq OWNED BY public.documents.id;


--
-- Name: dss_organization_organization_id_seq; Type: SEQUENCE; Schema: public; Owner: -
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
-- Name: events; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.events (
    id integer NOT NULL,
    event_type public.eventtype NOT NULL,
    event_name public.eventname NOT NULL,
    details character varying,
    created_date timestamp with time zone DEFAULT now() NOT NULL,
    visible_to_applicant boolean DEFAULT false NOT NULL,
    registration_id integer,
    application_id integer,
    user_id integer
);


--
-- Name: events_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.events_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: events_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.events_id_seq OWNED BY public.events.id;


--
-- Name: ltsa; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.ltsa (
    id integer NOT NULL,
    record jsonb NOT NULL,
    creation_date timestamp without time zone DEFAULT now() NOT NULL,
    application_id integer NOT NULL
);


--
-- Name: ltsa_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.ltsa_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: ltsa_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.ltsa_id_seq OWNED BY public.ltsa.id;


--
-- Name: notice_of_consideration; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.notice_of_consideration (
    id integer NOT NULL,
    application_id integer NOT NULL,
    content text NOT NULL,
    start_date timestamp with time zone NOT NULL,
    end_date timestamp with time zone NOT NULL,
    creation_date timestamp with time zone DEFAULT now() NOT NULL
);


--
-- Name: notice_of_consideration_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.notice_of_consideration_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: notice_of_consideration_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.notice_of_consideration_id_seq OWNED BY public.notice_of_consideration.id;


--
-- Name: platform_brands; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platform_brands (
    id integer NOT NULL,
    name character varying(150) NOT NULL,
    website character varying(500) NOT NULL,
    platform_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: platform_brands_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platform_brands_history (
    id integer NOT NULL,
    name character varying(150) NOT NULL,
    website character varying(500) NOT NULL,
    platform_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: platform_brands_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.platform_brands_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: platform_brands_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.platform_brands_id_seq OWNED BY public.platform_brands.id;


--
-- Name: platform_registration; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platform_registration (
    id integer NOT NULL,
    registration_id integer NOT NULL,
    platform_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: platform_registration_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platform_registration_history (
    id integer NOT NULL,
    registration_id integer NOT NULL,
    platform_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: platform_registration_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.platform_registration_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: platform_registration_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.platform_registration_id_seq OWNED BY public.platform_registration.id;


--
-- Name: platform_representatives; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platform_representatives (
    id integer NOT NULL,
    contact_id integer NOT NULL,
    platform_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: platform_representatives_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platform_representatives_history (
    id integer NOT NULL,
    contact_id integer NOT NULL,
    platform_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: platform_representatives_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.platform_representatives_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: platform_representatives_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.platform_representatives_id_seq OWNED BY public.platform_representatives.id;


--
-- Name: platforms; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platforms (
    id integer NOT NULL,
    legal_name character varying NOT NULL,
    business_number character varying,
    home_jurisdiction character varying(150) NOT NULL,
    cpbc_licence_number character varying(50),
    primary_non_compliance_notice_email character varying(100) NOT NULL,
    secondary_non_compliance_notice_email character varying(100),
    primary_take_down_request_email character varying(100) NOT NULL,
    secondary_take_down_request_email character varying(100),
    attorney_name character varying(150),
    listing_size public.listingsize,
    mailing_address_id integer NOT NULL,
    registered_office_attorney_mailing_address_id integer,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: platforms_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.platforms_history (
    id integer NOT NULL,
    legal_name character varying(150) NOT NULL,
    home_jurisdiction character varying(150) NOT NULL,
    business_number character varying(150),
    cpbc_licence_number character varying(50),
    primary_non_compliance_notice_email character varying(100) NOT NULL,
    secondary_non_compliance_notice_email character varying(100),
    primary_take_down_request_email character varying(100) NOT NULL,
    secondary_take_down_request_email character varying(100),
    attorney_name character varying(150),
    listing_size public.listingsize,
    mailing_address_id integer NOT NULL,
    registered_office_attorney_mailing_address_id integer,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: platforms_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.platforms_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: platforms_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.platforms_id_seq OWNED BY public.platforms.id;


--
-- Name: property_contacts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.property_contacts (
    id integer NOT NULL,
    is_primary boolean NOT NULL,
    contact_id integer NOT NULL,
    property_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    contact_type public.contacttype,
    business_legal_name character varying(1000)
);


--
-- Name: property_contacts_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.property_contacts_history (
    id integer NOT NULL,
    is_primary boolean NOT NULL,
    contact_id integer NOT NULL,
    property_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone,
    contact_type public.contacttype,
    business_legal_name character varying(1000)
);


--
-- Name: property_contacts_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.property_contacts_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: property_contacts_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.property_contacts_id_seq OWNED BY public.property_contacts.id;


--
-- Name: property_listings; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.property_listings (
    id integer NOT NULL,
    platform character varying,
    url character varying NOT NULL,
    type character varying,
    property_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: property_listings_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.property_listings_history (
    id integer NOT NULL,
    platform character varying,
    url character varying NOT NULL,
    type character varying,
    property_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: property_listings_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.property_listings_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: property_listings_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.property_listings_id_seq OWNED BY public.property_listings.id;


--
-- Name: property_manager; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.property_manager (
    id integer NOT NULL,
    business_legal_name character varying(250),
    business_number character varying(100),
    business_mailing_address_id integer,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    property_manager_type public.propertymanagertype,
    primary_contact_id integer
);


--
-- Name: property_manager_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.property_manager_history (
    id integer NOT NULL,
    business_legal_name character varying(250),
    business_number character varying(100),
    business_mailing_address_id integer,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone,
    property_manager_type public.propertymanagertype,
    primary_contact_id integer
);


--
-- Name: property_manager_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.property_manager_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: property_manager_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.property_manager_id_seq OWNED BY public.property_manager.id;


--
-- Name: real_time_validation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.real_time_validation (
    id integer NOT NULL,
    request_json jsonb NOT NULL,
    response_json jsonb,
    status_code character varying(100),
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer
);


--
-- Name: real_time_validation_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.real_time_validation_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: real_time_validation_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.real_time_validation_id_seq OWNED BY public.real_time_validation.id;


--
-- Name: registration_notice_of_consideration; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.registration_notice_of_consideration (
    id integer NOT NULL,
    registration_id integer NOT NULL,
    content text NOT NULL,
    start_date timestamp with time zone NOT NULL,
    end_date timestamp with time zone NOT NULL,
    creation_date timestamp with time zone DEFAULT now() NOT NULL
);


--
-- Name: registration_notice_of_consideration_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.registration_notice_of_consideration_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: registration_notice_of_consideration_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.registration_notice_of_consideration_id_seq OWNED BY public.registration_notice_of_consideration.id;


--
-- Name: registration_snapshot; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.registration_snapshot (
    id integer NOT NULL,
    snapshot_data jsonb NOT NULL,
    version integer NOT NULL,
    snapshot_datetime timestamp with time zone,
    registration_id integer NOT NULL
);


--
-- Name: registration_snapshot_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.registration_snapshot_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: registration_snapshot_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.registration_snapshot_id_seq OWNED BY public.registration_snapshot.id;


--
-- Name: registrations; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.registrations (
    id integer NOT NULL,
    updated_date timestamp without time zone NOT NULL,
    status public.registrationstatus NOT NULL,
    sbc_account_id integer NOT NULL,
    user_id integer NOT NULL,
    start_date timestamp without time zone NOT NULL,
    expiry_date timestamp without time zone NOT NULL,
    registration_number character varying,
    registration_type character varying,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    cancelled_date timestamp without time zone,
    is_set_aside boolean,
    noc_status public.registrationnocstatus,
    reviewer_id integer,
    decider_id integer
);


--
-- Name: registrations_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.registrations_history (
    id integer NOT NULL,
    registration_type character varying,
    registration_number character varying,
    sbc_account_id integer NOT NULL,
    status public.registrationstatus NOT NULL,
    start_date timestamp without time zone NOT NULL,
    expiry_date timestamp without time zone NOT NULL,
    updated_date timestamp without time zone NOT NULL,
    user_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone,
    cancelled_date timestamp without time zone,
    is_set_aside boolean,
    noc_status public.registrationnocstatus,
    reviewer_id integer,
    decider_id integer
);


--
-- Name: registrations_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.registrations_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: registrations_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.registrations_id_seq OWNED BY public.registrations.id;


--
-- Name: rental_properties; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.rental_properties (
    id integer NOT NULL,
    address_id integer NOT NULL,
    nickname character varying,
    parcel_identifier character varying,
    local_business_licence character varying,
    property_type public.propertytype NOT NULL,
    ownership_type public.ownershiptype,
    is_principal_residence boolean NOT NULL,
    rental_act_accepted boolean NOT NULL,
    pr_exempt_reason character varying,
    service_provider character varying,
    registration_id integer NOT NULL,
    local_business_licence_expiry_date date,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    property_manager_id integer,
    space_type public.rentalunitspacetype,
    host_residence public.hostresidence,
    is_unit_on_principal_residence_property boolean,
    number_of_rooms_for_rent integer,
    strata_hotel_registration_number character varying,
    bl_exempt_reason character varying,
    strata_hotel_category public.stratahotelcategory,
    pr_required boolean,
    bl_required boolean,
    jurisdiction character varying,
    strr_exempt boolean,
    rental_space_option public.rentalspaceoption,
    host_type public.hosttype
);


--
-- Name: rental_properties_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.rental_properties_history (
    id integer NOT NULL,
    nickname character varying,
    parcel_identifier character varying,
    local_business_licence character varying,
    local_business_licence_expiry_date date,
    property_type public.propertytype NOT NULL,
    ownership_type public.ownershiptype,
    is_principal_residence boolean NOT NULL,
    rental_act_accepted boolean NOT NULL,
    pr_exempt_reason character varying,
    service_provider character varying,
    address_id integer NOT NULL,
    registration_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone,
    property_manager_id integer,
    space_type public.rentalunitspacetype,
    host_residence public.hostresidence,
    is_unit_on_principal_residence_property boolean,
    number_of_rooms_for_rent integer,
    strata_hotel_registration_number character varying,
    bl_exempt_reason character varying,
    strata_hotel_category public.stratahotelcategory,
    pr_required boolean,
    bl_required boolean,
    jurisdiction character varying,
    strr_exempt boolean,
    rental_space_option public.rentalspaceoption,
    host_type public.hosttype
);


--
-- Name: rental_properties_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.rental_properties_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: rental_properties_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.rental_properties_id_seq OWNED BY public.rental_properties.id;


--
-- Name: strata_hotel_buildings; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotel_buildings (
    id integer NOT NULL,
    address_id integer NOT NULL,
    strata_hotel_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: strata_hotel_buildings_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotel_buildings_history (
    id integer NOT NULL,
    address_id integer NOT NULL,
    strata_hotel_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: strata_hotel_buildings_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.strata_hotel_buildings_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: strata_hotel_buildings_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.strata_hotel_buildings_id_seq OWNED BY public.strata_hotel_buildings.id;


--
-- Name: strata_hotel_registration; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotel_registration (
    id integer NOT NULL,
    registration_id integer NOT NULL,
    strata_hotel_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: strata_hotel_registration_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotel_registration_history (
    id integer NOT NULL,
    registration_id integer NOT NULL,
    strata_hotel_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: strata_hotel_registration_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.strata_hotel_registration_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: strata_hotel_registration_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.strata_hotel_registration_id_seq OWNED BY public.strata_hotel_registration.id;


--
-- Name: strata_hotel_representatives; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotel_representatives (
    id integer NOT NULL,
    contact_id integer NOT NULL,
    strata_hotel_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL
);


--
-- Name: strata_hotel_representatives_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotel_representatives_history (
    id integer NOT NULL,
    contact_id integer NOT NULL,
    strata_hotel_id integer NOT NULL,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone
);


--
-- Name: strata_hotel_representatives_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.strata_hotel_representatives_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: strata_hotel_representatives_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.strata_hotel_representatives_id_seq OWNED BY public.strata_hotel_representatives.id;


--
-- Name: strata_hotels; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotels (
    id integer NOT NULL,
    legal_name character varying(250) NOT NULL,
    home_jurisdiction character varying(150) NOT NULL,
    business_number character varying(150),
    attorney_name character varying(250),
    brand_name character varying(250) NOT NULL,
    website character varying(1000) NOT NULL,
    number_of_units integer NOT NULL,
    mailing_address_id integer NOT NULL,
    location_id integer NOT NULL,
    registered_office_attorney_mailing_address_id integer,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    category public.stratahotelcategory
);


--
-- Name: strata_hotels_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.strata_hotels_history (
    id integer NOT NULL,
    legal_name character varying(250) NOT NULL,
    home_jurisdiction character varying(150) NOT NULL,
    business_number character varying(150),
    attorney_name character varying(250),
    brand_name character varying(250) NOT NULL,
    website character varying(1000) NOT NULL,
    number_of_units integer NOT NULL,
    mailing_address_id integer NOT NULL,
    location_id integer NOT NULL,
    registered_office_attorney_mailing_address_id integer,
    created timestamp without time zone,
    modified timestamp without time zone,
    created_by_id integer,
    modified_by_id integer,
    version integer NOT NULL,
    changed timestamp without time zone,
    category public.stratahotelcategory
);


--
-- Name: strata_hotels_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.strata_hotels_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: strata_hotels_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.strata_hotels_id_seq OWNED BY public.strata_hotels.id;


--
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    id integer NOT NULL,
    username character varying(1000),
    firstname character varying(1000),
    lastname character varying(1000),
    middlename character varying(1000),
    email character varying(1024),
    sub character varying(36),
    iss character varying(1024),
    idp_userid character varying(256),
    login_source character varying(200),
    creation_date timestamp with time zone
);


--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: account_roles id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.account_roles ALTER COLUMN id SET DEFAULT nextval('public.account_roles_id_seq'::regclass);


--
-- Name: addresses id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.addresses ALTER COLUMN id SET DEFAULT nextval('public.addresses_id_seq'::regclass);


--
-- Name: application id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application ALTER COLUMN id SET DEFAULT nextval('public.application_id_seq'::regclass);


--
-- Name: auto_approval_records id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.auto_approval_records ALTER COLUMN id SET DEFAULT nextval('public.auto_approval_records_id_seq'::regclass);


--
-- Name: bulk_validation id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bulk_validation ALTER COLUMN id SET DEFAULT nextval('public.bulk_validation_id_seq'::regclass);


--
-- Name: certificates id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.certificates ALTER COLUMN id SET DEFAULT nextval('public.certificates_id_seq'::regclass);


--
-- Name: conditions_of_approval id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval ALTER COLUMN id SET DEFAULT nextval('public.conditions_of_approval_id_seq'::regclass);


--
-- Name: contacts id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacts ALTER COLUMN id SET DEFAULT nextval('public.contacts_id_seq'::regclass);


--
-- Name: documents id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents ALTER COLUMN id SET DEFAULT nextval('public.documents_id_seq'::regclass);


--
-- Name: events id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events ALTER COLUMN id SET DEFAULT nextval('public.events_id_seq'::regclass);


--
-- Name: ltsa id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ltsa ALTER COLUMN id SET DEFAULT nextval('public.ltsa_id_seq'::regclass);


--
-- Name: notice_of_consideration id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notice_of_consideration ALTER COLUMN id SET DEFAULT nextval('public.notice_of_consideration_id_seq'::regclass);


--
-- Name: platform_brands id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands ALTER COLUMN id SET DEFAULT nextval('public.platform_brands_id_seq'::regclass);


--
-- Name: platform_registration id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration ALTER COLUMN id SET DEFAULT nextval('public.platform_registration_id_seq'::regclass);


--
-- Name: platform_representatives id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives ALTER COLUMN id SET DEFAULT nextval('public.platform_representatives_id_seq'::regclass);


--
-- Name: platforms id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms ALTER COLUMN id SET DEFAULT nextval('public.platforms_id_seq'::regclass);


--
-- Name: property_contacts id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts ALTER COLUMN id SET DEFAULT nextval('public.property_contacts_id_seq'::regclass);


--
-- Name: property_listings id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings ALTER COLUMN id SET DEFAULT nextval('public.property_listings_id_seq'::regclass);


--
-- Name: property_manager id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager ALTER COLUMN id SET DEFAULT nextval('public.property_manager_id_seq'::regclass);


--
-- Name: real_time_validation id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.real_time_validation ALTER COLUMN id SET DEFAULT nextval('public.real_time_validation_id_seq'::regclass);


--
-- Name: registration_notice_of_consideration id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registration_notice_of_consideration ALTER COLUMN id SET DEFAULT nextval('public.registration_notice_of_consideration_id_seq'::regclass);


--
-- Name: registration_snapshot id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registration_snapshot ALTER COLUMN id SET DEFAULT nextval('public.registration_snapshot_id_seq'::regclass);


--
-- Name: registrations id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations ALTER COLUMN id SET DEFAULT nextval('public.registrations_id_seq'::regclass);


--
-- Name: rental_properties id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties ALTER COLUMN id SET DEFAULT nextval('public.rental_properties_id_seq'::regclass);


--
-- Name: strata_hotel_buildings id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings ALTER COLUMN id SET DEFAULT nextval('public.strata_hotel_buildings_id_seq'::regclass);


--
-- Name: strata_hotel_registration id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration ALTER COLUMN id SET DEFAULT nextval('public.strata_hotel_registration_id_seq'::regclass);


--
-- Name: strata_hotel_representatives id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives ALTER COLUMN id SET DEFAULT nextval('public.strata_hotel_representatives_id_seq'::regclass);


--
-- Name: strata_hotels id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels ALTER COLUMN id SET DEFAULT nextval('public.strata_hotels_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Data for Name: account_roles; Type: TABLE DATA; Schema: public; Owner: -
--


--
-- Name: account_roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.account_roles_id_seq', 31, true);


--
-- Name: addresses_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.addresses_id_seq', 1521, true);


--
-- Name: application_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.application_id_seq', 1259, true);


--
-- Name: auto_approval_records_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.auto_approval_records_id_seq', 525, true);


--
-- Name: bulk_validation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.bulk_validation_id_seq', 1, false);


--
-- Name: certificates_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.certificates_id_seq', 26, true);


--
-- Name: conditions_of_approval_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.conditions_of_approval_id_seq', 28, true);


--
-- Name: contacts_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.contacts_id_seq', 831, true);


--
-- Name: documents_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.documents_id_seq', 928, true);


--
-- Name: dss_organization_organization_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.dss_organization_organization_id_seq', 1173, true);


--
-- Name: events_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.events_id_seq', 6082, true);


--
-- Name: ltsa_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.ltsa_id_seq', 5, true);


--
-- Name: notice_of_consideration_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.notice_of_consideration_id_seq', 165, true);


--
-- Name: platform_brands_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.platform_brands_id_seq', 65, true);


--
-- Name: platform_registration_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.platform_registration_id_seq', 57, true);


--
-- Name: platform_representatives_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.platform_representatives_id_seq', 62, true);


--
-- Name: platforms_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.platforms_id_seq', 57, true);


--
-- Name: property_contacts_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.property_contacts_id_seq', 510, true);


--
-- Name: property_listings_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.property_listings_id_seq', 8, true);


--
-- Name: property_manager_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.property_manager_id_seq', 100, true);


--
-- Name: real_time_validation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.real_time_validation_id_seq', 29, true);


--
-- Name: registration_notice_of_consideration_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.registration_notice_of_consideration_id_seq', 86, true);


--
-- Name: registration_snapshot_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.registration_snapshot_id_seq', 1, false);


--
-- Name: registrations_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.registrations_id_seq', 660, true);


--
-- Name: rental_properties_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.rental_properties_id_seq', 579, true);


--
-- Name: strata_hotel_buildings_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.strata_hotel_buildings_id_seq', 8, true);


--
-- Name: strata_hotel_registration_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.strata_hotel_registration_id_seq', 23, true);


--
-- Name: strata_hotel_representatives_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.strata_hotel_representatives_id_seq', 23, true);


--
-- Name: strata_hotels_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.strata_hotels_id_seq', 23, true);


--
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.users_id_seq', 106, true);


--
-- Name: account_roles account_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.account_roles
    ADD CONSTRAINT account_roles_pkey PRIMARY KEY (id);


--
-- Name: addresses_history addresses_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.addresses_history
    ADD CONSTRAINT addresses_history_pkey PRIMARY KEY (id, version);


--
-- Name: addresses addresses_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.addresses
    ADD CONSTRAINT addresses_pkey PRIMARY KEY (id);


--
-- Name: alembic_version alembic_version_pkc; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.alembic_version
    ADD CONSTRAINT alembic_version_pkc PRIMARY KEY (version_num);


--
-- Name: application application_application_number_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT application_application_number_key UNIQUE (application_number);


--
-- Name: application application_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT application_pkey PRIMARY KEY (id);


--
-- Name: auto_approval_records auto_approval_records_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.auto_approval_records
    ADD CONSTRAINT auto_approval_records_pkey PRIMARY KEY (id);


--
-- Name: bulk_validation bulk_validation_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bulk_validation
    ADD CONSTRAINT bulk_validation_pkey PRIMARY KEY (id);


--
-- Name: certificates certificates_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.certificates
    ADD CONSTRAINT certificates_pkey PRIMARY KEY (id);


--
-- Name: conditions_of_approval_history conditions_of_approval_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval_history
    ADD CONSTRAINT conditions_of_approval_history_pkey PRIMARY KEY (id, version);


--
-- Name: conditions_of_approval conditions_of_approval_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval
    ADD CONSTRAINT conditions_of_approval_pkey PRIMARY KEY (id);


--
-- Name: contacts_history contacts_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacts_history
    ADD CONSTRAINT contacts_history_pkey PRIMARY KEY (id, version);


--
-- Name: contacts contacts_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacts
    ADD CONSTRAINT contacts_pkey PRIMARY KEY (id);


--
-- Name: documents_history documents_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents_history
    ADD CONSTRAINT documents_history_pkey PRIMARY KEY (id, version);


--
-- Name: documents documents_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT documents_pkey PRIMARY KEY (id);


--
-- Name: dss_organization dss_organization_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dss_organization
    ADD CONSTRAINT dss_organization_pk PRIMARY KEY (organization_id);


--
-- Name: events events_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events
    ADD CONSTRAINT events_pkey PRIMARY KEY (id);


--
-- Name: ltsa ltsa_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ltsa
    ADD CONSTRAINT ltsa_pkey PRIMARY KEY (id);


--
-- Name: notice_of_consideration notice_of_consideration_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notice_of_consideration
    ADD CONSTRAINT notice_of_consideration_pkey PRIMARY KEY (id);


--
-- Name: platform_brands_history platform_brands_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands_history
    ADD CONSTRAINT platform_brands_history_pkey PRIMARY KEY (id, version);


--
-- Name: platform_brands platform_brands_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands
    ADD CONSTRAINT platform_brands_pkey PRIMARY KEY (id);


--
-- Name: platform_registration_history platform_registration_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration_history
    ADD CONSTRAINT platform_registration_history_pkey PRIMARY KEY (id, version);


--
-- Name: platform_registration platform_registration_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration
    ADD CONSTRAINT platform_registration_pkey PRIMARY KEY (id);


--
-- Name: platform_representatives_history platform_representatives_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives_history
    ADD CONSTRAINT platform_representatives_history_pkey PRIMARY KEY (id, version);


--
-- Name: platform_representatives platform_representatives_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives
    ADD CONSTRAINT platform_representatives_pkey PRIMARY KEY (id);


--
-- Name: platforms_history platforms_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms_history
    ADD CONSTRAINT platforms_history_pkey PRIMARY KEY (id, version);


--
-- Name: platforms platforms_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms
    ADD CONSTRAINT platforms_pkey PRIMARY KEY (id);


--
-- Name: property_contacts_history property_contacts_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts_history
    ADD CONSTRAINT property_contacts_history_pkey PRIMARY KEY (id, version);


--
-- Name: property_contacts property_contacts_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts
    ADD CONSTRAINT property_contacts_pkey PRIMARY KEY (id);


--
-- Name: property_listings_history property_listings_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings_history
    ADD CONSTRAINT property_listings_history_pkey PRIMARY KEY (id, version);


--
-- Name: property_listings property_listings_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings
    ADD CONSTRAINT property_listings_pkey PRIMARY KEY (id);


--
-- Name: property_manager_history property_manager_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager_history
    ADD CONSTRAINT property_manager_history_pkey PRIMARY KEY (id, version);


--
-- Name: property_manager property_manager_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager
    ADD CONSTRAINT property_manager_pkey PRIMARY KEY (id);


--
-- Name: real_time_validation real_time_validation_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.real_time_validation
    ADD CONSTRAINT real_time_validation_pkey PRIMARY KEY (id);


--
-- Name: registration_notice_of_consideration registration_notice_of_consideration_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registration_notice_of_consideration
    ADD CONSTRAINT registration_notice_of_consideration_pkey PRIMARY KEY (id);


--
-- Name: registration_snapshot registration_snapshot_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registration_snapshot
    ADD CONSTRAINT registration_snapshot_pkey PRIMARY KEY (id);


--
-- Name: registrations_history registrations_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations_history
    ADD CONSTRAINT registrations_history_pkey PRIMARY KEY (id, version);


--
-- Name: registrations registrations_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations
    ADD CONSTRAINT registrations_pkey PRIMARY KEY (id);


--
-- Name: rental_properties_history rental_properties_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties_history
    ADD CONSTRAINT rental_properties_history_pkey PRIMARY KEY (id, version);


--
-- Name: rental_properties rental_properties_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties
    ADD CONSTRAINT rental_properties_pkey PRIMARY KEY (id);


--
-- Name: strata_hotel_buildings_history strata_hotel_buildings_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings_history
    ADD CONSTRAINT strata_hotel_buildings_history_pkey PRIMARY KEY (id, version);


--
-- Name: strata_hotel_buildings strata_hotel_buildings_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings
    ADD CONSTRAINT strata_hotel_buildings_pkey PRIMARY KEY (id);


--
-- Name: strata_hotel_registration_history strata_hotel_registration_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration_history
    ADD CONSTRAINT strata_hotel_registration_history_pkey PRIMARY KEY (id, version);


--
-- Name: strata_hotel_registration strata_hotel_registration_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration
    ADD CONSTRAINT strata_hotel_registration_pkey PRIMARY KEY (id);


--
-- Name: strata_hotel_representatives_history strata_hotel_representatives_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives_history
    ADD CONSTRAINT strata_hotel_representatives_history_pkey PRIMARY KEY (id, version);


--
-- Name: strata_hotel_representatives strata_hotel_representatives_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives
    ADD CONSTRAINT strata_hotel_representatives_pkey PRIMARY KEY (id);


--
-- Name: strata_hotels_history strata_hotels_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels_history
    ADD CONSTRAINT strata_hotels_history_pkey PRIMARY KEY (id, version);


--
-- Name: strata_hotels strata_hotels_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels
    ADD CONSTRAINT strata_hotels_pkey PRIMARY KEY (id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: users users_sub_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_sub_key UNIQUE (sub);


--
-- Name: idx_application_tsv; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_application_tsv ON public.application USING gin (application_tsv);


--
-- Name: idx_gin_application_json_path_ops; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_gin_application_json_path_ops ON public.application USING gin (application_json jsonb_path_ops);


--
-- Name: ix_application_application_number; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX ix_application_application_number ON public.application USING btree (application_number);


--
-- Name: ix_application_registration_type; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_application_registration_type ON public.application USING btree (registration_type);


--
-- Name: ix_application_type; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_application_type ON public.application USING btree (type);


--
-- Name: ix_bulk_validation_status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_bulk_validation_status ON public.bulk_validation USING btree (status);


--
-- Name: ix_documents_document_type; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_documents_document_type ON public.documents USING btree (document_type);


--
-- Name: ix_documents_history_document_type; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_documents_history_document_type ON public.documents_history USING btree (document_type);


--
-- Name: ix_events_registration_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_events_registration_id ON public.events USING btree (registration_id);


--
-- Name: ix_platform_brands_history_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_platform_brands_history_name ON public.platform_brands_history USING btree (name);


--
-- Name: ix_platform_brands_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_platform_brands_name ON public.platform_brands USING btree (name);


--
-- Name: ix_platforms_history_legal_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_platforms_history_legal_name ON public.platforms_history USING btree (legal_name);


--
-- Name: ix_platforms_legal_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_platforms_legal_name ON public.platforms USING btree (legal_name);


--
-- Name: ix_registration_snapshot_version; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_registration_snapshot_version ON public.registration_snapshot USING btree (version);


--
-- Name: ix_registrations_history_sbc_account_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_registrations_history_sbc_account_id ON public.registrations_history USING btree (sbc_account_id);


--
-- Name: ix_registrations_sbc_account_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_registrations_sbc_account_id ON public.registrations USING btree (sbc_account_id);


--
-- Name: ix_rental_properties_history_jurisdiction; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_rental_properties_history_jurisdiction ON public.rental_properties_history USING btree (jurisdiction);


--
-- Name: ix_rental_properties_history_strata_hotel_category; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_rental_properties_history_strata_hotel_category ON public.rental_properties_history USING btree (strata_hotel_category);


--
-- Name: ix_rental_properties_jurisdiction; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_rental_properties_jurisdiction ON public.rental_properties USING btree (jurisdiction);


--
-- Name: ix_rental_properties_strata_hotel_category; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_rental_properties_strata_hotel_category ON public.rental_properties USING btree (strata_hotel_category);


--
-- Name: ix_strata_hotels_category; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_strata_hotels_category ON public.strata_hotels USING btree (category);


--
-- Name: ix_strata_hotels_history_category; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_strata_hotels_history_category ON public.strata_hotels_history USING btree (category);


--
-- Name: ix_strata_hotels_history_legal_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_strata_hotels_history_legal_name ON public.strata_hotels_history USING btree (legal_name);


--
-- Name: ix_strata_hotels_legal_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_strata_hotels_legal_name ON public.strata_hotels USING btree (legal_name);


--
-- Name: ix_users_username; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_users_username ON public.users USING btree (username);


--
-- Name: dss_organization dss_organization_br_iu_tr; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER dss_organization_br_iu_tr BEFORE INSERT OR UPDATE ON public.dss_organization FOR EACH ROW EXECUTE FUNCTION public.dss_update_audit_columns();


--
-- Name: application application_registration_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT application_registration_id_fkey FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: application application_reviewer_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT application_reviewer_id_fkey FOREIGN KEY (reviewer_id) REFERENCES public.users(id);


--
-- Name: application application_submitter_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT application_submitter_id_fkey FOREIGN KEY (submitter_id) REFERENCES public.users(id);


--
-- Name: auto_approval_records auto_approval_records_application_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.auto_approval_records
    ADD CONSTRAINT auto_approval_records_application_id_fkey FOREIGN KEY (application_id) REFERENCES public.application(id) ON DELETE CASCADE;


--
-- Name: certificates certificates_issuer_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.certificates
    ADD CONSTRAINT certificates_issuer_id_fkey FOREIGN KEY (issuer_id) REFERENCES public.users(id);


--
-- Name: certificates certificates_registration_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.certificates
    ADD CONSTRAINT certificates_registration_id_fkey FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: contacts contacts_address_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacts
    ADD CONSTRAINT contacts_address_id_fkey FOREIGN KEY (address_id) REFERENCES public.addresses(id);


--
-- Name: documents documents_registration_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT documents_registration_id_fkey FOREIGN KEY (registration_id) REFERENCES public.registrations(id) ON DELETE CASCADE;


--
-- Name: dss_organization dss_organization_fk_managed_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dss_organization
    ADD CONSTRAINT dss_organization_fk_managed_by FOREIGN KEY (managing_organization_id) REFERENCES public.dss_organization(organization_id);


--
-- Name: events events_application_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events
    ADD CONSTRAINT events_application_id_fkey FOREIGN KEY (application_id) REFERENCES public.application(id) ON DELETE CASCADE;


--
-- Name: events events_registration_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events
    ADD CONSTRAINT events_registration_id_fkey FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: events events_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.events
    ADD CONSTRAINT events_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- Name: account_roles fk_account_roles_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.account_roles
    ADD CONSTRAINT fk_account_roles_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: account_roles fk_account_roles_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.account_roles
    ADD CONSTRAINT fk_account_roles_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: addresses_history fk_addresses_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.addresses_history
    ADD CONSTRAINT fk_addresses_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: addresses fk_addresses_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.addresses
    ADD CONSTRAINT fk_addresses_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: addresses_history fk_addresses_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.addresses_history
    ADD CONSTRAINT fk_addresses_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: addresses fk_addresses_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.addresses
    ADD CONSTRAINT fk_addresses_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: application fk_application_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT fk_application_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: application fk_application_decider_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT fk_application_decider_id_users FOREIGN KEY (decider_id) REFERENCES public.users(id);


--
-- Name: application fk_application_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.application
    ADD CONSTRAINT fk_application_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: bulk_validation fk_bulk_validation_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bulk_validation
    ADD CONSTRAINT fk_bulk_validation_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: bulk_validation fk_bulk_validation_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bulk_validation
    ADD CONSTRAINT fk_bulk_validation_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: certificates fk_certificates_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.certificates
    ADD CONSTRAINT fk_certificates_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: certificates fk_certificates_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.certificates
    ADD CONSTRAINT fk_certificates_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: conditions_of_approval fk_conditions_of_approval_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval
    ADD CONSTRAINT fk_conditions_of_approval_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: conditions_of_approval_history fk_conditions_of_approval_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval_history
    ADD CONSTRAINT fk_conditions_of_approval_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: conditions_of_approval fk_conditions_of_approval_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval
    ADD CONSTRAINT fk_conditions_of_approval_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: conditions_of_approval_history fk_conditions_of_approval_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval_history
    ADD CONSTRAINT fk_conditions_of_approval_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: conditions_of_approval fk_conditions_of_approval_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval
    ADD CONSTRAINT fk_conditions_of_approval_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: conditions_of_approval_history fk_conditions_of_approval_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conditions_of_approval_history
    ADD CONSTRAINT fk_conditions_of_approval_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: contacts_history fk_contacts_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacts_history
    ADD CONSTRAINT fk_contacts_address_id_addresses FOREIGN KEY (address_id) REFERENCES public.addresses(id);


--
-- Name: documents_history fk_documents_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents_history
    ADD CONSTRAINT fk_documents_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: documents fk_documents_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT fk_documents_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: documents_history fk_documents_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents_history
    ADD CONSTRAINT fk_documents_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: documents fk_documents_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT fk_documents_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: documents_history fk_documents_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents_history
    ADD CONSTRAINT fk_documents_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: notice_of_consideration fk_notice_of_consideration_application_id_application; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notice_of_consideration
    ADD CONSTRAINT fk_notice_of_consideration_application_id_application FOREIGN KEY (application_id) REFERENCES public.application(id);


--
-- Name: platform_brands fk_platform_brands_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands
    ADD CONSTRAINT fk_platform_brands_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platform_brands_history fk_platform_brands_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands_history
    ADD CONSTRAINT fk_platform_brands_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platform_brands fk_platform_brands_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands
    ADD CONSTRAINT fk_platform_brands_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platform_brands_history fk_platform_brands_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands_history
    ADD CONSTRAINT fk_platform_brands_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platform_brands fk_platform_brands_platform_id_platforms; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands
    ADD CONSTRAINT fk_platform_brands_platform_id_platforms FOREIGN KEY (platform_id) REFERENCES public.platforms(id);


--
-- Name: platform_brands_history fk_platform_brands_platform_id_platforms; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_brands_history
    ADD CONSTRAINT fk_platform_brands_platform_id_platforms FOREIGN KEY (platform_id) REFERENCES public.platforms(id);


--
-- Name: platform_registration fk_platform_registration_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration
    ADD CONSTRAINT fk_platform_registration_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platform_registration_history fk_platform_registration_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration_history
    ADD CONSTRAINT fk_platform_registration_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platform_registration fk_platform_registration_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration
    ADD CONSTRAINT fk_platform_registration_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platform_registration_history fk_platform_registration_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration_history
    ADD CONSTRAINT fk_platform_registration_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platform_registration fk_platform_registration_platform_id_platforms; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration
    ADD CONSTRAINT fk_platform_registration_platform_id_platforms FOREIGN KEY (platform_id) REFERENCES public.platforms(id);


--
-- Name: platform_registration_history fk_platform_registration_platform_id_platforms; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration_history
    ADD CONSTRAINT fk_platform_registration_platform_id_platforms FOREIGN KEY (platform_id) REFERENCES public.platforms(id);


--
-- Name: platform_registration_history fk_platform_registration_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration_history
    ADD CONSTRAINT fk_platform_registration_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: platform_registration fk_platform_registration_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_registration
    ADD CONSTRAINT fk_platform_registration_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id) ON DELETE CASCADE;


--
-- Name: platform_representatives fk_platform_representatives_contact_id_contacts; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives
    ADD CONSTRAINT fk_platform_representatives_contact_id_contacts FOREIGN KEY (contact_id) REFERENCES public.contacts(id);


--
-- Name: platform_representatives_history fk_platform_representatives_contact_id_contacts; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives_history
    ADD CONSTRAINT fk_platform_representatives_contact_id_contacts FOREIGN KEY (contact_id) REFERENCES public.contacts(id);


--
-- Name: platform_representatives fk_platform_representatives_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives
    ADD CONSTRAINT fk_platform_representatives_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platform_representatives_history fk_platform_representatives_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives_history
    ADD CONSTRAINT fk_platform_representatives_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platform_representatives fk_platform_representatives_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives
    ADD CONSTRAINT fk_platform_representatives_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platform_representatives_history fk_platform_representatives_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives_history
    ADD CONSTRAINT fk_platform_representatives_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platform_representatives fk_platform_representatives_platform_id_platforms; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives
    ADD CONSTRAINT fk_platform_representatives_platform_id_platforms FOREIGN KEY (platform_id) REFERENCES public.platforms(id);


--
-- Name: platform_representatives_history fk_platform_representatives_platform_id_platforms; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platform_representatives_history
    ADD CONSTRAINT fk_platform_representatives_platform_id_platforms FOREIGN KEY (platform_id) REFERENCES public.platforms(id);


--
-- Name: platforms_history fk_platforms_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms_history
    ADD CONSTRAINT fk_platforms_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platforms fk_platforms_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms
    ADD CONSTRAINT fk_platforms_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: platforms_history fk_platforms_mailing_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms_history
    ADD CONSTRAINT fk_platforms_mailing_address_id_addresses FOREIGN KEY (mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: platforms fk_platforms_mailing_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms
    ADD CONSTRAINT fk_platforms_mailing_address_id_addresses FOREIGN KEY (mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: platforms_history fk_platforms_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms_history
    ADD CONSTRAINT fk_platforms_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platforms fk_platforms_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms
    ADD CONSTRAINT fk_platforms_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: platforms_history fk_platforms_registered_office_attorney_mailing_address_0020; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms_history
    ADD CONSTRAINT fk_platforms_registered_office_attorney_mailing_address_0020 FOREIGN KEY (registered_office_attorney_mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: platforms fk_platforms_registered_office_attorney_mailing_address_0020; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.platforms
    ADD CONSTRAINT fk_platforms_registered_office_attorney_mailing_address_0020 FOREIGN KEY (registered_office_attorney_mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: property_contacts_history fk_property_contacts_contact_id_contacts; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts_history
    ADD CONSTRAINT fk_property_contacts_contact_id_contacts FOREIGN KEY (contact_id) REFERENCES public.contacts(id);


--
-- Name: property_contacts_history fk_property_contacts_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts_history
    ADD CONSTRAINT fk_property_contacts_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: property_contacts fk_property_contacts_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts
    ADD CONSTRAINT fk_property_contacts_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: property_contacts_history fk_property_contacts_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts_history
    ADD CONSTRAINT fk_property_contacts_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: property_contacts fk_property_contacts_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts
    ADD CONSTRAINT fk_property_contacts_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: property_contacts_history fk_property_contacts_property_id_rental_properties; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts_history
    ADD CONSTRAINT fk_property_contacts_property_id_rental_properties FOREIGN KEY (property_id) REFERENCES public.rental_properties(id);


--
-- Name: property_listings_history fk_property_listings_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings_history
    ADD CONSTRAINT fk_property_listings_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: property_listings fk_property_listings_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings
    ADD CONSTRAINT fk_property_listings_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: property_listings_history fk_property_listings_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings_history
    ADD CONSTRAINT fk_property_listings_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: property_listings fk_property_listings_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings
    ADD CONSTRAINT fk_property_listings_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: property_listings_history fk_property_listings_property_id_rental_properties; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings_history
    ADD CONSTRAINT fk_property_listings_property_id_rental_properties FOREIGN KEY (property_id) REFERENCES public.rental_properties(id);


--
-- Name: property_manager fk_property_manager_business_mailing_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager
    ADD CONSTRAINT fk_property_manager_business_mailing_address_id_addresses FOREIGN KEY (business_mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: property_manager_history fk_property_manager_business_mailing_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager_history
    ADD CONSTRAINT fk_property_manager_business_mailing_address_id_addresses FOREIGN KEY (business_mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: property_manager fk_property_manager_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager
    ADD CONSTRAINT fk_property_manager_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: property_manager_history fk_property_manager_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager_history
    ADD CONSTRAINT fk_property_manager_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: property_manager fk_property_manager_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager
    ADD CONSTRAINT fk_property_manager_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: property_manager_history fk_property_manager_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager_history
    ADD CONSTRAINT fk_property_manager_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: property_manager fk_property_manager_primary_contact_id_contacts; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager
    ADD CONSTRAINT fk_property_manager_primary_contact_id_contacts FOREIGN KEY (primary_contact_id) REFERENCES public.contacts(id);


--
-- Name: property_manager_history fk_property_manager_primary_contact_id_contacts; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_manager_history
    ADD CONSTRAINT fk_property_manager_primary_contact_id_contacts FOREIGN KEY (primary_contact_id) REFERENCES public.contacts(id);


--
-- Name: real_time_validation fk_real_time_validation_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.real_time_validation
    ADD CONSTRAINT fk_real_time_validation_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: real_time_validation fk_real_time_validation_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.real_time_validation
    ADD CONSTRAINT fk_real_time_validation_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: registration_notice_of_consideration fk_registration_notice_of_consideration_registration_id_e405; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registration_notice_of_consideration
    ADD CONSTRAINT fk_registration_notice_of_consideration_registration_id_e405 FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: registration_snapshot fk_registration_snapshot_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registration_snapshot
    ADD CONSTRAINT fk_registration_snapshot_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: registrations_history fk_registrations_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations_history
    ADD CONSTRAINT fk_registrations_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: registrations fk_registrations_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations
    ADD CONSTRAINT fk_registrations_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: registrations fk_registrations_decider_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations
    ADD CONSTRAINT fk_registrations_decider_id_users FOREIGN KEY (decider_id) REFERENCES public.users(id);


--
-- Name: registrations_history fk_registrations_decider_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations_history
    ADD CONSTRAINT fk_registrations_decider_id_users FOREIGN KEY (decider_id) REFERENCES public.users(id);


--
-- Name: registrations_history fk_registrations_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations_history
    ADD CONSTRAINT fk_registrations_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: registrations fk_registrations_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations
    ADD CONSTRAINT fk_registrations_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: registrations fk_registrations_reviewer_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations
    ADD CONSTRAINT fk_registrations_reviewer_id_users FOREIGN KEY (reviewer_id) REFERENCES public.users(id);


--
-- Name: registrations_history fk_registrations_reviewer_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations_history
    ADD CONSTRAINT fk_registrations_reviewer_id_users FOREIGN KEY (reviewer_id) REFERENCES public.users(id);


--
-- Name: registrations_history fk_registrations_user_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations_history
    ADD CONSTRAINT fk_registrations_user_id_users FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- Name: rental_properties_history fk_rental_properties_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties_history
    ADD CONSTRAINT fk_rental_properties_address_id_addresses FOREIGN KEY (address_id) REFERENCES public.addresses(id);


--
-- Name: rental_properties_history fk_rental_properties_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties_history
    ADD CONSTRAINT fk_rental_properties_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: rental_properties fk_rental_properties_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties
    ADD CONSTRAINT fk_rental_properties_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: rental_properties_history fk_rental_properties_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties_history
    ADD CONSTRAINT fk_rental_properties_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: rental_properties fk_rental_properties_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties
    ADD CONSTRAINT fk_rental_properties_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: rental_properties fk_rental_properties_property_manager_id_property_manager; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties
    ADD CONSTRAINT fk_rental_properties_property_manager_id_property_manager FOREIGN KEY (property_manager_id) REFERENCES public.property_manager(id);


--
-- Name: rental_properties_history fk_rental_properties_property_manager_id_property_manager; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties_history
    ADD CONSTRAINT fk_rental_properties_property_manager_id_property_manager FOREIGN KEY (property_manager_id) REFERENCES public.property_manager(id);


--
-- Name: rental_properties_history fk_rental_properties_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties_history
    ADD CONSTRAINT fk_rental_properties_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: strata_hotel_buildings fk_strata_hotel_buildings_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings
    ADD CONSTRAINT fk_strata_hotel_buildings_address_id_addresses FOREIGN KEY (address_id) REFERENCES public.addresses(id);


--
-- Name: strata_hotel_buildings_history fk_strata_hotel_buildings_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings_history
    ADD CONSTRAINT fk_strata_hotel_buildings_address_id_addresses FOREIGN KEY (address_id) REFERENCES public.addresses(id);


--
-- Name: strata_hotel_buildings fk_strata_hotel_buildings_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings
    ADD CONSTRAINT fk_strata_hotel_buildings_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_buildings_history fk_strata_hotel_buildings_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings_history
    ADD CONSTRAINT fk_strata_hotel_buildings_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_buildings fk_strata_hotel_buildings_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings
    ADD CONSTRAINT fk_strata_hotel_buildings_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_buildings_history fk_strata_hotel_buildings_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings_history
    ADD CONSTRAINT fk_strata_hotel_buildings_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_buildings fk_strata_hotel_buildings_strata_hotel_id_strata_hotels; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings
    ADD CONSTRAINT fk_strata_hotel_buildings_strata_hotel_id_strata_hotels FOREIGN KEY (strata_hotel_id) REFERENCES public.strata_hotels(id);


--
-- Name: strata_hotel_buildings_history fk_strata_hotel_buildings_strata_hotel_id_strata_hotels; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_buildings_history
    ADD CONSTRAINT fk_strata_hotel_buildings_strata_hotel_id_strata_hotels FOREIGN KEY (strata_hotel_id) REFERENCES public.strata_hotels(id);


--
-- Name: strata_hotel_registration fk_strata_hotel_registration_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration
    ADD CONSTRAINT fk_strata_hotel_registration_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_registration_history fk_strata_hotel_registration_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration_history
    ADD CONSTRAINT fk_strata_hotel_registration_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_registration fk_strata_hotel_registration_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration
    ADD CONSTRAINT fk_strata_hotel_registration_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_registration_history fk_strata_hotel_registration_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration_history
    ADD CONSTRAINT fk_strata_hotel_registration_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_registration_history fk_strata_hotel_registration_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration_history
    ADD CONSTRAINT fk_strata_hotel_registration_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id);


--
-- Name: strata_hotel_registration fk_strata_hotel_registration_registration_id_registrations; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration
    ADD CONSTRAINT fk_strata_hotel_registration_registration_id_registrations FOREIGN KEY (registration_id) REFERENCES public.registrations(id) ON DELETE CASCADE;


--
-- Name: strata_hotel_registration fk_strata_hotel_registration_strata_hotel_id_strata_hotels; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration
    ADD CONSTRAINT fk_strata_hotel_registration_strata_hotel_id_strata_hotels FOREIGN KEY (strata_hotel_id) REFERENCES public.strata_hotels(id);


--
-- Name: strata_hotel_registration_history fk_strata_hotel_registration_strata_hotel_id_strata_hotels; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_registration_history
    ADD CONSTRAINT fk_strata_hotel_registration_strata_hotel_id_strata_hotels FOREIGN KEY (strata_hotel_id) REFERENCES public.strata_hotels(id);


--
-- Name: strata_hotel_representatives fk_strata_hotel_representatives_contact_id_contacts; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives
    ADD CONSTRAINT fk_strata_hotel_representatives_contact_id_contacts FOREIGN KEY (contact_id) REFERENCES public.contacts(id);


--
-- Name: strata_hotel_representatives_history fk_strata_hotel_representatives_contact_id_contacts; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives_history
    ADD CONSTRAINT fk_strata_hotel_representatives_contact_id_contacts FOREIGN KEY (contact_id) REFERENCES public.contacts(id);


--
-- Name: strata_hotel_representatives fk_strata_hotel_representatives_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives
    ADD CONSTRAINT fk_strata_hotel_representatives_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_representatives_history fk_strata_hotel_representatives_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives_history
    ADD CONSTRAINT fk_strata_hotel_representatives_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_representatives fk_strata_hotel_representatives_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives
    ADD CONSTRAINT fk_strata_hotel_representatives_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_representatives_history fk_strata_hotel_representatives_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives_history
    ADD CONSTRAINT fk_strata_hotel_representatives_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotel_representatives fk_strata_hotel_representatives_strata_hotel_id_strata_hotels; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives
    ADD CONSTRAINT fk_strata_hotel_representatives_strata_hotel_id_strata_hotels FOREIGN KEY (strata_hotel_id) REFERENCES public.strata_hotels(id);


--
-- Name: strata_hotel_representatives_history fk_strata_hotel_representatives_strata_hotel_id_strata_hotels; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotel_representatives_history
    ADD CONSTRAINT fk_strata_hotel_representatives_strata_hotel_id_strata_hotels FOREIGN KEY (strata_hotel_id) REFERENCES public.strata_hotels(id);


--
-- Name: strata_hotels fk_strata_hotels_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels
    ADD CONSTRAINT fk_strata_hotels_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotels_history fk_strata_hotels_created_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels_history
    ADD CONSTRAINT fk_strata_hotels_created_by_id_users FOREIGN KEY (created_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotels fk_strata_hotels_location_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels
    ADD CONSTRAINT fk_strata_hotels_location_id_addresses FOREIGN KEY (location_id) REFERENCES public.addresses(id);


--
-- Name: strata_hotels_history fk_strata_hotels_location_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels_history
    ADD CONSTRAINT fk_strata_hotels_location_id_addresses FOREIGN KEY (location_id) REFERENCES public.addresses(id);


--
-- Name: strata_hotels fk_strata_hotels_mailing_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels
    ADD CONSTRAINT fk_strata_hotels_mailing_address_id_addresses FOREIGN KEY (mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: strata_hotels_history fk_strata_hotels_mailing_address_id_addresses; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels_history
    ADD CONSTRAINT fk_strata_hotels_mailing_address_id_addresses FOREIGN KEY (mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: strata_hotels fk_strata_hotels_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels
    ADD CONSTRAINT fk_strata_hotels_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotels_history fk_strata_hotels_modified_by_id_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels_history
    ADD CONSTRAINT fk_strata_hotels_modified_by_id_users FOREIGN KEY (modified_by_id) REFERENCES public.users(id);


--
-- Name: strata_hotels fk_strata_hotels_registered_office_attorney_mailing_add_364b; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels
    ADD CONSTRAINT fk_strata_hotels_registered_office_attorney_mailing_add_364b FOREIGN KEY (registered_office_attorney_mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: strata_hotels_history fk_strata_hotels_registered_office_attorney_mailing_add_364b; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.strata_hotels_history
    ADD CONSTRAINT fk_strata_hotels_registered_office_attorney_mailing_add_364b FOREIGN KEY (registered_office_attorney_mailing_address_id) REFERENCES public.addresses(id);


--
-- Name: ltsa ltsa_application_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ltsa
    ADD CONSTRAINT ltsa_application_id_fkey FOREIGN KEY (application_id) REFERENCES public.application(id) ON DELETE CASCADE;


--
-- Name: property_contacts property_contacts_contact_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts
    ADD CONSTRAINT property_contacts_contact_id_fkey FOREIGN KEY (contact_id) REFERENCES public.contacts(id);


--
-- Name: property_contacts property_contacts_property_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_contacts
    ADD CONSTRAINT property_contacts_property_id_fkey FOREIGN KEY (property_id) REFERENCES public.rental_properties(id) ON DELETE CASCADE;


--
-- Name: property_listings property_listings_property_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.property_listings
    ADD CONSTRAINT property_listings_property_id_fkey FOREIGN KEY (property_id) REFERENCES public.rental_properties(id) ON DELETE CASCADE;


--
-- Name: registrations registrations_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.registrations
    ADD CONSTRAINT registrations_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- Name: rental_properties rental_properties_address_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties
    ADD CONSTRAINT rental_properties_address_id_fkey FOREIGN KEY (address_id) REFERENCES public.addresses(id);


--
-- Name: rental_properties rental_properties_registration_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rental_properties
    ADD CONSTRAINT rental_properties_registration_id_fkey FOREIGN KEY (registration_id) REFERENCES public.registrations(id) ON DELETE CASCADE;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: -
--

GRANT ALL ON SCHEMA public TO cloudsqlsuperuser;
GRANT USAGE ON SCHEMA public TO readonly;
GRANT USAGE ON SCHEMA public TO readwrite;
GRANT ALL ON SCHEMA public TO admin;


--
-- Name: FUNCTION box2d_in(cstring); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box2d_in(cstring) TO readwrite;
GRANT ALL ON FUNCTION public.box2d_in(cstring) TO admin;


--
-- Name: FUNCTION box2d_out(public.box2d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box2d_out(public.box2d) TO readwrite;
GRANT ALL ON FUNCTION public.box2d_out(public.box2d) TO admin;


--
-- Name: FUNCTION box2df_in(cstring); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box2df_in(cstring) TO readwrite;
GRANT ALL ON FUNCTION public.box2df_in(cstring) TO admin;


--
-- Name: FUNCTION box2df_out(public.box2df); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box2df_out(public.box2df) TO readwrite;
GRANT ALL ON FUNCTION public.box2df_out(public.box2df) TO admin;


--
-- Name: FUNCTION box3d_in(cstring); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box3d_in(cstring) TO readwrite;
GRANT ALL ON FUNCTION public.box3d_in(cstring) TO admin;


--
-- Name: FUNCTION box3d_out(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box3d_out(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.box3d_out(public.box3d) TO admin;


--
-- Name: TYPE contacttype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.contacttype TO readwrite;
GRANT ALL ON TYPE public.contacttype TO admin;


--
-- Name: TYPE documenttype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.documenttype TO readwrite;
GRANT ALL ON TYPE public.documenttype TO admin;


--
-- Name: TYPE eventname; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.eventname TO admin WITH GRANT OPTION;


--
-- Name: TYPE eventtype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.eventtype TO readwrite;
GRANT ALL ON TYPE public.eventtype TO admin;


--
-- Name: FUNCTION geography_analyze(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_analyze(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_analyze(internal) TO admin;


--
-- Name: FUNCTION geography_in(cstring, oid, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_in(cstring, oid, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geography_in(cstring, oid, integer) TO admin;


--
-- Name: FUNCTION geography_out(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_out(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_out(public.geography) TO admin;


--
-- Name: FUNCTION geography_recv(internal, oid, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_recv(internal, oid, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geography_recv(internal, oid, integer) TO admin;


--
-- Name: FUNCTION geography_send(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_send(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_send(public.geography) TO admin;


--
-- Name: FUNCTION geography_typmod_in(cstring[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_typmod_in(cstring[]) TO readwrite;
GRANT ALL ON FUNCTION public.geography_typmod_in(cstring[]) TO admin;


--
-- Name: FUNCTION geography_typmod_out(integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_typmod_out(integer) TO readwrite;
GRANT ALL ON FUNCTION public.geography_typmod_out(integer) TO admin;


--
-- Name: FUNCTION geometry_analyze(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_analyze(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_analyze(internal) TO admin;


--
-- Name: FUNCTION geometry_in(cstring); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_in(cstring) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_in(cstring) TO admin;


--
-- Name: FUNCTION geometry_out(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_out(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_out(public.geometry) TO admin;


--
-- Name: FUNCTION geometry_recv(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_recv(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_recv(internal) TO admin;


--
-- Name: FUNCTION geometry_send(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_send(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_send(public.geometry) TO admin;


--
-- Name: FUNCTION geometry_typmod_in(cstring[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_typmod_in(cstring[]) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_typmod_in(cstring[]) TO admin;


--
-- Name: FUNCTION geometry_typmod_out(integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_typmod_out(integer) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_typmod_out(integer) TO admin;


--
-- Name: TYPE geometry_dump; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.geometry_dump TO readwrite;
GRANT ALL ON TYPE public.geometry_dump TO admin;


--
-- Name: FUNCTION gidx_in(cstring); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.gidx_in(cstring) TO readwrite;
GRANT ALL ON FUNCTION public.gidx_in(cstring) TO admin;


--
-- Name: FUNCTION gidx_out(public.gidx); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.gidx_out(public.gidx) TO readwrite;
GRANT ALL ON FUNCTION public.gidx_out(public.gidx) TO admin;


--
-- Name: TYPE hostresidence; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.hostresidence TO readwrite;
GRANT ALL ON TYPE public.hostresidence TO admin;


--
-- Name: TYPE hosttype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.hosttype TO admin WITH GRANT OPTION;


--
-- Name: TYPE listingsize; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.listingsize TO readwrite;
GRANT ALL ON TYPE public.listingsize TO admin;


--
-- Name: TYPE ownershiptype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.ownershiptype TO readwrite;
GRANT ALL ON TYPE public.ownershiptype TO admin;


--
-- Name: TYPE propertymanagertype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.propertymanagertype TO readwrite;
GRANT ALL ON TYPE public.propertymanagertype TO admin;


--
-- Name: TYPE propertytype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.propertytype TO readwrite;
GRANT ALL ON TYPE public.propertytype TO admin;


--
-- Name: TYPE registrationnocstatus; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.registrationnocstatus TO admin WITH GRANT OPTION;


--
-- Name: TYPE registrationstatus; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.registrationstatus TO readwrite;
GRANT ALL ON TYPE public.registrationstatus TO admin;


--
-- Name: TYPE registrationtype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.registrationtype TO readwrite;
GRANT ALL ON TYPE public.registrationtype TO admin;


--
-- Name: TYPE rentalspaceoption; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.rentalspaceoption TO admin WITH GRANT OPTION;


--
-- Name: TYPE rentalunitspacetype; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.rentalunitspacetype TO readwrite;
GRANT ALL ON TYPE public.rentalunitspacetype TO admin;


--
-- Name: FUNCTION spheroid_in(cstring); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.spheroid_in(cstring) TO readwrite;
GRANT ALL ON FUNCTION public.spheroid_in(cstring) TO admin;


--
-- Name: FUNCTION spheroid_out(public.spheroid); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.spheroid_out(public.spheroid) TO readwrite;
GRANT ALL ON FUNCTION public.spheroid_out(public.spheroid) TO admin;


--
-- Name: TYPE status; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.status TO readwrite;
GRANT ALL ON TYPE public.status TO admin;


--
-- Name: TYPE stratahotelcategory; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.stratahotelcategory TO readwrite;
GRANT ALL ON TYPE public.stratahotelcategory TO admin;


--
-- Name: TYPE valid_detail; Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON TYPE public.valid_detail TO readwrite;
GRANT ALL ON TYPE public.valid_detail TO admin;


--
-- Name: FUNCTION box3d(public.box2d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box3d(public.box2d) TO readwrite;
GRANT ALL ON FUNCTION public.box3d(public.box2d) TO admin;


--
-- Name: FUNCTION geometry(public.box2d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(public.box2d) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(public.box2d) TO admin;


--
-- Name: FUNCTION box(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.box(public.box3d) TO admin;


--
-- Name: FUNCTION box2d(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box2d(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.box2d(public.box3d) TO admin;


--
-- Name: FUNCTION geometry(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(public.box3d) TO admin;


--
-- Name: FUNCTION geography(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.geography(bytea) TO admin;


--
-- Name: FUNCTION geometry(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(bytea) TO admin;


--
-- Name: FUNCTION bytea(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.bytea(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.bytea(public.geography) TO admin;


--
-- Name: FUNCTION geography(public.geography, integer, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography(public.geography, integer, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.geography(public.geography, integer, boolean) TO admin;


--
-- Name: FUNCTION geometry(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(public.geography) TO admin;


--
-- Name: FUNCTION box(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.box(public.geometry) TO admin;


--
-- Name: FUNCTION box2d(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box2d(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.box2d(public.geometry) TO admin;


--
-- Name: FUNCTION box3d(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box3d(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.box3d(public.geometry) TO admin;


--
-- Name: FUNCTION bytea(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.bytea(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.bytea(public.geometry) TO admin;


--
-- Name: FUNCTION geography(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geography(public.geometry) TO admin;


--
-- Name: FUNCTION geometry(public.geometry, integer, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(public.geometry, integer, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(public.geometry, integer, boolean) TO admin;


--
-- Name: FUNCTION json(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.json(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.json(public.geometry) TO admin;


--
-- Name: FUNCTION jsonb(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.jsonb(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.jsonb(public.geometry) TO admin;


--
-- Name: FUNCTION path(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.path(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.path(public.geometry) TO admin;


--
-- Name: FUNCTION point(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.point(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.point(public.geometry) TO admin;


--
-- Name: FUNCTION polygon(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.polygon(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.polygon(public.geometry) TO admin;


--
-- Name: FUNCTION text(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.text(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.text(public.geometry) TO admin;


--
-- Name: FUNCTION geometry(path); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(path) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(path) TO admin;


--
-- Name: FUNCTION geometry(point); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(point) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(point) TO admin;


--
-- Name: FUNCTION geometry(polygon); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(polygon) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(polygon) TO admin;


--
-- Name: FUNCTION geometry(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry(text) TO readwrite;
GRANT ALL ON FUNCTION public.geometry(text) TO admin;


--
-- Name: FUNCTION pg_replication_origin_advance(text, pg_lsn); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_advance(text, pg_lsn) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_create(text); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_create(text) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_drop(text); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_drop(text) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_oid(text); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_oid(text) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_progress(text, boolean); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_progress(text, boolean) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_session_is_setup(); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_session_is_setup() TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_session_progress(boolean); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_session_progress(boolean) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_session_reset(); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_session_reset() TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_session_setup(text); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_session_setup(text) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_xact_reset(); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_xact_reset() TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_replication_origin_xact_setup(pg_lsn, timestamp with time zone); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_replication_origin_xact_setup(pg_lsn, timestamp with time zone) TO cloudsqlsuperuser;


--
-- Name: FUNCTION pg_show_replication_origin_status(OUT local_id oid, OUT external_id text, OUT remote_lsn pg_lsn, OUT local_lsn pg_lsn); Type: ACL; Schema: pg_catalog; Owner: -
--

GRANT ALL ON FUNCTION pg_catalog.pg_show_replication_origin_status(OUT local_id oid, OUT external_id text, OUT remote_lsn pg_lsn, OUT local_lsn pg_lsn) TO cloudsqlsuperuser;


--
-- Name: FUNCTION _postgis_deprecate(oldname text, newname text, version text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._postgis_deprecate(oldname text, newname text, version text) TO readwrite;
GRANT ALL ON FUNCTION public._postgis_deprecate(oldname text, newname text, version text) TO admin;


--
-- Name: FUNCTION _postgis_index_extent(tbl regclass, col text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._postgis_index_extent(tbl regclass, col text) TO readwrite;
GRANT ALL ON FUNCTION public._postgis_index_extent(tbl regclass, col text) TO admin;


--
-- Name: FUNCTION _postgis_join_selectivity(regclass, text, regclass, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._postgis_join_selectivity(regclass, text, regclass, text, text) TO readwrite;
GRANT ALL ON FUNCTION public._postgis_join_selectivity(regclass, text, regclass, text, text) TO admin;


--
-- Name: FUNCTION _postgis_pgsql_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._postgis_pgsql_version() TO readwrite;
GRANT ALL ON FUNCTION public._postgis_pgsql_version() TO admin;


--
-- Name: FUNCTION _postgis_scripts_pgsql_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._postgis_scripts_pgsql_version() TO readwrite;
GRANT ALL ON FUNCTION public._postgis_scripts_pgsql_version() TO admin;


--
-- Name: FUNCTION _postgis_selectivity(tbl regclass, att_name text, geom public.geometry, mode text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._postgis_selectivity(tbl regclass, att_name text, geom public.geometry, mode text) TO readwrite;
GRANT ALL ON FUNCTION public._postgis_selectivity(tbl regclass, att_name text, geom public.geometry, mode text) TO admin;


--
-- Name: FUNCTION _postgis_stats(tbl regclass, att_name text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._postgis_stats(tbl regclass, att_name text, text) TO readwrite;
GRANT ALL ON FUNCTION public._postgis_stats(tbl regclass, att_name text, text) TO admin;


--
-- Name: FUNCTION _st_3ddfullywithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_3ddfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public._st_3ddfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION _st_3ddwithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_3ddwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public._st_3ddwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION _st_3dintersects(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_3dintersects(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_3dintersects(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_asgml(integer, public.geometry, integer, integer, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_asgml(integer, public.geometry, integer, integer, text, text) TO readwrite;
GRANT ALL ON FUNCTION public._st_asgml(integer, public.geometry, integer, integer, text, text) TO admin;


--
-- Name: FUNCTION _st_asx3d(integer, public.geometry, integer, integer, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_asx3d(integer, public.geometry, integer, integer, text) TO readwrite;
GRANT ALL ON FUNCTION public._st_asx3d(integer, public.geometry, integer, integer, text) TO admin;


--
-- Name: FUNCTION _st_bestsrid(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_bestsrid(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public._st_bestsrid(public.geography) TO admin;


--
-- Name: FUNCTION _st_bestsrid(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_bestsrid(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public._st_bestsrid(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION _st_concavehull(param_inputgeom public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_concavehull(param_inputgeom public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_concavehull(param_inputgeom public.geometry) TO admin;


--
-- Name: FUNCTION _st_contains(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_contains(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_contains(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_containsproperly(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_containsproperly(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_containsproperly(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_coveredby(geog1 public.geography, geog2 public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_coveredby(geog1 public.geography, geog2 public.geography) TO readwrite;
GRANT ALL ON FUNCTION public._st_coveredby(geog1 public.geography, geog2 public.geography) TO admin;


--
-- Name: FUNCTION _st_coveredby(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_coveredby(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_coveredby(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_covers(geog1 public.geography, geog2 public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_covers(geog1 public.geography, geog2 public.geography) TO readwrite;
GRANT ALL ON FUNCTION public._st_covers(geog1 public.geography, geog2 public.geography) TO admin;


--
-- Name: FUNCTION _st_covers(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_covers(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_covers(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_crosses(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_crosses(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_crosses(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_dfullywithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_dfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public._st_dfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION _st_distancetree(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_distancetree(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public._st_distancetree(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION _st_distancetree(public.geography, public.geography, double precision, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_distancetree(public.geography, public.geography, double precision, boolean) TO readwrite;
GRANT ALL ON FUNCTION public._st_distancetree(public.geography, public.geography, double precision, boolean) TO admin;


--
-- Name: FUNCTION _st_distanceuncached(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_distanceuncached(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public._st_distanceuncached(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION _st_distanceuncached(public.geography, public.geography, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_distanceuncached(public.geography, public.geography, boolean) TO readwrite;
GRANT ALL ON FUNCTION public._st_distanceuncached(public.geography, public.geography, boolean) TO admin;


--
-- Name: FUNCTION _st_distanceuncached(public.geography, public.geography, double precision, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_distanceuncached(public.geography, public.geography, double precision, boolean) TO readwrite;
GRANT ALL ON FUNCTION public._st_distanceuncached(public.geography, public.geography, double precision, boolean) TO admin;


--
-- Name: FUNCTION _st_dwithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_dwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public._st_dwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION _st_dwithin(geog1 public.geography, geog2 public.geography, tolerance double precision, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_dwithin(geog1 public.geography, geog2 public.geography, tolerance double precision, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public._st_dwithin(geog1 public.geography, geog2 public.geography, tolerance double precision, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION _st_dwithinuncached(public.geography, public.geography, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_dwithinuncached(public.geography, public.geography, double precision) TO readwrite;
GRANT ALL ON FUNCTION public._st_dwithinuncached(public.geography, public.geography, double precision) TO admin;


--
-- Name: FUNCTION _st_dwithinuncached(public.geography, public.geography, double precision, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_dwithinuncached(public.geography, public.geography, double precision, boolean) TO readwrite;
GRANT ALL ON FUNCTION public._st_dwithinuncached(public.geography, public.geography, double precision, boolean) TO admin;


--
-- Name: FUNCTION _st_equals(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_equals(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_equals(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_expand(public.geography, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_expand(public.geography, double precision) TO readwrite;
GRANT ALL ON FUNCTION public._st_expand(public.geography, double precision) TO admin;


--
-- Name: FUNCTION _st_geomfromgml(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_geomfromgml(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public._st_geomfromgml(text, integer) TO admin;


--
-- Name: FUNCTION _st_intersects(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_intersects(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_intersects(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_linecrossingdirection(line1 public.geometry, line2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_linecrossingdirection(line1 public.geometry, line2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_linecrossingdirection(line1 public.geometry, line2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_longestline(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_longestline(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_longestline(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_maxdistance(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_maxdistance(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_maxdistance(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_orderingequals(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_orderingequals(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_orderingequals(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_overlaps(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_overlaps(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_overlaps(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_pointoutside(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_pointoutside(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public._st_pointoutside(public.geography) TO admin;


--
-- Name: FUNCTION _st_sortablehash(geom public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_sortablehash(geom public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_sortablehash(geom public.geometry) TO admin;


--
-- Name: FUNCTION _st_touches(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_touches(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_touches(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION _st_voronoi(g1 public.geometry, clip public.geometry, tolerance double precision, return_polygons boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_voronoi(g1 public.geometry, clip public.geometry, tolerance double precision, return_polygons boolean) TO readwrite;
GRANT ALL ON FUNCTION public._st_voronoi(g1 public.geometry, clip public.geometry, tolerance double precision, return_polygons boolean) TO admin;


--
-- Name: FUNCTION _st_within(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public._st_within(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public._st_within(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION addauth(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.addauth(text) TO readwrite;
GRANT ALL ON FUNCTION public.addauth(text) TO admin;


--
-- Name: FUNCTION addgeometrycolumn(table_name character varying, column_name character varying, new_srid integer, new_type character varying, new_dim integer, use_typmod boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.addgeometrycolumn(table_name character varying, column_name character varying, new_srid integer, new_type character varying, new_dim integer, use_typmod boolean) TO readwrite;
GRANT ALL ON FUNCTION public.addgeometrycolumn(table_name character varying, column_name character varying, new_srid integer, new_type character varying, new_dim integer, use_typmod boolean) TO admin;


--
-- Name: FUNCTION addgeometrycolumn(schema_name character varying, table_name character varying, column_name character varying, new_srid integer, new_type character varying, new_dim integer, use_typmod boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.addgeometrycolumn(schema_name character varying, table_name character varying, column_name character varying, new_srid integer, new_type character varying, new_dim integer, use_typmod boolean) TO readwrite;
GRANT ALL ON FUNCTION public.addgeometrycolumn(schema_name character varying, table_name character varying, column_name character varying, new_srid integer, new_type character varying, new_dim integer, use_typmod boolean) TO admin;


--
-- Name: FUNCTION addgeometrycolumn(catalog_name character varying, schema_name character varying, table_name character varying, column_name character varying, new_srid_in integer, new_type character varying, new_dim integer, use_typmod boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.addgeometrycolumn(catalog_name character varying, schema_name character varying, table_name character varying, column_name character varying, new_srid_in integer, new_type character varying, new_dim integer, use_typmod boolean) TO readwrite;
GRANT ALL ON FUNCTION public.addgeometrycolumn(catalog_name character varying, schema_name character varying, table_name character varying, column_name character varying, new_srid_in integer, new_type character varying, new_dim integer, use_typmod boolean) TO admin;


--
-- Name: FUNCTION box3dtobox(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.box3dtobox(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.box3dtobox(public.box3d) TO admin;


--
-- Name: FUNCTION checkauth(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.checkauth(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.checkauth(text, text) TO admin;


--
-- Name: FUNCTION checkauth(text, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.checkauth(text, text, text) TO readwrite;
GRANT ALL ON FUNCTION public.checkauth(text, text, text) TO admin;


--
-- Name: FUNCTION checkauthtrigger(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.checkauthtrigger() TO readwrite;
GRANT ALL ON FUNCTION public.checkauthtrigger() TO admin;


--
-- Name: FUNCTION contains_2d(public.box2df, public.box2df); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.contains_2d(public.box2df, public.box2df) TO readwrite;
GRANT ALL ON FUNCTION public.contains_2d(public.box2df, public.box2df) TO admin;


--
-- Name: FUNCTION contains_2d(public.box2df, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.contains_2d(public.box2df, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.contains_2d(public.box2df, public.geometry) TO admin;


--
-- Name: FUNCTION contains_2d(public.geometry, public.box2df); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.contains_2d(public.geometry, public.box2df) TO readwrite;
GRANT ALL ON FUNCTION public.contains_2d(public.geometry, public.box2df) TO admin;


--
-- Name: FUNCTION disablelongtransactions(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.disablelongtransactions() TO readwrite;
GRANT ALL ON FUNCTION public.disablelongtransactions() TO admin;


--
-- Name: FUNCTION dropgeometrycolumn(table_name character varying, column_name character varying); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dropgeometrycolumn(table_name character varying, column_name character varying) TO readwrite;
GRANT ALL ON FUNCTION public.dropgeometrycolumn(table_name character varying, column_name character varying) TO admin;


--
-- Name: FUNCTION dropgeometrycolumn(schema_name character varying, table_name character varying, column_name character varying); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dropgeometrycolumn(schema_name character varying, table_name character varying, column_name character varying) TO readwrite;
GRANT ALL ON FUNCTION public.dropgeometrycolumn(schema_name character varying, table_name character varying, column_name character varying) TO admin;


--
-- Name: FUNCTION dropgeometrycolumn(catalog_name character varying, schema_name character varying, table_name character varying, column_name character varying); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dropgeometrycolumn(catalog_name character varying, schema_name character varying, table_name character varying, column_name character varying) TO readwrite;
GRANT ALL ON FUNCTION public.dropgeometrycolumn(catalog_name character varying, schema_name character varying, table_name character varying, column_name character varying) TO admin;


--
-- Name: FUNCTION dropgeometrytable(table_name character varying); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dropgeometrytable(table_name character varying) TO readwrite;
GRANT ALL ON FUNCTION public.dropgeometrytable(table_name character varying) TO admin;


--
-- Name: FUNCTION dropgeometrytable(schema_name character varying, table_name character varying); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dropgeometrytable(schema_name character varying, table_name character varying) TO readwrite;
GRANT ALL ON FUNCTION public.dropgeometrytable(schema_name character varying, table_name character varying) TO admin;


--
-- Name: FUNCTION dropgeometrytable(catalog_name character varying, schema_name character varying, table_name character varying); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dropgeometrytable(catalog_name character varying, schema_name character varying, table_name character varying) TO readwrite;
GRANT ALL ON FUNCTION public.dropgeometrytable(catalog_name character varying, schema_name character varying, table_name character varying) TO admin;


--
-- Name: FUNCTION postgis_index_supportfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_index_supportfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_index_supportfn(internal) TO admin;


--
-- Name: FUNCTION st_area(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_area(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_area(public.geometry) TO admin;


--
-- Name: FUNCTION st_intersects(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_intersects(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_intersects(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: TABLE dss_organization; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.dss_organization TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.dss_organization TO readwrite;
GRANT ALL ON TABLE public.dss_organization TO admin;


--
-- Name: FUNCTION dss_containing_organization_id(p_point public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dss_containing_organization_id(p_point public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.dss_containing_organization_id(p_point public.geometry) TO admin;


--
-- Name: FUNCTION dss_update_audit_columns(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.dss_update_audit_columns() TO readwrite;
GRANT ALL ON FUNCTION public.dss_update_audit_columns() TO admin;


--
-- Name: FUNCTION enablelongtransactions(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.enablelongtransactions() TO readwrite;
GRANT ALL ON FUNCTION public.enablelongtransactions() TO admin;


--
-- Name: FUNCTION equals(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.equals(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.equals(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION find_srid(character varying, character varying, character varying); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.find_srid(character varying, character varying, character varying) TO readwrite;
GRANT ALL ON FUNCTION public.find_srid(character varying, character varying, character varying) TO admin;


--
-- Name: FUNCTION geog_brin_inclusion_add_value(internal, internal, internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geog_brin_inclusion_add_value(internal, internal, internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geog_brin_inclusion_add_value(internal, internal, internal, internal) TO admin;


--
-- Name: FUNCTION geography_cmp(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_cmp(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_cmp(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_distance_knn(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_distance_knn(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_distance_knn(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_eq(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_eq(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_eq(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_ge(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_ge(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_ge(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_gist_compress(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_compress(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_compress(internal) TO admin;


--
-- Name: FUNCTION geography_gist_consistent(internal, public.geography, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_consistent(internal, public.geography, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_consistent(internal, public.geography, integer) TO admin;


--
-- Name: FUNCTION geography_gist_decompress(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_decompress(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_decompress(internal) TO admin;


--
-- Name: FUNCTION geography_gist_distance(internal, public.geography, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_distance(internal, public.geography, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_distance(internal, public.geography, integer) TO admin;


--
-- Name: FUNCTION geography_gist_penalty(internal, internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_penalty(internal, internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_penalty(internal, internal, internal) TO admin;


--
-- Name: FUNCTION geography_gist_picksplit(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_picksplit(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_picksplit(internal, internal) TO admin;


--
-- Name: FUNCTION geography_gist_same(public.box2d, public.box2d, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_same(public.box2d, public.box2d, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_same(public.box2d, public.box2d, internal) TO admin;


--
-- Name: FUNCTION geography_gist_union(bytea, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gist_union(bytea, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gist_union(bytea, internal) TO admin;


--
-- Name: FUNCTION geography_gt(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_gt(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_gt(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_le(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_le(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_le(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_lt(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_lt(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_lt(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_overlaps(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_overlaps(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geography_overlaps(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION geography_spgist_choose_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_spgist_choose_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_spgist_choose_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geography_spgist_compress_nd(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_spgist_compress_nd(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_spgist_compress_nd(internal) TO admin;


--
-- Name: FUNCTION geography_spgist_config_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_spgist_config_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_spgist_config_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geography_spgist_inner_consistent_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_spgist_inner_consistent_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_spgist_inner_consistent_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geography_spgist_leaf_consistent_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_spgist_leaf_consistent_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_spgist_leaf_consistent_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geography_spgist_picksplit_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geography_spgist_picksplit_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geography_spgist_picksplit_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geom2d_brin_inclusion_add_value(internal, internal, internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geom2d_brin_inclusion_add_value(internal, internal, internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geom2d_brin_inclusion_add_value(internal, internal, internal, internal) TO admin;


--
-- Name: FUNCTION geom3d_brin_inclusion_add_value(internal, internal, internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geom3d_brin_inclusion_add_value(internal, internal, internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geom3d_brin_inclusion_add_value(internal, internal, internal, internal) TO admin;


--
-- Name: FUNCTION geom4d_brin_inclusion_add_value(internal, internal, internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geom4d_brin_inclusion_add_value(internal, internal, internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geom4d_brin_inclusion_add_value(internal, internal, internal, internal) TO admin;


--
-- Name: FUNCTION geometry_above(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_above(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_above(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_below(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_below(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_below(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_cmp(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_cmp(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_cmp(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_contained_3d(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_contained_3d(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_contained_3d(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_contains(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_contains(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_contains(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_contains_3d(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_contains_3d(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_contains_3d(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_contains_nd(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_contains_nd(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_contains_nd(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION geometry_distance_box(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_distance_box(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_distance_box(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_distance_centroid(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_distance_centroid(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_distance_centroid(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_distance_centroid_nd(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_distance_centroid_nd(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_distance_centroid_nd(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION geometry_distance_cpa(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_distance_cpa(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_distance_cpa(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION geometry_eq(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_eq(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_eq(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_ge(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_ge(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_ge(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_gist_compress_2d(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_compress_2d(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_compress_2d(internal) TO admin;


--
-- Name: FUNCTION geometry_gist_compress_nd(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_compress_nd(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_compress_nd(internal) TO admin;


--
-- Name: FUNCTION geometry_gist_consistent_2d(internal, public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_consistent_2d(internal, public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_consistent_2d(internal, public.geometry, integer) TO admin;


--
-- Name: FUNCTION geometry_gist_consistent_nd(internal, public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_consistent_nd(internal, public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_consistent_nd(internal, public.geometry, integer) TO admin;


--
-- Name: FUNCTION geometry_gist_decompress_2d(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_decompress_2d(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_decompress_2d(internal) TO admin;


--
-- Name: FUNCTION geometry_gist_decompress_nd(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_decompress_nd(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_decompress_nd(internal) TO admin;


--
-- Name: FUNCTION geometry_gist_distance_2d(internal, public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_distance_2d(internal, public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_distance_2d(internal, public.geometry, integer) TO admin;


--
-- Name: FUNCTION geometry_gist_distance_nd(internal, public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_distance_nd(internal, public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_distance_nd(internal, public.geometry, integer) TO admin;


--
-- Name: FUNCTION geometry_gist_penalty_2d(internal, internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_penalty_2d(internal, internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_penalty_2d(internal, internal, internal) TO admin;


--
-- Name: FUNCTION geometry_gist_penalty_nd(internal, internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_penalty_nd(internal, internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_penalty_nd(internal, internal, internal) TO admin;


--
-- Name: FUNCTION geometry_gist_picksplit_2d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_picksplit_2d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_picksplit_2d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_gist_picksplit_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_picksplit_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_picksplit_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_gist_same_2d(geom1 public.geometry, geom2 public.geometry, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_same_2d(geom1 public.geometry, geom2 public.geometry, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_same_2d(geom1 public.geometry, geom2 public.geometry, internal) TO admin;


--
-- Name: FUNCTION geometry_gist_same_nd(public.geometry, public.geometry, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_same_nd(public.geometry, public.geometry, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_same_nd(public.geometry, public.geometry, internal) TO admin;


--
-- Name: FUNCTION geometry_gist_sortsupport_2d(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_sortsupport_2d(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_sortsupport_2d(internal) TO admin;


--
-- Name: FUNCTION geometry_gist_union_2d(bytea, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_union_2d(bytea, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_union_2d(bytea, internal) TO admin;


--
-- Name: FUNCTION geometry_gist_union_nd(bytea, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gist_union_nd(bytea, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gist_union_nd(bytea, internal) TO admin;


--
-- Name: FUNCTION geometry_gt(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_gt(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_gt(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_hash(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_hash(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_hash(public.geometry) TO admin;


--
-- Name: FUNCTION geometry_le(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_le(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_le(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_left(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_left(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_left(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_lt(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_lt(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_lt(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_overabove(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_overabove(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_overabove(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_overbelow(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_overbelow(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_overbelow(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_overlaps(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_overlaps(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_overlaps(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_overlaps_3d(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_overlaps_3d(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_overlaps_3d(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_overlaps_nd(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_overlaps_nd(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_overlaps_nd(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION geometry_overleft(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_overleft(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_overleft(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_overright(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_overright(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_overright(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_right(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_right(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_right(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_same(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_same(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_same(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_same_3d(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_same_3d(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_same_3d(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_same_nd(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_same_nd(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_same_nd(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION geometry_sortsupport(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_sortsupport(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_sortsupport(internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_choose_2d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_choose_2d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_choose_2d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_choose_3d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_choose_3d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_choose_3d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_choose_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_choose_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_choose_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_compress_2d(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_compress_2d(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_compress_2d(internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_compress_3d(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_compress_3d(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_compress_3d(internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_compress_nd(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_compress_nd(internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_compress_nd(internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_config_2d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_config_2d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_config_2d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_config_3d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_config_3d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_config_3d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_config_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_config_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_config_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_inner_consistent_2d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_inner_consistent_2d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_inner_consistent_2d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_inner_consistent_3d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_inner_consistent_3d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_inner_consistent_3d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_inner_consistent_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_inner_consistent_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_inner_consistent_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_leaf_consistent_2d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_leaf_consistent_2d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_leaf_consistent_2d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_leaf_consistent_3d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_leaf_consistent_3d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_leaf_consistent_3d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_leaf_consistent_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_leaf_consistent_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_leaf_consistent_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_picksplit_2d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_picksplit_2d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_picksplit_2d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_picksplit_3d(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_picksplit_3d(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_picksplit_3d(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_spgist_picksplit_nd(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_spgist_picksplit_nd(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_spgist_picksplit_nd(internal, internal) TO admin;


--
-- Name: FUNCTION geometry_within(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_within(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_within(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION geometry_within_nd(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometry_within_nd(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometry_within_nd(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION geometrytype(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometrytype(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.geometrytype(public.geography) TO admin;


--
-- Name: FUNCTION geometrytype(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geometrytype(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.geometrytype(public.geometry) TO admin;


--
-- Name: FUNCTION geomfromewkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geomfromewkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.geomfromewkb(bytea) TO admin;


--
-- Name: FUNCTION geomfromewkt(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.geomfromewkt(text) TO readwrite;
GRANT ALL ON FUNCTION public.geomfromewkt(text) TO admin;


--
-- Name: FUNCTION get_proj4_from_srid(integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.get_proj4_from_srid(integer) TO readwrite;
GRANT ALL ON FUNCTION public.get_proj4_from_srid(integer) TO admin;


--
-- Name: FUNCTION gettransactionid(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.gettransactionid() TO readwrite;
GRANT ALL ON FUNCTION public.gettransactionid() TO admin;


--
-- Name: FUNCTION gserialized_gist_joinsel_2d(internal, oid, internal, smallint); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.gserialized_gist_joinsel_2d(internal, oid, internal, smallint) TO readwrite;
GRANT ALL ON FUNCTION public.gserialized_gist_joinsel_2d(internal, oid, internal, smallint) TO admin;


--
-- Name: FUNCTION gserialized_gist_joinsel_nd(internal, oid, internal, smallint); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.gserialized_gist_joinsel_nd(internal, oid, internal, smallint) TO readwrite;
GRANT ALL ON FUNCTION public.gserialized_gist_joinsel_nd(internal, oid, internal, smallint) TO admin;


--
-- Name: FUNCTION gserialized_gist_sel_2d(internal, oid, internal, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.gserialized_gist_sel_2d(internal, oid, internal, integer) TO readwrite;
GRANT ALL ON FUNCTION public.gserialized_gist_sel_2d(internal, oid, internal, integer) TO admin;


--
-- Name: FUNCTION gserialized_gist_sel_nd(internal, oid, internal, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.gserialized_gist_sel_nd(internal, oid, internal, integer) TO readwrite;
GRANT ALL ON FUNCTION public.gserialized_gist_sel_nd(internal, oid, internal, integer) TO admin;


--
-- Name: FUNCTION is_contained_2d(public.box2df, public.box2df); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.is_contained_2d(public.box2df, public.box2df) TO readwrite;
GRANT ALL ON FUNCTION public.is_contained_2d(public.box2df, public.box2df) TO admin;


--
-- Name: FUNCTION is_contained_2d(public.box2df, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.is_contained_2d(public.box2df, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.is_contained_2d(public.box2df, public.geometry) TO admin;


--
-- Name: FUNCTION is_contained_2d(public.geometry, public.box2df); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.is_contained_2d(public.geometry, public.box2df) TO readwrite;
GRANT ALL ON FUNCTION public.is_contained_2d(public.geometry, public.box2df) TO admin;


--
-- Name: FUNCTION lockrow(text, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.lockrow(text, text, text) TO readwrite;
GRANT ALL ON FUNCTION public.lockrow(text, text, text) TO admin;


--
-- Name: FUNCTION lockrow(text, text, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.lockrow(text, text, text, text) TO readwrite;
GRANT ALL ON FUNCTION public.lockrow(text, text, text, text) TO admin;


--
-- Name: FUNCTION lockrow(text, text, text, timestamp without time zone); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.lockrow(text, text, text, timestamp without time zone) TO readwrite;
GRANT ALL ON FUNCTION public.lockrow(text, text, text, timestamp without time zone) TO admin;


--
-- Name: FUNCTION lockrow(text, text, text, text, timestamp without time zone); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.lockrow(text, text, text, text, timestamp without time zone) TO readwrite;
GRANT ALL ON FUNCTION public.lockrow(text, text, text, text, timestamp without time zone) TO admin;


--
-- Name: FUNCTION longtransactionsenabled(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.longtransactionsenabled() TO readwrite;
GRANT ALL ON FUNCTION public.longtransactionsenabled() TO admin;


--
-- Name: FUNCTION overlaps_2d(public.box2df, public.box2df); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_2d(public.box2df, public.box2df) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_2d(public.box2df, public.box2df) TO admin;


--
-- Name: FUNCTION overlaps_2d(public.box2df, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_2d(public.box2df, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_2d(public.box2df, public.geometry) TO admin;


--
-- Name: FUNCTION overlaps_2d(public.geometry, public.box2df); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_2d(public.geometry, public.box2df) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_2d(public.geometry, public.box2df) TO admin;


--
-- Name: FUNCTION overlaps_geog(public.geography, public.gidx); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_geog(public.geography, public.gidx) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_geog(public.geography, public.gidx) TO admin;


--
-- Name: FUNCTION overlaps_geog(public.gidx, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_geog(public.gidx, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_geog(public.gidx, public.geography) TO admin;


--
-- Name: FUNCTION overlaps_geog(public.gidx, public.gidx); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_geog(public.gidx, public.gidx) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_geog(public.gidx, public.gidx) TO admin;


--
-- Name: FUNCTION overlaps_nd(public.geometry, public.gidx); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_nd(public.geometry, public.gidx) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_nd(public.geometry, public.gidx) TO admin;


--
-- Name: FUNCTION overlaps_nd(public.gidx, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_nd(public.gidx, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_nd(public.gidx, public.geometry) TO admin;


--
-- Name: FUNCTION overlaps_nd(public.gidx, public.gidx); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.overlaps_nd(public.gidx, public.gidx) TO readwrite;
GRANT ALL ON FUNCTION public.overlaps_nd(public.gidx, public.gidx) TO admin;


--
-- Name: FUNCTION pgis_asflatgeobuf_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_asflatgeobuf_transfn(internal, anyelement); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_transfn(internal, anyelement) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_transfn(internal, anyelement) TO admin;


--
-- Name: FUNCTION pgis_asflatgeobuf_transfn(internal, anyelement, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_transfn(internal, anyelement, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_transfn(internal, anyelement, boolean) TO admin;


--
-- Name: FUNCTION pgis_asflatgeobuf_transfn(internal, anyelement, boolean, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_transfn(internal, anyelement, boolean, text) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asflatgeobuf_transfn(internal, anyelement, boolean, text) TO admin;


--
-- Name: FUNCTION pgis_asgeobuf_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asgeobuf_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asgeobuf_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_asgeobuf_transfn(internal, anyelement); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asgeobuf_transfn(internal, anyelement) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asgeobuf_transfn(internal, anyelement) TO admin;


--
-- Name: FUNCTION pgis_asgeobuf_transfn(internal, anyelement, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asgeobuf_transfn(internal, anyelement, text) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asgeobuf_transfn(internal, anyelement, text) TO admin;


--
-- Name: FUNCTION pgis_asmvt_combinefn(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_combinefn(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_combinefn(internal, internal) TO admin;


--
-- Name: FUNCTION pgis_asmvt_deserialfn(bytea, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_deserialfn(bytea, internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_deserialfn(bytea, internal) TO admin;


--
-- Name: FUNCTION pgis_asmvt_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_asmvt_serialfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_serialfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_serialfn(internal) TO admin;


--
-- Name: FUNCTION pgis_asmvt_transfn(internal, anyelement); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement) TO admin;


--
-- Name: FUNCTION pgis_asmvt_transfn(internal, anyelement, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text) TO admin;


--
-- Name: FUNCTION pgis_asmvt_transfn(internal, anyelement, text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text, integer) TO admin;


--
-- Name: FUNCTION pgis_asmvt_transfn(internal, anyelement, text, integer, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text, integer, text) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text, integer, text) TO admin;


--
-- Name: FUNCTION pgis_asmvt_transfn(internal, anyelement, text, integer, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text, integer, text, text) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_asmvt_transfn(internal, anyelement, text, integer, text, text) TO admin;


--
-- Name: FUNCTION pgis_geometry_accum_transfn(internal, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_accum_transfn(internal, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_accum_transfn(internal, public.geometry) TO admin;


--
-- Name: FUNCTION pgis_geometry_accum_transfn(internal, public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_accum_transfn(internal, public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_accum_transfn(internal, public.geometry, double precision) TO admin;


--
-- Name: FUNCTION pgis_geometry_accum_transfn(internal, public.geometry, double precision, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_accum_transfn(internal, public.geometry, double precision, integer) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_accum_transfn(internal, public.geometry, double precision, integer) TO admin;


--
-- Name: FUNCTION pgis_geometry_clusterintersecting_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_clusterintersecting_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_clusterintersecting_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_clusterwithin_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_clusterwithin_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_clusterwithin_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_collect_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_collect_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_collect_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_coverageunion_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_coverageunion_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_coverageunion_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_makeline_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_makeline_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_makeline_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_polygonize_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_polygonize_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_polygonize_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_union_parallel_combinefn(internal, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_combinefn(internal, internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_combinefn(internal, internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_union_parallel_deserialfn(bytea, internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_deserialfn(bytea, internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_deserialfn(bytea, internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_union_parallel_finalfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_finalfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_finalfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_union_parallel_serialfn(internal); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_serialfn(internal) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_serialfn(internal) TO admin;


--
-- Name: FUNCTION pgis_geometry_union_parallel_transfn(internal, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_transfn(internal, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_transfn(internal, public.geometry) TO admin;


--
-- Name: FUNCTION pgis_geometry_union_parallel_transfn(internal, public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_transfn(internal, public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.pgis_geometry_union_parallel_transfn(internal, public.geometry, double precision) TO admin;


--
-- Name: FUNCTION populate_geometry_columns(use_typmod boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.populate_geometry_columns(use_typmod boolean) TO readwrite;
GRANT ALL ON FUNCTION public.populate_geometry_columns(use_typmod boolean) TO admin;


--
-- Name: FUNCTION populate_geometry_columns(tbl_oid oid, use_typmod boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.populate_geometry_columns(tbl_oid oid, use_typmod boolean) TO readwrite;
GRANT ALL ON FUNCTION public.populate_geometry_columns(tbl_oid oid, use_typmod boolean) TO admin;


--
-- Name: FUNCTION postgis_addbbox(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_addbbox(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_addbbox(public.geometry) TO admin;


--
-- Name: FUNCTION postgis_cache_bbox(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_cache_bbox() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_cache_bbox() TO admin;


--
-- Name: FUNCTION postgis_constraint_dims(geomschema text, geomtable text, geomcolumn text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_constraint_dims(geomschema text, geomtable text, geomcolumn text) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_constraint_dims(geomschema text, geomtable text, geomcolumn text) TO admin;


--
-- Name: FUNCTION postgis_constraint_srid(geomschema text, geomtable text, geomcolumn text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_constraint_srid(geomschema text, geomtable text, geomcolumn text) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_constraint_srid(geomschema text, geomtable text, geomcolumn text) TO admin;


--
-- Name: FUNCTION postgis_constraint_type(geomschema text, geomtable text, geomcolumn text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_constraint_type(geomschema text, geomtable text, geomcolumn text) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_constraint_type(geomschema text, geomtable text, geomcolumn text) TO admin;


--
-- Name: FUNCTION postgis_dropbbox(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_dropbbox(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_dropbbox(public.geometry) TO admin;


--
-- Name: FUNCTION postgis_extensions_upgrade(target_version text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_extensions_upgrade(target_version text) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_extensions_upgrade(target_version text) TO admin;


--
-- Name: FUNCTION postgis_full_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_full_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_full_version() TO admin;


--
-- Name: FUNCTION postgis_geos_compiled_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_geos_compiled_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_geos_compiled_version() TO admin;


--
-- Name: FUNCTION postgis_geos_noop(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_geos_noop(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_geos_noop(public.geometry) TO admin;


--
-- Name: FUNCTION postgis_geos_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_geos_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_geos_version() TO admin;


--
-- Name: FUNCTION postgis_getbbox(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_getbbox(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_getbbox(public.geometry) TO admin;


--
-- Name: FUNCTION postgis_hasbbox(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_hasbbox(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_hasbbox(public.geometry) TO admin;


--
-- Name: FUNCTION postgis_lib_build_date(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_lib_build_date() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_lib_build_date() TO admin;


--
-- Name: FUNCTION postgis_lib_revision(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_lib_revision() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_lib_revision() TO admin;


--
-- Name: FUNCTION postgis_lib_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_lib_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_lib_version() TO admin;


--
-- Name: FUNCTION postgis_libjson_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_libjson_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_libjson_version() TO admin;


--
-- Name: FUNCTION postgis_liblwgeom_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_liblwgeom_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_liblwgeom_version() TO admin;


--
-- Name: FUNCTION postgis_libprotobuf_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_libprotobuf_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_libprotobuf_version() TO admin;


--
-- Name: FUNCTION postgis_libxml_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_libxml_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_libxml_version() TO admin;


--
-- Name: FUNCTION postgis_noop(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_noop(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_noop(public.geometry) TO admin;


--
-- Name: FUNCTION postgis_proj_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_proj_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_proj_version() TO admin;


--
-- Name: FUNCTION postgis_scripts_build_date(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_scripts_build_date() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_scripts_build_date() TO admin;


--
-- Name: FUNCTION postgis_scripts_installed(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_scripts_installed() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_scripts_installed() TO admin;


--
-- Name: FUNCTION postgis_scripts_released(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_scripts_released() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_scripts_released() TO admin;


--
-- Name: FUNCTION postgis_srs(auth_name text, auth_srid text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_srs(auth_name text, auth_srid text) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_srs(auth_name text, auth_srid text) TO admin;


--
-- Name: FUNCTION postgis_srs_all(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_srs_all() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_srs_all() TO admin;


--
-- Name: FUNCTION postgis_srs_codes(auth_name text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_srs_codes(auth_name text) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_srs_codes(auth_name text) TO admin;


--
-- Name: FUNCTION postgis_srs_search(bounds public.geometry, authname text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_srs_search(bounds public.geometry, authname text) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_srs_search(bounds public.geometry, authname text) TO admin;


--
-- Name: FUNCTION postgis_svn_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_svn_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_svn_version() TO admin;


--
-- Name: FUNCTION postgis_transform_geometry(geom public.geometry, text, text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_transform_geometry(geom public.geometry, text, text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_transform_geometry(geom public.geometry, text, text, integer) TO admin;


--
-- Name: FUNCTION postgis_transform_pipeline_geometry(geom public.geometry, pipeline text, forward boolean, to_srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_transform_pipeline_geometry(geom public.geometry, pipeline text, forward boolean, to_srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_transform_pipeline_geometry(geom public.geometry, pipeline text, forward boolean, to_srid integer) TO admin;


--
-- Name: FUNCTION postgis_type_name(geomname character varying, coord_dimension integer, use_new_name boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_type_name(geomname character varying, coord_dimension integer, use_new_name boolean) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_type_name(geomname character varying, coord_dimension integer, use_new_name boolean) TO admin;


--
-- Name: FUNCTION postgis_typmod_dims(integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_typmod_dims(integer) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_typmod_dims(integer) TO admin;


--
-- Name: FUNCTION postgis_typmod_srid(integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_typmod_srid(integer) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_typmod_srid(integer) TO admin;


--
-- Name: FUNCTION postgis_typmod_type(integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_typmod_type(integer) TO readwrite;
GRANT ALL ON FUNCTION public.postgis_typmod_type(integer) TO admin;


--
-- Name: FUNCTION postgis_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_version() TO admin;


--
-- Name: FUNCTION postgis_wagyu_version(); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.postgis_wagyu_version() TO readwrite;
GRANT ALL ON FUNCTION public.postgis_wagyu_version() TO admin;


--
-- Name: FUNCTION st_3dclosestpoint(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dclosestpoint(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dclosestpoint(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_3ddfullywithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3ddfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_3ddfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_3ddistance(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3ddistance(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3ddistance(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_3ddwithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3ddwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_3ddwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_3dintersects(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dintersects(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dintersects(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_3dlength(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dlength(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dlength(public.geometry) TO admin;


--
-- Name: FUNCTION st_3dlineinterpolatepoint(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dlineinterpolatepoint(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dlineinterpolatepoint(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_3dlongestline(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dlongestline(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dlongestline(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_3dmakebox(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dmakebox(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dmakebox(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_3dmaxdistance(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dmaxdistance(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dmaxdistance(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_3dperimeter(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dperimeter(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dperimeter(public.geometry) TO admin;


--
-- Name: FUNCTION st_3dshortestline(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dshortestline(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dshortestline(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_addmeasure(public.geometry, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_addmeasure(public.geometry, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_addmeasure(public.geometry, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_addpoint(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_addpoint(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_addpoint(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_addpoint(geom1 public.geometry, geom2 public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_addpoint(geom1 public.geometry, geom2 public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_addpoint(geom1 public.geometry, geom2 public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_affine(public.geometry, double precision, double precision, double precision, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_affine(public.geometry, double precision, double precision, double precision, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_affine(public.geometry, double precision, double precision, double precision, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_affine(public.geometry, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_affine(public.geometry, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_affine(public.geometry, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_angle(line1 public.geometry, line2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_angle(line1 public.geometry, line2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_angle(line1 public.geometry, line2 public.geometry) TO admin;


--
-- Name: FUNCTION st_angle(pt1 public.geometry, pt2 public.geometry, pt3 public.geometry, pt4 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_angle(pt1 public.geometry, pt2 public.geometry, pt3 public.geometry, pt4 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_angle(pt1 public.geometry, pt2 public.geometry, pt3 public.geometry, pt4 public.geometry) TO admin;


--
-- Name: FUNCTION st_area(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_area(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_area(text) TO admin;


--
-- Name: FUNCTION st_area(geog public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_area(geog public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_area(geog public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_area2d(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_area2d(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_area2d(public.geometry) TO admin;


--
-- Name: FUNCTION st_asbinary(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asbinary(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_asbinary(public.geography) TO admin;


--
-- Name: FUNCTION st_asbinary(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asbinary(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_asbinary(public.geometry) TO admin;


--
-- Name: FUNCTION st_asbinary(public.geography, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asbinary(public.geography, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asbinary(public.geography, text) TO admin;


--
-- Name: FUNCTION st_asbinary(public.geometry, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asbinary(public.geometry, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asbinary(public.geometry, text) TO admin;


--
-- Name: FUNCTION st_asencodedpolyline(geom public.geometry, nprecision integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asencodedpolyline(geom public.geometry, nprecision integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asencodedpolyline(geom public.geometry, nprecision integer) TO admin;


--
-- Name: FUNCTION st_asewkb(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asewkb(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_asewkb(public.geometry) TO admin;


--
-- Name: FUNCTION st_asewkb(public.geometry, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asewkb(public.geometry, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asewkb(public.geometry, text) TO admin;


--
-- Name: FUNCTION st_asewkt(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asewkt(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asewkt(text) TO admin;


--
-- Name: FUNCTION st_asewkt(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asewkt(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_asewkt(public.geography) TO admin;


--
-- Name: FUNCTION st_asewkt(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asewkt(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_asewkt(public.geometry) TO admin;


--
-- Name: FUNCTION st_asewkt(public.geography, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asewkt(public.geography, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asewkt(public.geography, integer) TO admin;


--
-- Name: FUNCTION st_asewkt(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asewkt(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asewkt(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_asgeojson(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgeojson(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgeojson(text) TO admin;


--
-- Name: FUNCTION st_asgeojson(geog public.geography, maxdecimaldigits integer, options integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgeojson(geog public.geography, maxdecimaldigits integer, options integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgeojson(geog public.geography, maxdecimaldigits integer, options integer) TO admin;


--
-- Name: FUNCTION st_asgeojson(geom public.geometry, maxdecimaldigits integer, options integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgeojson(geom public.geometry, maxdecimaldigits integer, options integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgeojson(geom public.geometry, maxdecimaldigits integer, options integer) TO admin;


--
-- Name: FUNCTION st_asgeojson(r record, geom_column text, maxdecimaldigits integer, pretty_bool boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgeojson(r record, geom_column text, maxdecimaldigits integer, pretty_bool boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgeojson(r record, geom_column text, maxdecimaldigits integer, pretty_bool boolean) TO admin;


--
-- Name: FUNCTION st_asgml(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgml(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgml(text) TO admin;


--
-- Name: FUNCTION st_asgml(geom public.geometry, maxdecimaldigits integer, options integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgml(geom public.geometry, maxdecimaldigits integer, options integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgml(geom public.geometry, maxdecimaldigits integer, options integer) TO admin;


--
-- Name: FUNCTION st_asgml(geog public.geography, maxdecimaldigits integer, options integer, nprefix text, id text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgml(geog public.geography, maxdecimaldigits integer, options integer, nprefix text, id text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgml(geog public.geography, maxdecimaldigits integer, options integer, nprefix text, id text) TO admin;


--
-- Name: FUNCTION st_asgml(version integer, geog public.geography, maxdecimaldigits integer, options integer, nprefix text, id text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgml(version integer, geog public.geography, maxdecimaldigits integer, options integer, nprefix text, id text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgml(version integer, geog public.geography, maxdecimaldigits integer, options integer, nprefix text, id text) TO admin;


--
-- Name: FUNCTION st_asgml(version integer, geom public.geometry, maxdecimaldigits integer, options integer, nprefix text, id text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgml(version integer, geom public.geometry, maxdecimaldigits integer, options integer, nprefix text, id text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgml(version integer, geom public.geometry, maxdecimaldigits integer, options integer, nprefix text, id text) TO admin;


--
-- Name: FUNCTION st_ashexewkb(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_ashexewkb(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_ashexewkb(public.geometry) TO admin;


--
-- Name: FUNCTION st_ashexewkb(public.geometry, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_ashexewkb(public.geometry, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_ashexewkb(public.geometry, text) TO admin;


--
-- Name: FUNCTION st_askml(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_askml(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_askml(text) TO admin;


--
-- Name: FUNCTION st_askml(geog public.geography, maxdecimaldigits integer, nprefix text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_askml(geog public.geography, maxdecimaldigits integer, nprefix text) TO readwrite;
GRANT ALL ON FUNCTION public.st_askml(geog public.geography, maxdecimaldigits integer, nprefix text) TO admin;


--
-- Name: FUNCTION st_askml(geom public.geometry, maxdecimaldigits integer, nprefix text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_askml(geom public.geometry, maxdecimaldigits integer, nprefix text) TO readwrite;
GRANT ALL ON FUNCTION public.st_askml(geom public.geometry, maxdecimaldigits integer, nprefix text) TO admin;


--
-- Name: FUNCTION st_aslatlontext(geom public.geometry, tmpl text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_aslatlontext(geom public.geometry, tmpl text) TO readwrite;
GRANT ALL ON FUNCTION public.st_aslatlontext(geom public.geometry, tmpl text) TO admin;


--
-- Name: FUNCTION st_asmarc21(geom public.geometry, format text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asmarc21(geom public.geometry, format text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asmarc21(geom public.geometry, format text) TO admin;


--
-- Name: FUNCTION st_asmvtgeom(geom public.geometry, bounds public.box2d, extent integer, buffer integer, clip_geom boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asmvtgeom(geom public.geometry, bounds public.box2d, extent integer, buffer integer, clip_geom boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_asmvtgeom(geom public.geometry, bounds public.box2d, extent integer, buffer integer, clip_geom boolean) TO admin;


--
-- Name: FUNCTION st_assvg(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_assvg(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_assvg(text) TO admin;


--
-- Name: FUNCTION st_assvg(geog public.geography, rel integer, maxdecimaldigits integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_assvg(geog public.geography, rel integer, maxdecimaldigits integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_assvg(geog public.geography, rel integer, maxdecimaldigits integer) TO admin;


--
-- Name: FUNCTION st_assvg(geom public.geometry, rel integer, maxdecimaldigits integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_assvg(geom public.geometry, rel integer, maxdecimaldigits integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_assvg(geom public.geometry, rel integer, maxdecimaldigits integer) TO admin;


--
-- Name: FUNCTION st_astext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_astext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_astext(text) TO admin;


--
-- Name: FUNCTION st_astext(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_astext(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_astext(public.geography) TO admin;


--
-- Name: FUNCTION st_astext(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_astext(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_astext(public.geometry) TO admin;


--
-- Name: FUNCTION st_astext(public.geography, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_astext(public.geography, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_astext(public.geography, integer) TO admin;


--
-- Name: FUNCTION st_astext(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_astext(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_astext(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_astwkb(geom public.geometry, prec integer, prec_z integer, prec_m integer, with_sizes boolean, with_boxes boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_astwkb(geom public.geometry, prec integer, prec_z integer, prec_m integer, with_sizes boolean, with_boxes boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_astwkb(geom public.geometry, prec integer, prec_z integer, prec_m integer, with_sizes boolean, with_boxes boolean) TO admin;


--
-- Name: FUNCTION st_astwkb(geom public.geometry[], ids bigint[], prec integer, prec_z integer, prec_m integer, with_sizes boolean, with_boxes boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_astwkb(geom public.geometry[], ids bigint[], prec integer, prec_z integer, prec_m integer, with_sizes boolean, with_boxes boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_astwkb(geom public.geometry[], ids bigint[], prec integer, prec_z integer, prec_m integer, with_sizes boolean, with_boxes boolean) TO admin;


--
-- Name: FUNCTION st_asx3d(geom public.geometry, maxdecimaldigits integer, options integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asx3d(geom public.geometry, maxdecimaldigits integer, options integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asx3d(geom public.geometry, maxdecimaldigits integer, options integer) TO admin;


--
-- Name: FUNCTION st_azimuth(geog1 public.geography, geog2 public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_azimuth(geog1 public.geography, geog2 public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_azimuth(geog1 public.geography, geog2 public.geography) TO admin;


--
-- Name: FUNCTION st_azimuth(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_azimuth(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_azimuth(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_bdmpolyfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_bdmpolyfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_bdmpolyfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_bdpolyfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_bdpolyfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_bdpolyfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_boundary(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_boundary(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_boundary(public.geometry) TO admin;


--
-- Name: FUNCTION st_boundingdiagonal(geom public.geometry, fits boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_boundingdiagonal(geom public.geometry, fits boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_boundingdiagonal(geom public.geometry, fits boolean) TO admin;


--
-- Name: FUNCTION st_box2dfromgeohash(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_box2dfromgeohash(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_box2dfromgeohash(text, integer) TO admin;


--
-- Name: FUNCTION st_buffer(text, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(text, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(text, double precision) TO admin;


--
-- Name: FUNCTION st_buffer(public.geography, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(public.geography, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(public.geography, double precision) TO admin;


--
-- Name: FUNCTION st_buffer(text, double precision, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(text, double precision, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(text, double precision, integer) TO admin;


--
-- Name: FUNCTION st_buffer(text, double precision, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(text, double precision, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(text, double precision, text) TO admin;


--
-- Name: FUNCTION st_buffer(public.geography, double precision, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(public.geography, double precision, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(public.geography, double precision, integer) TO admin;


--
-- Name: FUNCTION st_buffer(public.geography, double precision, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(public.geography, double precision, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(public.geography, double precision, text) TO admin;


--
-- Name: FUNCTION st_buffer(geom public.geometry, radius double precision, quadsegs integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(geom public.geometry, radius double precision, quadsegs integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(geom public.geometry, radius double precision, quadsegs integer) TO admin;


--
-- Name: FUNCTION st_buffer(geom public.geometry, radius double precision, options text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buffer(geom public.geometry, radius double precision, options text) TO readwrite;
GRANT ALL ON FUNCTION public.st_buffer(geom public.geometry, radius double precision, options text) TO admin;


--
-- Name: FUNCTION st_buildarea(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_buildarea(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_buildarea(public.geometry) TO admin;


--
-- Name: FUNCTION st_centroid(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_centroid(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_centroid(text) TO admin;


--
-- Name: FUNCTION st_centroid(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_centroid(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_centroid(public.geometry) TO admin;


--
-- Name: FUNCTION st_centroid(public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_centroid(public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_centroid(public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_chaikinsmoothing(public.geometry, integer, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_chaikinsmoothing(public.geometry, integer, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_chaikinsmoothing(public.geometry, integer, boolean) TO admin;


--
-- Name: FUNCTION st_cleangeometry(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_cleangeometry(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_cleangeometry(public.geometry) TO admin;


--
-- Name: FUNCTION st_clipbybox2d(geom public.geometry, box public.box2d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clipbybox2d(geom public.geometry, box public.box2d) TO readwrite;
GRANT ALL ON FUNCTION public.st_clipbybox2d(geom public.geometry, box public.box2d) TO admin;


--
-- Name: FUNCTION st_closestpoint(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_closestpoint(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_closestpoint(text, text) TO admin;


--
-- Name: FUNCTION st_closestpoint(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_closestpoint(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_closestpoint(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_closestpoint(public.geography, public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_closestpoint(public.geography, public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_closestpoint(public.geography, public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_closestpointofapproach(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_closestpointofapproach(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_closestpointofapproach(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION st_clusterdbscan(public.geometry, eps double precision, minpoints integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterdbscan(public.geometry, eps double precision, minpoints integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterdbscan(public.geometry, eps double precision, minpoints integer) TO admin;


--
-- Name: FUNCTION st_clusterintersecting(public.geometry[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterintersecting(public.geometry[]) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterintersecting(public.geometry[]) TO admin;


--
-- Name: FUNCTION st_clusterintersectingwin(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterintersectingwin(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterintersectingwin(public.geometry) TO admin;


--
-- Name: FUNCTION st_clusterkmeans(geom public.geometry, k integer, max_radius double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterkmeans(geom public.geometry, k integer, max_radius double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterkmeans(geom public.geometry, k integer, max_radius double precision) TO admin;


--
-- Name: FUNCTION st_clusterwithin(public.geometry[], double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterwithin(public.geometry[], double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterwithin(public.geometry[], double precision) TO admin;


--
-- Name: FUNCTION st_clusterwithinwin(public.geometry, distance double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterwithinwin(public.geometry, distance double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterwithinwin(public.geometry, distance double precision) TO admin;


--
-- Name: FUNCTION st_collect(public.geometry[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_collect(public.geometry[]) TO readwrite;
GRANT ALL ON FUNCTION public.st_collect(public.geometry[]) TO admin;


--
-- Name: FUNCTION st_collect(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_collect(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_collect(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_collectionextract(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_collectionextract(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_collectionextract(public.geometry) TO admin;


--
-- Name: FUNCTION st_collectionextract(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_collectionextract(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_collectionextract(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_collectionhomogenize(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_collectionhomogenize(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_collectionhomogenize(public.geometry) TO admin;


--
-- Name: FUNCTION st_combinebbox(public.box2d, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_combinebbox(public.box2d, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_combinebbox(public.box2d, public.geometry) TO admin;


--
-- Name: FUNCTION st_combinebbox(public.box3d, public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_combinebbox(public.box3d, public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.st_combinebbox(public.box3d, public.box3d) TO admin;


--
-- Name: FUNCTION st_combinebbox(public.box3d, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_combinebbox(public.box3d, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_combinebbox(public.box3d, public.geometry) TO admin;


--
-- Name: FUNCTION st_concavehull(param_geom public.geometry, param_pctconvex double precision, param_allow_holes boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_concavehull(param_geom public.geometry, param_pctconvex double precision, param_allow_holes boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_concavehull(param_geom public.geometry, param_pctconvex double precision, param_allow_holes boolean) TO admin;


--
-- Name: FUNCTION st_contains(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_contains(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_contains(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_containsproperly(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_containsproperly(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_containsproperly(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_convexhull(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_convexhull(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_convexhull(public.geometry) TO admin;


--
-- Name: FUNCTION st_coorddim(geometry public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coorddim(geometry public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_coorddim(geometry public.geometry) TO admin;


--
-- Name: FUNCTION st_coverageinvalidedges(geom public.geometry, tolerance double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coverageinvalidedges(geom public.geometry, tolerance double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_coverageinvalidedges(geom public.geometry, tolerance double precision) TO admin;


--
-- Name: FUNCTION st_coveragesimplify(geom public.geometry, tolerance double precision, simplifyboundary boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coveragesimplify(geom public.geometry, tolerance double precision, simplifyboundary boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_coveragesimplify(geom public.geometry, tolerance double precision, simplifyboundary boolean) TO admin;


--
-- Name: FUNCTION st_coverageunion(public.geometry[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coverageunion(public.geometry[]) TO readwrite;
GRANT ALL ON FUNCTION public.st_coverageunion(public.geometry[]) TO admin;


--
-- Name: FUNCTION st_coveredby(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coveredby(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_coveredby(text, text) TO admin;


--
-- Name: FUNCTION st_coveredby(geog1 public.geography, geog2 public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coveredby(geog1 public.geography, geog2 public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_coveredby(geog1 public.geography, geog2 public.geography) TO admin;


--
-- Name: FUNCTION st_coveredby(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coveredby(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_coveredby(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_covers(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_covers(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_covers(text, text) TO admin;


--
-- Name: FUNCTION st_covers(geog1 public.geography, geog2 public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_covers(geog1 public.geography, geog2 public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_covers(geog1 public.geography, geog2 public.geography) TO admin;


--
-- Name: FUNCTION st_covers(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_covers(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_covers(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_cpawithin(public.geometry, public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_cpawithin(public.geometry, public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_cpawithin(public.geometry, public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_crosses(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_crosses(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_crosses(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_curvetoline(geom public.geometry, tol double precision, toltype integer, flags integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_curvetoline(geom public.geometry, tol double precision, toltype integer, flags integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_curvetoline(geom public.geometry, tol double precision, toltype integer, flags integer) TO admin;


--
-- Name: FUNCTION st_delaunaytriangles(g1 public.geometry, tolerance double precision, flags integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_delaunaytriangles(g1 public.geometry, tolerance double precision, flags integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_delaunaytriangles(g1 public.geometry, tolerance double precision, flags integer) TO admin;


--
-- Name: FUNCTION st_dfullywithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_dfullywithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_difference(geom1 public.geometry, geom2 public.geometry, gridsize double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_difference(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_difference(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO admin;


--
-- Name: FUNCTION st_dimension(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dimension(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_dimension(public.geometry) TO admin;


--
-- Name: FUNCTION st_disjoint(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_disjoint(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_disjoint(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_distance(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distance(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_distance(text, text) TO admin;


--
-- Name: FUNCTION st_distance(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distance(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_distance(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_distance(geog1 public.geography, geog2 public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distance(geog1 public.geography, geog2 public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_distance(geog1 public.geography, geog2 public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_distancecpa(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distancecpa(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_distancecpa(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION st_distancesphere(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distancesphere(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_distancesphere(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_distancesphere(geom1 public.geometry, geom2 public.geometry, radius double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distancesphere(geom1 public.geometry, geom2 public.geometry, radius double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_distancesphere(geom1 public.geometry, geom2 public.geometry, radius double precision) TO admin;


--
-- Name: FUNCTION st_distancespheroid(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distancespheroid(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_distancespheroid(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_distancespheroid(geom1 public.geometry, geom2 public.geometry, public.spheroid); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_distancespheroid(geom1 public.geometry, geom2 public.geometry, public.spheroid) TO readwrite;
GRANT ALL ON FUNCTION public.st_distancespheroid(geom1 public.geometry, geom2 public.geometry, public.spheroid) TO admin;


--
-- Name: FUNCTION st_dump(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dump(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_dump(public.geometry) TO admin;


--
-- Name: FUNCTION st_dumppoints(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dumppoints(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_dumppoints(public.geometry) TO admin;


--
-- Name: FUNCTION st_dumprings(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dumprings(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_dumprings(public.geometry) TO admin;


--
-- Name: FUNCTION st_dumpsegments(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dumpsegments(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_dumpsegments(public.geometry) TO admin;


--
-- Name: FUNCTION st_dwithin(text, text, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dwithin(text, text, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_dwithin(text, text, double precision) TO admin;


--
-- Name: FUNCTION st_dwithin(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_dwithin(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_dwithin(geog1 public.geography, geog2 public.geography, tolerance double precision, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_dwithin(geog1 public.geography, geog2 public.geography, tolerance double precision, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_dwithin(geog1 public.geography, geog2 public.geography, tolerance double precision, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_endpoint(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_endpoint(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_endpoint(public.geometry) TO admin;


--
-- Name: FUNCTION st_envelope(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_envelope(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_envelope(public.geometry) TO admin;


--
-- Name: FUNCTION st_equals(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_equals(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_equals(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_estimatedextent(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_estimatedextent(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_estimatedextent(text, text) TO admin;


--
-- Name: FUNCTION st_estimatedextent(text, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_estimatedextent(text, text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_estimatedextent(text, text, text) TO admin;


--
-- Name: FUNCTION st_estimatedextent(text, text, text, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_estimatedextent(text, text, text, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_estimatedextent(text, text, text, boolean) TO admin;


--
-- Name: FUNCTION st_expand(public.box2d, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_expand(public.box2d, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_expand(public.box2d, double precision) TO admin;


--
-- Name: FUNCTION st_expand(public.box3d, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_expand(public.box3d, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_expand(public.box3d, double precision) TO admin;


--
-- Name: FUNCTION st_expand(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_expand(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_expand(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_expand(box public.box2d, dx double precision, dy double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_expand(box public.box2d, dx double precision, dy double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_expand(box public.box2d, dx double precision, dy double precision) TO admin;


--
-- Name: FUNCTION st_expand(box public.box3d, dx double precision, dy double precision, dz double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_expand(box public.box3d, dx double precision, dy double precision, dz double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_expand(box public.box3d, dx double precision, dy double precision, dz double precision) TO admin;


--
-- Name: FUNCTION st_expand(geom public.geometry, dx double precision, dy double precision, dz double precision, dm double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_expand(geom public.geometry, dx double precision, dy double precision, dz double precision, dm double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_expand(geom public.geometry, dx double precision, dy double precision, dz double precision, dm double precision) TO admin;


--
-- Name: FUNCTION st_exteriorring(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_exteriorring(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_exteriorring(public.geometry) TO admin;


--
-- Name: FUNCTION st_filterbym(public.geometry, double precision, double precision, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_filterbym(public.geometry, double precision, double precision, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_filterbym(public.geometry, double precision, double precision, boolean) TO admin;


--
-- Name: FUNCTION st_findextent(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_findextent(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_findextent(text, text) TO admin;


--
-- Name: FUNCTION st_findextent(text, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_findextent(text, text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_findextent(text, text, text) TO admin;


--
-- Name: FUNCTION st_flipcoordinates(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_flipcoordinates(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_flipcoordinates(public.geometry) TO admin;


--
-- Name: FUNCTION st_force2d(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_force2d(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_force2d(public.geometry) TO admin;


--
-- Name: FUNCTION st_force3d(geom public.geometry, zvalue double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_force3d(geom public.geometry, zvalue double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_force3d(geom public.geometry, zvalue double precision) TO admin;


--
-- Name: FUNCTION st_force3dm(geom public.geometry, mvalue double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_force3dm(geom public.geometry, mvalue double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_force3dm(geom public.geometry, mvalue double precision) TO admin;


--
-- Name: FUNCTION st_force3dz(geom public.geometry, zvalue double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_force3dz(geom public.geometry, zvalue double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_force3dz(geom public.geometry, zvalue double precision) TO admin;


--
-- Name: FUNCTION st_force4d(geom public.geometry, zvalue double precision, mvalue double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_force4d(geom public.geometry, zvalue double precision, mvalue double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_force4d(geom public.geometry, zvalue double precision, mvalue double precision) TO admin;


--
-- Name: FUNCTION st_forcecollection(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_forcecollection(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_forcecollection(public.geometry) TO admin;


--
-- Name: FUNCTION st_forcecurve(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_forcecurve(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_forcecurve(public.geometry) TO admin;


--
-- Name: FUNCTION st_forcepolygonccw(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_forcepolygonccw(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_forcepolygonccw(public.geometry) TO admin;


--
-- Name: FUNCTION st_forcepolygoncw(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_forcepolygoncw(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_forcepolygoncw(public.geometry) TO admin;


--
-- Name: FUNCTION st_forcerhr(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_forcerhr(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_forcerhr(public.geometry) TO admin;


--
-- Name: FUNCTION st_forcesfs(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_forcesfs(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_forcesfs(public.geometry) TO admin;


--
-- Name: FUNCTION st_forcesfs(public.geometry, version text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_forcesfs(public.geometry, version text) TO readwrite;
GRANT ALL ON FUNCTION public.st_forcesfs(public.geometry, version text) TO admin;


--
-- Name: FUNCTION st_frechetdistance(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_frechetdistance(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_frechetdistance(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_fromflatgeobuf(anyelement, bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_fromflatgeobuf(anyelement, bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_fromflatgeobuf(anyelement, bytea) TO admin;


--
-- Name: FUNCTION st_fromflatgeobuftotable(text, text, bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_fromflatgeobuftotable(text, text, bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_fromflatgeobuftotable(text, text, bytea) TO admin;


--
-- Name: FUNCTION st_generatepoints(area public.geometry, npoints integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_generatepoints(area public.geometry, npoints integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_generatepoints(area public.geometry, npoints integer) TO admin;


--
-- Name: FUNCTION st_generatepoints(area public.geometry, npoints integer, seed integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_generatepoints(area public.geometry, npoints integer, seed integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_generatepoints(area public.geometry, npoints integer, seed integer) TO admin;


--
-- Name: FUNCTION st_geogfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geogfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geogfromtext(text) TO admin;


--
-- Name: FUNCTION st_geogfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geogfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_geogfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_geographyfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geographyfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geographyfromtext(text) TO admin;


--
-- Name: FUNCTION st_geohash(geog public.geography, maxchars integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geohash(geog public.geography, maxchars integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geohash(geog public.geography, maxchars integer) TO admin;


--
-- Name: FUNCTION st_geohash(geom public.geometry, maxchars integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geohash(geom public.geometry, maxchars integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geohash(geom public.geometry, maxchars integer) TO admin;


--
-- Name: FUNCTION st_geomcollfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomcollfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomcollfromtext(text) TO admin;


--
-- Name: FUNCTION st_geomcollfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomcollfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomcollfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_geomcollfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomcollfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomcollfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_geomcollfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomcollfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomcollfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_geometricmedian(g public.geometry, tolerance double precision, max_iter integer, fail_if_not_converged boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geometricmedian(g public.geometry, tolerance double precision, max_iter integer, fail_if_not_converged boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_geometricmedian(g public.geometry, tolerance double precision, max_iter integer, fail_if_not_converged boolean) TO admin;


--
-- Name: FUNCTION st_geometryfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geometryfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geometryfromtext(text) TO admin;


--
-- Name: FUNCTION st_geometryfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geometryfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geometryfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_geometryn(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geometryn(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geometryn(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_geometrytype(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geometrytype(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_geometrytype(public.geometry) TO admin;


--
-- Name: FUNCTION st_geomfromewkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromewkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromewkb(bytea) TO admin;


--
-- Name: FUNCTION st_geomfromewkt(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromewkt(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromewkt(text) TO admin;


--
-- Name: FUNCTION st_geomfromgeohash(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromgeohash(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromgeohash(text, integer) TO admin;


--
-- Name: FUNCTION st_geomfromgeojson(json); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromgeojson(json) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromgeojson(json) TO admin;


--
-- Name: FUNCTION st_geomfromgeojson(jsonb); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromgeojson(jsonb) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromgeojson(jsonb) TO admin;


--
-- Name: FUNCTION st_geomfromgeojson(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromgeojson(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromgeojson(text) TO admin;


--
-- Name: FUNCTION st_geomfromgml(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromgml(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromgml(text) TO admin;


--
-- Name: FUNCTION st_geomfromgml(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromgml(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromgml(text, integer) TO admin;


--
-- Name: FUNCTION st_geomfromkml(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromkml(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromkml(text) TO admin;


--
-- Name: FUNCTION st_geomfrommarc21(marc21xml text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfrommarc21(marc21xml text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfrommarc21(marc21xml text) TO admin;


--
-- Name: FUNCTION st_geomfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromtext(text) TO admin;


--
-- Name: FUNCTION st_geomfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_geomfromtwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromtwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromtwkb(bytea) TO admin;


--
-- Name: FUNCTION st_geomfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_geomfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_geomfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_geomfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_gmltosql(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_gmltosql(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_gmltosql(text) TO admin;


--
-- Name: FUNCTION st_gmltosql(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_gmltosql(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_gmltosql(text, integer) TO admin;


--
-- Name: FUNCTION st_hasarc(geometry public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_hasarc(geometry public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_hasarc(geometry public.geometry) TO admin;


--
-- Name: FUNCTION st_hausdorffdistance(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_hausdorffdistance(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_hausdorffdistance(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_hausdorffdistance(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_hausdorffdistance(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_hausdorffdistance(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_hexagon(size double precision, cell_i integer, cell_j integer, origin public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_hexagon(size double precision, cell_i integer, cell_j integer, origin public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_hexagon(size double precision, cell_i integer, cell_j integer, origin public.geometry) TO admin;


--
-- Name: FUNCTION st_hexagongrid(size double precision, bounds public.geometry, OUT geom public.geometry, OUT i integer, OUT j integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_hexagongrid(size double precision, bounds public.geometry, OUT geom public.geometry, OUT i integer, OUT j integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_hexagongrid(size double precision, bounds public.geometry, OUT geom public.geometry, OUT i integer, OUT j integer) TO admin;


--
-- Name: FUNCTION st_interiorringn(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_interiorringn(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_interiorringn(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_interpolatepoint(line public.geometry, point public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_interpolatepoint(line public.geometry, point public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_interpolatepoint(line public.geometry, point public.geometry) TO admin;


--
-- Name: FUNCTION st_intersection(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_intersection(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_intersection(text, text) TO admin;


--
-- Name: FUNCTION st_intersection(public.geography, public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_intersection(public.geography, public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_intersection(public.geography, public.geography) TO admin;


--
-- Name: FUNCTION st_intersection(geom1 public.geometry, geom2 public.geometry, gridsize double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_intersection(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_intersection(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO admin;


--
-- Name: FUNCTION st_intersects(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_intersects(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_intersects(text, text) TO admin;


--
-- Name: FUNCTION st_intersects(geog1 public.geography, geog2 public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_intersects(geog1 public.geography, geog2 public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_intersects(geog1 public.geography, geog2 public.geography) TO admin;


--
-- Name: FUNCTION st_inversetransformpipeline(geom public.geometry, pipeline text, to_srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_inversetransformpipeline(geom public.geometry, pipeline text, to_srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_inversetransformpipeline(geom public.geometry, pipeline text, to_srid integer) TO admin;


--
-- Name: FUNCTION st_isclosed(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isclosed(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_isclosed(public.geometry) TO admin;


--
-- Name: FUNCTION st_iscollection(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_iscollection(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_iscollection(public.geometry) TO admin;


--
-- Name: FUNCTION st_isempty(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isempty(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_isempty(public.geometry) TO admin;


--
-- Name: FUNCTION st_ispolygonccw(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_ispolygonccw(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_ispolygonccw(public.geometry) TO admin;


--
-- Name: FUNCTION st_ispolygoncw(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_ispolygoncw(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_ispolygoncw(public.geometry) TO admin;


--
-- Name: FUNCTION st_isring(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isring(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_isring(public.geometry) TO admin;


--
-- Name: FUNCTION st_issimple(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_issimple(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_issimple(public.geometry) TO admin;


--
-- Name: FUNCTION st_isvalid(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isvalid(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_isvalid(public.geometry) TO admin;


--
-- Name: FUNCTION st_isvalid(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isvalid(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_isvalid(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_isvaliddetail(geom public.geometry, flags integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isvaliddetail(geom public.geometry, flags integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_isvaliddetail(geom public.geometry, flags integer) TO admin;


--
-- Name: FUNCTION st_isvalidreason(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isvalidreason(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_isvalidreason(public.geometry) TO admin;


--
-- Name: FUNCTION st_isvalidreason(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isvalidreason(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_isvalidreason(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_isvalidtrajectory(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_isvalidtrajectory(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_isvalidtrajectory(public.geometry) TO admin;


--
-- Name: FUNCTION st_largestemptycircle(geom public.geometry, tolerance double precision, boundary public.geometry, OUT center public.geometry, OUT nearest public.geometry, OUT radius double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_largestemptycircle(geom public.geometry, tolerance double precision, boundary public.geometry, OUT center public.geometry, OUT nearest public.geometry, OUT radius double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_largestemptycircle(geom public.geometry, tolerance double precision, boundary public.geometry, OUT center public.geometry, OUT nearest public.geometry, OUT radius double precision) TO admin;


--
-- Name: FUNCTION st_length(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_length(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_length(text) TO admin;


--
-- Name: FUNCTION st_length(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_length(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_length(public.geometry) TO admin;


--
-- Name: FUNCTION st_length(geog public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_length(geog public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_length(geog public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_length2d(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_length2d(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_length2d(public.geometry) TO admin;


--
-- Name: FUNCTION st_length2dspheroid(public.geometry, public.spheroid); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_length2dspheroid(public.geometry, public.spheroid) TO readwrite;
GRANT ALL ON FUNCTION public.st_length2dspheroid(public.geometry, public.spheroid) TO admin;


--
-- Name: FUNCTION st_lengthspheroid(public.geometry, public.spheroid); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lengthspheroid(public.geometry, public.spheroid) TO readwrite;
GRANT ALL ON FUNCTION public.st_lengthspheroid(public.geometry, public.spheroid) TO admin;


--
-- Name: FUNCTION st_letters(letters text, font json); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_letters(letters text, font json) TO readwrite;
GRANT ALL ON FUNCTION public.st_letters(letters text, font json) TO admin;


--
-- Name: FUNCTION st_linecrossingdirection(line1 public.geometry, line2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linecrossingdirection(line1 public.geometry, line2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_linecrossingdirection(line1 public.geometry, line2 public.geometry) TO admin;


--
-- Name: FUNCTION st_lineextend(geom public.geometry, distance_forward double precision, distance_backward double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lineextend(geom public.geometry, distance_forward double precision, distance_backward double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_lineextend(geom public.geometry, distance_forward double precision, distance_backward double precision) TO admin;


--
-- Name: FUNCTION st_linefromencodedpolyline(txtin text, nprecision integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linefromencodedpolyline(txtin text, nprecision integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_linefromencodedpolyline(txtin text, nprecision integer) TO admin;


--
-- Name: FUNCTION st_linefrommultipoint(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linefrommultipoint(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_linefrommultipoint(public.geometry) TO admin;


--
-- Name: FUNCTION st_linefromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linefromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_linefromtext(text) TO admin;


--
-- Name: FUNCTION st_linefromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linefromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_linefromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_linefromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linefromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_linefromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_linefromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linefromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_linefromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_lineinterpolatepoint(text, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lineinterpolatepoint(text, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_lineinterpolatepoint(text, double precision) TO admin;


--
-- Name: FUNCTION st_lineinterpolatepoint(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lineinterpolatepoint(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_lineinterpolatepoint(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_lineinterpolatepoint(public.geography, double precision, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lineinterpolatepoint(public.geography, double precision, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_lineinterpolatepoint(public.geography, double precision, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_lineinterpolatepoints(text, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lineinterpolatepoints(text, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_lineinterpolatepoints(text, double precision) TO admin;


--
-- Name: FUNCTION st_lineinterpolatepoints(public.geometry, double precision, repeat boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lineinterpolatepoints(public.geometry, double precision, repeat boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_lineinterpolatepoints(public.geometry, double precision, repeat boolean) TO admin;


--
-- Name: FUNCTION st_lineinterpolatepoints(public.geography, double precision, use_spheroid boolean, repeat boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_lineinterpolatepoints(public.geography, double precision, use_spheroid boolean, repeat boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_lineinterpolatepoints(public.geography, double precision, use_spheroid boolean, repeat boolean) TO admin;


--
-- Name: FUNCTION st_linelocatepoint(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linelocatepoint(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_linelocatepoint(text, text) TO admin;


--
-- Name: FUNCTION st_linelocatepoint(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linelocatepoint(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_linelocatepoint(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_linelocatepoint(public.geography, public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linelocatepoint(public.geography, public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_linelocatepoint(public.geography, public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_linemerge(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linemerge(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_linemerge(public.geometry) TO admin;


--
-- Name: FUNCTION st_linemerge(public.geometry, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linemerge(public.geometry, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_linemerge(public.geometry, boolean) TO admin;


--
-- Name: FUNCTION st_linestringfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linestringfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_linestringfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_linestringfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linestringfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_linestringfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_linesubstring(text, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linesubstring(text, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_linesubstring(text, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_linesubstring(public.geography, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linesubstring(public.geography, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_linesubstring(public.geography, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_linesubstring(public.geometry, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linesubstring(public.geometry, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_linesubstring(public.geometry, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_linetocurve(geometry public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_linetocurve(geometry public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_linetocurve(geometry public.geometry) TO admin;


--
-- Name: FUNCTION st_locatealong(geometry public.geometry, measure double precision, leftrightoffset double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_locatealong(geometry public.geometry, measure double precision, leftrightoffset double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_locatealong(geometry public.geometry, measure double precision, leftrightoffset double precision) TO admin;


--
-- Name: FUNCTION st_locatebetween(geometry public.geometry, frommeasure double precision, tomeasure double precision, leftrightoffset double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_locatebetween(geometry public.geometry, frommeasure double precision, tomeasure double precision, leftrightoffset double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_locatebetween(geometry public.geometry, frommeasure double precision, tomeasure double precision, leftrightoffset double precision) TO admin;


--
-- Name: FUNCTION st_locatebetweenelevations(geometry public.geometry, fromelevation double precision, toelevation double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_locatebetweenelevations(geometry public.geometry, fromelevation double precision, toelevation double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_locatebetweenelevations(geometry public.geometry, fromelevation double precision, toelevation double precision) TO admin;


--
-- Name: FUNCTION st_longestline(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_longestline(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_longestline(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_m(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_m(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_m(public.geometry) TO admin;


--
-- Name: FUNCTION st_makebox2d(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makebox2d(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_makebox2d(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_makeenvelope(double precision, double precision, double precision, double precision, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makeenvelope(double precision, double precision, double precision, double precision, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_makeenvelope(double precision, double precision, double precision, double precision, integer) TO admin;


--
-- Name: FUNCTION st_makeline(public.geometry[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makeline(public.geometry[]) TO readwrite;
GRANT ALL ON FUNCTION public.st_makeline(public.geometry[]) TO admin;


--
-- Name: FUNCTION st_makeline(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makeline(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_makeline(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_makepoint(double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makepoint(double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_makepoint(double precision, double precision) TO admin;


--
-- Name: FUNCTION st_makepoint(double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makepoint(double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_makepoint(double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_makepoint(double precision, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makepoint(double precision, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_makepoint(double precision, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_makepointm(double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makepointm(double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_makepointm(double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_makepolygon(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makepolygon(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_makepolygon(public.geometry) TO admin;


--
-- Name: FUNCTION st_makepolygon(public.geometry, public.geometry[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makepolygon(public.geometry, public.geometry[]) TO readwrite;
GRANT ALL ON FUNCTION public.st_makepolygon(public.geometry, public.geometry[]) TO admin;


--
-- Name: FUNCTION st_makevalid(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makevalid(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_makevalid(public.geometry) TO admin;


--
-- Name: FUNCTION st_makevalid(geom public.geometry, params text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makevalid(geom public.geometry, params text) TO readwrite;
GRANT ALL ON FUNCTION public.st_makevalid(geom public.geometry, params text) TO admin;


--
-- Name: FUNCTION st_maxdistance(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_maxdistance(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_maxdistance(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_maximuminscribedcircle(public.geometry, OUT center public.geometry, OUT nearest public.geometry, OUT radius double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_maximuminscribedcircle(public.geometry, OUT center public.geometry, OUT nearest public.geometry, OUT radius double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_maximuminscribedcircle(public.geometry, OUT center public.geometry, OUT nearest public.geometry, OUT radius double precision) TO admin;


--
-- Name: FUNCTION st_memsize(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_memsize(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_memsize(public.geometry) TO admin;


--
-- Name: FUNCTION st_minimumboundingcircle(inputgeom public.geometry, segs_per_quarter integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_minimumboundingcircle(inputgeom public.geometry, segs_per_quarter integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_minimumboundingcircle(inputgeom public.geometry, segs_per_quarter integer) TO admin;


--
-- Name: FUNCTION st_minimumboundingradius(public.geometry, OUT center public.geometry, OUT radius double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_minimumboundingradius(public.geometry, OUT center public.geometry, OUT radius double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_minimumboundingradius(public.geometry, OUT center public.geometry, OUT radius double precision) TO admin;


--
-- Name: FUNCTION st_minimumclearance(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_minimumclearance(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_minimumclearance(public.geometry) TO admin;


--
-- Name: FUNCTION st_minimumclearanceline(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_minimumclearanceline(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_minimumclearanceline(public.geometry) TO admin;


--
-- Name: FUNCTION st_mlinefromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mlinefromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_mlinefromtext(text) TO admin;


--
-- Name: FUNCTION st_mlinefromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mlinefromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_mlinefromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_mlinefromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mlinefromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_mlinefromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_mlinefromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mlinefromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_mlinefromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_mpointfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpointfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpointfromtext(text) TO admin;


--
-- Name: FUNCTION st_mpointfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpointfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpointfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_mpointfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpointfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpointfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_mpointfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpointfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpointfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_mpolyfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpolyfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpolyfromtext(text) TO admin;


--
-- Name: FUNCTION st_mpolyfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpolyfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpolyfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_mpolyfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpolyfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpolyfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_mpolyfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_mpolyfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_mpolyfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_multi(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multi(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_multi(public.geometry) TO admin;


--
-- Name: FUNCTION st_multilinefromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multilinefromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_multilinefromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_multilinestringfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multilinestringfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_multilinestringfromtext(text) TO admin;


--
-- Name: FUNCTION st_multilinestringfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multilinestringfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_multilinestringfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_multipointfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multipointfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_multipointfromtext(text) TO admin;


--
-- Name: FUNCTION st_multipointfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multipointfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_multipointfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_multipointfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multipointfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_multipointfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_multipolyfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multipolyfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_multipolyfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_multipolyfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multipolyfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_multipolyfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_multipolygonfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multipolygonfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_multipolygonfromtext(text) TO admin;


--
-- Name: FUNCTION st_multipolygonfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_multipolygonfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_multipolygonfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_ndims(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_ndims(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_ndims(public.geometry) TO admin;


--
-- Name: FUNCTION st_node(g public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_node(g public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_node(g public.geometry) TO admin;


--
-- Name: FUNCTION st_normalize(geom public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_normalize(geom public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_normalize(geom public.geometry) TO admin;


--
-- Name: FUNCTION st_npoints(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_npoints(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_npoints(public.geometry) TO admin;


--
-- Name: FUNCTION st_nrings(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_nrings(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_nrings(public.geometry) TO admin;


--
-- Name: FUNCTION st_numgeometries(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_numgeometries(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_numgeometries(public.geometry) TO admin;


--
-- Name: FUNCTION st_numinteriorring(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_numinteriorring(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_numinteriorring(public.geometry) TO admin;


--
-- Name: FUNCTION st_numinteriorrings(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_numinteriorrings(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_numinteriorrings(public.geometry) TO admin;


--
-- Name: FUNCTION st_numpatches(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_numpatches(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_numpatches(public.geometry) TO admin;


--
-- Name: FUNCTION st_numpoints(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_numpoints(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_numpoints(public.geometry) TO admin;


--
-- Name: FUNCTION st_offsetcurve(line public.geometry, distance double precision, params text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_offsetcurve(line public.geometry, distance double precision, params text) TO readwrite;
GRANT ALL ON FUNCTION public.st_offsetcurve(line public.geometry, distance double precision, params text) TO admin;


--
-- Name: FUNCTION st_orderingequals(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_orderingequals(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_orderingequals(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_orientedenvelope(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_orientedenvelope(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_orientedenvelope(public.geometry) TO admin;


--
-- Name: FUNCTION st_overlaps(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_overlaps(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_overlaps(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_patchn(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_patchn(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_patchn(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_perimeter(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_perimeter(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_perimeter(public.geometry) TO admin;


--
-- Name: FUNCTION st_perimeter(geog public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_perimeter(geog public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_perimeter(geog public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_perimeter2d(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_perimeter2d(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_perimeter2d(public.geometry) TO admin;


--
-- Name: FUNCTION st_point(double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_point(double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_point(double precision, double precision) TO admin;


--
-- Name: FUNCTION st_point(double precision, double precision, srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_point(double precision, double precision, srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_point(double precision, double precision, srid integer) TO admin;


--
-- Name: FUNCTION st_pointfromgeohash(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointfromgeohash(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointfromgeohash(text, integer) TO admin;


--
-- Name: FUNCTION st_pointfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointfromtext(text) TO admin;


--
-- Name: FUNCTION st_pointfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_pointfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_pointfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_pointinsidecircle(public.geometry, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointinsidecircle(public.geometry, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointinsidecircle(public.geometry, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_pointm(xcoordinate double precision, ycoordinate double precision, mcoordinate double precision, srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointm(xcoordinate double precision, ycoordinate double precision, mcoordinate double precision, srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointm(xcoordinate double precision, ycoordinate double precision, mcoordinate double precision, srid integer) TO admin;


--
-- Name: FUNCTION st_pointn(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointn(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointn(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_pointonsurface(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointonsurface(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointonsurface(public.geometry) TO admin;


--
-- Name: FUNCTION st_points(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_points(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_points(public.geometry) TO admin;


--
-- Name: FUNCTION st_pointz(xcoordinate double precision, ycoordinate double precision, zcoordinate double precision, srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointz(xcoordinate double precision, ycoordinate double precision, zcoordinate double precision, srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointz(xcoordinate double precision, ycoordinate double precision, zcoordinate double precision, srid integer) TO admin;


--
-- Name: FUNCTION st_pointzm(xcoordinate double precision, ycoordinate double precision, zcoordinate double precision, mcoordinate double precision, srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_pointzm(xcoordinate double precision, ycoordinate double precision, zcoordinate double precision, mcoordinate double precision, srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_pointzm(xcoordinate double precision, ycoordinate double precision, zcoordinate double precision, mcoordinate double precision, srid integer) TO admin;


--
-- Name: FUNCTION st_polyfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polyfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_polyfromtext(text) TO admin;


--
-- Name: FUNCTION st_polyfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polyfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_polyfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_polyfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polyfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_polyfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_polyfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polyfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_polyfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_polygon(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polygon(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_polygon(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_polygonfromtext(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polygonfromtext(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_polygonfromtext(text) TO admin;


--
-- Name: FUNCTION st_polygonfromtext(text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polygonfromtext(text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_polygonfromtext(text, integer) TO admin;


--
-- Name: FUNCTION st_polygonfromwkb(bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polygonfromwkb(bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_polygonfromwkb(bytea) TO admin;


--
-- Name: FUNCTION st_polygonfromwkb(bytea, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polygonfromwkb(bytea, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_polygonfromwkb(bytea, integer) TO admin;


--
-- Name: FUNCTION st_polygonize(public.geometry[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polygonize(public.geometry[]) TO readwrite;
GRANT ALL ON FUNCTION public.st_polygonize(public.geometry[]) TO admin;


--
-- Name: FUNCTION st_project(geog public.geography, distance double precision, azimuth double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_project(geog public.geography, distance double precision, azimuth double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_project(geog public.geography, distance double precision, azimuth double precision) TO admin;


--
-- Name: FUNCTION st_project(geog_from public.geography, geog_to public.geography, distance double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_project(geog_from public.geography, geog_to public.geography, distance double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_project(geog_from public.geography, geog_to public.geography, distance double precision) TO admin;


--
-- Name: FUNCTION st_project(geom1 public.geometry, distance double precision, azimuth double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_project(geom1 public.geometry, distance double precision, azimuth double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_project(geom1 public.geometry, distance double precision, azimuth double precision) TO admin;


--
-- Name: FUNCTION st_project(geom1 public.geometry, geom2 public.geometry, distance double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_project(geom1 public.geometry, geom2 public.geometry, distance double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_project(geom1 public.geometry, geom2 public.geometry, distance double precision) TO admin;


--
-- Name: FUNCTION st_quantizecoordinates(g public.geometry, prec_x integer, prec_y integer, prec_z integer, prec_m integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_quantizecoordinates(g public.geometry, prec_x integer, prec_y integer, prec_z integer, prec_m integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_quantizecoordinates(g public.geometry, prec_x integer, prec_y integer, prec_z integer, prec_m integer) TO admin;


--
-- Name: FUNCTION st_reduceprecision(geom public.geometry, gridsize double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_reduceprecision(geom public.geometry, gridsize double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_reduceprecision(geom public.geometry, gridsize double precision) TO admin;


--
-- Name: FUNCTION st_relate(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_relate(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_relate(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_relate(geom1 public.geometry, geom2 public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_relate(geom1 public.geometry, geom2 public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_relate(geom1 public.geometry, geom2 public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_relate(geom1 public.geometry, geom2 public.geometry, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_relate(geom1 public.geometry, geom2 public.geometry, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_relate(geom1 public.geometry, geom2 public.geometry, text) TO admin;


--
-- Name: FUNCTION st_relatematch(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_relatematch(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_relatematch(text, text) TO admin;


--
-- Name: FUNCTION st_removepoint(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_removepoint(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_removepoint(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_removerepeatedpoints(geom public.geometry, tolerance double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_removerepeatedpoints(geom public.geometry, tolerance double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_removerepeatedpoints(geom public.geometry, tolerance double precision) TO admin;


--
-- Name: FUNCTION st_reverse(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_reverse(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_reverse(public.geometry) TO admin;


--
-- Name: FUNCTION st_rotate(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_rotate(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_rotate(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_rotate(public.geometry, double precision, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_rotate(public.geometry, double precision, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_rotate(public.geometry, double precision, public.geometry) TO admin;


--
-- Name: FUNCTION st_rotate(public.geometry, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_rotate(public.geometry, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_rotate(public.geometry, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_rotatex(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_rotatex(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_rotatex(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_rotatey(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_rotatey(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_rotatey(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_rotatez(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_rotatez(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_rotatez(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_scale(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_scale(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_scale(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION st_scale(public.geometry, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_scale(public.geometry, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_scale(public.geometry, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_scale(public.geometry, public.geometry, origin public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_scale(public.geometry, public.geometry, origin public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_scale(public.geometry, public.geometry, origin public.geometry) TO admin;


--
-- Name: FUNCTION st_scale(public.geometry, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_scale(public.geometry, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_scale(public.geometry, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_scroll(public.geometry, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_scroll(public.geometry, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_scroll(public.geometry, public.geometry) TO admin;


--
-- Name: FUNCTION st_segmentize(geog public.geography, max_segment_length double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_segmentize(geog public.geography, max_segment_length double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_segmentize(geog public.geography, max_segment_length double precision) TO admin;


--
-- Name: FUNCTION st_segmentize(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_segmentize(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_segmentize(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_seteffectivearea(public.geometry, double precision, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_seteffectivearea(public.geometry, double precision, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_seteffectivearea(public.geometry, double precision, integer) TO admin;


--
-- Name: FUNCTION st_setpoint(public.geometry, integer, public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_setpoint(public.geometry, integer, public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_setpoint(public.geometry, integer, public.geometry) TO admin;


--
-- Name: FUNCTION st_setsrid(geog public.geography, srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_setsrid(geog public.geography, srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_setsrid(geog public.geography, srid integer) TO admin;


--
-- Name: FUNCTION st_setsrid(geom public.geometry, srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_setsrid(geom public.geometry, srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_setsrid(geom public.geometry, srid integer) TO admin;


--
-- Name: FUNCTION st_sharedpaths(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_sharedpaths(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_sharedpaths(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_shiftlongitude(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_shiftlongitude(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_shiftlongitude(public.geometry) TO admin;


--
-- Name: FUNCTION st_shortestline(text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_shortestline(text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_shortestline(text, text) TO admin;


--
-- Name: FUNCTION st_shortestline(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_shortestline(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_shortestline(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_shortestline(public.geography, public.geography, use_spheroid boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_shortestline(public.geography, public.geography, use_spheroid boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_shortestline(public.geography, public.geography, use_spheroid boolean) TO admin;


--
-- Name: FUNCTION st_simplify(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_simplify(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_simplify(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_simplify(public.geometry, double precision, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_simplify(public.geometry, double precision, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_simplify(public.geometry, double precision, boolean) TO admin;


--
-- Name: FUNCTION st_simplifypolygonhull(geom public.geometry, vertex_fraction double precision, is_outer boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_simplifypolygonhull(geom public.geometry, vertex_fraction double precision, is_outer boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_simplifypolygonhull(geom public.geometry, vertex_fraction double precision, is_outer boolean) TO admin;


--
-- Name: FUNCTION st_simplifypreservetopology(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_simplifypreservetopology(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_simplifypreservetopology(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_simplifyvw(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_simplifyvw(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_simplifyvw(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_snap(geom1 public.geometry, geom2 public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_snap(geom1 public.geometry, geom2 public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_snap(geom1 public.geometry, geom2 public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_snaptogrid(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_snaptogrid(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_snaptogrid(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_snaptogrid(public.geometry, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_snaptogrid(public.geometry, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_snaptogrid(public.geometry, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_snaptogrid(public.geometry, double precision, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_snaptogrid(public.geometry, double precision, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_snaptogrid(public.geometry, double precision, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_snaptogrid(geom1 public.geometry, geom2 public.geometry, double precision, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_snaptogrid(geom1 public.geometry, geom2 public.geometry, double precision, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_snaptogrid(geom1 public.geometry, geom2 public.geometry, double precision, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_split(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_split(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_split(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_square(size double precision, cell_i integer, cell_j integer, origin public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_square(size double precision, cell_i integer, cell_j integer, origin public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_square(size double precision, cell_i integer, cell_j integer, origin public.geometry) TO admin;


--
-- Name: FUNCTION st_squaregrid(size double precision, bounds public.geometry, OUT geom public.geometry, OUT i integer, OUT j integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_squaregrid(size double precision, bounds public.geometry, OUT geom public.geometry, OUT i integer, OUT j integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_squaregrid(size double precision, bounds public.geometry, OUT geom public.geometry, OUT i integer, OUT j integer) TO admin;


--
-- Name: FUNCTION st_srid(geog public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_srid(geog public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_srid(geog public.geography) TO admin;


--
-- Name: FUNCTION st_srid(geom public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_srid(geom public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_srid(geom public.geometry) TO admin;


--
-- Name: FUNCTION st_startpoint(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_startpoint(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_startpoint(public.geometry) TO admin;


--
-- Name: FUNCTION st_subdivide(geom public.geometry, maxvertices integer, gridsize double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_subdivide(geom public.geometry, maxvertices integer, gridsize double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_subdivide(geom public.geometry, maxvertices integer, gridsize double precision) TO admin;


--
-- Name: FUNCTION st_summary(public.geography); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_summary(public.geography) TO readwrite;
GRANT ALL ON FUNCTION public.st_summary(public.geography) TO admin;


--
-- Name: FUNCTION st_summary(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_summary(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_summary(public.geometry) TO admin;


--
-- Name: FUNCTION st_swapordinates(geom public.geometry, ords cstring); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_swapordinates(geom public.geometry, ords cstring) TO readwrite;
GRANT ALL ON FUNCTION public.st_swapordinates(geom public.geometry, ords cstring) TO admin;


--
-- Name: FUNCTION st_symdifference(geom1 public.geometry, geom2 public.geometry, gridsize double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_symdifference(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_symdifference(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO admin;


--
-- Name: FUNCTION st_symmetricdifference(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_symmetricdifference(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_symmetricdifference(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_tileenvelope(zoom integer, x integer, y integer, bounds public.geometry, margin double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_tileenvelope(zoom integer, x integer, y integer, bounds public.geometry, margin double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_tileenvelope(zoom integer, x integer, y integer, bounds public.geometry, margin double precision) TO admin;


--
-- Name: FUNCTION st_touches(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_touches(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_touches(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_transform(public.geometry, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_transform(public.geometry, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_transform(public.geometry, integer) TO admin;


--
-- Name: FUNCTION st_transform(geom public.geometry, to_proj text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_transform(geom public.geometry, to_proj text) TO readwrite;
GRANT ALL ON FUNCTION public.st_transform(geom public.geometry, to_proj text) TO admin;


--
-- Name: FUNCTION st_transform(geom public.geometry, from_proj text, to_srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_transform(geom public.geometry, from_proj text, to_srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_transform(geom public.geometry, from_proj text, to_srid integer) TO admin;


--
-- Name: FUNCTION st_transform(geom public.geometry, from_proj text, to_proj text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_transform(geom public.geometry, from_proj text, to_proj text) TO readwrite;
GRANT ALL ON FUNCTION public.st_transform(geom public.geometry, from_proj text, to_proj text) TO admin;


--
-- Name: FUNCTION st_transformpipeline(geom public.geometry, pipeline text, to_srid integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_transformpipeline(geom public.geometry, pipeline text, to_srid integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_transformpipeline(geom public.geometry, pipeline text, to_srid integer) TO admin;


--
-- Name: FUNCTION st_translate(public.geometry, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_translate(public.geometry, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_translate(public.geometry, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_translate(public.geometry, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_translate(public.geometry, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_translate(public.geometry, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_transscale(public.geometry, double precision, double precision, double precision, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_transscale(public.geometry, double precision, double precision, double precision, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_transscale(public.geometry, double precision, double precision, double precision, double precision) TO admin;


--
-- Name: FUNCTION st_triangulatepolygon(g1 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_triangulatepolygon(g1 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_triangulatepolygon(g1 public.geometry) TO admin;


--
-- Name: FUNCTION st_unaryunion(public.geometry, gridsize double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_unaryunion(public.geometry, gridsize double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_unaryunion(public.geometry, gridsize double precision) TO admin;


--
-- Name: FUNCTION st_union(public.geometry[]); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_union(public.geometry[]) TO readwrite;
GRANT ALL ON FUNCTION public.st_union(public.geometry[]) TO admin;


--
-- Name: FUNCTION st_union(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_union(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_union(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_union(geom1 public.geometry, geom2 public.geometry, gridsize double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_union(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_union(geom1 public.geometry, geom2 public.geometry, gridsize double precision) TO admin;


--
-- Name: FUNCTION st_voronoilines(g1 public.geometry, tolerance double precision, extend_to public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_voronoilines(g1 public.geometry, tolerance double precision, extend_to public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_voronoilines(g1 public.geometry, tolerance double precision, extend_to public.geometry) TO admin;


--
-- Name: FUNCTION st_voronoipolygons(g1 public.geometry, tolerance double precision, extend_to public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_voronoipolygons(g1 public.geometry, tolerance double precision, extend_to public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_voronoipolygons(g1 public.geometry, tolerance double precision, extend_to public.geometry) TO admin;


--
-- Name: FUNCTION st_within(geom1 public.geometry, geom2 public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_within(geom1 public.geometry, geom2 public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_within(geom1 public.geometry, geom2 public.geometry) TO admin;


--
-- Name: FUNCTION st_wkbtosql(wkb bytea); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_wkbtosql(wkb bytea) TO readwrite;
GRANT ALL ON FUNCTION public.st_wkbtosql(wkb bytea) TO admin;


--
-- Name: FUNCTION st_wkttosql(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_wkttosql(text) TO readwrite;
GRANT ALL ON FUNCTION public.st_wkttosql(text) TO admin;


--
-- Name: FUNCTION st_wrapx(geom public.geometry, wrap double precision, move double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_wrapx(geom public.geometry, wrap double precision, move double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_wrapx(geom public.geometry, wrap double precision, move double precision) TO admin;


--
-- Name: FUNCTION st_x(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_x(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_x(public.geometry) TO admin;


--
-- Name: FUNCTION st_xmax(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_xmax(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.st_xmax(public.box3d) TO admin;


--
-- Name: FUNCTION st_xmin(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_xmin(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.st_xmin(public.box3d) TO admin;


--
-- Name: FUNCTION st_y(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_y(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_y(public.geometry) TO admin;


--
-- Name: FUNCTION st_ymax(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_ymax(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.st_ymax(public.box3d) TO admin;


--
-- Name: FUNCTION st_ymin(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_ymin(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.st_ymin(public.box3d) TO admin;


--
-- Name: FUNCTION st_z(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_z(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_z(public.geometry) TO admin;


--
-- Name: FUNCTION st_zmax(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_zmax(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.st_zmax(public.box3d) TO admin;


--
-- Name: FUNCTION st_zmflag(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_zmflag(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_zmflag(public.geometry) TO admin;


--
-- Name: FUNCTION st_zmin(public.box3d); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_zmin(public.box3d) TO readwrite;
GRANT ALL ON FUNCTION public.st_zmin(public.box3d) TO admin;


--
-- Name: FUNCTION unlockrows(text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.unlockrows(text) TO readwrite;
GRANT ALL ON FUNCTION public.unlockrows(text) TO admin;


--
-- Name: FUNCTION updategeometrysrid(character varying, character varying, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.updategeometrysrid(character varying, character varying, integer) TO readwrite;
GRANT ALL ON FUNCTION public.updategeometrysrid(character varying, character varying, integer) TO admin;


--
-- Name: FUNCTION updategeometrysrid(character varying, character varying, character varying, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.updategeometrysrid(character varying, character varying, character varying, integer) TO readwrite;
GRANT ALL ON FUNCTION public.updategeometrysrid(character varying, character varying, character varying, integer) TO admin;


--
-- Name: FUNCTION updategeometrysrid(catalogn_name character varying, schema_name character varying, table_name character varying, column_name character varying, new_srid_in integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.updategeometrysrid(catalogn_name character varying, schema_name character varying, table_name character varying, column_name character varying, new_srid_in integer) TO readwrite;
GRANT ALL ON FUNCTION public.updategeometrysrid(catalogn_name character varying, schema_name character varying, table_name character varying, column_name character varying, new_srid_in integer) TO admin;


--
-- Name: FUNCTION st_3dextent(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_3dextent(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_3dextent(public.geometry) TO admin;


--
-- Name: FUNCTION st_asflatgeobuf(anyelement); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asflatgeobuf(anyelement) TO readwrite;
GRANT ALL ON FUNCTION public.st_asflatgeobuf(anyelement) TO admin;


--
-- Name: FUNCTION st_asflatgeobuf(anyelement, boolean); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asflatgeobuf(anyelement, boolean) TO readwrite;
GRANT ALL ON FUNCTION public.st_asflatgeobuf(anyelement, boolean) TO admin;


--
-- Name: FUNCTION st_asflatgeobuf(anyelement, boolean, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asflatgeobuf(anyelement, boolean, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asflatgeobuf(anyelement, boolean, text) TO admin;


--
-- Name: FUNCTION st_asgeobuf(anyelement); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgeobuf(anyelement) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgeobuf(anyelement) TO admin;


--
-- Name: FUNCTION st_asgeobuf(anyelement, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asgeobuf(anyelement, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asgeobuf(anyelement, text) TO admin;


--
-- Name: FUNCTION st_asmvt(anyelement); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asmvt(anyelement) TO readwrite;
GRANT ALL ON FUNCTION public.st_asmvt(anyelement) TO admin;


--
-- Name: FUNCTION st_asmvt(anyelement, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text) TO admin;


--
-- Name: FUNCTION st_asmvt(anyelement, text, integer); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text, integer) TO readwrite;
GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text, integer) TO admin;


--
-- Name: FUNCTION st_asmvt(anyelement, text, integer, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text, integer, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text, integer, text) TO admin;


--
-- Name: FUNCTION st_asmvt(anyelement, text, integer, text, text); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text, integer, text, text) TO readwrite;
GRANT ALL ON FUNCTION public.st_asmvt(anyelement, text, integer, text, text) TO admin;


--
-- Name: FUNCTION st_clusterintersecting(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterintersecting(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterintersecting(public.geometry) TO admin;


--
-- Name: FUNCTION st_clusterwithin(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_clusterwithin(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_clusterwithin(public.geometry, double precision) TO admin;


--
-- Name: FUNCTION st_collect(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_collect(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_collect(public.geometry) TO admin;


--
-- Name: FUNCTION st_coverageunion(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_coverageunion(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_coverageunion(public.geometry) TO admin;


--
-- Name: FUNCTION st_extent(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_extent(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_extent(public.geometry) TO admin;


--
-- Name: FUNCTION st_makeline(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_makeline(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_makeline(public.geometry) TO admin;


--
-- Name: FUNCTION st_memcollect(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_memcollect(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_memcollect(public.geometry) TO admin;


--
-- Name: FUNCTION st_memunion(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_memunion(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_memunion(public.geometry) TO admin;


--
-- Name: FUNCTION st_polygonize(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_polygonize(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_polygonize(public.geometry) TO admin;


--
-- Name: FUNCTION st_union(public.geometry); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_union(public.geometry) TO readwrite;
GRANT ALL ON FUNCTION public.st_union(public.geometry) TO admin;


--
-- Name: FUNCTION st_union(public.geometry, double precision); Type: ACL; Schema: public; Owner: -
--

GRANT ALL ON FUNCTION public.st_union(public.geometry, double precision) TO readwrite;
GRANT ALL ON FUNCTION public.st_union(public.geometry, double precision) TO admin;


--
-- Name: TABLE account_roles; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.account_roles TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.account_roles TO readwrite;
GRANT ALL ON TABLE public.account_roles TO admin;


--
-- Name: SEQUENCE account_roles_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.account_roles_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.account_roles_id_seq TO admin;


--
-- Name: TABLE addresses; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.addresses TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.addresses TO readwrite;
GRANT ALL ON TABLE public.addresses TO admin;


--
-- Name: TABLE addresses_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.addresses_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.addresses_history TO readwrite;
GRANT ALL ON TABLE public.addresses_history TO admin;


--
-- Name: SEQUENCE addresses_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.addresses_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.addresses_id_seq TO admin;


--
-- Name: TABLE alembic_version; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.alembic_version TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.alembic_version TO readwrite;
GRANT ALL ON TABLE public.alembic_version TO admin;


--
-- Name: TABLE application; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.application TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.application TO readwrite;
GRANT ALL ON TABLE public.application TO admin;


--
-- Name: SEQUENCE application_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.application_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.application_id_seq TO admin;


--
-- Name: TABLE auto_approval_records; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.auto_approval_records TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.auto_approval_records TO readwrite;
GRANT ALL ON TABLE public.auto_approval_records TO admin;


--
-- Name: SEQUENCE auto_approval_records_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.auto_approval_records_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.auto_approval_records_id_seq TO admin;


--
-- Name: TABLE bulk_validation; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.bulk_validation TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.bulk_validation TO readwrite;
GRANT ALL ON TABLE public.bulk_validation TO admin;


--
-- Name: SEQUENCE bulk_validation_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.bulk_validation_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.bulk_validation_id_seq TO admin;


--
-- Name: TABLE certificates; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.certificates TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.certificates TO readwrite;
GRANT ALL ON TABLE public.certificates TO admin;


--
-- Name: SEQUENCE certificates_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.certificates_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.certificates_id_seq TO admin;


--
-- Name: TABLE conditions_of_approval; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.conditions_of_approval TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.conditions_of_approval TO readwrite;
GRANT ALL ON TABLE public.conditions_of_approval TO admin WITH GRANT OPTION;


--
-- Name: TABLE conditions_of_approval_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.conditions_of_approval_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.conditions_of_approval_history TO readwrite;
GRANT ALL ON TABLE public.conditions_of_approval_history TO admin WITH GRANT OPTION;


--
-- Name: SEQUENCE conditions_of_approval_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.conditions_of_approval_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.conditions_of_approval_id_seq TO admin WITH GRANT OPTION;


--
-- Name: TABLE contacts; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.contacts TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.contacts TO readwrite;
GRANT ALL ON TABLE public.contacts TO admin;


--
-- Name: TABLE contacts_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.contacts_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.contacts_history TO readwrite;
GRANT ALL ON TABLE public.contacts_history TO admin;


--
-- Name: SEQUENCE contacts_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.contacts_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.contacts_id_seq TO admin;


--
-- Name: TABLE documents; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.documents TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.documents TO readwrite;
GRANT ALL ON TABLE public.documents TO admin;


--
-- Name: TABLE documents_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.documents_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.documents_history TO readwrite;
GRANT ALL ON TABLE public.documents_history TO admin;


--
-- Name: SEQUENCE documents_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.documents_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.documents_id_seq TO admin;


--
-- Name: SEQUENCE dss_organization_organization_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.dss_organization_organization_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.dss_organization_organization_id_seq TO admin;


--
-- Name: TABLE events; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.events TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.events TO readwrite;
GRANT ALL ON TABLE public.events TO admin;


--
-- Name: SEQUENCE events_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.events_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.events_id_seq TO admin;


--
-- Name: TABLE geography_columns; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.geography_columns TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.geography_columns TO readwrite;
GRANT ALL ON TABLE public.geography_columns TO admin;


--
-- Name: TABLE geometry_columns; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.geometry_columns TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.geometry_columns TO readwrite;
GRANT ALL ON TABLE public.geometry_columns TO admin;


--
-- Name: TABLE ltsa; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.ltsa TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.ltsa TO readwrite;
GRANT ALL ON TABLE public.ltsa TO admin;


--
-- Name: SEQUENCE ltsa_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.ltsa_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.ltsa_id_seq TO admin;


--
-- Name: TABLE notice_of_consideration; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.notice_of_consideration TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.notice_of_consideration TO readwrite;
GRANT ALL ON TABLE public.notice_of_consideration TO admin;


--
-- Name: SEQUENCE notice_of_consideration_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.notice_of_consideration_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.notice_of_consideration_id_seq TO admin;


--
-- Name: TABLE platform_brands; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platform_brands TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platform_brands TO readwrite;
GRANT ALL ON TABLE public.platform_brands TO admin;


--
-- Name: TABLE platform_brands_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platform_brands_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platform_brands_history TO readwrite;
GRANT ALL ON TABLE public.platform_brands_history TO admin;


--
-- Name: SEQUENCE platform_brands_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.platform_brands_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.platform_brands_id_seq TO admin;


--
-- Name: TABLE platform_registration; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platform_registration TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platform_registration TO readwrite;
GRANT ALL ON TABLE public.platform_registration TO admin;


--
-- Name: TABLE platform_registration_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platform_registration_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platform_registration_history TO readwrite;
GRANT ALL ON TABLE public.platform_registration_history TO admin;


--
-- Name: SEQUENCE platform_registration_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.platform_registration_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.platform_registration_id_seq TO admin;


--
-- Name: TABLE platform_representatives; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platform_representatives TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platform_representatives TO readwrite;
GRANT ALL ON TABLE public.platform_representatives TO admin;


--
-- Name: TABLE platform_representatives_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platform_representatives_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platform_representatives_history TO readwrite;
GRANT ALL ON TABLE public.platform_representatives_history TO admin;


--
-- Name: SEQUENCE platform_representatives_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.platform_representatives_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.platform_representatives_id_seq TO admin;


--
-- Name: TABLE platforms; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platforms TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platforms TO readwrite;
GRANT ALL ON TABLE public.platforms TO admin;


--
-- Name: TABLE platforms_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.platforms_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.platforms_history TO readwrite;
GRANT ALL ON TABLE public.platforms_history TO admin;


--
-- Name: SEQUENCE platforms_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.platforms_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.platforms_id_seq TO admin;


--
-- Name: TABLE property_contacts; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.property_contacts TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.property_contacts TO readwrite;
GRANT ALL ON TABLE public.property_contacts TO admin;


--
-- Name: TABLE property_contacts_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.property_contacts_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.property_contacts_history TO readwrite;
GRANT ALL ON TABLE public.property_contacts_history TO admin;


--
-- Name: SEQUENCE property_contacts_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.property_contacts_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.property_contacts_id_seq TO admin;


--
-- Name: TABLE property_listings; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.property_listings TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.property_listings TO readwrite;
GRANT ALL ON TABLE public.property_listings TO admin;


--
-- Name: TABLE property_listings_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.property_listings_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.property_listings_history TO readwrite;
GRANT ALL ON TABLE public.property_listings_history TO admin;


--
-- Name: SEQUENCE property_listings_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.property_listings_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.property_listings_id_seq TO admin;


--
-- Name: TABLE property_manager; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.property_manager TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.property_manager TO readwrite;
GRANT ALL ON TABLE public.property_manager TO admin;


--
-- Name: TABLE property_manager_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.property_manager_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.property_manager_history TO readwrite;
GRANT ALL ON TABLE public.property_manager_history TO admin;


--
-- Name: SEQUENCE property_manager_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.property_manager_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.property_manager_id_seq TO admin;


--
-- Name: TABLE real_time_validation; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.real_time_validation TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.real_time_validation TO readwrite;
GRANT ALL ON TABLE public.real_time_validation TO admin;


--
-- Name: SEQUENCE real_time_validation_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.real_time_validation_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.real_time_validation_id_seq TO admin;


--
-- Name: TABLE registration_notice_of_consideration; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.registration_notice_of_consideration TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.registration_notice_of_consideration TO readwrite;
GRANT ALL ON TABLE public.registration_notice_of_consideration TO admin WITH GRANT OPTION;


--
-- Name: SEQUENCE registration_notice_of_consideration_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.registration_notice_of_consideration_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.registration_notice_of_consideration_id_seq TO admin WITH GRANT OPTION;


--
-- Name: TABLE registration_snapshot; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.registration_snapshot TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.registration_snapshot TO readwrite;
GRANT ALL ON TABLE public.registration_snapshot TO admin WITH GRANT OPTION;


--
-- Name: SEQUENCE registration_snapshot_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.registration_snapshot_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.registration_snapshot_id_seq TO admin WITH GRANT OPTION;


--
-- Name: TABLE registrations; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.registrations TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.registrations TO readwrite;
GRANT ALL ON TABLE public.registrations TO admin;


--
-- Name: TABLE registrations_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.registrations_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.registrations_history TO readwrite;
GRANT ALL ON TABLE public.registrations_history TO admin;


--
-- Name: SEQUENCE registrations_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.registrations_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.registrations_id_seq TO admin;


--
-- Name: TABLE rental_properties; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.rental_properties TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.rental_properties TO readwrite;
GRANT ALL ON TABLE public.rental_properties TO admin;


--
-- Name: TABLE rental_properties_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.rental_properties_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.rental_properties_history TO readwrite;
GRANT ALL ON TABLE public.rental_properties_history TO admin;


--
-- Name: SEQUENCE rental_properties_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.rental_properties_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.rental_properties_id_seq TO admin;


--
-- Name: TABLE spatial_ref_sys; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.spatial_ref_sys TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.spatial_ref_sys TO readwrite;
GRANT ALL ON TABLE public.spatial_ref_sys TO admin;


--
-- Name: TABLE strata_hotel_buildings; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotel_buildings TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotel_buildings TO readwrite;
GRANT ALL ON TABLE public.strata_hotel_buildings TO admin;


--
-- Name: TABLE strata_hotel_buildings_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotel_buildings_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotel_buildings_history TO readwrite;
GRANT ALL ON TABLE public.strata_hotel_buildings_history TO admin;


--
-- Name: SEQUENCE strata_hotel_buildings_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.strata_hotel_buildings_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.strata_hotel_buildings_id_seq TO admin;


--
-- Name: TABLE strata_hotel_registration; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotel_registration TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotel_registration TO readwrite;
GRANT ALL ON TABLE public.strata_hotel_registration TO admin;


--
-- Name: TABLE strata_hotel_registration_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotel_registration_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotel_registration_history TO readwrite;
GRANT ALL ON TABLE public.strata_hotel_registration_history TO admin;


--
-- Name: SEQUENCE strata_hotel_registration_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.strata_hotel_registration_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.strata_hotel_registration_id_seq TO admin;


--
-- Name: TABLE strata_hotel_representatives; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotel_representatives TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotel_representatives TO readwrite;
GRANT ALL ON TABLE public.strata_hotel_representatives TO admin;


--
-- Name: TABLE strata_hotel_representatives_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotel_representatives_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotel_representatives_history TO readwrite;
GRANT ALL ON TABLE public.strata_hotel_representatives_history TO admin;


--
-- Name: SEQUENCE strata_hotel_representatives_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.strata_hotel_representatives_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.strata_hotel_representatives_id_seq TO admin;


--
-- Name: TABLE strata_hotels; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotels TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotels TO readwrite;
GRANT ALL ON TABLE public.strata_hotels TO admin;


--
-- Name: TABLE strata_hotels_history; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.strata_hotels_history TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.strata_hotels_history TO readwrite;
GRANT ALL ON TABLE public.strata_hotels_history TO admin;


--
-- Name: SEQUENCE strata_hotels_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.strata_hotels_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.strata_hotels_id_seq TO admin;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT ON TABLE public.users TO readonly;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.users TO readwrite;
GRANT ALL ON TABLE public.users TO admin;


--
-- Name: SEQUENCE users_id_seq; Type: ACL; Schema: public; Owner: -
--

GRANT SELECT,USAGE ON SEQUENCE public.users_id_seq TO readwrite;
GRANT ALL ON SEQUENCE public.users_id_seq TO admin;


--
-- Name: DEFAULT PRIVILEGES FOR SEQUENCES; Type: DEFAULT ACL; Schema: public; Owner: -
--

ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT SELECT,USAGE ON SEQUENCES  TO readwrite;
ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT ALL ON SEQUENCES  TO admin WITH GRANT OPTION;


--
-- Name: DEFAULT PRIVILEGES FOR TYPES; Type: DEFAULT ACL; Schema: public; Owner: -
--

ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT ALL ON TYPES  TO admin WITH GRANT OPTION;


--
-- Name: DEFAULT PRIVILEGES FOR FUNCTIONS; Type: DEFAULT ACL; Schema: public; Owner: -
--

ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT ALL ON FUNCTIONS  TO readwrite;
ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT ALL ON FUNCTIONS  TO admin WITH GRANT OPTION;


--
-- Name: DEFAULT PRIVILEGES FOR TABLES; Type: DEFAULT ACL; Schema: public; Owner: -
--

ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT SELECT ON TABLES  TO readonly;
ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT SELECT,INSERT,DELETE,UPDATE ON TABLES  TO readwrite;
ALTER DEFAULT PRIVILEGES FOR ROLE strr IN SCHEMA public GRANT ALL ON TABLES  TO admin WITH GRANT OPTION;


--
-- PostgreSQL database dump complete
--

\unrestrict qbFtGTIWvifiEpqPpQgcsr2YimbDaJaBEAnUabRGqhrbB0xlkpRhmM4jXDdi0EK

