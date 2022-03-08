using System.Net.WebSockets;
using System.Text;

namespace NetCord.WebSockets;

internal class WebSocket : IDisposable
{
    private readonly Uri _uri;
    private ClientWebSocket? _webSocket;
    private bool _closed;
    private Task? _readAsync;
    private bool _disposed;

    public event Action? Connecting;
    public event Action? Connected;
    public event DisconnectedEventHandler? Disconnected;
    public event Action? Closed;
    public event MessageReceivedEventHandler? MessageReceived;

    public delegate void DisconnectedEventHandler(WebSocketCloseStatus? closeStatus, string? closeStatusDescription);
    public delegate void MessageReceivedEventHandler(ReadOnlyMemory<byte> data);

    public bool IsConnected { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="WebSocket"/>
    /// </summary>
    /// <param name="uri"></param>
    public WebSocket(Uri uri)
    {
        _uri = uri;
    }

    /// <summary>
    /// Connect to a WebSocket server
    /// </summary>
    /// <returns></returns>
    public async Task ConnectAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WebSocket));
        Connecting?.Invoke();
        _webSocket?.Dispose();
        await (_webSocket = new()).ConnectAsync(_uri, default).ConfigureAwait(false);
        IsConnected = true;
        Connected?.Invoke();
        _closed = false;
        _readAsync = ReadAsync();
    }

    /// <summary>
    /// Close the <see cref="WebSocket"/>
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
    /// Send a message
    /// </summary>
    public Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default)
    {
        ThrowIfInvalid();
        return _webSocket!.SendAsync(buffer, WebSocketMessageType.Text, true, token).AsTask();
    }

    /// <summary>
    /// Send a message
    /// </summary>
    public Task SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageFlags flags, CancellationToken token = default)
    {
        ThrowIfInvalid();
        return _webSocket!.SendAsync(buffer, WebSocketMessageType.Text, flags, token).AsTask();
    }

    /// <summary>
    /// Send a message
    /// </summary>
    public Task SendAsync(string message, CancellationToken token = default)
        => SendAsync(Encoding.UTF8.GetBytes(message), token);

    /// <summary>
    /// Send a message
    /// </summary>
    public Task SendAsync(string message, WebSocketMessageFlags flags, CancellationToken token = default)
        => SendAsync(Encoding.UTF8.GetBytes(message), flags, token);

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