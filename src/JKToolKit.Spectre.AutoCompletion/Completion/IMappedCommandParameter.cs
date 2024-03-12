using Spectre.Console.Cli;

namespace JKToolKit.Spectre.AutoCompletion.Completion;

/// <summary>
/// Defines a mapped command parameter.
/// </summary>
public interface IMappedCommandParameter
{
    /// <summary>
    /// Gets the parameter information for the command.
    /// </summary>
    ICommandParameterInfo Parameter { get; }

    /// <summary>
    /// Gets the value of the command parameter.
    /// </summary>
    string? Value { get; }
}