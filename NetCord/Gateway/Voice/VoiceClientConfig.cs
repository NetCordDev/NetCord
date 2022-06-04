using NetCord.Gateway.Voice.UdpSockets;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway.Voice;

public class VoiceClientConfig
{
    public IWebSocket? WebSocket { get; init; }

    public bool RedirectInputStreams { get; init; }

    public IUdpSocket? UdpClient { get; init; }
}