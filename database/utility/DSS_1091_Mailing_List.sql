SELECT DISTINCT ON (drl.locating_physical_address_id)
    trim(regexp_replace(full_nm, '\s+\S+$', '')) AS FIRSTNAME,
    NULL AS INITIAL,
    split_part(full_nm, ' ', array_length(string_to_array(full_nm, ' '), 1)) AS LASTNAME,
    NULL AS TITLE,
    NULL AS POSITION,
    NULL AS COMPANY_NAME,
    NULL AS BRANCH_DIVISION,
    NULL AS Pre_Street,
    dpa.unit_no || ' ' || dpa.civic_no || ' ' || dpa.street_nm || ' ' || dpa.street_type_dsc || ' ' || dpa.street_direction_dsc AS ADDRESS_1,
    dpa.original_address_txt AS ADDRESS_2,
    dpa.locality_nm AS CITY,
    dpa.province_cd AS Province,
    CASE 
        WHEN dpa.original_address_txt ~ 'BC,?\s*[A-Za-z0-9\s]+,' THEN
            regexp_replace(dpa.original_address_txt, '.*BC,?\s*([A-Za-z0-9\s]+),.*', '\1')
        ELSE NULL
    END AS POSTALCODE,
    'Canada' AS Country,	
    NULL AS QUANTITY,
    NULL AS Merge_Data_1,
    NULL AS Merge_data_2
FROM dss_rental_listing_contact drlc
LEFT JOIN dss_rental_listing drl ON drlc.contacted_through_rental_listing_id = drl.rental_listing_id
LEFT JOIN dss_physical_address dpa ON drl.locating_physical_address_id = dpa.physical_address_id
ORDER BY drl.locating_physical_address_id, full_nm;

SELECT DISTINCT
    trim(regexp_replace(drlc.full_nm, '\s+\S+$', '')) AS FIRSTNAME,
    NULL AS INITIAL,
    split_part(drlc.full_nm, ' ', array_length(string_to_array(drlc.full_nm, ' '), 1)) AS LASTNAME,
    NULL AS TITLE,
    NULL AS POSITION,
    NULL AS COMPANY_NAME,
    NULL AS BRANCH_DIVISION,
    NULL AS Pre_Street,
    dpa.unit_no || ' ' || dpa.civic_no || ' ' || dpa.street_nm || ' ' || dpa.street_type_dsc || ' ' || dpa.street_direction_dsc AS ADDRESS_1,
    dpa.original_address_txt AS ADDRESS_2,
    dpa.locality_nm AS CITY,
    dpa.province_cd AS Province,
    CASE 
        WHEN dpa.original_address_txt ~ 'BC,?\s*[A-Za-z0-9\s]+,' THEN
            regexp_replace(dpa.original_address_txt, '.*BC,?\s*([A-Za-z0-9\s]+),.*', '\1')
        ELSE NULL
    END AS POSTALCODE,
    'Canada' AS Country,	
    NULL AS QUANTITY,
    NULL AS Merge_Data_1,
    NULL AS Merge_data_2
FROM dss_physical_address dpa
LEFT JOIN dss_rental_listing drl ON dpa.physical_address_id = drl.locating_physical_address_id
LEFT JOIN dss_rental_listing_contact drlc ON drl.rental_listing_id = drlc.contacted_through_rental_listing_id



SELECT DISTINCT ON (dpa.physical_address_id)
    trim(regexp_replace(drlc.full_nm, '\s+\S+$', '')) AS FIRSTNAME,
    NULL AS INITIAL,
    split_part(drlc.full_nm, ' ', array_length(string_to_array(drlc.full_nm, ' '), 1)) AS LASTNAME,
    NULL AS TITLE,
    NULL AS POSITION,
    NULL AS COMPANY_NAME,
    NULL AS BRANCH_DIVISION,
    NULL AS Pre_Street,
    dpa.unit_no || ' ' || dpa.civic_no || ' ' || dpa.street_nm || ' ' || dpa.street_type_dsc || ' ' || dpa.street_direction_dsc AS ADDRESS_1,
    dpa.original_address_txt AS ADDRESS_2,
    dpa.locality_nm AS CITY,
    dpa.province_cd AS Province,
    CASE 
        WHEN dpa.original_address_txt ~ 'BC,?\s*[A-Za-z0-9\s]+,' THEN
            regexp_replace(dpa.original_address_txt, '.*BC,?\s*([A-Za-z0-9\s]+),.*', '\1')
        ELSE NULL
    END AS POSTALCODE,
    'Canada' AS Country,	
    NULL AS QUANTITY,
    NULL AS Merge_Data_1,
    NULL AS Merge_data_2
FROM dss_physical_address dpa
JOIN dss_rental_listing drl ON dpa.physical_address_id = drl.locating_physical_address_id
LEFT JOIN dss_rental_listing_contact drlc ON drl.rental_listing_id = drlc.contacted_through_rental_listing_id
ORDER BY dpa.physical_address_id, drlc.full_nm;

