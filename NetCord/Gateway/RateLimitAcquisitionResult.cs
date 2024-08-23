namespace NetCord.Gateway;

public readonly struct RateLimitAcquisitionResult
{
    private RateLimitAcquisitionResult(int resetAfter, bool rateLimited)
    {
        ResetAfter = resetAfter;
        RateLimited = rateLimited;
    }

    public static RateLimitAcquisitionResult NoRateLimit() => new(0, false);

    public static RateLimitAcquisitionResult RateLimit(int resetAfter) => new(resetAfter, true);

    public int ResetAfter { get; }

    public bool RateLimited { get; }
}
