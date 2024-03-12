using System.Text;

namespace JKToolKit.Spectre.AutoCompletion.Completion.Internals;

internal class HighjackedTextWriter : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    public TextWriter OriginalWriter { get; }

    public HighjackedTextWriter(TextWriter originalWriter)
    {
        OriginalWriter = originalWriter;
    }

    public override void Write(char value)
    {
    }

    public override void Write(string? value)
    {
    }

    public override void WriteLine(string? value)
    {
    }
}