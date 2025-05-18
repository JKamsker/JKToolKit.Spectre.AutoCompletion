# Spectre.Console.Cli Testing Scripts

This directory contains scripts to help test your project with different versions of Spectre.Console.Cli.

## PowerShell Scripts (Windows)

### Fetch-SpectreVersions.ps1

This script fetches all available Spectre.Console.Cli package versions from NuGet and selects a subset for testing:
- The latest patch versions from the two most recent stable minor versions
- The two most recent preview versions

**Usage:**
```powershell
.\Fetch-SpectreVersions.ps1
```

**Output:**
- Creates a JSON file at `.github\package-versions\spectre-console-cli-versions.json`

### Test-WithSpectreVersions.ps1

This script tests your project with multiple versions of Spectre.Console.Cli.

**Usage:**
```powershell
# Test with default versions
.\Test-WithSpectreVersions.ps1

# Test with versions from the JSON file
.\Test-WithSpectreVersions.ps1 -UseVersionsFromJson

# Test with specific versions
.\Test-WithSpectreVersions.ps1 -SpectreVersions @("0.47.0", "0.48.0")
```

## Bash Script (Linux/macOS)

### fetch_spectre_versions.sh

Bash version of the version fetching script for Linux/macOS.

**Usage:**
```bash
bash fetch_spectre_versions.sh
```

**Note:** This script requires `jq` to be installed:
```bash
# On Ubuntu/Debian
sudo apt install jq

# On macOS with Homebrew
brew install jq
```

## Purpose

These scripts reproduce the GitHub Actions workflows locally, allowing you to:
1. Fetch the latest Spectre.Console.Cli versions
2. Test your project against multiple versions without waiting for CI
