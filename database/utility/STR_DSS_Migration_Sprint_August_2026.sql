/* Sprint August 2026 Master Database Migration Script
 *
 * Run as the application database user (not postgres) so new objects inherit correct ownership:
 *   psql -U strdssdev -d strdssdev -f STR_DSS_Migration_Sprint_August_2026.sql
 *
 * From psql interactively (must use / rather than \ in paths):
 *   \i '<folder>/STR_DSS_Migration_Sprint_August_2026.sql'
 *
 * Include additional \ir calls in the upgrade and full-build branches below
 * as other August 2026 database changes are added.
 */
\conninfo
\encoding UTF8
\set VERBOSITY terse
\set ECHO none
\set QUIET on
\set AUTOCOMMIT off
START TRANSACTION;
SET search_path TO dss,public; /*schema name dss ignored if it does not exist*/
SELECT current_schema() as dflt_schema
\gset
select exists(
select 1 from information_schema.tables where table_name like 'dss%')
as db_has_dss_tab
\gset
select exists(
select 1 from information_schema.tables where table_name='dss_rental_listing_action' and table_schema=:'dflt_schema')
as db_has_s22_listing_actions
\gset
select exists(
select 1 from dss_organization where organization_cd = 'LGMC_UBC')
as db_has_aug2026_lg_codes
\gset
\if :db_has_aug2026_lg_codes
	\echo 'Sprint August 2026 migration appears complete - Exiting without changes'
\elif :db_has_s22_listing_actions
	\echo 'Sprint 22 migration appears complete - Beginning upgrade to Sprint August 2026'
	\echo 'Calling DSS-1392.sql'
	\ir '../utility/DSS-1392.sql'
	\echo 'Calling STR_DSS_Data_Seeding_LGs_Sprint_August_2026.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_LGs_Sprint_August_2026.sql'
\elif :db_has_dss_tab
	\echo 'Database migration state is unknown - Try Sprint 22 first - Exiting without changes'
\else
	\echo 'Database has no DSS tables - Beginning complete build to Sprint August 2026'
	\echo 'Calling STR_DSS_Physical_DB_DDL_Sprint_22.sql'
	\ir '../ddl/STR_DSS_Physical_DB_DDL_Sprint_22.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_21.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_21.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_22.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_22.sql'
	\echo 'Calling Backfill_Listing_Actions_Sprint_22.sql'
	\ir '../utility/Backfill_Listing_Actions_Sprint_22.sql'
	\echo 'Calling DSS-1392.sql'
	\ir '../utility/DSS-1392.sql'
	\echo 'Calling STR_DSS_Data_Seeding_LGs_Sprint_August_2026.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_LGs_Sprint_August_2026.sql'
	\echo 'Users, platforms, and their contacts must be added in the application'
\endif
\if :ERROR
	/*commit cannot succeed because transaction was aborted due to an error*/
	\echo 'Undoing transaction'
	ROLLBACK;
\else
	\echo 'Committing transaction'
	COMMIT;
\endif
\echo done!
