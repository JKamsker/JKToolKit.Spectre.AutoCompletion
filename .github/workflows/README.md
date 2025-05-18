# GitHub Actions for Spectre.Console.Cli Version Testing

This directory contains GitHub Actions workflows for testing compatibility across multiple versions of Spectre.Console.Cli.

## Workflows

### 1. Update Package Versions (`update-package-versions.yml`)

This workflow runs daily at midnight UTC to:

- Query NuGet.org API for all available versions of Spectre.Console.Cli
- Select a subset of versions for testing:
  - The latest patch releases from the last two stable minor versions
  - The latest two preview releases
- Store these versions in a JSON file: `.github/package-versions/spectre-console-cli-versions.json`
- Commit and push changes only if the versions have changed

You can also trigger this workflow manually from the GitHub Actions tab.

### 2. Build and Test (`build-test.yml`)

This workflow runs:

- On every push and pull request
- When the "Update Spectre Package Versions" workflow completes

It uses the versions from the JSON file created by the first workflow to build and test the project against multiple Spectre.Console.Cli versions. If the JSON file doesn't exist, it uses a hardcoded set of versions.

## How It Works

1. The daily version check keeps the test suite updated with the latest Spectre.Console.Cli versions
2. The build workflow uses dynamic matrix strategy to test against these versions
3. The two workflows are linked - when new versions are detected, tests automatically run against them

This ensures continuous compatibility testing with minimal manual intervention.
