using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.Gateway.JsonModels;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway;

public abstract class WebSocketClient : IDisposable
{
    private protected sealed class ConnectionState(IWebSocketConnection connection, IRateLimiter rateLimiter) : IDisposable
    {
        public IWebSocketConnection Connection => connection;

        public IRateLimiter RateLimiter => rateLimiter;

        public CancellationTokenProvider DisconnectedTokenProvider { get; } = new();

        public Task ReadTask => _readCompletionSource.Task;

        private readonly TaskCompletionSource _readCompletionSource = new();

        private int _state;

        public async void StartReading(State state, Func<State, Task> readAsync)
        {
            try
            {
                await readAsync(state).ConfigureAwait(false);
            }
            finally
            {
                _readCompletionSource.TrySetResult();
            }
        }

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

        public ConnectionState? ConnectionState => _connectionState;

        public CancellationTokenProvider ClosedTokenProvider { get; } = new();

        public Task<ConnectionState> ReadyTask => _readyCompletionSource.Task;

        private TaskCompletionSource<ConnectionState> _readyCompletionSource = new();

        public Task<ConnectionState> ConnectedTask => _connectedCompletionSource.Task;

        private TaskCompletionSource<ConnectionState> _connectedCompletionSource = new();

        public void IndicateConnected(ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                if (_connectionState != connectionState)
                    return;

                _connectedCompletionSource.TrySetResult(connectionState);
            }
        }

        public void IndicateReady(ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                if (_connectionState != connectionState)
                    return;

                _readyCompletionSource.TrySetResult(connectionState);
            }
        }

        public bool TryIndicateConnecting(ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                var previousState = _connectionState;
                if (previousState is not null)
                    return false;

                _connectionState = connectionState;
            }

            return true;
        }

        public bool TryIndicateDisconnecting([MaybeNullWhen(false)] out ConnectionState connectionState)
        {
            lock (ClosedTokenProvider)
            {
                var previousState = _connectionState;
                if (previousState is null || !previousState.TryIndicateDisconnecting())
                {
                    connectionState = null;
                    return false;
                }

                _connectionState = null;
                connectionState = previousState;

                _readyCompletionSource.TrySetCanceled();
                _readyCompletionSource = new();

                _connectedCompletionSource.TrySetCanceled();
                _connectedCompletionSource = new();
            }

            return true;
        }

        public void Dispose()
        {
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
        MessageFlags = WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.BypassReady,
        RetryHandling = WebSocketRetryHandling.RetryRateLimit,
    };

    private readonly object _eventsLock = new();
    private readonly IWebSocketConnectionProvider _connectionProvider;
    private readonly IReconnectStrategy _reconnectStrategy;
    private readonly IRateLimiterProvider _rateLimiterProvider;
    private readonly WebSocketPayloadProperties _defaultPayloadProperties;

    private protected readonly ILatencyTimer _latencyTimer;
    private protected readonly TaskCompletionSource _readyCompletionSource = new();

    private State? _state;

    private protected abstract Uri Uri { get; }

    public Task ReadyAsync => _readyCompletionSource.Task;

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

    private async void HandleConnected(State state)
    {
        OnConnected();
        state.IndicateConnected(state.ConnectionState!);
        InvokeLog(LogMessage.Info("Connected"));
        await InvokeEventAsync(Connect).ConfigureAwait(false);
    }

    private async void HandleDisconnected(State state, WebSocketCloseStatus? closeStatus, string? description)
    {
        InvokeLog(LogMessage.Info("Disconnected", string.IsNullOrEmpty(description) ? null : (description.EndsWith('.') ? description[..^1] : description)));
        var reconnect = Reconnect(closeStatus, description);
        var disconnectTask = InvokeEventAsync(Disconnect, reconnect);
        if (reconnect)
            await ReconnectAsync(state).ConfigureAwait(false);
        else
        {
            _state = null;
            _readyCompletionSource.TrySetCanceled();
        }

        await disconnectTask.ConfigureAwait(false);
    }

    private async void HandleClosed()
    {
        InvokeLog(LogMessage.Info("Closed"));
        var closeTask = InvokeEventAsync(Close).ConfigureAwait(false);

        _readyCompletionSource.TrySetCanceled();

        await closeTask;
    }

    private async void HandleMessageReceived(State state, ReadOnlyMemory<byte> data)
    {
        try
        {
            JsonPayload payload;
            try
            {
                payload = CreatePayload(data);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
                await AbortAndReconnectAsync(state).ConfigureAwait(false);
                return;
            }

            await ProcessPayloadAsync(state, payload).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
        }
    }

    private protected Task<ConnectionState> StartAsync(CancellationToken cancellationToken = default)
    {
        State state = new();
        if (Interlocked.CompareExchange(ref _state, state, null) is not null)
        {
            state.Dispose();
            ThrowConnectionAlreadyStarted();
        }

        return ConnectAsync(state, cancellationToken);

        //CancellationTokenProvider newTokenProvider = new();
        //if (Interlocked.CompareExchange(ref _closedTokenProvider, newTokenProvider, null) is not null)
        //{
        //    newTokenProvider.Dispose();
        //    throw new InvalidOperationException("Connection already started.");
        //}

        //if (Interlocked.CompareExchange(ref _state, new(), null) is not null)
        //    throw new InvalidOperationException("Connection already started.");

        //return ConnectAsync(cancellationToken);
    }

    private protected async Task<ConnectionState> ConnectAsync(State state, CancellationToken cancellationToken = default)
    {
        var connection = _connectionProvider.CreateConnection();
        var rateLimiter = _rateLimiterProvider.CreateRateLimiter();
        ConnectionState connectionState = new(connection, rateLimiter);
        if (!state.TryIndicateConnecting(connectionState))
        {
            connectionState.Dispose();
            ThrowConnectionAlreadyStarted();
        }

        HandleConnecting();
        await connection.OpenAsync(Uri, cancellationToken).ConfigureAwait(false);
        HandleConnected(state);
        connectionState.StartReading(state, ReadAsync);
        return connectionState;

        //HandleConnecting();
        //var connection = await _connectionProvider.CreateWebSocketConnectionAsync(Uri, cancellationToken).ConfigureAwait(false);
        //var state = _state = new(connection);
        //HandleConnected();
        //state.StartReading(ReadAsync);
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

        if (!state.TryIndicateDisconnecting(out var connectionState))
            return;

        var connection = connectionState.Connection;
        try
        {
            await connection.CloseAsync((int)status, statusDescription, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            connection.Abort();
            HandleClosed();
            throw;
        }

        await connectionState.ReadTask.ConfigureAwait(false);

        HandleClosed();
    }

    private async Task ReadAsync(State state)
    {
        var connectionState = state.ConnectionState!;
        var connection = connectionState.Connection;
        var token = connectionState.DisconnectedTokenProvider.Token;
        try
        {
            using RentedArrayBufferWriter<byte> writer = new(DefaultBufferSize);
            while (true)
            {
                var result = await connection.ReceiveAsync(writer.GetMemory(), token).ConfigureAwait(false);

                if (result.EndOfMessage)
                {
                    if (result.MessageType is WebSocketMessageType.Close)
                        break;

                    writer.Advance(result.Count);
                    HandleMessageReceived(state, writer.WrittenMemory);
                    writer.Clear();
                }
                else
                    writer.Advance(result.Count);
            }
        }
        catch
        {
        }

        if (state.TryIndicateDisconnecting(out _))
        {
            connectionState.Dispose();
            HandleDisconnected(state, (WebSocketCloseStatus?)connection.CloseStatus, connection.CloseStatusDescription);
        }
    }

    public void Abort()
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is null)
            return;

        if (state.TryIndicateDisconnecting(out var connectionState))
        {
            connectionState.Connection.Abort();
            HandleClosed();
        }
    }

    private protected virtual void OnConnected()
    {
    }

    private protected ValueTask AbortAndReconnectAsync(State state)
    {
        if (!state.TryIndicateDisconnecting(out var connectionState))
            return default;

        try
        {
            connectionState.Connection.Abort();
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
        }

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

            var task = properties.MessageFlags.HasFlag(WebSocketMessageFlags.BypassReady) ? state.ConnectedTask : state.ReadyTask;

            ConnectionState connectionState;
            if (!task.IsCompleted)
            {
                if (properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
                    connectionState = await task.ConfigureAwait(false);
                else
                {
                    ThrowConnectionNotStarted();
                    return;
                }
            }
            else
                connectionState = state.ConnectionState!;

            var exception = await TrySendConnectionPayloadAsync(connectionState, buffer, properties, cancellationToken).ConfigureAwait(false);

            if (exception is null)
                return;

            if (!properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
                ThrowConnectionNotStarted();

            //var rateLimiter = connectionState.RateLimiter;

            //if (state is null)
            //{
            //    //if (_closedTokenProvider is null)
            //    //    throw new InvalidOperationException("Connection not started.");

            //    //if (properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
            //    //{
            //    //    await Task.Delay(1000, cancellationToken).ConfigureAwait(false); //
            //    //    continue;
            //    //}

            //    //throw new InvalidOperationException($"The {nameof(WebSocketClient)} is reconnecting.");
            //}

            //var result = await rateLimiter.TryAcquireAsync().ConfigureAwait(false);

            //if (result.RateLimited)
            //{
            //    if (properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryRateLimit))
            //    {
            //        await Task.Delay(result.ResetAfter, cancellationToken).ConfigureAwait(false);
            //        continue;
            //    }

            //    throw new InvalidOperationException("Rate limit triggered.");
            //}

            //try
            //{
            //    await connectionState.Connection.SendAsync(buffer, properties.MessageType, properties.MessageFlags, cancellationToken).ConfigureAwait(false);
            //}
            //catch (Exception ex) when (ex is not ArgumentException)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    if (properties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
            //        continue;

            //    throw new InvalidOperationException($"The {nameof(WebSocketClient)} is reconnecting.", ex);
            //}

            //return;
        }
    }

    private protected static async ValueTask SendConnectionPayloadAsync(ConnectionState connectionState, ReadOnlyMemory<byte> buffer, WebSocketPayloadProperties properties, CancellationToken cancellationToken = default)
    {
        var exception = await TrySendConnectionPayloadAsync(connectionState, buffer, properties, cancellationToken).ConfigureAwait(false);
        if (exception is null)
            return;

        ThrowConnectionNotStarted(exception);
    }

    private protected static async ValueTask<Exception?> TrySendConnectionPayloadAsync(ConnectionState connectionState, ReadOnlyMemory<byte> buffer, WebSocketPayloadProperties properties, CancellationToken cancellationToken = default)
    {
        var rateLimiter = connectionState.RateLimiter;

        var disconnectedToken = connectionState.DisconnectedTokenProvider.Token;

        using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(disconnectedToken, cancellationToken);
        var linkedToken = linkedTokenSource.Token;

        while (true)
        {
            var result = await rateLimiter.TryAcquireAsync().ConfigureAwait(false);

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
                        if (disconnectedToken.IsCancellationRequested)
                            return ex;

                        throw;
                    }

                    continue;
                }

                ThrowRateLimitTriggered(result.ResetAfter);
            }

            try
            {
                await connectionState.Connection.SendAsync(buffer, properties.MessageType, properties.MessageFlags, linkedToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return ex;
            }

            return null;
        }
    }

    private protected abstract bool Reconnect(WebSocketCloseStatus? status, string? description);

    private protected async ValueTask ReconnectAsync(State state)
    {
        if (state is not { ClosedTokenProvider.Token: var cancellationToken })
            return;

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

    private protected abstract ValueTask TryResumeAsync(ConnectionState state, CancellationToken cancellationToken = default);

    private protected async void StartHeartbeating(ConnectionState state, double interval)
    {
        var cancellationToken = state.DisconnectedTokenProvider.Token;

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
                    await HeartbeatAsync(state, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    return;
                }
            }
        }
    }

    private protected abstract ValueTask HeartbeatAsync(ConnectionState connectionState, CancellationToken cancellationToken = default);

    private protected virtual JsonPayload CreatePayload(ReadOnlyMemory<byte> payload) => JsonSerializer.Deserialize(payload.Span, Serialization.Default.JsonPayload)!;

    private protected abstract Task ProcessPayloadAsync(State state, JsonPayload payload);

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
