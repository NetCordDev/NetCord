using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Rest;

namespace NetCord.Gateway;

internal static class GatewayClientConfigurationFactory
{
    public static GatewayClientConfiguration Create(IWebSocketConnectionProvider? webSocketConnectionProvider,
                                                    IRateLimiterProvider? rateLimiterProvider,
                                                    WebSocketPayloadProperties? defaultPayloadProperties,
                                                    IReconnectStrategy? reconnectStrategy,
                                                    ILatencyTimer? latencyTimer,
                                                    ApiVersion? version,
                                                    IGatewayClientCache? cache,
                                                    IGatewayCompression? compression,
                                                    GatewayIntents? intents,
                                                    string? hostname,
                                                    ConnectionPropertiesProperties? connectionProperties,
                                                    int? largeThreshold,
                                                    PresenceProperties? presence,
                                                    Shard? shard,
                                                    RestClientConfiguration? restClientConfiguration)
    {
        return new()
        {
            WebSocketConnectionProvider = webSocketConnectionProvider,
            RateLimiterProvider = rateLimiterProvider,
            DefaultPayloadProperties = defaultPayloadProperties,
            ReconnectStrategy = reconnectStrategy,
            LatencyTimer = latencyTimer,
            Version = version,
            Cache = cache,
            Compression = compression,
            Intents = intents,
            Hostname = hostname,
            ConnectionProperties = connectionProperties,
            LargeThreshold = largeThreshold,
            Presence = presence,
            Shard = shard,
            RestClientConfiguration = restClientConfiguration,
        };
    }
}
