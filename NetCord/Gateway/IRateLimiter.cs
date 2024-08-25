namespace NetCord.Gateway;

public interface IRateLimiter : IDisposable
{
    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync();
}
