namespace NetCord.Gateway;

internal sealed class NullRateLimiter : IRateLimiter
{
    public static NullRateLimiter Instance { get; } = new();

    private NullRateLimiter()
    {
    }

    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync() => new(RateLimitAcquisitionResult.NoRateLimit());

    public void Reset()
    {
    }

    public void Dispose()
    {
    }
}
