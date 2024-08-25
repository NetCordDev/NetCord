namespace NetCord.Gateway;

internal class NullRateLimiterProvider : IRateLimiterProvider
{
    public static NullRateLimiterProvider Instance { get; } = new();

    public IRateLimiter CreateRateLimiter() => NullRateLimiter.Instance;
}
