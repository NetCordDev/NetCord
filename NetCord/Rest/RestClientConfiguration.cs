using NetCord.Rest.HttpClients;

namespace NetCord.Rest;

public class RestClientConfiguration
{
    public string? Hostname { get; init; }
    public ApiVersion Version { get; init; } = ApiVersion.V10;
    public IHttpClient? HttpClient { get; init; }
}
