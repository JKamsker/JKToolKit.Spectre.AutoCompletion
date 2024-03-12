# adds for eg. Write-Host "Hello World! [Randomnumber]" to profile only once, otherwise does it overwrites it
function Add-ProfileLine {
    param(
        [string]$identifier,
        [string]$Line

    )
    $ProfilePath = $PROFILE.CurrentUserAllHosts

    # If not exists: Empty string array
    $ProfileContent = @()
    if (Test-Path -Path $ProfilePath) {
        $ProfileContent = [System.IO.File]::ReadAllLines($ProfilePath)
    }

    # we need to remove Id: [identifier] and the line after it
    $IdLine = "#Id: $identifier"
    # $IdLineIndex = $ProfileContent.IndexOf($IdLine)
    if ($IdLineIndex -ne -1 -and $IdLineIndex -lt ($ProfileContent.Count - 1)) {
        $newContent = @()
        $switch = $false
        for ($i = 0; $i -lt $ProfileContent.Count; $i++) {
            $content = $ProfileContent[$i]
            if ($content -eq $IdLine) {
                $switch = $true
                continue
            }
            if ($switch) {
                $switch = $false
                continue
            }

            $newContent += $content
        }
        $ProfileContent = $newContent
    }

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

    $ProfilePath = $profile.CurrentUserAllHosts
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