using System.Net.WebSockets;
using System.IO.Pipelines;

namespace NetCord.Gateway.WebSockets;

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
    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default)
    {
        ThrowIfInvalid();
        return _webSocket!.SendAsync(buffer, WebSocketMessageType.Text, true, token);
    }

    private async Task ReadAsync()
    {
        var webSocket = _webSocket!;
        try
        {
            Pipe pipe = new();
            while (true)
            {
                var writeBuffer = pipe.Writer.GetMemory(128);
                var r = await webSocket.ReceiveAsync(writeBuffer, default).ConfigureAwait(false);
                if (r.EndOfMessage)
                {
                    if (r.MessageType != WebSocketMessageType.Close)
                    {
                        pipe.Writer.Advance(r.Count);
                        await pipe.Writer.FlushAsync().ConfigureAwait(false);
                        var readResult = await pipe.Reader.ReadAsync().ConfigureAwait(false);
                        var buffer = readResult.Buffer;

                        MessageReceived?.Invoke(buffer);
                        pipe.Reader.AdvanceTo(buffer.End);
                    }
                }
                else
                    pipe.Writer.Advance(r.Count);
            }
        }
        catch
        {
            IsConnected = false;
            if (_closed)
                Closed?.Invoke();
            else
                Disconnected?.Invoke(webSocket.CloseStatus, webSocket.CloseStatusDescription);
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
            throw new WebSocketException("WebSocket wasn't connected.");
    }
}
