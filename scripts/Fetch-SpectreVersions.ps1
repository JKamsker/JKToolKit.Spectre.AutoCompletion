# PowerShell script to fetch Spectre.Console.Cli package versions from NuGet
# Fetch-SpectreVersions.ps1

# Create directory for output if it doesn't exist
$outputDir = ".\.github\package-versions"
if (!(Test-Path -Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    Write-Host "Created directory: $outputDir"
}

# Use NuGet API to query all versions
Write-Host "Fetching all Spectre.Console.Cli versions from NuGet..."
$allVersions = (Invoke-RestMethod -Uri "https://api.nuget.org/v3-flatcontainer/spectre.console.cli/index.json").versions

# Separate stable and preview versions
$stableVersions = $allVersions | Where-Object { $_ -notmatch "-" } | Sort-Object { [version]($_ -replace '(\d+\.\d+\.\d+).*', '$1') }
$previewVersions = $allVersions | Where-Object { $_ -match "-" } | Sort-Object

# Get latest minor version groups (e.g., 0.46.x, 0.47.x)
$minorVersionGroups = $stableVersions | ForEach-Object { 
    $parts = $_ -split '\.'
    if ($parts.Count -ge 2) {
        "$($parts[0]).$($parts[1])"
    }
} | Sort-Object -Unique

# Get the latest two minor version groups
$latestMinorGroups = $minorVersionGroups | Select-Object -Last 2

# For each minor group, get latest patch
$selectedVersions = @()
foreach ($minorGroup in $latestMinorGroups) {
    $latestPatch = $stableVersions | Where-Object { $_ -like "$minorGroup.*" } | Select-Object -Last 1
    if ($latestPatch) {
        $selectedVersions += $latestPatch
    }
}

# Add latest two preview versions
$latestPreviews = $previewVersions | Select-Object -Last 2
$selectedVersions += $latestPreviews

# Remove empty entries
$selectedVersions = $selectedVersions | Where-Object { $_ }

Write-Host "Selected versions for testing:"
$selectedVersions | ForEach-Object { Write-Host "- $_" }

# Create JSON structure
$versionsObject = @{
    versions = $selectedVersions
}

# Convert to JSON and save to file
$jsonOutput = $versionsObject | ConvertTo-Json
$outputFile = Join-Path $outputDir "spectre-console-cli-versions.json"
$jsonOutput | Out-File -FilePath $outputFile -Encoding utf8

Write-Host "JSON file created at: $outputFile"
Write-Host "Contents:"
Get-Content $outputFile
