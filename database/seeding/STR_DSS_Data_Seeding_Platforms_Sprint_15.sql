/* STR DSS Sprint 15 Platforms Data Seeding */

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
('Major',NULL      ,'PLAT-AIRBNB'   ,'Airbnb'),
('Major',NULL      ,'PLAT-BOOKING'  ,'Booking.com'),
('Major',NULL      ,'PLAT-EXP'      ,'Expedia'),
('Major','PLAT-EXP','PLAT-EXP-VRBO' ,'VRBO'),
('Minor',NULL      ,'PLAT-ALLURA'   ,'alluraDirect Vacation Rentals'),
('Minor',NULL      ,'PLAT-ARTIN'    ,'Artin Properties'),
('Minor',NULL      ,'PLAT-BCCAB'    ,'BC Cabin and Cottage'),
('Minor',NULL      ,'PLAT-BEACH'    ,'Beach Acres Resort'),
('Minor',NULL      ,'PLAT-BLAC-PA'  ,'Blackcomb Peaks Accommodations'),
('Minor',NULL      ,'PLAT-COHOST'   ,'Co-Hosts Vacation Rental Specialists'),
('Minor',NULL      ,'PLAT-ELITE'    ,'Elite Vacation Homes Inc.'),
('Minor',NULL      ,'PLAT-EMR-V'    ,'EMR VACATION RENTALS INC'),
('Minor',NULL      ,'PLAT-HOMEGO'   ,'HomeToGo GmbH'),
('Minor',NULL      ,'PLAT-HOMEST'   ,'Homestay.com'),
('Minor',NULL      ,'PLAT-IRIS'     ,'Iris Properties'),
('Minor',NULL      ,'PLAT-ISLANDVH' ,'Island Vacation Homes Ltd.'),
('Minor',NULL      ,'PLAT-KEYSK'    ,'Keys to Kelowna'),
('Minor',NULL      ,'PLAT-LIFTY'    ,'Lifty Life'),
('Minor',NULL      ,'PLAT-MISTER-BB','Mister B&B'),
('Minor',NULL      ,'PLAT-MYHORNBY' ,'My Hornby Stay'),
('Minor',NULL      ,'PLAT-OVHR'     ,'Okanagan Vacation Home Rentals'),
('Minor',NULL      ,'PLAT-OWNERD'   ,'OwnerDirect'),
('Minor',NULL      ,'PLAT-PLUMG'    ,'Plum Guide'),
('Minor',NULL      ,'PLAT-VACREN'   ,'VacationRenter, LLC'),
('Minor',NULL      ,'PLAT-VAN-SS'   ,'Vancouver Short Stay Apts. Inc.'),
('Minor',NULL      ,'PLAT-WHISKI'   ,'Whiski Jack Resorts'),
('Minor',NULL      ,'PLAT-WHISRES'  ,'Whistler Reservations'))
AS s (platform_type, parent_org_cd, organization_cd, organization_nm)
) AS src
ON (tgt.organization_cd=src.organization_cd)
WHEN matched and (tgt.organization_nm != src.organization_nm or tgt.platform_type is null)
THEN UPDATE set organization_nm=src.organization_nm, platform_type=src.platform_type
WHEN NOT MATCHED
THEN INSERT (organization_type, platform_type, organization_cd, organization_nm, managing_organization_id)
VALUES ('Platform', src.platform_type, src.organization_cd, src.organization_nm, (select o.organization_id from dss_organization as o where o.organization_cd=src.parent_org_cd));
