/*simple extract of listings for a month - quoted_report_period parameter must be in form 'YYYY-MM-01'*/
select org.organization_cd as listing_platform_cd
	, drl.platform_listing_no
	, lg.organization_nm as local_government_nm
	, lgs.economic_region_dsc
FROM dss_rental_listing drl
join dss_rental_listing_report drlr on drlr.rental_listing_report_id=drl.including_rental_listing_report_id
join dss_organization org on org.organization_id=drl.offering_organization_id
LEFT JOIN dss_physical_address dpa on drl.locating_physical_address_id=dpa.physical_address_id
left join dss_organization lgs on lgs.organization_id=dpa.containing_organization_id
left join dss_organization lg on lgs.managing_organization_id=lg.organization_id
where drlr.report_period_ym=:quoted_report_period
order by 1,2;

/*verbose extract of listings for a month - quoted_report_period parameter must be in form 'YYYY-MM-01'*/
select org.organization_cd as listing_platform_cd
	, org.organization_nm as listing_platform_nm
	, drl.platform_listing_no
	, drl.platform_listing_url
	, dpa.original_address_txt
	, dpa.match_score_amt
	, dpa.match_address_txt
	, dpa.province_cd as address_sort_1_province_cd
	, dpa.locality_nm as address_sort_2_locality_nm
	, dpa.locality_type_dsc as address_sort_3_locality_type_dsc
	, dpa.street_nm as address_sort_4_street_nm
	, dpa.street_type_dsc as address_sort_5_street_type_dsc
	, dpa.street_direction_dsc as address_sort_6_street_direction_dsc
	, dpa.civic_no as address_sort_7_civic_no
	, dpa.unit_no as address_sort_8_unit_no
	, (select string_agg(full_nm,' ; ') from dss_rental_listing_contact drlc where drlc.contacted_through_rental_listing_id=drl.rental_listing_id) as listing_contact_names_txt
--	, lg.local_government_type
	, lg.organization_nm as local_government_nm
	, lgs.organization_nm as subdivision_nm
	, lgs.economic_region_dsc
	, lgs.is_principal_residence_required
	, lgs.is_business_licence_required
	, drl.is_entire_unit
	, drl.available_bedrooms_qty
	, drl.nights_booked_qty
	, drl.separate_reservations_qty
	, drl.business_licence_no
	, drl.bc_registry_no
FROM dss_rental_listing drl
join dss_rental_listing_report drlr on drlr.rental_listing_report_id=drl.including_rental_listing_report_id
join dss_organization org on org.organization_id=drl.offering_organization_id
LEFT JOIN dss_physical_address dpa on drl.locating_physical_address_id=dpa.physical_address_id
left join dss_organization lgs on lgs.organization_id=dpa.containing_organization_id
left join dss_organization lg on lgs.managing_organization_id=lg.organization_id
where drlr.report_period_ym=:quoted_report_period
order by 1,2,3;

/*extract of listing counts by platform/jurisdiction for all reporting periods*/
select drlr.report_period_ym
	, org.organization_cd as listing_platform_cd
	, org.organization_nm as listing_platform_nm
--	, lg.local_government_type
	, lg.organization_nm as local_government_nm
	, lgs.organization_nm as subdivision_nm
	, lgs.economic_region_dsc
	, lgs.is_principal_residence_required
	, lgs.is_business_licence_required
	, count(1) as listing_count
FROM dss_rental_listing drl
join dss_rental_listing_report drlr on drlr.rental_listing_report_id=drl.including_rental_listing_report_id
join dss_organization org on org.organization_id=drl.offering_organization_id
LEFT JOIN dss_physical_address dpa on drl.locating_physical_address_id=dpa.physical_address_id
left join dss_organization lgs on lgs.organization_id=dpa.containing_organization_id
left join dss_organization lg on lgs.managing_organization_id=lg.organization_id
group by drlr.report_period_ym
	, org.organization_cd
	, org.organization_nm
--	, lg.local_government_type
	, lg.organization_nm
	, lgs.organization_nm
	, lgs.economic_region_dsc
	, lgs.is_principal_residence_required
	, lgs.is_business_licence_required
order by 1,2,3,4,5;
