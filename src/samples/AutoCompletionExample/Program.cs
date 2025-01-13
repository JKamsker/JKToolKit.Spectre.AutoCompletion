using JKToolKit.Spectre.AutoCompletion.Completion;
using JKToolKit.Spectre.AutoCompletion.Integrations.Powershell;
using Spectre.Console.Cli;
using System.Diagnostics;

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
            // args = new[] { "completion", "complete", "\"myapp Li\"" };

            // completion complete --position 16 my lion --teeth 
            args = new[] { "completion", "complete", "--position", "16", "my lion --teeth " };
        }

        LogToFile(args);

        var app = new CommandApp();
        app.Configure
        (
            config =>
            {
                config
                    .AddAutoCompletion(x => x.AddPowershell(opts => opts
                        .WithAlias("myls", ["ls"])
                        .WithAlias("my")
                        .WithAlias("mylion", ["lion"])
                        .WithAlias("clion", ["lion", "5"])
                    ))
                    .AddCommand<LionCommand>("lion")
                    ;

                config.AddCommand<LsCommand>("ls")
                    .WithDescription("List files and directories in the current directory.");

                config.PropagateExceptions();
            }
        );

        try
        {
            app.Run(args);
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached)
            {
                throw;
            }

            File.AppendAllText("Error.txt", ex.ToString() +
                                            "\n-------------------\n");
        }
    }

    private static void LogToFile(string[] args)
    {
        if (Debugger.IsAttached)
        {
            return;
        }

        var myargs = args
            .Select(x => x.Contains(" ") ? $"\"{x}\"" : x);

        var log = string.Join(" ", myargs);
        System.IO.File.AppendAllText("log.txt", log + "\n");
    }

    // log stdout to file
    //private static void LogStdoutToFile()
    //{
    //    var stdout = new System.IO.StreamWriter("stdout.txt");
    //    System.Console.SetOut(stdout);
    //}
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