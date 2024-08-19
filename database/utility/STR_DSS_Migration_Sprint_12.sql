/* Sprint 12 Master Database Migration Script -- must be run from psql using the following command: \i '<folder>/<script>' (must use / rather than \) */
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
select 1 from information_schema.columns where column_name='governing_business_licence_id' and table_name='dss_rental_listing' and table_schema=:'dflt_schema')
as list_has_bl_id
\gset
select exists(
select 1 from information_schema.columns where column_name='local_government_type' and table_name='dss_organization' and table_schema=:'dflt_schema')
as org_has_lg_type
\gset
\if :list_has_bl_id
	\echo 'Sprint 12 migration appears complete - Exiting without changes'
\elif :org_has_lg_type
	\echo 'Sprint 11 migration appears complete - Beginning upgrade to Sprint 12'
	\echo 'Calling STR_DSS_Incremental_DB_DDL_Sprint_12.sql'
	\ir '../ddl/STR_DSS_Incremental_DB_DDL_Sprint_12.sql'
	\echo 'Replacing views'
	\ir '../ddl/STR_DSS_Views_Sprint_9.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_12.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_12.sql'
\elif :db_has_dss_tab
	\echo 'Database migration state is unknown - Exiting without changes'
\else
	\echo 'Database has no DSS tables - Beginning complete build to Sprint 12'
	\echo 'Calling STR_DSS_Physical_DB_DDL_Sprint_12.sql'
	\ir '../ddl/STR_DSS_Physical_DB_DDL_Sprint_12.sql'
	\echo 'Calling STR_DSS_Data_Seeding_Sprint_12.sql'
	\ir '../seeding/STR_DSS_Data_Seeding_Sprint_12.sql'
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
