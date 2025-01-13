using JKToolKit.Spectre.AutoCompletion.Completion.Internals;

using Spectre.Console.Cli;

namespace JKToolKit.Spectre.AutoCompletion.Completion;

public static class Extensions
{
    public static AsyncCommandParameterMatcher<TSettings> MatchAsync<TSettings>(this Command<TSettings> command)
        where TSettings : CommandSettings
    {
        return new AsyncCommandParameterMatcher<TSettings>();
    }

    public static CommandParameterMatcher<TSettings> Match<TSettings>(this Command<TSettings> command)
        where TSettings : CommandSettings
    {
        return new CommandParameterMatcher<TSettings>();
    }

    public static AsyncCommandParameterMatcher<TSettings> MatchAsync<TSettings>(this AsyncCommand<TSettings> command)
    where TSettings : CommandSettings
    {
        return new AsyncCommandParameterMatcher<TSettings>();
    }

    public static CommandParameterMatcher<TSettings> Match<TSettings>(this AsyncCommand<TSettings> command)
        where TSettings : CommandSettings
    {
        return new CommandParameterMatcher<TSettings>();
    }

    public static IConfigurator AddAutoCompletion(this IConfigurator configurator, Action<AutoCompletionConfiguration>? action = null)
    {
        configurator.AddBranch("completion", cfg =>
        {
            cfg.AddCommand<CompleteCommand>("complete").IsHidden();
            
            // cfg.AddDelegate<CompleteCommandSettings>("complete", (context, complete) =>
            // {
            //     var command = new CompleteCommand();
            //
            //     return 0;
            // });

            if (action is not null)
            {
                var configuration = new AutoCompletionConfiguration(cfg);
                action(configuration);
            }
        });

        return configurator;
    }
}