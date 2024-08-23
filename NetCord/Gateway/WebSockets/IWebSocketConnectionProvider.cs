namespace NetCord.Gateway.WebSockets;

public interface IWebSocketConnectionProvider
{
    public ValueTask<IWebSocketConnection> CreateWebSocketConnectionAsync(Uri uri, CancellationToken cancellationToken = default);
}
