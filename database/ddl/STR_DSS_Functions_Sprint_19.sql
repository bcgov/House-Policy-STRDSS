/* STR DSS Sprint 19 Modified Function */

create or replace function dss_containing_organization_id(p_point public.geometry(point, 4326))
	returns bigint
	LANGUAGE sql
	immutable
	RETURNS NULL ON NULL input
	RETURN (
		select do1.organization_id
		from dss_organization do1
		where ST_Intersects(p_point,do1.area_geometry)
		  and do1.is_active
		  and not exists(
			select do2.organization_nm
			from dss_organization do2
			where ST_Intersects(p_point,do2.area_geometry)
			and do2.is_active
			and do2.organization_id != do1.organization_id
			and st_area(do2.area_geometry) < st_area(do1.area_geometry)));
