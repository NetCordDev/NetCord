using System.Text.Json;

using NetCord.Gateway.ReconnectTimers;
using NetCord.Gateway.WebSockets;
using NetCord.JsonModels;

namespace NetCord.Gateway;

public abstract class WebSocketClient : IDisposable
{
    private protected WebSocketClient(IWebSocket? webSocket, IReconnectTimer? reconnectTimer)
    {
        webSocket ??= new WebSocket();
        reconnectTimer ??= new ReconnectTimer();

        webSocket.Connecting += async () =>
        {
            InvokeLog(LogMessage.Info("Connecting"));
            await InvokeEventAsync(Connecting).ConfigureAwait(false);
        };
        webSocket.Connected += async () =>
        {
            _disconnectedToken = (_disconnectedTokenSource = new()).Token;
            _closedToken = (_closedTokenSource = new()).Token;

            InvokeLog(LogMessage.Info("Connected"));
            await InvokeEventAsync(Connected).ConfigureAwait(false);
        };
        webSocket.Disconnected += async (closeStatus, description) =>
        {
            var disconnectedTokenSource = _disconnectedTokenSource!;
            disconnectedTokenSource.Cancel();
            disconnectedTokenSource.Dispose();

            InvokeLog(string.IsNullOrEmpty(description) ? LogMessage.Info("Disconnected") : LogMessage.Info("Disconnected", description.EndsWith('.') ? description[..^1] : description));
            var reconnect = Reconnect(closeStatus, description);
            var disconnectedTask = InvokeEventAsync(Disconnected, reconnect);
            if (reconnect)
                await ReconnectAsync().ConfigureAwait(false);
            else
                _readyCompletionSource.TrySetCanceled();

            await disconnectedTask.ConfigureAwait(false);
        };
        webSocket.Closed += async () =>
        {
            var disconnectedTokenSource = _disconnectedTokenSource!;
            var closedTokenSource = _closedTokenSource!;

            disconnectedTokenSource.Cancel();
            closedTokenSource.Cancel();

            disconnectedTokenSource.Dispose();
            closedTokenSource.Dispose();

            InvokeLog(LogMessage.Info("Closed"));
            var closedTask = InvokeEventAsync(Closed).ConfigureAwait(false);

            _readyCompletionSource.TrySetCanceled();

            await closedTask;
        };
        webSocket.MessageReceived += async data =>
        {
            try
            {
                await ProcessMessageAsync(JsonSerializer.Deserialize(data.Span, JsonPayload.JsonPayloadSerializerContext.WithOptions.JsonPayload)!).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        };
        _webSocket = webSocket;
        _reconnectTimer = reconnectTimer;
    }

    private protected readonly IWebSocket _webSocket;
    private protected readonly LatencyTimer _latencyTimer = new();
    private protected readonly IReconnectTimer _reconnectTimer;
    private protected readonly object _eventsLock = new();
    private protected readonly TaskCompletionSource _readyCompletionSource = new();

    private protected CancellationTokenSource? _disconnectedTokenSource;
    private protected CancellationToken _disconnectedToken;
    private protected CancellationTokenSource? _closedTokenSource;
    private protected CancellationToken _closedToken;

    public Task ReadyAsync => _readyCompletionSource.Task;

    public TimeSpan? Latency => _latency;
    private TimeSpan? _latency;

    public event Func<TimeSpan, ValueTask>? LatencyUpdated;
    public event Func<ValueTask>? Resumed;
    public event Func<ValueTask>? Connecting;
    public event Func<ValueTask>? Connected;
    public event Func<bool, ValueTask>? Disconnected;
    public event Func<ValueTask>? Closed;
    public event Func<LogMessage, ValueTask>? Log;

    /// <summary>
    /// Closes the <see cref="WebSocketClient"/>.
    /// </summary>
    /// <param name="status">The status to close with.</param>
    /// <returns></returns>
    public Task CloseAsync(System.Net.WebSockets.WebSocketCloseStatus status = System.Net.WebSockets.WebSocketCloseStatus.NormalClosure)
        => _webSocket.CloseAsync(status);

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

    private protected abstract Task ProcessMessageAsync(JsonPayload payload);

    private protected async void InvokeLog(LogMessage logMessage)
    {
        var log = Log;
        if (log != null)
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
        if (log != null)
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
        => InvokeEventAsync(LatencyUpdated, latency, out _latency);

    private protected ValueTask InvokeResumedEventAsync()
        => InvokeEventAsync(Resumed);

    private protected ValueTask InvokeEventAsync(Func<ValueTask>? @event)
    {
        if (@event != null)
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
        if (@event != null)
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
        if (@event != null)
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

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, out T dataField) where T : class
    {
        if (@event != null)
        {
            ValueTask task;
            lock (_eventsLock)
            {
                try
                {
                    task = @event(data);
                    dataField = data;
                }
                catch (Exception ex)
                {
                    dataField = data;
                    InvokeLogWithoutLock(LogMessage.Error(ex));
                    return default;
                }
            }

            return AwaitEventAsync(task);
        }
        else
        {
            lock (_eventsLock)
                dataField = data;
            return default;
        }
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, out T? dataField) where T : struct
    {
        if (@event != null)
        {
            ValueTask task;
            lock (_eventsLock)
            {
                try
                {
                    task = @event(data);
                    dataField = data;
                }
                catch (Exception ex)
                {
                    dataField = data;
                    InvokeLogWithoutLock(LogMessage.Error(ex));
                    return default;
                }
            }

            return AwaitEventAsync(task);
        }
        else
        {
            lock (_eventsLock)
                dataField = data;
            return default;
        }
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, Action<T> updateData)
    {
        if (@event != null)
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
        if (@event != null)
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
        if (@event != null)
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
