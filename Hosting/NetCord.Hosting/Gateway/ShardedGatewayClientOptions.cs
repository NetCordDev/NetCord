using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Hosting.Rest;
using NetCord.Logging;
using NetCord.Rest;

namespace NetCord.Hosting.Gateway;

public partial class ShardedGatewayClientOptions : IDiscordOptions
{
    [OptionsValidator]
    internal partial class Validator : IValidateOptions<ShardedGatewayClientOptions>
    {
    }

    [Required]
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    public bool? AutoStartStop { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.WebSocketConnectionProviderFactory" />
    public Func<Shard, IWebSocketConnectionProvider?>? WebSocketConnectionProviderFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.RateLimiterProviderFactory" />
    public Func<Shard, IRateLimiterProvider?>? RateLimiterProviderFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.DefaultMessagePropertiesFactory" />
    public Func<Shard, WebSocketMessageProperties?>? DefaultMessagePropertiesFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.ReconnectStrategyFactory" />
    public Func<Shard, IReconnectStrategy?>? ReconnectStrategyFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.LatencyTimerFactory" />
    public Func<Shard, ILatencyTimer?>? LatencyTimerFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.VersionFactory" />
    public Func<Shard, ApiVersion?>? VersionFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.CacheProviderFactory" />
    public Func<Shard, IGatewayClientCacheProvider?>? CacheProviderFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.CompressionFactory" />
    public Func<Shard, IGatewayCompression?>? CompressionFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.IntentsFactory" />
    public Func<Shard, GatewayIntents?>? IntentsFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.Hostname" />
    public string? Hostname { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.ConnectionPropertiesFactory" />
    public Func<Shard, ConnectionPropertiesProperties?>? ConnectionPropertiesFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.LargeThresholdFactory" />
    public Func<Shard, int?>? LargeThresholdFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.PresenceFactory" />
    public Func<Shard, PresenceProperties?>? PresenceFactory { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.MaxConcurrency" />
    public int? MaxConcurrency { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.ShardRange" />
    public Range? ShardRange { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.TotalShardCount" />
    public int? TotalShardCount { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.RestClientConfiguration" />
    public RestClientConfiguration? RestClientConfiguration { get; set; }

    // Simple properties

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.WebSocketConnectionProviderFactory" />
    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.RateLimiterProviderFactory" />
    public IRateLimiterProvider? RateLimiterProvider { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.DefaultMessagePropertiesFactory" />
    public WebSocketMessageProperties? DefaultMessageProperties { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.ReconnectStrategyFactory" />
    public IReconnectStrategy? ReconnectStrategy { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.LatencyTimerFactory" />
    public ILatencyTimer? LatencyTimer { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.VersionFactory" />
    public ApiVersion? Version { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.CacheProviderFactory" />
    public IGatewayClientCacheProvider? CacheProvider { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.CompressionFactory" />
    public IGatewayCompression? Compression { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.IntentsFactory" />
    public GatewayIntents? Intents { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.ConnectionPropertiesFactory" />
    public ConnectionPropertiesProperties? ConnectionProperties { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.LargeThresholdFactory" />
    public int? LargeThreshold { get; set; }

    /// <inheritdoc cref="ShardedGatewayClientConfiguration.PresenceFactory" />
    public PresenceProperties? Presence { get; set; }

    internal ShardedGatewayClientConfiguration CreateConfiguration(IServiceProvider services)
    {
        return ShardedGatewayClientConfigurationFactory.Create(CreateFactory(WebSocketConnectionProvider, WebSocketConnectionProviderFactory),
                                                               CreateFactory(RateLimiterProvider, RateLimiterProviderFactory),
                                                               CreateFactory(DefaultMessageProperties, DefaultMessagePropertiesFactory),
                                                               CreateFactory(ReconnectStrategy, ReconnectStrategyFactory),
                                                               CreateFactory(LatencyTimer, LatencyTimerFactory),
                                                               CreateFactory(Version, VersionFactory),
                                                               CreateFactory(CacheProvider, CacheProviderFactory),
                                                               CreateFactory(Compression, CompressionFactory),
                                                               CreateFactory(Intents, IntentsFactory),
                                                               Hostname,
                                                               CreateFactory(ConnectionProperties, ConnectionPropertiesFactory),
                                                               CreateFactory(LargeThreshold, LargeThresholdFactory),
                                                               CreateFactory(Presence, PresenceFactory),
                                                               MaxConcurrency,
                                                               ShardRange,
                                                               TotalShardCount,
                                                               RestClientConfiguration,
                                                               CreateLogger);

        static Func<Shard, T?>? CreateFactory<T>(T? value, Func<Shard, T?>? func, [CallerArgumentExpression(nameof(value))] string valueName = "", [CallerArgumentExpression(nameof(func))] string funcName = "")
        {
            if (value is not null)
            {
                if (func is not null)
                    throw new InvalidOperationException($"Cannot specify both '{valueName}' and '{funcName}' at the same time.");

                return _ => value;
            }

            return func;
        }

        IGatewayLogger? CreateLogger(Shard? shard)
        {
            if (shard.HasValue)
                return new ShardedGatewayMicrosoftExtensionsLogger(shard.GetValueOrDefault().Id, services);

            return new RestMicrosoftExtensionsLogger(services);
        }
    }
}
