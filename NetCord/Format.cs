namespace NetCord;

public static class Format
{
    public static string Bold(ReadOnlySpan<char> text) => $"**{text}**";
    public static string Italic(ReadOnlySpan<char> text) => $"*{text}*";
    public static string StrikeOut(ReadOnlySpan<char> text) => $"~~{text}~~";
    public static string Underline(ReadOnlySpan<char> text) => $"__{text}__";
    public static string Spoiler(ReadOnlySpan<char> text) => $"||{text}||";
    public static string EscapeUrl(ReadOnlySpan<char> url) => $"<{url}>";
    public static string Link(ReadOnlySpan<char> text, ReadOnlySpan<char> url) => $"[{text}]({url})";
    public static string CodeBlock(ReadOnlySpan<char> code, ReadOnlySpan<char> formatter = default) => new CodeBlock(code, formatter).ToString();
    public static string Timestamp(DateTimeOffset dateTime, TimestampStyle? style) => new Timestamp(dateTime, style).ToString();
    public static string Quote(ReadOnlySpan<char> text) => $">>> {text}";
    public static string Escape(ReadOnlySpan<char> text)
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