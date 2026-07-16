-- Hotfix: restore is_takedown_action_suspended on dss_rental_listing_vw (Sprint 22 listing-actions view regression)
-- Run as strdssdev in DBeaver, or: psql -U strdssdev -d strdssdev -f database/ddl/STR_DSS_Views_Sprint_22_Listing_Actions.sql
--
-- The Sprint 22 view rebuild omitted drl.is_takedown_action_suspended (added in R14).
-- StrDss.Data.Entities.DssRentalListingVw maps this column; without it, listing detail API calls fail.

\ir '../ddl/STR_DSS_Views_Sprint_22_Listing_Actions.sql'
