using NetCord.Rest.HttpClients;

namespace NetCord.Rest;

public class RestClientConfiguration
{
    public IHttpClient? HttpClient { get; init; }
}
