namespace NetCord.Rest.RateLimits;

public record Route
{
    public Route(HttpMethod method, string endPoint, TopLevelResourceInfo? resourceInfo = null)
    {
        Method = method;
        EndPoint = endPoint;
        ResourceInfo = resourceInfo;
    }

    public HttpMethod Method { get; }

    public string EndPoint { get; }

    public TopLevelResourceInfo? ResourceInfo { get; }
}
