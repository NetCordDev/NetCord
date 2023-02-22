using System.Text.Json;

using NetCord.Gateway.WebSockets;
using NetCord.JsonModels;

namespace NetCord.Gateway;

public abstract class WebSocketClient : IDisposable
{
    private protected WebSocketClient(IWebSocket webSocket)
    {
        _webSocket = webSocket;
        _webSocket.Connecting += async () =>
        {
            InvokeLog(LogMessage.Info("Connecting"));
            await InvokeEventAsync(Connecting).ConfigureAwait(false);
        };
        _webSocket.Connected += async () =>
        {
            _token = (_tokenSource = new()).Token;
            InvokeLog(LogMessage.Info("Connected"));
            await InvokeEventAsync(Connected).ConfigureAwait(false);
        };
        _webSocket.Disconnected += async (closeStatus, description) =>
        {
            _tokenSource!.Cancel();
            InvokeLog(string.IsNullOrEmpty(description) ? LogMessage.Info("Disconnected") : LogMessage.Info("Disconnected", description.EndsWith('.') ? description[..^1] : description));
            var reconnect = Reconnect(closeStatus, description);
            var disconnectedTask = InvokeEventAsync(Disconnected, reconnect);
            if (reconnect)
                await ReconnectAsync().ConfigureAwait(false);
            await disconnectedTask.ConfigureAwait(false);
        };
        _webSocket.Closed += async () =>
        {
            _tokenSource!.Cancel();
            InvokeLog(LogMessage.Info("Closed"));
            await InvokeEventAsync(Closed).ConfigureAwait(false);
        };
        _webSocket.MessageReceived += async data =>
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
    }

    private protected readonly IWebSocket _webSocket;
    private protected readonly LatencyTimer _latencyTimer = new();
    private protected readonly ReconnectTimer _reconnectTimer = new();

    private protected CancellationTokenSource? _tokenSource;
    private protected CancellationToken _token;

    public TimeSpan? Latency => _latency;
    private TimeSpan? _latency;

    public event Func<TimeSpan, ValueTask>? LatencyUpdated;
    public event Func<ValueTask>? Resumed;
    public event Func<ValueTask>? Connecting;
    public event Func<ValueTask>? Connected;
    public event Func<bool, ValueTask>? Disconnected;
    public event Func<ValueTask>? Closed;
    public event Func<LogMessage, ValueTask>? Log;

    private protected Task CloseAsync(System.Net.WebSockets.WebSocketCloseStatus status)
    {
        _tokenSource!.Cancel();
        return _webSocket.CloseAsync(status);
    }

    private protected abstract bool Reconnect(System.Net.WebSockets.WebSocketCloseStatus? status, string? description);

    private protected async ValueTask ReconnectAsync()
    {
        while (true)
        {
            try
            {
                await _reconnectTimer.NextAsync(_token).ConfigureAwait(false);
            }
            catch
            {
                return;
            }
            try
            {
                await ResumeAsync().ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
    }

    private protected abstract Task ResumeAsync();

    private protected async void BeginHeartbeating(double interval)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(interval));
        while (true)
        {
            try
            {
                await timer.WaitForNextTickAsync(_token).ConfigureAwait(false);
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

    private protected async ValueTask InvokeEventAsync(Func<ValueTask>? @event)
    {
        if (@event != null)
        {
            try
            {
                await @event().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
    }

    private protected async ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, Func<T> dataFunc)
    {
        if (@event != null)
        {
            try
            {
                await @event(dataFunc()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
    }

    private protected async ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data)
    {
        if (@event != null)
        {
            try
            {
                await @event(data).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, out T dataField)
    {
        if (@event != null)
        {
            ValueTask task;
            try
            {
                task = @event(data);
                dataField = data;
            }
            catch (Exception ex)
            {
                dataField = data;
                InvokeLog(LogMessage.Error(ex));
                return default;
            }
            return AwaitEventAsync();

            async ValueTask AwaitEventAsync()
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
        }
        else
        {
            dataField = data;
            return default;
        }
    }

    private protected ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, out T? dataField) where T : struct
    {
        if (@event != null)
        {
            ValueTask task;
            try
            {
                task = @event(data);
                dataField = data;
            }
            catch (Exception ex)
            {
                dataField = data;
                InvokeLog(LogMessage.Error(ex));
                return default;
            }
            return AwaitEventAsync();

            async ValueTask AwaitEventAsync()
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
        }
        else
        {
            dataField = data;
            return default;
        }
    }

    private protected async ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, T data, Action<T> updateData)
    {
        if (@event != null)
        {
            ValueTask task;
            try
            {
                task = @event(data);
                updateData(data);
            }
            catch (Exception ex)
            {
                updateData(data);
                InvokeLog(LogMessage.Error(ex));
                return;
            }
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
        else
            updateData(data);
    }

    private protected async ValueTask InvokeEventAsync<T>(Func<T, ValueTask>? @event, Func<T> dataFunc, Action updateData)
    {
        if (@event != null)
        {
            ValueTask task;
            try
            {
                task = @event(dataFunc());
                updateData();
            }
            catch (Exception ex)
            {
                updateData();
                InvokeLog(LogMessage.Error(ex));
                return;
            }
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        }
        else
            updateData();
    }

    public virtual void Dispose()
    {
        _webSocket.Dispose();
        _tokenSource?.Dispose();
    }
}
