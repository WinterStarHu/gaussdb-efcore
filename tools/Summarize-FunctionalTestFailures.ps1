param(
    [Parameter(Mandatory = $true)]
    [string]$TrxPath,

    [string]$ReportPath,

    [string]$CsvPath,

    [string]$JsonPath
)

$ErrorActionPreference = 'Stop'

if (-not $ReportPath) {
    $ReportPath = Join-Path (Split-Path -Parent $TrxPath) 'failure-summary.md'
}

if (-not $CsvPath) {
    $CsvPath = Join-Path (Split-Path -Parent $TrxPath) 'failed-tests.csv'
}

if (-not $JsonPath) {
    $JsonPath = Join-Path (Split-Path -Parent $TrxPath) 'failure-summary.json'
}

function Get-FailureCategory {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Text
    )

    $rules = @(
        @{ Category = 'actionable-connection'; Reason = 'transport connection failure'; Pattern = 'Failed to connect to|actively refused|No such host is known|timeout expired|Connection refused|SocketException|08001|08006|08000|transport connection' },
        @{ Category = 'actionable-connection'; Reason = 'authentication failed'; Pattern = '28P01|password authentication failed|Invalid username/password|invalid username/password' },
        @{ Category = 'actionable-connection'; Reason = 'account locked'; Pattern = 'account.*locked|The account has been locked|账户.*锁定' },
        @{ Category = 'actionable-connection'; Reason = 'container or server unavailable'; Pattern = 'connection terminated unexpectedly|server closed the connection unexpectedly|remaining connection slots are reserved' },
        @{ Category = 'informational-compatibility'; Reason = 'extension support missing'; Pattern = 'EXTENSION is not yet supported|could not open extension control file|DROP EXTENSION|CREATE EXTENSION|pg_available_extensions' },
        @{ Category = 'informational-compatibility'; Reason = 'database object missing after incompatible initialization'; Pattern = '42P01: relation .* does not exist|relation ".*" does not exist|42P01' },
        @{ Category = 'informational-compatibility'; Reason = 'database file or resource missing'; Pattern = '58P01|could not open file|No such file or directory' },
        @{ Category = 'informational-compatibility'; Reason = 'feature not supported by openGauss'; Pattern = '0A000|feature not supported|is not yet supported|not supported' },
        @{ Category = 'informational-compatibility'; Reason = 'function or operator missing'; Pattern = '42883|function .* does not exist|operator does not exist' },
        @{ Category = 'informational-compatibility'; Reason = 'database object missing'; Pattern = '42704|does not exist on gaussdb' }
    )

    foreach ($rule in $rules) {
        if ($Text -match $rule.Pattern) {
            return [pscustomobject]@{
                Category = $rule.Category
                Reason = $rule.Reason
            }
        }
    }

    return [pscustomobject]@{
        Category = 'informational-other'
        Reason = 'unclassified test failure'
    }
}

function Get-OneLine {
    param(
        [AllowNull()]
        [string]$Value
    )

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return ''
    }

    return (($Value -split "(`r`n|`n|`r)" | Where-Object { $_.Trim() -ne '' } | Select-Object -First 1) -replace '\s+', ' ').Trim()
}

[xml]$trx = Get-Content -Raw $TrxPath
$namespaceManager = New-Object System.Xml.XmlNamespaceManager($trx.NameTable)
$namespaceManager.AddNamespace('t', 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010')

$failedResults = $trx.SelectNodes('//t:UnitTestResult[@outcome="Failed"]', $namespaceManager)
$items = foreach ($result in $failedResults) {
    $message = $result.SelectSingleNode('t:Output/t:ErrorInfo/t:Message', $namespaceManager).'#text'
    $stackTrace = $result.SelectSingleNode('t:Output/t:ErrorInfo/t:StackTrace', $namespaceManager).'#text'
    $combinedText = @($message, $stackTrace) -join "`n"
    $classification = Get-FailureCategory -Text $combinedText
    $sqlStateMatch = [regex]::Match($combinedText, '\b[0-9A-Z]{5}\b')

    [pscustomobject]@{
        TestName = $result.testName
        Category = $classification.Category
        Reason = $classification.Reason
        SqlState = if ($sqlStateMatch.Success) { $sqlStateMatch.Value } else { '' }
        Evidence = Get-OneLine -Value $message
    }
}

$items = $items | Sort-Object Category, TestName
$items | Export-Csv -Path $CsvPath -NoTypeInformation -Encoding UTF8

$counts = [ordered]@{
    'actionable-connection' = @($items | Where-Object Category -eq 'actionable-connection').Count
    'informational-compatibility' = @($items | Where-Object Category -eq 'informational-compatibility').Count
    'informational-other' = @($items | Where-Object Category -eq 'informational-other').Count
}

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# EFCore.GaussDB.FunctionalTests Docker Triage')
$lines.Add('')
$lines.Add(('- TRX: `{0}`' -f $TrxPath))
$lines.Add(('- Failed tests: {0}' -f $items.Count))
$lines.Add(('- Actionable connection/setup failures: {0}' -f $counts['actionable-connection']))
$lines.Add(('- Informational compatibility failures: {0}' -f $counts['informational-compatibility']))
$lines.Add(('- Informational other failures: {0}' -f $counts['informational-other']))
$lines.Add('')

foreach ($category in @('actionable-connection', 'informational-compatibility', 'informational-other')) {
    $categoryItems = @($items | Where-Object Category -eq $category)
    $lines.Add("## $category")
    $lines.Add('')

    if ($categoryItems.Count -eq 0) {
        $lines.Add('No failures in this category.')
        $lines.Add('')
        continue
    }

    $lines.Add('| Test | Reason | SQLSTATE | Evidence |')
    $lines.Add('| --- | --- | --- | --- |')
    foreach ($item in $categoryItems) {
        $safeTest = $item.TestName.Replace('|', '\|')
        $safeReason = $item.Reason.Replace('|', '\|')
        $safeState = $item.SqlState.Replace('|', '\|')
        $safeEvidence = $item.Evidence.Replace('|', '\|')
        $lines.Add("| $safeTest | $safeReason | $safeState | $safeEvidence |")
    }

    $lines.Add('')
}

Set-Content -Path $ReportPath -Value $lines -Encoding UTF8

$summary = [pscustomobject]@{
    TrxPath = $TrxPath
    ReportPath = $ReportPath
    CsvPath = $CsvPath
    TotalFailed = $items.Count
    Categories = $counts
}

$summary | ConvertTo-Json -Depth 4 | Set-Content -Path $JsonPath -Encoding UTF8
$summary
