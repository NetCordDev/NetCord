namespace NetCord.Rest.RateLimits;

internal class NoRateLimitRouteRateLimiter : ITrackingRouteRateLimiter
{
    public bool HasBucketInfo => true;

    public BucketInfo? BucketInfo => null;

    public long LastAccess { get; private set; } = Environment.TickCount64;

    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync()
    {
        return new(RateLimitAcquisitionResult.NoRateLimit());
    }

    public ValueTask CancelAcquireAsync(long timestamp)
    {
        return default;
    }

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo)
    {
        return default;
    }

    public ValueTask IndicateExchangeAsync(long timestamp)
    {
        return default;
    }

    public void IndicateAccess()
    {
        LastAccess = Environment.TickCount64;
    }
}
