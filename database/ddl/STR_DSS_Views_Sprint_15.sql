/* Sprint 15 View Changes to STR DSS */

drop view if exists dss_platform_vw;

CREATE OR REPLACE VIEW dss_platform_vw AS 
SELECT do2.organization_id
, do2.organization_type 
, do2.organization_cd
, do2.organization_nm
, do2.managing_organization_id
, do2.upd_dtm
, do2.upd_user_guid
, docp.organization_contact_person_id as notice_of_takedown_contact_id_1
, docp.email_address_dsc as notice_of_takedown_contact_email_1
, docp2.organization_contact_person_id as takedown_request_contact_id_1
, docp2.email_address_dsc as takedown_request_contact_email_1
, docp3.organization_contact_person_id as notice_of_takedown_contact_id_2
, docp3.email_address_dsc as notice_of_takedown_contact_email_2
, docp4.organization_contact_person_id as takedown_request_contact_id_2
, docp4.email_address_dsc as takedown_request_contact_email_2
FROM dss_organization do2
left join dss_organization_contact_person docp on docp.contacted_through_organization_id = do2.organization_id and docp.email_message_type = 'Notice of Takedown' and docp.is_primary = true
left join dss_organization_contact_person docp2 on docp2.contacted_through_organization_id = do2.organization_id and docp2.email_message_type = 'Takedown Request' and docp2.is_primary = true
left join dss_organization_contact_person docp3 on docp3.contacted_through_organization_id = do2.organization_id and docp3.email_message_type = 'Notice of Takedown' and docp.is_primary != true
left join dss_organization_contact_person docp4 on docp4.contacted_through_organization_id = do2.organization_id and docp4.email_message_type = 'Takedown Request' and docp4.is_primary != true
where do2.organization_type = 'Platform';