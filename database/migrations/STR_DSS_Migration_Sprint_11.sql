/* Sprint 11 Master Database Migration Script -- must be run from psql using the following command: \i '<folder>/<script>' (must use / rather than \) */
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
select 1 from information_schema.columns where column_name='economic_region_dsc' and table_name='dss_organization' and table_schema=:'dflt_schema')
as org_has_ec_reg
\gset
select exists(
select 1 from information_schema.columns where column_name='local_government_type' and table_name='dss_organization' and table_schema=:'dflt_schema')
as org_has_lg_type
\gset
\if :org_has_lg_type
	\echo 'Sprint 11 migration appears complete - Exiting without changes'
\elif :org_has_ec_reg
	\echo 'Release 4 / Sprint 9 migration appears complete - Beginning upgrade'
	\echo 'Calling STR_DSS_Data_Seeding_Platforms_Sprint_10.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Platforms_Sprint_10.sql'
	\echo 'New Platforms contacts must be set manually'
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_11.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_11.sql'
	\echo 'Replacing views'
	\ir '../ddl/STR_DSS_Views_Sprint_9.sql'
	\ir '../ddl/STR_DSS_Views_Sprint_11.sql'
	\echo 'Calling STR_DSS_Data_Seeding_LGs_Sprint_11.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_LGs_Sprint_11.sql'
	\echo 'Calling DSS-627_Correct_UPLOAD_DELIVERY_Sprint_11.sql'
	\ir '../utility/DSS-627_Correct_UPLOAD_DELIVERY_Sprint_11.sql'
\elif :db_has_dss_tab
	\echo 'Database migration state is unknown - Exiting without changes'
\else
	\echo 'Database has no DSS tables - Beginning complete build to Sprint 11'
	\echo 'Calling STR_DSS_Physical_DB_DDL_Sprint_11.sql'
	\ir '../ddl/STR_DSS_Physical_DB_DDL_Sprint_11.sql'
	\echo 'Replacing views'
	\ir '../ddl/STR_DSS_Views_Sprint_11.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_9.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_9.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Platforms_Sprint_10.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Platforms_Sprint_10.sql'
	\echo 'Platform contacts must be set manually'
	\echo 'Calling STR_DSS_Data_Seeding_LGs_Sprint_11.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_LGs_Sprint_11.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Geometry_Sprint_9.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Geometry_Sprint_9.sql'
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
