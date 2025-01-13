namespace JKToolKit.Spectre.AutoCompletion.Integrations.Powershell;

public class StartArgs
{
    public string Runtime { get; }
    public string Command { get; }

    public string CommandName => Path.GetFileNameWithoutExtension(Command);

    public string CommandExtension => Path.GetExtension(Command);

    public StartArgs(string runtime, string command)
    {
        Runtime = runtime;
        Command = command;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Runtime))
        {
            return Command;
        }

        return string.Join(" ", Runtime, Command);
    }

    public static StartArgs ParseStartArgs(params string[] args)
    {
        var command = args[0];
        if (command.EndsWith(".dll"))
        {
            return new StartArgs("dotnet", command);
        }

        var commandIsDotnet = Path.GetFileNameWithoutExtension(command)
            .Equals("dotnet", StringComparison.OrdinalIgnoreCase);

        if (commandIsDotnet)
        {
            return new StartArgs(command, args[1]);
        }

        return new StartArgs(string.Empty, args[0]);
    }

    public static StartArgs GetSelfStartCommandFromCommandLineArgs()
    {
        var args = Environment.GetCommandLineArgs();
        return ParseStartArgs(args);
    }
}