/* Sprint R14 - Add new listing status types and reorder existing */

-- Change Inactive sort order from 3 to 5
UPDATE dss_listing_status_type
SET listing_status_sort_no = 5
WHERE listing_status_type = 'I';

-- Insert new listing status types: Updated and Deactivated
INSERT INTO dss_listing_status_type (listing_status_type, listing_status_type_nm, listing_status_sort_no)
VALUES ('U', 'Updated', 3);

INSERT INTO dss_listing_status_type (listing_status_type, listing_status_type_nm, listing_status_sort_no)
VALUES ('D', 'Deactivated', 4);
