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
            try
            {
                if (Connecting != null)
                    await Connecting().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        };
        _webSocket.Connected += async () =>
        {
            _token = (_tokenSource = new()).Token;
            InvokeLog(LogMessage.Info("Connected"));
            try
            {
                if (Connected != null)
                    await Connected().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        };
        _webSocket.Disconnected += async (closeStatus, description) =>
        {
            _tokenSource!.Cancel();
            InvokeLog(string.IsNullOrEmpty(description) ? LogMessage.Info("Disconnected") : LogMessage.Info("Disconnected", description.EndsWith('.') ? description[..^1] : description));
            try
            {
                if (Disconnected != null)
                    await Disconnected().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
            await ReconnectAsync(closeStatus, description).ConfigureAwait(false);
        };
        _webSocket.Closed += async () =>
        {
            _tokenSource!.Cancel();
            InvokeLog(LogMessage.Info("Closed"));
            try
            {
                if (Closed != null)
                    await Closed.Invoke().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        };
        _webSocket.MessageReceived += async data =>
        {
            try
            {
                //json = JsonDocument.Parse(data);
                //Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions() { WriteIndented = true }));
                await ProcessMessageAsync(JsonSerializer.Deserialize<JsonPayload>(data.Span, ToObjectExtensions._options)!).ConfigureAwait(false);
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

    public int? Latency { get; private protected set; }

    public event Func<ValueTask>? Connecting;
    public event Func<ValueTask>? Connected;
    public event Func<ValueTask>? Disconnected;
    public event Func<ValueTask>? Closed;
    public event Func<LogMessage, ValueTask>? Log;

    private protected virtual async ValueTask ReconnectAsync(System.Net.WebSockets.WebSocketCloseStatus? status, string? description)
    {
        while (true)
        {
            await _reconnectTimer.NextAsync().ConfigureAwait(false);
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

    private protected abstract Task HeartbeatAsync();

    private protected abstract Task ResumeAsync();

    private protected abstract Task ProcessMessageAsync(JsonPayload payload);

    private protected async void InvokeLog(LogMessage logMessage)
    {
        try
        {
            if (Log != null)
                await Log(logMessage).ConfigureAwait(false);
        }
        catch
        {
        }
    }

    public virtual void Dispose()
    {
        _webSocket.Dispose();
        _tokenSource!.Dispose();
    }
}
