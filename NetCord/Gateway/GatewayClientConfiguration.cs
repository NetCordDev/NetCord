using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectTimers;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public class GatewayClientConfiguration
{
    public IWebSocket? WebSocket { get; init; }
    public IReconnectTimer? ReconnectTimer { get; init; }
    public ILatencyTimer? LatencyTimer { get; init; }
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
    public Rest.RestClientConfiguration? RestClientConfiguration { get; init; }
}
