using NetCord.Rest;
using NetCord.Rest.RateLimits;

namespace NetCord.Hosting.Rest;

public class RestClientOptions : IDiscordOptions
{
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.Hostname" />
    public string? Hostname { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.Version" />
    public ApiVersion? Version { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.RequestHandler" />
    public IRestRequestHandler? RequestHandler { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.DefaultRequestProperties" />
    public RestRequestProperties? DefaultRequestProperties { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.RateLimitManager" />
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
