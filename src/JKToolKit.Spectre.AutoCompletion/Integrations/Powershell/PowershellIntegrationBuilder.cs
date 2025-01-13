using System.Text;
using JKToolKit.Spectre.AutoCompletion.Properties;

namespace JKToolKit.Spectre.AutoCompletion.Integrations.Powershell;

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

        if (settings.Aliases?.Count > 0)
        {
            /*# Globals:
                $global:aliases = @{
                "myls" = "my ls"
                "mylion" = "my lion"
                "my" = ""
            }*/

            var globals = settings.Aliases
                .Where(alias => alias.Command?.Length > 0)
                .Select(alias => $"\t\"{alias.Alias}\" = {alias.MakeGlobalAliasValue()}");

            sb.AppendLine("$global:aliases = @{");
            foreach (var global in globals)
            {
                sb
                    .AppendLine(global);
            }

            sb.AppendLine("}");
        }


        sb.AppendLine(GetResource(Resources.PowershellIntegration_Completion_and_alias, replacements));
        if (settings.Install)
        {
            sb.AppendLine(GetResource(Resources.PowershellIntegration_Install, replacements));
        }

        if (settings.Aliases?.Count > 0)
        {
            foreach (var alias in settings.Aliases)
            {
                if (true || alias.Command?.Length is null or 0)
                {
                    // Set-alias -name AutoCompletionExample -Value Invoke-AutoCompletionExample
                    // Register-CompleterFor -name "AutoCompletionExample"

                    // Set-alias -name [APPNAME] -Value Invoke-[APPNAME]

                    sb.AppendLine($"Set-alias -name {alias.Alias} -Value Invoke-{startArgs.CommandName}");
                    sb.AppendLine($"Register-CompleterFor -name \"{alias.Alias}\"");

                    continue;
                }

                sb.AppendLine(GetResource(Resources.PowershellIntegration_alias, new Dictionary<string, string>
                {
                    ["[ALIAS_NAME]"] = alias.Alias,
                    ["[PARAMS]"] = alias.Command == null
                        ? string.Empty
                        : string.Join(" ", alias.Command),

                    ["[INVOKE_NAME]"] =
                        $"Invoke-{startArgs.CommandName}" // e.g. Invoke-AutoCompletionExample from PowershellIntegration_Completion_and_alias
                }));
            }
        }

        return sb.ToString();
    }

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