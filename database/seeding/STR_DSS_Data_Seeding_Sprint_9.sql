/* STR DSS Sprint 9 Mandatory Data Seeding */

MERGE INTO dss_access_request_status AS tgt
USING ( SELECT * FROM (VALUES
('Requested','Requested'),
('Approved','Approved'),
('Denied','Denied'))
AS s (access_request_status_cd, access_request_status_nm)
) AS src
ON (tgt.access_request_status_cd=src.access_request_status_cd)
WHEN MATCHED and tgt.access_request_status_nm!=src.access_request_status_nm
THEN UPDATE SET
access_request_status_nm=src.access_request_status_nm
WHEN NOT matched
THEN INSERT (access_request_status_cd, access_request_status_nm)
VALUES (src.access_request_status_cd, src.access_request_status_nm);

MERGE INTO dss_email_message_type AS tgt
USING ( SELECT * FROM (VALUES
('Listing Upload Error','Listing Upload Error'),
('Notice of Takedown','Notice of Non-Compliance'),
('Takedown Request','Takedown Request Confirmation'),
('Batch Takedown Request','Batch Takedown Request'),
('Completed Takedown','Completed Takedown'),
('Escalation Request','STR Escalation Request'),
('Compliance Order','Provincial Compliance Order'),
('Access Requested','Access Requested Notification'),
('Access Granted','Access Granted Notification'),
('Access Denied','Access Denied Notification'))
AS s (email_message_type, email_message_type_nm)
) AS src
ON (tgt.email_message_type=src.email_message_type)
WHEN matched and tgt.email_message_type_nm!=src.email_message_type_nm
THEN UPDATE SET
email_message_type_nm=src.email_message_type_nm
WHEN NOT MATCHED
THEN INSERT (email_message_type, email_message_type_nm)
VALUES (src.email_message_type, src.email_message_type_nm);

MERGE INTO dss_listing_status_type AS tgt
USING ( SELECT * FROM (VALUES
(1,'N','New'),
(2,'A','Active'),
(3,'I','Inactive'),
(4,'T','Takedown Complete'),
(5,'R','Reassigned'))
AS s (listing_status_sort_no,listing_status_type, listing_status_type_nm)
) AS src
ON (tgt.listing_status_type=src.listing_status_type)
WHEN matched and (
tgt.listing_status_type_nm!=src.listing_status_type_nm or
tgt.listing_status_sort_no!=src.listing_status_sort_no)
THEN UPDATE SET
listing_status_type_nm=src.listing_status_type_nm,
listing_status_sort_no=src.listing_status_sort_no
WHEN NOT MATCHED
THEN INSERT (listing_status_type, listing_status_type_nm, listing_status_sort_no)
VALUES (src.listing_status_type, src.listing_status_type_nm, src.listing_status_sort_no);

MERGE INTO dss_organization_type AS tgt
USING ( SELECT * FROM (VALUES
('BCGov','BC Government Staff'),
('LG','Local Government'),
('LGSub','Local Government Subdivision'),
('Platform','Short-term Rental Platform'))
AS s (organization_type, organization_type_nm)
) AS src
ON (tgt.organization_type=src.organization_type)
WHEN matched and tgt.organization_type_nm!=src.organization_type_nm
THEN UPDATE SET
organization_type_nm=src.organization_type_nm
WHEN NOT MATCHED
THEN INSERT (organization_type, organization_type_nm)
VALUES (src.organization_type, src.organization_type_nm);

MERGE INTO dss_user_privilege AS tgt
USING ( SELECT * FROM (VALUES
('user_read','View users'),
('user_write','Manage/edit users'),
('role_read','View roles and permissions'),
('role_write','Manage roles and permissions'),
('listing_read','View/search/export listings and address log'),
('address_write','Edit addresses'),
('licence_file_upload','Upload business licence files'),
('listing_file_upload','Upload platform listing files'),
('takedown_file_upload','Upload platform takedown files'),
('upload_history_read','View platform upload listing/takedown history'),
('audit_read','View audit logs'),
('takedown_action','Create Takedown Action'),
('province_action','Create Provincial Compliance Order'),
('ceu_action','Send email to all hosts or platforms'))
AS s (user_privilege_cd, user_privilege_nm)
) AS src
ON (tgt.user_privilege_cd=src.user_privilege_cd)
WHEN matched and tgt.user_privilege_nm!=src.user_privilege_nm
THEN UPDATE SET
user_privilege_nm=src.user_privilege_nm
WHEN NOT MATCHED
THEN INSERT (user_privilege_cd, user_privilege_nm)
VALUES (src.user_privilege_cd, src.user_privilege_nm);

MERGE INTO dss_user_role AS tgt
USING ( SELECT * FROM (VALUES
('ceu_admin','CEU Admin'),
('ceu_staff','CEU Staff'),
('bc_staff','BC Government Staff'),
('lg_staff','Local Government'),
('platform_staff','Short-term Rental Platform'))
AS s (user_role_cd, user_role_nm)
) AS src
ON (tgt.user_role_cd=src.user_role_cd)
WHEN matched and tgt.user_role_nm!=src.user_role_nm
THEN UPDATE SET
user_role_nm=src.user_role_nm
WHEN NOT MATCHED
THEN INSERT (user_role_cd, user_role_nm)
VALUES (src.user_role_cd, src.user_role_nm);

MERGE INTO dss_user_role_privilege AS tgt
USING ( SELECT * FROM (VALUES
('ceu_admin','user_read'),
('ceu_admin','user_write'),
('ceu_admin','role_read'),
('ceu_admin','role_write'),
('ceu_admin','listing_read'),
('ceu_admin','address_write'),
('ceu_admin','licence_file_upload'),
('ceu_admin','listing_file_upload'),
('ceu_admin','takedown_file_upload'),
('ceu_admin','upload_history_read'),
('ceu_admin','audit_read'),
('ceu_admin','ceu_action'),
--
('ceu_staff','listing_read'), -- to be confirmed
('ceu_staff','address_write'), -- to be confirmed
('ceu_staff','upload_history_read'),
('ceu_staff','audit_read'),
('ceu_staff','province_action'),
('ceu_staff','ceu_action'),
--
('bc_staff','listing_read'), -- to be confirmed
('bc_staff','upload_history_read'),
--
('lg_staff','listing_read'), -- to be confirmed
('lg_staff','address_write'), -- to be confirmed
('lg_staff','licence_file_upload'),
('lg_staff','upload_history_read'),
('lg_staff','audit_read'),
('lg_staff','takedown_action'),
--
('platform_staff','upload_history_read'),
('platform_staff','takedown_file_upload'),
('platform_staff','listing_file_upload'))
AS s (user_role_cd, user_privilege_cd)
) AS src
ON (tgt.user_role_cd=src.user_role_cd AND tgt.user_privilege_cd=src.user_privilege_cd)
WHEN NOT MATCHED
THEN INSERT (user_role_cd, user_privilege_cd)
VALUES (src.user_role_cd, src.user_privilege_cd);

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
('BCGov','CEU','Provincial Compliance and Enforcement Unit'),
('BCGov','BC','BC Government Staff'))
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
