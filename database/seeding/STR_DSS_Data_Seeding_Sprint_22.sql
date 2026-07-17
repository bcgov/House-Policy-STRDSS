/* STR DSS Sprint 22 Mandatory Data Seeding - Listing action types */

MERGE INTO dss_listing_action_type AS tgt
USING (
	SELECT * FROM (VALUES
		('NonComplianceNotice', 'Non-Compliance Notice',           'User',     1),
		('NoticeOfTakedown',    'Notice of Takedown',                'User',     2),
		('TakedownRequest',     'Takedown Request',                  'User',     3),
		('DelistingWarning',    'Delisting Warning',                 'User',     4),
		('DelistingRequest',    'Delisting Request',                 'User',     5),
		('PlatformTakedown',    'Platform Takedown Confirmed',       'Platform', 6),
		('LgTransfer',          'Transferred to Local Government',   'System',   7),
		('ComplianceOrder',     'Compliance Order',                  'User',     8)
	) AS s (
		listing_action_type,
		listing_action_type_nm,
		action_source_type,
		listing_action_sort_no
	)
) AS src
ON (tgt.listing_action_type = src.listing_action_type)
WHEN MATCHED AND (
	   tgt.listing_action_type_nm  <> src.listing_action_type_nm
	OR tgt.action_source_type      <> src.action_source_type
	OR tgt.listing_action_sort_no  <> src.listing_action_sort_no
) THEN UPDATE SET
	listing_action_type_nm = src.listing_action_type_nm,
	action_source_type     = src.action_source_type,
	listing_action_sort_no = src.listing_action_sort_no
WHEN NOT MATCHED THEN INSERT (
	listing_action_type,
	listing_action_type_nm,
	action_source_type,
	listing_action_sort_no
) VALUES (
	src.listing_action_type,
	src.listing_action_type_nm,
	src.action_source_type,
	src.listing_action_sort_no
);
