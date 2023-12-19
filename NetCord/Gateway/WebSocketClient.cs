using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.Gateway.JsonModels;
using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectTimers;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

public abstract class WebSocketClient : IDisposable
{
    private protected WebSocketClient(IWebSocket? webSocket, IReconnectTimer? reconnectTimer, ILatencyTimer? latencyTimer)
    {
        webSocket ??= new WebSocket();
        reconnectTimer ??= new ReconnectTimer();
        latencyTimer ??= new LatencyTimer();

        webSocket.Connecting += async () =>
        {
            InvokeLog(LogMessage.Info("Connecting"));
            await InvokeEventAsync(Connecting).ConfigureAwait(false);
        };
        webSocket.Connected += async () =>
        {
            _disconnectedToken = (_disconnectedTokenSource = new()).Token;

            OnConnected();
            InvokeLog(LogMessage.Info("Connected"));
            await InvokeEventAsync(Connect).ConfigureAwait(false);
        };
        webSocket.Disconnected += async (closeStatus, description) =>
        {
            var disconnectedTokenSource = _disconnectedTokenSource!;
            disconnectedTokenSource.Cancel();
            disconnectedTokenSource.Dispose();

            InvokeLog(string.IsNullOrEmpty(description) ? LogMessage.Info("Disconnected") : LogMessage.Info("Disconnected", description.EndsWith('.') ? description[..^1] : description));
            var reconnect = Reconnect(closeStatus, description);
            var disconnectTask = InvokeEventAsync(Disconnect, reconnect);
            if (reconnect)
                await ReconnectAsync().ConfigureAwait(false);
            else
                _readyCompletionSource.TrySetCanceled();

            await disconnectTask.ConfigureAwait(false);
        };
        webSocket.Closed += async () =>
        {
            var disconnectedTokenSource = _disconnectedTokenSource!;
            disconnectedTokenSource.Cancel();
            disconnectedTokenSource.Dispose();

            InvokeLog(LogMessage.Info("Closed"));
            var closeTask = InvokeEventAsync(Close).ConfigureAwait(false);

            _readyCompletionSource.TrySetCanceled();

            await closeTask;
        };
        webSocket.MessageReceived += async data =>
        {
            try
            {
                var payload = CreatePayload(data);
                await ProcessPayloadAsync(payload).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        };
        _webSocket = webSocket;
        _reconnectTimer = reconnectTimer;
        _latencyTimer = latencyTimer;
    }

    private readonly IWebSocket _webSocket;
    private readonly object _eventsLock = new();

    private protected readonly IReconnectTimer _reconnectTimer;
    private protected readonly ILatencyTimer _latencyTimer;
    private protected readonly TaskCompletionSource _readyCompletionSource = new();

    private CancellationTokenSource? _disconnectedTokenSource;
    private CancellationToken _disconnectedToken;
    private CancellationTokenSource? _closedTokenSource;
    private CancellationToken _closedToken;

    public Task ReadyAsync => _readyCompletionSource.Task;

    public TimeSpan Latency => _latency;
    private TimeSpan _latency;

    public event Func<TimeSpan, ValueTask>? LatencyUpdate;
    public event Func<ValueTask>? Resume;
    public event Func<ValueTask>? Connecting;
    public event Func<ValueTask>? Connect;
    public event Func<bool, ValueTask>? Disconnect;
    public event Func<ValueTask>? Close;
    public event Func<LogMessage, ValueTask>? Log;

    private protected Task ConnectAsync(Uri uri)
    {
        var closedTokenSource = _closedTokenSource;
        if (closedTokenSource is null || closedTokenSource.IsCancellationRequested)
            _closedToken = (_closedTokenSource = new()).Token;
        return _webSocket.ConnectAsync(uri);
    }

    /// <summary>
    /// Closes the <see cref="WebSocketClient"/>.
    /// </summary>
    /// <param name="status">The status to close with.</param>
    /// <returns></returns>
    public async Task CloseAsync(System.Net.WebSockets.WebSocketCloseStatus status = System.Net.WebSockets.WebSocketCloseStatus.NormalClosure)
    {
        var closedTokenSource = _closedTokenSource;
        if (closedTokenSource is not null && !closedTokenSource.IsCancellationRequested)
        {
            closedTokenSource.Cancel();
            closedTokenSource.Dispose();
        }

        try
        {
            await _webSocket.CloseAsync(status).ConfigureAwait(false);
        }
        catch
        {
        }
    }

    private protected virtual void OnConnected()
    {
    }

    private protected async Task CloseAndReconnectAsync(System.Net.WebSockets.WebSocketCloseStatus status)
    {
        try
        {
            await _webSocket.CloseAsync(status).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            InvokeLog(LogMessage.Error(ex));
        }

        await ReconnectAsync().ConfigureAwait(false);
    }

    public ValueTask SendPayloadAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => _webSocket.SendAsync(buffer, cancellationToken);

    private protected abstract bool Reconnect(System.Net.WebSockets.WebSocketCloseStatus? status, string? description);

    private protected async ValueTask ReconnectAsync()
    {
        while (true)
        {
            try
            {
                await _reconnectTimer.NextAsync(_closedToken).ConfigureAwait(false);
            }
            catch
            {
                return;
            }
            try
            {
                await TryResumeAsync().ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
    }

    private protected abstract Task TryResumeAsync();

    private protected async void BeginHeartbeating(double interval)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(interval));
        while (true)
        {
            try
            {
                await timer.WaitForNextTickAsync(_disconnectedToken).ConfigureAwait(false);
                await HeartbeatAsync().ConfigureAwait(false);
            }
            catch
            {
                return;
            }
        }
    }

    private protected abstract ValueTask HeartbeatAsync();

    private protected virtual JsonPayload CreatePayload(ReadOnlyMemory<byte> payload) => JsonSerializer.Deserialize(payload.Span, JsonPayload.JsonPayloadSerializerContext.WithOptions.JsonPayload)!;

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

    public virtual void Dispose()
    {
        _webSocket.Dispose();
        var disconnectedTokenSource = _disconnectedTokenSource;
        if (disconnectedTokenSource is not null && !disconnectedTokenSource.IsCancellationRequested)
        {
            disconnectedTokenSource.Cancel();
            disconnectedTokenSource.Dispose();
        }
        var closedTokenSource = _closedTokenSource;
        if (closedTokenSource is not null && !closedTokenSource.IsCancellationRequested)
        {
            closedTokenSource.Cancel();
            closedTokenSource.Dispose();
        }
        _reconnectTimer.Dispose();
    }
}
