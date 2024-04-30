namespace NetCord.Rest.RateLimits;

public sealed class RateLimitManager : IRateLimitManager
{
    private readonly object _lock = new();
    private readonly GlobalRateLimiter _globalRateLimiter;
    private readonly Dictionary<Route, ITrackingRouteRateLimiter> _routeRateLimiters;
    private readonly Dictionary<BucketInfo, ITrackingRouteRateLimiter> _bucketRateLimiters;
    private readonly int _cacheSize;
    private readonly int _cacheCleanupCount;

    public RateLimitManager(RateLimitManagerConfiguration? configuration = null)
    {
        configuration ??= new();

        int cacheSize = configuration.CacheSize;
        int cacheCleanupCount = configuration.CacheCleanupCount;

        ArgumentOutOfRangeException.ThrowIfLessThan(cacheSize, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(cacheCleanupCount, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(cacheCleanupCount, cacheSize);

        _cacheSize = cacheSize;
        _cacheCleanupCount = cacheCleanupCount;

        _globalRateLimiter = new(configuration.GlobalRateLimit, configuration.GlobalRateLimitDuration);
        _routeRateLimiters = new(cacheSize);
        _bucketRateLimiters = new(cacheSize);
    }

    public ValueTask<IGlobalRateLimiter> GetGlobalRateLimiterAsync() => new(_globalRateLimiter);

    public ValueTask<IRouteRateLimiter> GetRouteRateLimiterAsync(Route route)
    {
        ITrackingRouteRateLimiter? rateLimiter;

        lock (_lock)
        {
            var routeRateLimiters = _routeRateLimiters;

            if (routeRateLimiters.TryGetValue(route, out rateLimiter))
                rateLimiter.IndicateAccess();
            else
            {
                CleanupRateLimiters();
                routeRateLimiters[route] = rateLimiter = new UnknownRouteRateLimiter();
            }
        }

        return new(rateLimiter);
    }

    public async ValueTask ExchangeRouteRateLimiterAsync(Route route, RateLimitInfo? rateLimitInfo, BucketInfo? previousBucketInfo)
    {
        if (rateLimitInfo is null)
        {
            lock (_lock)
            {
                CleanupRateLimiters();
                _routeRateLimiters[route] = new NoRateLimitRouteRateLimiter();
            }

            return;
        }

        ITrackingRouteRateLimiter? rateLimiter;

        var bucketRateLimiters = _bucketRateLimiters;
        lock (_lock)
        {
            if (previousBucketInfo is not null)
                bucketRateLimiters.Remove(previousBucketInfo);

            var bucketInfo = rateLimitInfo.BucketInfo;
            if (bucketRateLimiters.TryGetValue(bucketInfo, out rateLimiter))
            {
                rateLimiter.IndicateAccess();
                CleanupRateLimiters();
                if (bucketRateLimiters.ContainsKey(bucketInfo))
                    _routeRateLimiters[route] = rateLimiter;
            }
            else
            {
                CleanupRateLimiters();
                bucketRateLimiters[bucketInfo] = _routeRateLimiters[route] = new RouteRateLimiter(rateLimitInfo);
                return;
            }
        }

        await rateLimiter.UpdateAsync(rateLimitInfo).ConfigureAwait(false);
    }

    private void CleanupRateLimiters()
    {
        var routeRateLimiters = _routeRateLimiters;
        if (routeRateLimiters.Count != _cacheSize)
            return;

        var bucketRateLimiters = _bucketRateLimiters;

        foreach (var group in routeRateLimiters.GroupBy(pair => pair.Value, pair => pair.Key)
                                               .OrderBy(pair => pair.Key.LastAccess)
                                               .Take(_cacheCleanupCount))
        {
            foreach (var route in group)
                routeRateLimiters.Remove(route);

            var rateLimiter = group.Key;
            if (rateLimiter.HasBucketInfo)
            {
                var bucketInfo = rateLimiter.BucketInfo;
                if (bucketInfo is not null)
                    bucketRateLimiters.Remove(bucketInfo);
            }
        }
    }

    public void Dispose()
    {
    }
}
