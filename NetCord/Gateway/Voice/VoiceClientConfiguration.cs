using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectTimers;
using NetCord.Gateway.Voice.Encryption;
using NetCord.Gateway.Voice.UdpSockets;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway.Voice;

public class VoiceClientConfiguration
{
    public IWebSocket? WebSocket { get; init; }
    public IUdpSocket? UdpSocket { get; init; }
    public IReconnectTimer? ReconnectTimer { get; init; }
    public ILatencyTimer? LatencyTimer { get; init; }
    public VoiceApiVersion Version { get; init; } = VoiceApiVersion.V4;
    public IVoiceClientCache? Cache { get; init; }
    public IVoiceEncryption? Encryption { get; init; }
    public bool RedirectInputStreams { get; init; }
}
