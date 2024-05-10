SELECT coalesce(org.organization_nm,'Unknown') as "Name of Local government"
	, sum(case dem.email_message_type when 'Notice of Takedown' then 1 else 0 end) as "Notice of Non-compliance sent"
	, sum(case dem.email_message_type when 'Takedown Request' then 1 else 0 end) as "Takedown Request sent"
from dss_email_message dem
LEFT JOIN dss_user_identity dui on dem.initiating_user_identity_id=dui.user_identity_id
left join dss_organization org on org.organization_id=coalesce(dem.requesting_organization_id,represented_by_organization_id)
where email_message_type in ('Takedown Request','Notice of Takedown')
group by org.organization_nm
order by 1;
