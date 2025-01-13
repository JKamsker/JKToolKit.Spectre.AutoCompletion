
function Invoke-[APPNAME] {
    $invokName = $MyInvocation.InvocationName
    $actualCommand = $global:aliases[$invokName]

    # if actualName is not null or empty, then we need to prepend the actualName to the arguments
    if ($null -ne $actualCommand -and "" -ne $actualCommand) {
        # args is an array
        $args = @($actualCommand) + $args
    }

    if (Test-Path -PathType Leaf -Path ".\[COMMAND_LOCAL]") {
        return [RUNCOMMAND_LOCAL] $args
    }
    return [RUNCOMMAND] $args
}

function Register-CompleterFor{
    param(
        [Parameter(Mandatory=$true)][string]$name

    )

    Register-ArgumentCompleter -Native -CommandName $name -ScriptBlock {
        param($commandName, [System.Management.Automation.Language.CommandAst] $wordToComplete, $cursorPosition)

        $aliases = $global:aliases

        $stringWordToComplete = "$wordToComplete"

        try {
            $firstCommand = $wordToComplete.CommandElements[0].Extent.Text

            $remainingCommands = $null;
            if($wordToComplete.CommandElements.Count -gt 1){
                $remainingCommandsTmp = $wordToComplete.CommandElements[1..($wordToComplete.CommandElements.Count - 1)] 
                        | Select-Object -ExpandProperty Extent 
                        | Select-Object -ExpandProperty Text

                $remainingCommands = " $($remainingCommandsTmp -join " ")"
            }
            else{
                $remainingCommands = ""
            }

            # $remainingCommands =
            #     $wordToComplete.CommandElements[1..($wordToComplete.CommandElements.Count - 1)] | Select-Object -ExpandProperty Extent | Select-Object -ExpandProperty Text

            $params = $aliases[$firstCommand]

            if ($null -ne $params -and "" -ne $params) {

                $paramsString = "$params"

                $newWordToComplete = "my $paramsString$remainingCommands"
                # $newCursorPosition = $cursorPosition + $params.Length - $firstCommand.Length + 3

                $newCursorPosition = $cursorPosition

                $newCursorPosition -= $firstCommand.Length # remove the length of the first command
                $newCursorPosition += 3 # for the space and the "my" command
                $newCursorPosition += $paramsString.Length # add the length of the alias
                
            
                $stringWordToComplete = $newWordToComplete
                $cursorPosition = $newCursorPosition
            }
        }
        catch {
            
        }

        $completions = Invoke-[APPNAME] completion complete --position $cursorPosition "$stringWordToComplete"
        
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