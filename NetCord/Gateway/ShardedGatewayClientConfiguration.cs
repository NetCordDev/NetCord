using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;
using NetCord.Rest;

namespace NetCord.Gateway;

public class ShardedGatewayClientConfiguration : IRestClientOwnerConfiguration
{
    /// <inheritdoc cref="GatewayClientConfiguration.WebSocketConnectionProvider" />
    public Func<Shard, IWebSocketConnectionProvider?>? WebSocketConnectionProviderFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.RateLimiterProvider" />
    public Func<Shard, IRateLimiterProvider?>? RateLimiterProviderFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.DefaultPayloadProperties" />
    public Func<Shard, WebSocketPayloadProperties?>? DefaultPayloadPropertiesFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.ReconnectStrategy" />
    public Func<Shard, IReconnectStrategy?>? ReconnectStrategyFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.LatencyTimer" />
    public Func<Shard, ILatencyTimer?>? LatencyTimerFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.Version" />
    public Func<Shard, ApiVersion?>? VersionFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.CacheProvider" />
    public Func<Shard, IGatewayClientCacheProvider?>? CacheProviderFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.Compression" />
    public Func<Shard, IGatewayCompression?>? CompressionFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.Intents" />
    public Func<Shard, GatewayIntents?>? IntentsFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.Hostname" />
    public string? Hostname { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.ConnectionProperties" />
    public Func<Shard, ConnectionPropertiesProperties?>? ConnectionPropertiesFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.LargeThreshold" />
    public Func<Shard, int?>? LargeThresholdFactory { get; init; }

    /// <inheritdoc cref="GatewayClientConfiguration.Presence" />
    public Func<Shard, PresenceProperties?>? PresenceFactory { get; init; }

    /// <summary>
    /// The maximum number of shards that can connect concurrently. If <see langword="null"/>, it will be determined by Discord.
    /// </summary>
    public int? MaxConcurrency { get; init; }

    /// <summary>
    /// The range of shards to use. Note that shard IDs are zero-based and the end value is exclusive. For example, to use shards 2, 3, and 4, set this to <c>2..5</c>. If <see langword="null"/>, all shards from 0 to <see cref="TotalShardCount"/> - 1 will be used.
    /// </summary>
    /// <remarks>
    /// <see cref="TotalShardCount"/> is required to be set when this property is set.
    /// </remarks>
    public Range? ShardRange { get; init; }

    /// <summary>
    /// The total number of shards. If <see langword="null"/>, the number of shards will be determined by Discord.
    /// </summary>
    public int? TotalShardCount { get; init; }

    /// <summary>
    /// The configuration for the <see cref="RestClient"/> at <see cref="ShardedGatewayClient.Rest"/>
    /// and for each shard at <see cref="GatewayClient.Rest"/>. Defaults to <see langword="null"/>.
    /// </summary>
    public RestClientConfiguration? RestClientConfiguration { get; init; }

    /// <summary>
    /// <inheritdoc cref="GatewayClientConfiguration.Logger" />The <see cref="Shard"/> argument is <see langword="null"/> for the <see cref="RestClient"/>'s logger.
    /// </summary>
    public Func<Shard?, IGatewayLogger?>? LoggerFactory { get; init; }

    IRestLogger? IRestClientOwnerConfiguration.Logger => LoggerFactory is { } loggerFactory ? loggerFactory(null) as IRestLogger : null;
}
