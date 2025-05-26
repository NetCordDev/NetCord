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

    public override string ToString()
    {
        if (ResourceInfo is TopLevelResourceInfo resourceInfo)
            return $"{Method} {string.Format(EndPoint, resourceInfo.ResourceId, resourceInfo.ResourceToken)}";

        return $"{Method} {EndPoint}";
    }
}
