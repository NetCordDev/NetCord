using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NetCord;

public class CodeBlock(string code, string? formatter = null) : ISpanFormattable, ISpanParsable<CodeBlock>
{
    public string Code { get; } = code;
    public string? Formatter { get; } = formatter;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // Inline so that 'strictMode' branches can be eliminated if it is a constant
    public static bool TryParse(ReadOnlySpan<char> s, bool strictMode, [MaybeNullWhen(false)] out CodeBlock result)
    {
        //                It needs to start and end with 3 backticks and have at least 1 character in between
        //                3 + 1 + 3 = 7
        var isCodeBlock = s.Length >= 7 && s.StartsWith("```") && s.EndsWith("```");

        if (!isCodeBlock)
        {
            result = null;
            goto Ret;
        }

        string? formatter = null;
        s = s[3..^3];
        var firstNewLine = s.IndexOf('\n');

        if (firstNewLine > 0)
        {
            var formatterSpan = s[..firstNewLine];

            foreach (var c in formatterSpan)
            {
                if (char.IsAsciiLetterOrDigit(c) || c is '+' or '-' or '#' or '_')
                    continue;

                goto Success;
            }

            s = s[(firstNewLine + 1)..];

            if (strictMode && s.IsWhiteSpace())
                s = formatterSpan;
            else
                formatter = formatterSpan.ToString();
        }

        Success:
        result = new(s.ToString(), formatter);

        Ret:
        return isCodeBlock;
    }

    public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s, true, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s, true, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s.AsSpan(), true, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // Inline so that 'strictMode' branches can be eliminated if it is a constant
    public static CodeBlock Parse(ReadOnlySpan<char> s, bool strictMode)
    {
        if (TryParse(s, strictMode, out var result))
            return result;

        throw new FormatException($"Cannot parse '{nameof(CodeBlock)}'.");
    }

    public static CodeBlock Parse(ReadOnlySpan<char> s) => Parse(s, true);

    public static CodeBlock Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, true);

    public static CodeBlock Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), true);
}
