using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public class RestClientConfiguration
{
    public string? Hostname { get; init; }
    public ApiVersion Version { get; init; } = ApiVersion.V10;
    public IRestRequestHandler? RequestHandler { get; init; }
    public RequestProperties? DefaultRequestProperties { get; init; }
    public IRateLimitManager? RateLimitManager { get; init; }
}
