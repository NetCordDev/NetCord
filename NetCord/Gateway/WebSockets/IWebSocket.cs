using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public interface IWebSocket : IDisposable
{
    public event Action? Connecting;
    public event Action? Connected;
    public event Action<WebSocketCloseStatus?, string?>? Disconnected;
    public event Action? Closed;
    public event Action<ReadOnlyMemory<byte>>? MessageReceived;

    /// <summary>
    /// Connects to a WebSocket server.
    /// </summary>
    public Task ConnectAsync(Uri uri);

    /// <summary>
    /// Closes the <see cref="IWebSocket"/>.
    /// </summary>
    public Task CloseAsync(WebSocketCloseStatus status);

    /// <summary>
    /// Sends a message.
    /// </summary>
    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
}
