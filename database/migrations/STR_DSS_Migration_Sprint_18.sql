/* Sprint 18 Master Database Migration Script -- must be run from psql using the following command: \i '<folder>/<script>' (must use / rather than \) */
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
select 1 from information_schema.routines where routine_definition like'%regexp_replace(regexp_replace(UPPER(%' and routine_name='dss_process_biz_lic_table_update' and routine_schema=:'dflt_schema')
as db_has_s18_chg
\gset
select exists(
select 1 from information_schema.columns where column_name='business_licence_format_txt' and table_name='dss_organization' and table_schema=:'dflt_schema')
as db_has_s17_col
\gset
\if :db_has_s18_chg
	\echo 'Sprint 18 migration appears complete - Exiting without changes'
\elif :db_has_s17_col
	\echo 'Sprint 17 migration appears complete - Beginning upgrade to Sprint 18'
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_18.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_18.sql'
	\echo 'Calling STR_DSS_Routines_Sprint_18.sql'
	\ir '../ddl/STR_DSS_Routines_Sprint_18.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_18.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_18.sql'
	\echo 'Calling Correct_Rental_Listings_Sprint_18.sql'
	\ir '../utility/Correct_Rental_Listings_Sprint_18.sql'
\elif :db_has_dss_tab
	\echo 'Database migration state is unknown - Try an earlier release - Exiting without changes'
\else
	\echo 'Database has no DSS tables - Beginning complete build to Sprint 18'
	\echo 'Calling STR_DSS_Physical_DB_DDL_Sprint_18.sql'
	\ir '../ddl/STR_DSS_Physical_DB_DDL_Sprint_18.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_18.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_18.sql'
	\echo 'Calling STR_DSS_Data_Seeding_LGs_Sprint_17.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_LGs_Sprint_17.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Geometry_Sprint_17.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Geometry_Sprint_17.sql'
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
