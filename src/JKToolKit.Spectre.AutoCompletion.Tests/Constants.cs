namespace Spectre.Console.Tests;

public static class Constants
{
    public static string[] VersionCommand { get; } =
        new[]
        {
                CliConstants.Commands.Branch,
                CliConstants.Commands.Version,
        };

    public static string[] XmlDocCommand { get; } =
        new[]
        {
                CliConstants.Commands.Branch,
                CliConstants.Commands.XmlDoc,
        };

    public static string[] CompleteCommand { get; } =
        new[]
        {
                "completion",
                CliConstants.Commands.Complete,
        };
}

internal static class CliConstants
{
    public const string DefaultCommandName = "__default_command";
    public const string True = "true";
    public const string False = "false";

    public static string[] AcceptedBooleanValues { get; } = new string[]
    {
            True,
            False,
    };

    public static class Commands
    {
        public const string Branch = "cli";
        public const string Version = "version";
        public const string XmlDoc = "xmldoc";
        public const string Explain = "explain";
        public const string Complete = "complete";

        public const string CompletionBranch = "completion";
        public const string PowershellCompletion = "powershell";
    }
}