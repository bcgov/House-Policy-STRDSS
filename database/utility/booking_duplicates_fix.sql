----------------------------------------------------------------------------
-- Remove duplicate master records created by Booking.com's report upload
-- using the new listing IDs (for 1:1 mappings only)
--
-- These duplicates were created because the platform uploaded a report
-- with the new listing_id numbers, which the system treated as new listings.
----------------------------------------------------------------------------

-- Duplicate platform_listing_no master records for booking.com:
SELECT 
    *
FROM dss_rental_listing rlm
WHERE rlm.platform_listing_no in (
	select listing_id
	from booking_id_map b
	where b.new_status = 'U')
  and rlm.offering_organization_id = 203
  AND rlm.including_rental_listing_report_id IS null
  group by platform_listing_no
  having count(*) > 1
order by rlm.platform_listing_no


----------------------------------------------------------------------------
--Pre game - create the tables with the ids, makes the deletes simpler
----------------------------------------------------------------------------

----------------------------------------------------------------------------
-- Identify the duplicate master records to remove
-- These are the NEWER masters (higher rental_listing_id) that share
-- a platform_listing_no with an existing master for Booking.com
----------------------------------------------------------------------------
CREATE TABLE duplicate_listings AS
SELECT 
    newer.rental_listing_id AS remove_id,
    older.rental_listing_id AS keep_id,
    newer.platform_listing_no,
    CAST(NULL AS bigint) AS locating_physical_address_id,
    CAST(NULL AS bigint) AS rental_listing_contact_id
FROM dss_rental_listing newer
INNER JOIN dss_rental_listing older
    ON newer.platform_listing_no = older.platform_listing_no
    AND older.offering_organization_id = 203
    AND older.including_rental_listing_report_id IS NULL
    AND older.rental_listing_id < newer.rental_listing_id
WHERE newer.offering_organization_id = 203
  AND newer.including_rental_listing_report_id IS NULL
  AND newer.platform_listing_no IN (
      SELECT platform_listing_no
      FROM dss_rental_listing
      WHERE offering_organization_id = 203
        AND including_rental_listing_report_id IS NULL
      GROUP BY platform_listing_no
      HAVING COUNT(*) > 1
  );

-- Populate physical address IDs
UPDATE duplicate_listings dl
SET locating_physical_address_id = rl.locating_physical_address_id
FROM dss_rental_listing rl
WHERE rl.rental_listing_id = dl.remove_id;

-- Populate rental listing contact IDs
UPDATE duplicate_listings dl
SET rental_listing_contact_id = rlc.rental_listing_contact_id
FROM dss_rental_listing_contact rlc
WHERE rlc.contacted_through_rental_listing_id = dl.remove_id;

-- Preview what will be removed
SELECT *
FROM dss_rental_listing rl
WHERE rl.rental_listing_id IN (
    SELECT remove_id FROM duplicate_listings
)
ORDER BY platform_listing_no;

----------------------------------------------------------------------------
-- Identify child records (report-level listings) derived from
-- the duplicate masters
----------------------------------------------------------------------------
CREATE TABLE duplicate_children AS
SELECT rl.derived_from_rental_listing_id AS rental_listing_id
FROM dss_rental_listing rl
INNER JOIN duplicate_listings dl ON rl.rental_listing_id = dl.remove_id
WHERE rl.derived_from_rental_listing_id IS NOT NULL;

--- Populate the associated rental listing report IDs for the child records
-- no need, single report id is 561
select distinct(including_rental_listing_report_id)
from dss_rental_listing rl
where rl.offering_organization_id = 203                 -- billing.com
and rl.including_rental_listing_report_id is not null   -- child (report) record
and rl.platform_listing_no in (
	select platform_listing_no
	from duplicate_listings
)                                                       -- with the new platform listing number

-- Preview child records
SELECT *
FROM dss_rental_listing rl
WHERE rl.rental_listing_id IN (
    SELECT rental_listing_id FROM duplicate_children
)
ORDER BY platform_listing_no;


----------------------------------------------------------------------------
-- Identify all the upload deliverys associated with the lines
----------------------------------------------------------------------------
-- 1514	Listing Data   -- uses new IDs   DELETE
-- 1495	Takedown Data  - no upload lines DELETE?
-- 1511	Listing Data   -- uses new IDs   DELETE
-- 1449	Listing Data   -- uses old IDs  STAYS

-- No need to do this, delete the records directly
-- 1,511 and 1,514 are related to listing uploads
-- 1495 is a takedown upload on the new listings.
-- 1449  is an upload listing for the old listing - might be over written, but no harm.


----------------------------------------------------------------------------
-- Identify all the upload lines that are associated with 
----------------------------------------------------------------------------
CREATE TABLE duplicate_upload_lines AS
SELECT ul.upload_line_id
FROM dss_upload_line ul
WHERE ul.source_record_no IN (
    SELECT platform_listing_no FROM duplicate_listings
);


----------------------------------------------------------------------------
-- Game on: At this point we have identified all the records that need to be removed.
----------------------------------------------------------------------------

BEGIN;

----------------------------------------------------------------------------
-- Related Emails
----------------------------------------------------------------------------
-- Preview
-- NOTE: No emails associated with child duplicate records
-- related emails are due based on takedown requests
SELECT * 
FROM dss_email_message
WHERE concerned_with_rental_listing_id IN (
    SELECT remove_id FROM duplicate_listings
);

-- TODO:  Extract the info for these actions along with the user id who initiated the action and jurisdiction (and any other rleated relevant data).

-- delete the related email messages
DELETE FROM dss_email_message
WHERE concerned_with_rental_listing_id IN (
    SELECT remove_id FROM duplicate_listings
);

----------------------------------------------------------------------------
-- Physical Address 
----------------------------------------------------------------------------
-- preview
SELECT * 
FROM dss_physical_address pa
WHERE pa.physical_address_id IN (
    SELECT locating_physical_address_id FROM duplicate_listings
    WHERE locating_physical_address_id IS NOT NULL
);


-- Show other rental listings that share a physical address with the duplicates
-- query run, no other hits found.  should be safe to remove
/**
SELECT rl.rental_listing_id, rl.platform_listing_no, rl.offering_organization_id, 
       rl.locating_physical_address_id, pa.*
FROM dss_rental_listing rl
JOIN dss_physical_address pa ON pa.physical_address_id = rl.locating_physical_address_id
WHERE rl.locating_physical_address_id IN (
    -- physical addresses used by the duplicates being removed
    SELECT dup_rl.locating_physical_address_id
    FROM dss_rental_listing dup_rl
    WHERE dup_rl.rental_listing_id IN (
        SELECT remove_id FROM duplicate_listings
    )
    AND dup_rl.locating_physical_address_id IS NOT NULL
)
AND rl.rental_listing_id NOT IN (
    -- exclude the duplicates themselves
    SELECT remove_id FROM duplicate_listings
)
AND rl.rental_listing_id NOT IN (
    -- exclude the child records
    SELECT rental_listing_id FROM duplicate_children
)
ORDER BY rl.locating_physical_address_id, rl.rental_listing_id;
**/

-- delete them
DELETE FROM dss_physical_address
WHERE physical_address_id IN (
    SELECT locating_physical_address_id FROM duplicate_listings
    WHERE locating_physical_address_id IS NOT NULL
);


----------------------------------------------------------------------------
-- Listing Contact
----------------------------------------------------------------------------
-- preview
SELECT *
FROM dss_rental_listing_contact
WHERE rental_listing_contact_id IN (
    SELECT rental_listing_contact_id FROM duplicate_listings
);

-- Contact IDs are 1:1 to the rental listing and not associated with anything else. So we can safely delete them without worrying about orphaned records or data integrity issues.

-- delete them
DELETE FROM dss_rental_listing_contact
WHERE rental_listing_contact_id IN (
    SELECT rental_listing_contact_id FROM duplicate_listings
);

----------------------------------------------------------------------------
-- Upload Lines
----------------------------------------------------------------------------
-- preview
SELECT *
FROM dss_upload_line
WHERE upload_line_id IN (
    SELECT upload_line_id FROM duplicate_upload_lines
);

-- delete
DELETE FROM dss_upload_line
WHERE upload_line_id IN (
    SELECT upload_line_id FROM duplicate_upload_lines
); 

----------------------------------------------------------------------------
-- Upload Deliveries
----------------------------------------------------------------------------
-- delete
delete from dss_upload_delivery where upload_delivery_id = 1511;
delete from dss_upload_delivery where upload_delivery_id = 1514;
delete from dss_upload_delivery where upload_delivery_id = 1495;


----------------------------------------------------------------------------
-- Related Reports
----------------------------------------------------------------------------
-- preview
-- no need, single report 561

--delete
DELETE FROM dss_rental_listing_report rlp
WHERE rental_listing_report_id = 561;


----------------------------------------------------------------------------
-- Remove the child listing records (report-level)
----------------------------------------------------------------------------
DELETE FROM dss_rental_listing
WHERE rental_listing_id IN (
    SELECT rental_listing_id FROM duplicate_children
);

----------------------------------------------------------------------------
-- Remove the duplicate master listing records
----------------------------------------------------------------------------
DELETE FROM dss_rental_listing
WHERE rental_listing_id IN (
    SELECT remove_id FROM duplicate_listings
);

----------------------------------------------------------------------------
-- Verify no more duplicates remain
----------------------------------------------------------------------------
SELECT 
    platform_listing_no,
    COUNT(*) AS cnt
FROM dss_rental_listing
WHERE offering_organization_id = 203
  AND including_rental_listing_report_id IS NULL
GROUP BY platform_listing_no
HAVING COUNT(*) > 1;

----------------------------------------------------------------------------
-- Update all the original child records to the new platform listing no
----------------------------------------------------------------------------
UPDATE dss_rental_listing rl
SET platform_listing_no = bm.listing_id
FROM booking_id_map bm
WHERE rl.platform_listing_no = bm.old_id
  AND rl.offering_organization_id = 203
  AND rl.including_rental_listing_report_id IS NOT NULL
  AND bm.new_status = 'U';


-- Transaction is still open. Review results above.
-- Run ONE of the following manually:
--   COMMIT;    -- to apply all changes
--   ROLLBACK;  -- to undo all changes
-- (If you close the session without committing, all changes are automatically rolled back)



----------------------------------------------------------------------------
-- Clean up tables
----------------------------------------------------------------------------
DROP TABLE IF EXISTS duplicate_listings;
DROP TABLE IF EXISTS duplicate_children;
DROP TABLE IF EXISTS duplicate_upload_lines;
