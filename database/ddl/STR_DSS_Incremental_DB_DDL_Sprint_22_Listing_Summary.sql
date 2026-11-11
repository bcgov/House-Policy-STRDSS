/* STR DSS - Rental listing summary (incremental DDL)
 *
 * One row per master listing (including_rental_listing_report_id IS NULL).
 * Listing and grouped listing searches read this table instead of recomputing
 * the latest action, latest report month, property-owner name, trimmed host
 * name, and licence digits on every query.
 *
 * Safe inside a transaction. The Sprint 22 migration calls this script.
 * Run on its own as the application role:
 *   psql -U strdssdev -d strdssdev -f STR_DSS_Incremental_DB_DDL_Sprint_22_Listing_Summary.sql
 *
 * Unqualified names resolve through search_path. Do not schema-qualify them.
 */

SET search_path TO dss, public;

CREATE TABLE dss_rental_listing_summary (
	rental_listing_id            bigint       NOT NULL,
	last_action_nm               varchar(200),
	last_action_dtm              timestamptz,
	last_rental_listing_action_id bigint,
	latest_report_period_ym      date,
	primary_host_full_nm         varchar(320),
	business_licence_digits         text,
	effective_host_nm           varchar(320),
	bc_registry_no               varchar(50),
	match_address_txt            varchar(250),
	match_unit_no                varchar(100),
	listing_business_licence_no  varchar(320),
	upd_dtm                      timestamptz  NOT NULL DEFAULT now(),
	CONSTRAINT dss_rental_listing_summary_pk PRIMARY KEY ( rental_listing_id )
);

CREATE INDEX dss_rental_listing_summary_i1
	ON dss_rental_listing_summary ( latest_report_period_ym );

CREATE INDEX dss_rental_listing_summary_i2
	ON dss_rental_listing_summary ( last_action_dtm );

ALTER TABLE dss_rental_listing_summary
	ADD CONSTRAINT dss_rental_listing_summary_fk_listing
	FOREIGN KEY ( rental_listing_id ) REFERENCES dss_rental_listing ( rental_listing_id )
	ON DELETE CASCADE;

COMMENT ON TABLE dss_rental_listing_summary IS
	'Precomputed search values for the current master rental listing. Maintained by triggers and rebuilt by dss_refresh_rental_listing_summary after a listing report upload.';

COMMENT ON COLUMN dss_rental_listing_summary.rental_listing_id IS
	'Master rental listing. Snapshots that belong to a monthly report are not stored.';

COMMENT ON COLUMN dss_rental_listing_summary.last_action_nm IS
	'action_long_nm of the latest dss_rental_listing_action, ordered by action_dtm DESC, rental_listing_action_id DESC. Null when the listing has no action.';

COMMENT ON COLUMN dss_rental_listing_summary.last_action_dtm IS
	'action_dtm of that same action. Null when the listing has no action.';

COMMENT ON COLUMN dss_rental_listing_summary.last_rental_listing_action_id IS
	'rental_listing_action_id of that same action. A new action replaces the name, timestamp, and this id. Null when the listing has no action.';

COMMENT ON COLUMN dss_rental_listing_summary.latest_report_period_ym IS
	'Latest report_period_ym for this offering organization and platform listing number. Null when the listing has never been in a report.';

COMMENT ON COLUMN dss_rental_listing_summary.primary_host_full_nm IS
	'Trimmed full_nm of the property-owner contact with the lowest rental_listing_contact_id. Null when the listing has no property-owner name. Grouped search falls back to effective_host_nm.';

COMMENT ON COLUMN dss_rental_listing_summary.business_licence_digits IS
	'Digits only from business_licence_no: NULLIF(regexp_replace(NULLIF(btrim(business_licence_no), ''''), ''[^0-9]'', '''', ''g''), ''''). Null when the licence has no digits.';

COMMENT ON COLUMN dss_rental_listing_summary.effective_host_nm IS
	'Trimmed effective_host_nm: NULLIF(btrim(effective_host_nm), ''''). Blank becomes null.';

COMMENT ON COLUMN dss_rental_listing_summary.bc_registry_no IS
	'Trimmed registration number: NULLIF(btrim(bc_registry_no), ''''). Null groups the listing by address, unit, host, and licence digits.';

COMMENT ON COLUMN dss_rental_listing_summary.match_address_txt IS
	'Normalized matched address: NULLIF(btrim(match_address_txt), '''').';

COMMENT ON COLUMN dss_rental_listing_summary.match_unit_no IS
	'Normalized unit number: NULLIF(btrim(unit_no), '''').';

COMMENT ON COLUMN dss_rental_listing_summary.listing_business_licence_no IS
	'Normalized listing licence, before digit extraction: NULLIF(btrim(business_licence_no), '''').';

COMMENT ON COLUMN dss_rental_listing_summary.upd_dtm IS
	'Timestamp when this summary row was last rebuilt.';

CREATE OR REPLACE FUNCTION dss_rental_listing_summary_rows(p_rental_listing_id bigint)
RETURNS TABLE (
	rental_listing_id bigint,
	last_action_nm varchar(200),
	last_action_dtm timestamptz,
	last_rental_listing_action_id bigint,
	latest_report_period_ym date,
	primary_host_full_nm varchar(320),
	business_licence_digits text,
	effective_host_nm varchar(320),
	bc_registry_no varchar(50),
	match_address_txt varchar(250),
	match_unit_no varchar(100),
	listing_business_licence_no varchar(320),
	upd_dtm timestamptz
)
LANGUAGE sql
STABLE
SET search_path TO dss, public
AS $$
	SELECT
		drl.rental_listing_id,
		la.action_long_nm,
		la.action_dtm,
		la.rental_listing_action_id,
		lrp.latest_report_period_ym,
		host.primary_host_full_nm,
		NULLIF(regexp_replace(NULLIF(btrim(drl.business_licence_no), ''), '[^0-9]', '', 'g'), ''),
		NULLIF(btrim(drl.effective_host_nm), ''),
		NULLIF(btrim(drl.bc_registry_no), ''),
		NULLIF(btrim(dpa.match_address_txt), ''),
		NULLIF(btrim(dpa.unit_no), ''),
		NULLIF(btrim(drl.business_licence_no), ''),
		now()
	FROM dss_rental_listing drl
	LEFT JOIN dss_physical_address dpa
		ON dpa.physical_address_id = drl.locating_physical_address_id
	LEFT JOIN LATERAL (
		SELECT a.rental_listing_action_id, a.action_long_nm, a.action_dtm
		FROM dss_rental_listing_action a
		WHERE a.rental_listing_id = drl.rental_listing_id
		ORDER BY a.action_dtm DESC, a.rental_listing_action_id DESC
		LIMIT 1
	) la ON TRUE
	LEFT JOIN LATERAL (
		SELECT NULLIF(btrim(c.full_nm), '') AS primary_host_full_nm
		FROM dss_rental_listing_contact c
		WHERE c.contacted_through_rental_listing_id = drl.rental_listing_id
		  AND c.is_property_owner
		ORDER BY c.rental_listing_contact_id
		LIMIT 1
	) host ON TRUE
	LEFT JOIN (
		SELECT
			snap.offering_organization_id,
			snap.platform_listing_no,
			max(drlr.report_period_ym) AS latest_report_period_ym
		FROM dss_rental_listing snap
		JOIN dss_rental_listing_report drlr
			ON drlr.rental_listing_report_id = snap.including_rental_listing_report_id
		WHERE snap.including_rental_listing_report_id IS NOT NULL
		  AND snap.offering_organization_id IS NOT NULL
		  AND snap.platform_listing_no IS NOT NULL
		  AND (
			p_rental_listing_id IS NULL
			OR (snap.offering_organization_id, snap.platform_listing_no) IN (
				SELECT one.offering_organization_id, one.platform_listing_no
				FROM dss_rental_listing one
				WHERE one.rental_listing_id = p_rental_listing_id
			)
		  )
		GROUP BY snap.offering_organization_id, snap.platform_listing_no
	) lrp
		ON lrp.offering_organization_id = drl.offering_organization_id
	   AND lrp.platform_listing_no = drl.platform_listing_no
	WHERE drl.including_rental_listing_report_id IS NULL
	  AND (p_rental_listing_id IS NULL OR drl.rental_listing_id = p_rental_listing_id);
$$;

CREATE OR REPLACE FUNCTION dss_upsert_rental_listing_summary(p_rental_listing_id bigint)
RETURNS void
LANGUAGE plpgsql
SET search_path TO dss, public
AS $$
BEGIN
	IF NOT EXISTS (
		SELECT 1
		FROM dss_rental_listing
		WHERE rental_listing_id = p_rental_listing_id
		  AND including_rental_listing_report_id IS NULL
	) THEN
		DELETE FROM dss_rental_listing_summary
		WHERE rental_listing_id = p_rental_listing_id;
		RETURN;
	END IF;

	INSERT INTO dss_rental_listing_summary (
		rental_listing_id, last_action_nm, last_action_dtm, last_rental_listing_action_id,
		latest_report_period_ym, primary_host_full_nm, business_licence_digits, effective_host_nm,
		bc_registry_no, match_address_txt, match_unit_no, listing_business_licence_no, upd_dtm
	)
	SELECT
		rental_listing_id, last_action_nm, last_action_dtm, last_rental_listing_action_id,
		latest_report_period_ym, primary_host_full_nm, business_licence_digits, effective_host_nm,
		bc_registry_no, match_address_txt, match_unit_no, listing_business_licence_no, upd_dtm
	FROM dss_rental_listing_summary_rows(p_rental_listing_id)
	ON CONFLICT (rental_listing_id) DO UPDATE SET
		last_action_nm = EXCLUDED.last_action_nm,
		last_action_dtm = EXCLUDED.last_action_dtm,
		last_rental_listing_action_id = EXCLUDED.last_rental_listing_action_id,
		latest_report_period_ym = EXCLUDED.latest_report_period_ym,
		primary_host_full_nm = EXCLUDED.primary_host_full_nm,
		business_licence_digits = EXCLUDED.business_licence_digits,
		effective_host_nm = EXCLUDED.effective_host_nm,
		bc_registry_no = EXCLUDED.bc_registry_no,
		match_address_txt = EXCLUDED.match_address_txt,
		match_unit_no = EXCLUDED.match_unit_no,
		listing_business_licence_no = EXCLUDED.listing_business_licence_no,
		upd_dtm = EXCLUDED.upd_dtm;
END;
$$;

CREATE OR REPLACE FUNCTION dss_refresh_rental_listing_summary_report_period(
	p_offering_organization_id bigint,
	p_platform_listing_no varchar
)
RETURNS void
LANGUAGE sql
SET search_path TO dss, public
AS $$
	UPDATE dss_rental_listing_summary s
	SET latest_report_period_ym = lrp.latest_report_period_ym,
		upd_dtm = now()
	FROM dss_rental_listing master
	LEFT JOIN (
		SELECT max(drlr.report_period_ym) AS latest_report_period_ym
		FROM dss_rental_listing snap
		JOIN dss_rental_listing_report drlr
			ON drlr.rental_listing_report_id = snap.including_rental_listing_report_id
		WHERE snap.including_rental_listing_report_id IS NOT NULL
		  AND snap.offering_organization_id = p_offering_organization_id
		  AND snap.platform_listing_no = p_platform_listing_no
	) lrp ON TRUE
	WHERE master.including_rental_listing_report_id IS NULL
	  AND master.offering_organization_id = p_offering_organization_id
	  AND master.platform_listing_no = p_platform_listing_no
	  AND s.rental_listing_id = master.rental_listing_id
	  AND s.latest_report_period_ym IS DISTINCT FROM lrp.latest_report_period_ym;
$$;

CREATE OR REPLACE FUNCTION dss_refresh_rental_listing_summary()
RETURNS void
LANGUAGE plpgsql
SET search_path TO dss, public
AS $$
BEGIN
	INSERT INTO dss_rental_listing_summary (
		rental_listing_id, last_action_nm, last_action_dtm, last_rental_listing_action_id,
		latest_report_period_ym, primary_host_full_nm, business_licence_digits, effective_host_nm,
		bc_registry_no, match_address_txt, match_unit_no, listing_business_licence_no, upd_dtm
	)
	SELECT
		rental_listing_id, last_action_nm, last_action_dtm, last_rental_listing_action_id,
		latest_report_period_ym, primary_host_full_nm, business_licence_digits, effective_host_nm,
		bc_registry_no, match_address_txt, match_unit_no, listing_business_licence_no, upd_dtm
	FROM dss_rental_listing_summary_rows(NULL)
	ON CONFLICT (rental_listing_id) DO UPDATE SET
		last_action_nm = EXCLUDED.last_action_nm,
		last_action_dtm = EXCLUDED.last_action_dtm,
		last_rental_listing_action_id = EXCLUDED.last_rental_listing_action_id,
		latest_report_period_ym = EXCLUDED.latest_report_period_ym,
		primary_host_full_nm = EXCLUDED.primary_host_full_nm,
		business_licence_digits = EXCLUDED.business_licence_digits,
		effective_host_nm = EXCLUDED.effective_host_nm,
		bc_registry_no = EXCLUDED.bc_registry_no,
		match_address_txt = EXCLUDED.match_address_txt,
		match_unit_no = EXCLUDED.match_unit_no,
		listing_business_licence_no = EXCLUDED.listing_business_licence_no,
		upd_dtm = EXCLUDED.upd_dtm;

	DELETE FROM dss_rental_listing_summary s
	WHERE NOT EXISTS (
		SELECT 1
		FROM dss_rental_listing drl
		WHERE drl.rental_listing_id = s.rental_listing_id
		  AND drl.including_rental_listing_report_id IS NULL
	);
END;
$$;

CREATE OR REPLACE FUNCTION dss_rental_listing_summary_set_last_action(p_rental_listing_id bigint)
RETURNS void
LANGUAGE plpgsql
SET search_path TO dss, public
AS $$
BEGIN
	UPDATE dss_rental_listing_summary s
	SET last_rental_listing_action_id = la.rental_listing_action_id,
		last_action_nm = la.action_long_nm,
		last_action_dtm = la.action_dtm,
		upd_dtm = now()
	FROM (
		SELECT a.rental_listing_action_id, a.action_long_nm, a.action_dtm
		FROM dss_rental_listing_action a
		WHERE a.rental_listing_id = p_rental_listing_id
		ORDER BY a.action_dtm DESC, a.rental_listing_action_id DESC
		LIMIT 1
	) la
	WHERE s.rental_listing_id = p_rental_listing_id;

	IF NOT FOUND THEN
		UPDATE dss_rental_listing_summary
		SET last_rental_listing_action_id = NULL,
			last_action_nm = NULL,
			last_action_dtm = NULL,
			upd_dtm = now()
		WHERE rental_listing_id = p_rental_listing_id;
	END IF;
END;
$$;

CREATE OR REPLACE FUNCTION dss_rental_listing_summary_set_primary_host(p_rental_listing_id bigint)
RETURNS void
LANGUAGE sql
SET search_path TO dss, public
AS $$
	UPDATE dss_rental_listing_summary s
	SET primary_host_full_nm = (
			SELECT NULLIF(btrim(c.full_nm), '')
			FROM dss_rental_listing_contact c
			WHERE c.contacted_through_rental_listing_id = p_rental_listing_id
			  AND c.is_property_owner
			ORDER BY c.rental_listing_contact_id
			LIMIT 1
		),
		upd_dtm = now()
	WHERE s.rental_listing_id = p_rental_listing_id;
$$;

CREATE OR REPLACE FUNCTION dss_rental_listing_summary_from_listing()
RETURNS trigger
LANGUAGE plpgsql
SET search_path TO dss, public
AS $$
BEGIN
	IF TG_OP = 'DELETE' THEN
		IF OLD.including_rental_listing_report_id IS NOT NULL THEN
			PERFORM dss_refresh_rental_listing_summary_report_period(
				OLD.offering_organization_id, OLD.platform_listing_no);
		END IF;
		RETURN OLD;
	END IF;

	IF NEW.including_rental_listing_report_id IS NULL THEN
		IF TG_OP = 'INSERT' THEN
			PERFORM dss_upsert_rental_listing_summary(NEW.rental_listing_id);
		ELSE
			IF OLD.including_rental_listing_report_id IS NULL
			   AND (OLD.offering_organization_id IS DISTINCT FROM NEW.offering_organization_id
				OR OLD.platform_listing_no IS DISTINCT FROM NEW.platform_listing_no) THEN
				PERFORM dss_refresh_rental_listing_summary_report_period(
					OLD.offering_organization_id, OLD.platform_listing_no);
			END IF;

			IF OLD.including_rental_listing_report_id IS NOT NULL
			   OR OLD.business_licence_no IS DISTINCT FROM NEW.business_licence_no
			   OR OLD.effective_host_nm IS DISTINCT FROM NEW.effective_host_nm
			   OR OLD.bc_registry_no IS DISTINCT FROM NEW.bc_registry_no
			   OR OLD.locating_physical_address_id IS DISTINCT FROM NEW.locating_physical_address_id
			   OR OLD.offering_organization_id IS DISTINCT FROM NEW.offering_organization_id
			   OR OLD.platform_listing_no IS DISTINCT FROM NEW.platform_listing_no
			THEN
				PERFORM dss_upsert_rental_listing_summary(NEW.rental_listing_id);
			END IF;
		END IF;
		RETURN NEW;
	END IF;

	IF TG_OP = 'UPDATE' THEN
		IF OLD.including_rental_listing_report_id IS NULL THEN
			DELETE FROM dss_rental_listing_summary
			WHERE rental_listing_id = NEW.rental_listing_id;
		END IF;
	END IF;

	IF TG_OP = 'INSERT' THEN
		UPDATE dss_rental_listing_summary s
		SET latest_report_period_ym = GREATEST(s.latest_report_period_ym, drlr.report_period_ym),
			upd_dtm = now()
		FROM dss_rental_listing master
		JOIN dss_rental_listing_report drlr
			ON drlr.rental_listing_report_id = NEW.including_rental_listing_report_id
		WHERE master.including_rental_listing_report_id IS NULL
		  AND master.offering_organization_id = NEW.offering_organization_id
		  AND master.platform_listing_no = NEW.platform_listing_no
		  AND s.rental_listing_id = master.rental_listing_id
		  AND (s.latest_report_period_ym IS NULL OR s.latest_report_period_ym < drlr.report_period_ym);
		RETURN NEW;
	END IF;

	IF OLD.including_rental_listing_report_id IS DISTINCT FROM NEW.including_rental_listing_report_id
	   OR OLD.offering_organization_id IS DISTINCT FROM NEW.offering_organization_id
	   OR OLD.platform_listing_no IS DISTINCT FROM NEW.platform_listing_no
	THEN
		PERFORM dss_refresh_rental_listing_summary_report_period(
			NEW.offering_organization_id, NEW.platform_listing_no);
		IF OLD.including_rental_listing_report_id IS NOT NULL
		   AND (OLD.offering_organization_id IS DISTINCT FROM NEW.offering_organization_id
			OR OLD.platform_listing_no IS DISTINCT FROM NEW.platform_listing_no) THEN
			PERFORM dss_refresh_rental_listing_summary_report_period(
				OLD.offering_organization_id, OLD.platform_listing_no);
		END IF;
	END IF;

	RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION dss_rental_listing_summary_from_action()
RETURNS trigger
LANGUAGE plpgsql
SET search_path TO dss, public
AS $$
BEGIN
	IF TG_OP = 'DELETE' THEN
		PERFORM dss_rental_listing_summary_set_last_action(OLD.rental_listing_id);
		RETURN OLD;
	END IF;

	PERFORM dss_rental_listing_summary_set_last_action(NEW.rental_listing_id);
	IF TG_OP = 'UPDATE' THEN
		IF OLD.rental_listing_id IS DISTINCT FROM NEW.rental_listing_id THEN
			PERFORM dss_rental_listing_summary_set_last_action(OLD.rental_listing_id);
		END IF;
	END IF;
	RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION dss_rental_listing_summary_from_contact()
RETURNS trigger
LANGUAGE plpgsql
SET search_path TO dss, public
AS $$
BEGIN
	IF TG_OP = 'DELETE' THEN
		PERFORM dss_rental_listing_summary_set_primary_host(OLD.contacted_through_rental_listing_id);
		RETURN OLD;
	END IF;

	PERFORM dss_rental_listing_summary_set_primary_host(NEW.contacted_through_rental_listing_id);
	IF TG_OP = 'UPDATE' THEN
		IF OLD.contacted_through_rental_listing_id IS DISTINCT FROM NEW.contacted_through_rental_listing_id THEN
			PERFORM dss_rental_listing_summary_set_primary_host(OLD.contacted_through_rental_listing_id);
		END IF;
	END IF;
	RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION dss_rental_listing_summary_from_address()
RETURNS trigger
LANGUAGE plpgsql
SET search_path TO dss, public
AS $$
BEGIN
	IF TG_OP = 'UPDATE' THEN
		IF OLD.match_address_txt IS NOT DISTINCT FROM NEW.match_address_txt
		   AND OLD.unit_no IS NOT DISTINCT FROM NEW.unit_no THEN
			RETURN NEW;
		END IF;
	END IF;

	UPDATE dss_rental_listing_summary s
	SET match_address_txt = NULLIF(btrim(NEW.match_address_txt), ''),
		match_unit_no = NULLIF(btrim(NEW.unit_no), ''),
		upd_dtm = now()
	FROM dss_rental_listing drl
	WHERE drl.locating_physical_address_id = NEW.physical_address_id
	  AND drl.including_rental_listing_report_id IS NULL
	  AND s.rental_listing_id = drl.rental_listing_id;

	RETURN NEW;
END;
$$;

CREATE TRIGGER dss_rental_listing_summary_ar_iud_tr
	AFTER INSERT OR UPDATE OR DELETE ON dss_rental_listing
	FOR EACH ROW EXECUTE FUNCTION dss_rental_listing_summary_from_listing();

CREATE TRIGGER dss_rental_listing_action_summary_ar_iud_tr
	AFTER INSERT OR UPDATE OR DELETE ON dss_rental_listing_action
	FOR EACH ROW EXECUTE FUNCTION dss_rental_listing_summary_from_action();

CREATE TRIGGER dss_rental_listing_contact_summary_ar_iud_tr
	AFTER INSERT OR UPDATE OR DELETE ON dss_rental_listing_contact
	FOR EACH ROW EXECUTE FUNCTION dss_rental_listing_summary_from_contact();

CREATE TRIGGER dss_physical_address_summary_ar_iu_tr
	AFTER INSERT OR UPDATE ON dss_physical_address
	FOR EACH ROW EXECUTE FUNCTION dss_rental_listing_summary_from_address();

COMMENT ON FUNCTION dss_rental_listing_summary_rows(bigint) IS
	'Master-listing summary values. Pass a rental_listing_id to build one row, or null to build every master listing.';

COMMENT ON FUNCTION dss_refresh_rental_listing_summary() IS
	'Rebuilds dss_rental_listing_summary for every current master listing and removes rows whose listing is no longer a master.';

ALTER TABLE dss_rental_listing_summary OWNER TO CURRENT_USER;
ALTER FUNCTION dss_rental_listing_summary_rows(bigint) OWNER TO CURRENT_USER;
ALTER FUNCTION dss_upsert_rental_listing_summary(bigint) OWNER TO CURRENT_USER;
ALTER FUNCTION dss_refresh_rental_listing_summary_report_period(bigint, varchar) OWNER TO CURRENT_USER;
ALTER FUNCTION dss_refresh_rental_listing_summary() OWNER TO CURRENT_USER;
ALTER FUNCTION dss_rental_listing_summary_set_last_action(bigint) OWNER TO CURRENT_USER;
ALTER FUNCTION dss_rental_listing_summary_set_primary_host(bigint) OWNER TO CURRENT_USER;
ALTER FUNCTION dss_rental_listing_summary_from_listing() OWNER TO CURRENT_USER;
ALTER FUNCTION dss_rental_listing_summary_from_action() OWNER TO CURRENT_USER;
ALTER FUNCTION dss_rental_listing_summary_from_contact() OWNER TO CURRENT_USER;
ALTER FUNCTION dss_rental_listing_summary_from_address() OWNER TO CURRENT_USER;

SELECT dss_refresh_rental_listing_summary();
