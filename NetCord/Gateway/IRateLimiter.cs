namespace NetCord.Gateway;

public interface IRateLimiter : IDisposable
{
    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync(CancellationToken cancellationToken = default);

    public ValueTask CancelAcquireAsync(long acquisitionTimestamp, CancellationToken cancellationToken = default);
}
