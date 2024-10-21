/* Sprint 16 View Changes to STR DSS */

drop view if exists dss_platform_vw;

CREATE OR REPLACE VIEW dss_platform_vw AS 
SELECT do2.organization_id
, do2.organization_type
, do2.organization_cd
, do2.organization_nm
, do2.managing_organization_id
, do2.is_active
, do2.platform_type
, dpt.platform_type_nm
, do2.upd_dtm
, do2.upd_user_guid
, dui.display_nm as upd_user_display_nm
, dui.display_nm as upd_user_given_nm
, dui.display_nm as upd_user_family_nm
, docp.organization_contact_person_id as primary_notice_of_takedown_contact_id
, docp.email_address_dsc as primary_notice_of_takedown_contact_email
, docp2.organization_contact_person_id as primary_takedown_request_contact_id
, docp2.email_address_dsc as primary_takedown_request_contact_email
, docp3.organization_contact_person_id as secondary_notice_of_takedown_contact_id
, docp3.email_address_dsc as secondary_notice_of_takedown_contact_email
, docp4.organization_contact_person_id as secondary_takedown_request_contact_id
, docp4.email_address_dsc as secondary_takedown_request_contact_email
FROM dss_organization do2
left join dss_platform_type dpt on dpt.platform_type = do2.platform_type
left join dss_user_identity dui on dui.user_guid = do2.upd_user_guid
left join dss_organization_contact_person docp on docp.contacted_through_organization_id = do2.organization_id and docp.email_message_type = 'Notice of Takedown' and docp.is_primary = true
left join dss_organization_contact_person docp2 on docp2.contacted_through_organization_id = do2.organization_id and docp2.email_message_type = 'Takedown Request' and docp2.is_primary = true
left join dss_organization_contact_person docp3 on docp3.contacted_through_organization_id = do2.organization_id and docp3.email_message_type = 'Notice of Takedown' and docp3.is_primary != true
left join dss_organization_contact_person docp4 on docp4.contacted_through_organization_id = do2.organization_id and docp4.email_message_type = 'Takedown Request' and docp4.is_primary != true
where do2.organization_type = 'Platform';