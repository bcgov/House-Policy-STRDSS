DO
$$
DECLARE
    v_username TEXT := '<username>'; 
    v_password TEXT;
    v_database TEXT := '<dbname>';
    v_schema   TEXT := 'public';
BEGIN

	SET client_min_messages = 'notice';
    
-- Ensure the pgcrypto extension is enabled to use gen_random_bytes()
    PERFORM * FROM pg_extension WHERE extname = 'pgcrypto';
    IF NOT FOUND THEN
        CREATE EXTENSION pgcrypto;
    END IF;

    -- Generate a random 12-character password
    v_password := encode(gen_random_bytes(10), 'base64');

    -- Create user with generated password
    EXECUTE format('CREATE USER %I WITH PASSWORD %L;', v_username, v_password);

    -- Revoke all default privileges on the database
    EXECUTE format('REVOKE ALL ON DATABASE %I FROM %I;', v_database, v_username);

    -- Grant CONNECT on the database
    EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I;', v_database, v_username);

    -- Grant USAGE on the schema
    EXECUTE format('GRANT USAGE ON SCHEMA %I TO %I;', v_schema, v_username);

    -- Grant SELECT on all tables in the schema
    EXECUTE format('GRANT SELECT ON ALL TABLES IN SCHEMA %I TO %I;', v_schema, v_username);

    -- Grant SELECT on all sequences in the schema
    EXECUTE format('GRANT SELECT ON ALL SEQUENCES IN SCHEMA %I TO %I;', v_schema, v_username);

    -- Alter default privileges for future tables
    EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT SELECT ON TABLES TO %I;', v_schema, v_username);

    -- Alter default privileges for future sequences
    EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT SELECT ON SEQUENCES TO %I;', v_schema, v_username);

    -- Print the generated password
    RAISE NOTICE 'Generated password for user % is: %', v_username, v_password;

END
$$;
