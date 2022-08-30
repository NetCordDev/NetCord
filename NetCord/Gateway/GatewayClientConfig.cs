using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public class GatewayClientConfig
{
    public ConnectionPropertiesProperties? ConnectionProperties { get; init; }
    public GatewayVersion Version { get; init; } = GatewayVersion.V10;
    public GatewayIntent Intents { get; init; } = GatewayIntent.AllNonPrivileged;
    public int? LargeThreshold { get; init; }
    public PresenceProperties? Presence { get; init; }
    public ShardProperties? Shard { get; init; }
    public IWebSocket? WebSocket { get; init; }
    public Rest.RestClientConfig? RestClientConfig { get; init; }
}
