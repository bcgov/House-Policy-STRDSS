----------------------------------------------------------------------------
-- DSS-1392: Update LG organization codes for UBC and University Endowment Lands
--
-- The Manage Jurisdictions UI cannot edit organization_cd. Names were already
-- updated in the portal; this script renames the reused FN placeholder codes.
--
--   LGXFN_ADAMS_LAKE  -> LGMC_UBC  (University of BC)
--   LGXFN_AITCHELITZ  -> LGMC_UEL  (University Endowment Lands)
--
-- Also updates denormalized dss_upload_line.source_organization_cd.
-- Does not change LGSUB_UBC (UBC Point Grey Campus subdivision).
-- Does not rewrite source_line_txt / source_bin (historical CSV blobs).
-- Preferred entry point: STR_DSS_Migration_Sprint_August_2026.sql
-- (this file has no BEGIN/COMMIT so the master migration can wrap it).
----------------------------------------------------------------------------


----------------------------------------------------------------------------
-- Pre-check: current LG rows
----------------------------------------------------------------------------
SELECT organization_id,
       organization_cd,
       organization_nm,
       local_government_type,
       is_active
FROM dss_organization
WHERE organization_cd IN (
    'LGXFN_ADAMS_LAKE',
    'LGXFN_AITCHELITZ',
    'LGMC_UBC',
    'LGMC_UEL'
)
ORDER BY organization_cd;

----------------------------------------------------------------------------
-- Pre-check: denormalized upload-line counts
----------------------------------------------------------------------------
SELECT source_organization_cd, COUNT(*) AS upload_line_count
FROM dss_upload_line
WHERE source_organization_cd IN (
    'LGXFN_ADAMS_LAKE',
    'LGXFN_AITCHELITZ',
    'LGMC_UBC',
    'LGMC_UEL'
)
GROUP BY source_organization_cd
ORDER BY source_organization_cd;

----------------------------------------------------------------------------
-- Apply code remaps (idempotent; aborts on unique-code collision)
----------------------------------------------------------------------------
DO $$
DECLARE
    rec RECORD;
    v_old_id bigint;
    v_new_id bigint;
    v_upload_rows integer;
BEGIN
    FOR rec IN
        SELECT *
        FROM (
            VALUES
                ('LGXFN_ADAMS_LAKE', 'LGMC_UBC'),
                ('LGXFN_AITCHELITZ', 'LGMC_UEL')
        ) AS m(old_cd, new_cd)
    LOOP
        v_old_id := NULL;
        v_new_id := NULL;

        SELECT organization_id INTO v_old_id
        FROM dss_organization
        WHERE organization_cd = rec.old_cd;

        SELECT organization_id INTO v_new_id
        FROM dss_organization
        WHERE organization_cd = rec.new_cd;

        IF v_old_id IS NOT NULL AND v_new_id IS NOT NULL THEN
            RAISE EXCEPTION
                'DSS-1392 collision: both % and % exist (ids %, %). Aborting.',
                rec.old_cd, rec.new_cd, v_old_id, v_new_id;
        END IF;

        IF v_old_id IS NULL AND v_new_id IS NULL THEN
            RAISE NOTICE
                'DSS-1392: neither % nor % found. Skipping (seed will insert % if needed).',
                rec.old_cd, rec.new_cd, rec.new_cd;
            CONTINUE;
        END IF;

        IF v_old_id IS NULL AND v_new_id IS NOT NULL THEN
            RAISE NOTICE
                'DSS-1392: % already applied (organization_id %). Skipping org update.',
                rec.new_cd, v_new_id;
        ELSE
            UPDATE dss_organization
            SET organization_cd = rec.new_cd,
                local_government_type = CASE
                    WHEN local_government_type = 'First Nations Community'
                        THEN 'Municipality'
                    ELSE local_government_type
                END
            WHERE organization_id = v_old_id;

            RAISE NOTICE
                'DSS-1392: updated organization_id % from % to %.',
                v_old_id, rec.old_cd, rec.new_cd;
        END IF;

        UPDATE dss_upload_line
        SET source_organization_cd = rec.new_cd
        WHERE source_organization_cd = rec.old_cd;

        GET DIAGNOSTICS v_upload_rows = ROW_COUNT;
        RAISE NOTICE
            'DSS-1392: updated % dss_upload_line row(s) from % to %.',
            v_upload_rows, rec.old_cd, rec.new_cd;
    END LOOP;
END $$;

----------------------------------------------------------------------------
-- Post-check: remapped LG rows
----------------------------------------------------------------------------
SELECT organization_id,
       organization_cd,
       organization_nm,
       local_government_type,
       is_active
FROM dss_organization
WHERE organization_cd IN ('LGMC_UBC', 'LGMC_UEL')
ORDER BY organization_cd;

----------------------------------------------------------------------------
-- Post-check: leftover old codes must be 0
----------------------------------------------------------------------------
SELECT COUNT(*) AS leftover_old_org_codes
FROM dss_organization
WHERE organization_cd IN ('LGXFN_ADAMS_LAKE', 'LGXFN_AITCHELITZ');

SELECT COUNT(*) AS leftover_old_upload_line_codes
FROM dss_upload_line
WHERE source_organization_cd IN ('LGXFN_ADAMS_LAKE', 'LGXFN_AITCHELITZ');

----------------------------------------------------------------------------
-- Confirm subdivision LGSUB_UBC was not changed
----------------------------------------------------------------------------
SELECT organization_id, organization_cd, organization_nm, organization_type
FROM dss_organization
WHERE organization_cd = 'LGSUB_UBC';
