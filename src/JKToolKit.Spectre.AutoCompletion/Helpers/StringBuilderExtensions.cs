using System.Text;

namespace JKToolKit.Spectre.AutoCompletion.Helpers;

internal static class StringBuilderExtensions
{
    // indexof
    public static int IndexOf(this StringBuilder sb, char value, int startIndex)
    {
        if (sb == null)
        {
            throw new ArgumentNullException(nameof(sb));
        }

        if (startIndex < 0 || startIndex > sb.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }


        for (var i = startIndex; i < sb.Length; i++)
        {
            if (sb[i] == value)
            {
                return i;
            }
        }

        return -1;
    }
    
    // IsWhiteSpace (sb, int start, int end)
    public static bool IsWhiteSpace(this StringBuilder sb, int start, int end)
    {
        if (sb == null)
        {
            throw new ArgumentNullException(nameof(sb));
        }

        if (start < 0 || start > sb.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (end < 0 || end > sb.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(end));
        }

        for (var i = start; i < end; i++)
        {
            if (!char.IsWhiteSpace(sb[i]))
            {
                return false;
            }
        }

        return true;
    }
    
    internal static StringBuilder Cleanup(this StringBuilder sb)
    {
        if (sb == null)
        {
            throw new ArgumentNullException(nameof(sb));
        }

        int start = 0;
        int length = sb.Length;

        StringBuilder cleaned = new StringBuilder();

        while (start < length)
        {
            int lineEnd = sb.IndexOf('\n', start);
            if (lineEnd == -1) lineEnd = length; // End of the last line

            // Check if the line is empty or whitespace only
            if (sb.IsWhiteSpace(start, lineEnd))
            {
                start = lineEnd + 1;
                continue;
            }

            // Locate the comment start, if any, without extra allocations
            int commentIndex = sb.IndexOf('#', start);
            if (commentIndex != -1 && commentIndex < lineEnd)
            {
                lineEnd = commentIndex;
            }

            // Trim the line's leading and trailing spaces manually
            int trimStart = start;
            while (trimStart < lineEnd && char.IsWhiteSpace(sb[trimStart]))
            {
                trimStart++;
            }

            int trimEnd = lineEnd - 1;
            while (trimEnd >= trimStart && char.IsWhiteSpace(sb[trimEnd]))
            {
                trimEnd--;
            }

            if (trimStart <= trimEnd)
            {
                for (int i = trimStart; i <= trimEnd; i++)
                {
                    cleaned.Append(sb[i]);
                }
                cleaned.Append('\n');
            }

            start = lineEnd + 1; // Move to the next line
        }

        // Replace the original content of StringBuilder with the cleaned content
        // sb.Clear();
        // for (int i = 0; i < cleaned.Length; i++)
        // {
        //     sb.Append(cleaned[i]);
        // }

        return cleaned;
    }
}