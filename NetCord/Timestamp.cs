using System.Globalization;

namespace NetCord;

public readonly struct Timestamp
{
    public DateTimeOffset DateTime { get; }
    public TimestampStyle? Style { get; }

    public Timestamp(DateTimeOffset dateTime)
    {
        DateTime = dateTime;
    }

    public Timestamp(DateTimeOffset dateTime, TimestampStyle? style)
    {
        DateTime = dateTime;
        Style = style;
    }

    public static bool TryParse(ReadOnlySpan<char> value, out Timestamp timestamp)
    {
        if (value.StartsWith("<t:") && value.EndsWith(">"))
        {
            if (value.Length > 5 && value[^3] == ':')
            {
                if (long.TryParse(value[3..^3], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result))
                {
                    timestamp = new(DateTimeOffset.FromUnixTimeSeconds(result), (TimestampStyle)value[^2]);
                    return true;
                }
            }
            else
            {
                if (long.TryParse(value[3..^1], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result))
                {
                    timestamp = new(DateTimeOffset.FromUnixTimeSeconds(result));
                    return true;
                }
            }
        }
        timestamp = default;
        return false;
    }

    public static Timestamp Parse(ReadOnlySpan<char> value)
    {
        if (TryParse(value, out var timestamp))
            return timestamp;
        else
            throw new FormatException($"Cannot parse '{nameof(Timestamp)}'.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>discord formatted timestamap with a default style</returns>
    public override string ToString() => Style.HasValue ? $"<t:{DateTime.ToUnixTimeSeconds()}:{(char)Style.GetValueOrDefault()}>" : $"<t:{DateTime.ToUnixTimeSeconds()}>";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    /// <returns>discord formatted timestamap with specified <paramref name="style"/></returns>
    public string ToString(TimestampStyle style) => $"<t:{DateTime.ToUnixTimeSeconds()}:{(char)style}>";
}
