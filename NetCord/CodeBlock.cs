using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public class CodeBlock : ISpanFormattable, ISpanParsable<CodeBlock>
{
    public string Code { get; }
    public string? Formatter { get; }

    public CodeBlock(string code, string? formatter = null)
    {
        Code = code;
        Formatter = formatter;
    }

    public override string ToString() => $"```{Formatter}\n{Code}```";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var code = Code;
        var formatter = Formatter;

        int requiredLength = formatter is null ? (code.Length + 7) : (code.Length + formatter.Length + 7);
        if (destination.Length < requiredLength)
        {
            charsWritten = 0;
            return false;
        }

        int written = 0;

        "```".CopyTo(destination);
        written += 3;

        if (formatter is not null)
        {
            formatter.CopyTo(destination[written..]);
            written += formatter.Length;
        }

        destination[written++] = '\n';

        code.CopyTo(destination[written..]);
        written += code.Length;

        "```".CopyTo(destination[written..]);

        charsWritten = requiredLength;
        return true;
    }

    public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out CodeBlock result)
    {
        if (s.StartsWith("```") && s.EndsWith("```"))
        {
            s = s[3..^3];
            int i = s.IndexOf('\n');
            if (i != -1)
            {
                var formatter = s[..i];
                foreach (var c in formatter)
                {
                    if (c is not ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '+' or '-'))
                    {
                        result = new(s.ToString());
                        return true;
                    }
                }

                result = new(s[(formatter.Length + 1)..].ToString(), formatter.ToString());
                return true;
            }
            else
            {
                result = new(s.ToString());
                return true;
            }
        }

        result = null;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s.AsSpan(), out result);

    public static CodeBlock Parse(ReadOnlySpan<char> s)
    {
        if (TryParse(s, out var result))
            return result;

        throw new FormatException($"Cannot parse '{nameof(CodeBlock)}'.");
    }

    public static CodeBlock Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s);

    public static CodeBlock Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);
}
