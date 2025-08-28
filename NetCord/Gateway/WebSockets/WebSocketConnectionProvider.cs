namespace NetCord.Gateway.WebSockets;

public class WebSocketConnectionProvider(WebSocketConnectionProviderConfiguration? configuration = null) : IWebSocketConnectionProvider
{
    public IWebSocketConnection CreateConnection()
    {
        return new WebSocketConnection(configuration?.ConfigureConnectionOptions);
    }
}
