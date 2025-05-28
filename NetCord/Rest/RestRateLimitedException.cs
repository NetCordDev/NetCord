using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public class RestRateLimitedException(Route route, int retryAfter, RateLimitScope scope) : Exception($"{RateLimitScopeHelpers.GetString(scope)} rate limit exceeded for route '{route}'. Retry after {retryAfter} ms.")
{
    /// <summary>
    /// The route that was rate limited.
    /// </summary>
    public Route Route => route;

    /// <summary>
    /// The time in milliseconds after which the request can be retried.
    /// </summary>
    public int ResetAfter => retryAfter;

    /// <summary>
    /// The scope of the rate limit that was exceeded.
    /// </summary>
    public RateLimitScope Scope => scope;
}
