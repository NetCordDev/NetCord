namespace NetCord.Rest.RateLimits;

public class RateLimitedException(long reset, RateLimitScope scope) : Exception("Rate limit triggered.")
{
    public long Reset { get; } = reset;
    public RateLimitScope Scope { get; } = scope;
    public long ResetAfter => Reset - Environment.TickCount64;
}
