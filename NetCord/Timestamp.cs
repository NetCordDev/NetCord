using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NetCord;

public readonly struct Timestamp : IEquatable<Timestamp>, ISpanFormattable, ISpanParsable<Timestamp>
{
    public Timestamp(DateTimeOffset dateTime)
    {
        DateTime = dateTime;
    }

    public Timestamp(DateTimeOffset dateTime, TimestampStyle? style)
    {
        DateTime = dateTime;
        Style = style;
    }

    public DateTimeOffset DateTime { get; }

    public TimestampStyle? Style { get; }

    public static bool operator ==(Timestamp left, Timestamp right) => left.Equals(right);

    public static bool operator !=(Timestamp left, Timestamp right) => !(left == right);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Timestamp timestamp && Equals(timestamp);

    public bool Equals(Timestamp other) => DateTime == other.DateTime && Style == other.Style;

    public override int GetHashCode() => HashCode.Combine(DateTime, Style);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        int written;
        int length;
        char style;
        switch (format.Length)
        {
            case 0:
                if (Style.HasValue)
                {
                    style = (char)Style.GetValueOrDefault();
                    goto WriteStyled;
                }

                // 3 + 1 + 1 = 5
                if (destination.Length < 5 || !DateTime.ToUnixTimeSeconds().TryFormat(destination[3..^1], out length))
                {
                    charsWritten = 0;
                    return false;
                }

                written = 3 + length;
                break;
            case 1:
                style = format[0];
                goto WriteStyled;
            default:
                throw new FormatException("Format specifier was invalid.");
        }

        Finish:
        "<t:".CopyTo(destination);
        destination[written++] = '>';

        charsWritten = written;
        return true;

        WriteStyled:

        // 3 + 1 + 3 = 7
        if (destination.Length < 7 || !DateTime.ToUnixTimeSeconds().TryFormat(destination[3..^3], out length))
        {
            charsWritten = 0;
            return false;
        }

        written = 3 + length;

        destination[written++] = ':';
        destination[written++] = style;

        goto Finish;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="formatProvider"></param>
    /// <returns>Discord formatted timestamp.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return format switch
        {
            null => ToString(),
            { Length: 1 } => ToString(format[0]),
            _ => throw new FormatException($"The '{format}' format string is not supported."),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Discord formatted timestamp with a default style.</returns>
    public override string ToString() => Style.HasValue ? $"<t:{DateTime.ToUnixTimeSeconds()}:{(char)Style.GetValueOrDefault()}>" : $"<t:{DateTime.ToUnixTimeSeconds()}>";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    /// <returns>Discord formatted timestamp with specified <paramref name="style"/>.</returns>
    public string ToString(TimestampStyle style) => ToString((char)style);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    /// <returns>Discord formatted timestamp with specified <paramref name="style"/>.</returns>
    public string ToString(char style) => $"<t:{DateTime.ToUnixTimeSeconds()}:{style}>";

    public static bool TryParse(ReadOnlySpan<char> value, [MaybeNullWhen(false)] out Timestamp timestamp)
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

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Timestamp result) => TryParse(s, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Timestamp result) => TryParse(s.AsSpan(), out result);

    public static Timestamp Parse(ReadOnlySpan<char> value)
    {
        if (TryParse(value, out var timestamp))
            return timestamp;
        else
            throw new FormatException($"Cannot parse '{nameof(Timestamp)}'.");
    }

    public static Timestamp Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s);

    public static Timestamp Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan());
}
