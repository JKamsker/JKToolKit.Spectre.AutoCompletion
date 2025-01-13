using JKToolKit.Spectre.AutoCompletion.Completion;
using JKToolKit.Spectre.AutoCompletion.Helpers;
using Spectre.Console.Cli;

namespace JKToolKit.Spectre.AutoCompletion.Integrations.Powershell;

public static class Extensions
{
    public static AutoCompletionConfiguration AddPowershell(
        this AutoCompletionConfiguration settings,
        Action<PowershellIntegrationOptions>? configure = null
    )
    {
        settings.SpectreConfig.AddDelegate<PowershellSettings>("powershell", (context, pwsh) =>
        {
            var options = new PowershellIntegrationOptions();
            configure?.Invoke(options);

            var settings = new PowershellIntegrationBuilderSettings
            {
                Install = options.Install ?? pwsh.Install,
                Aliases = options.Aliases,
                Diagnostic = pwsh.Diagnostic
            };

            var result = PowershellIntegrationBuilder.BuildIntegration(settings);
            using var _ = OutputEncodingHelper.SetOutputEncodingIfNeccessary();
            System.Console.WriteLine(result);
            return 0;
        });

        return settings;
    }

    private class PowershellSettings : CommandSettings
    {
        [CommandOption("-i|--install")] public bool Install { get; set; }
        
        [CommandOption("-d|--diag")] public bool Diagnostic { get; set; }
    }

    public class PowershellIntegrationOptions
    {
        /// <summary>
        /// Sets the default value for the install option.
        /// </summary>
        public bool? Install { get; set; }

        public List<PowershellIntegrationAlias> Aliases { get; } = new();

        /// <summary>
        /// for eg: "mycommand branch1 branch2 --lol 123" becomes "mbbl 123"
        /// </summary>
        /// <param name="alias">fore eg: "mbbl"</param>
        /// <param name="command">for eg: ["branch1", "branch2", "--lol"]</param>
        /// <returns></returns>
        public PowershellIntegrationOptions WithAlias(string alias, string[]? command = null)
        {
            Aliases.Add(new PowershellIntegrationAlias(alias, command));
            return this;
        }
    }
}