namespace NetCord.Rest.RateLimits;

public class RateLimitManager : IRateLimitManager
{
    private readonly object _lock = new();
    private readonly GlobalRateLimiter _globalRateLimiter = new();
    private readonly Dictionary<Route, IRouteRateLimiter> _routeRateLimiters;
    private readonly Dictionary<BucketInfo, IRouteRateLimiter> _bucketRateLimiters;
    private readonly int _cacheSize;
    private readonly int _cacheCleanupFrequency;
    private int _routeCacheCounter;
    private int _bucketCacheCounter;

    public RateLimitManager(int cacheSize = 9103, int cacheCleanupFrequency = 1000)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(cacheSize, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(cacheCleanupFrequency, 0);

        var maxSize = (_cacheSize = cacheSize) + (_cacheCleanupFrequency = cacheCleanupFrequency);
        _routeRateLimiters = new(maxSize);
        _bucketRateLimiters = new(maxSize);
    }

    public ValueTask<IGlobalRateLimiter> GetGlobalRateLimiterAsync() => new(_globalRateLimiter);

    public ValueTask<IRouteRateLimiter> GetRouteRateLimiterAsync(Route route)
    {
        IRouteRateLimiter? rateLimiter;

        lock (_lock)
        {
            var routeRateLimiters = _routeRateLimiters;

            if (routeRateLimiters.TryGetValue(route, out rateLimiter))
            {
                var bucket = rateLimiter.BucketInfo;
                if (bucket is not null && _bucketRateLimiters.TryAdd(bucket, rateLimiter!))
                    TrimRateLimiters(_bucketRateLimiters, ref _bucketCacheCounter);

                return new(rateLimiter);
            }

            routeRateLimiters[route] = rateLimiter = new UnknownRouteRateLimiter();

            TrimRateLimiters(routeRateLimiters, ref _routeCacheCounter);
        }

        return new(rateLimiter);
    }

    public async ValueTask ExchangeRouteRateLimiterAsync(Route route, RateLimitInfo? rateLimitInfo)
    {
        if (rateLimitInfo is null)
        {
            lock (_lock)
            {
                _routeRateLimiters[route] = new UnknownRouteRateLimiter();

                TrimRateLimiters(_routeRateLimiters, ref _routeCacheCounter);
            }

            return;
        }

        bool update;
        IRouteRateLimiter? rateLimiter;
        var bucketInfo = rateLimitInfo.BucketInfo;
        lock (_lock)
        {
            if (update = _bucketRateLimiters.TryGetValue(bucketInfo, out rateLimiter))
                _routeRateLimiters[route] = rateLimiter!;
            else
            {
                _bucketRateLimiters[bucketInfo] = _routeRateLimiters[route] = new RouteRateLimiter(rateLimitInfo);

                TrimRateLimiters(_bucketRateLimiters, ref _bucketCacheCounter);
            }
        }

        if (update)
            await rateLimiter!.UpdateAsync(rateLimitInfo).ConfigureAwait(false);
    }

    private void TrimRateLimiters<TKey>(Dictionary<TKey, IRouteRateLimiter> cache, ref int counter) where TKey : notnull
    {
        var toRemove = cache.Count - _cacheSize;
        if (toRemove > 0 && ++counter == _cacheCleanupFrequency)
        {
            counter = 0;
            foreach (var pair in cache)
            {
                cache.Remove(pair.Key);

                if (--toRemove == 0)
                    break;
            }
        }
    }

    public void Dispose()
    {
    }
}
