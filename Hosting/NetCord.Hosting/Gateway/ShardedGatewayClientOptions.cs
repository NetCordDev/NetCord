using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
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

    public Func<Shard, IWebSocketConnectionProvider?>? WebSocketConnectionProviderFactory { get; set; }

    public Func<Shard, IRateLimiterProvider?>? RateLimiterProviderFactory { get; set; }

    public Func<Shard, WebSocketPayloadProperties?>? DefaultPayloadPropertiesFactory { get; set; }

    public Func<Shard, IReconnectStrategy?>? ReconnectStrategyFactory { get; set; }

    public Func<Shard, ILatencyTimer?>? LatencyTimerFactory { get; set; }

    public Func<Shard, ApiVersion?>? VersionFactory { get; set; }

    public Func<Shard, IGatewayClientCache?>? CacheFactory { get; set; }

    public Func<Shard, IGatewayCompression?>? CompressionFactory { get; set; }

    public Func<Shard, GatewayIntents?>? IntentsFactory { get; set; }

    public string? Hostname { get; set; }

    public Func<Shard, ConnectionPropertiesProperties?>? ConnectionPropertiesFactory { get; set; }

    public Func<Shard, int?>? LargeThresholdFactory { get; set; }

    public Func<Shard, PresenceProperties?>? PresenceFactory { get; set; }

    public int? ShardCount { get; set; }

    public RestClientConfiguration? RestClientConfiguration { get; set; }

    // Simple properties

    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; set; }

    public IRateLimiterProvider? RateLimiterProvider { get; set; }

    public WebSocketPayloadProperties? DefaultPayloadProperties { get; set; }

    public IReconnectStrategy? ReconnectStrategy { get; set; }

    public ILatencyTimer? LatencyTimer { get; set; }

    public ApiVersion? Version { get; set; }

    public IGatewayClientCache? Cache { get; set; }

    public IGatewayCompression? Compression { get; set; }

    public GatewayIntents? Intents { get; set; }

    public ConnectionPropertiesProperties? ConnectionProperties { get; set; }

    public int? LargeThreshold { get; set; }

    public PresenceProperties? Presence { get; set; }

    internal ShardedGatewayClientConfiguration CreateConfiguration()
    {
        return ShardedGatewayClientConfigurationFactory.Create(CreateFactory(WebSocketConnectionProvider, WebSocketConnectionProviderFactory),
                                                               CreateFactory(RateLimiterProvider, RateLimiterProviderFactory),
                                                               CreateFactory(DefaultPayloadProperties, DefaultPayloadPropertiesFactory),
                                                               CreateFactory(ReconnectStrategy, ReconnectStrategyFactory),
                                                               CreateFactory(LatencyTimer, LatencyTimerFactory),
                                                               CreateFactory(Version, VersionFactory),
                                                               CreateFactory(Cache, CacheFactory),
                                                               CreateFactory(Compression, CompressionFactory),
                                                               CreateFactory(Intents, IntentsFactory),
                                                               Hostname,
                                                               CreateFactory(ConnectionProperties, ConnectionPropertiesFactory),
                                                               CreateFactory(LargeThreshold, LargeThresholdFactory),
                                                               CreateFactory(Presence, PresenceFactory),
                                                               ShardCount,
                                                               RestClientConfiguration);

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
    }
}
