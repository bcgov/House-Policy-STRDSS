# Data integration tests

These .NET 8/xUnit tests execute repository queries against real PostgreSQL using Testcontainers. The `postgis/postgis:18-3.6` image matches the local AppHost database and supports the spatial types mapped by `DssDbContext`.

The fixture mounts local SQL scripts from `database-init`. Those scripts are not in Git today. Generate them from a development database, remove personal information, and only then check them in. Production data is never a source for this folder.

## Prerequisites

- A .NET SDK capable of building .NET 8 projects.
- Docker running with Linux containers (for example, Docker Desktop).
- Access to pull the PostGIS image on the first run. Testcontainers also manages its resource-cleanup container.
- Numbered `.sql` scripts in `database-init`, produced by `database-init/Export-DatabaseInit.ps1` (see below). The project copies `*.sql` and `*.sql.gz` to the build output and the fixture mounts that output directory.

No AppHost process has to stay running while the tests execute. Testcontainers chooses an available host port and starts its own disposable container. Tests fail rather than silently skip when Docker is unavailable or when initialization SQL fails.

## Run

From the repository root:

```powershell
dotnet test server/StrDss.Data.IntegrationTest/StrDss.Data.IntegrationTest.csproj
```

Alternatively, build the solution and run `StrDss.Data.IntegrationTest` in Visual Studio Test Explorer. Sample tests have the `Category=Integration` trait. Running the whole solution's tests also runs this project and therefore requires Docker.

## Initialization scripts

`database-init/.gitignore` ignores `*.sql` and `*.sql.gz`, so a dump cannot be committed by accident. PostgreSQL runs every `.sql` and `.sql.gz` file in this folder, in filename order, the first time the container data directory is created. Do not leave a compressed copy next to the same script.

`Export-DatabaseInit.ps1` splits a Postgres database into numbered plain-SQL files. The source is either the Aspire PostGIS container (`postgis/postgis:18-3.6`, database and role `strdssdev`) or an external development server. File numbers advance by 10 inside each band so a later script can be inserted without renaming the set. Set `Number` on an export entry to claim a specific slot.

| Band | Contents |
| --- | --- |
| `000-099` | Schema setup. The default entry is `000-pre-data.sql` (`pg_dump --section=pre-data`): schemas, PostGIS, tables, views, functions, procedures, and the initially unpopulated latest-report materialized view. |
| `100-799` | Data loads, `--data-only`, one file per entry. Each `Tables` array is passed to `pg_dump` as repeated `-t` arguments. A very large table stays in its own entry. |
| `800-999` | Post-data, materialized view refresh, and maintenance. `800-post-data.sql` is constraints, indexes, and triggers. Each populated materialized view then gets its own file, in dependency order, for example `810-refresh-public.dss_rental_listing_latest_report_mv.sql`. `ANALYZE` is the last file in the band. |

With the default `$dataExports` list, the data files are:

| File | Tables |
| --- | --- |
| `100-reference-data.sql` | Code and privilege tables: access-request status, licence status, economic region, email message type, listing action and status, local-government type, organization type, platform type, user privilege, user role, and user-role privilege. |
| `110-users.sql` | `dss_user_identity`, `dss_user_role_assignment` |
| `120-dss_organization.sql` | `dss_organization` |
| `130-organization-details.sql` | `dss_organization_contact_person`, `dss_business_licence` |
| `140-dss_physical_address.sql` | `dss_physical_address` |
| `150-listing-reports.sql` | `dss_rental_listing_report`, `dss_email_message`, `dss_rental_listing_extract`, `dss_rental_listing_action` |
| `160-dss_rental_listing.sql` | `dss_rental_listing` |
| `170-dss_rental_listing_contact.sql` | `dss_rental_listing_contact` |
| `180-dss_upload_delivery.sql` | `dss_upload_delivery` |
| `190-dss_upload_line.sql` | `dss_upload_line` |

`public.spatial_ref_sys` is excluded because `CREATE EXTENSION` already loads it. The script lists every other user table in the database and warns when one is missing from `$dataExports`. Dumps use `--no-owner` and `--no-acl`. Edit the three export arrays in the script when the source schema gains or drops a table, then export again.

On each run the script also queries the source database for populated materialized views. It writes one `REFRESH MATERIALIZED VIEW` file per view, after `800-post-data.sql` and before `ANALYZE`. A view that reads another materialized view is refreshed after that view. The post-data dump uses `--exclude-table-data` for those views so initialization refreshes each one once. Views that are not populated, including a populated view that reads an unpopulated materialized view, stay empty. Ordinary views need no refresh. Sequence values are already in the table data files.

### Export from the Aspire container

Start AppHost so the local PostGIS container is running and initialized, then from `database-init`:

```powershell
$container = docker ps --filter "ancestor=postgis/postgis:18-3.6" --format "{{.Names}}" | Select-Object -First 1
if (-not $container) { throw "The AppHost PostGIS container is not running." }
.\Export-DatabaseInit.ps1 -ContainerName $container
```

`-Username` and `-Database` both default to `strdssdev`. Pass `-Password` when the role inside the container does not authenticate locally without one; otherwise `pg_dump` uses the container's local authentication, or `PGPASSWORD` if it is already set. `-WhatIf` prints the planned commands and does not write files. `-OutputDirectory` defaults to the script folder.

### Export from an external database

Pass `-Server` instead of `-ContainerName`. The script then runs `pg_dump` and `psql` on this machine. Both tools must be on `PATH`, and the client major version must be the same as the server or newer. `-Port` defaults to 5432. Hosted Postgres often needs `-SslMode require`. `-Password` sets `PGPASSWORD` for the run; a password already in the environment or in the local `pgpass` file is also used.

```powershell
.\Export-DatabaseInit.ps1 -Server dev-db.example.com -Username strdssdev -Database strdssdev -Password $env:PGPASSWORD -SslMode require
```

Both paths write the same file layout from the same table list. Use a development database only. Do not point the export at production.

Remove numbered `.sql` files this run did not rewrite. Initialization executes every match, including an older `001-pre-data.sql`, `002-data.sql`, or `003-post-data.sql` left beside the new set. The script warns about those leftovers. Clean the test project after adding, renaming, or deleting scripts so the mounted build output does not keep a stale file.

### Personal information, then check-in

A raw export is a copy of whatever is in the source database. Treat it as local-only until it has been reviewed. These data files carry personal information and must be rewritten with synthetic values before they are committed:

- `110-users.sql` — display, given, and family names, email, business name, and access-request justification on `dss_user_identity`.
- `130-organization-details.sql` — contact names, phones, and emails; licence-holder names, phones, emails, and mailing addresses on `dss_business_licence`.
- `140-dss_physical_address.sql` — street addresses, match text, match JSON, and coordinates.
- `150-listing-reports.sql` — host, local-government, and copy-recipient emails, phone numbers, and message text on `dss_email_message`.
- `160-dss_rental_listing.sql` — host names, listing URLs, and published licence or registry numbers.
- `170-dss_rental_listing_contact.sql` — names, phones, fax numbers, mailing addresses, and emails.
- `190-dss_upload_line.sql` — `source_line_txt` is the original upload line and can contain the same host and address fields.

Keep primary keys, foreign keys, and sequence values intact so the loaded baseline still satisfies the queries under test. Reference rows in `100-reference-data.sql` are code tables; still read them before committing. `dss_organization` rows are mostly local governments and platforms; review contact-related columns and `source_attributes_json` in the same pass.

After that review:

1. Confirm the source was a development database. Production data is never committed, including a scrubbed extract of production.
2. Search the scripts for real names, email addresses, phone numbers, street addresses, and licence-holder details.
3. Narrow `database-init/.gitignore` so the reviewed `.sql` files can be added. Keep ignoring `*.sql.gz` so a compressed raw dump cannot be committed beside them.
4. Update test assertions that depend on the new baseline in the same change.

These tests load the selected local baseline into a disposable container. They do not upgrade a production database, and the fixture must not be pointed at an external database.

## Add query tests

1. Derive the test class from `DatabaseTestBase`.
2. Add `[Collection(PostgresCollection.Name)]` and `[Trait("Category", "Integration")]`.
3. Accept `PostgresFixture` in the constructor and pass it to the base constructor.
4. Identify the expected records in the `100-799` data scripts; do not insert replacement data for repository query tests.
5. Create a read context with `CreateDbContext()` (optionally passing EF interceptors) and dispose it using `await using`. Construct the real repository with `Mapper`, `CurrentUser`, and a logger, execute its query, and assert its execution and behavioral contracts using the initialized records.

`CodeSetRepositoryShould` demonstrates successful database-reader counts and verifies that no entities are tracked after an `AsNoTracking` query. Compare unordered results using deterministic ordering. For value assertions, keep expected values explicit. Behavioral tests can compare related query results, such as ascending versus descending sort keys, a page versus an unpaged result, or grouped counts versus expanded children.

## Rental listing query coverage

The rental listing tests use the real repository and `UserRepository`, with no mocked database or per-test replacement records. They assert successful execution and materialization rather than hard-coded listing counts or business data:

- `RentalListingRepositoryShould`: every grid filter independently, combined filters, null/empty/whitespace and quote-containing search inputs, all nine sort columns in both directions, paging boundaries, optional total counts, safe sort fallback, sanitized host/licence searches, local-government scoping, and host-property counts.
- `GroupedRentalListingRepositoryShould`: the same filter matrix against raw SQL counts/pages/children; both registration and composite child-key predicates; typed null and quote-containing keys; all six sort columns and null placement; deterministic paging; trimmed keys and digits-only licence equivalence; aggregate/child consistency; page-anchor host hydration; host-property flags and local-government scoping; nullable action timestamps.
- `RentalListingReadRepositoryShould`: details with/without history, history ordering and contiguous months, Pacific-time and invalid-registration action transforms, unauthorized and missing listings, takedown host-email options, export reads, extract lookups and scoping, address history, latest export time, and pending-data existence.

`ExecuteReaderAsyncCountingInterceptor` counts successful **EF Core asynchronous reader commands only**. Grid pages use one read without a count and two with a count; standalone grid/host counts use one. Grouped methods call `DbCommand.ExecuteReaderAsync` directly and bypass EF interception: zero intercepted reads does not mean no SQL ran. Those commands are validated by awaiting execution against PostgreSQL, materializing populated baseline results in dedicated contract tests, and checking that the connection closes. A nonempty grouped page also executes one intercepted EF read to hydrate its anchors; its raw count/page/host-array commands are not included in that count. Synchronous export-time reads are not counted by this interceptor either.

The `recent` filter executes the branch selected by the current Pacific date. The static baseline may have no rows for that month; this checks SQL execution, not time-dependent expected results. It does not force both sides of the day-19 cutoff. Write and get-or-create methods are intentionally excluded.

## Database lifecycle and schema boundaries

The xUnit collection starts one disposable container and removes it when the collection finishes. The fixture mounts the test output's `database-init` directory read-only at `/docker-entrypoint-initdb.d`, similarly to Aspire. PostgreSQL executes its top-level SQL scripts in filename order during first initialization of the container's empty data directory.

The container entrypoint runs psql with `ON_ERROR_STOP`, so initialization fails if a script fails. Do not also run the schema-only repository DDL: the pre-data script already creates those objects. Aspire's initialization folder is not used.

Each test instance clones the initialized `integration_tests` database into its own uniquely named database, then drops the clone after the test. Cloning preserves SQL-created objects and any initialization seed data. Administrative connections use `postgres` because the source database must have no active connections while cloning; do not open connections directly to `PostgresFixture.ConnectionString`. Use `CreateDbContext()` to connect to the test's clone instead. Tests in this collection run sequentially and cannot leak changes into another test.

EF is used only for querying and saving data, not for schema creation or migrations. Test contexts enable Npgsql's NetTopologySuite support. Schema objects, including views and audit triggers, come from the initialization scripts.

## Change the baseline

Re-run `Export-DatabaseInit.ps1` when the shared development database changes, scrub personal information again, and update affected test assertions at the same time. Add further `.sql` files with numeric prefixes when needed; every clone inherits their data. Repository query tests should use this initialized baseline rather than seed scenario-specific data inside each test.

Each test run creates a fresh container, so scripts run again without manual volume removal.
