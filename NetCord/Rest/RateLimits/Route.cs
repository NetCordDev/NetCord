namespace NetCord.Rest.RateLimits;

internal record Route
{
    public Route(HttpMethod method, string endpoint, TopLevelResourceInfo? resourceInfo = null)
    {
        Method = method;
        Endpoint = endpoint;
        ResourceInfo = resourceInfo;
    }

    public HttpMethod Method { get; }

    public string Endpoint { get; }

    public TopLevelResourceInfo? ResourceInfo { get; }
}
