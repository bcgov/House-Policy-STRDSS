/* STR DSS Sprint 6 Local Government Subdivision assignments to Physical Addresses */

update dss_physical_address
set containing_organization_id=dss_containing_organization_id(location_geometry)
where containing_organization_id is null and location_geometry is not null;
