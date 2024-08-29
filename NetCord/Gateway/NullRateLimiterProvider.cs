namespace NetCord.Gateway;

public class NullRateLimiterProvider : IRateLimiterProvider
{
    public static NullRateLimiterProvider Instance { get; } = new();

    public IRateLimiter CreateRateLimiter() => NullRateLimiter.Instance;

    private sealed class NullRateLimiter : IRateLimiter
    {
        public static NullRateLimiter Instance { get; } = new();

        private NullRateLimiter()
        {
        }

        public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync(CancellationToken cancellationToken = default) => new(RateLimitAcquisitionResult.NoRateLimit);

        public ValueTask CancelAcquireAsync(long acquisitionTimestamp, CancellationToken cancellationToken = default) => default;

        public void Dispose()
        {
        }
    }
}
