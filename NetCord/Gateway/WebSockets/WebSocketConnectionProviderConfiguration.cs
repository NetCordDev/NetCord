using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public class WebSocketConnectionProviderConfiguration
{
    public Action<ClientWebSocketOptions>? ConfigureConnectionOptions { get; init; }
}
