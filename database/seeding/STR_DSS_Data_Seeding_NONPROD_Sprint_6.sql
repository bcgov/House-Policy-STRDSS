/* STR DSS Sprint 6 NON-PROD Contact Emails Data Seeding */

INSERT INTO dss_organization_contact_person
(is_primary, email_message_type, email_address_dsc, contacted_through_organization_id)
SELECT true, emt.email_message_type, :quoted_tester_email, o.organization_id
FROM dss_organization AS o, dss_email_message_type as emt
where o.organization_type='Platform'
and emt.email_message_type in ('Notice of Takedown','Takedown Request','Batch Takedown Request')
and NOT EXISTS (
SELECT 1
FROM dss_organization_contact_person AS ocp
WHERE o.organization_id=ocp.contacted_through_organization_id AND emt.email_message_type=ocp.email_message_type);
