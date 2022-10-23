namespace NetCord.Rest.RateLimits;

public record Route
{
    public Route(RouteParameter? sensitiveParameter, ulong? sensitiveId = null, bool globalRateLimit = true)
    {
        SensitiveParameter = sensitiveParameter;
        SensitiveId = sensitiveId;
        GlobalRateLimit = globalRateLimit;
    }

    public RouteParameter? SensitiveParameter { get; }

    public ulong? SensitiveId { get; }

    public bool GlobalRateLimit { get; }
}
