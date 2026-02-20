using Spectre.Console.Cli;
using Spectre.Console.Cli.Completion;
using Spectre.Console.Cli.Help;

using System.Text.RegularExpressions;

namespace JKToolKit.Spectre.AutoCompletion.Completion.Internals;

internal sealed class MappedCompletionContext : ICompletionContext
{
    private readonly CommandCompletionContext _context;
    private readonly Lazy<IReadOnlyCollection<IMappedCommandParameter>> _mappedParametersLazy;

    public string[] CommandElements => _context.CommandElements;
    public string PartialElement => _context.PartialElement;
    public ICommandInfo? Parent => _context.Parent;
    public IReadOnlyCollection<IMappedCommandParameter> MappedParameters => _mappedParametersLazy.Value;
    public IOriginalCommandInfo OriginalCommand => _context.OriginalCommand;

    public MappedCompletionContext(CommandCompletionContext context)
    {
        _context = context;
        _mappedParametersLazy = new Lazy<IReadOnlyCollection<IMappedCommandParameter>>(MapParams);
    }

    private IReadOnlyCollection<IMappedCommandParameter> MapParams()
    {
        return _context.MappedParameters
            .Select(x => (IMappedCommandParameter)new InternalMappedCommandParameter(x))
            .ToList()
            .AsReadOnly();
    }
}

internal sealed class InternalMappedCommandParameter : IMappedCommandParameter
{
    public ICommandParameterInfo Parameter { get; }
    public string? Value { get; }

    public InternalMappedCommandParameter(CommandLineMappedParameter parameter)
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

internal sealed class OriginalCommandInfo : IOriginalCommandInfo
{
    public string OriginalCommand { get; set; }
    public int? Position { get; set; }

    public OriginalCommandInfo(string? originalCommand, int? position)
    {
        OriginalCommand = originalCommand ?? string.Empty;
        Position = position;
    }
}

internal sealed class CommandCompletionContext
{
    public OriginalCommandInfo OriginalCommand { get; set; } = new(null, null);

    public bool ShouldSuggestMatchingInRoot { get; set; }
    public bool ShouldReturnEarly { get; set; }

    public string[] CommandElements { get; set; } = Array.Empty<string>();
    public string PartialElement { get; set; } = string.Empty;
    public ICommandInfo? Parent { get; set; }

    public Type? CommandType { get; set; }
    public Type? SettingsType { get; set; }

    public IReadOnlyList<CommandLineMappedParameter> MappedParameters { get; set; } = Array.Empty<CommandLineMappedParameter>();

    internal ICompletionContext ToMappedContext()
    {
        return new MappedCompletionContext(this);
    }
}

internal sealed class CommandCompletionContextParser
{
    private readonly ICommandModel _model;
    private readonly ICommandAppSettings _settings;

    public CommandCompletionContextParser(ICommandModel model, ICommandAppSettings settings)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public CommandCompletionContext Parse(string? commandToComplete, int? position)
    {
        var originalCommand = new OriginalCommandInfo(commandToComplete, position);
        var normalizedCommand = NormalizeCommand(commandToComplete, position);
        var commandElements = SplitBySpace(normalizedCommand).Skip(1).ToArray();

        if (commandElements.Length == 0)
        {
            return new CommandCompletionContext
            {
                ShouldSuggestMatchingInRoot = true,
                OriginalCommand = originalCommand,
            };
        }

        CommandLineParseResult? parsedResult = null;
        var context = string.Empty;
        var partialElement = string.Empty;

        try
        {
            parsedResult = CommandLineParser.Parse(_model, _settings, commandElements);
            context = commandElements.LastOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(context) && commandElements.Length >= 2)
            {
                context = commandElements[commandElements.Length - 2];
            }

            // Early return when "myapp feline"
            // but show completions for feline if "myapp feline "
            // spacing matters
            if (parsedResult.Command?.Name == context && commandElements.Length == 1)
            {
                return new CommandCompletionContext
                {
                    CommandElements = commandElements,
                    PartialElement = partialElement,
                    ShouldSuggestMatchingInRoot = false,
                    ShouldReturnEarly = true,
                    OriginalCommand = originalCommand,
                };
            }
        }
        catch (CommandParseException)
        {
            var strippedCommandElements = commandElements.Take(commandElements.Length - 1).ToArray();
            if (strippedCommandElements.Length > 0)
            {
                parsedResult = CommandLineParser.Parse(_model, _settings, strippedCommandElements);
                context = strippedCommandElements.Last();
            }

            partialElement = commandElements.LastOrDefault()?.ToLowerInvariant() ?? string.Empty;
        }

        if (parsedResult?.Command == null)
        {
            return new CommandCompletionContext
            {
                CommandElements = commandElements,
                PartialElement = partialElement,
                ShouldSuggestMatchingInRoot = true,
                OriginalCommand = originalCommand,
            };
        }

        return new CommandCompletionContext
        {
            CommandElements = commandElements,
            PartialElement = partialElement,
            Parent = parsedResult.Command,
            CommandType = parsedResult.CommandType,
            SettingsType = parsedResult.SettingsType,
            MappedParameters = parsedResult.MappedParameters,
            OriginalCommand = originalCommand,
        };
    }

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
        const string Pattern = @"\s+(?=(?:[^""]*""[^""]*"")*[^""]*$)";

        // Split the input string using the regular expression pattern
        var result = Regex.Split(input, Pattern);

        // Remove any leading or trailing " characters on each element
        for (var i = 0; i < result.Length; i++)
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
                // Extend the command to the position with whitespace.
                var requiredWhitespace = position.Value - normalizedCommand.Length;
                normalizedCommand += new string(' ', requiredWhitespace);
            }
            else if (position < normalizedCommand.Length)
            {
                // Trim the command to the position.
                normalizedCommand = normalizedCommand.Substring(0, position.Value);
            }
        }

        return normalizedCommand;
    }
}
