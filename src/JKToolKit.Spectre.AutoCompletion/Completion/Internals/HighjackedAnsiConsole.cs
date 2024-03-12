using Spectre.Console;
using Spectre.Console.Rendering;

namespace JKToolKit.Spectre.AutoCompletion.Completion.Internals;

internal class HighjackedAnsiConsole : IAnsiConsole
{
    public IAnsiConsole OriginalConsole { get; }

    public HighjackedAnsiConsole(IAnsiConsole console)
    {
        OriginalConsole = console;
    }

    public Profile Profile => OriginalConsole.Profile;

    public IAnsiConsoleCursor Cursor => OriginalConsole.Cursor;
    public IAnsiConsoleInput Input => OriginalConsole.Input;
    public IExclusivityMode ExclusivityMode => OriginalConsole.ExclusivityMode;
    public RenderPipeline Pipeline => OriginalConsole.Pipeline;

    public void Clear(bool home)
    {
    }

    public void Write(IRenderable renderable)
    {
    }
}
