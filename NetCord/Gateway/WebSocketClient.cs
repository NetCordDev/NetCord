using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway;

public abstract class WebSocketClient : IDisposable
{
    private protected record ConnectionStateResult
    {
        public record Success(ConnectionState ConnectionState) : ConnectionStateResult;

        public record Retry : ConnectionStateResult
        {
            private Retry()
            {
            }

            public static Retry Instance { get; } = new();
        }
    }

    private protected sealed class ConnectionState(IWebSocketConnection connection, IRateLimiter rateLimiter) : IDisposable
    {
        public IWebSocketConnection Connection => connection;

        public IRateLimiter RateLimiter => rateLimiter;

        public CancellationTokenProvider DisconnectedTokenProvider { get; } = new();

        private int _state;

        public bool TryIndicateDisconnecting()
        {
            var disconnecting = Interlocked.Exchange(ref _state, 1) is 0;

            if (disconnecting)
                DisconnectedTokenProvider.Cancel();

            return disconnecting;
        }

        public void Dispose()
        {
            DisconnectedTokenProvider.Dispose();
            RateLimiter.Dispose();
            Connection.Dispose();
        }
    }

    private protected sealed class State : IDisposable
    {
        private ConnectionState? _connectionState;

        public CancellationTokenProvider ClosedTokenProvider { get; } = new();

        public Task<ConnectionStateResult> ReadyTask => _readyCompletionSource.Task;

        private TaskCompletionSource<ConnectionStateResult> _readyCompletionSource = new();

        public void IndicateReady(ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                if (_connectionState != connectionState)
                    return;

                _readyCompletionSource.TrySetResult(new ConnectionStateResult.Success(connectionState));
            }
        }

        public ConnectingResult TryIndicateConnecting(ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                if (ClosedTokenProvider.IsCancellationRequested)
                    return ConnectingResult.Closed;

                if (_connectionState is not null)
                    return ConnectingResult.AlreadyStarted;

                _connectionState = connectionState;
            }

            return ConnectingResult.Success;
        }

        public enum ConnectingResult : byte
        {
            Success,
            AlreadyStarted,
            Closed,
        }

        public void IndicateConnectingFailed(ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                _ = connectionState.TryIndicateDisconnecting();

                if (_connectionState != connectionState)
                    return;

                _connectionState = null;
            }
        }

        public bool TryIndicateClosing([MaybeNullWhen(false)] out ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                ClosedTokenProvider.Cancel();

                _readyCompletionSource.TrySetCanceled();

                var previousState = _connectionState;
                if (previousState is null || !previousState.TryIndicateDisconnecting())
                {
                    connectionState = null;
                    return false;
                }

                _connectionState = null;
                connectionState = previousState;
            }

            return true;
        }

        public bool TryIndicateDisconnecting(ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                if (_connectionState != connectionState || !connectionState.TryIndicateDisconnecting())
                    return false;

                _connectionState = null;

                _readyCompletionSource.TrySetResult(ConnectionStateResult.Retry.Instance);
                _readyCompletionSource = new();
            }

            return true;
        }

        public void Dispose()
        {
            _readyCompletionSource.TrySetCanceled();
            _connectionState?.Dispose();
            ClosedTokenProvider.Dispose();
        }
    }

    private const int DefaultBufferSize = 8192;

    private protected WebSocketClient(IWebSocketClientConfiguration configuration)
    {
        _connectionProvider = configuration.WebSocketConnectionProvider ?? new WebSocketConnectionProvider();
        _reconnectStrategy = configuration.ReconnectStrategy ?? new ReconnectStrategy();
        _latencyTimer = configuration.LatencyTimer ?? new LatencyTimer();
        _rateLimiterProvider = configuration.RateLimiterProvider ?? NullRateLimiterProvider.Instance;
        _defaultPayloadProperties = configuration.DefaultPayloadProperties is { } defaultPayloadProperties ? defaultPayloadProperties with { } : new();
    }

    private protected static readonly WebSocketPayloadProperties _internalPayloadProperties = new()
    {
        MessageFlags = WebSocketMessageFlags.EndOfMessage,
        RetryHandling = WebSocketRetryHandling.RetryRateLimit,
    };

    private readonly object _eventsLock = new();
    private readonly IWebSocketConnectionProvider _connectionProvider;
    private readonly IReconnectStrategy _reconnectStrategy;
    private readonly IRateLimiterProvider _rateLimiterProvider;
    private readonly WebSocketPayloadProperties _defaultPayloadProperties;

    private protected readonly ILatencyTimer _latencyTimer;

    private State? _state;

    private protected abstract Uri Uri { get; }

    public TimeSpan Latency
    {
        get
        {
            var latency = Interlocked.Read(ref Unsafe.As<TimeSpan, long>(ref _latency));
            return Unsafe.As<long, TimeSpan>(ref latency);
        }
    }

    private TimeSpan _latency;

    public event Func<TimeSpan, ValueTask>? LatencyUpdate;
    public event Func<ValueTask>? Resume;
    public event Func<ValueTask>? Connecting;
    public event Func<ValueTask>? Connect;
    public event Func<bool, ValueTask>? Disconnect;
    public event Func<ValueTask>? Close;
    public event Func<LogMessage, ValueTask>? Log;

    private async void HandleConnecting()
    {
        InvokeLog(LogMessage.Info("Connecting"));
        await InvokeEventAsync(Connecting).ConfigureAwait(false);
    }

    private async void HandleConnected()
    {
        OnConnected();
        InvokeLog(LogMessage.Info("Connected"));
        await InvokeEventAsync(Connect).ConfigureAwait(false);
    }

    private async void HandleDisconnected(State state, ConnectionState connectionState)
    {
        var connection = connectionState.Connection;

        var description = connection.CloseStatusDescription;
        InvokeLog(LogMessage.Info("Disconnected", string.IsNullOrEmpty(description) ? null : (description.EndsWith('.') ? description[..^1] : description)));

        var reconnect = Reconnect((WebSocketCloseStatus?)connection.CloseStatus, description);

        var disconnectTask = InvokeEventAsync(Disconnect, reconnect);

        if (reconnect)
        {
            connectionState.Dispose();
            await ReconnectAsync(state).ConfigureAwait(false);
        }
        else
        {
            Interlocked.CompareExchange(ref _state, null, state);
            connectionState.Dispose();
            state.Dispose();
        }

        await disconnectTask.ConfigureAwait(false);
    }

    private async void HandleClosed()
    {
        InvokeLog(LogMessage.Info("Closed"));
        await InvokeEventAsync(Close).ConfigureAwait(false);
    }

    private async void HandleMessageReceived(State state, ConnectionState connectionState, ReadOnlyMemory<byte> data)
    {
        try
        {
            await ProcessPayloadAsync(state, connectionState, data.Span).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
            await AbortAndReconnectAsync(state, connectionState).ConfigureAwait(false);
        }
    }

    private protected async Task<ConnectionState> StartAsync(CancellationToken cancellationToken = default)
    {
        State state = new();
        if (Interlocked.CompareExchange(ref _state, state, null) is not null)
        {
            state.Dispose();
            ThrowConnectionAlreadyStarted();
        }

        ConnectionState connectionState;
        try
        {
            connectionState = await ConnectAsync(state, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            Interlocked.CompareExchange(ref _state, null, state);
            state.Dispose();
            throw;
        }

        return connectionState;
    }

    private protected async Task<ConnectionState> ConnectAsync(State state, CancellationToken cancellationToken = default)
    {
        var connection = _connectionProvider.CreateConnection();
        var rateLimiter = _rateLimiterProvider.CreateRateLimiter();
        ConnectionState connectionState = new(connection, rateLimiter);

        switch (state.TryIndicateConnecting(connectionState))
        {
            case State.ConnectingResult.Success:
                break;
            case State.ConnectingResult.AlreadyStarted:
                connectionState.Dispose();
                ThrowConnectionAlreadyStarted();
                break;
            case State.ConnectingResult.Closed:
                connectionState.Dispose();
                ThrowConnectionNotStarted();
                break;
        }

        try
        {
            HandleConnecting();
            await connection.OpenAsync(Uri, cancellationToken).ConfigureAwait(false);
            HandleConnected();
        }
        catch (Exception)
        {
            state.IndicateConnectingFailed(connectionState);
            connectionState.Dispose();
            throw;
        }

        _ = ReadAsync(state, connectionState);

        return connectionState;
    }

    /// <summary>
    /// Closes the <see cref="WebSocketClient"/>.
    /// </summary>
    /// <param name="status">The status to close with.</param>
    /// <param name="statusDescription">The status description to close with.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? statusDescription = null, CancellationToken cancellationToken = default)
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is null)
            ThrowConnectionNotStarted();

        if (!state.TryIndicateClosing(out var connectionState))
            return;

        var connection = connectionState.Connection;
        try
        {
            await connection.CloseAsync((int)status, statusDescription, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            InvokeLog(LogMessage.Error(ex));
            try
            {
                connection.Abort();
            }
            catch (Exception abortEx)
            {
                InvokeLog(LogMessage.Error(abortEx));
            }
        }

        connectionState.Dispose();
        state.Dispose();
        HandleClosed();
    }

    private async Task ReadAsync(State state, ConnectionState connectionState)
    {
        var connection = connectionState.Connection;
        try
        {
            using RentedArrayBufferWriter<byte> writer = new(DefaultBufferSize);
            while (true)
            {
                var result = await connection.ReceiveAsync(writer.GetMemory()).ConfigureAwait(false);

                if (result.EndOfMessage)
                {
                    if (result.MessageType is WebSocketMessageType.Close)
                        break;

                    writer.Advance(result.Count);
                    HandleMessageReceived(state, connectionState, writer.WrittenMemory);
                    writer.Clear();
                }
                else
                    writer.Advance(result.Count);
            }
        }
        catch
        {
        }

        if (state.TryIndicateDisconnecting(connectionState))
            HandleDisconnected(state, connectionState);
    }

    public void Abort()
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is null)
            return;

        if (!state.TryIndicateClosing(out var connectionState))
            return;

        try
        {
            connectionState.Connection.Abort();
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
        }

        connectionState.Dispose();
        state.Dispose();
        HandleClosed();
    }

    private protected virtual void OnConnected()
    {
    }

    private protected ValueTask AbortAndReconnectAsync(State state, ConnectionState connectionState)
    {
        if (!state.TryIndicateDisconnecting(connectionState))
            return default;

        try
        {
            connectionState.Connection.Abort();
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
        }

        connectionState.Dispose();
        HandleClosed();

        return ReconnectAsync(state);
    }

    public async ValueTask SendPayloadAsync(ReadOnlyMemory<byte> buffer, WebSocketPayloadProperties? properties = null, CancellationToken cancellationToken = default)
    {
        properties ??= _defaultPayloadProperties;

        while (true)
        {
            var state = _state;

            if (state is null)
                ThrowConnectionNotStarted();

            var task = state.ReadyTask;

            ConnectionState connectionState;
            if (task.IsCompleted)
            {
                if (task.Result is ConnectionStateResult.Success successResult)
                    connectionState = successResult.ConnectionState;
                else
                {
                    ThrowConnectionNotStarted();
                    return;
                }
            }
            else
            {
                if (properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
                {
                    var result = await task.WaitAsync(cancellationToken).ConfigureAwait(false);
                    if (result is ConnectionStateResult.Success successResult)
                        connectionState = successResult.ConnectionState;
                    else
                        continue;
                }
                else
                {
                    ThrowConnectionNotStarted();
                    return;
                }
            }

            var exception = await TrySendConnectionPayloadAsync(connectionState, buffer, properties, cancellationToken).ConfigureAwait(false);

            if (exception is null)
                return;

            if (!properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
                ThrowConnectionNotStarted();
        }
    }

    private protected static async ValueTask SendConnectionPayloadAsync(ConnectionState connectionState, ReadOnlyMemory<byte> buffer, WebSocketPayloadProperties properties, CancellationToken cancellationToken = default)
    {
        var exception = await TrySendConnectionPayloadAsync(connectionState, buffer, properties, cancellationToken).ConfigureAwait(false);
        if (exception is null)
            return;

        ThrowConnectionNotStarted(exception);
    }

    private static async ValueTask<Exception?> TrySendConnectionPayloadAsync(ConnectionState connectionState, ReadOnlyMemory<byte> buffer, WebSocketPayloadProperties properties, CancellationToken cancellationToken = default)
    {
        var rateLimiter = connectionState.RateLimiter;

        var disconnectedToken = connectionState.DisconnectedTokenProvider.Token;

        using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(disconnectedToken, cancellationToken);
        var linkedToken = linkedTokenSource.Token;

        while (true)
        {
            RateLimitAcquisitionResult result;
            try
            {
                result = await rateLimiter.TryAcquireAsync(linkedToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return ex;
            }

            if (result.RateLimited)
            {
                if (properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryRateLimit))
                {
                    try
                    {
                        await Task.Delay(result.ResetAfter, linkedToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException ex)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        return ex;
                    }

                    continue;
                }

                ThrowRateLimitTriggered(result.ResetAfter);
            }

            var timestamp = Environment.TickCount64;

            try
            {
                await connectionState.Connection.SendAsync(buffer, properties.MessageType, properties.MessageFlags, linkedToken).ConfigureAwait(false);
            }
            catch (ArgumentException)
            {
                try
                {
                    await rateLimiter.CancelAcquireAsync(timestamp, default).ConfigureAwait(false);
                }
                catch
                {
                }

                throw;
            }
            catch (Exception ex)
            {
                try
                {
                    await rateLimiter.CancelAcquireAsync(timestamp, default).ConfigureAwait(false);
                }
                catch
                {
                }

                cancellationToken.ThrowIfCancellationRequested();

                return ex;
            }

            return null;
        }
    }

    private protected abstract bool Reconnect(WebSocketCloseStatus? status, string? description);

    private protected async ValueTask ReconnectAsync(State state)
    {
        var cancellationToken = state.ClosedTokenProvider.Token;

        foreach (var delay in _reconnectStrategy.GetDelays())
        {
            try
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                return;
            }

            ConnectionState connectionState;
            try
            {
                connectionState = await ConnectAsync(state, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
                continue;
            }

            try
            {
                await TryResumeAsync(connectionState, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }

            return;
        }
    }

    private protected abstract ValueTask TryResumeAsync(ConnectionState connectionState, CancellationToken cancellationToken = default);

    private protected async void StartHeartbeating(ConnectionState connectionState, double interval)
    {
        var cancellationToken = connectionState.DisconnectedTokenProvider.Token;

        PeriodicTimer timer;

        try
        {
            timer = new(TimeSpan.FromMilliseconds(interval));
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
            return;
        }

        using (timer)
        {
            while (true)
            {
                try
                {
                    await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
                    await HeartbeatAsync(connectionState, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    return;
                }
            }
        }
    }

    private protected abstract ValueTask HeartbeatAsync(ConnectionState connectionState, CancellationToken cancellationToken = default);

    private protected abstract Task ProcessPayloadAsync(State state, ConnectionState connectionState, ReadOnlySpan<byte> payload);

    private protected async void InvokeLog(LogMessage logMessage)
    {
        var log = Log;
        if (log is not null)
        {
            try
            {
                ValueTask task;
                lock (_eventsLock)
                    task = log(logMessage);
                await task.ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    private async void InvokeLogWithoutLock(LogMessage logMessage)
    {
        var log = Log;
        if (log is not null)
        {
            try
            {
                await log(logMessage).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    private protected ValueTask UpdateLatencyAsync(TimeSpan latency)
        => InvokeEventAsync(LatencyUpdate, latency, latency => Interlocked.Exchange(ref Unsafe.As<TimeSpan, long>(ref _latency), Unsafe.As<TimeSpan, long>(ref latency)));

    private protected ValueTask InvokeResumeEventAsync()
        => InvokeEventAsync(Resume);

    private protected ValueTask InvokeEventAsync(Func<ValueTask>? @event)
    {
        if (@event is not null)
        {
            ValueTask task;
            lock (_eventsLock)
            {
                try
                {
                    task = @event();
                }
                catch (Exception ex)
                {
                    InvokeLogWithoutLock(LogMessage.Error(ex));
                    return default;
                }
            }

            return AwaitEventAsync(task);
        }
        else
            return default;
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, Func<T> dataFunc)
    {
        if (@event is not null)
        {
            ValueTask task;
            var data = dataFunc();
            lock (_eventsLock)
            {
                try
                {
                    task = @event(data);
                }
                catch (Exception ex)
                {
                    InvokeLogWithoutLock(LogMessage.Error(ex));
                    return default;
                }
            }

            return AwaitEventAsync(task);
        }
        else
            return default;
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data)
    {
        if (@event is not null)
        {
            ValueTask task;
            lock (_eventsLock)
            {
                try
                {
                    task = @event(data);
                }
                catch (Exception ex)
                {
                    InvokeLogWithoutLock(LogMessage.Error(ex));
                    return default;
                }
            }

            return AwaitEventAsync(task);
        }
        else
            return default;
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, Action<T> updateData)
    {
        if (@event is not null)
        {
            ValueTask task;
            lock (_eventsLock)
            {
                try
                {
                    task = @event(data);
                    updateData(data);
                }
                catch (Exception ex)
                {
                    updateData(data);
                    InvokeLogWithoutLock(LogMessage.Error(ex));
                    return default;
                }
            }

            return AwaitEventAsync(task);
        }
        else
        {
            lock (_eventsLock)
                updateData(data);
            return default;
        }
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, Func<T> dataFunc, Action updateData)
    {
        if (@event is not null)
        {
            ValueTask task;
            var data = dataFunc();
            lock (_eventsLock)
            {
                try
                {
                    task = @event(data);
                    updateData();
                }
                catch (Exception ex)
                {
                    updateData();
                    InvokeLogWithoutLock(LogMessage.Error(ex));
                    return default;
                }
            }

            return AwaitEventAsync(task);
        }
        else
        {
            lock (_eventsLock)
                updateData();
            return default;
        }
    }

    private protected async ValueTask InvokeEventAsync<TPartial, T>(Func<T, ValueTask>? @event, Func<TPartial> partialDataFunc, Func<TPartial, T> dataFunc, Func<TPartial, bool> cacheFunc, Func<TPartial, SemaphoreSlim> semaphoreFunc, Func<TPartial, ValueTask> cacheAsyncFunc)
    {
        if (@event is not null)
        {
            var partialData = partialDataFunc();
            ValueTask task;
            if (cacheFunc(partialData))
            {
                var semaphore = semaphoreFunc(partialData);
                await semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    await cacheAsyncFunc(partialData).ConfigureAwait(false);
                    var data = dataFunc(partialData);
                    lock (_eventsLock)
                    {
                        try
                        {
                            task = @event(data);
                        }
                        catch (Exception ex)
                        {
                            InvokeLogWithoutLock(LogMessage.Error(ex));
                            return;
                        }
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            else
            {
                var data = dataFunc(partialData);
                lock (_eventsLock)
                {
                    try
                    {
                        task = @event(data);
                    }
                    catch (Exception ex)
                    {
                        InvokeLogWithoutLock(LogMessage.Error(ex));
                        return;
                    }
                }
            }

            await AwaitEventAsync(task).ConfigureAwait(false);
        }
    }

    private async ValueTask AwaitEventAsync(ValueTask task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
        }
    }

    [DoesNotReturn]
    private static void ThrowConnectionAlreadyStarted()
    {
        throw new InvalidOperationException("Connection already started.");
    }

    [DoesNotReturn]
    private static void ThrowConnectionNotStarted(Exception? innerException = null)
    {
        throw new InvalidOperationException("Connection not started.", innerException);
    }

    [DoesNotReturn]
    private static void ThrowRateLimitTriggered(int resetAfter)
    {
        throw new InvalidOperationException("Rate limit triggered.");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _state?.Dispose();
    }
}
