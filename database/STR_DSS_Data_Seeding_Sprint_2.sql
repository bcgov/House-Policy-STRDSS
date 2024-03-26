MERGE INTO dss_access_request_status AS tgt
USING ( SELECT * FROM (VALUES
('Requested','Requested'),
('Approved','Approved'),
('Denied','Denied'))
AS s (access_request_status_cd, access_request_status_nm)
) AS src
ON (tgt.access_request_status_cd=src.access_request_status_cd)
WHEN MATCHED
THEN UPDATE SET
access_request_status_nm=src.access_request_status_nm
WHEN NOT MATCHED
THEN INSERT (access_request_status_cd, access_request_status_nm)
VALUES (src.access_request_status_cd, src.access_request_status_nm);

MERGE INTO dss_email_message_type AS tgt
USING ( SELECT * FROM (VALUES
('Notice of Takedown','Notice of Takedown of Short Term Rental Platform Offer'),
('Takedown Request','Takedown Request'),
('Escalation Request','STR Escalation Request'),
('Compliance Order','Provincial Compliance Order'),
('Access Granted','Access Granted Notification'),
('Access Denied','Access Denied Notification'))
AS s (email_message_type, email_message_type_nm)
) AS src
ON (tgt.email_message_type=src.email_message_type)
WHEN MATCHED
THEN UPDATE SET
email_message_type_nm=src.email_message_type_nm
WHEN NOT MATCHED
THEN INSERT (email_message_type, email_message_type_nm)
VALUES (src.email_message_type, src.email_message_type_nm);

MERGE INTO dss_message_reason AS tgt
USING ( SELECT * FROM (VALUES
('Notice of Takedown','No Business Licence Number on Listing'),
('Notice of Takedown','Invalid Business Licence Number'),
('Notice of Takedown','Expired Business Licence'),
('Notice of Takedown','Suspended Business Licence'),
('Notice of Takedown','Revoked Business Licence'),
('Notice of Takedown','Business Licence Denied'))
AS s (email_message_type, message_reason_dsc)
) AS src
ON (tgt.email_message_type=src.email_message_type AND tgt.message_reason_dsc=src.message_reason_dsc)
WHEN NOT MATCHED
THEN INSERT (email_message_type, message_reason_dsc)
VALUES (src.email_message_type, src.message_reason_dsc);

MERGE INTO dss_organization_type AS tgt
USING ( SELECT * FROM (VALUES
('BCGov','BC Government Component'),
('LG','Local Government'),
('Platform','Short Term Rental Platform'))
AS s (organization_type, organization_type_nm)
) AS src
ON (tgt.organization_type=src.organization_type)
WHEN MATCHED
THEN UPDATE SET
organization_type_nm=src.organization_type_nm
WHEN NOT MATCHED
THEN INSERT (organization_type, organization_type_nm)
VALUES (src.organization_type, src.organization_type_nm);

MERGE INTO dss_user_privilege AS tgt
USING ( SELECT * FROM (VALUES
('user_read','View users'),
('user_write','Manage users'),
('listing_read','View listings'),
('licence_file_upload','Upload business licence files'),
('listing_file_upload','Upload platform listing files'),
('audit_read','View audit logs'),
('takedown_action','Create Takedown Action'),
('ceu_action','Create CEU Action'))
AS s (user_privilege_cd, user_privilege_nm)
) AS src
ON (tgt.user_privilege_cd=src.user_privilege_cd)
WHEN MATCHED
THEN UPDATE SET
user_privilege_nm=src.user_privilege_nm
WHEN NOT MATCHED
THEN INSERT (user_privilege_cd, user_privilege_nm)
VALUES (src.user_privilege_cd, src.user_privilege_nm);

MERGE INTO dss_user_role AS tgt
USING ( SELECT * FROM (VALUES
('ceu_admin','CEU Admin'),
('ceu_staff','CEU Staff'),
('bc_staff','Other Provincial Government'),
('lg_staff','Local Government'),
('platform_staff','Short Term Rental Platform'))
AS s (user_role_cd, user_role_nm)
) AS src
ON (tgt.user_role_cd=src.user_role_cd)
WHEN MATCHED
THEN UPDATE SET
user_role_nm=src.user_role_nm
WHEN NOT MATCHED
THEN INSERT (user_role_cd, user_role_nm)
VALUES (src.user_role_cd, src.user_role_nm);

MERGE INTO dss_user_role_privilege AS tgt
USING ( SELECT * FROM (VALUES
('ceu_admin','user_read'),
('ceu_admin','user_write'),
('ceu_admin','listing_read'),
('ceu_admin','licence_file_upload'),
('ceu_admin','listing_file_upload'),
('ceu_admin','ceu_action'),
('ceu_staff','listing_read'),
('ceu_staff','audit_read'),
('ceu_staff','ceu_action'),
('bc_staff','listing_read'),
('bc_staff','audit_read'),
('lg_staff','listing_read'),
('lg_staff','licence_file_upload'),
('lg_staff','audit_read'),
('lg_staff','takedown_action'),
('platform_staff','listing_file_upload'))
AS s (user_role_cd, user_privilege_cd)
) AS src
ON (tgt.user_role_cd=src.user_role_cd AND tgt.user_privilege_cd=src.user_privilege_cd)
WHEN NOT MATCHED
THEN INSERT (user_role_cd, user_privilege_cd)
VALUES (src.user_role_cd, src.user_privilege_cd);

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
('BCGov','CEU','Compliance Enforcement Unit'),
('BCGov','BC','Other BC Government Components'),
('LG','LGTEST','Test Town'),
('Platform','PLATFORMTEST','Test Platform'))
AS s (organization_type, organization_cd, organization_nm)
) AS src
ON (tgt.organization_cd=src.organization_cd AND tgt.organization_type=src.organization_type)
WHEN MATCHED
THEN UPDATE SET
organization_nm=src.organization_nm
WHEN NOT MATCHED
THEN INSERT (organization_type, organization_cd, organization_nm)
VALUES (src.organization_type, src.organization_cd, src.organization_nm);

INSERT INTO dss_organization_contact_person
(is_primary, given_nm, family_nm, phone_no, email_address_dsc, contacted_through_organization_id)
SELECT true, 'Test', 'Contact', '1234567890', :quoted_tester_email, organization_id
FROM dss_organization AS o
WHERE NOT EXISTS (
SELECT 1
FROM dss_organization_contact_person AS ocp
WHERE o.organization_id=ocp.contacted_through_organization_id AND is_primary=true);
