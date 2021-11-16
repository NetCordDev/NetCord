namespace NetCord
{
    public readonly struct Timestamp
    {
        public DateTimeOffset DateTime { get; }
        public TimestampStyle? Style { get; }

        public Timestamp()
        {
            DateTime = DateTimeOffset.UnixEpoch;
            Style = null;
        }

        public Timestamp(DateTimeOffset dateTime)
        {
            DateTime = dateTime;
            Style = null;
        }

        public Timestamp(DateTimeOffset dateTime, TimestampStyle style)
        {
            DateTime = dateTime;
            Style = style;
        }

        public static bool TryParse(ReadOnlySpan<char> value, out Timestamp timeStamp)
        {
            if (value.StartsWith("<t:"))
            {
                if (value.EndsWith(">"))
                {
                    if (value[^3] == ':')
                    {
                        if (long.TryParse(value[3..^3], out var result))
                        {
                            timeStamp = new(DateTimeOffset.FromUnixTimeSeconds(result), (TimestampStyle)value[^2]);
                            return true;
                        }
                    }
                    else
                    {
                        if (long.TryParse(value[3..^3], out var result))
                        {
                            timeStamp = new(DateTimeOffset.FromUnixTimeSeconds(result));
                            return true;
                        }
                    }
                }
            }
            timeStamp = default;
            return false;
        }

        public static Timestamp Parse(ReadOnlySpan<char> value)
        {
            if (TryParse(value, out var timestamp))
                return timestamp;
            else
                throw new FormatException($"{value} is not well formed {nameof(Timestamp)}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>discord formatted timestamap with a default style</returns>
        public override string ToString() => Style == null ? $"<t:{DateTime.ToUnixTimeSeconds()}>" : $"<t:{DateTime.ToUnixTimeSeconds()}:{(char)Style}>";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <returns>discord formatted timestamap with specified <paramref name="style"/></returns>
        public string ToString(TimestampStyle style) => $"<t:{DateTime.ToUnixTimeSeconds()}:{(char)style}>";
    }

    public enum TimestampStyle
    {
        ShortTime = 't',
        LongTime = 'T',
        ShortDate = 'd',
        LongDate = 'D',
        ShortDateTime = 'f',
        LongDateTime = 'F',
        RelativeTime = 'R'
    }
}
