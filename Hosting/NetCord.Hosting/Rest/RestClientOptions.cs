using NetCord.Rest;
using NetCord.Rest.RateLimits;

namespace NetCord.Hosting.Rest;

public class RestClientOptions : IDiscordOptions
{
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    public string? Hostname { get; set; }

    public ApiVersion? Version { get; set; }

    public IRestRequestHandler? RequestHandler { get; set; }

    public RestRequestProperties? DefaultRequestProperties { get; set; }

    public IRateLimitManager? RateLimitManager { get; set; }

    internal RestClientConfiguration CreateConfiguration()
    {
        return new()
        {
            Hostname = Hostname,
            Version = Version,
            RequestHandler = RequestHandler,
            DefaultRequestProperties = DefaultRequestProperties,
            RateLimitManager = RateLimitManager,
        };
    }
}
