using Spectre.Console.Cli;

namespace JKToolKit.Spectre.AutoCompletion.Completion;

public class AutoCompletionConfiguration
{
    public IConfigurator<CommandSettings> SpectreConfig { get; }

    internal AutoCompletionConfiguration(IConfigurator<CommandSettings> spectreConfig)
    {
        SpectreConfig = spectreConfig;
    }
}