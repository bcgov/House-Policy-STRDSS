/* Sprint 17 View Changes to STR DSS */

drop view if exists dss_local_gov_vw;

CREATE OR REPLACE VIEW dss_local_gov_vw AS 
SELECT do2.organization_id
, do2.organization_type
, do2.organization_cd
, do2.organization_nm
, do2.local_government_type
, lgt.local_government_type_nm
, do2.business_licence_format_txt
FROM dss_organization do2
left join dss_local_government_type lgt on lgt.local_government_type = do2.local_government_type
where do2.organization_type = 'LG';