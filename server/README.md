# Environment Variables

For actual values, refer to the [gitops repo](https://github.com/bcgov-c/tenant-gitops-b0471a/tree/develop/deploy) or the [vault](https://vault.developer.gov.bc.ca/ui/vault/auth?namespace=platform-services&with=oidc)

## Keycloak

* SSO_AUTHORITY:
* SSO_CLIENT:

## Common Hosted Email Service (CHES)

* CHES_TOKEN_URL:
* CHES_URL:
* CHES_ID:
* CHES_SECRET:

## Database

* DB_NAME:
* DB_HOST:
* DB_USER:
* DB_PASS:
* DB_PORT:

### AppHost local PostGIS database

By default, `server.AppHost` starts `postgis/postgis:18-3.6` in Docker and gives the API and Hangfire projects these database settings:

| Setting | Default |
| --- | --- |
| Database | `strdssdev` |
| User | `strdssdev` |
| Password | `strdss` |
| Port | Dynamically assigned by Aspire |
| Docker volume | `strdss-postgis-data` |

Before starting AppHost with a fresh local database, copy `strdssdev.sql.gz` (or an uncompressed `.sql` file) into `server/server.AppHost/database-init/` relative to the repository root. SQL dumps in that folder are ignored by Git and must not be committed. See [the initialization instructions](server.AppHost/database-init/README.md).

AppHost mounts that folder read-only at `/docker-entrypoint-initdb.d`. PostgreSQL creates the database and user and automatically runs the scripts in filename order **only when the database data directory is empty**. The volume persists the database between AppHost runs, so scripts are not rerun on subsequent starts. Wait for initialization to finish in the container logs before using the database.

Existing volumes retain their original databases and roles; changing AppHost defaults does not rename them. Existing parameter overrides, including user-secrets, still take precedence over these defaults.

### AppHost service selection

AppHost services can be enabled independently:

| Setting | Default | Resource |
| --- | --- | --- |
| `AppHost:Services:Api:Enabled` | `true` | STR DSS API project |
| `AppHost:Services:Frontend:Enabled` | `true` | Dockerized frontend |
| `AppHost:Services:Hangfire:Enabled` | `false` | Hangfire project |
| `AppHost:Services:Postgres:Mode` | `Aspire` | `Aspire` hosts local PostGIS; `External` uses a developer-supplied database |

Set these in `server.AppHost/appsettings.json`, the selected AppHost launch profile, or configuration overrides. For example, to use Angular's development server instead of building and launching the frontend container, set `AppHost:Services:Frontend:Enabled` to `false` and run `npm run dev` from `frontend`. The frontend development proxy in `frontend/proxy.conf.json` targets `http://127.0.0.1:5174`.

These keys replace the previous `AppHost:Launch*` and database selection settings; update any existing user-secrets or environment variables to use the new names. PostgreSQL is required by the API and Hangfire, so select its hosting mode rather than disabling the service.

To run only the API against an external PostgreSQL/PostGIS database, set `AppHost:Services:Api:Enabled` to `true`, `AppHost:Services:Frontend:Enabled` and `AppHost:Services:Hangfire:Enabled` to `false`, and `AppHost:Services:Postgres:Mode` to `External`. Configure the external database parameters as described below. AppHost will not create the local PostGIS container in external mode.

If the Dockerized frontend is enabled while the API is disabled, configure `AppHost:Services:Frontend:ApiUrl` with an API URL reachable from the frontend container. AppHost requires this setting in that configuration.

#### Alternative: repository schema migration

If you are not initializing from a dump, the repository migration scripts can be run manually instead. Do not also run this full initialization migration over a restored dump unless you have verified it is appropriate for that dump's schema version.

From the repository root, start `server.AppHost` in Visual Studio and wait for PostgreSQL to accept connections. Then run the following in PowerShell:

```powershell
$containerId = docker ps --filter "ancestor=postgis/postgis:18-3.6" --format "{{.ID}}" | Select-Object -First 1
if (-not $containerId) { throw "The AppHost PostGIS container is not running." }
docker exec $containerId rm -rf /database
docker cp ./database "${containerId}:/database"
docker exec $containerId psql -v ON_ERROR_STOP=1 -U strdssdev -d strdssdev -f /database/utility/STR_DSS_Migration_Sprint_22.sql
```

`STR_DSS_Migration_Sprint_22.sql` is the master migration. On an empty database it runs the full Sprint 22 schema and seed scripts. It can also detect an already initialized Sprint 22 database and exit without changes. See [the database documentation](../database/README.md) for migration details.

To verify that the schema was created:

```powershell
docker exec $containerId psql -U strdssdev -d strdssdev -c "SELECT to_regclass('public.dss_user_role');"
```

#### Resetting the local database

To rebuild the local database from scratch, stop AppHost and remove its container if Docker reports that the volume is in use, then remove the persistent volume:

```powershell
docker volume rm strdss-postgis-data
```

Removing this volume permanently deletes the local database data. Before restarting AppHost, put the desired dump in `server/server.AppHost/database-init/` for automatic initialization, or leave the folder without SQL files and use the manual migration alternative. If an initialization script fails, a restart may not retry it; inspect the logs and correct the script before resetting a disposable volume again.

### AppHost external PostgreSQL database

AppHost can use another reachable PostgreSQL/PostGIS instance instead of creating the local container. Store the selection and connection fields in the `server.AppHost` user-secrets store so they are not committed to source control. The fields are read from `AppHost:Services:Postgres:External` and passed to the API and Hangfire as `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, and `DB_PASS`:

```powershell
Set-Location ./server/server.AppHost
dotnet user-secrets set "AppHost:Services:Postgres:Mode" "External"
dotnet user-secrets set "AppHost:Services:Postgres:External:Host" "database.example.gov.bc.ca"
dotnet user-secrets set "AppHost:Services:Postgres:External:Port" "5432"
dotnet user-secrets set "AppHost:Services:Postgres:External:Database" "strdssuat"
dotnet user-secrets set "AppHost:Services:Postgres:External:User" "strdssuat"
dotnet user-secrets set "AppHost:Services:Postgres:External:Password" "<password>"
```

The host, database, user, and password fields are required in external mode; port defaults to `5432`. The password is registered with Aspire as a secret parameter. Replace any previous `Parameters:postgres-*` external connection secrets with the `AppHost:Services:Postgres:External:*` keys above.

Restart AppHost after changing these values. In external mode, AppHost does not start a `postgis` container and does not initialize or migrate the target database. The target must:

- Be reachable from the developer machine.
- Have PostGIS enabled.
- Have a schema version compatible with the current application.
- Allow the configured account to perform any writes made by the API or Hangfire.

Take particular care when targeting UAT: local API actions change UAT data. Hangfire remains disabled by default through `AppHost:Services:Hangfire:Enabled` and should not be enabled against a shared database unless its jobs are intentionally required.

To return to the local Docker database, set the mode to `Aspire` or remove the mode secret, then restart AppHost:

```powershell
dotnet user-secrets set "AppHost:Services:Postgres:Mode" "Aspire"
```

Unused external connection secrets may also be removed with `dotnet user-secrets remove "AppHost:Services:Postgres:External:<field>"`.

## Maximum File Size in MB

* RENTAL_LISTING_REPORT_MAX_SIZE:
* BUISINESS_LICENCE_MAX_SIZE:
* VALIDATE_REGISTRATION_MAX_SIZE:

## Service Account for BCeID web service

* SA_USER:
* SA_PASS:

## BCeID web service

* BCEID_URL:
* BCEID_OSID:
* BCEID_CACHE_LIFESPAN:

## App base URL for hyper link in email

* APP_BASE_URL:

## Email Addresses

* ADMIN_EMAIL:
* STR_NOTICE_EMAIL:
* STR_DATA_EMAIL:
* STR_CEU_EMAIL:

## Geocoder

* GEOCODER_URL:
* GEOCODER_API_KEY:

## API Gateway

* APS_AUTHORITY:
* APS_GW_JWT_JWKS_URL:
