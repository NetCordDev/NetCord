using NetCord.Logging;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public class RestClientConfiguration
{
    /// <summary>
    /// The hostname to use for the <see cref="RestClient"/>. Defaults to <see cref="Discord.RestHostname"/>.
    /// </summary>
    public string? Hostname { get; init; }

    /// <summary>
    /// The version of the Discord API to use. Defaults to <see cref="ApiVersion.V10"/>.
    /// </summary>
    public ApiVersion? Version { get; init; }

    /// <summary>
    /// The request handler for the <see cref="RestClient"/>. Defaults to <see cref="RestRequestHandler"/>.
    /// </summary>
    public IRestRequestHandler? RequestHandler { get; init; }

    /// <summary>
    /// The default request properties for the <see cref="RestClient"/>. Defaults to <see langword="null"/>.
    /// </summary>
    public RestRequestProperties? DefaultRequestProperties { get; init; }

    /// <summary>
    /// The rate limit manager for the <see cref="RestClient"/>. Defaults to <see cref="RateLimits.RateLimitManager"/>.
    /// </summary>
    public IRateLimitManager? RateLimitManager { get; init; }

    /// <summary>
    /// The logger for the <see cref="RestClient"/>. Defaults to <see cref="NullLogger"/>.
    /// </summary>
    public IRestLogger? Logger { get; init; }
}
