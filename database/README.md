# Database Design and Implementation

## Objective:
This document describes the content of the **`database`** folder and how to use it. This supports the following processes:
- Database design and documentation
- DDL script generation
- Custom DDL scripting
- Data seeding
- Database release management

## Subfolder Structure:
The subfolders have the following content and uses:
- **`model`** - Source documents and generated artifacts for the conceptual, logical, and physical data models
- **`ddl`** - Data definition scripts to create or transform the database schema
- **`seeding`** - Data manipulation (DML) scripts to populate database tables with rows that are not managed by the application
- **`utility`** - SQL scripts to generate database output or perform special operations

## Database Design and Documentation
The physical database design is documented and maintained using **DBSchema**. These are stored as XML files with the **`.dbs`** extension.

Generated reports and images are published in https://hous-hpb.atlassian.net/wiki/spaces/DSS/pages/22708294/STR+DSS+Data+model .

## DDL Script Generation
Each sprint, a DDL script is produced to fully instantiate the database objects into an empty schema. In addition, incremental scripts are provided to transform the database structure from the preceding sprint.

## Custom DDL Scripting
Often, database object definitions are changed after the sprint modeling activities have completed. These can be recorded as standalone patch scripts, but will later be incorporated into the main physical model.

## Data Seeding
There is always a base set of fixed data that are not managed by the application, and must be populated by script. Some of these are merely temporary situations that will be resolved as stories evolve to replace them.

Some data is periodically refreshed from external sources, as described in https://hous-hpb.atlassian.net/wiki/spaces/DSS/pages/171638798/STR+DSS+Jurisdiction+Geometry+Refresh .

## Database Change Summaries
Each sprint delivers a planned set of incremental changes to the database. The following summarizes the most recent sets of changes. Older design change summaries are documented in https://hous-hpb.atlassian.net/wiki/spaces/DSS/pages/22708294/STR+DSS+Data+model . 

### Sprint 17:
- Create and populate lookup table `dss_economic_region`:
  - Columns `economic_region_dsc`, `economic_region_nm`, `economic_region_sort_no`

- Create and populate lookup table `dss_local_government_type`:
  - Columns `local_government_type`, `local_government_type_nm`, `local_government_type_sort_no`

- Alter table `dss_organization`:
  - Update values in columns `economic_region_dsc`, `local_government_type`, `is_str_prohibited`
  - Add foreign key to table `dss_economic_region`
  - Add foreign key to table `dss_local_government_type`
  - Add column: `business_licence_format_txt`

- Create view `dss_local_gov_vw`

- Seed new rows in table `dss_user_privilege` and `dss_user_role_privilege`:
  - `jurisdiction_read` (for `ceu_admin`, `ceu_staff`)
  - `jurisdiction_write` (for `ceu_admin`)
  - `bl_link_write` (for `lg_staff`)

### Sprint 18:
- Replace procedures:
  - `dss_process_biz_lic_table_delete`
  - `dss_process_biz_lic_table_update`

- Alter table `dss_business_licence`:
  - Replace index `dss_business_licence_i4`

- Alter table `dss_rental_listing`:
  - Replace index `dss_rental_listing_i10`

- Insert new row in table `dss_business_licence_status_type`

- Update rows in table `dss_rental_listing`

### Sprint 19:
- Alter table `dss_organization`:
  - Add column: `is_straa_exempt`
  - Add column: `source_attributes_json`

## Database Release Management
Each production release depends on the execution of a fixed set of scripts against the database schema. Generally, these are applied in the same order as they were applied to the UAT database during each sprint. The following is the order list of scripts applied to deliver each release.

_Note: Master scripts are the preferred release method to use, starting with Release 5. To use one, connect to the target database from the `psql` command line, and execute the following command:_

`\i '<folder>/<script>' (must use / rather than \)`

### Release 1 Scripts:
- Sprint 1: N/A

- Sprint 2:
  - `ddl/STR_DSS_Physical_DB_DDL_Sprint_2.sql`
  - `ddl/STR_DSS_Views_Sprint_2.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_2.sql`

- Sprint 3:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_3.sql`

- Sprint 4:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_4.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_4.sql`
  - `seeding/STR_DSS_Data_Seeding_PRODONLY_Sprint_4.sql` **(PRD ONLY) OR** `seeding/STR_DSS_Data_Seeding_NONPROD_Sprint_4.sql`
  - `STR_DSS_Data_Seeding_PRODONLY_PRIVATE_Sprint_4.sql` **(PRD ONLY - not in repo)**

### Release 2 Scripts:
- Sprint 5:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_5.sql`
  - `ddl/STR_DSS_Views_Sprint_5.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_5.sql`

- Sprint 6:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_6.sql`
  - `ddl/STR_DSS_Functions_Sprint_6.sql`
  - `ddl/STR_DSS_Views_Sprint_6.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_6.sql`
  - `seeding/STR_DSS_Data_Seeding_LGs_Sprint_6.sql`
  - `seeding/STR_DSS_Data_Seeding_Geometry_Sprint_6.sql`
  - `seeding/STR_DSS_Data_Seeding_Platforms_Sprint_6.sql`
  - `STR_DSS_Data_Seeding_PRODONLY_PRIVATE_Sprint_6.sql` **(PRD ONLY - not in repo) OR** `seeding/STR_DSS_Data_Seeding_NONPROD_Sprint_6.sql`

### Release 3 Scripts:
- Sprint 7:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_7.sql`
  - `seeding/STR_DSS_Data_Seeding_LGs_Sprint_7.sql`

- Sprint 8:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_8.sql`
  - `ddl/STR_DSS_Views_Sprint_8.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_8.sql`

### Release 4 Scripts:
- Sprint 9:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_9.sql`
  - `ddl/STR_DSS_Views_Sprint_9.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_9.sql`
  - `seeding/STR_DSS_Data_Seeding_Geometry_Sprint_9.sql` **(TIP: run each MERGE statement independently)**
  - `utility/DSS-601_Correct_Rental_Listings_Sprint_9.sql`

### Release 5 Scripts:
- Master Scripts _(preferred release method)_:
  - `utility/STR_DSS_Migration_Sprint_11.sql`
  - `utility/STR_DSS_Migration_Sprint_12.sql`
  - `utility/STR_DSS_Migration_Sprint_13.sql`

- Sprint 10:
  - `seeding/STR_DSS_Data_Seeding_Platforms_Sprint_10.sql`
  - `STR_DSS_Data_Seeding_PRODONLY_PRIVATE_Sprint_10.sql` **(PRD ONLY - not in repo) OR** `seeding/STR_DSS_Data_Seeding_NONPROD_Sprint_6.sql` **(not a typo)**

- Sprint 11:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_11.sql`
  - `ddl/STR_DSS_Views_Sprint_9.sql` **(not a typo)**
  - `ddl/STR_DSS_Views_Sprint_11.sql`
  - `seeding/STR_DSS_Data_Seeding_LGs_Sprint_11.sql`
  - `utility/DSS-627_Correct_UPLOAD_DELIVERY_Sprint_11.sql`

- Sprint 12:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_12.sql`
  - `ddl/STR_DSS_Views_Sprint_12.sql`
  - `ddl/STR_DSS_Routines_Sprint_12.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_12.sql`
  - `seeding/STR_DSS_Data_Seeding_Geometry_Sprint_12.sql` **(TIP: run each MERGE statement independently)**
  - `seeding/STR_DSS_Data_Seeding_LGs_Sprint_12.sql` **(includes data corrections)**

- Sprint 13:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_13.sql`
  - `ddl/STR_DSS_Views_Sprint_13.sql`
  - `ddl/STR_DSS_Routines_Sprint_13.sql`
  - `utility/Correct_Rental_Listings_Sprint_13.sql`

### Release 6 Scripts:
- Master Scripts _(preferred release method)_:
  - `utility/STR_DSS_Migration_Sprint_14.sql`
  - `utility/STR_DSS_Migration_Sprint_15.sql`
  - `utility/STR_DSS_Migration_Sprint_16.sql`
  - `utility/STR_DSS_Migration_Sprint_17.sql`

- Sprint 14:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_14.sql`
  - `ddl/STR_DSS_Views_Sprint_14.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_14.sql`
  - `utility/Correct_Rental_Listings_Sprint_14.sql`

- Sprint 15:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_15.sql`
  - `ddl/STR_DSS_Views_Sprint_15.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_15.sql`
  - `seeding/STR_DSS_Data_Seeding_Platforms_Sprint_15.sql`

- Sprint 16:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_16.sql`
  - `ddl/STR_DSS_Views_Sprint_16.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_16.sql`
  - `seeding/STR_DSS_Data_Seeding_LGs_Sprint_16.sql`
  - `seeding/STR_DSS_Data_Seeding_Geometry_Sprint_16.sql` **(TIP: run each MERGE statement independently)**

- Sprint 17:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_17_pre_DML.sql`
  - `ddl/STR_DSS_Views_Sprint_17.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_17.sql`
  - `seeding/STR_DSS_Data_Seeding_LGs_Sprint_17.sql`
  - `seeding/STR_DSS_Data_Seeding_Geometry_Sprint_17.sql` **(TIP: run each MERGE statement independently)**
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_17_post_DML.sql`

### Release 7 Scripts:
- Master Scripts _(preferred release method)_:
  - `utility/STR_DSS_Migration_Sprint_18.sql`
  - `utility/STR_DSS_Migration_Sprint_19.sql`

- Sprint 18:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_18.sql`
  - `ddl/STR_DSS_Routines_Sprint_18.sql`
  - `seeding/STR_DSS_Data_Seeding_Sprint_18.sql`
  - `utility/Correct_Rental_Listings_Sprint_18.sql`

- Sprint 19:
  - `ddl/STR_DSS_Incremental_DB_DDL_Sprint_19.sql`
  - `ddl/STR_DSS_Functions_Sprint_19.sql`
  - `seeding/STR_DSS_Data_Seeding_LGs_Sprint_19.sql`
  - `seeding/STR_DSS_Data_Seeding_Geometry_Sprint_19.sql`