/* STR DSS Sprint 4 NON-PROD Data Seeding */

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
('LG','LGTEST','Test Town'),
('LG','LGTEST2','Test City'),
('Platform','PLATFORMTEST2','Test Platform 2'),
('Platform','PLATFORMTEST','Test Platform 1'))
AS s (organization_type, organization_cd, organization_nm)
) AS src
ON (tgt.organization_cd=src.organization_cd)
WHEN matched and (
tgt.organization_nm!=src.organization_nm or
tgt.organization_type!=src.organization_type)
THEN UPDATE SET
organization_nm=src.organization_nm,
organization_type=src.organization_type
WHEN NOT MATCHED
THEN INSERT (organization_type, organization_cd, organization_nm)
VALUES (src.organization_type, src.organization_cd, src.organization_nm);

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
