using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.JsonModels;
using NetCord.JsonModels.EventArgs;
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

    public event Func<Task>? Connecting;
    public event Func<Task>? Connected;
    public event Func<Task>? Disconnected;
    public event Func<Task>? Closed;
    public event Func<ReadyEventArgs, Task>? Ready;
    public event LogEventHandler? Log;

    public event Func<GuildChannelEventArgs, Task>? GuildChannelCreate;
    public event Func<GuildChannelEventArgs, Task>? GuildChannelUpdate;
    public event Func<GuildChannelEventArgs, Task>? GuildChannelDelete;
    public event Func<ChannelPinsUpdateEventArgs, Task>? ChannelPinsUpdate;
    public event Func<GuildThreadEventArgs, Task>? GuildThreadCreate;
    public event Func<GuildThreadEventArgs, Task>? GuildThreadUpdate;
    public event Func<GuildThreadDeleteEventArgs, Task>? GuildThreadDelete;
    public event Func<ThreadListSyncEventArgs, Task>? ThreadListSync;
    public event Func<ThreadMemberUpdateEventArgs, Task>? ThreadMemberUpdate;
    public event Func<GuildThreadMembersUpdateEventArgs, Task>? GuildThreadMembersUpdate;
    public event Func<Guild, Task>? GuildCreate;
    public event Func<Guild, Task>? GuildUpdate;
    public event Func<GuildDeleteEventArgs, Task>? GuildDelete;
    public event Func<GuildBanEventArgs, Task>? GuildBanAdd;
    public event Func<GuildBanEventArgs, Task>? GuildBanRemove;
    public event Func<GuildEmojisUpdateEventArgs, Task>? GuildEmojisUpdate;
    public event Func<GuildStickersUpdateEventArgs, Task>? GuildStickersUpdate;
    public event Func<GuildIntegrationsUpdateEventArgs, Task>? GuildIntegrationsUpdate;
    public event Func<GuildUser, Task>? GuildUserAdd;
    public event Func<GuildUser, Task>? GuildUserUpdate;
    public event Func<GuildUserRemoveEventArgs, Task>? GuildUserRemove;
    public event Func<GuildUserChunkEventArgs, Task>? GuildUserChunk;
    public event Func<GuildRoleEventArgs, Task>? GuildRoleCreate;
    public event Func<GuildRoleEventArgs, Task>? GuildRoleUpdate;
    public event Func<GuildRoleDeleteEventArgs, Task>? GuildRoleDelete;
    public event Func<GuildScheduledEvent, Task>? GuildScheduledEventCreate;
    public event Func<GuildScheduledEvent, Task>? GuildScheduledEventUpdate;
    public event Func<GuildScheduledEvent, Task>? GuildScheduledEventDelete;
    public event Func<GuildScheduledEventUserEventArgs, Task>? GuildScheduledEventUserAdd;
    public event Func<GuildScheduledEventUserEventArgs, Task>? GuildScheduledEventUserRemove;
    public event Func<GuildIntegrationEventArgs, Task>? GuildIntegrationCreate;
    public event Func<GuildIntegrationEventArgs, Task>? GuildIntegrationUpdate;
    public event Func<GuildIntegrationDeleteEventArgs, Task>? GuildIntegrationDelete;
    public event Func<GuildInvite, Task>? GuildInviteCreate;
    public event Func<GuildInviteDeleteEventArgs, Task>? GuildInviteDelete;
    public event Func<Message, Task>? MessageCreate;
    public event Func<Message, Task>? MessageUpdate;
    public event Func<MessageDeleteEventArgs, Task>? MessageDelete;
    public event Func<MessageDeleteBulkEventArgs, Task>? MessageDeleteBulk;
    public event Func<MessageReactionAddEventArgs, Task>? MessageReactionAdd;
    public event Func<MessageReactionRemoveEventArgs, Task>? MessageReactionRemove;
    public event Func<MessageReactionRemoveAllEventArgs, Task>? MessageReactionRemoveAll;
    public event Func<MessageReactionRemoveEmojiEventArgs, Task>? MessageReactionRemoveEmoji;
    public event Func<Presence, Task>? PresenceUpdate;
    public event Func<TypingStartEventArgs, Task>? TypingStart;
    public event Func<SelfUser, Task>? CurrentUserUpdate;
    public event Func<VoiceState, Task>? VoiceStateUpdate;
    public event Func<VoiceServerUpdateEventArgs, Task>? VoiceServerUpdate;
    public event Func<WebhooksUpdateEventArgs, Task>? WebhooksUpdate;
    public event Func<Interaction, Task>? InteractionCreate;
    public event Func<StageInstance, Task>? StageInstanceCreate;
    public event Func<StageInstance, Task>? StageInstanceUpdate;
    public event Func<StageInstance, Task>? StageInstanceDelete;

    public delegate Task LogEventHandler(LogMessage message);
    //public delegate Task MessageReceivedEventHandler(Message message);
    //public delegate Task InteractionCreatedEventHandler(Interaction interaction);

    /// <summary>
    /// Is <see langword="null"/> before <see cref="Ready"/> event
    /// </summary>
    public SelfUser? User { get; private set; }
    public string? SessionId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Shard? Shard { get; private set; }
    public DiscordId? ApplicationId { get; private set; }
    public ApplicationFlags? ApplicationFlags { get; private set; }
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
        _webSocket.Connecting += async () =>
        {
            InvokeLog(LogMessage.Info("Connecting"));
            try
            {
                if (Connecting != null)
                    await Connecting.Invoke().ConfigureAwait(false);
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
                    await Connected.Invoke().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
        };
        _webSocket.Disconnected += async (closeStatus, description) =>
        {
            _tokenSource!.Cancel();
            InvokeLog(LogMessage.Info("Disconnected"));
            try
            {
                if (Disconnected != null)
                    await Disconnected.Invoke().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                InvokeLog(LogMessage.Error(ex));
            }
            if (!string.IsNullOrEmpty(description))
                InvokeLog(LogMessage.Info(description.EndsWith('.') ? description[..^1] : description));

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
            var json = JsonDocument.Parse(data);
            //Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions() { WriteIndented = true }));
            await ProcessMessage(json).ConfigureAwait(false);
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
    private async Task ProcessMessage(JsonDocument message)
    {
        var rootElement = message.RootElement;
        switch ((GatewayOpcode)rootElement.GetProperty("op").GetByte())
        {
            case GatewayOpcode.Dispatch:
                UpdateSequenceNumber(rootElement);
                await ProcessEvent(rootElement).ConfigureAwait(false);
                break;
            case GatewayOpcode.Heartbeat:
                break;
            case GatewayOpcode.Reconnect:
                InvokeLog(LogMessage.Info("Reconnect request"));
                await _webSocket.CloseAsync().ConfigureAwait(false);
                _ = ResumeAsync();
                break;
            case GatewayOpcode.InvalidSession:
                InvokeLog(LogMessage.Info("Invalid session"));
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
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessEvent(JsonElement jsonElement)
    {
        var d = jsonElement.GetProperty("d");
        switch (jsonElement.GetProperty("t").GetString())
        {
            case "READY":
                {
                    Latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    ReadyEventArgs args = new(d.ToObject<JsonReadyEventArgs>(), this);
                    if (Ready != null)
                    {
                        try
                        {
                            await Ready(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    User = args.User;
                    foreach (var channel in args.DMChannels)
                    {
                        if (channel is GroupDMChannel groupDm)
                            _groupDMChannels = _groupDMChannels.Add(groupDm.Id, groupDm);
                        else if (channel is DMChannel dm)
                            _DMChannels = _DMChannels.Add(dm.Id, dm);
                    }
                    SessionId = args.SessionId;
                    Shard = args.Shard;
                    ApplicationId = args.ApplicationId;
                    ApplicationFlags = args.ApplicationFlags;
                    InvokeLog(LogMessage.Info("Ready"));
                    if (!_readyCompletionSource.Task.IsCompleted)
                        _readyCompletionSource.SetResult();
                }
                break;
            case "RESUMED":
                {
                    Latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    InvokeLog(LogMessage.Info("Resumed previous session"));
                }
                break;
            case "CHANNEL_CREATE":
                {
                    var channel = (IGuildChannel)Channel.CreateFromJson(d.ToObject<JsonChannel>(), Rest);
                    var guildId = GetGuildId();
                    if (GuildChannelCreate != null)
                    {
                        try
                        {
                            await GuildChannelCreate(new(channel, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(guildId, out var guild))
                        guild._channels = guild._channels.SetItem(channel.Id, channel);
                }
                break;
            case "CHANNEL_UPDATE":
                {
                    var channel = (IGuildChannel)Channel.CreateFromJson(d.ToObject<JsonChannel>(), Rest);
                    var guildId = GetGuildId();
                    if (GuildChannelUpdate != null)
                    {
                        try
                        {
                            await GuildChannelUpdate(new(channel, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(guildId, out var guild))
                        guild._channels = guild._channels.SetItem(channel.Id, channel);
                }
                break;
            case "CHANNEL_DELETE":
                {
                    var channel = (IGuildChannel)Channel.CreateFromJson(d.ToObject<JsonChannel>(), Rest);
                    var guildId = GetGuildId();
                    if (GuildChannelDelete != null)
                    {
                        try
                        {
                            await GuildChannelDelete(new(channel, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(guildId, out var guild))
                        guild._channels = guild._channels.Remove(channel.Id);
                }
                break;
            case "CHANNEL_PINS_UPDATE":
                {
                    if (ChannelPinsUpdate != null)
                    {
                        try
                        {
                            await ChannelPinsUpdate(new(d.ToObject<JsonChannelPinsUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "THREAD_CREATE":
                {
                    var thread = (GuildThread)Channel.CreateFromJson(d.ToObject<JsonChannel>(), Rest);
                    var guildId = GetGuildId();
                    if (GuildThreadCreate != null)
                    {
                        try
                        {
                            await GuildThreadCreate(new(thread, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(guildId, out var guild))
                        guild._activeThreads = guild._activeThreads.SetItem(thread.Id, thread);
                }
                break;
            case "THREAD_UPDATE":
                {
                    var thread = (GuildThread)Channel.CreateFromJson(d.ToObject<JsonChannel>(), Rest);
                    var guildId = GetGuildId();
                    if (GuildThreadUpdate != null)
                    {
                        try
                        {
                            await GuildThreadUpdate(new(thread, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(guildId, out var guild))
                        guild._activeThreads = guild._activeThreads.SetItem(thread.Id, thread);
                }
                break;
            case "THREAD_DELETE":
                {
                    var jsonThread = d.ToObject<JsonChannel>();
                    var guildId = GetGuildId();
                    if (GuildThreadDelete != null)
                    {
                        try
                        {
                            await GuildThreadDelete(new(jsonThread.Id, guildId, jsonThread.ParentId.GetValueOrDefault(), jsonThread.Type)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(guildId, out var guild))
                        guild._activeThreads = guild._activeThreads.Remove(jsonThread.Id);
                }
                break;
            case "THREAD_LIST_SYNC":
                {
                    ThreadListSyncEventArgs args = new(d.ToObject<JsonThreadListSyncEventArgs>(), Rest);
                    var guildId = GetGuildId();
                    if (ThreadListSync != null)
                    {
                        try
                        {
                            await ThreadListSync(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(guildId, out var guild))
                        guild._activeThreads = args.Threads;
                }
                break;
            case "THREAD_MEMBER_UPDATE":
                {
                    if (ThreadMemberUpdate != null)
                    {
                        try
                        {
                            await ThreadMemberUpdate(new(new(d.ToObject<JsonThreadUser>(), Rest), GetGuildId())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "THREAD_MEMBERS_UPDATE":
                {
                    if (GuildThreadMembersUpdate != null)
                    {
                        try
                        {
                            await GuildThreadMembersUpdate(new(d.ToObject<JsonGuildThreadMembersUpdateEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "GUILD_CREATE":
                {
                    Guild guild = new(d.ToObject<JsonGuild>(), Rest);
                    if (GuildCreate != null)
                    {
                        try
                        {
                            await GuildCreate(guild).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    _guilds = _guilds.SetItem(guild.Id, guild);
                }
                break;
            case "GUILD_UPDATE":
                {
                    var guildId = GetGuildId();
                    if (_guilds.TryGetValue(guildId, out var oldGuild))
                    {
                        Guild guild = new(d.ToObject<JsonGuild>(), oldGuild);
                        if (GuildUpdate != null)
                        {
                            try
                            {
                                await GuildUpdate(guild).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                InvokeLog(LogMessage.Error(ex));
                            }
                        }
                        _guilds = _guilds.SetItem(guildId, guild);
                    }
                }
                break;
            case "GUILD_DELETE":
                {
                    var guildId = GetGuildId();
                    if (GuildDelete != null)
                    {
                        try
                        {
                            await GuildDelete(new(guildId, !d.TryGetProperty("unavailable", out _))).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    _guilds = _guilds.Remove(guildId);
                }
                break;
            case "GUILD_BAN_ADD":
                {
                    if (GuildBanAdd != null)
                    {
                        try
                        {
                            await GuildBanAdd(new(d.ToObject<JsonGuildBanEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "GUILD_BAN_REMOVE":
                {
                    if (GuildBanRemove != null)
                    {
                        try
                        {
                            await GuildBanRemove(new(d.ToObject<JsonGuildBanEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "GUILD_EMOJIS_UPDATE":
                {
                    GuildEmojisUpdateEventArgs args = new(d.ToObject<JsonGuildEmojisUpdateEventArgs>(), Rest);
                    if (GuildEmojisUpdate != null)
                    {
                        try
                        {
                            await GuildEmojisUpdate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(args.GuildId, out var guild))
                        guild._emojis = args.Emojis;
                }
                break;
            case "GUILD_STICKERS_UPDATE":
                {
                    GuildStickersUpdateEventArgs args = new(d.ToObject<JsonGuildStickersUpdateEventArgs>(), Rest);
                    if (GuildStickersUpdate != null)
                    {
                        try
                        {
                            await GuildStickersUpdate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(args.GuildId, out var guild))
                        guild._stickers = args.Stickers;
                }
                break;
            case "GUILD_INTEGRATIONS_UPDATE":
                {
                    if (GuildIntegrationsUpdate != null)
                    {
                        try
                        {
                            await GuildIntegrationsUpdate(new(d.ToObject<JsonGuildIntegrationsUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "GUILD_MEMBER_ADD":
                {
                    GuildUser user = new(d.ToObject<JsonGuildUser>(), GetGuildId(), Rest);
                    if (GuildUserAdd != null)
                    {
                        try
                        {
                            await GuildUserAdd(user).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(user.GuildId, out var guild))
                        guild._users = guild._users.Remove(user.Id);
                }
                break;
            case "GUILD_MEMBER_UPDATE":
                {
                    GuildUser user = new(d.ToObject<JsonGuildUser>(), GetGuildId(), Rest);
                    if (GuildUserUpdate != null)
                    {
                        try
                        {
                            await GuildUserUpdate(user).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(user.GuildId, out var guild))
                        guild._users = guild._users.SetItem(user.Id, user);
                }
                break;
            case "GUILD_MEMBER_REMOVE":
                {
                    GuildUserRemoveEventArgs args = new(d.ToObject<JsonGuildUserRemoveEventArgs>(), Rest);
                    if (GuildUserRemove != null)
                    {
                        try
                        {
                            await GuildUserRemove(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(args.GuildId, out var guild))
                        guild._users = guild._users.Remove(args.User.Id);
                }
                break;
            case "GUILD_MEMBERS_CHUNK":
                {
                    GuildUserChunkEventArgs args = new(d.ToObject<JsonGuildUserChunkEventArgs>(), Rest);
                    if (GuildUserChunk != null)
                    {
                        try
                        {
                            await GuildUserChunk(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild._users = guild._users.SetItems(args.Users);
                        if (args.Presences != null)
                            guild._presences = guild._presences.SetItems(args.Presences);
                    }
                }
                break;
            case "GUILD_ROLE_CREATE":
                {
                    GuildRoleEventArgs args = new(d.ToObject<JsonGuildRoleEventArgs>(), Rest);
                    if (GuildRoleCreate != null)
                    {
                        try
                        {
                            await GuildRoleCreate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(args.GuildId, out var guild))
                        guild.Roles.SetItem(args.Role.Id, args.Role);
                }
                break;
            case "GUILD_ROLE_UPDATE":
                {
                    GuildRoleEventArgs args = new(d.ToObject<JsonGuildRoleEventArgs>(), Rest);
                    if (GuildRoleUpdate != null)
                    {
                        try
                        {
                            await GuildRoleUpdate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(args.GuildId, out var guild))
                        guild.Roles.SetItem(args.Role.Id, args.Role);
                }
                break;
            case "GUILD_ROLE_DELETE":
                {
                    GuildRoleDeleteEventArgs args = new(d.ToObject<JsonGuildRoleDeleteEventArgs>());
                    if (GuildRoleDelete != null)
                    {
                        try
                        {
                            await GuildRoleDelete(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(args.GuildId, out var guild))
                        guild.Roles.Remove(args.RoleId);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_CREATE":
                {
                    GuildScheduledEvent scheduledEvent = new(d.ToObject<JsonGuildScheduledEvent>(), Rest);
                    if (GuildScheduledEventCreate != null)
                    {
                        try
                        {
                            await GuildScheduledEventCreate(scheduledEvent).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                        guild._scheduledEvents = guild._scheduledEvents.SetItem(scheduledEvent.Id, scheduledEvent);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_UPDATE":
                {
                    GuildScheduledEvent scheduledEvent = new(d.ToObject<JsonGuildScheduledEvent>(), Rest);
                    if (GuildScheduledEventUpdate != null)
                    {
                        try
                        {
                            await GuildScheduledEventUpdate(scheduledEvent).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                        guild._scheduledEvents = guild._scheduledEvents.SetItem(scheduledEvent.Id, scheduledEvent);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_DELETE":
                {
                    GuildScheduledEvent scheduledEvent = new(d.ToObject<JsonGuildScheduledEvent>(), Rest);
                    if (GuildScheduledEventDelete != null)
                    {
                        try
                        {
                            await GuildScheduledEventDelete(scheduledEvent).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                        guild._scheduledEvents = guild._scheduledEvents.Remove(scheduledEvent.Id);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_ADD":
                {
                    if (GuildScheduledEventUserAdd != null)
                    {
                        try
                        {
                            await GuildScheduledEventUserAdd(new(d.ToObject<JsonGuildScheduledEventUserEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                {
                    if (GuildScheduledEventUserRemove != null)
                    {
                        try
                        {
                            await GuildScheduledEventUserRemove(new(d.ToObject<JsonGuildScheduledEventUserEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "INTEGRATION_CREATE":
                {
                    if (GuildIntegrationCreate != null)
                    {
                        try
                        {
                            await GuildIntegrationCreate(new(new(d.ToObject<JsonIntegration>(), Rest), GetGuildId())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "INTEGRATION_UPDATE":
                {
                    if (GuildIntegrationUpdate != null)
                    {
                        try
                        {
                            await GuildIntegrationUpdate(new(new(d.ToObject<JsonIntegration>(), Rest), GetGuildId())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "INTEGRATION_DELETE":
                {
                    if (GuildIntegrationDelete != null)
                    {
                        try
                        {
                            await GuildIntegrationDelete(new(d.ToObject<JsonGuildIntegrationDeleteEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "INTERACTION_CREATE":
                {
                    JsonInteraction interaction = d.ToObject<JsonInteraction>();
                    if (interaction.GuildId.HasValue && interaction.ChannelId.HasValue)
                        await CacheChannelAsync(interaction.ChannelId.GetValueOrDefault()).ConfigureAwait(false);

                    if (InteractionCreate != null)
                    {
                        try
                        {
                            await InteractionCreate.Invoke(Interaction.CreateFromJson(interaction, this)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "INVITE_CREATE":
                {
                    if (GuildInviteCreate != null)
                    {
                        try
                        {
                            await GuildInviteCreate(new(d.ToObject<JsonGuildInvite>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "INVITE_DELETE":
                {
                    if (GuildInviteDelete != null)
                    {
                        try
                        {
                            await GuildInviteDelete(new(d.ToObject<JsonGuildInviteDeleteEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_CREATE":
                {
                    var jsonMessage = d.ToObject<JsonMessage>();
                    if (!jsonMessage.GuildId.HasValue)
                        await CacheChannelAsync(jsonMessage.ChannelId).ConfigureAwait(false);

                    if (MessageCreate != null)
                    {
                        try
                        {
                            await MessageCreate(new(jsonMessage, this)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_UPDATE":
                {
                    var jsonMessage = d.ToObject<JsonMessage>();
                    if (!jsonMessage.GuildId.HasValue)
                        await CacheChannelAsync(jsonMessage.ChannelId).ConfigureAwait(false);

                    if (MessageUpdate != null)
                    {
                        try
                        {
                            await MessageUpdate(new(jsonMessage, this)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_DELETE":
                {
                    if (MessageDelete != null)
                    {
                        try
                        {
                            await MessageDelete(new(d.ToObject<JsonMessageDeleteEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_DELETE_BULK":
                {
                    if (MessageDeleteBulk != null)
                    {
                        try
                        {
                            await MessageDeleteBulk(new(d.ToObject<JsonMessageDeleteBulkEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_REACTION_ADD":
                {
                    if (MessageReactionAdd != null)
                    {
                        try
                        {
                            await MessageReactionAdd(new(d.ToObject<JsonMessageReactionAddEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_REACTION_REMOVE":
                {
                    if (MessageReactionRemove != null)
                    {
                        try
                        {
                            await MessageReactionRemove(new(d.ToObject<JsonMessageReactionRemoveEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_REACTION_REMOVE_ALL":
                {
                    if (MessageReactionRemoveAll != null)
                    {
                        try
                        {
                            await MessageReactionRemoveAll(new(d.ToObject<JsonMessageReactionRemoveAllEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                {
                    if (MessageReactionRemoveEmoji != null)
                    {
                        try
                        {
                            await MessageReactionRemoveEmoji(new(d.ToObject<JsonMessageReactionRemoveEmojiEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "PRESENCE_UPDATE":
                {
                    Presence presence = new(d.ToObject<JsonPresence>(), Rest);
                    if (PresenceUpdate != null)
                    {
                        try
                        {
                            await PresenceUpdate(presence).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(presence.GuildId.GetValueOrDefault(), out var guild))
                        guild._presences = guild._presences.SetItem(presence.User.Id, presence);
                }
                break;
            case "STAGE_INSTANCE_CREATE":
                {
                    StageInstance stageInstance = new(d.ToObject<JsonStageInstance>(), Rest);
                    if (StageInstanceCreate != null)
                    {
                        try
                        {
                            await StageInstanceCreate(stageInstance).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(stageInstance.GuildId, out var guild))
                        guild._stageInstances = guild._stageInstances.SetItem(stageInstance.Id, stageInstance);
                }
                break;
            case "STAGE_INSTANCE_UPDATE":
                {
                    StageInstance stageInstance = new(d.ToObject<JsonStageInstance>(), Rest);
                    if (StageInstanceUpdate != null)
                    {
                        try
                        {
                            await StageInstanceUpdate(stageInstance).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(stageInstance.GuildId, out var guild))
                        guild._stageInstances = guild._stageInstances.SetItem(stageInstance.Id, stageInstance);
                }
                break;
            case "STAGE_INSTANCE_DELETE":
                {
                    StageInstance stageInstance = new(d.ToObject<JsonStageInstance>(), Rest);
                    if (StageInstanceDelete != null)
                    {
                        try
                        {
                            await StageInstanceDelete(stageInstance).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(stageInstance.GuildId, out var guild))
                        guild._stageInstances = guild._stageInstances.Remove(stageInstance.Id);
                }
                break;
            case "TYPING_START":
                {
                    if (TypingStart != null)
                    {
                        try
                        {
                            await TypingStart(new(d.ToObject<JsonTypingStartEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "USER_UPDATE":
                {
                    SelfUser user = new(d.ToObject<JsonUser>(), Rest);
                    if (CurrentUserUpdate != null)
                    {
                        try
                        {
                            await CurrentUserUpdate(user).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "VOICE_STATE_UPDATE":
                {
                    VoiceState voiceState = new(d.ToObject<JsonVoiceState>());
                    if (VoiceStateUpdate != null)
                    {
                        try
                        {
                            await VoiceStateUpdate(voiceState).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                    if (TryGetGuild(voiceState.GuildId.GetValueOrDefault(), out var guild))
                    {
                        if (voiceState.ChannelId.HasValue)
                            guild._voiceStates = guild._voiceStates.Remove(voiceState.UserId);
                        else
                            guild._voiceStates = guild._voiceStates.SetItem(voiceState.UserId, voiceState);
                    }
                }
                break;
            case "VOICE_SERVER_UPDATE":
                {
                    if (VoiceServerUpdate != null)
                    {
                        try
                        {
                            await VoiceServerUpdate(new(d.ToObject<JsonVoiceServerUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "WEBHOOKS_UPDATE":
                {
                    if (WebhooksUpdate != null)
                    {
                        try
                        {
                            await WebhooksUpdate(new(d.ToObject<JsonWebhooksUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        DiscordId GetGuildId() => d.GetProperty("guild_id").ToObject<DiscordId>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryGetGuild(DiscordId guildId, [NotNullWhen(true)] out Guild? guild) => Guilds.TryGetValue(guildId, out guild);
    }

    private async Task CacheChannelAsync(DiscordId channelId)
    {
        if (!_DMChannels.ContainsKey(channelId) && !_groupDMChannels.ContainsKey(channelId))
        {
            var channel = await Rest.GetChannelAsync(channelId).ConfigureAwait(false);
            if (channel is GroupDMChannel groupDMChannel)
                _groupDMChannels = _groupDMChannels.SetItem(channelId, groupDMChannel);
            else if (channel is DMChannel dMChannel)
                _DMChannels = _DMChannels.SetItem(channelId, dMChannel);
        }
    }

    private async void InvokeLog(LogMessage logMessage)
    {
        try
        {
            if (Log != null)
                await Log.Invoke(logMessage).ConfigureAwait(false);
        }
        catch
        {
        }
    }
}