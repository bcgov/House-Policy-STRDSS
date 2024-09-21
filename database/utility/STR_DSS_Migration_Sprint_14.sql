/* SORRY -- UNFINISHED!! Sprint 14 Master Database Migration Script -- must be run from psql using the following command: \i '<folder>/<script>' (must use / rather than \) */
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
select 1 from information_schema.routines where routine_name='dss_process_biz_lic_table' and routine_schema=:'dflt_schema')
as db_has_old_proc
\gset
select exists(
select 1 from information_schema.routines where routine_name='dss_process_biz_lic_table_update' and routine_schema=:'dflt_schema')
as db_has_new_proc
\gset
\if :db_has_new_proc
	\echo 'Sprint 13 migration appears complete - Exiting without changes'
\elif :db_has_old_proc
	\echo 'Sprint 12 migration appears complete - Beginning upgrade to Sprint 13'
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_13.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_13.sql'
	\echo 'Replacing views'
	\ir '../ddl/STR_DSS_Views_Sprint_13.sql'
	\echo 'Creating routines'
	\ir '../ddl/STR_DSS_Routines_Sprint_13.sql'
	\echo 'Calling Correct_Rental_Listings_Sprint_13.sql'
	\ir '../utility/Correct_Rental_Listings_Sprint_13.sql'
\elif :db_has_dss_tab
	\echo 'Database migration state is unknown - Exiting without changes'
\else
	\echo 'Database has no DSS tables - Beginning complete build to Sprint 13'
	\echo 'Calling STR_DSS_Physical_DB_DDL_Sprint_13.sql'
	\ir '../ddl/STR_DSS_Physical_DB_DDL_Sprint_13.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_12.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_12.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Platforms_Sprint_10.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Platforms_Sprint_10.sql'
	\echo 'Platform contacts must be set manually'
	\echo 'Calling STR_DSS_Data_Seeding_LGs_Sprint_12.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_LGs_Sprint_12.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Geometry_Sprint_12.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Geometry_Sprint_12.sql'
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
