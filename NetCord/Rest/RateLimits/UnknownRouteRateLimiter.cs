namespace NetCord.Rest.RateLimits;

internal class UnknownRouteRateLimiter : ITrackingRouteRateLimiter
{
    private bool _retry;
    private readonly SemaphoreSlim _semaphore = new(1);

    public bool HasBucketInfo => false;

    public BucketInfo? BucketInfo => throw new InvalidOperationException();

    public long LastAccess { get; private set; } = Environment.TickCount64;

    public async ValueTask<RateLimitAcquisitionResult> TryAcquireAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        if (_retry)
            return RateLimitAcquisitionResult.Retry;

        _retry = true;

        return RateLimitAcquisitionResult.NoRateLimit;
    }

    public ValueTask CancelAcquireAsync(long acquisitionTimestamp, CancellationToken cancellationToken = default)
    {
        _retry = false;
        _semaphore.Release();

        return default;
    }

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo, CancellationToken cancellationToken = default)
    {
        _retry = false;
        _semaphore.Release();

        return default;
    }

    public ValueTask IndicateExchangeAsync(long timestamp, CancellationToken cancellationToken = default)
    {
        _retry = true;
        _semaphore.Release(int.MaxValue);

        return default;
    }

    public void IndicateAccess()
    {
        LastAccess = Environment.TickCount64;
    }
}
