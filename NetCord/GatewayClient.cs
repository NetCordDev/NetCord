using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.JsonModels;
using NetCord.WebSockets;

namespace NetCord;

public partial class GatewayClient : IDisposable
{
    private readonly string _botToken;

    private readonly WebSocket _webSocket = new(new(Discord.GatewayUrl));

    private readonly GatewayClientConfig _config;
    private readonly ReconnectTimer _reconnectTimer = new();
    private readonly LatencyTimer _latencyTimer = new();

    private CancellationTokenSource? _tokenSource;
    private CancellationToken _token;
    private bool _disposed;

    public ImmutableDictionary<DiscordId, Guild> Guilds => _guilds;
    public ImmutableDictionary<DiscordId, DMChannel> DMChannels => _DMChannels;
    public ImmutableDictionary<DiscordId, GroupDMChannel> GroupDMChannels => _groupDMChannels;

    private ImmutableDictionary<DiscordId, Guild> _guilds = CollectionsUtils.CreateImmutableDictionary<DiscordId, Guild>();
    private ImmutableDictionary<DiscordId, DMChannel> _DMChannels = CollectionsUtils.CreateImmutableDictionary<DiscordId, DMChannel>();
    private ImmutableDictionary<DiscordId, GroupDMChannel> _groupDMChannels = CollectionsUtils.CreateImmutableDictionary<DiscordId, GroupDMChannel>();

    public event Action? Connecting;
    public event Action? Connected;
    public event Action? Disconnected;
    public event Action? Closed;
    public event Action? Ready;
    public event LogEventHandler? Log;
    public event MessageReceivedEventHandler? MessageReceived;
    public event InteractionCreatedEventHandler? InteractionCreated;

    public delegate void LogEventHandler(string text, LogType type);
    public delegate void MessageReceivedEventHandler(Message message);
    public delegate void InteractionCreatedEventHandler(Interaction interaction);

    /// <summary>
    /// Is <see langword="null"/> before <see cref="Ready"/> event
    /// </summary>
    public SelfUser? User { get; private set; }
    public string? SessionId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Application? Application { get; private set; }
    public RestClient Rest { get; }
    public int? Latency { get; private set; }
    public int? HeartbeatInterval { get; private set; }

    public Task ReadyAsync => _readyCompletionSource!.Task;
    private readonly TaskCompletionSource _readyCompletionSource = new();

    public GatewayClient(string token, TokenType tokenType)
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));
        SetupWebSocket();
        _botToken = token;
        _config = new();
        Rest = new(token, tokenType);
    }

    public GatewayClient(string token, TokenType tokenType, GatewayClientConfig? config) : this(token, tokenType)
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
                LogInfo(description.EndsWith('.') ? description[..^1] : description, LogType.Exception);

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
    /// Connects the <see cref="GatewayClient"/> to gateway
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
        PayloadProperties<IdentifyProperties> payload = new(GatewayOpcode.Identify, new(_botToken)
        {
            LargeThreshold = _config.LargeThreshold,
            Shard = _config.Shard,
            Presence = _config.Presence,
            Intents = _config.Intents,
        });
        return _webSocket.SendAsync(payload.Serialize(), _token);
    }

    private async Task ResumeAsync()
    {
        await _webSocket.ConnectAsync().ConfigureAwait(false);

        _latencyTimer.Start();
        PayloadProperties<ResumeProperties> payload = new(GatewayOpcode.Resume, new(_botToken, SessionId!, SequenceNumber));
        await _webSocket.SendAsync(payload.Serialize(), _token).ConfigureAwait(false);
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
    /// Disconnects the <see cref="GatewayClient"/> from gateway
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
                PayloadProperties<int> payload = new(GatewayOpcode.Heartbeat, SequenceNumber);
                await _webSocket.SendAsync(payload.Serialize(), _token).ConfigureAwait(false);
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
            throw new ObjectDisposedException(nameof(GatewayClient));
    }

    public Task UpdateVoiceStateAsync(VoiceStateProperties voiceState)
    {
        PayloadProperties<VoiceStateProperties> payload = new(GatewayOpcode.VoiceStateUpdate, voiceState);
        return _webSocket.SendAsync(payload.Serialize(), _token);
    }

    public Task UpdatePresenceAsync(PresenceProperties presence)
    {
        PayloadProperties<PresenceProperties> payload = new(GatewayOpcode.PresenceUpdate, presence);
        return _webSocket.SendAsync(payload.Serialize(), _token);
    }

    public Task RequestGuildUsersAsync(GuildUsersRequestProperties requestProperties)
    {
        PayloadProperties<GuildUsersRequestProperties> payload = new(GatewayOpcode.RequestGuildUsers, requestProperties);
        return _webSocket.SendAsync(payload.Serialize(), _token);
    }

    /// <summary>
    /// Runs an action based on a <paramref name="message"/>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private async void ProcessMessage(JsonDocument message)
    {
        var rootElement = message.RootElement;
        switch ((GatewayOpcode)rootElement.GetProperty("op").GetByte())
        {
            case GatewayOpcode.Dispatch:
                UpdateSequenceNumber(rootElement);
                ProcessEvent(rootElement);
                break;
            case GatewayOpcode.Heartbeat:
                break;
            case GatewayOpcode.Reconnect:
                LogInfo("Reconnect request", LogType.Gateway);
                await _webSocket.CloseAsync().ConfigureAwait(false);
                _ = ResumeAsync();
                break;
            case GatewayOpcode.InvalidSession:
                LogInfo("Invalid session", LogType.Gateway);
                _ = SendIdentifyAsync();
                break;
            case GatewayOpcode.Hello:
                BeginHeartbeatAsync(rootElement);
                break;
            case GatewayOpcode.HeartbeatACK:
                Latency = _latencyTimer.Elapsed;
                break;
        }
    }

    /// <summary>
    /// Runs an action based on a <paramref name="jsonElement"/>
    /// </summary>
    /// <param name="jsonElement"></param>
    /// <returns></returns>
    private async void ProcessEvent(JsonElement jsonElement)
    {
        switch (jsonElement.GetProperty("t").GetString())
        {
            case "READY":
                Latency = _latencyTimer.Elapsed;
                _reconnectTimer.Reset();
                Ready ready = new(GetD().ToObject<JsonReady>(), this);
                User = ready.User;
                foreach (var channel in ready.DMChannels)
                {
                    if (channel is GroupDMChannel groupDm)
                        _groupDMChannels = _groupDMChannels.Add(groupDm.Id, groupDm);
                    else if (channel is DMChannel dm)
                        _DMChannels = _DMChannels.Add(dm.Id, dm);
                }
                SessionId = ready.SessionId;
                Application = ready.Application;
                LogInfo("Ready", LogType.Gateway);
                if (!_readyCompletionSource.Task.IsCompleted)
                    _readyCompletionSource.SetResult();

                try
                {
                    Ready?.Invoke();
                }
                catch (Exception ex)
                {
                    LogInfo(ex.Message, LogType.Exception);
                }
                break;
            case "RESUMED":
                Latency = _latencyTimer.Elapsed;
                _reconnectTimer.Reset();
                LogInfo("Resumed previous session", LogType.Gateway);
                break;
            case "CHANNEL_CREATE":
            case "CHANNEL_UPDATE":
                var property = GetD();
                if (TryGetGuild(property, out var g))
                    AddOrUpdate<JsonChannel, IGuildChannel>(property, ref g._channels, (ch, c) => (IGuildChannel)Channel.CreateFromJson(ch, c));
                break;
            case "CHANNEL_DELETE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    TryRemove(property, ref g._channels);
                break;
            case "CHANNEL_PINS_UPDATE":
                break;
            case "THREAD_CREATE":
            case "THREAD_UPDATE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    AddOrUpdate(property, ref g._activeThreads, (JsonChannel ch, RestClient c) => (Thread)Channel.CreateFromJson(ch, c));
                break;
            case "THREAD_DELETE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    TryRemove(property, ref g._activeThreads);
                break;
            case "THREAD_LIST_SYNC":
                break;
            case "THREAD_MEMBER_UPDATE":
                break;
            case "THREAD_MEMBERS_UPDATE ":
                break;
            case "GUILD_CREATE":
                AddOrUpdate(GetD(), ref _guilds, (JsonGuild j, RestClient c) => new Guild(j, c));
                break;
            case "GUILD_UPDATE":
                AddOrUpdate(GetD(), ref _guilds, (JsonGuild j, Guild g, RestClient _) => new Guild(j, g));
                break;
            case "GUILD_DELETE":
                TryRemove(GetD(), ref _guilds);
                break;
            case "GUILD_BAN_ADD":
                break;
            case "GUILD_BAN_REMOVE":
                break;
            case "GUILD_EMOJIS_UPDATE":
                break;
            case "GUILD_STICKERS_UPDATE":
                break;
            case "GUILD_INTEGRATIONS_UPDATE":
                break;
            case "GUILD_MEMBER_ADD":
                property = GetD();
                if (TryGetGuild(property, out g))
                    AddOrUpdate(property, ref g._users, (JsonGuildUser u, RestClient c) => new GuildUser(u, g, c));
                break;
            case "GUILD_MEMBER_UPDATE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    AddOrUpdate(property, ref g._users, (JsonGuildUser u, RestClient c) => new GuildUser(u, g, c));
                break;
            case "GUILD_MEMBER_REMOVE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    TryRemove(property, ref g._users);
                break;
            case "GUILD_MEMBERS_CHUNK":
                property = GetD();
                if (TryGetGuild(property, out g))
                    g._users = g._users.SetItems(property.GetProperty("members").EnumerateArray().Select(u => new GuildUser(u.ToObject<JsonGuildUser>(), g.Id, Rest)).Select(u => KeyValuePair.Create(u.Id, u)));
                break;
            case "GUILD_ROLE_CREATE":
            case "GUILD_ROLE_UPDATE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    AddOrUpdate(property.GetProperty("role"), ref g._roles, (JsonGuildRole r, RestClient c) => new GuildRole(r, c));
                break;
            case "GUILD_ROLE_DELETE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    TryRemove(property, ref g._roles, "role_id");
                break;
            case "GUILD_SCHEDULED_EVENT_CREATE":
                break;
            case "GUILD_SCHEDULED_EVENT_UPDATE":
                break;
            case "GUILD_SCHEDULED_EVENT_DELETE":
                break;
            case "GUILD_SCHEDULED_EVENT_USER_ADD":
                break;
            case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                break;
            case "INTEGRATION_CREATE":
                break;
            case "INTEGRATION_UPDATE":
                break;
            case "INTEGRATION_DELETE":
                break;
            case "INTERACTION_CREATE":
                JsonInteraction interaction = GetD().ToObject<JsonInteraction>();
                if (interaction.GuildId == null && interaction.ChannelId.HasValue)
                    await CacheChannelAsync(interaction.ChannelId.GetValueOrDefault()).ConfigureAwait(false);

                try
                {
                    InteractionCreated?.Invoke(Interaction.CreateFromJson(interaction, this));
                }
                catch (Exception ex)
                {
                    LogInfo(ex.Message, LogType.Exception);
                }
                break;
            case "INVITE_CREATE":
                break;
            case "INVITE_DELETE":
                break;
            case "MESSAGE_CREATE":
                property = GetD();
                var jsonMessage = property.ToObject<JsonMessage>();
                if (!jsonMessage.GuildId.HasValue)
                    await CacheChannelAsync(jsonMessage.ChannelId).ConfigureAwait(false);

                try
                {
                    MessageReceived?.Invoke(new(jsonMessage, this));
                }
                catch (Exception ex)
                {
                    LogInfo(ex.Message, LogType.Exception);
                }
                break;
            case "MESSAGE_UPDATE":
                break;
            case "MESSAGE_DELETE":
                break;
            case "MESSAGE_DELETE_BULK":
                break;
            case "MESSAGE_REACTION_ADD":
                break;
            case "MESSAGE_REACTION_REMOVE":
                break;
            case "MESSAGE_REACTION_REMOVE_ALL":
                break;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                break;
            case "PRESENCE_UPDATE":
                break;
            case "STAGE_INSTANCE_CREATE":
            case "STAGE_INSTANCE_UPDATE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    AddOrUpdate(property, ref g._stageInstances, (JsonStageInstance i, RestClient c) => new StageInstance(i, c));
                break;
            case "STAGE_INSTANCE_DELETE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    TryRemove(property, ref g._stageInstances);
                break;
            case "TYPING_START":
                break;
            case "USER_UPDATE":
                User = new(GetD().ToObject<JsonUser>(), Rest);
                break;
            case "VOICE_STATE_UPDATE":
                property = GetD();
                if (TryGetGuild(property, out g))
                    AddUpdateOrDelete(property, ref g._voiceStates);
                break;
            case "VOICE_SERVER_UPDATE":
                break;
            case "WEBHOOKS_UPDATE":
                break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        JsonElement GetD() => jsonElement.GetProperty("d");
    }

    private async Task CacheChannelAsync(DiscordId channelId)
    {
        if (!_DMChannels.ContainsKey(channelId) && !_groupDMChannels.ContainsKey(channelId))
        {
            var channel = await Rest.Channel.GetAsync(channelId).ConfigureAwait(false);
            if (channel is GroupDMChannel groupDMChannel)
                _groupDMChannels = _groupDMChannels.SetItem(channelId, groupDMChannel);
            else if (channel is DMChannel dMChannel)
                _DMChannels = _DMChannels.SetItem(channelId, dMChannel);
        }
    }

    private bool TryGetGuild(JsonElement jsonElement, [NotNullWhen(true)] out Guild? guild) => Guilds.TryGetValue(jsonElement.GetProperty("guild_id").ToObject<DiscordId>(), out guild);

    private TType AddOrUpdate<TJsonType, TType>(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, TType> propertyToUpdate, Func<TJsonType, RestClient, TType> constructor) where TType : IEntity
    {
        var jsonObj = jsonElement.ToObject<TJsonType>();
        TType obj = constructor(jsonObj, Rest);
        lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            propertyToUpdate = propertyToUpdate.SetItem(obj.Id, obj);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
        return obj;
    }

    private TType AddOrUpdate<TJsonType, TType>(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, TType> propertyToUpdate, Func<TJsonType, TType, RestClient, TType> constructor) where TType : IEntity where TJsonType : JsonEntity
    {
        var jsonObj = jsonElement.ToObject<TJsonType>();
        lock (propertyToUpdate)
        {
            TType obj = constructor(jsonObj, propertyToUpdate[jsonObj.Id], Rest);
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            propertyToUpdate = propertyToUpdate.SetItem(obj.Id, obj);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            return obj;
        }
    }

    private static VoiceState? AddUpdateOrDelete(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, VoiceState> propertyToUpdate)
    {
        var jsonObj = jsonElement.ToObject<JsonVoiceState>();
        if (jsonObj.ChannelId == null)
        {
            lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                propertyToUpdate = propertyToUpdate.Remove(jsonObj.UserId);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            return null;
        }
        else
        {
            VoiceState obj = new(jsonObj);
            lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                propertyToUpdate = propertyToUpdate.SetItem(jsonObj.UserId, obj);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            return obj;
        }
    }

    private static void TryRemove<T>(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, T> propertyToUpdate, string propertyName = "id")
    {
        DiscordId id = jsonElement.GetProperty(propertyName).ToObject<DiscordId>();
        lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            propertyToUpdate = propertyToUpdate.Remove(id);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
    }

    //private void AddOrUpdate(JsonElement jsonElement, Dictionary<DiscordId, GuildUser> propertyToUpdate)
    //{
    //    var jsonObj = jsonElement.ToObject<JsonGuildUser>();
    //    GuildUser obj = new(jsonObj, null, this);
    //    DiscordId id = obj.Id;
    //    obj.VoiceState = propertyToUpdate[id].VoiceState;
    //    lock (propertyToUpdate)
    //        propertyToUpdate[id] = obj;
    //}

    private static void TryRemove(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, GuildUser> propertyToUpdate)
    {
        DiscordId id = jsonElement.GetProperty("user").GetProperty("id").ToObject<DiscordId>();
        lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            propertyToUpdate = propertyToUpdate.Remove(id);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
    }
}