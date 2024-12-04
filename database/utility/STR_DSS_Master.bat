rem This script must be run from the House-Policy-STRDSS/database/utility folder
rem by replacing 'rem' with '#', this file will run with /bin/sh under Linux
docker cp ../database/utility house-policy-strdss-strdss-db-1:/utility
docker cp ../database/ddl house-policy-strdss-strdss-db-1:/ddl
docker cp ../database/seeding house-policy-strdss-strdss-db-1:/seeding
docker exec -i house-policy-strdss-strdss-db-1 psql -U postgres  -f utility/STR_DSS_Database_Create.sql
rem The SQL script below must be the correct sprint migration script for the current sprint (i.e. STR_DSS_Migration_Sprint_<SprintNumber>.sql)
docker exec -i house-policy-strdss-strdss-db-1 psql -U postgres  -d strdssdev -f utility/STR_DSS_Migration_Sprint_18.sql
