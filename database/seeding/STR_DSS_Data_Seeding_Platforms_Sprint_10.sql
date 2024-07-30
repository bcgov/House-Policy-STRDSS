/* STR DSS Sprint 10 Platforms Data Seeding */

MERGE INTO dss_organization AS tgt
USING ( SELECT * FROM (VALUES
(NULL      ,'PLAT-AIRBNB'   ,'Airbnb'),
(NULL      ,'PLAT-ALLURA'   ,'alluraDirect Vacation Rentals'),
(NULL      ,'PLAT-ARTIN'    ,'Artin Properties'),
(NULL      ,'PLAT-BCCAB'    ,'BC Cabin and Cottage'),
(NULL      ,'PLAT-BEACH'    ,'Beach Acres Resort'),
(NULL      ,'PLAT-BLAC-PA'  ,'Blackcomb Peaks Accommodations'),
(NULL      ,'PLAT-BOOKING'  ,'Booking.com'),
(NULL      ,'PLAT-COHOST'   ,'Co-Hosts Vacation Rental Specialists'),
(NULL      ,'PLAT-ELITE'    ,'Elite Vacation Homes Inc.'),
(NULL      ,'PLAT-EMR-V'    ,'EMR VACATION RENTALS INC'),
(NULL      ,'PLAT-EXP'      ,'Expedia'),
('PLAT-EXP','PLAT-EXP-VRBO' ,'VRBO'),
(NULL      ,'PLAT-HOMEGO'   ,'HomeToGo GmbH'),
(NULL      ,'PLAT-HOMEST'   ,'Homestay.com'),
(NULL      ,'PLAT-IRIS'     ,'Iris Properties'),
(NULL      ,'PLAT-ISLANDVH' ,'Island Vacation Homes Ltd.'),
(NULL      ,'PLAT-KEYSK'    ,'Keys to Kelowna'),
(NULL      ,'PLAT-LIFTY'    ,'Lifty Life'),
(NULL      ,'PLAT-MISTER-BB','Mister B&B'),
(NULL      ,'PLAT-MYHORNBY' ,'My Hornby Stay'),
(NULL      ,'PLAT-OVHR'     ,'Okanagan Vacation Home Rentals'),
(NULL      ,'PLAT-OWNERD'   ,'OwnerDirect'),
(NULL      ,'PLAT-PLUMG'    ,'Plum Guide'),
(NULL      ,'PLAT-VACREN'   ,'VacationRenter, LLC'),
(NULL      ,'PLAT-VAN-SS'   ,'Vancouver Short Stay Apts. Inc.'),
(NULL      ,'PLAT-WHISKI'   ,'Whiski Jack Resorts'),
(NULL      ,'PLAT-WHISRES'  ,'Whistler Reservations'))
AS s (parent_org_cd, organization_cd, organization_nm)
) AS src
ON (tgt.organization_cd=src.organization_cd)
WHEN matched and tgt.organization_nm != src.organization_nm
THEN UPDATE set organization_nm=src.organization_nm
WHEN NOT MATCHED
THEN INSERT (organization_type, organization_cd, organization_nm, managing_organization_id)
VALUES ('Platform', src.organization_cd, src.organization_nm, (select o.organization_id from dss_organization as o where o.organization_cd=src.parent_org_cd));
