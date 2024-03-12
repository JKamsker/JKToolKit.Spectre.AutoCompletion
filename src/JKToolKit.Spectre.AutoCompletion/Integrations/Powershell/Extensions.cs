using Spectre.Console.Cli;

namespace JKToolKit.Spectre.AutoCompletion.Integrations;

public static class Extensions
{
    public static IConfigurator<CommandSettings> AddPowershell(this IConfigurator<CommandSettings> settings)
    {
        settings.AddDelegate<PowershellSettings>("powershell", (context, pwsh) =>
        {
            var settings = new PowershellIntegrationBuilderSettings
            {
                Install = pwsh.Install
            };

            var result = PowershellIntegrationBuilder.BuildIntegration(settings);
            System.Console.WriteLine(result);

            return 0;
        });

        return settings;
    }

    private class PowershellSettings : CommandSettings
    {
        public bool Install { get; set; }
    }
}