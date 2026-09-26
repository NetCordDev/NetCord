using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;
using NetCord.Rest;

namespace NetCord.Gateway;

internal static class GatewayClientConfigurationFactory
{
    public static GatewayClientConfiguration Create(IWebSocketConnectionProvider? webSocketConnectionProvider,
                                                    IRateLimiterProvider? rateLimiterProvider,
                                                    WebSocketMessageProperties? defaultMessageProperties,
                                                    IReconnectStrategy? reconnectStrategy,
                                                    ILatencyTimer? latencyTimer,
                                                    ApiVersion? version,
                                                    IGatewayClientCacheProvider? cacheProvider,
                                                    IGatewayCompression? compression,
                                                    GatewayIntents? intents,
                                                    string? hostname,
                                                    ConnectionPropertiesProperties? connectionProperties,
                                                    int? largeThreshold,
                                                    PresenceProperties? presence,
                                                    Shard? shard,
                                                    RestClientConfiguration? restClientConfiguration,
                                                    IGatewayLogger? logger)
    {
        return new()
        {
            WebSocketConnectionProvider = webSocketConnectionProvider,
            RateLimiterProvider = rateLimiterProvider,
            DefaultMessageProperties = defaultMessageProperties,
            ReconnectStrategy = reconnectStrategy,
            LatencyTimer = latencyTimer,
            Version = version,
            CacheProvider = cacheProvider,
            Compression = compression,
            Intents = intents,
            Hostname = hostname,
            ConnectionProperties = connectionProperties,
            LargeThreshold = largeThreshold,
            Presence = presence,
            Shard = shard,
            RestClientConfiguration = restClientConfiguration,
            Logger = logger,
        };
    }
}
