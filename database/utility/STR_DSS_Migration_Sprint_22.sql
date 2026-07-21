/* Sprint 22 Master Database Migration Script
 *
 * Run as the application database user (not postgres) so new objects inherit correct ownership:
 *   psql -U strdssdev -d strdssdev -f STR_DSS_Migration_Sprint_22.sql
 *
 * From psql interactively (must use / rather than \ in paths):
 *   \i '<folder>/STR_DSS_Migration_Sprint_22.sql'
 *
 * DBeaver: connect as strdssdev, then run the individual SQL files listed in database/README.md
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
select 1 from information_schema.columns where table_name='dss_physical_address' and column_name='unit_no' and character_maximum_length=100 and table_schema=:'dflt_schema')
as db_has_s22_unit_resize
\gset
select exists(
select 1 from information_schema.columns where table_name='dss_business_licence' and column_name='mailing_province_cd' and character_maximum_length=50 and table_schema=:'dflt_schema')
as db_has_s22_mailing_province_resize
\gset
select exists(
select 1 from information_schema.columns where table_name='dss_rental_listing' and column_name='takedown_reason' and table_schema=:'dflt_schema')
as db_has_s20_chg
\gset
\if :db_has_s22_listing_actions
	\if :db_has_s22_mailing_province_resize
		\echo 'Sprint 22 migration appears complete - Exiting without changes'
	\else
		\echo 'Applying Sprint 22 mailing province column resize'
		ALTER TABLE dss_business_licence ALTER COLUMN mailing_province_cd TYPE varchar(50);
	\endif
\elif :db_has_s22_unit_resize
	\if :db_has_s22_mailing_province_resize
		\echo 'Sprint 22 unit resize appears complete - Beginning listing action upgrade'
	\else
		\echo 'Applying Sprint 22 mailing province column resize'
		ALTER TABLE dss_business_licence ALTER COLUMN mailing_province_cd TYPE varchar(50);
		\echo 'Sprint 22 unit resize appears complete - Beginning listing action upgrade'
	\endif
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_22_Listing_Actions.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_22_Listing_Actions.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_22.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_22.sql'
	\echo 'Calling Backfill_Listing_Actions_Sprint_22.sql'
	\ir '../utility/Backfill_Listing_Actions_Sprint_22.sql'
\elif :db_has_s20_chg
	\echo 'Sprint 20 migration appears complete - Beginning upgrade to Sprint 22'
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_22.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_22.sql'
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_22_Listing_Actions.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_22_Listing_Actions.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_22.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_22.sql'
	\echo 'Calling Backfill_Listing_Actions_Sprint_22.sql'
	\ir '../utility/Backfill_Listing_Actions_Sprint_22.sql'
\elif :db_has_dss_tab
	\echo 'Database migration state is unknown - Try an earlier release - Exiting without changes'
\else
	\echo 'Database has no DSS tables - Beginning complete build to Sprint 22'
	\echo 'Calling STR_DSS_Physical_DB_DDL_Sprint_22.sql'
	\ir '../ddl/STR_DSS_Physical_DB_DDL_Sprint_22.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_21.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_21.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_22.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_22.sql'
	\echo 'Calling Backfill_Listing_Actions_Sprint_22.sql'
	\ir '../utility/Backfill_Listing_Actions_Sprint_22.sql'
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
