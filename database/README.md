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

Generated reports and images are published in https://hous-hpb.atlassian.net/wiki/spaces/DSS/pages/22708294/STR+DSS+Data+model

## DDL Script Generation
Each sprint, a DDL script is produced to fully instantiate the database objects into an empty schema. In addition, incremental scripts are provided to transform the database structure from the preceding sprint.

## Custom DDL Scripting
Often, database object definitions are changed after the sprint modeling activities have completed. These can be recorded as standalone patch scripts, but will later be incorporated into the main physical model.

## Data Seeding
There is always a base set of fixed data that are not managed by the application, and must be populated by script. Some of these are merely temporary situations that will be resolved as stories evolve to replace them.

## Database Release Management
Each production release depends on the execution of a fixed set of scripts against the database schema. Generally, these are applied in the same order as they were applied to the UAT database during each sprint. The following is the order list of scripts applied to deliver each release.
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