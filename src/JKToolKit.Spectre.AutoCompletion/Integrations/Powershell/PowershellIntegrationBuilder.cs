using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JKToolKit.Spectre.AutoCompletion.Helpers;
using JKToolKit.Spectre.AutoCompletion.Properties;

public class StartArgs
{
    public string Runtime { get; }
    public string Command { get; }

    public string CommandName => Path.GetFileNameWithoutExtension(Command);

    public string CommandExtension => Path.GetExtension(Command);

    public StartArgs(string runtime, string command)
    {
        Runtime = runtime;
        Command = command;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Runtime))
        {
            return Command;
        }

        return string.Join(" ", Runtime, Command);
    }

    public static StartArgs ParseStartArgs(params string[] args)
    {
        var command = args[0];
        if (command.EndsWith(".dll"))
        {
            return new StartArgs("dotnet", command);
        }

        var commandIsDotnet = Path.GetFileNameWithoutExtension(command)
            .Equals("dotnet", StringComparison.OrdinalIgnoreCase);

        if (commandIsDotnet)
        {
            return new StartArgs(command, args[1]);
        }

        return new StartArgs(string.Empty, args[0]);
    }

    public static StartArgs GetSelfStartCommandFromCommandLineArgs()
    {
        var args = Environment.GetCommandLineArgs();
        return ParseStartArgs(args);
    }
}

public record PowershellIntegrationAlias(string Alias, string[] Command);

public class PowershellIntegrationBuilderSettings
{
    public bool Diagnostic { get; set; }

    public bool Install { get; set; }
    public StartArgs? StartArgs { get; set; }

    public List<PowershellIntegrationAlias> Aliases { get; set; } = new();
}

public static class PowershellIntegrationBuilder
{
    public static string BuildIntegration(PowershellIntegrationBuilderSettings settings)
    {
        var startArgs = settings.StartArgs ?? StartArgs.GetSelfStartCommandFromCommandLineArgs();
        var startCommand = string.IsNullOrEmpty(startArgs.Runtime)
            ? "& \"" + startArgs.Command + "\""
            : "& \"" + startArgs.Runtime + "\" \"" + startArgs.Command + "\"";

        // startCommand is either "C:\Users\jkams\Documents\PowerShell\Tools\Apget\ApGet.exe"
        // or dotnet "C:\Users\jkams\Documents\PowerShell\Tools\Apget\ApGet.dll"
        // localStartCommand should be either "./ApGet.exe" or "dotnet ./ApGet.dll"
        var localCommand = startArgs.CommandName + startArgs.CommandExtension;
        var localStartCommand = string.IsNullOrEmpty(startArgs.Runtime)
            ? "& \".\\" + localCommand + "\""
            : "& \"" + startArgs.Runtime + "\" \".\\" + localCommand + "\"";

        var replacements = new Dictionary<string, string>
        {
            ["[RUNCOMMAND]"] = startCommand,
            ["[RUNCOMMAND_LOCAL]"] = localStartCommand,
            ["[COMMAND_LOCAL]"] = localCommand,
            ["[APPNAME]"] = startArgs.CommandName,
            ["[APPNAME_LowerCase]"] = startArgs.CommandName.ToLowerInvariant(),
        };

        if (settings.Diagnostic)
        {
            /*
                "C:\repos\JKamsker\JKToolKit.Spectre.AutoCompletion\src\samples\AutoCompletionExample\bin\Debug\net8.0> .\AutoCompletionExample.exe completion powershell -d | Out-String"
                gives:

                CommandName: AutoCompletionExample
                Command: C:\repos\JKamsker\JKToolKit.Spectre.AutoCompletion\src\samples\AutoCompletionExample\bin\Debug\net8.0\AutoCompletionExample.dll
                Runtime: dotnet
                StartCommand: & "dotnet" "C:\repos\JKamsker\JKToolKit.Spectre.AutoCompletion\src\samples\AutoCompletionExample\bin\Debug\net8.0\AutoCompletionExample.dll"
                Install: False
                CommandLine: C:\repos\JKamsker\JKToolKit.Spectre.AutoCompletion\src\samples\AutoCompletionExample\bin\Debug\net8.0\AutoCompletionExample.dll completion powershell -d
             */

            System.Console.WriteLine($"CommandName: {startArgs.CommandName}");
            System.Console.WriteLine($"Command: {startArgs.Command}");
            System.Console.WriteLine($"Runtime: {startArgs.Runtime}");
            System.Console.WriteLine($"StartCommand: {startCommand}");
            System.Console.WriteLine($"Install: {settings.Install}");

            var args = string.Join(" ", Environment.GetCommandLineArgs());
            System.Console.WriteLine($"CommandLine: {args}");
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.AppendLine(GetResource(Resources.PowershellIntegration_Completion_and_alias, replacements));
        if (settings.Install)
        {
            sb.AppendLine(GetResource(Resources.PowershellIntegration_Install, replacements));
        }

        if (settings.Aliases?.Count > 0)
        {
            foreach (var alias in settings.Aliases)
            {
                sb.AppendLine(GetResource(Resources.PowershellIntegration_alias, new Dictionary<string, string>
                {
                    ["[ALIAS_NAME]"] = alias.Alias,
                    ["[PARAMS]"] = string.Join(" ", alias.Command),
                    ["[INVOKE_NAME]"] =
                        $"Invoke-{startArgs.CommandName}" // e.g. Invoke-AutoCompletionExample from PowershellIntegration_Completion_and_alias
                }));
            }
        }


        return sb.ToString();
    }

    // removes everything after a comment in a line
    // removes empty lines (whitespace also counts as empty)
    // private static void Cleanup(StringBuilder sb)
    // {
    //     for (int i = 0; i < sb.Length; i++)
    //     {
    //         if (sb[i] == '#')
    //         {
    //             var nextLineBreak = sb.IndexOf('\n', i);
    //             var length = nextLineBreak == -1 
    //                 ? sb.Length - i 
    //                 : nextLineBreak - i;
    //             
    //             sb.Remove(i, length);
    //         }
    //     }
    //     
    //     // Remove empty lines (whitespace also counts as empty)
    //     for (int i = 0; i < sb.Length; i++)
    //     {
    //         var indexOfLineBreak = sb.IndexOf('\n', i);
    //         if (indexOfLineBreak == -1)
    //         {
    //             break;
    //         }
    //         
    //         var isWhitespace = sb.IsWhiteSpace(i, indexOfLineBreak);
    //         if (isWhitespace)
    //         {
    //             sb.Remove(i, indexOfLineBreak - i);
    //         }
    //     }
    // }


    private static string GetResource(byte[] powershellIntegration_Install, Dictionary<string, string> replacements)
    {
        var result = Encoding.UTF8.GetString(powershellIntegration_Install);
        return ApplyReplacements(replacements, result);
    }

    private static string GetResource(string resourceName, Dictionary<string, string> replacements)
    {
        var result = Resources.ResourceManager.GetString(resourceName) ??
                     throw new InvalidOperationException($"Could not find resource '{resourceName}'.");
        return ApplyReplacements(replacements, result);
    }

    private static string ApplyReplacements(Dictionary<string, string> replacements, string result)
    {
        foreach (var replacement in replacements)
        {
            result = result.Replace(replacement.Key, replacement.Value);
        }

        return result;
    }
}