using Spectre.Console;
using Spectre.Console.Tests.Data;

public class GreeterCommand : Command<OptionalArgumentWithDefaultValueSettings>
{
    private readonly IAnsiConsole _console;

    public GreeterCommand(IAnsiConsole console)
    {
        _console = console;
    }

    public override int Execute(CommandContext context, OptionalArgumentWithDefaultValueSettings settings)
    {
        _console.WriteLine(settings.Greeting);
        return 0;
    }
}