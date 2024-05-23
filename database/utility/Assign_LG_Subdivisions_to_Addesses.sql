SELECT physical_address_id,original_address_txt,match_address_txt,(select max(organization_nm) from dss_organization do2 where location_geometry && area_geometry)
FROM dss_physical_address dpa
where containing_organization_id is null and location_geometry is not null;

update dss_physical_address
set containing_organization_id=(select max(organization_id) from dss_organization where location_geometry && area_geometry)
where containing_organization_id is null and location_geometry is not null;
