namespace NetCord.Rest.RateLimits;

public interface IRateLimitManager : IDisposable
{
    public ValueTask<IGlobalRateLimiter> GetGlobalRateLimiterAsync();

    public ValueTask<IRouteRateLimiter> GetRouteRateLimiterAsync(Route route);

    public ValueTask ExchangeRouteRateLimiterAsync(Route route, RateLimitInfo? rateLimitInfo);
}
