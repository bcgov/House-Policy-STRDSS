MERGE INTO dss_user_privilege AS tgt
USING ( SELECT * FROM (VALUES
('takedown_action_write','Create Takedown Action'))
AS s (privilege_cd, privilege_nm)
) AS src
ON (tgt.privilege_cd=src.privilege_cd)
WHEN MATCHED
THEN UPDATE SET
privilege_nm=src.privilege_nm
WHEN NOT MATCHED
THEN INSERT (privilege_cd, privilege_nm)
VALUES (src.privilege_cd, src.privilege_nm);

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

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
('BCGov','CEU','Compliance Enforcement Unit'))
AS s (organization_type, organization_cd, organization_nm)
) AS src
ON (tgt.organization_cd=src.organization_cd AND tgt.organization_type=src.organization_type)
WHEN MATCHED
THEN UPDATE SET
organization_nm=src.organization_nm
WHEN NOT MATCHED
THEN INSERT (organization_type, organization_cd, organization_nm)
VALUES (src.organization_type, src.organization_cd, src.organization_nm);
