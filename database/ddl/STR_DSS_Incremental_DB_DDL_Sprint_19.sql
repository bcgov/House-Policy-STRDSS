/* Sprint 19 Incremental DB Changes to STR DSS */

ALTER TABLE dss_organization ADD is_straa_exempt boolean    ;

ALTER TABLE dss_organization ADD source_attributes_json jsonb    ;

COMMENT ON COLUMN dss_organization.is_straa_exempt IS 'Indicates whether a LOCAL GOVERNMENT SUBDIVISION is exempt from all Provincial Short Term Rental restrictions';

COMMENT ON COLUMN dss_organization.source_attributes_json IS 'A JSON object containing non-geometry fields from the Local Government Subdivision information source';
