namespace NetCord.Gateway.WebSockets;

public class WebSocketConnectionProvider : IWebSocketConnectionProvider
{
    public IWebSocketConnection CreateConnection()
    {
        return new WebSocketConnection();
    }
}
