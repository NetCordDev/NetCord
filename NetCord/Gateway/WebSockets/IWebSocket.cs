using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public interface IWebSocket : IDisposable
{
    public event Action? Connecting;
    public event Action? Connected;
    public event DisconnectedEventHandler? Disconnected;
    public event Action? Closed;
    public event MessageReceivedEventHandler? MessageReceived;

    public delegate void DisconnectedEventHandler(WebSocketCloseStatus? closeStatus, string? closeStatusDescription);
    public delegate void MessageReceivedEventHandler(ReadOnlyMemory<byte> data);

    public Task ConnectAsync(Uri uri);
    public Task CloseAsync();
    public Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default);
}