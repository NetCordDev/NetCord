namespace NetCord.Rest.RateLimits;

public class RateLimitManagerConfiguration
{
    public int CacheSize { get; init; } = 10103;
    public int CacheCleanupCount { get; init; } = 5000;
    public int GlobalRateLimit { get; init; } = 50;
    public long GlobalRateLimitDuration { get; init; } = 1000;
}
