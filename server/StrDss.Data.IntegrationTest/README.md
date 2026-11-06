# Data integration tests

These .NET 8/xUnit tests execute repository queries against real PostgreSQL using Testcontainers. The `postgis/postgis:18-3.6` image matches the local AppHost database and supports the spatial types mapped by `DssDbContext`.

## Prerequisites

- A .NET SDK capable of building .NET 8 projects.
- Docker running with Linux containers (for example, Docker Desktop).
- Access to pull the PostGIS image on the first run. Testcontainers also manages its resource-cleanup container.

No existing database, AppHost, credentials, or external database dump is required. The checked-in initialization scripts provide the schema and data. Testcontainers chooses an available host port automatically. Tests fail rather than silently skip when Docker is unavailable.

## Run

From the repository root:

```powershell
dotnet test server/StrDss.Data.IntegrationTest/StrDss.Data.IntegrationTest.csproj
```

Alternatively, build the solution and run `StrDss.Data.IntegrationTest` in Visual Studio Test Explorer. Sample tests have the `Category=Integration` trait. Running the whole solution's tests also runs this project and therefore requires Docker.

## Add query tests

1. Derive the test class from `DatabaseTestBase`.
2. Add `[Collection(PostgresCollection.Name)]` and `[Trait("Category", "Integration")]`.
3. Accept `PostgresFixture` in the constructor and pass it to the base constructor.
4. Identify the expected records in `database-init/002-data.sql`; do not insert replacement data for repository query tests.
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

The checked-in dump scripts run in this order:

1. `database-init/001-pre-data.sql` - schemas, PostGIS, tables, views, functions, procedures, and the initially unpopulated latest-report materialized view.
2. `database-init/002-data.sql` - pre-loaded database records and sequence values.
3. `database-init/003-post-data.sql` - constraints, indexes, audit triggers, and latest-report materialized-view refresh.

The project copies these scripts to the build output. The container entrypoint runs psql with `ON_ERROR_STOP`, so initialization fails if a script fails. Do not also run the schema-only repository DDL: the dump already creates those objects. Aspire's initialization folder is not used.

Each test instance clones the initialized `integration_tests` database into its own uniquely named database, then drops the clone after the test. Cloning preserves SQL-created objects and any initialization seed data. Administrative connections use `postgres` because the source database must have no active connections while cloning; do not open connections directly to `PostgresFixture.ConnectionString`. Use `CreateDbContext()` to connect to the test's clone instead. Tests in this collection run sequentially and cannot leak changes into another test.

EF is used only for querying and saving data, not for schema creation or migrations. Test contexts enable Npgsql's NetTopologySuite support. `DatabaseInitializationShould` verifies the populated cloned views and, in a separate write test, the audit trigger for an inserted role.

## Change initialization scripts

Update the checked-in initialization scripts when the shared database baseline changes, and update affected test assertions at the same time. Add additional `.sql` files with numeric filename prefixes when needed; every clone inherits their data. Repository query tests should use this initialized baseline rather than seed scenario-specific data inside each test.

Clean the project after renaming or removing scripts so stale files do not remain in the mounted output directory. Every test run creates a fresh container, so scripts run again without manual volume removal.

This exercises the selected checked-in initialization baseline, not an upgrade from an existing production database. Never add production data or point this infrastructure at an external database.
