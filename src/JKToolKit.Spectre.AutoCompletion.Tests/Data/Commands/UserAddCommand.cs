namespace Spectre.Console.Cli.Tests.Data.Commands;

[Description("The user command.")]
internal class UserAddCommand : Command<UserAddSettings>
{
    protected override int Execute(CommandContext context, UserAddSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
