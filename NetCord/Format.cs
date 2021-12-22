namespace NetCord;

public static class Format
{
    public static ReadOnlySpan<char> Bold(ReadOnlySpan<char> text) => $"**{text}**";
    public static ReadOnlySpan<char> Italic(ReadOnlySpan<char> text) => $"*{text}*";
    public static ReadOnlySpan<char> StrikeOut(ReadOnlySpan<char> text) => $"~~{text}~~";
    public static ReadOnlySpan<char> Underline(ReadOnlySpan<char> text) => $"__{text}__";
    public static ReadOnlySpan<char> Spoiler(ReadOnlySpan<char> text) => $"||{text}||";
    public static ReadOnlySpan<char> EscapeUrl(ReadOnlySpan<char> url) => $"<{url}>";
    public static ReadOnlySpan<char> Link(ReadOnlySpan<char> text, ReadOnlySpan<char> url) => $"[{text}]({url})";
    public static ReadOnlySpan<char> CodeBlock(ReadOnlySpan<char> code, ReadOnlySpan<char> formatter = default) => new CodeBlock(code, formatter).ToString();
    public static ReadOnlySpan<char> Timestamp(DateTimeOffset dateTime, TimestampStyle? style) => new Timestamp(dateTime, style).ToString();
    public static ReadOnlySpan<char> Quote(ReadOnlySpan<char> text) => $">>> {text}";
    public static ReadOnlySpan<char> Escape(ReadOnlySpan<char> text)
    {
        return text.ToString()
            .Replace("\\", "\\\\")
            .Replace("*", "\\*")
            .Replace("_", "\\_")
            .Replace("~", "\\~")
            .Replace("`", "\\`")
            .Replace(".", "\\.")
            .Replace(":", "\\:")
            .Replace("/", "\\/")
            .Replace(">", "\\>")
            .Replace("|", "\\|");
    }
}