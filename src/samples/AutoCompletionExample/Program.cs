using JKToolKit.Spectre.AutoCompletion.Completion;

using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;
using JKToolKit.Spectre.AutoCompletion.Integrations;

namespace AutoCompletionExample;

// Adding autocomplete to powershell:
//  - .\AutoCompletionExample.exe completion powershell
//
// Adding autocomplete to powershell (permanent):
// - .\AutoCompletionExample.exe completion powershell --install
//
// Test completing:
// - .\AutoCompletionExample.exe cli complete "myapp Li"
internal static class Program
{
    private static void Main(string[] args)
    {
        // If we just want to test the completion with f5 in visual studio
        if (Debugger.IsAttached)
        {
            args = new[] { "completion", "complete", "\"myapp Li\"" };
        }

        var app = new CommandApp();
        app.Configure
        (
            config =>
            {
                config
                    .AddAutoCompletion(x => x.AddPowershell())
                    .AddCommand<LionCommand>("lion");
            }
        );

        app.Run(args);
    }
}

public class LionSettings : CommandSettings
{
    [CommandArgument(0, "<TEETH>")]
    [Description("The number of teeth the lion has.")]
    public int Teeth { get; set; }

    [CommandArgument(1, "[LEGS]")]
    [Description("The number of legs.")]
    public int Legs { get; set; }

    [CommandOption("-c <CHILDREN>")]
    [Description("The number of children the lion has.")]
    public int Children { get; set; }

    [CommandOption("-d <DAY>")]
    [Description("The days the lion goes hunting.")]
    [DefaultValue(new[] { DayOfWeek.Monday, DayOfWeek.Thursday })]
    public DayOfWeek[] HuntDays { get; set; }

    [CommandOption("-n|-p|--name|--pet-name <VALUE>")]
    public string Name { get; set; }

    [CommandOption("-a|--age <AGE>")]
    [CompletionSuggestions("10", "15", "20", "30")]
    public int Age { get; set; }
}

[Description("The lion command.")]
public class LionCommand : Command<LionSettings>, IAsyncCommandCompletable
{
    public override int Execute(CommandContext context, LionSettings settings)
    {
        return 0;
    }

    public async Task<CompletionResult> GetSuggestionsAsync(IMappedCommandParameter parameter, ICompletionContext context)
    {
        if (string.IsNullOrEmpty(parameter.Value))
        {
            return CompletionResult.None();
        }

        return await this.MatchAsync()
            .Add(x => x.Legs, (prefix) =>
            {
                if (prefix.Length != 0)
                {
                    return FindNextEvenNumber(prefix);
                }

                return "16";
            })
            .Add(x => x.Teeth, (prefix) =>
            {
                if (prefix.Length != 0)
                {
                    return FindNextEvenNumber(prefix);
                }

                return "32";
            })
            .Add(x => x.Name, prefix =>
            {
                var names = new List<string>
                {
                    "angel", "angelika", "robert",
                    "jennifer", "michael", "lucy",
                    "david", "sarah", "john", "katherine",
                    "mark"
                };

                var bestMatches = names
                    .Where(name => name.StartsWith(prefix))
                    .ToList();

                return new CompletionResult(bestMatches, bestMatches.Any());
            })
            .MatchAsync(parameter)
            .WithPreventDefault();
    }

    private static string FindNextEvenNumber(string input)
    {
        var number = int.Parse(input); // Parse the input string to an integer

        // Find the next even number greater than the input number
        var nextEvenNumber = number + (2 - (number % 2));

        // Convert the number to string to check the prefix
        var nextEvenNumberString = nextEvenNumber.ToString();

        // Check if the prefix of the even number matches the input string
        while (!nextEvenNumberString.StartsWith(input))
        {
            nextEvenNumber += 2; // Increment by 2 to find the next even number
            nextEvenNumberString = nextEvenNumber.ToString(); // Update the string representation
        }

        return nextEvenNumber.ToString();
    }
}

/*
 export SAMPLE_DLL=/mnt/c/Users/W31rd0/source/repos/JKamsker/JKToolKit/src/Samples/AutoCompletionExample/bin/Release/net7.0/publish/AutoCompletionExample.dll

# using SAMPLE_DLL: dotnet $SAMPLE_DLL
# completion: dotnet $SAMPLE_DLL cli complete "myapp li"

# bash parameter completion for the dotnet CLI

function _dotnet_bash_complete()
{
  local cur="${COMP_WORDS[COMP_CWORD]}" IFS=$'\n' # On Windows you may need to use use IFS=$'\r\n'
  local candidates

  read -d '' -ra candidates < <(dotnet $SAMPLE_DLL cli complete --position "${COMP_POINT}" "${COMP_LINE}" 2>/dev/null)

  read -d '' -ra COMPREPLY < <(compgen -W "${candidates[*]:-}" -- "$cur")
}

complete -f -F _dotnet_bash_complete AutoCompletionExample
 */