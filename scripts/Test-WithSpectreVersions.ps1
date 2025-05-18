# PowerShell script to test with specific Spectre.Console.Cli versions
# Test-WithSpectreVersions.ps1

param(
    [Parameter(Mandatory=$false)]
    [string[]]$SpectreVersions,
    
    [Parameter(Mandatory=$false)]
    [switch]$UseVersionsFromJson
)

# Working directory - adjust if needed
$workingDirectory = ".\src"
Set-Location $workingDirectory

# If no versions specified and -UseVersionsFromJson is set, read from JSON
if ($UseVersionsFromJson -and !$SpectreVersions) {
    $jsonPath = ".\..\\.github\package-versions\spectre-console-cli-versions.json"
    
    if (Test-Path $jsonPath) {
        $versionData = Get-Content $jsonPath | ConvertFrom-Json
        $SpectreVersions = $versionData.versions
    }
    else {
        Write-Warning "Version JSON file not found at: $jsonPath"
        Write-Host "Falling back to default versions"
        $SpectreVersions = @('0.46.1-preview.0.19', '0.47.0', '0.48.0', '0.48.1-preview.0.35')
    }
}
elseif (!$SpectreVersions) {
    # Default if no versions specified and not using JSON
    $SpectreVersions = @('0.46.1-preview.0.19', '0.47.0', '0.48.0', '0.48.1-preview.0.35')
}

# Show which versions we'll be testing with
Write-Host "Testing with these Spectre.Console.Cli versions:" -ForegroundColor Cyan
$SpectreVersions | ForEach-Object { Write-Host "- $_" -ForegroundColor Cyan }

# Initial cleanup
Write-Host "`nCleaning solution..." -ForegroundColor Yellow
dotnet clean CiFilter.slnf

# For each version, run the tests
foreach ($version in $SpectreVersions) {
    Write-Host "`n====================================================" -ForegroundColor Green
    Write-Host "Testing with Spectre.Console.Cli version: $version" -ForegroundColor Green
    Write-Host "====================================================" -ForegroundColor Green
    
    # Remove existing packages
    Write-Host "Removing existing Spectre packages..." -ForegroundColor Yellow
    dotnet remove ./JKToolKit.Spectre.AutoCompletion.Tests/JKToolKit.Spectre.AutoCompletion.Tests.csproj package Spectre.Console
    dotnet remove ./JKToolKit.Spectre.AutoCompletion.Tests/JKToolKit.Spectre.AutoCompletion.Tests.csproj package Spectre.Console.Cli
    dotnet remove ./JKToolKit.Spectre.AutoCompletion.Tests/JKToolKit.Spectre.AutoCompletion.Tests.csproj package Spectre.Console.Testing

    # Add packages with specific version
    Write-Host "Installing Spectre packages version $version..." -ForegroundColor Yellow
    dotnet add ./JKToolKit.Spectre.AutoCompletion.Tests/JKToolKit.Spectre.AutoCompletion.Tests.csproj package Spectre.Console --version $version
    dotnet add ./JKToolKit.Spectre.AutoCompletion.Tests/JKToolKit.Spectre.AutoCompletion.Tests.csproj package Spectre.Console.Cli --version $version
    dotnet add ./JKToolKit.Spectre.AutoCompletion.Tests/JKToolKit.Spectre.AutoCompletion.Tests.csproj package Spectre.Console.Testing --version $version

    # Restore, build, and test
    Write-Host "Restoring packages..." -ForegroundColor Yellow
    dotnet restore CiFilter.slnf

    Write-Host "Building solution..." -ForegroundColor Yellow
    dotnet build CiFilter.slnf --no-restore

    Write-Host "Running tests..." -ForegroundColor Yellow
    dotnet test CiFilter.slnf --no-build
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed with version $version" -ForegroundColor Red
    }
    else {
        Write-Host "Tests passed with version $version" -ForegroundColor Green
    }
}

Write-Host "`nTesting completed for all versions." -ForegroundColor Cyan
