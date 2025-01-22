using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Rest;

namespace NetCord.Gateway;

// Needs to be in sync with GatewayClientConfigurationFactory

public class GatewayClientConfiguration : IWebSocketClientConfiguration
{
    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; init; }
    public IRateLimiterProvider? RateLimiterProvider { get; init; }
    public WebSocketPayloadProperties? DefaultPayloadProperties { get; init; }
    public IReconnectStrategy? ReconnectStrategy { get; init; }
    public ILatencyTimer? LatencyTimer { get; init; }
    public ApiVersion? Version { get; init; }
    public IGatewayClientCache? Cache { get; init; }
    public IGatewayCompression? Compression { get; init; }
    public GatewayIntents? Intents { get; init; }
    public string? Hostname { get; init; }
    public ConnectionPropertiesProperties? ConnectionProperties { get; init; }
    public int? LargeThreshold { get; init; }
    public PresenceProperties? Presence { get; init; }
    public Shard? Shard { get; init; }
    public RestClientConfiguration? RestClientConfiguration { get; init; }

    IRateLimiterProvider? IWebSocketClientConfiguration.RateLimiterProvider => RateLimiterProvider is { } rateLimiter ? rateLimiter : new GatewayRateLimiterProvider(120, 60_000);
}
