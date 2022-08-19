using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Gateway.WebSockets;
using NetCord.JsonModels;
using NetCord.JsonModels.EventArgs;

namespace NetCord.Gateway;

public partial class GatewayClient : WebSocketClient
{
    private readonly string _botToken;

    private readonly GatewayClientConfig _config;

    private bool _disposed;

    public ImmutableDictionary<Snowflake, Guild> Guilds { get; private set; } = CollectionsUtils.CreateImmutableDictionary<Snowflake, Guild>();
    public ImmutableDictionary<Snowflake, DMChannel> DMChannels { get; private set; } = CollectionsUtils.CreateImmutableDictionary<Snowflake, DMChannel>();
    public ImmutableDictionary<Snowflake, GroupDMChannel> GroupDMChannels { get; private set; } = CollectionsUtils.CreateImmutableDictionary<Snowflake, GroupDMChannel>();

    public event Func<ReadyEventArgs, ValueTask>? Ready;
    public event Func<ApplicationCommandPermission, ValueTask>? ApplicationCommandPermissionsUpdate;
    public event Func<AutoModerationRule, ValueTask>? AutoModerationRuleCreate;
    public event Func<AutoModerationRule, ValueTask>? AutoModerationRuleUpdate;
    public event Func<AutoModerationRule, ValueTask>? AutoModerationRuleDelete;
    public event Func<AutoModerationActionExecutionEventArgs, ValueTask>? AutoModerationActionExecution;
    public event Func<GuildChannelEventArgs, ValueTask>? GuildChannelCreate;
    public event Func<GuildChannelEventArgs, ValueTask>? GuildChannelUpdate;
    public event Func<GuildChannelEventArgs, ValueTask>? GuildChannelDelete;
    public event Func<ChannelPinsUpdateEventArgs, ValueTask>? ChannelPinsUpdate;
    public event Func<GuildThreadEventArgs, ValueTask>? GuildThreadCreate;
    public event Func<GuildThreadEventArgs, ValueTask>? GuildThreadUpdate;
    public event Func<GuildThreadDeleteEventArgs, ValueTask>? GuildThreadDelete;
    public event Func<ThreadListSyncEventArgs, ValueTask>? ThreadListSync;
    public event Func<ThreadUserUpdateEventArgs, ValueTask>? ThreadUserUpdate;
    public event Func<GuildThreadUsersUpdateEventArgs, ValueTask>? GuildThreadUsersUpdate;
    public event Func<GuildCreateEventArgs, ValueTask>? GuildCreate;
    public event Func<Guild, ValueTask>? GuildUpdate;
    public event Func<GuildDeleteEventArgs, ValueTask>? GuildDelete;
    public event Func<GuildBanEventArgs, ValueTask>? GuildBanAdd;
    public event Func<GuildBanEventArgs, ValueTask>? GuildBanRemove;
    public event Func<GuildEmojisUpdateEventArgs, ValueTask>? GuildEmojisUpdate;
    public event Func<GuildStickersUpdateEventArgs, ValueTask>? GuildStickersUpdate;
    public event Func<GuildIntegrationsUpdateEventArgs, ValueTask>? GuildIntegrationsUpdate;
    public event Func<GuildUser, ValueTask>? GuildUserAdd;
    public event Func<GuildUser, ValueTask>? GuildUserUpdate;
    public event Func<GuildUserRemoveEventArgs, ValueTask>? GuildUserRemove;
    public event Func<GuildUserChunkEventArgs, ValueTask>? GuildUserChunk;
    public event Func<GuildRoleEventArgs, ValueTask>? GuildRoleCreate;
    public event Func<GuildRoleEventArgs, ValueTask>? GuildRoleUpdate;
    public event Func<GuildRoleDeleteEventArgs, ValueTask>? GuildRoleDelete;
    public event Func<GuildScheduledEvent, ValueTask>? GuildScheduledEventCreate;
    public event Func<GuildScheduledEvent, ValueTask>? GuildScheduledEventUpdate;
    public event Func<GuildScheduledEvent, ValueTask>? GuildScheduledEventDelete;
    public event Func<GuildScheduledEventUserEventArgs, ValueTask>? GuildScheduledEventUserAdd;
    public event Func<GuildScheduledEventUserEventArgs, ValueTask>? GuildScheduledEventUserRemove;
    public event Func<GuildIntegrationEventArgs, ValueTask>? GuildIntegrationCreate;
    public event Func<GuildIntegrationEventArgs, ValueTask>? GuildIntegrationUpdate;
    public event Func<GuildIntegrationDeleteEventArgs, ValueTask>? GuildIntegrationDelete;
    public event Func<GuildInvite, ValueTask>? GuildInviteCreate;
    public event Func<GuildInviteDeleteEventArgs, ValueTask>? GuildInviteDelete;
    public event Func<Message, ValueTask>? MessageCreate;
    public event Func<Message, ValueTask>? MessageUpdate;
    public event Func<MessageDeleteEventArgs, ValueTask>? MessageDelete;
    public event Func<MessageDeleteBulkEventArgs, ValueTask>? MessageDeleteBulk;
    public event Func<MessageReactionAddEventArgs, ValueTask>? MessageReactionAdd;
    public event Func<MessageReactionRemoveEventArgs, ValueTask>? MessageReactionRemove;
    public event Func<MessageReactionRemoveAllEventArgs, ValueTask>? MessageReactionRemoveAll;
    public event Func<MessageReactionRemoveEmojiEventArgs, ValueTask>? MessageReactionRemoveEmoji;
    public event Func<Presence, ValueTask>? PresenceUpdate;
    public event Func<TypingStartEventArgs, ValueTask>? TypingStart;
    public event Func<SelfUser, ValueTask>? CurrentUserUpdate;
    public event Func<VoiceState, ValueTask>? VoiceStateUpdate;
    public event Func<VoiceServerUpdateEventArgs, ValueTask>? VoiceServerUpdate;
    public event Func<WebhooksUpdateEventArgs, ValueTask>? WebhooksUpdate;
    public event Func<Interaction, ValueTask>? InteractionCreate;
    public event Func<StageInstance, ValueTask>? StageInstanceCreate;
    public event Func<StageInstance, ValueTask>? StageInstanceUpdate;
    public event Func<StageInstance, ValueTask>? StageInstanceDelete;

    /// <summary>
    /// Is <see langword="null"/> before <see cref="Ready"/> event
    /// </summary>
    public SelfUser? User { get; private set; }
    public string? SessionId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Shard? Shard { get; private set; }
    public Snowflake? ApplicationId { get; private set; }
    public ApplicationFlags? ApplicationFlags { get; private set; }
    public Rest.RestClient Rest { get; }

    public Task ReadyAsync => _readyCompletionSource!.Task;
    private readonly TaskCompletionSource _readyCompletionSource = new();

    public GatewayClient(Token token, GatewayClientConfig? config = null) : base(config?.WebSocket ?? new WebSocket())
    {
        _botToken = token.RawToken;
        _config = config ?? new();
        Rest = new(token);
    }

    /// <summary>
    /// Connects the <see cref="GatewayClient"/> to gateway
    /// </summary>
    /// <returns></returns>
    public async Task StartAsync(PresenceProperties? presence = null)
    {
        ThrowIfDisposed();
        await _webSocket.ConnectAsync(new($"wss://gateway.discord.gg?v={(int)_config.Version}&encoding=json")).ConfigureAwait(false);
        await SendIdentifyAsync(presence).ConfigureAwait(false);
    }

    private Task SendIdentifyAsync(PresenceProperties? presence = null)
    {
        var serializedPayload = new GatewayPayloadProperties<GatewayIdentifyProperties>(GatewayOpcode.Identify, new(_botToken)
        {
            ConnectionProperties = _config.ConnectionProperties ?? ConnectionPropertiesProperties.Default,
            LargeThreshold = _config.LargeThreshold,
            Shard = _config.Shard,
            Presence = presence ?? _config.Presence,
            Intents = _config.Intents,
        }).Serialize();
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload, _token);
    }

    private protected override async Task ResumeAsync()
    {
        await _webSocket.ConnectAsync(new($"wss://gateway.discord.gg?v={(int)_config.Version}&encoding=json")).ConfigureAwait(false);

        var serializedPayload = new GatewayPayloadProperties<ResumeProperties>(GatewayOpcode.Resume, new(_botToken, SessionId!, SequenceNumber)).Serialize();
        _latencyTimer.Start();
        await _webSocket.SendAsync(serializedPayload, _token).ConfigureAwait(false);
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

    private protected override Task HeartbeatAsync()
    {
        var serializedPayload = new GatewayPayloadProperties<int>(GatewayOpcode.Heartbeat, SequenceNumber).Serialize();
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload, _token);
    }

    public override void Dispose()
    {
        Guilds = null!;
        DMChannels = null!;
        GroupDMChannels = null!;
        _disposed = true;
        base.Dispose();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GatewayClient));
    }

    public Task UpdateVoiceStateAsync(VoiceStateProperties voiceState)
    {
        GatewayPayloadProperties<VoiceStateProperties> payload = new(GatewayOpcode.VoiceStateUpdate, voiceState);
        return _webSocket.SendAsync(payload.Serialize(), _token);
    }

    public Task UpdatePresenceAsync(PresenceProperties presence)
    {
        GatewayPayloadProperties<PresenceProperties> payload = new(GatewayOpcode.PresenceUpdate, presence);
        return _webSocket.SendAsync(payload.Serialize(), _token);
    }

    public Task RequestGuildUsersAsync(GuildUsersRequestProperties requestProperties)
    {
        GatewayPayloadProperties<GuildUsersRequestProperties> payload = new(GatewayOpcode.RequestGuildUsers, requestProperties);
        return _webSocket.SendAsync(payload.Serialize(), _token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private protected override async Task ProcessMessageAsync(JsonPayload payload)
    {
        switch ((GatewayOpcode)payload.Opcode)
        {
            case GatewayOpcode.Dispatch:
                SequenceNumber = payload.SequenceNumber.GetValueOrDefault();
                try
                {
                    await ProcessEvent(payload).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    InvokeLog(LogMessage.Error(ex));
                }
                break;
            case GatewayOpcode.Heartbeat:
                break;
            case GatewayOpcode.Reconnect:
                InvokeLog(LogMessage.Info("Reconnect request"));
                try
                {
                    await _webSocket.CloseAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    InvokeLog(LogMessage.Error(ex));
                }
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
            case GatewayOpcode.InvalidSession:
                InvokeLog(LogMessage.Info("Invalid session"));
                try
                {
                    await SendIdentifyAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    InvokeLog(LogMessage.Error(ex));
                }
                break;
            case GatewayOpcode.Hello:
                BeginHeartbeating(payload.Data.GetValueOrDefault().GetProperty("heartbeat_interval").GetDouble());
                break;
            case GatewayOpcode.HeartbeatACK:
                Latency = _latencyTimer.Elapsed;
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessEvent(JsonPayload payload)
    {
        var data = payload.Data.GetValueOrDefault();
        switch (payload.Event)
        {
            case "READY":
                {
                    Latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    ReadyEventArgs args = new(data.ToObject<JsonReadyEventArgs>(), this);
                    InvokeLog(LogMessage.Info("Ready"));
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
                        if (channel is GroupDMChannel groupDm)
                            GroupDMChannels = GroupDMChannels.Add(groupDm.Id, groupDm);
                        else if (channel is DMChannel dm)
                            DMChannels = DMChannels.Add(dm.Id, dm);
                    SessionId = args.SessionId;
                    Shard = args.Shard;
                    ApplicationId = args.ApplicationId;
                    ApplicationFlags = args.ApplicationFlags;

                    if (!_readyCompletionSource.Task.IsCompleted)
                        _readyCompletionSource.SetResult();
                }
                break;
            case "RESUMED":
                {
                    Latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    InvokeLog(LogMessage.Info("Resumed"));
                }
                break;
            case "APPLICATION_COMMAND_PERMISSIONS_UPDATE":
                {
                    if (ApplicationCommandPermissionsUpdate != null)
                    {
                        try
                        {
                            await ApplicationCommandPermissionsUpdate(new(data.ToObject<JsonApplicationCommandGuildPermission>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "AUTO_MODERATION_RULE_CREATE":
                {
                    if (AutoModerationRuleCreate != null)
                    {
                        try
                        {
                            await AutoModerationRuleCreate(new(data.ToObject<JsonAutoModerationRule>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "AUTO_MODERATION_RULE_UPDATE":
                {
                    if (AutoModerationRuleUpdate != null)
                    {
                        try
                        {
                            await AutoModerationRuleUpdate(new(data.ToObject<JsonAutoModerationRule>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "AUTO_MODERATION_RULE_DELETE":
                {
                    if (AutoModerationRuleDelete != null)
                    {
                        try
                        {
                            await AutoModerationRuleDelete(new(data.ToObject<JsonAutoModerationRule>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "AUTO_MODERATION_ACTION_EXECUTION":
                {
                    if (AutoModerationActionExecution != null)
                    {
                        try
                        {
                            await AutoModerationActionExecution(new(data.ToObject<JsonAutoModerationActionExecutionEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    }
                }
                break;
            case "CHANNEL_CREATE":
                {
                    var json = data.ToObject<JsonChannel>();
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    if (GuildChannelCreate != null)
                        try
                        {
                            await GuildChannelCreate(new(channel, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(guildId, out var guild))
                    {
                        guild.Channels = guild.Channels.SetItem(channel.Id, channel);
                        guild._jsonModel.Channels[json.Id] = json;
                    }
                }
                break;
            case "CHANNEL_UPDATE":
                {
                    var json = data.ToObject<JsonChannel>();
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    if (GuildChannelUpdate != null)
                        try
                        {
                            await GuildChannelUpdate(new(channel, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(guildId, out var guild))
                    {
                        guild.Channels = guild.Channels.SetItem(channel.Id, channel);
                        guild._jsonModel.Channels[json.Id] = json;
                    }
                }
                break;
            case "CHANNEL_DELETE":
                {
                    var json = data.ToObject<JsonChannel>();
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    if (GuildChannelDelete != null)
                        try
                        {
                            await GuildChannelDelete(new(channel, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(guildId, out var guild))
                    {
                        guild.Channels = guild.Channels.Remove(channel.Id);
                        guild._jsonModel.Channels.Remove(json.Id);
                    }
                }
                break;
            case "CHANNEL_PINS_UPDATE":
                {
                    if (ChannelPinsUpdate != null)
                        try
                        {
                            await ChannelPinsUpdate(new(data.ToObject<JsonChannelPinsUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "THREAD_CREATE":
                {
                    var json = data.ToObject<JsonChannel>();
                    var thread = (GuildThread)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    if (GuildThreadCreate != null)
                        try
                        {
                            await GuildThreadCreate(new(thread, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(guildId, out var guild))
                    {
                        guild.ActiveThreads = guild.ActiveThreads.SetItem(thread.Id, thread);
                        guild._jsonModel.ActiveThreads[json.Id] = json;
                    }
                }
                break;
            case "THREAD_UPDATE":
                {
                    var json = data.ToObject<JsonChannel>();
                    var thread = (GuildThread)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    if (GuildThreadUpdate != null)
                        try
                        {
                            await GuildThreadUpdate(new(thread, guildId)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(guildId, out var guild))
                    {
                        guild.ActiveThreads = guild.ActiveThreads.SetItem(thread.Id, thread);
                        guild._jsonModel.ActiveThreads[json.Id] = json;
                    }
                }
                break;
            case "THREAD_DELETE":
                {
                    var json = data.ToObject<JsonChannel>();
                    var guildId = json.GuildId.GetValueOrDefault();
                    if (GuildThreadDelete != null)
                        try
                        {
                            await GuildThreadDelete(new(json.Id, guildId, json.ParentId.GetValueOrDefault(), json.Type)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(guildId, out var guild))
                    {
                        guild.ActiveThreads = guild.ActiveThreads.Remove(json.Id);
                        guild._jsonModel.ActiveThreads.Remove(json.Id);
                    }
                }
                break;
            case "THREAD_LIST_SYNC":
                {
                    ThreadListSyncEventArgs args = new(data.ToObject<JsonThreadListSyncEventArgs>(), Rest);
                    var guildId = args.GuildId;
                    if (ThreadListSync != null)
                        try
                        {
                            await ThreadListSync(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(guildId, out var guild))
                        guild.ActiveThreads = args.Threads;
                }
                break;
            case "THREAD_MEMBER_UPDATE":
                {
                    if (ThreadUserUpdate != null)
                        try
                        {
                            await ThreadUserUpdate(new(new(data.ToObject<JsonThreadUser>(), Rest), GetGuildId())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "THREAD_MEMBERS_UPDATE":
                {
                    if (GuildThreadUsersUpdate != null)
                        try
                        {
                            await GuildThreadUsersUpdate(new(data.ToObject<JsonGuildThreadUsersUpdateEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "GUILD_CREATE":
                {
                    var jsonGuild = data.ToObject<JsonGuild>();
                    var id = jsonGuild.Id;
                    if (jsonGuild.IsUnavaible)
                    {
                        if (GuildCreate != null)
                            try
                            {
                                await GuildCreate(new(id, null)).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                InvokeLog(LogMessage.Error(ex));
                            }
                    }
                    else
                    {
                        Guild guild = new(jsonGuild, Rest);
                        if (GuildCreate != null)
                            try
                            {
                                await GuildCreate(new(id, guild)).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                InvokeLog(LogMessage.Error(ex));
                            }
                        Guilds = Guilds.SetItem(id, guild);
                    }
                }
                break;
            case "GUILD_UPDATE":
                {
                    var guildId = GetGuildId();
                    if (Guilds.TryGetValue(guildId, out var oldGuild))
                    {
                        Guild guild = new(data.ToObject<JsonGuild>(), oldGuild);
                        if (GuildUpdate != null)
                            try
                            {
                                await GuildUpdate(guild).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                InvokeLog(LogMessage.Error(ex));
                            }
                        Guilds = Guilds.SetItem(guildId, guild);
                    }
                }
                break;
            case "GUILD_DELETE":
                {
                    var guildId = data.GetProperty("id").ToObject<Snowflake>();
                    if (GuildDelete != null)
                        try
                        {
                            await GuildDelete(new(guildId, !data.TryGetProperty("unavailable", out _))).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    Guilds = Guilds.Remove(guildId);
                }
                break;
            case "GUILD_BAN_ADD":
                {
                    if (GuildBanAdd != null)
                        try
                        {
                            await GuildBanAdd(new(data.ToObject<JsonGuildBanEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "GUILD_BAN_REMOVE":
                {
                    if (GuildBanRemove != null)
                        try
                        {
                            await GuildBanRemove(new(data.ToObject<JsonGuildBanEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "GUILD_EMOJIS_UPDATE":
                {
                    var json = data.ToObject<JsonGuildEmojisUpdateEventArgs>();
                    GuildEmojisUpdateEventArgs args = new(json, Rest);
                    if (GuildEmojisUpdate != null)
                        try
                        {
                            await GuildEmojisUpdate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild.Emojis = args.Emojis;
                        guild._jsonModel.Emojis.Clear();
                        guild._jsonModel.Emojis.AddRange(json.Emojis);
                    }
                }
                break;
            case "GUILD_STICKERS_UPDATE":
                {
                    var json = data.ToObject<JsonGuildStickersUpdateEventArgs>();
                    GuildStickersUpdateEventArgs args = new(json, Rest);
                    if (GuildStickersUpdate != null)
                        try
                        {
                            await GuildStickersUpdate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild.Stickers = args.Stickers;
                        guild._jsonModel.Stickers.Clear();
                        guild._jsonModel.Stickers.AddRange(json.Stickers);
                    }
                }
                break;
            case "GUILD_INTEGRATIONS_UPDATE":
                {
                    if (GuildIntegrationsUpdate != null)
                        try
                        {
                            await GuildIntegrationsUpdate(new(data.ToObject<JsonGuildIntegrationsUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "GUILD_MEMBER_ADD":
                {
                    var json = data.ToObject<JsonGuildUser>();
                    GuildUser user = new(json, GetGuildId(), Rest);
                    if (GuildUserAdd != null)
                        try
                        {
                            await GuildUserAdd(user).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(user.GuildId, out var guild))
                    {
                        guild.Users = guild.Users.SetItem(user.Id, user);
                        guild._jsonModel.Users[json.User.Id] = json;
                    }
                }
                break;
            case "GUILD_MEMBER_UPDATE":
                {
                    var json = data.ToObject<JsonGuildUser>();
                    GuildUser user = new(json, GetGuildId(), Rest);
                    if (GuildUserUpdate != null)
                        try
                        {
                            await GuildUserUpdate(user).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(user.GuildId, out var guild))
                    {
                        guild.Users = guild.Users.SetItem(user.Id, user);
                        guild._jsonModel.Users[json.User.Id] = json;
                    }
                }
                break;
            case "GUILD_MEMBER_REMOVE":
                {
                    var json = data.ToObject<JsonGuildUserRemoveEventArgs>();
                    GuildUserRemoveEventArgs args = new(json, Rest);
                    if (GuildUserRemove != null)
                        try
                        {
                            await GuildUserRemove(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild.Users = guild.Users.Remove(args.User.Id);
                        guild._jsonModel.Users.Remove(json.User.Id);
                    }
                }
                break;
            case "GUILD_MEMBERS_CHUNK":
                {
                    var json = data.ToObject<JsonGuildUserChunkEventArgs>();
                    GuildUserChunkEventArgs args = new(json, Rest);
                    if (GuildUserChunk != null)
                        try
                        {
                            await GuildUserChunk(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild.Users = guild.Users.SetItems(args.Users);
                        foreach (var user in json.Users)
                            guild._jsonModel.Users[user.User.Id] = user;

                        if (args.Presences != null)
                        {
                            guild.Presences = guild.Presences.SetItems(args.Presences);
                            foreach (var presence in json.Presences!)
                                guild._jsonModel.Presences[presence.User.Id] = presence;
                        }
                    }
                }
                break;
            case "GUILD_ROLE_CREATE":
                {
                    var json = data.ToObject<JsonGuildRoleEventArgs>();
                    GuildRoleEventArgs args = new(json, Rest);
                    if (GuildRoleCreate != null)
                        try
                        {
                            await GuildRoleCreate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild.Roles = guild.Roles.SetItem(args.Role.Id, args.Role);
                        guild._jsonModel.Roles[json.Role.Id] = json.Role;
                    }
                }
                break;
            case "GUILD_ROLE_UPDATE":
                {
                    var json = data.ToObject<JsonGuildRoleEventArgs>();
                    GuildRoleEventArgs args = new(json, Rest);
                    if (GuildRoleUpdate != null)
                        try
                        {
                            await GuildRoleUpdate(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild.Roles = guild.Roles.SetItem(args.Role.Id, args.Role);
                        guild._jsonModel.Roles[json.Role.Id] = json.Role;
                    }
                }
                break;
            case "GUILD_ROLE_DELETE":
                {
                    var json = data.ToObject<JsonGuildRoleDeleteEventArgs>();
                    GuildRoleDeleteEventArgs args = new(json);
                    if (GuildRoleDelete != null)
                        try
                        {
                            await GuildRoleDelete(args).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(args.GuildId, out var guild))
                    {
                        guild.Roles = guild.Roles.Remove(args.RoleId);
                        guild._jsonModel.Roles.Remove(json.RoleId);
                    }
                }
                break;
            case "GUILD_SCHEDULED_EVENT_CREATE":
                {
                    var json = data.ToObject<JsonGuildScheduledEvent>();
                    GuildScheduledEvent scheduledEvent = new(json, Rest);
                    if (GuildScheduledEventCreate != null)
                        try
                        {
                            await GuildScheduledEventCreate(scheduledEvent).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                    {
                        guild.ScheduledEvents = guild.ScheduledEvents.SetItem(scheduledEvent.Id, scheduledEvent);
                        guild._jsonModel.ScheduledEvents[json.Id] = json;
                    }
                }
                break;
            case "GUILD_SCHEDULED_EVENT_UPDATE":
                {
                    var json = data.ToObject<JsonGuildScheduledEvent>();
                    GuildScheduledEvent scheduledEvent = new(json, Rest);
                    if (GuildScheduledEventUpdate != null)
                        try
                        {
                            await GuildScheduledEventUpdate(scheduledEvent).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                    {
                        guild.ScheduledEvents = guild.ScheduledEvents.SetItem(scheduledEvent.Id, scheduledEvent);
                        guild._jsonModel.ScheduledEvents[json.Id] = json;
                    }
                }
                break;
            case "GUILD_SCHEDULED_EVENT_DELETE":
                {
                    var json = data.ToObject<JsonGuildScheduledEvent>();
                    GuildScheduledEvent scheduledEvent = new(json, Rest);
                    if (GuildScheduledEventDelete != null)
                        try
                        {
                            await GuildScheduledEventDelete(scheduledEvent).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                    {
                        guild.ScheduledEvents = guild.ScheduledEvents.Remove(scheduledEvent.Id);
                        guild._jsonModel.ScheduledEvents.Remove(json.Id);
                    }
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_ADD":
                {
                    if (GuildScheduledEventUserAdd != null)
                        try
                        {
                            await GuildScheduledEventUserAdd(new(data.ToObject<JsonGuildScheduledEventUserEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                {
                    if (GuildScheduledEventUserRemove != null)
                        try
                        {
                            await GuildScheduledEventUserRemove(new(data.ToObject<JsonGuildScheduledEventUserEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "INTEGRATION_CREATE":
                {
                    if (GuildIntegrationCreate != null)
                        try
                        {
                            await GuildIntegrationCreate(new(new(data.ToObject<JsonIntegration>(), Rest), GetGuildId())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "INTEGRATION_UPDATE":
                {
                    if (GuildIntegrationUpdate != null)
                        try
                        {
                            await GuildIntegrationUpdate(new(new(data.ToObject<JsonIntegration>(), Rest), GetGuildId())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "INTEGRATION_DELETE":
                {
                    if (GuildIntegrationDelete != null)
                        try
                        {
                            await GuildIntegrationDelete(new(data.ToObject<JsonGuildIntegrationDeleteEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "INTERACTION_CREATE":
                {
                    JsonInteraction interaction = data.ToObject<JsonInteraction>();
                    if (!interaction.GuildId.HasValue && interaction.ChannelId.HasValue)
                        await CacheChannelAsync(interaction.ChannelId.GetValueOrDefault()).ConfigureAwait(false);

                    if (InteractionCreate != null)
                        try
                        {
                            await InteractionCreate.Invoke(Interaction.CreateFromJson(interaction, this)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "INVITE_CREATE":
                {
                    if (GuildInviteCreate != null)
                        try
                        {
                            await GuildInviteCreate(new(data.ToObject<JsonGuildInvite>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "INVITE_DELETE":
                {
                    if (GuildInviteDelete != null)
                        try
                        {
                            await GuildInviteDelete(new(data.ToObject<JsonGuildInviteDeleteEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_CREATE":
                {
                    var jsonMessage = data.ToObject<JsonMessage>();
                    if (!jsonMessage.GuildId.HasValue && !jsonMessage.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral))
                        await CacheChannelAsync(jsonMessage.ChannelId).ConfigureAwait(false);

                    if (MessageCreate != null)
                        try
                        {
                            await MessageCreate(Message.CreateFromJson(jsonMessage, this)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_UPDATE":
                {
                    var jsonMessage = data.ToObject<JsonMessage>();
                    if (!jsonMessage.GuildId.HasValue && !jsonMessage.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral))
                        await CacheChannelAsync(jsonMessage.ChannelId).ConfigureAwait(false);

                    if (MessageUpdate != null)
                        try
                        {
                            await MessageUpdate(Message.CreateFromJson(jsonMessage, this)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_DELETE":
                {
                    if (MessageDelete != null)
                        try
                        {
                            await MessageDelete(new(data.ToObject<JsonMessageDeleteEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_DELETE_BULK":
                {
                    if (MessageDeleteBulk != null)
                        try
                        {
                            await MessageDeleteBulk(new(data.ToObject<JsonMessageDeleteBulkEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_REACTION_ADD":
                {
                    if (MessageReactionAdd != null)
                        try
                        {
                            await MessageReactionAdd(new(data.ToObject<JsonMessageReactionAddEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_REACTION_REMOVE":
                {
                    if (MessageReactionRemove != null)
                        try
                        {
                            await MessageReactionRemove(new(data.ToObject<JsonMessageReactionRemoveEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_REACTION_REMOVE_ALL":
                {
                    if (MessageReactionRemoveAll != null)
                        try
                        {
                            await MessageReactionRemoveAll(new(data.ToObject<JsonMessageReactionRemoveAllEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                {
                    if (MessageReactionRemoveEmoji != null)
                        try
                        {
                            await MessageReactionRemoveEmoji(new(data.ToObject<JsonMessageReactionRemoveEmojiEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "PRESENCE_UPDATE":
                {
                    var json = data.ToObject<JsonPresence>();
                    Presence presence = new(json, null, Rest);
                    if (PresenceUpdate != null)
                        try
                        {
                            await PresenceUpdate(presence).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(presence.GuildId, out var guild))
                    {
                        guild.Presences = guild.Presences.SetItem(presence.User.Id, presence);
                        guild._jsonModel.Presences[json.User.Id] = json;
                    }
                }
                break;
            case "STAGE_INSTANCE_CREATE":
                {
                    var json = data.ToObject<JsonStageInstance>();
                    StageInstance stageInstance = new(json, Rest);
                    if (StageInstanceCreate != null)
                        try
                        {
                            await StageInstanceCreate(stageInstance).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(stageInstance.GuildId, out var guild))
                    {
                        guild.StageInstances = guild.StageInstances.SetItem(stageInstance.Id, stageInstance);
                        guild._jsonModel.StageInstances[json.Id] = json;
                    }
                }
                break;
            case "STAGE_INSTANCE_UPDATE":
                {
                    var json = data.ToObject<JsonStageInstance>();
                    StageInstance stageInstance = new(json, Rest);
                    if (StageInstanceUpdate != null)
                        try
                        {
                            await StageInstanceUpdate(stageInstance).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(stageInstance.GuildId, out var guild))
                    {
                        guild.StageInstances = guild.StageInstances.SetItem(stageInstance.Id, stageInstance);
                        guild._jsonModel.StageInstances[json.Id] = json;
                    }
                }
                break;
            case "STAGE_INSTANCE_DELETE":
                {
                    var json = data.ToObject<JsonStageInstance>();
                    StageInstance stageInstance = new(json, Rest);
                    if (StageInstanceDelete != null)
                        try
                        {
                            await StageInstanceDelete(stageInstance).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(stageInstance.GuildId, out var guild))
                    {
                        guild.StageInstances = guild.StageInstances.Remove(stageInstance.Id);
                        guild._jsonModel.StageInstances.Remove(json.Id);
                    }
                }
                break;
            case "TYPING_START":
                {
                    if (TypingStart != null)
                        try
                        {
                            await TypingStart(new(data.ToObject<JsonTypingStartEventArgs>(), Rest)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "USER_UPDATE":
                {
                    SelfUser user = new(data.ToObject<JsonUser>(), Rest);
                    if (CurrentUserUpdate != null)
                        try
                        {
                            await CurrentUserUpdate(user).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    User = user;
                }
                break;
            case "VOICE_STATE_UPDATE":
                {
                    var json = data.ToObject<JsonVoiceState>();
                    VoiceState voiceState = new(json);
                    if (VoiceStateUpdate != null)
                        try
                        {
                            await VoiceStateUpdate(voiceState).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                    if (TryGetGuild(voiceState.GuildId.GetValueOrDefault(), out var guild))
                    {
                        if (voiceState.ChannelId.HasValue)
                        {
                            guild.VoiceStates = guild.VoiceStates.SetItem(voiceState.UserId, voiceState);
                            guild._jsonModel.VoiceStates[json.UserId] = json;
                        }
                        else
                        {
                            guild.VoiceStates = guild.VoiceStates.Remove(voiceState.UserId);
                            guild._jsonModel.VoiceStates.Remove(json.UserId);
                        }
                    }
                }
                break;
            case "VOICE_SERVER_UPDATE":
                {
                    if (VoiceServerUpdate != null)
                        try
                        {
                            await VoiceServerUpdate(new(data.ToObject<JsonVoiceServerUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
            case "WEBHOOKS_UPDATE":
                {
                    if (WebhooksUpdate != null)
                        try
                        {
                            await WebhooksUpdate(new(data.ToObject<JsonWebhooksUpdateEventArgs>())).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            InvokeLog(LogMessage.Error(ex));
                        }
                }
                break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Snowflake GetGuildId() => data.GetProperty("guild_id").ToObject<Snowflake>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryGetGuild(Snowflake guildId, [NotNullWhen(true)] out Guild? guild) => Guilds.TryGetValue(guildId, out guild);
    }

    private async Task CacheChannelAsync(Snowflake channelId)
    {
        if (!DMChannels.ContainsKey(channelId) && !GroupDMChannels.ContainsKey(channelId))
        {
            var channel = await Rest.GetChannelAsync(channelId).ConfigureAwait(false);
            if (channel is GroupDMChannel groupDMChannel)
                GroupDMChannels = GroupDMChannels.SetItem(channelId, groupDMChannel);
            else if (channel is DMChannel dMChannel)
                DMChannels = DMChannels.SetItem(channelId, dMChannel);
        }
    }
}