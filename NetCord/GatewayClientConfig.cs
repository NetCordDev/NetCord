using NetCord.WebSockets;

namespace NetCord;

public class GatewayClientConfig
{
    public GatewayVersion Version { get; init; } = GatewayVersion.V10;
    public GatewayIntent Intents { get; init; } = GatewayIntent.AllNonPrivileged;
    public int? LargeThreshold { get; init; }
    public PresenceProperties? Presence { get; init; }
    public ShardProperties? Shard { get; init; }
    public IWebSocket? WebSocket { get; init; }
}