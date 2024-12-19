using JKToolKit.Spectre.AutoCompletion.Completion;
using Spectre.Console.Cli;
using System.Diagnostics;
using JKToolKit.Spectre.AutoCompletion.Integrations;

namespace AutoCompletionExample;

// Adding autocomplete to powershell:
//  - .\AutoCompletionExample.exe completion powershell
//
// Adding autocomplete to powershell (permanent):
// - .\AutoCompletionExample.exe completion powershell --install
//
// Test completing:
// - .\AutoCompletionExample.exe cli complete "myapp Li"
internal static class Program
{
    private static void Main(string[] args)
    {
        // If we just want to test the completion with f5 in visual studio
        if (Debugger.IsAttached)
        {
            args = new[] { "completion", "complete", "\"myapp Li\"" };
        }

        var app = new CommandApp();
        app.Configure
        (
            config =>
            {
                config
                    .AddAutoCompletion(x => x.AddPowershell(opts => { opts.WithAlias("myls", [ "ls" ]); }))
                    .AddCommand<LionCommand>("lion")
                    ;

                config.AddCommand<LsCommand>("ls")
                    .WithDescription("List files and directories in the current directory.");
            }
        );

        app.Run(args);
    }
}

/*
 export SAMPLE_DLL=/mnt/c/Users/W31rd0/source/repos/JKamsker/JKToolKit/src/Samples/AutoCompletionExample/bin/Release/net7.0/publish/AutoCompletionExample.dll

# using SAMPLE_DLL: dotnet $SAMPLE_DLL
# completion: dotnet $SAMPLE_DLL cli complete "myapp li"

# bash parameter completion for the dotnet CLI

function _dotnet_bash_complete()
{
  local cur="${COMP_WORDS[COMP_CWORD]}" IFS=$'\n' # On Windows you may need to use use IFS=$'\r\n'
  local candidates

  read -d '' -ra candidates < <(dotnet $SAMPLE_DLL cli complete --position "${COMP_POINT}" "${COMP_LINE}" 2>/dev/null)

  read -d '' -ra COMPREPLY < <(compgen -W "${candidates[*]:-}" -- "$cur")
}

complete -f -F _dotnet_bash_complete AutoCompletionExample
 */