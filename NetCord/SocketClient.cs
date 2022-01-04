using System.Collections.Immutable;
using System.Text.Json;

using NetCord.WebSockets;

namespace NetCord;

public partial class SocketClient : IDisposable
{
    private readonly string _botToken;
    private readonly WebSocket _webSocket = new(new(Discord.GatewayUrl));
    private readonly ClientConfig _config;
    private readonly ReconnectTimer _reconnectTimer = new();
    private readonly LatencyTimer _latencyTimer = new();

    private CancellationTokenSource? _tokenSource;
    private CancellationToken _token;
    private bool _disposed;

    //internal Dictionary<DiscordId, Guild> _guilds = new();
    //internal Dictionary<DiscordId, DMChannel> _DMChannels = new();
    //internal Dictionary<DiscordId, GroupDMChannel> _groupDMChannels = new();

    public ImmutableDictionary<DiscordId, Guild> Guilds => _guilds;
    public ImmutableDictionary<DiscordId, DMChannel> DMChannels => _DMChannels;
    public ImmutableDictionary<DiscordId, GroupDMChannel> GroupDMChannels => _groupDMChannels;

    private ImmutableDictionary<DiscordId, Guild> _guilds = ImmutableDictionary.Create<DiscordId, Guild>();
    private ImmutableDictionary<DiscordId, DMChannel> _DMChannels = ImmutableDictionary.Create<DiscordId, DMChannel>();
    private ImmutableDictionary<DiscordId, GroupDMChannel> _groupDMChannels = ImmutableDictionary.Create<DiscordId, GroupDMChannel>();

    public event Action? Connecting;
    public event Action? Connected;
    public event Action? Disconnected;
    public event Action? Closed;
    public event Action? Ready;
    public event LogEventHandler? Log;
    public event MessageReceivedEventHandler? MessageReceived;
    public event InteractionCreatedEventHandler<MenuInteraction>? MenuInteractionCreated;
    public event InteractionCreatedEventHandler<ButtonInteraction>? ButtonInteractionCreated;
    //public event InteractionCreatedEventHandler<ApplicationCommandInteraction>? ApplicationCommandInteractionCreated;

    public delegate void LogEventHandler(string text, LogType type);
    public delegate void MessageReceivedEventHandler(UserMessage message);
    public delegate void InteractionCreatedEventHandler<T>(T interaction);

    /// <summary>
    /// Is <see langword="null"/> before <see cref="Ready"/> event
    /// </summary>
    public SelfUser? User { get; private set; }
    public string? SessionId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Application? Application { get; private set; }
    public ApplicationFlags? ApplicationFlags { get; private set; }
    public RestClient Rest { get; }
    public int? Latency { get; private set; }
    public int? HeartbeatInterval { get; private set; }

    //public IReadOnlyDictionary<DiscordId, Guild> Guilds
    //{
    //    get
    //    {
    //        lock (Guilds)
    //            return new Dictionary<DiscordId, Guild>(Guilds);
    //    }
    //}

    //public IReadOnlyDictionary<DiscordId, DMChannel> DMChannels
    //{
    //    get
    //    {
    //        lock (DMChannels)
    //            return new Dictionary<DiscordId, DMChannel>(DMChannels);
    //    }
    //}

    //public IReadOnlyDictionary<DiscordId, GroupDMChannel> GroupDMChannels
    //{
    //    get
    //    {
    //        lock (GroupDMChannels)
    //            return new Dictionary<DiscordId, GroupDMChannel>(GroupDMChannels);
    //    }
    //}

    public SocketClient(string token, TokenType tokenType)
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));
        SetupWebSocket();
        _botToken = token;
        _config = new();
        Rest = new(token, tokenType);
    }

    public SocketClient(string token, TokenType tokenType, ClientConfig? config) : this(token, tokenType)
    {
        if (config != null)
            _config = config;
    }

    private void SetupWebSocket()
    {
        _webSocket.Connecting += () =>
        {
            LogInfo("Connecting", LogType.Gateway);
            try
            {
                Connecting?.Invoke();
            }
            catch (Exception ex)
            {
                LogInfo(ex.Message, LogType.Exception);
            }
        };
        _webSocket.Connected += () =>
        {

            _token = (_tokenSource = new()).Token;
            LogInfo("Connected", LogType.Gateway);
            try
            {
                Connected?.Invoke();
            }
            catch (Exception ex)
            {
                LogInfo(ex.Message, LogType.Exception);
            }
        };
        _webSocket.Disconnected += async (closeStatus, description) =>
        {
            _tokenSource!.Cancel();
            LogInfo("Disconnected", LogType.Gateway);
            try
            {
                Disconnected?.Invoke();
            }
            catch (Exception ex)
            {
                LogInfo(ex.Message, LogType.Exception);
            }
            if (!string.IsNullOrEmpty(description))
                LogInfo(description[..^1], LogType.Exception);

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
                    LogInfo(ex.Message, LogType.Exception);
                }
            }
        };
        _webSocket.Closed += () =>
        {
            _tokenSource!.Cancel();
            LogInfo("Closed", LogType.Gateway);
            try
            {
                Closed?.Invoke();
            }
            catch (Exception ex)
            {
                LogInfo(ex.Message, LogType.Exception);
            }
        };
        _webSocket.MessageReceived += data =>
        {
            var json = JsonDocument.Parse(data);
            //Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions() { WriteIndented = true }));
            ProcessMessage(json);
        };
    }

    /// <summary>
    /// Connect the <see cref="SocketClient"/> to gateway
    /// </summary>
    /// <returns></returns>
    public async Task StartAsync()
    {
        ThrowIfDisposed();
        await _webSocket.ConnectAsync().ConfigureAwait(false);
        await SendIdentifyAsync().ConfigureAwait(false);
    }

    private Task SendIdentifyAsync()
    {
        _latencyTimer.Start();
        var authorizationMessage = $@"{{""op"":2,""d"":{{""token"":""{_botToken}"",""intents"":{(uint)_config.Intents},""properties"":{{""$os"":""linux"",""$browser"":""NetCord"",""$device"":""NetCord""}},""large_threshold"":250}}}}";
        return _webSocket.SendAsync(authorizationMessage, _token);
    }

    private async Task ResumeAsync()
    {
        await _webSocket.ConnectAsync().ConfigureAwait(false);

        _latencyTimer.Start();
        var resumeMessage = $@"{{""op"":6,""d"":{{""token"":""{_botToken}"",""session_id"":""{SessionId}"",""seq"":{SequenceNumber}}}}}";
        await _webSocket.SendAsync(resumeMessage, _token).ConfigureAwait(false);
    }

    private void LogInfo(string text, LogType type)
    {
        try
        {
            Log?.Invoke(text, type);
        }
        catch
        {
        }
    }

    /// <summary>
    /// Disconnect the <see cref="SocketClient"/> from gateway
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        ThrowIfDisposed();
        _tokenSource!.Cancel();
        await _webSocket.CloseAsync().ConfigureAwait(false);
    }

    private async void BeginHeartbeatAsync(JsonElement message)
    {
        int interval = message.GetProperty("d").GetProperty("heartbeat_interval").GetInt32();
        HeartbeatInterval = interval;

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(interval));
        while (true)
        {
            try
            {
                await timer.WaitForNextTickAsync(_token).ConfigureAwait(false);
                _latencyTimer.Start();
                await _webSocket.SendAsync($@"{{""op"":1,""d"":{SequenceNumber}}}", _token).ConfigureAwait(false);
            }
            catch
            {
                return;
            }
        }
    }

    /// <summary>
    /// Updates <see cref="SequenceNumber"/>
    /// </summary>
    /// <param name="element"></param>
    private void UpdateSequenceNumber(JsonElement element)
    {
        SequenceNumber = element.GetProperty("s").GetInt32();
    }

    public void Dispose()
    {
        _webSocket.Dispose();
        _tokenSource!.Dispose();
        _guilds = null!;
        _DMChannels = null!;
        _groupDMChannels = null!;
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SocketClient));
    }
}