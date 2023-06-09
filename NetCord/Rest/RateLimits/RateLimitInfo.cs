namespace NetCord.Rest.RateLimits;

internal readonly struct RateLimitInfo
{
    public RateLimitInfo(int resetAfter, bool rateLimited)
    {
        ResetAfter = resetAfter;
        RateLimited = rateLimited;
    }

    public int ResetAfter { get; }

    public bool RateLimited { get; }
}
