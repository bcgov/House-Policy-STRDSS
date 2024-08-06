alter table dss_upload_delivery
disable trigger dss_upload_delivery_br_iu_tr;

UPDATE dss_upload_delivery
set upload_delivery_type='Listing Data'
where upload_delivery_type = 'rental_report';

alter table dss_upload_delivery
enable trigger dss_upload_delivery_br_iu_tr;
