-- Check if DB exists and drop if it does
DO
$$
DECLARE
   db_name TEXT := 'strdssdev';
BEGIN
   -- Check if the database exists
   IF EXISTS (
       SELECT 1 FROM pg_database
       WHERE datname = db_name
   ) THEN
       -- Terminate active connections to the database
       PERFORM pg_terminate_backend(pid)
       FROM pg_stat_activity
       WHERE datname = db_name;
   END IF;
END;
$$;

DROP DATABASE IF EXISTS strdssdev;

-- Check if strdss role exists and drop if it does
DO
$$
BEGIN
   IF EXISTS (
       SELECT 1 FROM pg_roles
       WHERE rolname = 'strdssdev'
   ) THEN
       EXECUTE format('DROP ROLE %I', 'strdssdev');
   END IF;
END
$$;

CREATE ROLE strdssdev WITH LOGIN PASSWORD 'postgres';
CREATE DATABASE strdssdev;
ALTER DATABASE strdssdev OWNER TO strdssdev;
\c strdssdev
CREATE EXTENSION postgis;