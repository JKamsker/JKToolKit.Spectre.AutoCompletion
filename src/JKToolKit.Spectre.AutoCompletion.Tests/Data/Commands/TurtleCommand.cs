namespace Spectre.Console.Tests.Data;

[Description("The turtle command.")]
public class TurtleCommand : AnimalCommand<TurtleSettings>
{
    protected override int Execute(CommandContext context, TurtleSettings settings, CancellationToken cancellationToken)
    {
        DumpSettings(context, settings);
        return 0;
    }
}
