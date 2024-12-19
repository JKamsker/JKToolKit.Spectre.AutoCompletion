using System.ComponentModel;
using JKToolKit.Spectre.AutoCompletion.Completion;
using Spectre.Console;
using Spectre.Console.Cli;


namespace AutoCompletionExample;

public class LsCommand : Command<LsCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        public string Path { get; set; } = ".";
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var directory = new DirectoryInfo(settings.Path);

            if (!directory.Exists)
            {
                AnsiConsole.MarkupLine("[red]Error:[/] Directory does not exist.");
                return 1;
            }

            // Display directories
            AnsiConsole.MarkupLine("[bold blue]Directories:[/]");
            foreach (var dir in directory.GetDirectories())
            {
                AnsiConsole.MarkupLine($"[green]{dir.Name}[/]");
            }

            // Display files
            AnsiConsole.MarkupLine("\n[bold blue]Files:[/]");
            foreach (var file in directory.GetFiles())
            {
                AnsiConsole.MarkupLine($"[yellow]{file.Name}[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
            return 1;
        }

        return 0;
    }
}