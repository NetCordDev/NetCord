using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;
using NetCord.Rest;

namespace NetCord.Gateway;

// Needs to be in sync with GatewayClientConfigurationFactory

public class GatewayClientConfiguration : IWebSocketClientConfiguration, IRestClientOwnerConfiguration
{
    /// <inheritdoc/>
    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; init; }

    /// <summary>
    /// <inheritdoc cref="IWebSocketClientConfiguration.RateLimiterProvider"/> Defaults to <see cref="GatewayRateLimiterProvider"/>.
    /// </summary>
    public IRateLimiterProvider? RateLimiterProvider { get; init; }

    /// <inheritdoc/>
    public WebSocketPayloadProperties? DefaultPayloadProperties { get; init; }

    /// <inheritdoc/>
    public IReconnectStrategy? ReconnectStrategy { get; init; }

    /// <inheritdoc/>
    public ILatencyTimer? LatencyTimer { get; init; }

    /// <summary>
    /// The version of the Discord Gateway to use. Defaults to <see cref="ApiVersion.V10"/>.
    /// </summary>
    public ApiVersion? Version { get; init; }

    /// <summary>
    /// The cache provider for the <see cref="GatewayClient"/>. Defaults to <see cref="ImmutableGatewayClientCacheProvider.Empty"/>.
    /// </summary>
    public IGatewayClientCacheProvider? CacheProvider { get; init; }

    /// <summary>
    /// The compression provider for the <see cref="GatewayClient"/>. Defaults to <see cref="ZstandardGatewayCompression"/> if available, otherwise <see cref="ZLibGatewayCompression"/>.
    /// </summary>
    public IGatewayCompression? Compression { get; init; }

    /// <summary>
    /// The intents to use for the <see cref="GatewayClient"/>. Defaults to <see cref="GatewayIntents.AllNonPrivileged"/>.
    /// </summary>
    public GatewayIntents? Intents { get; init; }

    /// <summary>
    /// The hostname to use for the <see cref="GatewayClient"/>. Defaults to <see cref="Discord.GatewayHostname"/>.
    /// </summary>
    public string? Hostname { get; init; }

    /// <summary>
    /// The properties for the connection of the <see cref="GatewayClient"/>. Defaults to <see langword="null"/>.
    /// </summary>
    public ConnectionPropertiesProperties? ConnectionProperties { get; init; }

    /// <summary>
    /// The large threshold for the <see cref="GatewayClient"/>.
    /// Value between 50 and 250, total number of guild users where the gateway will stop sending offline guild users in the guild user list.
    /// Defaults to 50.
    /// </summary>
    public int? LargeThreshold { get; init; }

    /// <summary>
    /// The presence properties for the <see cref="GatewayClient"/>. Defaults to <see langword="null"/>.
    /// </summary>
    public PresenceProperties? Presence { get; init; }

    /// <summary>
    /// The shard for the <see cref="GatewayClient"/>. Defaults to <see langword="null"/>.
    /// </summary>
    public Shard? Shard { get; init; }

    /// <summary>
    /// The configuration for the <see cref="RestClient"/> at <see cref="GatewayClient.Rest"/>. Defaults to <see langword="null"/>.
    /// </summary>
    public RestClientConfiguration? RestClientConfiguration { get; init; }

    /// <summary>
    /// The logger for the <see cref="GatewayClient"/>. Defaults to <see cref="NullLogger"/>.
    /// </summary>
    public IGatewayLogger? Logger { get; init; }

    IRateLimiterProvider? IWebSocketClientConfiguration.RateLimiterProvider => RateLimiterProvider is { } rateLimiter ? rateLimiter : new GatewayRateLimiterProvider(120, 60_000);

    IWebSocketLogger? IWebSocketClientConfiguration.Logger => Logger switch
    {
        null or NullLogger => NullLogger.Instance,
        _ => new GatewayWebSocketLogger(Logger),
    };

    IRestLogger? IRestClientOwnerConfiguration.Logger => Logger as IRestLogger;
}
