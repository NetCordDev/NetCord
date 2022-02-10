namespace NetCord.Services.Commands;

internal static class StringExtensions
{
    public static bool ContainsAny(this string s, char[] chars) => s.IndexOfAny(chars) != -1;
}