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

    public static IConfigurator AddAutoCompletion(this IConfigurator configurator, Action<IConfigurator<CommandSettings>>? action = null)
    {
        configurator.AddBranch("completion", complete =>
        {
            complete.AddCommand<CompleteCommand>("complete").IsHidden();

            if (action is not null)
            {
                action(complete);
            }
        });

        return configurator;
    }
}