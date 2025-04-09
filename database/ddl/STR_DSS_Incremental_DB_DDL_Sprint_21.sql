-- Create new columns for holiding rental property info required for validating registration numbers
ALTER TABLE dss_physical_address ADD reg_rental_unit_no varchar(50);
ALTER TABLE dss_physical_address ADD reg_rental_street_no varchar(50);
ALTER TABLE dss_physical_address ADD reg_rental_postal_code varchar(7);

COMMENT ON COLUMN dss_physical_address.reg_rental_unit_no IS 'The rental property unit number used to validate the bc registry number ';
COMMENT ON COLUMN dss_physical_address.reg_rental_street_no IS 'The rental property street number used to validate the bc registry number ';
COMMENT ON COLUMN dss_physical_address.reg_rental_postal_code IS 'The rental property postal code number used to validate the bc registry number ';

-- Create new columns for holding upload status information for delivery files
ALTER TABLE dss_upload_delivery ADD upload_status varchar(20);
ALTER TABLE dss_upload_delivery ADD upload_lines_total smallint;
ALTER TABLE dss_upload_delivery ADD upload_lines_success smallint;
ALTER TABLE dss_upload_delivery ADD upload_lines_error smallint;
ALTER TABLE dss_upload_delivery ADD upload_lines_processed smallint;

COMMENT ON COLUMN dss_upload_delivery.upload_status IS 'The current processing status of the uploaded file: Pending, Processed, or Failed';
COMMENT ON COLUMN dss_upload_delivery.upload_lines_total IS 'The total number of lines in the uploaded file';
COMMENT ON COLUMN dss_upload_delivery.upload_lines_success IS 'The number of lines int the uploaded file that successfully processed'; 
COMMENT ON COLUMN dss_upload_delivery.upload_lines_error IS 'The number of lines in the uploaded file that failed to process';
COMMENT ON COLUMN dss_upload_delivery.upload_lines_processed IS 'The number of lines in the uploaded file that were processed';

-- Populate the upload_lines_total column
UPDATE dss_upload_delivery dud
SET upload_lines_total = subquery.total
FROM (
    SELECT 
        dud.upload_delivery_id,
        COUNT(*) AS total
    FROM dss_upload_delivery dud
    JOIN dss_upload_line dul ON dul.including_upload_delivery_id = dud.upload_delivery_id
    GROUP BY dud.upload_delivery_id
) AS subquery
WHERE dud.upload_delivery_id = subquery.upload_delivery_id;

-- Populate upload_lines_success column
UPDATE dss_upload_delivery dud
SET upload_lines_success = subquery.success
FROM (
    SELECT 
        dud.upload_delivery_id,
        SUM(
            CASE
                WHEN ((dul.is_processed = true) AND (dul.is_validation_failure = false) AND (dul.is_system_failure = false)) THEN 1
                ELSE 0
            END
        ) AS success
    FROM dss_upload_delivery dud
    JOIN dss_upload_line dul ON dul.including_upload_delivery_id = dud.upload_delivery_id
    GROUP BY dud.upload_delivery_id
) AS subquery
WHERE dud.upload_delivery_id = subquery.upload_delivery_id;

-- Populate the upload_lines_error column
UPDATE dss_upload_delivery dud
SET upload_lines_error = subquery.errors
FROM (
    SELECT 
        dud.upload_delivery_id,
        SUM(
            CASE
                WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
                ELSE 0
            END
        ) AS errors
    FROM dss_upload_delivery dud
    JOIN dss_upload_line dul ON dul.including_upload_delivery_id = dud.upload_delivery_id
    GROUP BY dud.upload_delivery_id
) AS subquery
WHERE dud.upload_delivery_id = subquery.upload_delivery_id;

-- Populate the upload_lines_processed column
UPDATE dss_upload_delivery dud
SET upload_lines_processed = subquery.processed
FROM (
    SELECT 
        dud.upload_delivery_id,
        SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END
        ) AS processed
    FROM dss_upload_delivery dud
    JOIN dss_upload_line dul ON dul.including_upload_delivery_id = dud.upload_delivery_id
    GROUP BY dud.upload_delivery_id
) AS subquery
WHERE dud.upload_delivery_id = subquery.upload_delivery_id;

-- Populate the upload_status column
UPDATE dss_upload_delivery dud
SET upload_status = subquery.status
FROM (
    SELECT 
        dud.upload_delivery_id,
        CASE
            WHEN ((dud.upload_delivery_type)::text = 'Takedown Data'::text) THEN 'Processed'::text
            WHEN (((dud.upload_delivery_type)::text = 'Licence Data'::text) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (SUM(
            CASE
                WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
                ELSE 0
            END) = 0)) THEN 'Processed'::text
            WHEN (((dud.upload_delivery_type)::text = 'Licence Data'::text) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (SUM(
            CASE
                WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
                ELSE 0
            END) > 0)) THEN 'Failed'::text
            WHEN (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) THEN 'Processed'::text
            WHEN (((dud.upload_delivery_type)::text = 'Registration Data'::text) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (SUM(
            CASE
                WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
                ELSE 0
            END) = 0)) THEN 'Processed'::text
            WHEN (((dud.upload_delivery_type)::text = 'Registration Data'::text) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) AND (SUM(
            CASE
                WHEN ((dul.is_validation_failure = true) OR (dul.is_system_failure = true)) THEN 1
                ELSE 0
            END) > 0)) THEN 'Failed'::text
            WHEN (COUNT(*) = SUM(
            CASE
                WHEN (dul.is_processed = true) THEN 1
                ELSE 0
            END)) THEN 'Processed'::text            
            ELSE 'Pending'::text
        END AS status
    FROM dss_upload_delivery dud
    JOIN dss_upload_line dul ON dul.including_upload_delivery_id = dud.upload_delivery_id
    GROUP BY dud.upload_delivery_id, dud.upload_delivery_type
) AS subquery
WHERE dud.upload_delivery_id = subquery.upload_delivery_id;

-- Now that we've populated the columns, we can set the not null constraint on status and total lines
ALTER TABLE dss_upload_delivery ALTER COLUMN upload_status SET NOT NULL;
ALTER TABLE dss_upload_delivery ALTER COLUMN upload_lines_total SET NOT NULL;

-- Add the new permission for registration validation
INSERT INTO dss_user_privilege (user_privilege_cd, user_privilege_nm)
VALUES ('validate_reg_no','Validate Registry Numbers');

-- We need to add the status calc into the view for the reg data until we start using the other table.
-- public.dss_rental_upload_history_view source
DROP VIEW IF EXISTS public.dss_rental_upload_history_view;

CREATE OR REPLACE VIEW public.dss_rental_upload_history_view
AS SELECT dud.upload_delivery_id,
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
            WHEN dul.is_processed = true THEN 1
            ELSE 0
        END) AS processed,
    sum(
        CASE
            WHEN dul.is_validation_failure = true OR dul.is_system_failure = true THEN 1
            ELSE 0
        END) AS errors,
    sum(
        CASE
            WHEN dul.is_processed = true AND dul.is_validation_failure = false AND dul.is_system_failure = false THEN 1
            ELSE 0
        END) AS success,
        CASE
            WHEN dud.upload_delivery_type::text = 'Takedown Data'::text THEN 'Processed'::text
            WHEN dud.upload_delivery_type::text = 'Licence Data'::text AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND sum(
            CASE
                WHEN dul.is_validation_failure = true OR dul.is_system_failure = true THEN 1
                ELSE 0
            END) = 0 THEN 'Processed'::text
            WHEN dud.upload_delivery_type::text = 'Licence Data'::text AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND sum(
            CASE
                WHEN dul.is_validation_failure = true OR dul.is_system_failure = true THEN 1
                ELSE 0
            END) > 0 THEN 'Failed'::text
            WHEN count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) THEN 'Processed'::text
            WHEN dud.upload_delivery_type::text = 'Registration Data'::text AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND sum(
            CASE
                WHEN dul.is_validation_failure = true OR dul.is_system_failure = true THEN 1
                ELSE 0
            END) = 0 THEN 'Processed'::text
            WHEN dud.upload_delivery_type::text = 'Registration Data'::text AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) AND sum(
            CASE
                WHEN dul.is_validation_failure = true OR dul.is_system_failure = true THEN 1
                ELSE 0
            END) > 0 THEN 'Failed'::text
            WHEN count(*) = sum(
            CASE
                WHEN dul.is_processed = true THEN 1
                ELSE 0
            END) THEN 'Processed'::text
            ELSE 'Pending'::text            
        END AS status
   FROM dss_upload_delivery dud
     JOIN dss_upload_line dul ON dul.including_upload_delivery_id = dud.upload_delivery_id
     JOIN dss_user_identity dui ON dud.upd_user_guid = dui.user_guid
     JOIN dss_organization do2 ON dud.providing_organization_id = do2.organization_id
  GROUP BY dud.upload_delivery_id, dud.upload_delivery_type, dud.report_period_ym, dud.providing_organization_id, do2.organization_nm, dud.upd_dtm, dui.given_nm, dui.family_nm;

MERGE INTO dss_email_message_type AS tgt
USING ( SELECT * FROM (VALUES
('Listing Upload Error','Listing Upload Error'),
('Notice of Takedown','Notice of Non-Compliance'),
('Takedown Request','Takedown Request'),
('Batch Takedown Request','Batch Takedown Request'),
('Completed Takedown','Takedown Reported'),
('Escalation Request','STR Escalation Request'),
('Compliance Order','Provincial Compliance Order'),
('Access Requested','Access Requested Notification'),
('Access Granted','Access Granted Notification'),
('Access Denied','Access Denied Notification'),
('Registration Validation', 'Registration Validation'))
AS s (email_message_type, email_message_type_nm)
) AS src
ON (tgt.email_message_type=src.email_message_type)
WHEN matched and tgt.email_message_type_nm!=src.email_message_type_nm
THEN UPDATE SET
email_message_type_nm=src.email_message_type_nm
WHEN NOT MATCHED
THEN INSERT (email_message_type, email_message_type_nm)
VALUES (src.email_message_type, src.email_message_type_nm);