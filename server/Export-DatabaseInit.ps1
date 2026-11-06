<#
.SYNOPSIS
    Exports a Postgres database into numbered database-init SQL scripts.

.DESCRIPTION
    Reads either a running Postgres container or an external Postgres server,
    then writes plain SQL scripts that PostgreSQL runs in filename order from
    docker-entrypoint-initdb.d. Numbers are assigned in steps of 10 so a new
    script can be inserted between existing ones.

    000-099  schema setup (pg_dump --section=pre-data, plus any extra SQL)
    100-799  data loads. Each entry is one file. Its Tables array is passed to
             pg_dump as repeated -t arguments. Put a very large table in its
             own entry.
    800-999  post-data (constraints, indexes, triggers), then one refresh file
             per populated materialized view, then statistics maintenance.
             Refresh files are discovered from the source database. Each view
             is its own file so initialization runs them in dependency order
             after the indexes exist and before ANALYZE.

    Edit the three export arrays below. Re-run this script after the source
    database changes. PostgreSQL executes every .sql file in this folder, so
    remove older dumps that this run replaces (for example 001-pre-data.sql,
    002-data.sql, and 003-post-data.sql) before using the new set. Sequence
    values travel with the table data files. Ordinary views have nothing to
    refresh.

.PARAMETER ContainerName
    Docker container name, for example postgis-crqmrkas. Choose this or -Server.

.PARAMETER Server
    Hostname or IP address of an external Postgres server. Choose this or
    -ContainerName. Requires pg_dump and psql on PATH, at the server's major
    version or newer.

.PARAMETER Port
    Port for -Server. Defaults to 5432.

.PARAMETER Username
    Postgres role passed to pg_dump -U. Defaults to strdssdev.

.PARAMETER Database
    Database name passed to pg_dump -d. Defaults to strdssdev.

.PARAMETER OutputDirectory
    Folder that receives the SQL files. Defaults to this script's folder.

.PARAMETER Password
    Password for the Postgres role. When omitted, pg_dump uses trust
    authentication inside the container, a password already in PGPASSWORD, or
    the local pgpass file when -Server is used.

.PARAMETER SslMode
    PGSSLMODE for this run: disable, allow, prefer, require, verify-ca, or
    verify-full. Hosted databases often need require.

.EXAMPLE
    .\Export-DatabaseInit.ps1 -ContainerName postgis-crqmrkas

.EXAMPLE
    .\Export-DatabaseInit.ps1 -ContainerName postgis-crqmrkas -Username strdssdev -Database strdssdev

.EXAMPLE
    .\Export-DatabaseInit.ps1 -Server dev-db.example.com -Username strdssdev -Database strdssdev -Password $env:PGPASSWORD -SslMode require
#>
[CmdletBinding(DefaultParameterSetName = 'Container', SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Container')]
    [string]$ContainerName,

    [Parameter(Mandatory = $true, ParameterSetName = 'Server')]
    [string]$Server,

    [Parameter(ParameterSetName = 'Server')]
    [ValidateRange(1, 65535)]
    [int]$Port = 5432,

    [string]$Username = 'strdssdev',

    [string]$Database = 'strdssdev',

    [string]$OutputDirectory = $PSScriptRoot,

    [string]$Password,

    [ValidateSet('disable', 'allow', 'prefer', 'require', 'verify-ca', 'verify-full')]
    [string]$SslMode
)

$UseContainer = $PSCmdlet.ParameterSetName -eq 'Container'

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Keep client stderr from becoming a terminating error. Exit codes are checked explicitly.
$PSNativeCommandUseErrorActionPreference = $false

# File numbers advance by 10 inside each band so later scripts can be inserted
# without renaming the set. Set Number on an entry to claim a specific slot.
$NumberStep = 10

# Tables created by extensions. They are already loaded by CREATE EXTENSION.
$excludedTables = @(
    'public.spatial_ref_sys'
)

# Third-party schemas. Their tables are created by those products, so the
# missing-table check does not require them in a data export.
$excludedSchemas = @(
    'hangfire'
)

# 000-099 schema setup.
$schemaExports = @(
    @{
        Name     = 'pre-data'
        DumpArgs = @('--section=pre-data')
    }
)

# 100-799 data loads.
# Each Tables array becomes repeated pg_dump arguments: -t table -t table ...
# Large tables are listed alone so their files stay separate.
$dataExports = @(
    @{
        Name   = 'reference-data'
        Tables = @(
            'public.dss_access_request_status',
            'public.dss_business_licence_status_type',
            'public.dss_economic_region',
            'public.dss_email_message_type',
            'public.dss_listing_action_type',
            'public.dss_listing_status_type',
            'public.dss_local_government_type',
            'public.dss_organization_type',
            'public.dss_platform_type',
            'public.dss_user_privilege',
            'public.dss_user_role',
            'public.dss_user_role_privilege'
        )
    },
    @{
        Name   = 'users'
        Tables = @(
            'public.dss_user_identity',
            'public.dss_user_role_assignment'
        )
    },
    @{
        Name   = 'dss_organization'
        Tables = @(
            'public.dss_organization'
        )
    },
    @{
        Name   = 'organization-details'
        Tables = @(
            'public.dss_organization_contact_person',
            'public.dss_business_licence'
        )
    },
    @{
        Name   = 'dss_physical_address'
        Tables = @(
            'public.dss_physical_address'
        )
    },
    @{
        Name   = 'listing-reports'
        Tables = @(
            'public.dss_rental_listing_report',
            'public.dss_email_message',
            'public.dss_rental_listing_extract',
            'public.dss_rental_listing_action'
        )
    },
    @{
        Name   = 'dss_rental_listing'
        Tables = @(
            'public.dss_rental_listing'
        )
    },
    @{
        Name   = 'dss_rental_listing_contact'
        Tables = @(
            'public.dss_rental_listing_contact'
        )
    },
    @{
        Name   = 'dss_upload_delivery'
        Tables = @(
            'public.dss_upload_delivery'
        )
    },
    @{
        Name   = 'dss_upload_line'
        Tables = @(
            'public.dss_upload_line'
        )
    }
)

# 800-999 post-data, generated materialized-view refresh files, then statistics.
# Refresh files are inserted immediately before the analyze entry.
$postExports = @(
    @{
        Name     = 'post-data'
        DumpArgs = @('--section=post-data')
    },
    @{
        Name = 'analyze'
        Sql  = @'
-- Refresh planner statistics after the data load and materialized view refresh.
ANALYZE;
'@
    }
)

$commonDumpArguments = @('--no-owner', '--no-acl')
$safeArgumentPattern = '^[A-Za-z0-9_.:/=+\-@]+$'

function Assert-SafeArgument {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Argument,
        [Parameter(Mandatory = $true)]
        [string]$Label
    )

    if ($Argument -notmatch $safeArgumentPattern) {
        throw "$Label '$Argument' contains characters this script does not pass to the database client. Use letters, digits, and . _ : / = + - @."
    }
}

function Get-ExportNumber {
    param(
        [Parameter(Mandatory = $true)]
        $Export,
        [Parameter(Mandatory = $true)]
        [int]$NextNumber
    )

    if ($Export.ContainsKey('Number') -and $null -ne $Export['Number']) {
        return [int]$Export['Number']
    }

    return $NextNumber
}

function Get-PlannedExports {
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Exports,
        [Parameter(Mandatory = $true)]
        [int]$RangeStart,
        [Parameter(Mandatory = $true)]
        [int]$RangeEnd,
        [Parameter(Mandatory = $true)]
        [string]$Band
    )

    $planned = New-Object System.Collections.Generic.List[object]
    $usedNumbers = @{}
    $nextNumber = $RangeStart

    foreach ($export in $Exports) {
        if (-not $export.ContainsKey('Name') -or [string]::IsNullOrWhiteSpace([string]$export['Name'])) {
            throw "A $Band export is missing Name."
        }

        $name = [string]$export['Name']
        if ($name -notmatch '^[A-Za-z0-9][A-Za-z0-9._-]{0,80}$') {
            throw "Export name '$name' must be a file-name fragment (letters, digits, dot, underscore, hyphen)."
        }

        while ($usedNumbers.ContainsKey($nextNumber)) {
            $nextNumber += $NumberStep
        }

        $number = Get-ExportNumber -Export $export -NextNumber $nextNumber
        if ($number -lt $RangeStart -or $number -gt $RangeEnd) {
            throw "File number $number for '$name' is outside the $Band range $RangeStart-$RangeEnd."
        }
        if ($usedNumbers.ContainsKey($number)) {
            throw "File number $number is assigned twice (second use is '$name')."
        }

        $usedNumbers[$number] = $true
        if (-not $export.ContainsKey('Number') -or $null -eq $export['Number']) {
            $nextNumber += $NumberStep
        }

        $fileName = '{0:D3}-{1}.sql' -f $number, $name
        $planned.Add([pscustomobject]@{
                Band     = $Band
                Number   = $number
                Name     = $name
                FileName = $fileName
                Path     = (Join-Path $OutputDirectory $fileName)
                Export   = $export
            })
    }

    return $planned
}

function Get-DatabaseTargetLabel {
    if ($UseContainer) {
        return "container '$ContainerName'"
    }

    return "server '${Server}:$Port'"
}

function Get-ConnectionArguments {
    if (-not $UseContainer) {
        '-h'
        $Server
        '-p'
        ([string]$Port)
    }

    '-U'
    $Username
    '-d'
    $Database
}

function Get-PgDumpBaseArguments {
    Get-ConnectionArguments
}

function Get-ForwardedEnvironmentNames {
    $names = New-Object System.Collections.Generic.List[string]
    if (-not [string]::IsNullOrEmpty($env:PGPASSWORD)) {
        $names.Add('PGPASSWORD')
    }
    if (-not [string]::IsNullOrEmpty($env:PGSSLMODE)) {
        $names.Add('PGSSLMODE')
    }

    return $names.ToArray()
}

function Get-TableArguments {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Tables
    )

    # pg_dump expects one -t before each table: -t schema.table -t schema.table
    foreach ($table in $Tables) {
        $trimmed = $table.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmed)) {
            throw 'A data export contains an empty table name.'
        }

        Assert-SafeArgument -Argument $trimmed -Label 'Table name'
        '-t'
        $trimmed
    }
}

function Get-DataDumpArguments {
    param(
        [Parameter(Mandatory = $true)]
        $Export
    )

    if (-not $Export.ContainsKey('Tables') -or $null -eq $Export['Tables']) {
        throw "Data export '$($Export['Name'])' is missing Tables."
    }

    $tables = @($Export['Tables'] | ForEach-Object { [string]$_ })
    if ($tables.Count -eq 0) {
        throw "Data export '$($Export['Name'])' has no tables. Add schema-qualified names to its Tables array."
    }

    @(
        Get-PgDumpBaseArguments
        '--data-only'
        $commonDumpArguments
        Get-TableArguments -Tables $tables
    )
}

function Get-SectionDumpArguments {
    param(
        [Parameter(Mandatory = $true)]
        $Export
    )

    $hasDumpArgs = $Export.ContainsKey('DumpArgs') -and $null -ne $Export['DumpArgs']
    $hasSql = $Export.ContainsKey('Sql') -and -not [string]::IsNullOrWhiteSpace([string]$Export['Sql'])
    if ($hasDumpArgs -and $hasSql) {
        throw "Export '$($Export['Name'])' sets both DumpArgs and Sql. Use only one."
    }
    if (-not $hasDumpArgs -and -not $hasSql) {
        throw "Export '$($Export['Name'])' needs DumpArgs or Sql."
    }
    if ($hasSql) {
        return
    }

    $arguments = @(
        Get-PgDumpBaseArguments
        @($Export['DumpArgs'] | ForEach-Object { [string]$_ })
        $commonDumpArguments
    )
    foreach ($argument in $arguments) {
        Assert-SafeArgument -Argument $argument -Label 'pg_dump argument'
    }

    $arguments
}

function Get-DockerExecPrefix {
    'exec'
    foreach ($name in @(Get-ForwardedEnvironmentNames)) {
        # The value stays in the process environment. docker copies the named variable into the container.
        '-e'
        $name
    }

    $ContainerName
}

function Get-DockerArguments {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$CommandArguments
    )

    foreach ($argument in $CommandArguments) {
        Assert-SafeArgument -Argument $argument -Label 'Docker argument'
    }

    @(
        Get-DockerExecPrefix
        $CommandArguments
    )
}

function Invoke-ExternalCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,
        [string]$StandardOutputPath
    )

    $errorPath = Join-Path ([System.IO.Path]::GetTempPath()) ('pgdump-err-' + [guid]::NewGuid().ToString('N') + '.txt')
    $startParameters = @{
        FilePath              = $FilePath
        ArgumentList          = $Arguments
        Wait                  = $true
        PassThru              = $true
        NoNewWindow           = $true
        RedirectStandardError = $errorPath
    }
    if ($StandardOutputPath) {
        $startParameters.RedirectStandardOutput = $StandardOutputPath
    }

    try {
        $process = Start-Process @startParameters
        $standardError = ''
        if (Test-Path -LiteralPath $errorPath) {
            $standardError = [System.IO.File]::ReadAllText($errorPath)
        }

        return [pscustomobject]@{
            ExitCode      = $process.ExitCode
            StandardError = $standardError
        }
    }
    finally {
        Remove-Item -LiteralPath $errorPath -Force -ErrorAction SilentlyContinue
    }
}

function Write-CommandFailure {
    param(
        [Parameter(Mandatory = $true)]
        $Result,
        [Parameter(Mandatory = $true)]
        [string]$Activity
    )

    if (-not [string]::IsNullOrWhiteSpace($Result.StandardError)) {
        Write-Host $Result.StandardError.TrimEnd()
    }

    throw "$Activity failed with exit code $($Result.ExitCode)."
}

function Invoke-Psql {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Sql
    )

    # The SQL text is one argument. It is not passed through the pg_dump safety check,
    # which rejects spaces. The call operator passes it as a single argument.
    $arguments = @(
        Get-ConnectionArguments
        '-v'
        'ON_ERROR_STOP=1'
        '-Atq'
        '-F'
        '|'
        '-c'
        $Sql
    )
    $executable = 'psql'
    if ($UseContainer) {
        $executable = 'docker'
        $arguments = @(Get-DockerExecPrefix) + @('psql') + $arguments
    }

    $previousErrorAction = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $output = & $executable @arguments 2>&1
    }
    finally {
        $ErrorActionPreference = $previousErrorAction
    }

    if ($LASTEXITCODE -ne 0) {
        $detail = ($output | Out-String).Trim()
        throw "psql failed on database '$Database' on $(Get-DatabaseTargetLabel). $detail"
    }

    return @(
        $output |
            ForEach-Object { $_.ToString().Trim() } |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    )
}

function Get-DatabaseTableNames {
    $sql = "SELECT n.nspname || '.' || c.relname FROM pg_class c JOIN pg_namespace n ON n.oid = c.relnamespace WHERE c.relkind IN ('r', 'p') AND n.nspname NOT IN ('pg_catalog', 'information_schema', 'topology', 'tiger', 'tiger_data') ORDER BY 1;"
    return @(
        Invoke-Psql -Sql $sql |
            Where-Object { $_ -match '^[A-Za-z_][A-Za-z0-9_]*\.[A-Za-z_][A-Za-z0-9_]*$' }
    )
}

function Get-MaterializedViewRefreshExports {
    # Match pg_dump: refresh populated materialized views, after any materialized
    # view they read (including through an ordinary view). A view that depends on
    # an unpopulated materialized view cannot be refreshed.
    $sql = @'
WITH RECURSIVE walk AS (
    SELECT
        n1.nspname AS schema_name,
        c1.relname AS view_name,
        c2.oid AS ref_oid,
        n2.nspname AS ref_schema,
        c2.relname AS ref_name,
        c2.relkind AS ref_kind,
        ARRAY[c1.oid] AS path
    FROM pg_class c1
    JOIN pg_namespace n1 ON n1.oid = c1.relnamespace
    JOIN pg_rewrite r1 ON r1.ev_class = c1.oid
    JOIN pg_depend d2
        ON d2.classid = 'pg_rewrite'::regclass
       AND d2.objid = r1.oid
       AND d2.refclassid = 'pg_class'::regclass
       AND d2.refobjid <> c1.oid
    JOIN pg_class c2
        ON c2.oid = d2.refobjid
       AND c2.relkind IN ('m', 'v')
    JOIN pg_namespace n2 ON n2.oid = c2.relnamespace
    WHERE c1.relkind = 'm'
      AND NOT (c2.oid = ANY (ARRAY[c1.oid]))
    UNION ALL
    SELECT
        walk.schema_name,
        walk.view_name,
        c3.oid,
        n3.nspname,
        c3.relname,
        c3.relkind,
        walk.path || walk.ref_oid
    FROM walk
    JOIN pg_rewrite r3 ON r3.ev_class = walk.ref_oid
    JOIN pg_depend d3
        ON d3.classid = 'pg_rewrite'::regclass
       AND d3.objid = r3.oid
       AND d3.refclassid = 'pg_class'::regclass
       AND d3.refobjid <> walk.ref_oid
    JOIN pg_class c3
        ON c3.oid = d3.refobjid
       AND c3.relkind IN ('m', 'v')
    JOIN pg_namespace n3 ON n3.oid = c3.relnamespace
    WHERE cardinality(walk.path) < 20
      AND NOT (c3.oid = ANY (walk.path || walk.ref_oid))
)
SELECT kind, schema_name, object_name, detail, ref_schema, ref_name
FROM (
    SELECT
        'v'::text AS kind,
        n.nspname AS schema_name,
        c.relname AS object_name,
        CASE WHEN c.relispopulated THEN 't' ELSE 'f' END AS detail,
        '-'::text AS ref_schema,
        '-'::text AS ref_name
    FROM pg_class c
    JOIN pg_namespace n ON n.oid = c.relnamespace
    WHERE c.relkind = 'm'
      AND n.nspname NOT IN ('pg_catalog', 'information_schema')
      AND n.nspname !~ '^pg_'
      AND NOT EXISTS (
          SELECT 1
          FROM pg_depend d
          WHERE d.classid = 'pg_class'::regclass
            AND d.objid = c.oid
            AND d.deptype = 'e'
      )
    UNION ALL
    SELECT DISTINCT
        'd'::text,
        schema_name,
        view_name,
        '-'::text,
        ref_schema,
        ref_name
    FROM walk
    WHERE ref_kind = 'm'
) catalog
ORDER BY 1, 2, 3, 5, 6;
'@

    $views = @{}
    $edges = New-Object System.Collections.Generic.List[object]
    foreach ($row in @(Invoke-Psql -Sql $sql)) {
        if ($row -notmatch '^[vd]\|') {
            continue
        }

        $parts = $row.Split('|')
        if ($parts.Count -lt 6) {
            throw "Unexpected materialized-view catalog row '$row'."
        }

        $schema = $parts[1]
        $name = $parts[2]
        if ($schema -notmatch '^[a-z_][a-z0-9_]*$' -or $name -notmatch '^[a-z_][a-z0-9_]*$') {
            throw "Materialized view '$schema.$name' is not a plain lowercase identifier, so this script cannot pass it to pg_dump."
        }

        if ($parts[0] -eq 'v') {
            $views["$schema.$name"] = [pscustomobject]@{
                QualifiedName = "$schema.$name"
                Populated     = ($parts[3] -eq 't')
            }
            continue
        }

        $refSchema = $parts[4]
        $refName = $parts[5]
        if ($refSchema -notmatch '^[a-z_][a-z0-9_]*$' -or $refName -notmatch '^[a-z_][a-z0-9_]*$') {
            throw "Materialized view dependency '$refSchema.$refName' is not a plain lowercase identifier."
        }

        $edges.Add([pscustomobject]@{
                Dependent  = "$schema.$name"
                Dependency = "$refSchema.$refName"
            })
    }

    $blocked = @{}
    foreach ($view in $views.Values) {
        if (-not $view.Populated) {
            $blocked[$view.QualifiedName] = $true
        }
    }

    $changed = $true
    while ($changed) {
        $changed = $false
        foreach ($edge in $edges) {
            if ($blocked.ContainsKey($edge.Dependency) -and
                $views.ContainsKey($edge.Dependent) -and
                -not $blocked.ContainsKey($edge.Dependent)) {
                $blocked[$edge.Dependent] = $true
                $changed = $true
            }
        }
    }

    $suppressed = @(
        $views.Values |
            Where-Object { $_.Populated -and $blocked.ContainsKey($_.QualifiedName) } |
            ForEach-Object { $_.QualifiedName } |
            Sort-Object
    )
    if ($suppressed.Count -gt 0) {
        Write-Warning ("These populated materialized views read an unpopulated materialized view, so they will stay empty: " + ($suppressed -join ', '))
    }

    $refreshViews = @(
        $views.Values |
            Where-Object { -not $blocked.ContainsKey($_.QualifiedName) } |
            Sort-Object QualifiedName
    )
    if ($refreshViews.Count -eq 0) {
        return @()
    }

    $orderedNames = @(Get-MaterializedViewRefreshOrder -Views $refreshViews -Edges $edges)
    $exports = New-Object System.Collections.Generic.List[object]
    foreach ($qualifiedName in $orderedNames) {
        $exportName = "refresh-$qualifiedName"
        if ($exportName -notmatch '^[A-Za-z0-9][A-Za-z0-9._-]{0,80}$') {
            throw "Refresh file name '$exportName' is too long or contains characters the export names do not allow."
        }

        $exports.Add(@{
                Name          = $exportName
                QualifiedName = $qualifiedName
                Sql           = @"
-- Populate $qualifiedName from the tables loaded above.
REFRESH MATERIALIZED VIEW $qualifiedName;
"@
            })
    }

    return $exports.ToArray()
}

function Get-MaterializedViewRefreshOrder {
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Views,

        # An empty list is valid: a materialized view may read only tables.
        [System.Collections.Generic.List[object]]$Edges = $(New-Object System.Collections.Generic.List[object])
    )

    $incoming = @{}
    $dependents = @{}
    foreach ($view in $Views) {
        $incoming[$view.QualifiedName] = 0
        $dependents[$view.QualifiedName] = New-Object System.Collections.Generic.List[string]
    }

    $seenEdges = @{}
    foreach ($edge in $Edges) {
        if (-not $incoming.ContainsKey($edge.Dependent) -or -not $incoming.ContainsKey($edge.Dependency)) {
            continue
        }

        $edgeKey = $edge.Dependency + '->' + $edge.Dependent
        if ($seenEdges.ContainsKey($edgeKey)) {
            continue
        }

        $seenEdges[$edgeKey] = $true
        $incoming[$edge.Dependent]++
        $dependents[$edge.Dependency].Add($edge.Dependent)
    }

    $ready = New-Object System.Collections.Generic.List[string]
    foreach ($view in $Views) {
        if ($incoming[$view.QualifiedName] -eq 0) {
            $ready.Add($view.QualifiedName)
        }
    }

    $ordered = New-Object System.Collections.Generic.List[string]
    while ($ready.Count -gt 0) {
        $ready.Sort([System.StringComparer]::Ordinal)
        $name = $ready[0]
        $ready.RemoveAt(0)
        $ordered.Add($name)
        foreach ($dependent in $dependents[$name]) {
            $incoming[$dependent]--
            if ($incoming[$dependent] -eq 0) {
                $ready.Add($dependent)
            }
        }
    }

    if ($ordered.Count -ne $Views.Count) {
        $stuck = @(
            $Views |
                Where-Object { -not $ordered.Contains($_.QualifiedName) } |
                ForEach-Object { $_.QualifiedName } |
                Sort-Object
        )
        throw ("Materialized views depend on each other in a cycle: " + ($stuck -join ', '))
    }

    return $ordered.ToArray()
}

function Get-PostExportsForRun {
    param(
        # Empty when the database has no populated materialized view to refresh.
        [object[]]$RefreshExports = @()
    )

    # pg_dump --section=post-data emits REFRESH for populated materialized views.
    # Exclude that data here so the numbered refresh files are the only refresh.
    $excludeArguments = New-Object System.Collections.Generic.List[string]
    foreach ($refresh in $RefreshExports) {
        $qualifiedName = [string]$refresh['QualifiedName']
        Assert-SafeArgument -Argument $qualifiedName -Label 'Materialized view'
        $excludeArguments.Add('--exclude-table-data=' + $qualifiedName)
    }

    $planned = New-Object System.Collections.Generic.List[object]
    $inserted = $false
    foreach ($export in $postExports) {
        if (-not $inserted -and [string]$export['Name'] -eq 'analyze') {
            foreach ($refresh in $RefreshExports) {
                $planned.Add($refresh)
            }
            $inserted = $true
        }

        if ([string]$export['Name'] -eq 'post-data' -and $excludeArguments.Count -gt 0) {
            $dumpArgs = New-Object System.Collections.Generic.List[string]
            foreach ($argument in @($export['DumpArgs'])) {
                $dumpArgs.Add([string]$argument)
            }
            $dumpArgs.AddRange($excludeArguments)
            $planned.Add(@{
                    Name     = [string]$export['Name']
                    DumpArgs = $dumpArgs.ToArray()
                })
        }
        else {
            $planned.Add($export)
        }
    }

    if (-not $inserted) {
        foreach ($refresh in $RefreshExports) {
            $planned.Add($refresh)
        }
    }

    return $planned.ToArray()
}

function Format-ByteCount {
    param([long]$Bytes)

    if ($Bytes -ge 1MB) {
        return '{0:N1} MB' -f ($Bytes / 1MB)
    }
    if ($Bytes -ge 1KB) {
        return '{0:N1} KB' -f ($Bytes / 1KB)
    }

    return "$Bytes bytes"
}

function Get-PrintedDumpCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$PgDumpArguments,
        [Parameter(Mandatory = $true)]
        [string]$FileName
    )

    $parts = New-Object System.Collections.Generic.List[string]
    if ($UseContainer) {
        $parts.Add('docker')
        $parts.Add('exec')
        foreach ($name in @(Get-ForwardedEnvironmentNames)) {
            $parts.Add('-e')
            $parts.Add($name)
        }

        $parts.Add($ContainerName)
    }

    $parts.Add('pg_dump')
    $parts.AddRange($PgDumpArguments)
    return (($parts.ToArray() -join ' ') + " > .\$FileName")
}

function Export-PgDumpFile {
    [CmdletBinding(SupportsShouldProcess = $true)]
    param(
        [Parameter(Mandatory = $true)]
        $Plan,
        [Parameter(Mandatory = $true)]
        [string[]]$PgDumpArguments
    )

    $command = Get-PrintedDumpCommand -PgDumpArguments $PgDumpArguments -FileName $Plan.FileName
    Write-Host $command

    if (-not $PSCmdlet.ShouldProcess($Plan.Path, 'pg_dump')) {
        return
    }

    if ($UseContainer) {
        $filePath = 'docker'
        $arguments = [string[]]@(Get-DockerArguments -CommandArguments (@('pg_dump') + [string[]]$PgDumpArguments))
    }
    else {
        $filePath = 'pg_dump'
        foreach ($argument in $PgDumpArguments) {
            Assert-SafeArgument -Argument $argument -Label 'pg_dump argument'
        }
        $arguments = [string[]]$PgDumpArguments
    }

    $result = Invoke-ExternalCommand -FilePath $filePath -Arguments $arguments -StandardOutputPath $Plan.Path
    if ($result.ExitCode -ne 0) {
        if (Test-Path -LiteralPath $Plan.Path) {
            Remove-Item -LiteralPath $Plan.Path -Force
        }
        Write-CommandFailure -Result $result -Activity "pg_dump for $($Plan.FileName)"
    }

    if (-not [string]::IsNullOrWhiteSpace($result.StandardError)) {
        Write-Host $result.StandardError.TrimEnd()
    }

    $size = (Get-Item -LiteralPath $Plan.Path).Length
    Write-Host ("Wrote {0} ({1})" -f $Plan.FileName, (Format-ByteCount $size))
    Write-Host ''
}

function Export-SqlFile {
    [CmdletBinding(SupportsShouldProcess = $true)]
    param(
        [Parameter(Mandatory = $true)]
        $Plan,
        [Parameter(Mandatory = $true)]
        [string]$Sql
    )

    Write-Host ("Write {0}" -f $Plan.FileName)
    if (-not $PSCmdlet.ShouldProcess($Plan.Path, 'Write SQL')) {
        return
    }

    $content = $Sql.Replace("`r`n", "`n").Replace("`r", "`n")
    if (-not $content.EndsWith("`n")) {
        $content += "`n"
    }

    $encoding = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($Plan.Path, $content, $encoding)
    Write-Host ("Wrote {0}" -f $Plan.FileName)
    Write-Host ''
}

if ($UseContainer) {
    Assert-SafeArgument -Argument $ContainerName -Label 'Container name'
}
else {
    Assert-SafeArgument -Argument $Server -Label 'Server'
}
Assert-SafeArgument -Argument $Username -Label 'Username'
Assert-SafeArgument -Argument $Database -Label 'Database'

if ($UseContainer) {
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        throw 'docker was not found on PATH.'
    }
}
else {
    foreach ($tool in @('pg_dump', 'psql')) {
        if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
            throw "$tool was not found on PATH. Install PostgreSQL client tools at the server major version or newer, or pass -ContainerName to run the tools inside Docker."
        }
    }
}

if (-not (Test-Path -LiteralPath $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
}
$OutputDirectory = (Resolve-Path -LiteralPath $OutputDirectory).Path

$previousPassword = $env:PGPASSWORD
$passwordOverridden = $PSBoundParameters.ContainsKey('Password')
if ($passwordOverridden) {
    $env:PGPASSWORD = $Password
}

$previousSslMode = $env:PGSSLMODE
$sslModeOverridden = $PSBoundParameters.ContainsKey('SslMode')
if ($sslModeOverridden) {
    $env:PGSSLMODE = $SslMode
}

try {
    Write-Host ("Source: {0}, database {1}, role {2}" -f (Get-DatabaseTargetLabel), $Database, $Username)
    Write-Host ''

    if ($UseContainer) {
        $inspect = & docker inspect -f '{{.State.Running}}' $ContainerName 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "Docker could not inspect container '$ContainerName'. $(($inspect | Out-String).Trim())"
        }
        $running = @($inspect | ForEach-Object { $_.ToString().Trim() } | Where-Object { $_ }) | Select-Object -Last 1
        if ($running -ne 'true') {
            throw "Container '$ContainerName' is not running."
        }
    }

    $refreshExports = @(Get-MaterializedViewRefreshExports)
    if ($refreshExports.Count -eq 0) {
        Write-Host 'No populated materialized views to refresh.'
        Write-Host ''
    }

    $schemaPlan = @(Get-PlannedExports -Exports $schemaExports -RangeStart 0 -RangeEnd 99 -Band 'schema')
    $dataPlan = @(Get-PlannedExports -Exports $dataExports -RangeStart 100 -RangeEnd 799 -Band 'data')
    $postExportsForRun = @(Get-PostExportsForRun -RefreshExports $refreshExports)
    $postPlan = @(Get-PlannedExports -Exports $postExportsForRun -RangeStart 800 -RangeEnd 999 -Band 'post-data')

    $allNumbers = @($schemaPlan + $dataPlan + $postPlan | ForEach-Object { $_.Number })
    $duplicateNumber = $allNumbers | Group-Object | Where-Object { $_.Count -gt 1 } | Select-Object -First 1
    if ($duplicateNumber) {
        throw "File number $($duplicateNumber.Name) is used in more than one export band."
    }

    $listedTables = New-Object System.Collections.Generic.List[string]
    $listedTableOwners = @{}
    foreach ($plan in $dataPlan) {
        $null = Get-DataDumpArguments -Export $plan.Export
        $tables = @($plan.Export['Tables'] | ForEach-Object { [string]$_.Trim() })
        foreach ($table in $tables) {
            $key = $table.ToLowerInvariant()
            if ($listedTableOwners.ContainsKey($key)) {
                throw "Table '$table' is listed in both '$($listedTableOwners[$key])' and '$($plan.Name)'."
            }
            $listedTableOwners[$key] = $plan.Name
            $listedTables.Add($table)
        }
    }

    foreach ($excluded in $excludedTables) {
        $key = $excluded.ToLowerInvariant()
        if ($listedTableOwners.ContainsKey($key)) {
            throw "Table '$excluded' is excluded and also listed in '$($listedTableOwners[$key])'."
        }
    }

    $databaseTables = @(Get-DatabaseTableNames)
    $databaseTableSet = @{}
    foreach ($table in $databaseTables) {
        $databaseTableSet[$table.ToLowerInvariant()] = $table
    }

    $missing = @(
        $listedTables |
            Where-Object { -not $databaseTableSet.ContainsKey($_.ToLowerInvariant()) }
    )
    if ($missing.Count -gt 0) {
        throw ("These tables are not in ${Database}: " + ($missing -join ', '))
    }

    $excludedSet = @{}
    foreach ($excluded in $excludedTables) {
        $excludedSet[$excluded.ToLowerInvariant()] = $true
    }
    $excludedSchemaSet = @{}
    foreach ($schema in $excludedSchemas) {
        $trimmed = $schema.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmed)) {
            throw 'An excluded schema name is empty.'
        }
        Assert-SafeArgument -Argument $trimmed -Label 'Excluded schema'
        $excludedSchemaSet[$trimmed.ToLowerInvariant()] = $true
    }
    $omitted = @(
        $databaseTables |
            Where-Object {
                $key = $_.ToLowerInvariant()
                $schema = ($key -split '\.', 2)[0]
                -not $listedTableOwners.ContainsKey($key) -and -not $excludedSet.ContainsKey($key) -and -not $excludedSchemaSet.ContainsKey($schema)
            }
    )
    if ($omitted.Count -gt 0) {
        Write-Warning ("These tables will not be exported. Add them to a Tables array in `$dataExports: " + ($omitted -join ', '))
    }

    $writtenPaths = New-Object System.Collections.Generic.List[string]

    foreach ($plan in $schemaPlan) {
        $dumpArguments = @(Get-SectionDumpArguments -Export $plan.Export)
        if ($dumpArguments.Count -eq 0) {
            Export-SqlFile -Plan $plan -Sql ([string]$plan.Export['Sql'])
        }
        else {
            Export-PgDumpFile -Plan $plan -PgDumpArguments ([string[]]$dumpArguments)
        }
        $writtenPaths.Add($plan.Path)
    }

    foreach ($plan in $dataPlan) {
        $dumpArguments = [string[]]@(Get-DataDumpArguments -Export $plan.Export)
        Export-PgDumpFile -Plan $plan -PgDumpArguments $dumpArguments
        $writtenPaths.Add($plan.Path)
    }

    foreach ($plan in $postPlan) {
        $dumpArguments = @(Get-SectionDumpArguments -Export $plan.Export)
        if ($dumpArguments.Count -eq 0) {
            Export-SqlFile -Plan $plan -Sql ([string]$plan.Export['Sql'])
        }
        else {
            Export-PgDumpFile -Plan $plan -PgDumpArguments ([string[]]$dumpArguments)
        }
        $writtenPaths.Add($plan.Path)
    }

    $writtenSet = @{}
    foreach ($path in $writtenPaths) {
        $writtenSet[$path.ToLowerInvariant()] = $true
    }

    $leftovers = @(
        Get-ChildItem -LiteralPath $OutputDirectory -File -Filter '*.sql' |
            Where-Object { $_.Name -match '^\d{3}-.+\.sql$' -and -not $writtenSet.ContainsKey($_.FullName.ToLowerInvariant()) } |
            Sort-Object Name
    )
    if ($leftovers.Count -gt 0) {
        Write-Warning "These numbered SQL files were not written by this run. Initialization executes every .sql file in name order:"
        foreach ($leftover in $leftovers) {
            Write-Warning ("  " + $leftover.Name)
        }
    }
}
finally {
    if ($passwordOverridden) {
        if ($null -eq $previousPassword) {
            Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
        }
        else {
            $env:PGPASSWORD = $previousPassword
        }
    }
    if ($sslModeOverridden) {
        if ($null -eq $previousSslMode) {
            Remove-Item Env:PGSSLMODE -ErrorAction SilentlyContinue
        }
        else {
            $env:PGSSLMODE = $previousSslMode
        }
    }
}
