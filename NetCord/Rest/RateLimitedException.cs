namespace NetCord.Rest.RateLimits;

public class RateLimitedException : Exception
{
    public bool Global { get; }
    public long Reset { get; }

    public RateLimitedException(long reset, bool global) : base("Rate limit triggered.")
    {
        Reset = reset;
        Global = global;
    }
}
