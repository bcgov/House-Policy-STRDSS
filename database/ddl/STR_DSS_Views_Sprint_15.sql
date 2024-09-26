/* Sprint 15 View Changes to STR DSS */

drop view if exists dss_platform_vw;

CREATE OR REPLACE VIEW dss_platform_vw AS 
SELECT do2.organization_id
, do2.organization_cd
, do2.organization_nm
, do2.managing_organization_id
, do2.upd_dtm
, do2.upd_user_guid
, docp.organization_contact_person_id as notice_of_takedown_contact_id
, docp.email_address_dsc as notice_of_takedown_contact_email
, docp2.organization_contact_person_id as takedown_request_contact_id
, docp2.email_address_dsc as takedown_request_contact_email
FROM dss_organization do2
left join dss_organization_contact_person docp on docp.contacted_through_organization_id = do2.organization_id and docp.email_message_type = 'Notice of Takedown'
left join dss_organization_contact_person docp2 on docp2.contacted_through_organization_id = do2.organization_id and docp2.email_message_type = 'Takedown Request'
where do2.organization_type = 'Platform';
