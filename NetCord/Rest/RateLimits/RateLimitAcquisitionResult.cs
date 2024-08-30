namespace NetCord.Rest.RateLimits;

public readonly struct RateLimitAcquisitionResult
{
    private RateLimitAcquisitionResult(int resetAfter, bool rateLimited, bool alwaysRetryOnce)
    {
        ResetAfter = resetAfter;
        RateLimited = rateLimited;
        AlwaysRetry = alwaysRetryOnce;
    }

    public static RateLimitAcquisitionResult Retry { get; } = new(0, false, true);

    public static RateLimitAcquisitionResult NoRateLimit { get; } = new(0, false, false);

    public static RateLimitAcquisitionResult RateLimit(int resetAfter) => new(resetAfter, true, false);

    public int ResetAfter { get; }

    public bool RateLimited { get; }

    public bool AlwaysRetry { get; }
}
