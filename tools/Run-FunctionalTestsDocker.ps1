param(
    [string]$ProjectPath = 'D:\code\gaussdb-efcore\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj',

    [string]$ContainerName = 'gaussdb-efcore-opengauss',

    [string]$ConnectionString = 'Server=localhost;Username=npgsql_tests;Password=NpGsql@123;Port=5432;Database=postgres',

    [string]$ArtifactsRoot = 'D:\code\gaussdb-efcore\artifacts\functionaltests-docker',

    [string]$BlameHangTimeout = '2m',

    [switch]$EnableDiag,

    [switch]$SmokeOnly
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_HOME = 'D:\code\.dotnet'
$env:Test__GaussDB__DefaultConnection = $ConnectionString

$probeProject = Join-Path $PSScriptRoot 'FunctionalTestsDockerProbe\FunctionalTestsDockerProbe.csproj'
$summaryScript = Join-Path $PSScriptRoot 'Summarize-FunctionalTestFailures.ps1'
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$runDirectory = Join-Path $ArtifactsRoot $timestamp
New-Item -ItemType Directory -Path $runDirectory -Force | Out-Null

function Write-Step {
    param([string]$Message)
    Write-Host "[functionaltests-docker] $Message"
}

function Quote-Argument {
    param([string]$Value)

    if ($Value -match '[\s;"]') {
        return '"' + $Value.Replace('"', '\"') + '"'
    }

    return $Value
}

function Join-Arguments {
    param([string[]]$Values)
    return ($Values | ForEach-Object { Quote-Argument $_ }) -join ' '
}

function Invoke-RedirectedProcess {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,

        [Parameter(Mandatory = $true)]
        [string]$StdOutPath,

        [Parameter(Mandatory = $true)]
        [string]$StdErrPath
    )

    $process = Start-Process `
        -FilePath $FilePath `
        -ArgumentList (Join-Arguments $Arguments) `
        -WorkingDirectory (Split-Path -Parent $ProjectPath) `
        -NoNewWindow `
        -Wait `
        -PassThru `
        -RedirectStandardOutput $StdOutPath `
        -RedirectStandardError $StdErrPath

    return $process.ExitCode
}

function Test-TcpPort {
    param(
        [string]$HostName,
        [int]$Port,
        [int]$TimeoutMs = 5000
    )

    $client = [System.Net.Sockets.TcpClient]::new()
    try {
        $async = $client.BeginConnect($HostName, $Port, $null, $null)
        if (-not $async.AsyncWaitHandle.WaitOne($TimeoutMs, $false)) {
            return $false
        }

        $client.EndConnect($async)
        return $true
    }
    catch {
        return $false
    }
    finally {
        $client.Dispose()
    }
}

function Mask-ConnectionString {
    param([string]$Value)
    return ($Value -replace '(?i)(Password=)([^;]+)', '$1***')
}

$runInfo = [ordered]@{
    ContainerName = $ContainerName
    ProjectPath = $ProjectPath
    ConnectionString = Mask-ConnectionString $ConnectionString
    BlameHangTimeout = $BlameHangTimeout
    EnableDiag = [bool]$EnableDiag
    SmokeOnly = [bool]$SmokeOnly
    RunDirectory = $runDirectory
}

$runInfoPath = Join-Path $runDirectory 'run-metadata.json'
$runInfo | ConvertTo-Json -Depth 4 | Set-Content -Path $runInfoPath -Encoding UTF8

Write-Step "Artifacts: $runDirectory"

$containerInfo = & docker.exe ps -a --filter "name=$ContainerName" --format "{{.Names}}|{{.Status}}|{{.Ports}}"
if (-not $containerInfo) {
    throw "Container '$ContainerName' was not found."
}

$containerFields = $containerInfo -split '\|', 3
$containerStatus = if ($containerFields.Length -ge 2) { $containerFields[1] } else { '' }
if ($containerStatus -notmatch '^Up\b') {
    throw "Container '$ContainerName' is not running. Status: $containerStatus"
}

Write-Step "Container is running: $containerStatus"

if (-not (Test-TcpPort -HostName '127.0.0.1' -Port 5432)) {
    throw 'Host port 5432 is not reachable from the Windows host.'
}

Write-Step 'Host port 5432 is reachable.'

$probeBuildOut = Join-Path $runDirectory 'probe-build.stdout.log'
$probeBuildErr = Join-Path $runDirectory 'probe-build.stderr.log'
$probeBuildExit = Invoke-RedirectedProcess `
    -FilePath 'dotnet' `
    -Arguments @('build', $probeProject, '--nologo', '-v:minimal', '-p:RestoreIgnoreFailedSources=true') `
    -StdOutPath $probeBuildOut `
    -StdErrPath $probeBuildErr
if ($probeBuildExit -ne 0) {
    throw "Probe build failed. See $probeBuildOut and $probeBuildErr"
}

$probeOut = Join-Path $runDirectory 'probe.stdout.log'
$probeErr = Join-Path $runDirectory 'probe.stderr.log'
$probeExit = Invoke-RedirectedProcess `
    -FilePath 'dotnet' `
    -Arguments @('run', '--project', $probeProject, '--no-build', '--no-restore', '--', '--connection', $ConnectionString) `
    -StdOutPath $probeOut `
    -StdErrPath $probeErr
if ($probeExit -ne 0) {
    throw "Authentication probe failed. See $probeOut and $probeErr"
}

Write-Step 'Authentication probe succeeded.'

if ($SmokeOnly) {
    Write-Step 'SmokeOnly requested, skipping full suite.'
    exit 0
}

$testStdOut = Join-Path $runDirectory 'dotnet-test.stdout.log'
$testStdErr = Join-Path $runDirectory 'dotnet-test.stderr.log'
$trxPath = Join-Path $runDirectory 'functionaltests.trx'
$diagPath = Join-Path $runDirectory 'dotnet-test.diag.log'
$testArguments = @(
    'test',
    $ProjectPath,
    '--no-build',
    '-v:q',
    '--results-directory',
    $runDirectory,
    '--logger',
    'console;verbosity=quiet',
    '--logger',
    'trx;LogFileName=functionaltests.trx',
    '--blame-hang',
    '--blame-hang-timeout',
    $BlameHangTimeout,
    '--blame-hang-dump-type',
    'none'
)

if ($EnableDiag) {
    $testArguments += @('--diag', $diagPath)
}

$testExit = Invoke-RedirectedProcess `
    -FilePath 'dotnet' `
    -Arguments $testArguments `
    -StdOutPath $testStdOut `
    -StdErrPath $testStdErr

Write-Step "dotnet test exit code: $testExit"

if (-not (Test-Path $trxPath)) {
    throw "TRX result file was not created. See $testStdOut and $testStdErr"
}

$summary = & $summaryScript -TrxPath $trxPath
$summaryExitPath = Join-Path $runDirectory 'workflow-summary.json'

$workflowSummary = [pscustomobject]@{
    TestExitCode = $testExit
    RunDirectory = $runDirectory
    TrxPath = $trxPath
    FailureSummary = $summary
}

$workflowSummary | ConvertTo-Json -Depth 6 | Set-Content -Path $summaryExitPath -Encoding UTF8

Write-Step "Failure summary: $($summary.ReportPath)"
Write-Step "Failure csv: $($summary.CsvPath)"

if ($summary.Categories.'actionable-connection' -gt 0) {
    exit 2
}

exit 0
