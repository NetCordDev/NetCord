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

    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; set; }

    public IRateLimiterProvider? RateLimiterProvider { get; set; }

    public WebSocketPayloadProperties? DefaultPayloadProperties { get; set; }

    public IReconnectStrategy? ReconnectStrategy { get; set; }

    public ILatencyTimer? LatencyTimer { get; set; }

    public ApiVersion? Version { get; set; }

    public IGatewayClientCache? Cache { get; set; }

    public IGatewayCompression? Compression { get; set; }

    public GatewayIntents? Intents { get; set; }

    public string? Hostname { get; set; }

    public ConnectionPropertiesProperties? ConnectionProperties { get; set; }

    public int? LargeThreshold { get; set; }

    public PresenceProperties? Presence { get; set; }

    public Shard? Shard { get; set; }

    public RestClientConfiguration? RestClientConfiguration { get; set; }

    internal GatewayClientConfiguration CreateConfiguration()
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
                                                        RestClientConfiguration);
    }
}
