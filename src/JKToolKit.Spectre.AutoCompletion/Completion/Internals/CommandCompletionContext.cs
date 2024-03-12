using Spectre.Console.Cli;

using System.Reflection;
using System.Text.RegularExpressions;

namespace JKToolKit.Spectre.AutoCompletion.Completion.Internals;

internal class MappedCompletionContext : ICompletionContext
{
    private readonly CommandCompletionContext _context;
    private readonly Lazy<IReadOnlyCollection<IMappedCommandParameter>> _mappedParametersLazy;

    public string[] CommandElements => _context.CommandElements;
    public string PartialElement => _context.PartialElement;
    public ICommandContainer? Parent => _context.Parent;
    public IReadOnlyCollection<IMappedCommandParameter> MappedParameters => _mappedParametersLazy.Value;

    public IOriginalCommandInfo OriginalCommand => _context.OriginalCommand;

    public MappedCompletionContext(CommandCompletionContext context)
    {
        _context = context;
        _mappedParametersLazy = new Lazy<IReadOnlyCollection<IMappedCommandParameter>>(MapParams);
    }

    private IReadOnlyCollection<InternalMappedCommandParameter> MapParams()
    {
        return _context.MappedParameters.Select(x => new InternalMappedCommandParameter(x)).ToList().AsReadOnly();
    }
}

internal class InternalMappedCommandParameter : IMappedCommandParameter
{
    public ICommandParameterInfo Parameter { get; }
    public string? Value { get; }

    public InternalMappedCommandParameter(MappedCommandParameter parameter)
    {
        Parameter = parameter.Parameter;
        Value = parameter.Value;
    }

    public InternalMappedCommandParameter(ICommandParameterInfo parameter, string? value)
    {
        Parameter = parameter;
        Value = value;
    }
}

internal static class Extensions
{
    public static IMappedCommandParameter ToMappedCommandParameter(this MappedCommandParameter parameter)
    {
        return new InternalMappedCommandParameter(parameter);
    }
}

internal class OriginalCommandInfo : IOriginalCommandInfo
{
    public string OriginalCommand { get; set; }
    public int? Position { get; set; }

    public OriginalCommandInfo(string originalCommand, int? position)
    {
        OriginalCommand = originalCommand;
        Position = position;
    }
}

internal class CommandCompletionContext
{
    public OriginalCommandInfo OriginalCommand { get; set; }

    public bool ShouldSuggestMatchingInRoot { get; set; }
    public bool ShouldReturnEarly { get; set; }

    public string[] CommandElements { get; set; }
    public string PartialElement { get; set; }
    public CommandInfo? Parent { get; set; }
    public List<MappedCommandParameter> MappedParameters { get; set; }

    public CommandCompletionContext()
    {
        CommandElements = Array.Empty<string>();
        PartialElement = string.Empty;
        MappedParameters = new List<MappedCommandParameter>();
    }

    internal ICompletionContext ToMappedContext()
    {
        return new MappedCompletionContext(this);
    }
}

internal class CommandCompletionContextParser
{
    private readonly CommandModel _model;
    private readonly IConfiguration _configuration;

    public CommandCompletionContextParser(CommandModel model, IConfiguration configuration)
    {
        _model = model;
        _configuration = configuration;
    }

    public CommandCompletionContext? Parse(string? commandToComplete, int? position)
    {
        var originalCommand = new OriginalCommandInfo(commandToComplete, position);
        var normalizedCommand = NormalizeCommand(commandToComplete, position);
        var commandElements = SplitBySpace(normalizedCommand).Skip(1).ToArray();

        if (commandElements?.Length is 0 or null)
        {
            return new CommandCompletionContext
            {
                ShouldSuggestMatchingInRoot = true,
                OriginalCommand = originalCommand
            };
        }

        CommandTreeParser parser = GetParser();

        CommandTreeParserResult? parsedResult = null;
        var context = string.Empty;
        var partialElement = string.Empty;
        try
        {
            parsedResult = parser.Parse(commandElements);
            context = commandElements.LastOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(context))
            {
                context = commandElements[commandElements.Length - 2];
            }

            // Early return when "myapp feline"
            // but show completions for feline if "myapp feline "
            // spacing matters
            if (parsedResult.Tree?.Command.Name == context)
            {
                return new CommandCompletionContext
                {
                    CommandElements = commandElements,
                    PartialElement = partialElement,
                    ShouldSuggestMatchingInRoot = false,
                    ShouldReturnEarly = true,
                    OriginalCommand = originalCommand
                };
            }
        }
        catch (CommandParseException)
        {
            var strippedCommandElements = commandElements.Take(commandElements.Length - 1);
            if (strippedCommandElements.Any())
            {
                parsedResult = parser.Parse(strippedCommandElements);
                context = strippedCommandElements.Last();
            }

            partialElement = commandElements.LastOrDefault()?.ToLowerInvariant() ?? string.Empty;
        }

        CommandInfo parent;
        List<MappedCommandParameter> mappedParameters;
        if (parsedResult?.Tree == null)
        {
            // Should suggest anything matching in the root
            return new CommandCompletionContext
            {
                CommandElements = commandElements,
                PartialElement = partialElement,
                ShouldSuggestMatchingInRoot = true,
                OriginalCommand = originalCommand
            };
        }

        var lastContext = FindContextInTree(parsedResult);

        if (lastContext?.Command == null)
        {
            parent = parsedResult.Tree.Command;
            mappedParameters = parsedResult.Tree.Mapped;
        }
        else
        {
            parent = lastContext.Command;
            mappedParameters = lastContext.Mapped;
        }

        return new CommandCompletionContext
        {
            CommandElements = commandElements,
            PartialElement = partialElement,
            Parent = parent,
            MappedParameters = mappedParameters,
            OriginalCommand = originalCommand
        };
    }

    private CommandTreeParser GetParser()
    {
        // should also be exempt:
        // 0.46.1-preview.0.19
        // 0.46.1-preview.0.20

        const string SpecialVersion1 = "0.46.1-preview.0.20";
        const string SpecialVersion2 = "0.46.1-preview.0.19";

#if SPECTRE_47_OR_NEWER || SpectreConsoleVersion == SpecialVersion1 || SpectreConsoleVersion == SpecialVersion2
        return new CommandTreeParser
        (
            _model,
            _configuration.Settings.CaseSensitivity,
            _configuration.Settings.ParsingMode
        //,_configuration.Settings.ConvertFlagsToRemainingArguments

        );
#else
        return new CommandTreeParser(_model, _configuration.Settings);
#endif
    }

    private static CommandTree? FindContextInTree(CommandTreeParserResult? parsedResult)
    {
        var tree = parsedResult?.Tree;
        return FindContextInTree(tree);
    }

    private static CommandTree? FindContextInTree(CommandTree? tree)
    {
        if (tree is null)
        {
            return null;
        }

        var next = tree.Next;

        if (next is null)
        {
            return tree;
        }

        return FindContextInTree(next) ?? tree;
    }

    /// <summary>
    /// Trims the first character and the last character.
    /// </summary>
    private static string TrimOnce(string input, char character)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        if (input[0] == character && input[input.Length - 1] == character)
        {
            return input.Substring(1, input.Length - 2);
        }

        return input;
    }

    private static string[] SplitBySpace(string input)
    {
        // Regular expression pattern to match spaces except those within double quotes
        string pattern = @"\s+(?=(?:[^""]*""[^""]*"")*[^""]*$)";

        // Split the input string using the regular expression pattern
        string[] result = Regex.Split(input, pattern);

        // Remove any leading or trailing " characters on each element
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = TrimOnce(result[i], '"');
        }

        return result;
    }

    private static string NormalizeCommand(string? commandToComplete, int? position)
    {
        if (string.IsNullOrEmpty(commandToComplete))
        {
            return string.Empty;
        }

        var normalizedCommand = TrimOnce(commandToComplete!, '"');

        if (!string.IsNullOrEmpty(normalizedCommand) && position != null)
        {
            if (position > normalizedCommand.Length)
            {
                // extend the command to the position with whitespace
                var requiredWhitespace = position - normalizedCommand.Length ?? 1;
                normalizedCommand += new string(' ', requiredWhitespace);
            }
            else if (position < normalizedCommand.Length)
            {
                // trim the command to the position
                normalizedCommand = normalizedCommand.Substring(0, position.Value);
            }
        }

        return normalizedCommand;
    }
}