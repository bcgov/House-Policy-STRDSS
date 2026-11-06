<#
.SYNOPSIS
    Exports a running Postgres container into numbered database-init SQL scripts.

.DESCRIPTION
    Writes plain SQL scripts that PostgreSQL runs in filename order from
    docker-entrypoint-initdb.d. Numbers are assigned in steps of 10 so a new
    script can be inserted between existing ones.

    000-099  schema setup (pg_dump --section=pre-data, plus any extra SQL)
    100-799  data loads. Each entry is one file. Its Tables array is passed to
             pg_dump as repeated -t arguments. Put a very large table in its
             own entry.
    800-999  post-data (constraints, indexes, triggers) and statistics maintenance

    Edit the three export arrays below. Re-run this script after the source
    database changes. PostgreSQL executes every .sql file in this folder, so
    remove older dumps that this run replaces (for example 001-pre-data.sql,
    002-data.sql, and 003-post-data.sql) before using the new set.

.PARAMETER ContainerName
    Docker container name, for example postgis-crqmrkas.

.PARAMETER Username
    Postgres role passed to pg_dump -U. Defaults to strdssdev.

.PARAMETER Database
    Database name passed to pg_dump -d. Defaults to strdssdev.

.PARAMETER OutputDirectory
    Folder that receives the SQL files. Defaults to this script's folder.

.PARAMETER Password
    Password for the Postgres role. When omitted, pg_dump uses the container's
    local authentication (trust, or PGPASSWORD already set in this session).

.EXAMPLE
    .\Export-DatabaseInit.ps1 -ContainerName postgis-crqmrkas

.EXAMPLE
    .\Export-DatabaseInit.ps1 -ContainerName postgis-crqmrkas -Username strdssdev -Database strdssdev
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$ContainerName,

    [string]$Username = 'strdssdev',

    [string]$Database = 'strdssdev',

    [string]$OutputDirectory = $PSScriptRoot,

    [string]$Password
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Keep docker's stderr from becoming a terminating error. Exit codes are checked explicitly.
$PSNativeCommandUseErrorActionPreference = $false

# File numbers advance by 10 inside each band so later scripts can be inserted
# without renaming the set. Set Number on an entry to claim a specific slot.
$NumberStep = 10

# Tables created by extensions. They are already loaded by CREATE EXTENSION.
$excludedTables = @(
    'public.spatial_ref_sys'
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

# 800-999 post-data and statistics maintenance.
$postExports = @(
    @{
        Name     = 'post-data'
        DumpArgs = @('--section=post-data')
    },
    @{
        Name = 'analyze'
        Sql  = @'
-- Refresh planner statistics after the data load.
ANALYZE;
'@
    }
)

$commonDumpArguments = @('--no-owner', '--no-acl')
$safeArgumentPattern = '^[A-Za-z0-9_.:/=+\-]+$'

function Assert-SafeArgument {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Argument,
        [Parameter(Mandatory = $true)]
        [string]$Label
    )

    if ($Argument -notmatch $safeArgumentPattern) {
        throw "$Label '$Argument' contains characters this script does not pass to docker. Use letters, digits, and . _ : / = + -."
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

function Get-PgDumpBaseArguments {
    '-U'
    $Username
    '-d'
    $Database
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
    if (-not [string]::IsNullOrEmpty($env:PGPASSWORD)) {
        # The value stays in the process environment. docker copies it into the container.
        '-e'
        'PGPASSWORD'
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

function Invoke-Docker {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,
        [string]$StandardOutputPath
    )

    $errorPath = Join-Path ([System.IO.Path]::GetTempPath()) ('pgdump-err-' + [guid]::NewGuid().ToString('N') + '.txt')
    $startParameters = @{
        FilePath              = 'docker'
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

function Write-DockerFailure {
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

function Get-DatabaseTableNames {
    $sql = "SELECT n.nspname || '.' || c.relname FROM pg_class c JOIN pg_namespace n ON n.oid = c.relnamespace WHERE c.relkind IN ('r', 'p') AND n.nspname NOT IN ('pg_catalog', 'information_schema', 'topology', 'tiger', 'tiger_data') ORDER BY 1;"

    # The SQL text is one argument. It is not passed through the pg_dump safety check,
    # which rejects spaces. The call operator quotes it for docker.
    $arguments = @(
        Get-DockerExecPrefix
        'psql'
        '-U'
        $Username
        '-d'
        $Database
        '-v'
        'ON_ERROR_STOP=1'
        '-Atq'
        '-c'
        $sql
    )

    $previousErrorAction = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $names = & docker @arguments 2>&1
    }
    finally {
        $ErrorActionPreference = $previousErrorAction
    }

    if ($LASTEXITCODE -ne 0) {
        $detail = ($names | Out-String).Trim()
        throw "Could not list tables in database '$Database' on container '$ContainerName'. $detail"
    }

    return @(
        $names |
            ForEach-Object { $_.ToString().Trim() } |
            Where-Object { $_ -match '^[A-Za-z_][A-Za-z0-9_]*\.[A-Za-z_][A-Za-z0-9_]*$' }
    )
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
    $parts.Add('docker')
    $parts.Add('exec')
    if (-not [string]::IsNullOrEmpty($env:PGPASSWORD)) {
        $parts.Add('-e')
        $parts.Add('PGPASSWORD')
    }

    $parts.Add($ContainerName)
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

    $dockerArguments = [string[]]@(Get-DockerArguments -CommandArguments (@('pg_dump') + [string[]]$PgDumpArguments))
    $result = Invoke-Docker -Arguments $dockerArguments -StandardOutputPath $Plan.Path
    if ($result.ExitCode -ne 0) {
        if (Test-Path -LiteralPath $Plan.Path) {
            Remove-Item -LiteralPath $Plan.Path -Force
        }
        Write-DockerFailure -Result $result -Activity "pg_dump for $($Plan.FileName)"
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

Assert-SafeArgument -Argument $ContainerName -Label 'Container name'
Assert-SafeArgument -Argument $Username -Label 'Username'
Assert-SafeArgument -Argument $Database -Label 'Database'

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw 'docker was not found on PATH.'
}

if (-not (Test-Path -LiteralPath $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
}
$OutputDirectory = (Resolve-Path -LiteralPath $OutputDirectory).Path

$previousPassword = $env:PGPASSWORD
if ($PSBoundParameters.ContainsKey('Password')) {
    $env:PGPASSWORD = $Password
}

try {
    $inspect = & docker inspect -f '{{.State.Running}}' $ContainerName 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker could not inspect container '$ContainerName'. $(($inspect | Out-String).Trim())"
    }
    $running = @($inspect | ForEach-Object { $_.ToString().Trim() } | Where-Object { $_ }) | Select-Object -Last 1
    if ($running -ne 'true') {
        throw "Container '$ContainerName' is not running."
    }

    $schemaPlan = @(Get-PlannedExports -Exports $schemaExports -RangeStart 0 -RangeEnd 99 -Band 'schema')
    $dataPlan = @(Get-PlannedExports -Exports $dataExports -RangeStart 100 -RangeEnd 799 -Band 'data')
    $postPlan = @(Get-PlannedExports -Exports $postExports -RangeStart 800 -RangeEnd 999 -Band 'post-data')

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
    $omitted = @(
        $databaseTables |
            Where-Object {
                $key = $_.ToLowerInvariant()
                -not $listedTableOwners.ContainsKey($key) -and -not $excludedSet.ContainsKey($key)
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
    if ($PSBoundParameters.ContainsKey('Password')) {
        if ($null -eq $previousPassword) {
            Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
        }
        else {
            $env:PGPASSWORD = $previousPassword
        }
    }
}
