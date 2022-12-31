using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public class GatewayClientConfiguration
{
    public ConnectionPropertiesProperties? ConnectionProperties { get; init; }
    public GatewayVersion Version { get; init; } = GatewayVersion.V10;
    public GatewayIntents Intents { get; init; } = GatewayIntents.AllNonPrivileged;
    public int? LargeThreshold { get; init; }
    public PresenceProperties? Presence { get; init; }
    public ShardProperties? Shard { get; init; }
    public IWebSocket? WebSocket { get; init; }
    public Rest.RestClientConfiguration? RestClientConfiguration { get; init; }
}
