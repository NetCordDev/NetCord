using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectTimers;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public class ShardedGatewayClientConfiguration
{
    public Func<Shard, IWebSocket?>? WebSocketFactory { get; init; }
    public Func<Shard, IReconnectTimer?>? ReconnectTimerFactory { get; init; }
    public Func<Shard, ILatencyTimer?>? LatencyTimerFactory { get; init; }
    public Func<Shard, IGatewayCompression?>? CompressionFactory { get; init; }
    public string? Hostname { get; init; }
    public Func<Shard, ConnectionPropertiesProperties?>? ConnectionPropertiesFactory { get; init; }
    public Func<Shard, ApiVersion>? VersionFactory { get; init; }
    public Func<Shard, GatewayIntents>? IntentsFactory { get; init; }
    public Func<Shard, int?>? LargeThresholdFactory { get; init; }
    public Func<Shard, PresenceProperties?>? PresenceFactory { get; init; }
    public int? ShardCount { get; init; }
    public bool CacheDMChannels { get; init; } = true;
    public Rest.RestClientConfiguration? RestClientConfiguration { get; init; }
}
