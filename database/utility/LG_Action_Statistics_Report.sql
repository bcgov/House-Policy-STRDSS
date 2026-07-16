SELECT coalesce(org.organization_nm,'Unknown') as "Name of Local government"
	, sum(case dem.email_message_type when 'Notice of Takedown' then 1 else 0 end) as "Notice of Non-compliance sent"
	, sum(case dem.email_message_type when 'Takedown Request' then 1 else 0 end) as "Takedown Request sent"
from dss_email_message dem
LEFT JOIN dss_user_identity dui on dem.initiating_user_identity_id=dui.user_identity_id
left join dss_organization org on org.organization_id=coalesce(dem.requesting_organization_id,represented_by_organization_id)
where email_message_type in ('Takedown Request','Notice of Takedown')
group by org.organization_nm
order by 1;

-- DSS-1346: Update above to report for a specific month.
SELECT to_char(date_trunc('month', current_date) - interval '1 month', 'YYYY-MM') as "Reporting Month"
    , coalesce(org.organization_nm,'Unknown') as "Name of Local government"
    , sum(case dem.email_message_type when 'Notice of Takedown' then 1 else 0 end) as "Notice of Non-compliance sent"
    , sum(case dem.email_message_type when 'Takedown Request' then 1 else 0 end) as "Takedown Request sent"
from dss_email_message dem
LEFT JOIN dss_user_identity dui on dem.initiating_user_identity_id=dui.user_identity_id
left join dss_organization org on org.organization_id=coalesce(dem.requesting_organization_id,represented_by_organization_id)
where email_message_type in ('Takedown Request','Notice of Takedown')
  and dem.message_delivery_dtm >= date_trunc('month', current_date) - interval '1 month'
  and dem.message_delivery_dtm < date_trunc('month', current_date)
group by org.organization_nm
order by 2;

-- via setting the date:
WITH params AS (SELECT '2026-03-01'::date AS report_month)
SELECT to_char(params.report_month, 'YYYY-MM') as "Reporting Month"
    , coalesce(org.organization_nm,'Unknown') as "Name of Local government"
    , sum(case dem.email_message_type when 'Notice of Takedown' then 1 else 0 end) as "Notice of Non-compliance sent"
    , sum(case dem.email_message_type when 'Takedown Request' then 1 else 0 end) as "Takedown Request sent"
from params, dss_email_message dem
LEFT JOIN dss_user_identity dui on dem.initiating_user_identity_id=dui.user_identity_id
left join dss_organization org on org.organization_id=coalesce(dem.requesting_organization_id,represented_by_organization_id)
where email_message_type in ('Takedown Request','Notice of Takedown')
  and dem.message_delivery_dtm >= params.report_month
  and dem.message_delivery_dtm < params.report_month + interval '1 month'
group by params.report_month, org.organization_nm
order by 2;

-- DSS-1346: Add report of actual takedowns per platform
  SELECT to_char(dud.report_period_ym, 'YYYY-MM') as "Reporting Month"
    , org.organization_nm as "Platform"
    , count(*) as "Takedown files uploaded"
    , sum(dud.upload_lines_total) as "Total lines"
    , sum(dud.upload_lines_success) as "Lines successful"
from dss_upload_delivery dud
join dss_organization org on org.organization_id = dud.providing_organization_id
where dud.upload_delivery_type = 'Takedown Data'
  and dud.report_period_ym = date_trunc('month', current_date) - interval '1 month'
group by dud.report_period_ym, org.organization_nm
order by 2;

-- and via setting the month:
WITH params AS (SELECT '2026-03-01'::date AS report_month)
SELECT to_char(params.report_month, 'YYYY-MM') as "Reporting Month"
    , org.organization_nm as "Platform"
    , count(*) as "Takedown files uploaded"
    , sum(dud.upload_lines_total) as "Total lines"
    , sum(dud.upload_lines_success) as "Lines successful"
from params, dss_upload_delivery dud
join dss_organization org on org.organization_id = dud.providing_organization_id
where dud.upload_delivery_type = 'Takedown Data'
  and dud.report_period_ym = params.report_month
group by params.report_month, org.organization_nm
order by 2;


-- BONUS: Unprocessed takedowns by platform within the las 12 months:
SELECT org.organization_nm as "Platform"
    , count(*) as "Unprocessed takedown lines"
from dss_upload_line dul
join dss_upload_delivery dud on dud.upload_delivery_id = dul.including_upload_delivery_id
join dss_organization org on org.organization_id = dud.providing_organization_id
where dud.upload_delivery_type = 'Takedown Data'
  and dud.report_period_ym >= date_trunc('month', current_date) - interval '12 months'
  and dul.is_processed = false
group by org.organization_nm
order by 1;

-- Sprint 22+: Listing action-based LG statistics (preferred once action hooks are live)
SELECT coalesce(org.organization_nm,'Unknown') as "Name of Local government"
	, sum(case drla.listing_action_type when 'NonComplianceNotice' then 1 else 0 end) as "Notice of Non-compliance sent"
	, sum(case drla.listing_action_type when 'TakedownRequest' then 1 else 0 end) as "Takedown Request sent"
from dss_rental_listing_action drla
inner join dss_rental_listing drl on drl.rental_listing_id = drla.rental_listing_id
left join dss_physical_address dpa on dpa.physical_address_id = drl.locating_physical_address_id
left join dss_organization lgs on lgs.organization_id = dpa.containing_organization_id
left join dss_organization org on org.organization_id = lgs.managing_organization_id
where drla.listing_action_type in ('NonComplianceNotice', 'TakedownRequest')
group by org.organization_nm
order by 1;

-- Sprint 22+: Listing action-based LG statistics for a specific month
WITH params AS (SELECT '2026-03-01'::date AS report_month)
SELECT to_char(params.report_month, 'YYYY-MM') as "Reporting Month"
    , coalesce(org.organization_nm,'Unknown') as "Name of Local government"
    , sum(case drla.listing_action_type when 'NonComplianceNotice' then 1 else 0 end) as "Notice of Non-compliance sent"
    , sum(case drla.listing_action_type when 'TakedownRequest' then 1 else 0 end) as "Takedown Request sent"
from params, dss_rental_listing_action drla
inner join dss_rental_listing drl on drl.rental_listing_id = drla.rental_listing_id
left join dss_physical_address dpa on dpa.physical_address_id = drl.locating_physical_address_id
left join dss_organization lgs on lgs.organization_id = dpa.containing_organization_id
left join dss_organization org on org.organization_id = lgs.managing_organization_id
where drla.listing_action_type in ('NonComplianceNotice', 'TakedownRequest')
  and drla.action_dtm >= params.report_month
  and drla.action_dtm < params.report_month + interval '1 month'
group by params.report_month, org.organization_nm
order by 2;