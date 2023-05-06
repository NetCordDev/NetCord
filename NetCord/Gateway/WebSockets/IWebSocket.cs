using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public interface IWebSocket : IDisposable
{
    public event Action? Connecting;
    public event Action? Connected;
    public event Action<WebSocketCloseStatus?, string?>? Disconnected;
    public event Action? Closed;
    public event Action<ReadOnlyMemory<byte>>? MessageReceived;

    public Task ConnectAsync(Uri uri);
    public Task CloseAsync(WebSocketCloseStatus status);
    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
}
