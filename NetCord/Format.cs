namespace NetCord;

public static class Format
{
    public static ReadOnlySpan<char> Bold(ReadOnlySpan<char> text) => $"**{text}**";
    public static ReadOnlySpan<char> Italic(ReadOnlySpan<char> text) => $"*{text}*";
    public static ReadOnlySpan<char> StrikeOut(ReadOnlySpan<char> text) => $"~~{text}~~";
    public static ReadOnlySpan<char> Escape(ReadOnlySpan<char> text)
    {
        var str = text.ToString();
        str = str.Replace("\\", "\\\\");
        str = str.Replace("`", "\\`");
        return str.Replace("<", "\\<");
    }
}