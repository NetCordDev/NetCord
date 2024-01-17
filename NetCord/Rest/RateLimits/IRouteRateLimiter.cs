namespace NetCord.Rest.RateLimits;

public interface IRouteRateLimiter : IRateLimiter
{
    public bool HasBucketInfo { get; }

    public BucketInfo? BucketInfo { get; }

    public ValueTask CancelAcquireAsync(long timestamp);

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo);

    public ValueTask IndicateExchangeAsync(long timestamp);
}
