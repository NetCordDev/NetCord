using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public class ShardedGatewayClientConfiguration
{
    public Func<Shard, IWebSocketConnectionProvider?>? WebSocketConnectionProviderFactory { get; init; }
    public Func<Shard, IRateLimiterProvider?>? RateLimiterProviderFactory { get; init; }
    public Func<Shard, WebSocketPayloadProperties?>? DefaultPayloadPropertiesFactory { get; init; }
    public Func<Shard, IReconnectStrategy?>? ReconnectStrategyFactory { get; init; }
    public Func<Shard, ILatencyTimer?>? LatencyTimerFactory { get; init; }
    public Func<Shard, ApiVersion?>? VersionFactory { get; init; }
    public Func<Shard, IGatewayClientCache?>? CacheFactory { get; init; }
    public Func<Shard, IGatewayCompression?>? CompressionFactory { get; init; }
    public Func<Shard, GatewayIntents?>? IntentsFactory { get; init; }
    public string? Hostname { get; init; }
    public Func<Shard, ConnectionPropertiesProperties?>? ConnectionPropertiesFactory { get; init; }
    public Func<Shard, int?>? LargeThresholdFactory { get; init; }
    public Func<Shard, PresenceProperties?>? PresenceFactory { get; init; }
    public int? ShardCount { get; init; }
    public Rest.RestClientConfiguration? RestClientConfiguration { get; init; }
}
