using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.Voice.UdpSockets;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;

namespace NetCord.Gateway.Voice;

public class VoiceClientConfiguration : IWebSocketClientConfiguration
{
    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; init; }
    public IRateLimiterProvider? RateLimiterProvider { get; init; }
    public WebSocketPayloadProperties? DefaultPayloadProperties { get; init; }
    public IUdpConnectionProvider? UdpConnectionProvider { get; init; }
    public IReconnectStrategy? ReconnectStrategy { get; init; }
    public ILatencyTimer? LatencyTimer { get; init; }
    public VoiceApiVersion? Version { get; init; }
    public IVoiceClientCacheProvider? CacheProvider { get; init; }
    public IVoiceEncryptionProvider? EncryptionProvider { get; init; }
    public IVoiceLogger? Logger { get; init; }
    public bool? ReceiveVoice { get; init; }
    public TimeSpan? ExternalSocketAddressDiscoveryTimeout { get; init; }

    IWebSocketLogger? IWebSocketClientConfiguration.Logger => Logger switch
    {
        null or NullLogger => NullLogger.Instance,
        _ => new VoiceWebSocketLogger(Logger)
    };
}
