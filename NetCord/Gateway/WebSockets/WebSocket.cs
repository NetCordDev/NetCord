using System.Buffers;
using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public class WebSocket : IWebSocket, IDisposable
{
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
    public async Task CloseAsync(WebSocketCloseStatus status)
    {
        ThrowIfInvalid();
        _closed = true;
        IsConnected = false;
        await _webSocket!.CloseAsync(status, null, default).ConfigureAwait(false);
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
        var owner = MemoryPool<byte>.Shared.Rent(4096);
        try
        {
            var buffer = owner.Memory;
            int position = 0;
            while (true)
            {
                var result = await webSocket.ReceiveAsync(buffer[position..], default).ConfigureAwait(false);
                if (result.EndOfMessage)
                {
                    if (result.MessageType != WebSocketMessageType.Close)
                    {
                        MessageReceived?.Invoke(buffer[..(position + result.Count)]);
                        position = 0;
                    }
                }
                else
                {
                    position += result.Count;

                    int length = buffer.Length;
                    if (length == position)
                    {
                        var newOwner = MemoryPool<byte>.Shared.Rent(length * 2);
                        var newBuffer = newOwner.Memory;
                        buffer.CopyTo(newBuffer);
                        owner.Dispose();
                        owner = newOwner;
                        buffer = newBuffer;
                    }
                }
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
        finally
        {
            owner.Dispose();
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
            throw new WebSocketException("WebSocket was not connected.");
    }
}
