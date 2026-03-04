----------------------------------------------------------------------------
-- Create a table to hold the mapping ids for booking.com
----------------------------------------------------------------------------
CREATE TABLE booking_id_map (
    hotel_id TEXT,
    room_id TEXT,
    listing_id TEXT
);

----------------------------------------------------------------------------
-- Populate the data with the csv created from the excel spreadsheet (no script)
----------------------------------------------------------------------------

----------------------------------------------------------------------------
-- Add a new column to hold the new status (Updated or Deactivated)
----------------------------------------------------------------------------
ALTER TABLE booking_id_map ADD COLUMN new_status varchar(1);


----------------------------------------------------------------------------
-- Set the status to 'U' if there is a 1:1 mapping for ids
----------------------------------------------------------------------------
UPDATE booking_id_map
SET new_status = 'U'
WHERE hotel_id IN (
    SELECT hotel_id
    FROM booking_id_map
    GROUP BY hotel_id
    HAVING COUNT(DISTINCT listing_id) = 1
);

----------------------------------------------------------------------------
-- Set the status to 'U' if there is a 1:1 mapping for ids
----------------------------------------------------------------------------
UPDATE booking_id_map
SET new_status = 'D'
WHERE hotel_id IN (
    SELECT hotel_id
    FROM booking_id_map
    GROUP BY hotel_id
    HAVING COUNT(DISTINCT listing_id) = 1
);

----------------------------------------------------------------------------
-- Update listing numbers and status for listings with new status 'U'
----------------------------------------------------------------------------
-- Preview first
SELECT 
    rl.rental_listing_id,
    rl.platform_listing_no AS old_listing_no,
    m.listing_id AS new_listing_no,
    m.new_status
FROM dss_rental_listing rl
INNER JOIN booking_id_map m
    ON rl.platform_listing_no = m.hotel_id
WHERE rl.offering_organization_id = 203
  AND rl.including_rental_listing_report_id IS NULL
  AND m.new_status = 'U';

-- Update listing number and status
UPDATE dss_rental_listing rl
SET platform_listing_no = m.listing_id,
    listing_status_type = 'U',
    upd_dtm = CURRENT_TIMESTAMP
FROM booking_id_map m
WHERE rl.platform_listing_no = m.hotel_id
  AND rl.offering_organization_id = 203
  AND rl.including_rental_listing_report_id IS NULL
  AND m.new_status = 'U';

----------------------------------------------------------------------------
-- Update listings as Deactivated for listings with new status 'D'
----------------------------------------------------------------------------
  -- Preview first
SELECT 
    rl.rental_listing_id,
    rl.platform_listing_no,
    m.hotel_id,
    m.new_status
FROM dss_rental_listing rl
INNER JOIN tmp_booking_id_map m
    ON rl.platform_listing_no = m.hotel_id
WHERE rl.offering_organization_id = 203
  AND rl.including_rental_listing_report_id IS NULL 
  AND m.new_status = 'D';

-- Deactivate 1:many listings
UPDATE dss_rental_listing rl
SET listing_status_type = 'D',
    upd_dtm = CURRENT_TIMESTAMP
FROM tmp_booking_id_map m
WHERE rl.platform_listing_no = m.hotel_id
  AND rl.offering_organization_id = 203
  AND rl.including_rental_listing_report_id IS NULL
  AND m.new_status = 'D';