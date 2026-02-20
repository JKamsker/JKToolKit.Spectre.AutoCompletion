namespace Spectre.Console.Cli.Tests.Data.Commands;

[Description("The user command.")]
internal class UserSuperAddCommand : Command<UserSuperAddSettings>
{
    protected override int Execute(CommandContext context, UserSuperAddSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
