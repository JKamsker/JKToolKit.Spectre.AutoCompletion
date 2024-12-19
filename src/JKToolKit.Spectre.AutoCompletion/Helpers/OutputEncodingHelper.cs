using System.Runtime.InteropServices;
using System.Text;

namespace JKToolKit.Spectre.AutoCompletion.Helpers;

internal class OutputEncodingHelper
{
    public static IDisposable SetOutputEncodingIfNeccessary()
    {
        var originalEncoding = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && System.Console.OutputEncoding.CodePage == Encoding.Unicode.CodePage
            ? System.Console.OutputEncoding
            : null;

        if (originalEncoding is not null)
        {
            System.Console.OutputEncoding = Encoding.UTF8;
        }

        return new ActionDisposable(() =>
        {
            if (originalEncoding is not null)
            {
                System.Console.OutputEncoding = originalEncoding;
            }
        });
    }
}