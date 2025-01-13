namespace JKToolKit.Spectre.AutoCompletion.Integrations.Powershell;

public class PowershellIntegrationBuilderSettings
{
    public bool Diagnostic { get; set; }

    public bool Install { get; set; }
    public StartArgs? StartArgs { get; set; }

    public List<PowershellIntegrationAlias> Aliases { get; set; } = new();
}

public record PowershellIntegrationAlias(string Alias, string[]? Command)
{
    private static readonly string EmptyQuotes = "\"\"";

    private static string QuoteString(string value)
    {
        return $"\"{value}\"";
    }

    public string MakeGlobalAliasValue()
    {
        if (Command == null || Command.Length == 0)
        {
            return EmptyQuotes;
        }

        if (Command.Length == 1)
        {
            return QuoteString(Command[0]);
        }

        var quotedCommand = string.Join(", ", Command.Select(QuoteString));
        return $"@({quotedCommand})";
    }
}