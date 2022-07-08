namespace NetCord.Rest.RateLimits;

public class RateLimitedException : Exception
{
    public bool Global { get; }
    public DateTimeOffset Reset => DateTimeOffset.FromUnixTimeMilliseconds(_reset);

    private readonly long _reset;

    public RateLimitedException(long reset, bool global) : base("Rate limit triggered.")
    {
        _reset = reset;
        Global = global;
    }
}