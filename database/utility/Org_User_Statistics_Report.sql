SELECT
	max(dot.organization_type_nm) as "Org Type",
	max(org.organization_nm) as "Organization",
	count(1) as "Enabled Users"
from dss_user_identity dui
join dss_organization org on org.organization_id=dui.represented_by_organization_id
JOIN dss_organization_type dot ON org.organization_type=dot.organization_type
WHERE dui.is_enabled=TRUE
group by org.organization_id
order by 1,2;
