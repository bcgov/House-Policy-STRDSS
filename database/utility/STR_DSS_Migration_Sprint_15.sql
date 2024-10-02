/* Sprint 15 Master Database Migration Script -- must be run from psql using the following command: \i '<folder>/<script>' (must use / rather than \) */
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
select 1 from information_schema.columns where column_name='is_str_prohibited' and table_name='dss_organization' and table_schema=:'dflt_schema')
as db_has_s14_col
\gset
select exists(
select 1 from information_schema.columns where column_name='platform_type' and table_name='dss_organization' and table_schema=:'dflt_schema')
as db_has_s15_col
\gset
\if :db_has_s15_col
	\echo 'Sprint 15 migration appears complete - Exiting without changes'
\elif :db_has_s14_col
	\echo 'Sprint 14 migration appears complete - Beginning upgrade to Sprint 15'
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_15.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_15.sql'
	\echo 'Adding/replacing views'
	\ir '../ddl/STR_DSS_Views_Sprint_15.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_15.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_15.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Platforms_Sprint_15.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Platforms_Sprint_15.sql'
\elif :db_has_dss_tab
	\echo 'Database migration state is unknown - Exiting without changes'
\else
	\echo 'Database has no DSS tables - Beginning complete build to Sprint 15'
	\echo 'Calling STR_DSS_Physical_DB_DDL_Sprint_15.sql'
	\ir '../ddl/STR_DSS_Physical_DB_DDL_Sprint_15.sql'
	\echo 'Adding/replacing views'
	\ir '../ddl/STR_DSS_Views_Sprint_15.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_15.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_15.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Platforms_Sprint_15.sql (1st pass)'
	\ir '../seeding/STR_DSS_Data_Seeding_Platforms_Sprint_15.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Platforms_Sprint_15.sql (2nd pass)'
	\ir '../seeding/STR_DSS_Data_Seeding_Platforms_Sprint_15.sql'
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
