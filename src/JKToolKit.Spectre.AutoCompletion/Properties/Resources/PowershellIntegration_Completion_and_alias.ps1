
function Invoke-[APPNAME] {
    if (Test-Path -PathType Leaf -Path ".\[COMMAND_LOCAL]") {
      [RUNCOMMAND_LOCAL] $args
      return
    }
    return [RUNCOMMAND] $args
}

# Register-CompleterFor -CommandName [APPNAME]
function Register-CompleterFor{
    #appname parameter
    param(
        [Parameter(Mandatory=$true)]
        [string]$name
    )

    Register-ArgumentCompleter -Native -CommandName $name -ScriptBlock {
        param($commandName, $wordToComplete, $cursorPosition)
        # $completions = [RUNCOMMAND] completion complete --position $cursorPosition "$wordToComplete"
        $completions = @()
        if(Test-Path -PathType Leaf -Path ".\[COMMAND_LOCAL]"){
            $completions = [RUNCOMMAND_LOCAL] completion complete --position $cursorPosition "$wordToComplete"
        }
        else{
            $completions = [RUNCOMMAND] completion complete --position $cursorPosition "$wordToComplete"
        }
        
        if ($completions) {
            foreach ($completion in $completions) {
                [System.Management.Automation.CompletionResult]::new($completion, $completion, 'ParameterValue', $completion)
            }
        }
        else {
            $null
        }
    }
}

Set-alias -name [APPNAME] -Value Invoke-[APPNAME]
Set-alias -name [APPNAME_LowerCase] -Value Invoke-[APPNAME]

Register-CompleterFor -name "[APPNAME]"
Register-CompleterFor -name "[APPNAME_LowerCase]"

Register-CompleterFor -name "Invoke-[APPNAME]"