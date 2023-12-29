namespace NetCord.Rest.RateLimits;

internal class UnknownRouteRateLimiter : IRouteRateLimiter
{
    private bool _retry;
    private readonly SemaphoreSlim _semaphore = new(1);

    public BucketInfo? BucketInfo => null;

    public async ValueTask<RateLimitAcquisitionResult> TryAcquireAsync()
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        if (_retry)
            return RateLimitAcquisitionResult.Retry();

        _retry = true;

        return RateLimitAcquisitionResult.NoRateLimit();
    }

    public ValueTask CancelAcquireAsync(long timestamp)
    {
        _retry = false;
        _semaphore.Release();

        return default;
    }

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo)
    {
        _retry = false;
        _semaphore.Release();

        return default;
    }

    public ValueTask IndicateExchangeAsync(long timestamp)
    {
        _retry = true;
        _semaphore.Release(int.MaxValue);

        return default;
    }
}
