# Local database initialization

Before running `server.AppHost` with a fresh local database, copy `strdssdev.sql.gz` (or an uncompressed `.sql` file) into this directory. Obtain the dump through an approved channel; database dumps are intentionally excluded from Git and must not be committed.

AppHost mounts this directory read-only at `/docker-entrypoint-initdb.d` in the PostGIS container. PostgreSQL automatically executes `.sql` and `.sql.gz` files in filename order when it initializes an empty data directory. Do not copy both compressed and uncompressed copies of the same script, since both would run.

The default database and username are both `strdssdev`. The local default password remains `strdss`. Make sure the dump is compatible with PostgreSQL 18/PostGIS 3.6 and these database/role names. Existing AppHost parameter overrides still take precedence.

## Existing databases

Initialization scripts do not run again when the `strdss-postgis-data` volume already contains an initialized database. Changing the database/user defaults does not rename existing databases or roles.

For a disposable local database, stop AppHost and remove its container if Docker reports that the volume is in use, then run:

```powershell
docker volume rm strdss-postgis-data
```

**This permanently deletes all data in that volume.** Copy the dump into this directory before restarting AppHost. Preserve or back up any data you need instead of deleting the volume.

If initialization fails partway through, inspect the container logs and correct the script. A restart may not retry initialization because the data directory has already been initialized; a disposable database may need the same volume reset.

External-database mode does not mount or execute these scripts.
