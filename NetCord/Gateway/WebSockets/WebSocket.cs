using System.Buffers;
using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public class WebSocket : IWebSocket
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
            using ByteBufferWriter writer = new(4096);
            while (true)
            {
                var result = await webSocket.ReceiveAsync(writer.GetMemory(4096), default).ConfigureAwait(false);
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
        if (_disposed)
            throw new ObjectDisposedException(nameof(WebSocket));
        else if (!IsConnected)
            throw new WebSocketException("WebSocket was not connected.");
    }

    private struct ByteBufferWriter : IBufferWriter<byte>, IDisposable
    {
        private int _index;
        private byte[] _buffer;

        public ByteBufferWriter()
        {
            _buffer = Array.Empty<byte>();
        }

        public ByteBufferWriter(int minimumInitialCapacity)
        {
            _buffer = ArrayPool<byte>.Shared.Rent(minimumInitialCapacity);
        }

        public readonly ReadOnlyMemory<byte> WrittenMemory => _buffer.AsMemory(0, _index);

        public void Advance(int count)
        {
            _index += count;
        }

        public void Clear()
        {
            _index = 0;
        }

        public Memory<byte> GetMemory(int sizeHint = 1)
        {
            ResizeBuffer(sizeHint);
            return _buffer.AsMemory(_index);
        }

        public Span<byte> GetSpan(int sizeHint = 1)
        {
            ResizeBuffer(sizeHint);
            return _buffer.AsSpan(_index);
        }

#pragma warning disable IDE0251 // Make member 'readonly'
        public void Dispose()
#pragma warning restore IDE0251 // Make member 'readonly'
        {
            ArrayPool<byte>.Shared.Return(_buffer);
        }

        private void ResizeBuffer(int sizeHint)
        {
            var buffer = _buffer;
            var index = _index;
            var sum = index + sizeHint;
            if (buffer.Length < sum)
            {
                var pool = ArrayPool<byte>.Shared;
                var newBuffer = pool.Rent(sum);
                Array.Copy(buffer, newBuffer, index);
                pool.Return(buffer);
                _buffer = newBuffer;
            }
        }
    }
}
