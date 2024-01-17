namespace NetCord;

public enum TimestampStyle : short
{
    ShortTime = (short)'t',
    LongTime = (short)'T',
    ShortDate = (short)'d',
    LongDate = (short)'D',
    ShortDateTime = (short)'f',
    LongDateTime = (short)'F',
    RelativeTime = (short)'R',
}
