using System.Text.RegularExpressions;

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
    public static string SmallCodeBlock(ReadOnlySpan<char> code) => $"`{code}`";
    public static string CodeBlock(ReadOnlySpan<char> code, ReadOnlySpan<char> formatter = default) => $"```{formatter}\n{code}```";
    public static string Timestamp(DateTimeOffset dateTime, TimestampStyle? style) => new Timestamp(dateTime, style).ToString();
    public static string Quote(ReadOnlySpan<char> text) => $">>> {text}";
    public static string Escape(string text) => Regex.Replace(text, @"[\*_~`.:/>|]", @"\$0", RegexOptions.Compiled);
}
