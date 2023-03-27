using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public class GatewayClientConfiguration
{
    public string? Hostname { get; init; }
    public ConnectionPropertiesProperties? ConnectionProperties { get; init; }
    public ApiVersion Version { get; init; } = ApiVersion.V10;
    public GatewayIntents Intents { get; init; } = GatewayIntents.AllNonPrivileged;
    public int? LargeThreshold { get; init; }
    public PresenceProperties? Presence { get; init; }
    public ShardProperties? Shard { get; init; }
    public IWebSocket? WebSocket { get; init; }
    public bool CacheDMChannels { get; init; } = true;
    public Rest.RestClientConfiguration? RestClientConfiguration { get; init; }
}
