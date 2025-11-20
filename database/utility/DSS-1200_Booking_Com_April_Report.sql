select *
from dss_upload_delivery
where upload_delivery_type = 'Listing Data' and upload_lines_total = 2638

select count(*)
from dss_rental_listing
where including_rental_listing_report_id = 786

select *
from dss_rental_listing_report d 
where d.providing_organization_id = 203

select *
from dss_organization d 
where d.organization_id = 203


select *
from dss_rental_upload_history_view d 
where d.upload_delivery_id = 786


select *
from dss_upload_line d 
where d.including_upload_delivery_id = 786 and d.is_system_failure = true

select d.offering_organization_id, d.including_rental_listing_report_id, d2.report_period_ym,  d.nights_booked_qty, d.separate_reservations_qty 
from dss_rental_listing d, dss_rental_listing_report d2  
where d.including_rental_listing_report_id = d2.rental_listing_report_id 
and d.platform_listing_no = '4268485'
--where d.bc_registry_no = 'H727416135'

select d.offering_organization_id, d.including_rental_listing_report_id, d2.report_period_ym,  d.nights_booked_qty, d.separate_reservations_qty, d.upd_dtm, d2.upd_dtm
from dss_rental_listing d, dss_rental_listing_report d2  
where d.including_rental_listing_report_id = d2.rental_listing_report_id 
and d.including_rental_listing_report_id = 189
and d.platform_listing_no = '4268485'
order by d.upd_dtm ASC
and d.nights_booked_qty is not null

select d.offering_organization_id, d.including_rental_listing_report_id, d2.report_period_ym,  d.nights_booked_qty, d.separate_reservations_qty, d.upd_dtm, d2.upd_dtm
from dss_rental_listing d, dss_rental_listing_report d2  
where d.including_rental_listing_report_id = d2.rental_listing_report_id 
and d.including_rental_listing_report_id != 189
and d.nights_booked_qty is null
and d.platform_listing_no = '4268485'
order by d.upd_dtm ASC



select *
from dss_upload_delivery
where upload_delivery_id = 786


select *
from dss_upload_line
where including_upload_delivery_id = 786
and source_line_txt like '%4268485%'

-- Script to parse out values and update the rentel listing with the nights and reservations:
do $$
declare
	rec record;
begin
for rec in
	select ul.upload_line_id,
	       arr[1] as rpt_period,
	       arr[2] as org_cd,
	       arr[3] as listing_id,
           COALESCE(NULLIF(trim(arr[12]), '')::int, 0) AS nights_booked,
           COALESCE(NULLIF(trim(arr[14]), '')::int, 0) AS reservation_qty
	--into v_rpt_period, v_org_cd, v_listing_id, v_nights_booked, v_reservation_qty
	from dss_upload_line ul
	cross join lateral string_to_array(ul.source_line_txt, ',') as arr
	where ul.including_upload_delivery_id = 786

loop
    update dss_rental_listing rl
    set nights_booked_qty = rec.nights_booked,
        separate_reservations_qty = rec.reservation_qty,
        upd_dtm = now()
    WHERE rl.platform_listing_no = rec.listing_id
      AND rl.including_rental_listing_report_id = 189;
end loop;

end $$;


-- Robust CSV line parser (handles quoted fields and embedded commas, double quote escapes "")
CREATE OR REPLACE FUNCTION util_parse_csv_line(line text, delim text DEFAULT ',')
RETURNS text[] LANGUAGE plpgsql IMMUTABLE AS $$
DECLARE
  result text[] := ARRAY[]::text[];
  i int := 1;
  len int := length(line);
  c text;
  in_quotes boolean := false;
  cur text := '';
BEGIN
  WHILE i <= len LOOP
    c := substr(line, i, 1);
    IF in_quotes THEN
      IF c = '"' THEN
        -- Escaped quote ("")
        IF i < len AND substr(line, i+1, 1) = '"' THEN
          cur := cur || '"';
          i := i + 1;
        ELSE
          in_quotes := false;
        END IF;
      ELSE
        cur := cur || c;
      END IF;
    ELSE
      IF c = '"' THEN
        in_quotes := true;
      ELSIF c = delim THEN
        result := result || cur;
        cur := '';
      ELSE
        cur := cur || c;
      END IF;
    END IF;
    i := i + 1;
  END LOOP;
  result := result || cur;
  RETURN result;
END $$;

-- Preview: verify field positions (adjust indexes if needed)
WITH parsed AS (
  SELECT ul.upload_line_id,
         flds,
         cardinality(flds) AS field_count
  FROM dss_upload_line ul
  CROSS JOIN LATERAL util_parse_csv_line(ul.source_line_txt) AS flds
  WHERE ul.including_upload_delivery_id = 786
  LIMIT 5
)
SELECT * FROM parsed;

-- Dry-run list of affected listings (ensure array has enough fields)
WITH parsed AS (
  SELECT ul.upload_line_id,
         flds,
         flds[1]  AS rpt_period,
         flds[2]  AS org_cd,
         flds[3]  AS listing_id,
         flds[12] AS nights_raw,
         flds[14] AS reservations_raw
  FROM dss_upload_line ul
  CROSS JOIN LATERAL util_parse_csv_line(ul.source_line_txt) AS flds
  WHERE ul.including_upload_delivery_id = 786
    AND cardinality(flds) >= 14           -- ensure expected width
    AND ul.is_system_failure = false
),
clean AS (
  SELECT listing_id,
         CASE WHEN nights_raw ~ '^[0-9]+$' THEN nights_raw::int ELSE 0 END AS nights_booked,
         CASE WHEN reservations_raw ~ '^[0-9]+$' THEN reservations_raw::int ELSE 0 END AS reservation_qty
  FROM parsed
  WHERE listing_id IS NOT NULL
),
targets AS (
  SELECT rl.rental_listing_id,
         rl.platform_listing_no,
         rl.nights_booked_qty AS current_nights,
         rl.separate_reservations_qty AS current_reservations,
         c.nights_booked AS new_nights,
         c.reservation_qty AS new_reservations
  FROM clean c
  JOIN dss_rental_listing rl
    ON rl.platform_listing_no = c.listing_id
   AND rl.including_rental_listing_report_id = 189
   AND rl.is_current = true
)
SELECT *
FROM targets
WHERE (new_nights <> current_nights OR new_reservations <> current_reservations)
ORDER BY platform_listing_no;