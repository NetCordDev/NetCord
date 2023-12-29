using NetCord.Rest.HttpClients;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public class RestClientConfiguration
{
    public string? Hostname { get; init; }
    public ApiVersion Version { get; init; } = ApiVersion.V10;
    public IHttpClient? HttpClient { get; init; }
    public RequestProperties? DefaultRequestProperties { get; init; }
    public IRateLimitManager? RateLimitManager { get; init; }
}
