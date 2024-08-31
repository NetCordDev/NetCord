namespace NetCord.Rest.RateLimits;

public interface IRateLimitManager : IDisposable
{
    public ValueTask<IGlobalRateLimiter> GetGlobalRateLimiterAsync(CancellationToken cancellationToken = default);

    public ValueTask<IRouteRateLimiter> GetRouteRateLimiterAsync(Route route, CancellationToken cancellationToken = default);

    public ValueTask ExchangeRouteRateLimiterAsync(Route route, RateLimitInfo? rateLimitInfo, BucketInfo? previousBucketInfo, CancellationToken cancellationToken = default);
}
