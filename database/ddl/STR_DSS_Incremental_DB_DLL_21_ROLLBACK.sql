-- Rollback script for STR_DSS_Incremental_DB_DDL_Sprint_21.sql

-- Remove columns added to dss_physical_address
ALTER TABLE dss_physical_address DROP COLUMN IF EXISTS reg_rental_unit_no;
ALTER TABLE dss_physical_address DROP COLUMN IF EXISTS reg_rental_street_no;
ALTER TABLE dss_physical_address DROP COLUMN IF EXISTS reg_rental_postal_code;

-- Remove columns added to dss_upload_delivery
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS upload_status;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS upload_lines_total;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS upload_lines_success;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS upload_lines_error;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS upload_lines_processed;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS registration_status;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS registration_lines_failure;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS registration_lines_success;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS upload_user_guid;
ALTER TABLE dss_upload_delivery DROP COLUMN IF EXISTS upload_date;

-- Remove columns added to dss_upload_line
ALTER TABLE dss_upload_line DROP COLUMN IF EXISTS is_registration_failure;
ALTER TABLE dss_upload_line DROP COLUMN IF EXISTS registration_text;

-- Drop the updated view dss_rental_upload_history_view
DROP VIEW IF EXISTS dss_rental_upload_history_view;

CREATE OR REPLACE VIEW dss_rental_upload_history_view AS SELECT dud.upload_delivery_id,
    dud.upload_delivery_type,
    dud.report_period_ym,
    dud.providing_organization_id,
    do2.organization_nm,
    dud.upd_dtm,
    dui.given_nm,
    dui.family_nm,
    count(*) AS total,
    sum(
        CASE
            WHEN (dul.is_processed = true) THEN 1
            ELSE 0
        END) AS processed,
    sum(
        CASE
            WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
            ELSE 0
        END) AS errors,
    sum(
        CASE
            WHEN ((dul.is_processed = true) AND (dul.is_validation_failure = false) AND (dul.is_system_failure = false)) THEN 1
            ELSE 0
        END) AS success,
        CASE
            WHEN ((dud.upload_delivery_type)::text = 'Takedown Data'::text) THEN 'Processed'::text
            WHEN (((dud.upload_delivery_type)::text = 'Licence Data'::text) AND (count(*) = sum(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (count(*) = sum(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (sum(
            CASE
                WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
                ELSE 0
            END) = 0)) THEN 'Processed'::text
            WHEN (((dud.upload_delivery_type)::text = 'Licence Data'::text) AND (count(*) = sum(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (count(*) = sum(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (sum(
            CASE
                WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
                ELSE 0
            END) > 0)) THEN 'Failed'::text
            WHEN (count(*) = sum(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) THEN 'Processed'::text
            ELSE 'Pending'::text
        END AS status
   FROM (((dss_upload_delivery dud
     JOIN dss_upload_line dul ON ((dul.including_upload_delivery_id = dud.upload_delivery_id)))
     JOIN dss_user_identity dui ON ((dud.upd_user_guid = dui.user_guid)))
     JOIN dss_organization do2 ON ((dud.providing_organization_id = do2.organization_id)))
  GROUP BY dud.upload_delivery_id, dud.upload_delivery_type, dud.report_period_ym, dud.providing_organization_id, do2.organization_nm, dud.upd_dtm, dui.given_nm, dui.family_nm;


-- Rollback complete