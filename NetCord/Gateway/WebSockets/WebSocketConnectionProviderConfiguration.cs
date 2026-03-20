using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public class WebSocketConnectionProviderConfiguration
{
    /// <summary>
    /// A delegate to configure the <see cref="ClientWebSocketOptions"/> of the connection.
    /// </summary>
    public Action<ClientWebSocketOptions>? ConfigureConnectionOptions { get; init; }
}
