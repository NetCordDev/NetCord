namespace NetCord.Rest.RateLimits;

public record Route
{
    public Route(RouteParameter? sensitiveParameter, Snowflake? sensitiveId = null)
    {
        SensitiveParameter = sensitiveParameter;
        SensitiveId = sensitiveId;
    }

    public RouteParameter? SensitiveParameter { get; }

    public Snowflake? SensitiveId { get; }
}