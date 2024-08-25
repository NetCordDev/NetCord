using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

internal sealed class WebSocketConnection : IWebSocketConnection
{
    private readonly ClientWebSocket _webSocket = new();

    public int? CloseStatus => (int?)_webSocket.CloseStatus;

    public string? CloseStatusDescription => _webSocket.CloseStatusDescription;

    public ValueTask OpenAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        return new(_webSocket.ConnectAsync(uri, cancellationToken));
    }

    public void Abort()
    {
        _webSocket.Abort();
    }

    public ValueTask CloseAsync(int closeStatus, string? closeStatusDescription, CancellationToken cancellationToken = default)
    {
        return new(_webSocket.CloseOutputAsync((WebSocketCloseStatus)closeStatus, closeStatusDescription, cancellationToken));
    }

    public async ValueTask<WebSocketConnectionReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var result = await _webSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
        return new(result.Count, (WebSocketMessageType)result.MessageType, result.EndOfMessage);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, WebSocketMessageFlags messageFlags, CancellationToken cancellationToken = default)
    {
        return _webSocket.SendAsync(buffer, (System.Net.WebSockets.WebSocketMessageType)messageType, (System.Net.WebSockets.WebSocketMessageFlags)messageFlags, cancellationToken);
    }

    public void Dispose()
    {
        _webSocket.Dispose();
    }
}
