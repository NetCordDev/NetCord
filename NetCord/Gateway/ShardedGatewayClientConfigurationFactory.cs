using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Rest;

namespace NetCord.Gateway;

internal static class ShardedGatewayClientConfigurationFactory
{
    public static ShardedGatewayClientConfiguration Create(Func<Shard, IWebSocketConnectionProvider?>? webSocketConnectionProviderFactory,
                                                           Func<Shard, IRateLimiterProvider?>? rateLimiterProviderFactory,
                                                           Func<Shard, WebSocketPayloadProperties?>? defaultPayloadPropertiesFactory,
                                                           Func<Shard, IReconnectStrategy?>? reconnectStrategyFactory,
                                                           Func<Shard, ILatencyTimer?>? latencyTimerFactory,
                                                           Func<Shard, ApiVersion?>? versionFactory,
                                                           Func<Shard, IGatewayClientCache?>? cacheFactory,
                                                           Func<Shard, IGatewayCompression?>? compressionFactory,
                                                           Func<Shard, GatewayIntents?>? intentsFactory,
                                                           string? hostname,
                                                           Func<Shard, ConnectionPropertiesProperties?>? connectionPropertiesFactory,
                                                           Func<Shard, int?>? largeThresholdFactory,
                                                           Func<Shard, PresenceProperties?>? presenceFactory,
                                                           int? shardCount,
                                                           RestClientConfiguration? restClientConfiguration)
    {
        return new()
        {
            WebSocketConnectionProviderFactory = webSocketConnectionProviderFactory,
            RateLimiterProviderFactory = rateLimiterProviderFactory,
            DefaultPayloadPropertiesFactory = defaultPayloadPropertiesFactory,
            ReconnectStrategyFactory = reconnectStrategyFactory,
            LatencyTimerFactory = latencyTimerFactory,
            VersionFactory = versionFactory,
            CacheFactory = cacheFactory,
            CompressionFactory = compressionFactory,
            IntentsFactory = intentsFactory,
            Hostname = hostname,
            ConnectionPropertiesFactory = connectionPropertiesFactory,
            LargeThresholdFactory = largeThresholdFactory,
            PresenceFactory = presenceFactory,
            ShardCount = shardCount,
            RestClientConfiguration = restClientConfiguration,
        };
    }
}
