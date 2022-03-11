using System.Net.WebSockets;

namespace NetCord.WebSockets;

public class WebSocket : IWebSocket, IDisposable
{
    private ClientWebSocket? _webSocket;
    private bool _closed;
    private Task? _readAsync;
    private bool _disposed;

    public event Action? Connecting;
    public event Action? Connected;
    public event IWebSocket.DisconnectedEventHandler? Disconnected;
    public event Action? Closed;
    public event IWebSocket.MessageReceivedEventHandler? MessageReceived;

    public bool IsConnected { get; private set; }

    /// <summary>
    /// Connects to a WebSocket server
    /// </summary>
    /// <returns></returns>
    public async Task ConnectAsync(Uri uri)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WebSocket));
        Connecting?.Invoke();
        _webSocket?.Dispose();
        await (_webSocket = new()).ConnectAsync(uri, default).ConfigureAwait(false);
        IsConnected = true;
        Connected?.Invoke();
        _closed = false;
        _readAsync = ReadAsync();
    }

    /// <summary>
    /// Closes the <see cref="WebSocket"/>
    /// </summary>
    public async Task CloseAsync()
    {
        ThrowIfInvalid();
        _closed = true;
        IsConnected = false;
        await _webSocket!.CloseAsync(WebSocketCloseStatus.Empty, null, default).ConfigureAwait(false);
        await _readAsync!.ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a message
    /// </summary>
    public Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default)
    {
        ThrowIfInvalid();
        return _webSocket!.SendAsync(buffer, WebSocketMessageType.Text, true, token).AsTask();
    }

    /// <summary>
    /// Sends a message
    /// </summary>
    public Task SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageFlags flags, CancellationToken token = default)
    {
        ThrowIfInvalid();
        return _webSocket!.SendAsync(buffer, WebSocketMessageType.Text, flags, token).AsTask();
    }

    private async Task ReadAsync()
    {
        var webSocket = _webSocket;
        try
        {
            Memory<byte> receiveBuffer = new(new byte[128]);
            using MemoryStream stream = new();
            while (true)
            {
                while (true)
                {
                    var r = await webSocket!.ReceiveAsync(receiveBuffer, default).ConfigureAwait(false);
                    if (r.EndOfMessage)
                    {
                        if (r.MessageType != WebSocketMessageType.Close)
                        {
                            stream.Write(receiveBuffer[..r.Count].Span);
                            MessageReceived?.Invoke(stream.ToArray());
                        }
                        break;
                    }
                    else
                        stream.Write(receiveBuffer.Span);
                }
                stream.SetLength(0);
            }
        }
        catch
        {
            IsConnected = false;
            if (_closed)
                Closed?.Invoke();
            else
                Disconnected?.Invoke(webSocket!.CloseStatus, webSocket!.CloseStatusDescription);
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
        if (_disposed)
            throw new ObjectDisposedException(nameof(WebSocket));
        else if (!IsConnected)
            throw new WebSocketException("WebSocket wasn't connected");
    }
}