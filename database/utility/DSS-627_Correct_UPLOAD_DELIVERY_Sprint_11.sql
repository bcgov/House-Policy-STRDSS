UPDATE dss_upload_delivery
set upload_delivery_type='rental_report'
where upload_delivery_type is null;
