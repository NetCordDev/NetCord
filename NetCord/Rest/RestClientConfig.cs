using NetCord.Rest.HttpClients;

namespace NetCord.Rest;

public class RestClientConfig
{
    public IHttpClient? HttpClient { get; init; }
}
