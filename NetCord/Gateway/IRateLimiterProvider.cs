namespace NetCord.Gateway;

public interface IRateLimiterProvider
{
    public IRateLimiter CreateRateLimiter();
}
