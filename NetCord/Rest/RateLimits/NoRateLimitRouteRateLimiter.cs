
namespace NetCord.Rest.RateLimits;

internal class NoRateLimitRouteRateLimiter : IRouteRateLimiter
{
    public BucketInfo? BucketInfo => null;

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
}
