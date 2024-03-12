namespace JKToolKit.Spectre.AutoCompletion.Completion;

/// <summary>
/// Represents a mapped command parameter.
/// </summary>
public interface IOriginalCommandInfo
{
    /// <summary>
    /// Gets the original command.
    /// </summary>
    string OriginalCommand { get; }

    /// <summary>
    /// Gets the original command.
    /// </summary>
    int? Position { get; }
}