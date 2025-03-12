ALTER TABLE dss_physical_address ADD reg_rental_unit_no varchar(50);
ALTER TABLE dss_physical_address ADD reg_rental_street_no varchar(50);
ALTER TABLE dss_physical_address ADD reg_rental_postal_code varchar(7);

COMMENT ON COLUMN dss_physical_address.reg_rental_unit_no IS 'The rental property unit number used to validate the bc registry number ';
COMMENT ON COLUMN dss_physical_address.reg_rental_unit_no IS 'The rental property street number used to validate the bc registry number ';
COMMENT ON COLUMN dss_physical_address.reg_rental_unit_no IS 'The rental property postal code number used to validate the bc registry number ';

ALTER TABLE dss_upload_delivery ADD upload_status type varchar(20);
ALTER TABLE dss_upload_delivery ADD upload_lines_total type smallint;
ALTER TABLE dss_upload_delivery ADD upload_lines_success type smallint;
ALTER TABLE dss_upload_delivery ADD upload_lines_error type smallint;
ALTER TABLE dss_upload_delivery ADD upload_lines_processed type smallint;

COMMENT ON COLUMN dss_upload_delivery.upload_status IS 'The current processing status of the uploaded file: Pending, Processed, or Failed';
COMMENT ON COLUMN dss_upload_delivery.upload_lines_total IS 'The total number of lines in the uploaded file';
COMMENT ON COLUMN dss_upload_delivery.upload_lines_success IS 'The number of lines int the uploaded file that successfully processed'; 
COMMENT ON COLUMN dss_upload_delivery.upload_lines_error IS 'The number of lines in the uploaded file that failed to process';
COMMENT ON COLUMN dss_upload_delivery.upload_lines_processed IS 'The number of lines in the uploaded file that were processed';