﻿namespace NetCord.Rest.RateLimits;

internal class NoRateLimitRouteRateLimiter : ITrackingRouteRateLimiter
{
    public bool HasBucketInfo => true;

    public BucketInfo? BucketInfo => null;

    public long LastAccess { get; private set; } = Environment.TickCount64;

    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync(CancellationToken cancellationToken = default)
    {
        return new(RateLimitAcquisitionResult.NoRateLimit);
    }

    public ValueTask CancelAcquireAsync(long acquisitionTimestamp, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask IndicateExchangeAsync(long timestamp, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public void IndicateAccess()
    {
        LastAccess = Environment.TickCount64;
    }
}
