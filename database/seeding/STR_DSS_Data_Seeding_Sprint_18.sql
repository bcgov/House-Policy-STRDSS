/* STR DSS Sprint 17 Mandatory Data Seeding */

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

MERGE INTO dss_business_licence_status_type AS tgt
USING ( SELECT * FROM (VALUES
(1,'PENDING','Pending'),
(2,'DENIED','Denied'),
(2,'ISSUED','Issued'),
(2,'SUSPENDED','Suspended'),
(2,'REVOKED','Revoked'),
(2,'CANCELLED','Cancelled'),
(3,'EXPIRED','Expired'))
AS s (licence_status_sort_no,licence_status_type, licence_status_type_nm)
) AS src
ON (tgt.licence_status_type=src.licence_status_type)
WHEN matched and (
tgt.licence_status_type_nm!=src.licence_status_type_nm or
tgt.licence_status_sort_no!=src.licence_status_sort_no)
THEN UPDATE SET
licence_status_type_nm=src.licence_status_type_nm,
licence_status_sort_no=src.licence_status_sort_no
WHEN NOT MATCHED
THEN INSERT (licence_status_type, licence_status_type_nm, licence_status_sort_no)
VALUES (src.licence_status_type, src.licence_status_type_nm, src.licence_status_sort_no);

MERGE INTO dss_economic_region AS tgt
USING ( SELECT * FROM (VALUES
(1,'Cariboo','Cariboo'),
(2,'Kootenay','Kootenay'),
(3,'Lower Mainland Southwest','Lower Mainland Southwest'),
(4,'Nechako','Nechako'),
(5,'North Coast','North Coast'),
(6,'Northeast','Northeast'),
(7,'Thompson Okanagan','Thompson Okanagan'),
(8,'Vancouver Island and Coast','Vancouver Island and Coast'))
AS s (economic_region_sort_no,economic_region_dsc, economic_region_nm)
) AS src
ON (tgt.economic_region_dsc=src.economic_region_dsc)
WHEN matched and (
tgt.economic_region_nm!=src.economic_region_nm or
coalesce(tgt.economic_region_sort_no,-1)!=coalesce(src.economic_region_sort_no,-1))
THEN UPDATE SET
economic_region_nm=src.economic_region_nm,
economic_region_sort_no=src.economic_region_sort_no
WHEN NOT MATCHED
THEN INSERT (economic_region_dsc, economic_region_nm, economic_region_sort_no)
VALUES (src.economic_region_dsc, src.economic_region_nm, src.economic_region_sort_no);

MERGE INTO dss_email_message_type AS tgt
USING ( SELECT * FROM (VALUES
('Listing Upload Error','Listing Upload Error'),
('Notice of Takedown','Notice of Non-Compliance'),
('Takedown Request','Takedown Request'),
('Batch Takedown Request','Batch Takedown Request'),
('Completed Takedown','Takedown Reported'),
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
(3,'I','Inactive'))
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

MERGE INTO dss_local_government_type AS tgt
USING ( SELECT * FROM (VALUES
(5,'First Nations Community','First Nations Community'),
(3,'Mountain Resort Area','Mountain Resort Area'),
(1,'Municipality','Municipality'),
(4,'Other','Other'),
(2,'Regional District','Regional District'))
AS s (local_government_type_sort_no, local_government_type, local_government_type_nm)
) AS src
ON (tgt.local_government_type=src.local_government_type)
WHEN matched and (
tgt.local_government_type_nm!=src.local_government_type_nm or
coalesce(tgt.local_government_type_sort_no,-1)!=coalesce(src.local_government_type_sort_no,-1))
THEN UPDATE SET
local_government_type_nm=src.local_government_type_nm,
local_government_type_sort_no=src.local_government_type_sort_no
WHEN NOT MATCHED
THEN INSERT (local_government_type, local_government_type_nm, local_government_type_sort_no)
VALUES (src.local_government_type, src.local_government_type_nm, src.local_government_type_sort_no);

MERGE INTO dss_organization_type AS tgt
USING ( SELECT * FROM (VALUES
('APS','API Program Service Consumer'),
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

MERGE INTO dss_platform_type AS tgt
USING ( SELECT * FROM (VALUES
('Major','Major (More than 1,000 Listings)'),
('Minor','Minor (Less than 1,000 Listings)'))
AS s (platform_type, platform_type_nm)
) AS src
ON (tgt.platform_type=src.platform_type)
WHEN matched and tgt.platform_type_nm!=src.platform_type_nm
THEN UPDATE SET
platform_type_nm=src.platform_type_nm
WHEN NOT MATCHED
THEN INSERT (platform_type, platform_type_nm)
VALUES (src.platform_type, src.platform_type_nm);

MERGE INTO dss_user_privilege AS tgt
USING ( SELECT * FROM (VALUES
('address_write','Edit addresses'),
('audit_read','View audit logs'),
('bl_link_write','Link/Unlink Business Licence'),
('ceu_action','Send email to all hosts or platforms'),
('jurisdiction_read','View Local Governments/Juridictions'),
('jurisdiction_write','Edit Local Governments/Juridictions'),
('licence_file_upload','Upload business licence files'),
('listing_file_upload','Upload platform listing files'),
('listing_read','View/search/export listings and address log'),
('platform_read','View Manage Platforms'),
('platform_write','Edit Platforms'),
('province_action','Create Provincial Compliance Order'),
('registry_view','View Registry Information'),
('role_read','View roles and permissions'),
('role_write','Manage roles and permissions'),
('takedown_action','Create Takedown Action'),
('takedown_file_upload','Upload platform takedown files'),
('upload_history_read','View platform upload listing/takedown history'),
('user_read','View users'),
('user_write','Manage/edit users'))
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
('bc_staff','listing_read'),
('bc_staff','registry_view'),
('bc_staff','upload_history_read'),
--
('ceu_admin','address_write'),
('ceu_admin','audit_read'),
('ceu_admin','ceu_action'),
('ceu_admin','jurisdiction_read'),
('ceu_admin','jurisdiction_write'),
('ceu_admin','licence_file_upload'),
('ceu_admin','listing_file_upload'),
('ceu_admin','listing_read'),
('ceu_admin','platform_read'),
('ceu_admin','platform_write'),
('ceu_admin','registry_view'),
('ceu_admin','role_read'),
('ceu_admin','role_write'),
('ceu_admin','takedown_file_upload'),
('ceu_admin','upload_history_read'),
('ceu_admin','user_read'),
('ceu_admin','user_write'),
--
('ceu_staff','address_write'),
('ceu_staff','audit_read'),
('ceu_staff','ceu_action'),
('ceu_staff','jurisdiction_read'),
('ceu_staff','listing_read'),
('ceu_staff','platform_read'),
('ceu_staff','province_action'),
('ceu_staff','registry_view'),
('ceu_staff','upload_history_read'),
--
('lg_staff','address_write'),
('lg_staff','audit_read'),
('lg_staff','bl_link_write'),
('lg_staff','licence_file_upload'),
('lg_staff','listing_read'),
('lg_staff','registry_view'),
('lg_staff','takedown_action'),
('lg_staff','upload_history_read'),
--
('platform_staff','listing_file_upload'),
('platform_staff','takedown_file_upload'),
('platform_staff','upload_history_read'))
AS s (user_role_cd, user_privilege_cd)
) AS src
ON (tgt.user_role_cd=src.user_role_cd AND tgt.user_privilege_cd=src.user_privilege_cd)
WHEN NOT MATCHED
THEN INSERT (user_role_cd, user_privilege_cd)
VALUES (src.user_role_cd, src.user_privilege_cd);

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
('APS','MFIN','Ministry of Finance'),
('APS','SBCC','Service BC Connect'),
('BCGov','CEU','Provincial Compliance and Enforcement Unit'),
('BCGov','BC','BC Government Staff'))
AS s (organization_type, organization_cd, organization_nm)
) AS src
ON (tgt.organization_cd=src.organization_cd)
WHEN matched and (
coalesce(tgt.is_active, false) != true or
tgt.organization_nm!=src.organization_nm or
tgt.organization_type!=src.organization_type)
THEN UPDATE SET
is_active = true,
organization_nm=src.organization_nm,
organization_type=src.organization_type
WHEN NOT MATCHED
THEN INSERT (is_active, organization_type, organization_cd, organization_nm)
VALUES (true, src.organization_type, src.organization_cd, src.organization_nm);
