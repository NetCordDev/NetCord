namespace NetCord;

public enum TimestampStyle : byte
{
    ShortTime = (byte)'t',
    LongTime = (byte)'T',
    ShortDate = (byte)'d',
    LongDate = (byte)'D',
    ShortDateTime = (byte)'f',
    LongDateTime = (byte)'F',
    RelativeTime = (byte)'R',
}
