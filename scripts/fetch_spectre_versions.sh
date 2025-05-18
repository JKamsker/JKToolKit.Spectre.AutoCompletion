#!/bin/bash
# fetch_spectre_versions.sh
# Script to fetch all Spectre.Console.Cli package versions from NuGet

# Use NuGet API to query all versions
ALL_VERSIONS=$(curl -s "https://api.nuget.org/v3-flatcontainer/spectre.console.cli/index.json" | jq -r '.versions[]')

# Get the last 20 versions (most recent when sorted)
SELECTED_VERSIONS=$(echo "$ALL_VERSIONS" | sort -V | tail -n 20)

echo "Selected versions: $SELECTED_VERSIONS"

# Format versions into JSON array
echo '{ "versions": [' > versions.json

# Add each version as a JSON string in the array
FIRST=true
for VERSION in $SELECTED_VERSIONS; do
  if [ -z "$VERSION" ]; then
    continue
  fi
  
  if [ "$FIRST" = true ]; then
    echo "  \"$VERSION\"" >> versions.json
    FIRST=false
  else
    echo "  ,\"$VERSION\"" >> versions.json
  fi
done

echo '] }' >> versions.json

# Output versions found
echo "Selected versions for testing:"
cat versions.json

# Store in file
mkdir -p .github/package-versions
mv versions.json .github/package-versions/spectre-console-cli-versions.json
