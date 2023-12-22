namespace NetCord.Rest.RateLimits;

public class RateLimitedException : Exception
{
    public long Reset { get; }
    public RateLimitScope Scope { get; }
    public long ResetAfter => Reset - Environment.TickCount64;

    public RateLimitedException(long reset, RateLimitScope scope) : base("Rate limit triggered.")
    {
        Reset = reset;
        Scope = scope;
    }
}
