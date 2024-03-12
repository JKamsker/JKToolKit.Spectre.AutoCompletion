namespace JKToolKit.Spectre.AutoCompletion.Completion.Internals;

internal class TabCompletionArgs
{
    public string Command { get; set; }
    public int? CursorPosition { get; set; }

    public TabCompletionArgs(string command, int? cursorPosition = null)
    {
        Command = command;
        CursorPosition = cursorPosition;
    }
}