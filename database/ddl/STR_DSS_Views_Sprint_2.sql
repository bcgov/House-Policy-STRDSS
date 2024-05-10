DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.views WHERE table_name = 'dss_user_identity_view') THEN
        EXECUTE 'DROP VIEW dss_user_identity_view';
    END IF;

    EXECUTE '
    CREATE VIEW dss_user_identity_view AS
    SELECT
        u.user_identity_id,
        u.is_enabled,
        u.access_request_status_cd,
        u.access_request_dtm,
        u.access_request_justification_txt,
        u.identity_provider_nm,
        u.given_nm,
        u.family_nm,
        u.email_address_dsc,
        u.business_nm,
        u.terms_acceptance_dtm,
        u.represented_by_organization_id,
        o.organization_type,
        o.organization_cd,
        o.organization_nm,
        u.upd_dtm
    FROM dss_user_identity u
    LEFT JOIN dss_organization o
        ON u.represented_by_organization_id = o.organization_id;
    ';
END $$;
