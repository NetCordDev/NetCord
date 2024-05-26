using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace NetCord.Gateway.WebSockets;

public sealed class WebSocket : IWebSocket
{
    private const int DefaultBufferSize = 8192;

    private State? _state;
    private bool _disposed;

    public event Action? Connecting;
    public event Action? Connected;
    public event Action<WebSocketCloseStatus?, string?>? Disconnected;
    public event Action? Closed;
    public event Action<ReadOnlyMemory<byte>>? MessageReceived;

    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(WebSocket));

        State newState = new();
        var state = Interlocked.CompareExchange(ref _state, newState, null);
        if (state is not null)
        {
            newState.Dispose();
            ThrowAlreadyConnectingOrConnected();
        }

        InvokeEvent(Connecting);

        try
        {
            await newState.WebSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            Interlocked.Exchange(ref _state, null)?.Dispose();
            throw;
        }

        InvokeEvent(Connected);

        newState.StartReading(ReadAsync);
    }

    public async Task CloseAsync(WebSocketCloseStatus status, string? statusDescription, CancellationToken cancellationToken = default)
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is null || !state.TryIndicateDisconnecting())
            ThrowNotConnected();

        var webSocket = state.WebSocket;

        try
        {
            await webSocket.CloseOutputAsync(status, statusDescription, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            webSocket.Abort();
            InvokeEvent(Closed);
            throw;
        }

        await state.ReadTask.ConfigureAwait(false);

        InvokeEvent(Closed);
    }

    public void Abort()
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is null)
            return;

        var disconnecting = state.TryIndicateDisconnecting();

        state.WebSocket.Abort();

        if (disconnecting)
            InvokeEvent(Closed);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var state = _state;

        if (state is null)
            ThrowNotConnected();

        return state.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
    }

    private async Task ReadAsync(State state)
    {
        var webSocket = state.WebSocket;
        try
        {
            using RentedArrayBufferWriter<byte> writer = new(DefaultBufferSize);
            while (true)
            {
                var result = await webSocket.ReceiveAsync(writer.GetMemory(), default).ConfigureAwait(false);

                if (result.EndOfMessage)
                {
                    if (result.MessageType is WebSocketMessageType.Close)
                        break;

                    writer.Advance(result.Count);
                    InvokeEvent(MessageReceived, writer.WrittenMemory);
                    writer.Clear();
                }
                else
                    writer.Advance(result.Count);
            }
        }
        catch
        {
        }

        if (state.TryIndicateDisconnecting())
        {
            _state = null;
            state.Dispose();
            InvokeEvent(Disconnected, webSocket.CloseStatus, webSocket.CloseStatusDescription);
        }
    }

    private static void InvokeEvent(Action? action)
    {
        if (action is not null)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }
    }

    private static void InvokeEvent(Action<WebSocketCloseStatus?, string?>? action, WebSocketCloseStatus? status, string? description)
    {
        if (action is not null)
        {
            try
            {
                action(status, description);
            }
            catch
            {
            }
        }
    }

    private static void InvokeEvent(Action<ReadOnlyMemory<byte>>? action, ReadOnlyMemory<byte> buffer)
    {
        if (action is not null)
        {
            try
            {
                action(buffer);
            }
            catch
            {
            }
        }
    }

    public void Dispose()
    {
        _state?.Dispose();
        _disposed = true;
    }

    [DoesNotReturn]
    private static void ThrowAlreadyConnectingOrConnected()
    {
        throw new InvalidOperationException("The WebSocket is already connecting or connected.");
    }

    [DoesNotReturn]
    private static void ThrowNotConnected()
    {
        throw new InvalidOperationException("The WebSocket is not connected.");
    }

    private sealed class State : IDisposable
    {
        public ClientWebSocket WebSocket { get; } = new();

        public Task ReadTask => _readCompletionSource.Task;

        public TaskCompletionSource _readCompletionSource = new();

        private int _state;

        public async void StartReading(Func<State, Task> readAsync)
        {
            await readAsync(this).ConfigureAwait(false);

            _readCompletionSource.TrySetResult();
        }

        public bool TryIndicateDisconnecting()
        {
            return Interlocked.Exchange(ref _state, 1) is 0;
        }

        public void Dispose()
        {
            WebSocket.Dispose();
        }
    }
}
