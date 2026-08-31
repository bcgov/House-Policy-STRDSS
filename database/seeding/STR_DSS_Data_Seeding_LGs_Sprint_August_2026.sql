/* STR DSS Sprint August 2026 Local Governments Data Seeding
 * DSS-1392: University of BC and University Endowment Lands
 *
 * Additive MERGE only — does not deactivate other LGs (unlike Sprint 19).
 * On environments that still have LGXFN_ADAMS_LAKE / LGXFN_AITCHELITZ,
 * run database/utility/DSS-1392.sql first (included by the August 2026
 * migration) so those rows are remapped instead of inserting duplicates.
 */

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
('Municipality'           ,'LGMC_UBC'                 ,'University of BC'                                    ),
('Municipality'           ,'LGMC_UEL'                 ,'University Endowment Lands'                          ))
AS s (local_government_type, organization_cd, organization_nm)
) AS src
ON (tgt.organization_cd=UPPER(src.organization_cd))
WHEN MATCHED AND (
	coalesce(tgt.is_active, false) != true or
	coalesce(tgt.is_lg_participating, false) != true or
	tgt.organization_nm!=src.organization_nm or
	tgt.organization_type!='LG' or
	tgt.local_government_type!=src.local_government_type or
	tgt.local_government_type is null)
THEN UPDATE SET
	is_active = true,
	is_lg_participating = true,
	organization_nm=src.organization_nm,
	organization_type='LG',
	local_government_type=src.local_government_type
WHEN NOT MATCHED
THEN INSERT (is_active, is_lg_participating, organization_type, organization_cd, organization_nm, local_government_type)
VALUES (true, true, 'LG', src.organization_cd, src.organization_nm, src.local_government_type);
