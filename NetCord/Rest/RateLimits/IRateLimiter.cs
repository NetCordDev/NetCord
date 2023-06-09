namespace NetCord.Rest.RateLimits;

internal interface IRateLimiter
{
    public RateLimitInfo TryAcquire();
}
