namespace NetCord.Gateway.WebSockets;

public class WebSocketConnectionProvider : IWebSocketConnectionProvider
{
    public static WebSocketConnectionProvider Instance { get; } = new();

    private WebSocketConnectionProvider()
    {
    }

    public IWebSocketConnection CreateConnection()
    {
        return new WebSocketConnection();
    }
}
