namespace NetCord.Gateway.WebSockets;

public interface IWebSocketConnection : IDisposable
{
    public int? CloseStatus { get; }

    public string? CloseStatusDescription { get; }

    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, WebSocketMessageFlags messageFlags, CancellationToken cancellationToken = default);

    public ValueTask<WebSocketConnectionReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

    public ValueTask CloseAsync(int closeStatus, string? closeStatusDescription, CancellationToken cancellationToken = default);

    public void Abort();
}
