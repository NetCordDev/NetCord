namespace NetCord.Rest.RateLimits;

public record Route
{
    public Route(RouteParameter? sensitiveParameter, Snowflake? sensitiveId = null, bool globalRateLimit = true)
    {
        SensitiveParameter = sensitiveParameter;
        SensitiveId = sensitiveId;
        GlobalRateLimit = globalRateLimit;
    }

    public RouteParameter? SensitiveParameter { get; }

    public Snowflake? SensitiveId { get; }

    public bool GlobalRateLimit { get; }
}
