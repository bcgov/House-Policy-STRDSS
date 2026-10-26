/* STR DSS - Latest rental listing report period (incremental DDL)
 *
 * Apply on a database that already has the Sprint 22 listing-action DDL.
 * Run as the Hangfire database role for that environment
 * (strdssdev, strdsstest, strdssuat, or strdssprod) so CURRENT_USER owns the view.
 * Only the owner can REFRESH or VACUUM it.
 *
 * Leave psql autocommit on (the default). REFRESH and VACUUM cannot run inside
 * a transaction block or a function, and this script must not be included in
 * STR_DSS_Migration_Sprint_22.sql, which starts a transaction.
 *
 *   psql -U strdssdev -d strdssdev -f STR_DSS_Incremental_DB_DDL_Sprint_22_Latest_Report_MV.sql
 *
 * Unqualified names resolve through search_path. Do not schema-qualify them.
 */

SET search_path TO dss, public;

CREATE MATERIALIZED VIEW dss_rental_listing_latest_report_mv AS
SELECT
    drl.offering_organization_id,
    drl.platform_listing_no,
    max(drlr.report_period_ym) AS latest_report_period_ym
FROM dss_rental_listing drl
JOIN dss_rental_listing_report drlr
    ON drlr.rental_listing_report_id = drl.including_rental_listing_report_id
WHERE drl.including_rental_listing_report_id IS NOT NULL
  AND drl.offering_organization_id IS NOT NULL
  AND drl.platform_listing_no IS NOT NULL
GROUP BY
    drl.offering_organization_id,
    drl.platform_listing_no
WITH NO DATA;

CREATE UNIQUE INDEX dss_rental_listing_latest_report_mv_u1
    ON dss_rental_listing_latest_report_mv (offering_organization_id, platform_listing_no);

CREATE INDEX dss_rental_listing_latest_report_mv_i1
    ON dss_rental_listing_latest_report_mv (latest_report_period_ym, offering_organization_id, platform_listing_no);

COMMENT ON MATERIALIZED VIEW dss_rental_listing_latest_report_mv IS
    'Latest reported period for each organization and platform listing number. Maintained by a Hangfire job after a listing report upload is fully processed.';

COMMENT ON COLUMN dss_rental_listing_latest_report_mv.offering_organization_id IS
    'Platform organization that offered the listing';

COMMENT ON COLUMN dss_rental_listing_latest_report_mv.platform_listing_no IS
    'Platform issued identification number for the listing';

COMMENT ON COLUMN dss_rental_listing_latest_report_mv.latest_report_period_ym IS
    'Latest report_period_ym among reported snapshots of this listing';

REFRESH MATERIALIZED VIEW dss_rental_listing_latest_report_mv;

VACUUM ANALYZE dss_rental_listing_latest_report_mv;

ALTER MATERIALIZED VIEW dss_rental_listing_latest_report_mv
    OWNER TO CURRENT_USER;
