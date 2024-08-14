/* DANGER -- DROP ALL DB OBJECTS PREFIXED BY DSS -- DANGER -- To reset database, run from psql using the following command: \i '<folder>/<script>' (must use / rather than \) */
\conninfo
\encoding UTF8
\set VERBOSITY terse
\set ECHO none
\set QUIET on
\set AUTOCOMMIT off
START TRANSACTION;
select format('DROP %s IF EXISTS %I.%I CASCADE',routine_type,routine_schema,routine_name)
from information_schema.routines where routine_name like 'dss%'
\gexec
\echo Dropped :ROW_COUNT routines
select format('DROP VIEW IF EXISTS %I.%I CASCADE',schemaname,viewname)
from pg_views where viewname like 'dss%'
\gexec
\echo Dropped :ROW_COUNT views
select format('DROP TABLE IF EXISTS %I.%I CASCADE',schemaname,tablename)
from pg_tables where tablename like 'dss%'
\gexec
\echo Dropped :ROW_COUNT tables
\if :ERROR
	/*commit cannot succeed because transaction was aborted due to an error*/
	\echo 'Undoing transaction'
	ROLLBACK;
\else
	\echo 'Committing transaction'
	COMMIT;
\endif
\echo done!
