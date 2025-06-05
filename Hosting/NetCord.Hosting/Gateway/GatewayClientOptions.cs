using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Gateway.Compression;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Rest;

namespace NetCord.Hosting.Gateway;

public partial class GatewayClientOptions : IDiscordOptions
{
    [OptionsValidator]
    internal partial class Validator : IValidateOptions<GatewayClientOptions>
    {
    }

    [Required]
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.WebSocketConnectionProvider" />
    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.RateLimiterProvider" />
    public IRateLimiterProvider? RateLimiterProvider { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.DefaultPayloadProperties" />
    public WebSocketPayloadProperties? DefaultPayloadProperties { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.ReconnectStrategy" />
    public IReconnectStrategy? ReconnectStrategy { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.LatencyTimer" />
    public ILatencyTimer? LatencyTimer { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.Version" />
    public ApiVersion? Version { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.Cache" />
    public IGatewayClientCache? Cache { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.Compression" />
    public IGatewayCompression? Compression { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.Intents" />
    public GatewayIntents? Intents { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.Hostname" />
    public string? Hostname { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.ConnectionProperties" />
    public ConnectionPropertiesProperties? ConnectionProperties { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.LargeThreshold" />
    public int? LargeThreshold { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.Presence" />
    public PresenceProperties? Presence { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.Shard" />
    public Shard? Shard { get; set; }

    /// <inheritdoc cref="GatewayClientConfiguration.RestClientConfiguration" />
    public RestClientConfiguration? RestClientConfiguration { get; set; }

    internal GatewayClientConfiguration CreateConfiguration(IServiceProvider services)
    {
        return GatewayClientConfigurationFactory.Create(WebSocketConnectionProvider,
                                                        RateLimiterProvider,
                                                        DefaultPayloadProperties,
                                                        ReconnectStrategy,
                                                        LatencyTimer,
                                                        Version,
                                                        Cache,
                                                        Compression,
                                                        Intents,
                                                        Hostname,
                                                        ConnectionProperties,
                                                        LargeThreshold,
                                                        Presence,
                                                        Shard,
                                                        RestClientConfiguration,
                                                        new GatewayMicrosoftExtensionsLogger(services));
    }
}
