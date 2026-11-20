/* STR DSS Sprint August 2026 Local Governments Data Seeding - DSS-1392 Update LG Codes - UBC, UEL */
/* Names/types were already updated in the Portal; only the codes change. organization_id is unchanged so linked data follows the new code. */

UPDATE dss_organization AS o SET
	organization_cd = m.new_cd
FROM (VALUES
	('LGXFN_AITCHELITZ', 'LGMC_UEL'),
	('LGXFN_ADAMS_LAKE', 'LGMC_UBC')
) AS m (old_cd, new_cd)
WHERE UPPER(o.organization_cd) = m.old_cd
  AND o.organization_type = 'LG'
  AND NOT EXISTS (SELECT 1 FROM dss_organization x WHERE x.organization_cd = m.new_cd);
