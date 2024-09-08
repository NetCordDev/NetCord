namespace NetCord.Rest.RateLimits;

public interface IRouteRateLimiter : IRateLimiter
{
    public bool HasBucketInfo { get; }

    public BucketInfo? BucketInfo { get; }

    public ValueTask CancelAcquireAsync(long acquisitionTimestamp, CancellationToken cancellationToken = default);

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo, CancellationToken cancellationToken = default);

    public ValueTask IndicateExchangeAsync(long timestamp, CancellationToken cancellationToken = default);
}
