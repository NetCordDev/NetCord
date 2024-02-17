using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public sealed class WebSocket : IWebSocket
{
    private const int DefaultBufferSize = 4096;

    private ClientWebSocket? _webSocket;
    private bool _closed;
    private Task? _readAsync;
    private bool _disposed;

    public event Action? Connecting;
    public event Action? Connected;
    public event Action<WebSocketCloseStatus?, string?>? Disconnected;
    public event Action? Closed;
    public event Action<ReadOnlyMemory<byte>>? MessageReceived;

    public bool IsConnected { get; private set; }

    public async Task ConnectAsync(Uri uri)
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(WebSocket));
        Connecting?.Invoke();
        _webSocket?.Dispose();
        await (_webSocket = new()).ConnectAsync(uri, default).ConfigureAwait(false);
        IsConnected = true;
        Connected?.Invoke();
        _closed = false;
        _readAsync = ReadAsync();
    }

    public async Task CloseAsync(WebSocketCloseStatus status)
    {
        ThrowIfInvalid();
        _closed = true;
        IsConnected = false;
        await _webSocket!.CloseOutputAsync(status, null, default).ConfigureAwait(false);
        await _readAsync!.ConfigureAwait(false);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfInvalid();
        return _webSocket!.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
    }

    private async Task ReadAsync()
    {
        var webSocket = _webSocket!;
        try
        {
            using RentedArrayBufferWriter<byte> writer = new(DefaultBufferSize);
            while (true)
            {
                var result = await webSocket.ReceiveAsync(writer.GetMemory(DefaultBufferSize), default).ConfigureAwait(false);
                if (result.EndOfMessage)
                {
                    if (result.MessageType != WebSocketMessageType.Close)
                    {
                        writer.Advance(result.Count);
                        MessageReceived?.Invoke(writer.WrittenMemory);
                        writer.Clear();
                    }
                }
                else
                    writer.Advance(result.Count);
            }
        }
        catch
        {
            IsConnected = false;
            try
            {
                if (_closed)
                    Closed?.Invoke();
                else
                    Disconnected?.Invoke(webSocket.CloseStatus, webSocket.CloseStatusDescription);
            }
            catch
            {
            }
        }
    }

    public void Dispose()
    {
        Connecting = null;
        Connected = null;
        Disconnected = null;
        Closed = null;
        MessageReceived = null;
        _webSocket?.Dispose();
        IsConnected = false;
        _disposed = true;
    }

    private void ThrowIfInvalid()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(WebSocket));
        if (!IsConnected)
            throw new WebSocketException("WebSocket was not connected.");
    }
}
