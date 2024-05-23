CREATE OR REPLACE VIEW dss_rental_listing_vw AS select drl.rental_listing_id
	, case drl.is_taken_down when 'Y' then 'R' else 'A' end as listing_status_type
	, (select max(drlr.report_period_ym)
		from dss_rental_listing drl2
		join dss_rental_listing_report drlr on drlr.rental_listing_report_id=drl2.including_rental_listing_report_id
		where drl2.offering_organization_id=drl.offering_organization_id and drl2.platform_listing_no=drl.platform_listing_no
		) as latest_report_period_ym
	, drl.is_taken_down
	, drl.offering_organization_id
	, org.organization_nm as offering_organization_nm
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
	, lgs.managing_organization_id
	, lg.organization_nm as managing_organization_nm
	, drl.is_entire_unit
	, drl.available_bedrooms_qty
	, (select sum(drl2.nights_booked_qty) from dss_rental_listing drl2 where drl2.offering_organization_id=drl.offering_organization_id and drl2.platform_listing_no=drl.platform_listing_no) as nights_booked_ytd_qty
	, (select sum(drl2.separate_reservations_qty) from dss_rental_listing drl2 where drl2.offering_organization_id=drl.offering_organization_id and drl2.platform_listing_no=drl.platform_listing_no) as separate_reservations_ytd_qty
	, drl.business_licence_no
	, drl.bc_registry_no
	, demt.email_message_type_nm as last_action_nm
	, dem.message_delivery_dtm as last_action_dtm
FROM dss_rental_listing drl
join dss_organization org on org.organization_id=drl.offering_organization_id
LEFT JOIN dss_physical_address dpa on drl.locating_physical_address_id=dpa.physical_address_id
left join dss_organization lgs on lgs.organization_id=dpa.containing_organization_id
left join dss_organization lg on lgs.managing_organization_id=lg.organization_id
LEFT JOIN dss_email_message dem on dem.concerned_with_rental_listing_id=drl.rental_listing_id
LEFT JOIN dss_email_message_type demt on dem.email_message_type=demt.email_message_type
where drl.including_rental_listing_report_id is null;
