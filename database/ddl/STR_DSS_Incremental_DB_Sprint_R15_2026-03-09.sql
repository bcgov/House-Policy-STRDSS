/* Sprint R15 - Indexes for GetRentalListings / count query performance
   Targets the base-table listings query and count used by the rental listings table.
   Partial index on current listings (including_rental_listing_report_id IS NULL) reduces
   work for the main table and count endpoints. */

-- Partial index on current listings: supports base filter and common join/filter columns
CREATE INDEX IF NOT EXISTS dss_rental_listing_i12
ON dss_rental_listing (offering_organization_id, locating_physical_address_id, listing_status_type)
WHERE including_rental_listing_report_id IS NULL;

-- Partial index for status / reassigned / takedown filters
CREATE INDEX IF NOT EXISTS dss_rental_listing_i13
ON dss_rental_listing (listing_status_type, is_taken_down, is_lg_transferred)
WHERE including_rental_listing_report_id IS NULL;
