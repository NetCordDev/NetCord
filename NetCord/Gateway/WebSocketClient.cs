using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;

using static NetCord.Gateway.GatewayClientThrowHelper;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway;

public abstract partial class WebSocketClient : IDisposable
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

        private byte _state;

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

    private protected class State : IDisposable
    {
        private readonly Lock _lock = new();

        private TaskCompletionSource<ConnectionStateResult> _readyCompletionSource = new();

        private ConnectionState? _connectionState;

        public CancellationTokenProvider ClosedTokenProvider { get; } = new();

        public Task<ConnectionStateResult> ReadyTask => _readyCompletionSource.Task;

        public void IndicateReady(ConnectionState connectionState)
        {
            lock (_lock)
            {
                if (_connectionState != connectionState)
                    return;

                _readyCompletionSource.TrySetResult(new ConnectionStateResult.Success(connectionState));
            }
        }

        public ConnectingResult TryIndicateConnecting(ConnectionState connectionState)
        {
            lock (_lock)
            {
                if (ClosedTokenProvider.IsCancellationRequested)
                    return ConnectingResult.Closed;

                if (_connectionState is not null)
                    return ConnectingResult.AlreadyStarted;

                _connectionState = connectionState;
            }

            return ConnectingResult.Success;
        }

        public virtual void Abort()
        {
        }

        public enum ConnectingResult : byte
        {
            Success,
            AlreadyStarted,
            Closed,
        }

        public void IndicateConnectingFailed(ConnectionState connectionState)
        {
            lock (_lock)
            {
                _ = connectionState.TryIndicateDisconnecting();

                if (_connectionState != connectionState)
                    return;

                _connectionState = null;
            }
        }

        public bool TryIndicateClosing([MaybeNullWhen(false)] out ConnectionState connectionState)
        {
            lock (_lock)
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
            lock (_lock)
            {
                if (_connectionState != connectionState || !connectionState.TryIndicateDisconnecting())
                    return false;

                _connectionState = null;

                _readyCompletionSource.TrySetResult(ConnectionStateResult.Retry.Instance);
                _readyCompletionSource = new();
            }

            return true;
        }

        public WebSocketStatus GetStatus()
        {
            var readyTask = _readyCompletionSource.Task;

            return readyTask.Status switch
            {
                TaskStatus.RanToCompletion => readyTask.Result is ConnectionStateResult.Success
                                                    ? WebSocketStatus.Ready
                                                    : WebSocketStatus.Connecting,
                TaskStatus.Canceled => WebSocketStatus.Disconnected,
                _ => WebSocketStatus.Connecting,
            };
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
        _defaultPayloadProperties = CreatePayloadProperties(configuration.DefaultPayloadProperties);
        _logger = configuration.Logger ?? NullLogger.Instance;
    }

    private static InternalWebSocketPayloadProperties CreatePayloadProperties(WebSocketPayloadProperties? properties)
    {
        return properties switch
        {
            null => new(default, WebSocketMessageFlags.EndOfMessage, WebSocketRetryHandling.Retry),
            _ => new InternalWebSocketPayloadProperties(properties.MessageType.GetValueOrDefault(),
                                                        properties.MessageFlags.GetValueOrDefault(WebSocketMessageFlags.EndOfMessage),
                                                        properties.RetryHandling.GetValueOrDefault(WebSocketRetryHandling.Retry))
        };
    }

    private protected record struct InternalWebSocketPayloadProperties(WebSocketMessageType MessageType, WebSocketMessageFlags MessageFlags, WebSocketRetryHandling RetryHandling)
    {
        public readonly InternalWebSocketPayloadProperties Compose(WebSocketPayloadProperties? properties)
        {
            return properties switch
            {
                null => this,
                _ => new InternalWebSocketPayloadProperties(properties.MessageType.GetValueOrDefault(MessageType),
                                                            properties.MessageFlags.GetValueOrDefault(MessageFlags),
                                                            properties.RetryHandling.GetValueOrDefault(RetryHandling))
            };
        }
    }

    private readonly IWebSocketConnectionProvider _connectionProvider;
    private readonly IReconnectStrategy _reconnectStrategy;
    private readonly IRateLimiterProvider _rateLimiterProvider;
    private readonly InternalWebSocketPayloadProperties _defaultPayloadProperties;
    private protected readonly InternalWebSocketPayloadProperties _internalPayloadProperties = new(default, WebSocketMessageFlags.EndOfMessage, WebSocketRetryHandling.RetryRateLimit);

    private protected readonly ILatencyTimer _latencyTimer;
    private protected readonly IWebSocketLogger _logger;

    private State? _state;

    private protected abstract Uri Uri { get; }

    /// <summary>
    /// The latency of the <see cref="WebSocketClient"/>.
    /// </summary>
    public TimeSpan Latency
    {
        get
        {
            var latency = Interlocked.Read(ref Unsafe.As<TimeSpan, long>(ref _latency));
            return Unsafe.As<long, TimeSpan>(ref latency);
        }
    }

    private TimeSpan _latency;

    /// <summary>
    /// The status of the <see cref="WebSocketClient"/>.
    /// </summary>
    public WebSocketStatus Status => _state is { } state ? state.GetStatus() : WebSocketStatus.Disconnected;

    public partial event Func<TimeSpan, ValueTask>? LatencyUpdate;
    public partial event Func<ValueTask>? Resume;
    public partial event Func<ValueTask>? Connecting;
    public partial event Func<ValueTask>? Connect;
    public partial event Func<DisconnectEventArgs, ValueTask>? Disconnect;
    public partial event Func<ValueTask>? Close;

    private protected State GetState()
    {
        var state = _state;

        if (state is null)
            ThrowConnectionNotStarted();

        return state;
    }

    private protected void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        try
        {
            _logger.Log(logLevel, state, exception, formatter);
        }
        catch
        {
        }
    }

    private protected static void AddEventHandler<T>(ref ImmutableList<T> handlers, T? value) where T : class
    {
        if (value is null)
            return;

        var currentHandlers = handlers;

        while (true)
        {
            var newHandlers = currentHandlers.Add(value);
            var oldHandlers = Interlocked.CompareExchange(ref handlers, newHandlers, currentHandlers);
            if (currentHandlers == oldHandlers)
                break;
        }
    }

    private protected static void RemoveEventHandler<T>(ref ImmutableList<T> handlers, T? value) where T : class
    {
        if (value is null)
            return;

        var currentHandlers = handlers;

        while (true)
        {
            var newHandlers = currentHandlers.Remove(value);
            var oldHandlers = Interlocked.CompareExchange(ref handlers, newHandlers, currentHandlers);
            if (currentHandlers == oldHandlers)
                break;
        }
    }

    private async void HandleConnecting()
    {
        Log<object?>(LogLevel.Information, null, null, static (s, e) => "Connecting.");

        await InvokeEventAsync(_connecting).ConfigureAwait(false);
    }

    private async void HandleConnected()
    {
        OnConnected();

        Log<object?>(LogLevel.Information, null, null, static (s, e) => "Connected.");

        await InvokeEventAsync(_connect).ConfigureAwait(false);
    }

    private async void HandleDisconnected(State state, ConnectionState connectionState)
    {
        try
        {
            var connection = connectionState.Connection;

            var description = connection.CloseStatusDescription;

            Log(LogLevel.Information, description, null, static (s, e) =>
            {
                return s switch
                {
                    null or { Length: 0 } => "Disconnected.",
                    [.., '.'] => $"Disconnected: {s}",
                    _ => $"Disconnected: {s}.",
                };
            });

            var reconnect = Reconnect((WebSocketCloseStatus?)connection.CloseStatus, description);

            var disconnectTask = InvokeEventAsync(_disconnect, () => new DisconnectEventArgs(reconnect));

            if (reconnect)
            {
                connectionState.Dispose();
                await ResumeAsync(state).ConfigureAwait(false);
            }
            else
            {
                Interlocked.CompareExchange(ref _state, null, state);
                connectionState.Dispose();
                state.Dispose();
            }

            await disconnectTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while handling the disconnection.{Environment.NewLine}{e}";
            });
        }
    }

    private async void HandleClosed()
    {
        Log<object?>(LogLevel.Information, null, null, static (s, e) => "Closed.");

        await InvokeEventAsync(_close).ConfigureAwait(false);
    }

    private async void HandleMessageReceived(State state, ConnectionState connectionState, ReadOnlyMemory<byte> data)
    {
        try
        {
            await ProcessPayloadAsync(state, connectionState, data.Span).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while processing the payload.{Environment.NewLine}{e}";
            });

            try
            {
                await AbortAndResumeAsync(state, connectionState).ConfigureAwait(false);
            }
            catch (Exception abortEx)
            {
                Log<object?>(LogLevel.Error, null, abortEx, static (s, e) =>
                {
                    return $"An error occurred while aborting and resuming.{Environment.NewLine}{e}";
                });
            }
        }
    }

    private protected async ValueTask<ConnectionState> StartAsync(State newState, CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _state, newState, null) is not null)
        {
            newState.Dispose();
            ThrowConnectionAlreadyStarted();
        }

        ConnectionState connectionState;
        try
        {
            connectionState = await ConnectAsync(newState, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            Interlocked.CompareExchange(ref _state, null, newState);
            newState.Dispose();
            throw;
        }

        return connectionState;
    }

    private protected async ValueTask<ConnectionState> ConnectAsync(State state, CancellationToken cancellationToken = default)
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

    internal async ValueTask<bool> CloseIfStartedAsync(WebSocketCloseStatus status, string? statusDescription, CancellationToken cancellationToken)
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is null)
            return false;

        if (!state.TryIndicateClosing(out var connectionState))
            return true;

        var connection = connectionState.Connection;
        try
        {
            await connection.CloseAsync((int)status, statusDescription, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while closing the connection.{Environment.NewLine}{e}";
            });

            try
            {
                connection.Abort();
                state.Abort();
            }
            catch (Exception abortEx)
            {
                Log<object?>(LogLevel.Error, null, abortEx, static (s, e) =>
                {
                    return $"An error occurred while aborting the connection.{Environment.NewLine}{e}";
                });
            }
        }

        connectionState.Dispose();
        state.Dispose();
        HandleClosed();

        return true;
    }

    /// <summary>
    /// Closes the <see cref="WebSocketClient"/>.
    /// </summary>
    /// <param name="status">The status to close with.</param>
    /// <param name="statusDescription">The status description to close with.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async ValueTask CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? statusDescription = null, CancellationToken cancellationToken = default)
    {
        if (!await CloseIfStartedAsync(status, statusDescription, cancellationToken).ConfigureAwait(false))
            ThrowConnectionNotStarted();
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
            state.Abort();
        }
        catch (Exception ex)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while aborting the connection.{Environment.NewLine}{e}";
            });
        }

        connectionState.Dispose();
        state.Dispose();
        HandleClosed();
    }

    private protected virtual void OnConnected()
    {
    }

    private protected ValueTask AbortAndResumeAsync(State state, ConnectionState connectionState)
    {
        if (!state.TryIndicateDisconnecting(connectionState))
            return default;

        try
        {
            connectionState.Connection.Abort();
        }
        catch (Exception ex)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while aborting the connection.{Environment.NewLine}{e}";
            });
        }

        connectionState.Dispose();
        HandleClosed();

        return ResumeAsync(state);
    }

    private protected ValueTask AbortAndRestartAsync(State state, ConnectionState connectionState)
    {
        if (!state.TryIndicateDisconnecting(connectionState))
            return default;

        try
        {
            connectionState.Connection.Abort();
        }
        catch (Exception ex)
        {
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while aborting the connection.{Environment.NewLine}{e}";
            });
        }

        connectionState.Dispose();
        HandleClosed();

        return RestartAsync(state);
    }

    public async ValueTask SendPayloadAsync(ReadOnlyMemory<byte> buffer, WebSocketPayloadProperties? properties = null, CancellationToken cancellationToken = default)
    {
        var payloadProperties = _defaultPayloadProperties.Compose(properties);

        while (true)
        {
            var state = GetState();

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
                if (payloadProperties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
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

            var exception = await TrySendConnectionPayloadAsync(connectionState, buffer, payloadProperties, cancellationToken).ConfigureAwait(false);

            if (exception is null)
                return;

            if (!payloadProperties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryReconnect))
                ThrowConnectionNotStarted();
        }
    }

    private protected async ValueTask SendConnectionPayloadAsync(ConnectionState connectionState, ReadOnlyMemory<byte> buffer, InternalWebSocketPayloadProperties payloadProperties, CancellationToken cancellationToken = default)
    {
        var exception = await TrySendConnectionPayloadAsync(connectionState, buffer, payloadProperties, cancellationToken).ConfigureAwait(false);
        if (exception is null)
            return;

        ThrowConnectionNotStarted(exception);
    }

    private async ValueTask<Exception?> TrySendConnectionPayloadAsync(ConnectionState connectionState, ReadOnlyMemory<byte> buffer, InternalWebSocketPayloadProperties payloadProperties, CancellationToken cancellationToken = default)
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
                if (payloadProperties.RetryHandling.HasFlag(WebSocketRetryHandling.RetryRateLimit))
                {
                    try
                    {
                        Log<object?>(LogLevel.Warning, result.ResetAfter, null, static (s, e) =>
                        {
                            return $"Rate limit exceeded. Retrying after {s} ms.";
                        });

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

            Log<object?>(LogLevel.Debug, null, null, static (s, e) => "Sending a payload.");

            var timestamp = Environment.TickCount64;

            try
            {
                await connectionState.Connection.SendAsync(buffer, payloadProperties.MessageType, payloadProperties.MessageFlags, linkedToken).ConfigureAwait(false);
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

    private protected async ValueTask ResumeAsync(State state)
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
                Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
                {
                    return $"An error occurred while reconnecting.{Environment.NewLine}{e}";
                });

                continue;
            }

            try
            {
                await TryResumeAsync(connectionState, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
                {
                    return $"An error occurred while resuming the connection.{Environment.NewLine}{e}";
                });
            }

            return;
        }
    }

    private protected async ValueTask RestartAsync(State state)
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
                Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
                {
                    return $"An error occurred while reconnecting.{Environment.NewLine}{e}";
                });

                continue;
            }

            try
            {
                await SendIdentifyAsync(connectionState, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
                {
                    return $"An error occurred while restarting the connection.{Environment.NewLine}{e}";
                });
            }

            return;
        }
    }

    private protected abstract ValueTask SendIdentifyAsync(ConnectionState connectionState, CancellationToken cancellationToken = default);

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
            Log<object?>(LogLevel.Error, null, ex, static (s, e) =>
            {
                return $"An error occurred while creating the heartbeat timer.{Environment.NewLine}{e}";
            });

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

    private protected abstract ValueTask ProcessPayloadAsync(State state, ConnectionState connectionState, ReadOnlySpan<byte> payload);

    private protected ValueTask UpdateLatencyAsync(TimeSpan latency)
        => InvokeEventAsync(_latencyUpdate, latency, latency => Interlocked.Exchange(ref Unsafe.As<TimeSpan, long>(ref _latency), Unsafe.As<TimeSpan, long>(ref latency)));

    private protected ValueTask InvokeResumeEventAsync()
        => InvokeEventAsync(_resume);

    private protected ValueTask InvokeEventAsync(ImmutableList<Func<ValueTask>> handlers, [CallerArgumentExpression(nameof(handlers))] string handlersName = "")
    {
        int count = handlers.Count;

        if (count is 0)
            return default;

        var tasks = ArrayPool<ValueTask>.Shared.Rent(count);

        for (int i = 0; i < count; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i]();
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogEventHandlerException(handlersName, ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(tasks, handlersName, count);
    }

    private protected ValueTask InvokeEventAsync<T>(ImmutableList<Func<T, ValueTask>> handlers, Func<T> dataFunc, [CallerArgumentExpression(nameof(handlers))] string handlersName = "") where T : allows ref struct
    {
        int count = handlers.Count;

        if (count is 0)
            return default;

        var data = dataFunc();

        var tasks = ArrayPool<ValueTask>.Shared.Rent(count);

        for (int i = 0; i < count; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogEventHandlerException(handlersName, ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(tasks, handlersName, count);
    }

    private protected ValueTask InvokeEventAsync<T>(ImmutableList<Func<T, ValueTask>> handlers, T data, [CallerArgumentExpression(nameof(handlers))] string handlersName = "") where T : allows ref struct
    {
        int count = handlers.Count;

        if (count is 0)
            return default;

        var tasks = ArrayPool<ValueTask>.Shared.Rent(count);

        for (int i = 0; i < count; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogEventHandlerException(handlersName, ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(tasks, handlersName, count);
    }

    private protected ValueTask InvokeEventAsync<T>(ImmutableList<Func<T, ValueTask>> handlers, T data, Action<T> updateCache, [CallerArgumentExpression(nameof(handlers))] string handlersName = "") where T : allows ref struct
    {
        int count = handlers.Count;

        if (count is 0)
        {
            updateCache(data);
            return default;
        }

        var tasks = ArrayPool<ValueTask>.Shared.Rent(count);

        for (int i = 0; i < count; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogEventHandlerException(handlersName, ex);

                tasks[i] = default;
            }
        }

        updateCache(data);

        return HandleTasksAsync(tasks, handlersName, count);
    }

    private protected ValueTask InvokeEventAsync<T>(ImmutableList<Func<T, ValueTask>> handlers, Func<T> dataFunc, Action updateCache, [CallerArgumentExpression(nameof(handlers))] string handlersName = "") where T : allows ref struct
    {
        int count = handlers.Count;

        if (count is 0)
        {
            updateCache();
            return default;
        }

        var data = dataFunc();

        var tasks = ArrayPool<ValueTask>.Shared.Rent(count);

        for (int i = 0; i < count; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogEventHandlerException(handlersName, ex);

                tasks[i] = default;
            }
        }

        updateCache();

        return HandleTasksAsync(tasks, handlersName, count);
    }

    private protected ValueTask InvokeEventWithDisposalAsync<T>(ImmutableList<Func<T, ValueTask>> handlers, Func<T> dataFunc, Action<T> disposeData, [CallerArgumentExpression(nameof(handlers))] string handlersName = "") where T : allows ref struct
    {
        int count = handlers.Count;

        if (count is 0)
            return default;

        var data = dataFunc();

        var tasks = ArrayPool<ValueTask>.Shared.Rent(count);

        for (int i = 0; i < count; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogEventHandlerException(handlersName, ex);

                tasks[i] = default;
            }
        }

        disposeData(data);

        return HandleTasksAsync(tasks, handlersName, count);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask HandleTasksAsync(ValueTask[] tasks, string handlersName, int count)
    {
        for (int i = 0; i < count; i++)
        {
            try
            {
                await tasks[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogEventHandlerException(handlersName, ex);
            }
        }

        ArrayPool<ValueTask>.Shared.Return(tasks);
    }

    private void LogEventHandlerException(string handlersName, Exception ex)
    {
        Log(LogLevel.Error, handlersName, ex, static (s, e) =>
        {
            return $"An error occurred while invoking an event handler of '{new EventNameFormatter(s)}'.{Environment.NewLine}{e}";
        });
    }

    private struct EventNameFormatter(string handlersName) : ISpanFormattable
    {
        public readonly string HandlersName => handlersName;

        public override readonly string ToString()
        {
            return string.Create(HandlersName.Length - 1, HandlersName, static (span, handlersName) =>
            {
                span[0] = (char)(handlersName[1] & ~0x20);
                handlersName.AsSpan(2).CopyTo(span[1..]);
            });
        }

        public readonly string ToString(string? format, IFormatProvider? formatProvider) => ToString();

        public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            int resultLength = HandlersName.Length - 1;
            if (destination.Length < resultLength)
            {
                charsWritten = 0;
                return false;
            }

            destination[0] = (char)(HandlersName[1] & ~0x20);
            HandlersName.AsSpan(2).CopyTo(destination[1..]);

            charsWritten = resultLength;
            return true;
        }
    }

    [DoesNotReturn]
    private static void ThrowRateLimitTriggered(int resetAfter)
    {
        throw new WebSocketRateLimitedException(resetAfter);
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
