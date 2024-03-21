SET search_path TO dss, public;

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
('Notice of Takedown','Notice of Takedown'),
('Takedown Request','Takedown Request'),
('Delisting Warning','Delisting Warning'),
('Delisting Request','Delisting Request'),
('Access Granted Notification','Access Granted Notification'),
('Access Denied Notification','Access Denied Notification'))
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
('takedown_action_write','Create Takedown Action'))
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
('ceu_admin','CEU Admin'))
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
('ceu_admin','takedown_action_write'))
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
