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
    private protected WebSocketClient(IWebSocket? webSocket, IReconnectStrategy? reconnectStrategy, ILatencyTimer? latencyTimer)
    {
        webSocket ??= new WebSocket();
        reconnectStrategy ??= new ReconnectStrategy();
        latencyTimer ??= new LatencyTimer();

        webSocket.Connecting += HandleConnecting;
        webSocket.Connected += HandleConnected;
        webSocket.Disconnected += HandleDisconnected;
        webSocket.Closed += HandleClosed;
        webSocket.MessageReceived += HandleMessageReceived;

        _webSocket = webSocket;
        _reconnectStrategy = reconnectStrategy;
        _latencyTimer = latencyTimer;
    }

    private readonly object _eventsLock = new();
    private readonly IWebSocket _webSocket;
    private readonly IReconnectStrategy _reconnectStrategy;

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
    public event Func<LogMessage, ValueTask>? Log;

    private async void HandleConnecting()
    {
        InvokeLog(LogMessage.Info("Connecting"));
        await InvokeEventAsync(Connecting).ConfigureAwait(false);
    }

    private async void HandleConnected()
    {
        Interlocked.Exchange(ref _disconnectedTokenProvider, new())?.Cancel();

        OnConnected();
        InvokeLog(LogMessage.Info("Connected"));
        await InvokeEventAsync(Connect).ConfigureAwait(false);
    }

    private async void HandleDisconnected(WebSocketCloseStatus? closeStatus, string? description)
    {
        Interlocked.Exchange(ref _disconnectedTokenProvider, null)?.Cancel();

        InvokeLog(string.IsNullOrEmpty(description) ? LogMessage.Info("Disconnected") : LogMessage.Info("Disconnected", description.EndsWith('.') ? description[..^1] : description));
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

        InvokeLog(LogMessage.Info("Closed"));
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
                InvokeLog(LogMessage.Error(ex));
                await AbortAndReconnectAsync().ConfigureAwait(false);
                return;
            }

            await ProcessPayloadAsync(payload).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
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
            InvokeLog(LogMessage.Error(ex));
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
                InvokeLog(LogMessage.Error(ex));
                continue;
            }

            try
            {
                await TryResumeAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
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
