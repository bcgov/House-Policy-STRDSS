DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.views WHERE table_name = 'dss_rental_upload_history_view') THEN
        EXECUTE 'DROP VIEW dss_rental_upload_history_view';
    END IF;

    EXECUTE '
    CREATE VIEW dss_rental_upload_history_view AS
    SELECT
      dud.upload_delivery_id,
      dud.report_period_ym,
      dud.providing_organization_id,
      do2.organization_nm,
      dud.upd_dtm,
      dui.given_nm,
      dui.family_nm,
      count(*) as total,
      count(dul.is_processed) as processed,
      count(dul.is_validation_failure) as errors
    FROM
      dss_upload_delivery dud,
      dss_upload_line dul,
      dss_user_identity dui,
      dss_organization do2 
    WHERE
      dul.including_upload_delivery_id = dud.upload_delivery_id
      and dud.upd_user_guid = dui.upd_user_guid
      and dud.providing_organization_id = do2.organization_id 
    group by
      dud.upload_delivery_id,
      dud.report_period_ym,
      dud.providing_organization_id,
      do2.organization_nm,
      dud.upd_dtm,
      dui.given_nm,
      dui.family_nm
    ;
    ';
END $$;
