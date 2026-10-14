using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NetCord;

/// <summary>
/// Represents a Discord Markdown fenced code block.
/// </summary>
/// <param name="code">The content of the code block.</param>
/// <param name="formatter">The optional formatter or language identifier following the opening backticks.</param>
public class CodeBlock(string code, string? formatter = null) : ISpanFormattable, ISpanParsable<CodeBlock>
{
    /// <summary>
    /// Gets the content of the code block.
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// Gets the formatter or language identifier of the code block, if present.
    /// </summary>
    public string? Formatter { get; } = formatter;

    /// <inheritdoc/>
    public override string ToString() => $"```{Formatter}\n{Code}```";

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    /// <inheritdoc/>
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

    /// <summary>
    /// Attempts to parse a Discord Markdown fenced code block.
    /// </summary>
    /// <param name="s">The characters to parse.</param>
    /// <param name="strictMode">
    /// Whether an apparent formatter followed only by whitespace should instead be treated as code.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the parsed code block if parsing succeeded; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="s"/> starts and ends with triple backticks and contains at least
    /// one character between them; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // Inline so that 'strictMode' branches can be eliminated if it is a constant
    public static bool TryParse(ReadOnlySpan<char> s, bool strictMode, [MaybeNullWhen(false)] out CodeBlock result)
    {
        // It needs to start and end with 3 backticks and have at least 1 character in between
        var isCodeBlock = s is ['`', '`', '`', _, .., '`', '`', '`'];

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

    /// <summary>
    /// Attempts to parse a Discord Markdown fenced code block in strict mode.
    /// </summary>
    /// <param name="s">The characters to parse.</param>
    /// <param name="result">
    /// When this method returns, contains the parsed code block if parsing succeeded; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s, true, out result);

    /// <inheritdoc/>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s, true, out result);

    /// <inheritdoc/>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CodeBlock result) => TryParse(s.AsSpan(), true, out result);

    /// <summary>
    /// Parses a Discord Markdown fenced code block.
    /// </summary>
    /// <param name="s">The characters to parse.</param>
    /// <param name="strictMode">
    /// Whether an apparent formatter followed only by whitespace should instead be treated as code.
    /// </param>
    /// <returns>The parsed code block.</returns>
    /// <exception cref="FormatException"><paramref name="s"/> is not a valid fenced code block.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // Inline so that 'strictMode' branches can be eliminated if it is a constant
    public static CodeBlock Parse(ReadOnlySpan<char> s, bool strictMode)
    {
        if (TryParse(s, strictMode, out var result))
            return result;

        throw new FormatException($"Cannot parse '{nameof(CodeBlock)}'.");
    }

    /// <summary>
    /// Parses a Discord Markdown fenced code block in strict mode.
    /// </summary>
    /// <param name="s">The characters to parse.</param>
    /// <returns>The parsed code block.</returns>
    /// <exception cref="FormatException"><paramref name="s"/> is not a valid fenced code block.</exception>
    public static CodeBlock Parse(ReadOnlySpan<char> s) => Parse(s, true);

    /// <inheritdoc/>
    public static CodeBlock Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, true);

    /// <inheritdoc/>
    public static CodeBlock Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), true);
}
