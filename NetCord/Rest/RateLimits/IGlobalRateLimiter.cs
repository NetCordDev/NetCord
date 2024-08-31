namespace NetCord.Rest.RateLimits;

public interface IGlobalRateLimiter : IRateLimiter
{
    public ValueTask IndicateRateLimitAsync(long reset, CancellationToken cancellationToken = default);
}
