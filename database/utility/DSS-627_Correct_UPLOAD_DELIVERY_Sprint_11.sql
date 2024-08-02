alter table dss_upload_delivery
disable trigger all;

UPDATE dss_upload_delivery
set upload_delivery_type='Listing Data'
where upload_delivery_type = 'rental_report';

alter table dss_upload_delivery
enable trigger all;
