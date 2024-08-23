namespace NetCord.Gateway.WebSockets;

public class WebSocketConnectionProvider : IWebSocketConnectionProvider
{
    public ValueTask<IWebSocketConnection> CreateWebSocketConnectionAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        return WebSocketConnection.CreateAsync(uri, cancellationToken);
    }
}
