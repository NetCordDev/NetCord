namespace NetCord.Rest.RateLimits;

public class RateLimitedException : Exception
{
    public bool Global { get; }
    public int Reset { get; }

    public RateLimitedException(int reset, bool global) : base("Rate limit triggered.")
    {
        Reset = reset;
        Global = global;
    }
}
