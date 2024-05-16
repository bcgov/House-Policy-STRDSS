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
      COUNT(*) AS total,
      SUM(CASE WHEN dul.is_processed = true THEN 1 ELSE 0 END) AS processed,
      SUM(CASE WHEN dul.is_validation_failure = true THEN 1 ELSE 0 END) AS errors,
      SUM(CASE WHEN dul.is_validation_failure = false THEN 1 ELSE 0 END) AS success,
      CASE WHEN COUNT(*) = SUM(CASE WHEN dul.is_processed = true THEN 1 ELSE 0 END) THEN ''Processed'' ELSE ''Pending'' END AS status
    FROM
      dss_upload_delivery dud
      JOIN dss_upload_line dul ON dul.including_upload_delivery_id = dud.upload_delivery_id
      JOIN dss_user_identity dui ON dud.upd_user_guid = dui.user_guid
      JOIN dss_organization do2 ON dud.providing_organization_id = do2.organization_id 
    GROUP BY
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
