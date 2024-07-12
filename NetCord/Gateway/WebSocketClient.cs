using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.Gateway.JsonModels;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;
using NetCord.Logging;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway;

public abstract class WebSocketClient : IDisposable
{
    private protected WebSocketClient(IWebSocketClientConfiguration configuration)
    {
        var webSocket = configuration.WebSocket ?? new WebSocket();

        webSocket.Connecting += HandleConnecting;
        webSocket.Connected += HandleConnected;
        webSocket.Disconnected += HandleDisconnected;
        webSocket.Closed += HandleClosed;
        webSocket.MessageReceived += HandleMessageReceived;

        _logger = configuration.Logger;
        _webSocket = webSocket;
        _reconnectStrategy = configuration.ReconnectStrategy ?? new ReconnectStrategy();
        _latencyTimer = configuration.LatencyTimer ?? new LatencyTimer();
    }

    private readonly object _eventsLock = new();
    private readonly IWebSocket _webSocket;
    private readonly IReconnectStrategy _reconnectStrategy;

    private protected readonly IWebSocketLogger _logger;
    private protected readonly ILatencyTimer _latencyTimer;
    private protected readonly TaskCompletionSource _readyCompletionSource = new();

    private CancellationTokenProvider? _disconnectedTokenProvider;
    private CancellationTokenProvider? _closedTokenProvider;

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

    private async void HandleConnecting()
    {
        Log(LogLevel.Information, null, "Connecting");
        await InvokeEventAsync(Connecting).ConfigureAwait(false);
    }

    private async void HandleConnected()
    {
        Interlocked.Exchange(ref _disconnectedTokenProvider, new())?.Cancel();

        OnConnected();
        Log(LogLevel.Information, null, "Connected");
        await InvokeEventAsync(Connect).ConfigureAwait(false);
    }

    private async void HandleDisconnected(WebSocketCloseStatus? closeStatus, string? description)
    {
        Interlocked.Exchange(ref _disconnectedTokenProvider, null)?.Cancel();

        Log(LogLevel.Information, null, "Disconnected. Close status: '{0}'. Description: '{1}'", closeStatus, description);
        var reconnect = Reconnect(closeStatus, description);
        var disconnectTask = InvokeEventAsync(Disconnect, reconnect);
        if (reconnect)
            await ReconnectAsync().ConfigureAwait(false);
        else
            _readyCompletionSource.TrySetCanceled();

        await disconnectTask.ConfigureAwait(false);
    }

    private async void HandleClosed()
    {
        Interlocked.Exchange(ref _disconnectedTokenProvider, null)?.Cancel();

        Log(LogLevel.Information, null, "Closed");
        var closeTask = InvokeEventAsync(Close).ConfigureAwait(false);

        _readyCompletionSource.TrySetCanceled();

        await closeTask;
    }

    private async void HandleMessageReceived(ReadOnlyMemory<byte> data)
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
                Log(LogLevel.Error, ex, "Failed to deserialize payload");
                await AbortAndReconnectAsync().ConfigureAwait(false);
                return;
            }

            await ProcessPayloadAsync(payload).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, ex, "Failed to process payload");
        }
    }

    private protected Task StartAsync(CancellationToken cancellationToken = default)
    {
        CancellationTokenProvider newTokenProvider = new();
        if (Interlocked.CompareExchange(ref _closedTokenProvider, newTokenProvider, null) is not null)
        {
            newTokenProvider.Dispose();
            throw new InvalidOperationException("Connection already started.");
        }

        return ConnectAsync(cancellationToken);
    }

    private protected Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        return _webSocket.ConnectAsync(Uri, cancellationToken);
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
        var closedTokenProvider = Interlocked.Exchange(ref _closedTokenProvider, null) ?? throw new InvalidOperationException("Connection not started.");

        closedTokenProvider.Cancel();

        try
        {
            await _webSocket.CloseAsync(status, statusDescription, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
        }
    }

    private protected virtual void OnConnected()
    {
    }

    private protected ValueTask AbortAndReconnectAsync()
    {
        try
        {
            _webSocket.Abort();
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, ex, "Failed to abort the connection");
        }

        return ReconnectAsync();
    }

    public ValueTask SendPayloadAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => _webSocket.SendAsync(buffer, cancellationToken);

    private protected abstract bool Reconnect(WebSocketCloseStatus? status, string? description);

    private protected async ValueTask ReconnectAsync()
    {
        if (_closedTokenProvider is not { Token: var cancellationToken })
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

            try
            {
                await ConnectAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, ex, "Failed to reconnect");
                continue;
            }

            try
            {
                await TryResumeAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, ex, "Failed to resume");
            }

            return;
        }
    }

    private protected abstract ValueTask TryResumeAsync(CancellationToken cancellationToken = default);

    private protected async void StartHeartbeating(double interval)
    {
        if (_disconnectedTokenProvider is not { Token: var cancellationToken })
            return;

        PeriodicTimer timer;

        try
        {
            timer = new(TimeSpan.FromMilliseconds(interval));
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, ex, "Failed to create timer");
            return;
        }

        using (timer)
        {
            while (true)
            {
                try
                {
                    await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
                    await HeartbeatAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    return;
                }
            }
        }
    }

    private protected abstract ValueTask HeartbeatAsync(CancellationToken cancellationToken = default);

    private protected virtual JsonPayload CreatePayload(ReadOnlyMemory<byte> payload) => JsonSerializer.Deserialize(payload.Span, Serialization.Default.JsonPayload)!;

    private protected abstract Task ProcessPayloadAsync(JsonPayload payload);

    private protected ValueTask UpdateLatencyAsync(TimeSpan latency)
        => InvokeEventAsync(LatencyUpdate, latency, latency => Interlocked.Exchange(ref Unsafe.As<TimeSpan, long>(ref _latency), Unsafe.As<TimeSpan, long>(ref latency)));

    private protected ValueTask InvokeResumeEventAsync()
        => InvokeEventAsync(Resume);

    private protected ValueTask InvokeEventAsync(Func<ValueTask>? @event, [CallerArgumentExpression(nameof(@event))] string? eventName = null)
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
                    LogEventError(ex, eventName);
                    return default;
                }
            }

            return AwaitEventAsync(task, eventName);
        }
        else
            return default;
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, Func<T> dataFunc, [CallerArgumentExpression(nameof(@event))] string? eventName = null)
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
                    LogEventError(ex, eventName);
                    return default;
                }
            }

            return AwaitEventAsync(task, eventName);
        }
        else
            return default;
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, [CallerArgumentExpression(nameof(@event))] string? eventName = null)
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
                    LogEventError(ex, eventName);
                    return default;
                }
            }

            return AwaitEventAsync(task, eventName);
        }
        else
            return default;
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, Action<T> updateData, [CallerArgumentExpression(nameof(@event))] string? eventName = null)
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
                    LogEventError(ex, eventName);
                    return default;
                }
            }

            return AwaitEventAsync(task, eventName);
        }
        else
        {
            lock (_eventsLock)
                updateData(data);
            return default;
        }
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, Func<T> dataFunc, Action updateData, [CallerArgumentExpression(nameof(@event))] string? eventName = null)
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
                    LogEventError(ex, eventName);
                    return default;
                }
            }

            return AwaitEventAsync(task, eventName);
        }
        else
        {
            lock (_eventsLock)
                updateData();
            return default;
        }
    }

    private protected async ValueTask InvokeEventAsync<TPartial, T>(Func<T, ValueTask>? @event, Func<TPartial> partialDataFunc, Func<TPartial, T> dataFunc, Func<TPartial, bool> cacheFunc, Func<TPartial, SemaphoreSlim> semaphoreFunc, Func<TPartial, ValueTask> cacheAsyncFunc, [CallerArgumentExpression(nameof(@event))] string? eventName = null)
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
                            LogEventError(ex, eventName);
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
                        LogEventError(ex, eventName);
                        return;
                    }
                }
            }

            await AwaitEventAsync(task, eventName).ConfigureAwait(false);
        }
    }

    private async ValueTask AwaitEventAsync(ValueTask task, string? eventName)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogEventError(ex, eventName);
        }
    }

    private void LogEventError(Exception ex, string? eventName)
    {
        Log(LogLevel.Error, ex, "An exception occured while invoking '{0}' event", eventName);
    }

    private protected void Log(LogLevel logLevel, Exception? exception, string message, params object?[] args)
    {
        try
        {
            _logger.Log(logLevel, exception, message, args);
        }
        catch
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _webSocket.Dispose();
            _disconnectedTokenProvider?.Dispose();
            _closedTokenProvider?.Dispose();
        }
    }
}
