using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;

namespace NetCord.Gateway;

public class GatewayClientConfiguration : IWebSocketClientConfiguration
{
    public IWebSocket? WebSocket { get; init; }
    public IReconnectStrategy? ReconnectStrategy { get; init; }
    public ILatencyTimer? LatencyTimer { get; init; }
    public IGatewayLogger? Logger { get; init; }
    public ApiVersion Version { get; init; } = ApiVersion.V10;
    public IGatewayClientCache? Cache { get; init; }
    public IGatewayCompression? Compression { get; init; }
    public GatewayIntents Intents { get; init; } = GatewayIntents.AllNonPrivileged;
    public string? Hostname { get; init; }
    public ConnectionPropertiesProperties? ConnectionProperties { get; init; }
    public int? LargeThreshold { get; init; }
    public PresenceProperties? Presence { get; init; }
    public Shard? Shard { get; init; }
    public bool CacheDMChannels { get; init; } = true;
    public Rest.RestClientConfiguration? RestConfiguration { get; init; }

    IWebSocketLogger IWebSocketClientConfiguration.Logger => new GatewayWebSocketLogger(Logger ?? new ConsoleLogger(LogLevel.Information));
}
