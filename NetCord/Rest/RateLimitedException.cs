using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public class RateLimitedException(long reset, Route route, RateLimitScope scope) : Exception($"{RateLimitScopeHelpers.GetString(scope)} rate limit exceeded for route '{route}'.")
{
    public long Reset => reset;
    public Route Route => route;
    public RateLimitScope Scope => scope;
    public long ResetAfter => reset - Environment.TickCount64;
}
