# adds for eg. Write-Host "Hello World! [Randomnumber]" to profile only once, otherwise does it overwrites it
function Add-ProfileLine {
    param(
        [string]$identifier,
        [string]$Line

    )
    $ProfilePath = $null
    try { $ProfilePath = $PROFILE.CurrentUserAllHosts } catch { }
    if ([string]::IsNullOrWhiteSpace($ProfilePath)) { $ProfilePath = $PROFILE }

    # If not exists: Empty string array
    $ProfileContent = @()
    if (Test-Path -Path $ProfilePath) {
        $ProfileContent = [System.IO.File]::ReadAllLines($ProfilePath)
    }

    # we need to remove Id: [identifier] and the line after it (if present)
    $IdLine = "#Id: $identifier"
    $newContent = @()
    $skipNext = $false
    foreach ($content in $ProfileContent) {
        if ($skipNext) {
            $skipNext = $false
            continue
        }

        if ($content -eq $IdLine) {
            $skipNext = $true
            continue
        }

        $newContent += $content
    }
    $ProfileContent = $newContent

    # ProfileContent suddendly is a string instead of a string array
    # make it an array if it is a string
    if ($ProfileContent -is [string]) {
        $ProfileContent = @($ProfileContent)
    }

    # adding the line
    $ProfileContent += $IdLine
    $ProfileContent += $Line

    # writing the file
    $ProfileContent | Set-Content $ProfilePath
}


function Add-Profile-Script {
    param(
        [string]$identifier,
        [string]$scriptContent
    )

    $ProfilePath = $null
    try { $ProfilePath = $PROFILE.CurrentUserAllHosts } catch { }
    if ([string]::IsNullOrWhiteSpace($ProfilePath)) { $ProfilePath = $PROFILE }
    $ScriptsPath = Join-Path (Split-Path $ProfilePath) "Scripts"
    $ScriptPath = Join-Path $ScriptsPath "$identifier.ps1"

    # Create the Scripts directory if it doesn't exist
    if (-not (Test-Path -Path $ScriptsPath -PathType Container)) {
        New-Item -ItemType Directory -Path $ScriptsPath -Force
    }
    
    # Write the content to the file
    # overwrite if it exists
    $scriptContent | Set-Content $ScriptPath
    Add-ProfileLine -identifier $identifier -Line "if (Test-Path '$ScriptPath') { . '$ScriptPath' }"
}

$scriptContent = [RUNCOMMAND] completion powershell | Out-String
Add-Profile-Script -identifier [APPNAME_LowerCase] -scriptContent $scriptContent
