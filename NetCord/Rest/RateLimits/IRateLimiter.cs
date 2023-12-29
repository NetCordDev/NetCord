namespace NetCord.Rest.RateLimits;

public interface IRateLimiter
{
    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync();
}
