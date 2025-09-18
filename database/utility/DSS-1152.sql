select drl.rental_listing_id as ListingId, drl.platform_listing_no as PlatformNo, drl.platform_listing_url as PlatformURL,
	dpa.original_address_txt as Address,
	po.organization_id as PlatformOrgId, po.organization_nm as PlatformOrgName,
	lg.organization_id as LocalGovId, lg.organization_type LocalGovType, lg.organization_cd as LocalGovCode, lg.organization_nm as LocalGovName,
	mo.organization_id as ManagingOrgID, mo.organization_type as ManagingType, mo.organization_cd as ManagingCode, mo.organization_nm as ManagingName
from dss_rental_listing drl, dss_physical_address dpa, dss_organization po, dss_organization lg, dss_organization mo 
where drl.locating_physical_address_id = dpa.physical_address_id
and drl.offering_organization_id = po.organization_id
and dpa.containing_organization_id = lg.organization_id
and lg.managing_organization_id = mo.organization_id
--and dpa.containing_organization_id = 599
and lg.is_
and drl.including_rental_listing_report_id is null

select *
from dss_local_gov_vw dlgv 
where organization_nm = 'Islands Trust'

select *
from dss_organization do2 
where organization_nm = 'Islands Trust'

select * from dss_organization do2 
where organization_id = 184

-- 171 = LG 	LGNILTA_SALT_SPRING		Salt Spring Island Local Trust Area
-- 235 = LG 	LGNILTA_ISLANDS			Islands Trust
-- 599 = LGSub	LGSUB-633				Saltspring Island
-- 192 

select *
from dss_organization do2 
where organization_nm = 'Saltspring Island'

select drl.rental_listing_id, drl.upd_dtm, dpa.original_address_txt, do2.organization_id, do2.organization_cd, do2.organization_type, do2.organization_nm, do2.economic_region_dsc
from dss_rental_listing drl, dss_physical_address dpa, dss_organization do2 
where drl.locating_physical_address_id = dpa.physical_address_id
and dpa.containing_organization_id = do2.organization_id
and do2.managing_organization_id = 171
and drl.including_rental_listing_report_id is null
order by drl.rental_listing_id desc

select * --count(drl.rental_listing_id)
from dss_rental_listing drl, dss_physical_address dpa, dss_organization do2 
where drl.locating_physical_address_id = dpa.physical_address_id
and dpa.containing_organization_id = do2.organization_id
and do2.managing_organization_id = 235
and drl.including_rental_listing_report_id is null

select drl.rental_listing_id as ListingId, drl.platform_listing_no as PlatformNo, drl.platform_listing_url as PlatformURL,
	dpa.original_address_txt as Address,
	po.organization_id as PlatformOrgId, po.organization_nm as PlatformOrgName,
	lg.organization_id as LocalGovId, lg.organization_type LocalGovType, lg.organization_cd as LocalGovCode, lg.organization_nm as LocalGovName,
	mo.organization_id as ManagingOrgID, mo.organization_type as ManagingType, mo.organization_cd as ManagingCode, mo.organization_nm as ManagingName
from dss_rental_listing drl, dss_physical_address dpa, dss_organization po, dss_organization lg, dss_organization mo 
where drl.locating_physical_address_id = dpa.physical_address_id
and drl.offering_organization_id = po.organization_id
and dpa.containing_organization_id = lg.organization_id
and lg.managing_organization_id = mo.organization_id
-- and dpa.containing_organization_id = 599
and lg.organization_type = 'LGSub' and lg.is_active is true
and drl.including_rental_listing_report_id is null

select count(drl.rental_listing_id)
from dss_rental_listing drl, dss_physical_address dpa, dss_organization po, dss_organization lg, dss_organization mo 
where drl.locating_physical_address_id = dpa.physical_address_id
and drl.offering_organization_id = po.organization_id
and dpa.containing_organization_id = lg.organization_id
and lg.managing_organization_id = mo.organization_id
and lg.organization_type = 'LGSub' and lg.is_active is false
and drl.including_rental_listing_report_id is null

select count(drl.rental_listing_id)
from dss_rental_listing drl, dss_physical_address dpa, dss_organization lg 
where drl.including_rental_listing_report_id is null
and drl.locating_physical_address_id = dpa.physical_address_id
and dpa.containing_organization_id is null 

select count(drl.rental_listing_id)
from dss_rental_listing drl
where drl.including_rental_listing_report_id is null
and drl.is_current is true

select *
from dss_physical_address dpa
where containing_organization_id = 1471

select *
from dss_organization do2 
where organization_id = 527

select do2.organization_id, do2.organization_cd, do2.organization_type, do2.organization_nm, do2.economic_region_dsc
from dss_organization do2
where do2.managing_organization_id = 171

select *
from dss_physical_address dpa
where dpa.original_address_txt like '%Gabr%'

SELECT org1.organization_id AS org1_id, org1.organization_nm as org1_nm, org1.organization_type as org1_type, org1.organization_cd as org1_cd,
       org2.organization_id AS org2_id, org2.organization_nm as org2_nm, org2.organization_type as org2_type, org2.organization_cd as org2_cd, org2.managing_organization_id as org2_mng,
       ST_Overlaps(org1.area_geometry, org2.area_geometry) AS is_overlapping
FROM dss_organization org1
JOIN dss_organization org2
  ON org1.organization_id != org2.organization_id
WHERE ST_Equals(org1.area_geometry, org2.area_geometry)
and org1.organization_id = 599;

SELECT org1.organization_id AS ManagingOrgId, org1.organization_nm as ManagingOrgName, org1.organization_type as ManagingOrdType, org1.organization_cd as ManagingOrgCode,
       org2.organization_id AS org2_id, org2.organization_nm as org2_nm, org2.organization_type as org2_type, org2.organization_cd as org2_cd
FROM dss_organization org1,dss_organization org2
where org1.organization_id = org2.managing_organization_id
order by ManagingOrgId

select dpa.original_address_txt as Address, dpa.match_address_txt as matched, dpa.match_score_amt as Accuracy, do2.organization_nm as JurisdictionName, do2.organization_cd as JusrisdictionCode
from dss_rental_listing drl
join dss_physical_address dpa on drl.locating_physical_address_id = dpa.physical_address_id 
join dss_organization do2 on dpa.containing_organization_id = do2.organization_id
where drl.including_rental_listing_report_id is null
	and drl.is_active = true
	and dpa.upd_dtm 
order by Accuracy asc