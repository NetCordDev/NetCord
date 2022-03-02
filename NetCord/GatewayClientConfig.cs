namespace NetCord;

public class GatewayClientConfig
{
    public GatewayIntent Intents { get; init; } = GatewayIntent.AllNonPrivileged;
    public int? LargeThreshold { get; init; }
    public PresenceProperties? Presence { get; init; }
    public ShardProperties? Shard { get; init; }
}