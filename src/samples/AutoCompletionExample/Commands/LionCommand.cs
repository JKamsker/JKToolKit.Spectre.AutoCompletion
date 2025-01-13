using System.ComponentModel;
using JKToolKit.Spectre.AutoCompletion.Attributes;
using JKToolKit.Spectre.AutoCompletion.Completion;
using Spectre.Console.Cli;

namespace AutoCompletionExample;

[Description("The lion command.")]
public class LionCommand : Command<LionCommand.LionSettings>, IAsyncCommandCompletable
{
    public class LionSettings : CommandSettings
    {
        [CommandArgument(0, "[LEGS]")]
        // [CommandOption("-l|--legs <LEGS>")]
        [Description("The number of legs.")]
        public int Legs { get; set; }

        //[CommandArgument(0, "<TEETH>")]
        [CommandOption("-t|--teeth <TEETH>")]
        [Description("The number of teeth the lion has.")]
        public int Teeth { get; set; }


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

    public override int Execute(CommandContext context, LionSettings settings)
    {
        Console.WriteLine("Executing LionCommand with the following settings:");
        Console.WriteLine($"Teeth: {settings.Teeth}");
        Console.WriteLine($"Legs: {settings.Legs}");
        Console.WriteLine($"Children: {settings.Children}");
        Console.WriteLine($"HuntDays: {string.Join(", ", settings.HuntDays)}");
        Console.WriteLine($"Name: {settings.Name}");
        Console.WriteLine($"Age: {settings.Age}");

        return 0;
    }

    public async Task<CompletionResult> GetSuggestionsAsync(IMappedCommandParameter parameter, ICompletionContext context)
    {
        // if (string.IsNullOrEmpty(parameter.Value))
        // {
        //     return CompletionResult.None();
        // }

        return await this.MatchAsync()
            .Add(x => x.Legs, (prefix) =>
            {
                if (prefix.Length != 0)
                {
                    return FindNextEvenNumber(prefix);
                }

                // return "16";
                return new CompletionResult(EvenNumbers().Take(20).Select(x => x.ToString()).ToList(), true);
            })
            .Add(x => x.Teeth, (prefix) =>
            {
                if (prefix.Length != 0)
                {
                    return FindNextEvenNumber(prefix);
                }

                return new CompletionResult(EvenNumbers().Take(20).Select(x => x.ToString()).ToList(), true);
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

    private IEnumerable<int> EvenNumbers()
    {
        for (var i = 0; i < int.MaxValue; i += 2)
        {
            yield return i;
        }
    }

    private static string FindNextEvenNumber(string input)
    {
        var number = int.Parse(input); // Parse the input string to an integer
        var isEven = number % 2 == 0; // Check if the number is even
        if (isEven)
        {
            return input;
        }


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