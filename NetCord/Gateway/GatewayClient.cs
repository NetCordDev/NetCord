using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.Gateway.Compression;
using NetCord.Gateway.JsonModels;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway;

public partial class GatewayClient : WebSocketClient
{
    private readonly string _botToken;
    private readonly GatewayClientConfiguration _configuration;
    private readonly Uri _url;
    private readonly object? _DMsLock;
    private readonly Dictionary<ulong, SemaphoreSlim>? _DMSemaphores;
    private readonly IGatewayCompression _compression;
    private readonly bool _disposeRest;

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
    public event Func<GuildThreadCreateEventArgs, ValueTask>? GuildThreadCreate;
    public event Func<GuildThread, ValueTask>? GuildThreadUpdate;
    public event Func<GuildThreadDeleteEventArgs, ValueTask>? GuildThreadDelete;
    public event Func<GuildThreadListSyncEventArgs, ValueTask>? GuildThreadListSync;
    public event Func<GuildThreadUserUpdateEventArgs, ValueTask>? GuildThreadUserUpdate;
    public event Func<GuildThreadUsersUpdateEventArgs, ValueTask>? GuildThreadUsersUpdate;
    public event Func<GuildCreateEventArgs, ValueTask>? GuildCreate;
    public event Func<Guild, ValueTask>? GuildUpdate;
    public event Func<GuildDeleteEventArgs, ValueTask>? GuildDelete;
    public event Func<AuditLogEntry, ValueTask>? GuildAuditLogEntryCreate;
    public event Func<GuildBanEventArgs, ValueTask>? GuildBanAdd;
    public event Func<GuildBanEventArgs, ValueTask>? GuildBanRemove;
    public event Func<GuildEmojisUpdateEventArgs, ValueTask>? GuildEmojisUpdate;
    public event Func<GuildStickersUpdateEventArgs, ValueTask>? GuildStickersUpdate;
    public event Func<GuildIntegrationsUpdateEventArgs, ValueTask>? GuildIntegrationsUpdate;
    public event Func<GuildUser, ValueTask>? GuildUserAdd;
    public event Func<GuildUser, ValueTask>? GuildUserUpdate;
    public event Func<GuildUserRemoveEventArgs, ValueTask>? GuildUserRemove;
    public event Func<GuildUserChunkEventArgs, ValueTask>? GuildUserChunk;
    public event Func<RoleEventArgs, ValueTask>? RoleCreate;
    public event Func<RoleEventArgs, ValueTask>? RoleUpdate;
    public event Func<RoleDeleteEventArgs, ValueTask>? RoleDelete;
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
    public event Func<CurrentUser, ValueTask>? CurrentUserUpdate;
    public event Func<VoiceState, ValueTask>? VoiceStateUpdate;
    public event Func<VoiceServerUpdateEventArgs, ValueTask>? VoiceServerUpdate;
    public event Func<WebhooksUpdateEventArgs, ValueTask>? WebhooksUpdate;
    public event Func<Interaction, ValueTask>? InteractionCreate;
    public event Func<StageInstance, ValueTask>? StageInstanceCreate;
    public event Func<StageInstance, ValueTask>? StageInstanceUpdate;
    public event Func<StageInstance, ValueTask>? StageInstanceDelete;
    public event Func<Entitlement, ValueTask>? EntitlementCreate;
    public event Func<Entitlement, ValueTask>? EntitlementUpdate;
    public event Func<Entitlement, ValueTask>? EntitlementDelete;
    public event Func<UnknownEventEventArgs, ValueTask>? UnknownEvent;

    /// <summary>
    /// The cache of the <see cref="GatewayClient"/>.
    /// </summary>
    /// <remarks>It is <see langword="null"/> before starting of the <see cref="GatewayClient"/>.</remarks>
    [AllowNull]
    public IGatewayClientCache Cache { get; private set; }

    /// <summary>
    /// The session id of the <see cref="GatewayClient"/>.
    /// </summary>
    public string? SessionId { get; private set; }

    /// <summary>
    /// The sequence number of the <see cref="GatewayClient"/>.
    /// </summary>
    public int SequenceNumber { get; private set; }

    /// <summary>
    /// The shard of the <see cref="GatewayClient"/>.
    /// </summary>
    public Shard? Shard => _configuration.Shard;

    /// <summary>
    /// The application id of the <see cref="GatewayClient"/>.
    /// </summary>
    public ulong ApplicationId => _applicationId;
    private ulong _applicationId;

    /// <summary>
    /// The application flags of the <see cref="GatewayClient"/>.
    /// </summary>
    public ApplicationFlags ApplicationFlags { get; private set; }

    /// <summary>
    /// The <see cref="Rest.RestClient"/> of the <see cref="GatewayClient"/>.
    /// </summary>
    public Rest.RestClient Rest { get; }

    public GatewayClient(Token token, GatewayClientConfiguration? configuration = null) : this(token, new(token, (configuration ??= new()).RestClientConfiguration), configuration)
    {
        _disposeRest = true;
    }

    internal GatewayClient(Token token, Rest.RestClient rest, GatewayClientConfiguration configuration) : base(configuration.WebSocket, configuration.ReconnectTimer, configuration.LatencyTimer)
    {
        _botToken = token.RawToken;

        _configuration = configuration;
        var compression = _compression = configuration.Compression ?? new ZLibGatewayCompression();
        _url = new($"wss://{configuration.Hostname ?? Discord.GatewayHostname}/?v={(int)configuration.Version}&encoding=json&compress={compression.Name}", UriKind.Absolute);
        Rest = rest;

        if (configuration.CacheDMChannels)
        {
            _DMsLock = new();
            _DMSemaphores = [];
        }
    }

    private protected override void OnConnected()
    {
        _compression.Initialize();
    }

    private ValueTask SendIdentifyAsync(PresenceProperties? presence = null)
    {
        var serializedPayload = new GatewayPayloadProperties<GatewayIdentifyProperties>(GatewayOpcode.Identify, new(_botToken)
        {
            ConnectionProperties = _configuration.ConnectionProperties ?? ConnectionPropertiesProperties.Default,
            LargeThreshold = _configuration.LargeThreshold,
            Shard = _configuration.Shard,
            Presence = presence ?? _configuration.Presence,
            Intents = _configuration.Intents,
        }).Serialize(Serialization.Default.GatewayPayloadPropertiesGatewayIdentifyProperties);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload);
    }

    /// <summary>
    /// Starts the <see cref="GatewayClient"/>.
    /// </summary>
    /// <param name="presence">The presence to set.</param>
    /// <param name="cache">The cache to use.</param>
    /// <returns></returns>
    public async Task StartAsync(PresenceProperties? presence = null, IGatewayClientCache? cache = null)
    {
        if (cache is null)
            Cache ??= new GatewayClientCache();
        else
            Cache = cache;

        await ConnectAsync(_url).ConfigureAwait(false);
        await SendIdentifyAsync(presence).ConfigureAwait(false);
    }

    /// <summary>
    /// Resumes the session specified by <paramref name="sessionId"/>.
    /// </summary>
    /// <param name="sessionId">The session to resume.</param>
    /// <param name="sequenceNumber">The sequence number of the payload to resume from.</param>
    /// <param name="cache">The cache to use.</param>
    /// <returns></returns>
    public Task ResumeAsync(string sessionId, int sequenceNumber, IGatewayClientCache? cache = null)
    {
        SessionId = sessionId;
        SequenceNumber = sequenceNumber;

        if (cache is null)
            Cache ??= new GatewayClientCache();
        else
            Cache = cache;

        return TryResumeAsync();
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
        => status is not ((WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4010 or (WebSocketCloseStatus)4011 or (WebSocketCloseStatus)4012 or (WebSocketCloseStatus)4013 or (WebSocketCloseStatus)4014);

    private protected override async Task TryResumeAsync()
    {
        await ConnectAsync(_url).ConfigureAwait(false);

        var serializedPayload = new GatewayPayloadProperties<GatewayResumeProperties>(GatewayOpcode.Resume, new(_botToken, SessionId!, SequenceNumber)).Serialize(Serialization.Default.GatewayPayloadPropertiesGatewayResumeProperties);
        _latencyTimer.Start();
        await SendPayloadAsync(serializedPayload).ConfigureAwait(false);
    }

    private protected override ValueTask HeartbeatAsync()
    {
        var serializedPayload = new GatewayPayloadProperties<int>(GatewayOpcode.Heartbeat, SequenceNumber).Serialize(Serialization.Default.GatewayPayloadPropertiesInt32);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload);
    }

    private protected override JsonPayload CreatePayload(ReadOnlyMemory<byte> payload) => JsonSerializer.Deserialize(_compression.Decompress(payload).Span, Serialization.Default.JsonPayload)!;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private protected override async Task ProcessPayloadAsync(JsonPayload payload)
    {
        switch ((GatewayOpcode)payload.Opcode)
        {
            case GatewayOpcode.Dispatch:
                SequenceNumber = payload.SequenceNumber.GetValueOrDefault();
                try
                {
                    await ProcessEventAsync(payload).ConfigureAwait(false);
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
                await CloseAndReconnectAsync(WebSocketCloseStatus.Empty).ConfigureAwait(false);
                break;
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
                BeginHeartbeating(payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonHello).HeartbeatInterval);
                break;
            case GatewayOpcode.HeartbeatACK:
                await UpdateLatencyAsync(_latencyTimer.Elapsed).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>
    /// Joins, moves, or disconnects the app from a voice channel.
    /// </summary>
    /// <param name="voiceState"></param>
    /// <returns></returns>
    public ValueTask UpdateVoiceStateAsync(VoiceStateProperties voiceState)
    {
        GatewayPayloadProperties<VoiceStateProperties> payload = new(GatewayOpcode.VoiceStateUpdate, voiceState);
        return SendPayloadAsync(payload.Serialize(Serialization.Default.GatewayPayloadPropertiesVoiceStateProperties));
    }

    /// <summary>
    /// Updates an app's presence.
    /// </summary>
    /// <param name="presence">The presence to set.</param>
    /// <returns></returns>
    public ValueTask UpdatePresenceAsync(PresenceProperties presence)
    {
        GatewayPayloadProperties<PresenceProperties> payload = new(GatewayOpcode.PresenceUpdate, presence);
        return SendPayloadAsync(payload.Serialize(Serialization.Default.GatewayPayloadPropertiesPresenceProperties));
    }

    /// <summary>
    /// Requests user for a guild.
    /// </summary>
    /// <param name="requestProperties"></param>
    /// <returns></returns>
    public ValueTask RequestGuildUsersAsync(GuildUsersRequestProperties requestProperties)
    {
        GatewayPayloadProperties<GuildUsersRequestProperties> payload = new(GatewayOpcode.RequestGuildUsers, requestProperties);
        return SendPayloadAsync(payload.Serialize(Serialization.Default.GatewayPayloadPropertiesGuildUsersRequestProperties));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessEventAsync(JsonPayload payload)
    {
        var data = payload.Data.GetValueOrDefault();
        var name = payload.Event!;
        switch (name)
        {
            case "READY":
                {
                    var latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    InvokeLog(LogMessage.Info("Ready"));
                    var updateLatencyTask = UpdateLatencyAsync(latency);
                    ReadyEventArgs args = new(data.ToObject(Serialization.Default.JsonReadyEventArgs), Rest);
                    await InvokeEventAsync(Ready, args, data =>
                    {
                        var cache = Cache;
                        cache = cache.CacheCurrentUser(data.User);
                        if (_configuration.CacheDMChannels)
                        {
                            var dMChannels = args.DMChannels;
                            var count = dMChannels.Count;
                            if (count != 0)
                            {
                                lock (_DMsLock!)
                                {
                                    for (var i = 0; i < count; i++)
                                        cache = cache.CacheDMChannel(dMChannels[i]);
                                }
                            }
                        }
                        Cache = cache;

                        SessionId = args.SessionId;
                        Interlocked.Exchange(ref _applicationId, args.ApplicationId);
                        ApplicationFlags = args.ApplicationFlags;

                        _readyCompletionSource.TrySetResult();
                    }).ConfigureAwait(false);
                    await updateLatencyTask.ConfigureAwait(false);
                }
                break;
            case "RESUMED":
                {
                    var latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    InvokeLog(LogMessage.Info("Resumed"));
                    var updateLatencyTask = UpdateLatencyAsync(latency);
                    var resumeTask = InvokeResumeEventAsync();

                    _readyCompletionSource.TrySetResult();

                    await updateLatencyTask.ConfigureAwait(false);
                    await resumeTask.ConfigureAwait(false);
                }
                break;
            case "APPLICATION_COMMAND_PERMISSIONS_UPDATE":
                {
                    await InvokeEventAsync(ApplicationCommandPermissionsUpdate, () => new(data.ToObject(Serialization.Default.JsonApplicationCommandGuildPermission))).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_CREATE":
                {
                    await InvokeEventAsync(AutoModerationRuleCreate, () => new(data.ToObject(Serialization.Default.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_UPDATE":
                {
                    await InvokeEventAsync(AutoModerationRuleUpdate, () => new(data.ToObject(Serialization.Default.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_DELETE":
                {
                    await InvokeEventAsync(AutoModerationRuleDelete, () => new(data.ToObject(Serialization.Default.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_ACTION_EXECUTION":
                {
                    await InvokeEventAsync(AutoModerationActionExecution, () => new(data.ToObject(Serialization.Default.JsonAutoModerationActionExecutionEventArgs))).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildChannelCreate, () => new(channel, guildId), () => Cache = Cache.CacheGuildChannel(guildId, channel)).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildChannelUpdate, () => new(channel, guildId), () => Cache = Cache.CacheGuildChannel(guildId, channel)).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildChannelDelete, () => new(channel, guildId), () => Cache = Cache.RemoveGuildChannel(guildId, channel.Id)).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_PINS_UPDATE":
                {
                    await InvokeEventAsync(ChannelPinsUpdate, () => new(data.ToObject(Serialization.Default.JsonChannelPinsUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "THREAD_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var thread = (GuildThread)Channel.CreateFromJson(json, Rest);
                    await InvokeEventAsync(GuildThreadCreate, () => new(thread, json.NewlyCreated.GetValueOrDefault()), () => Cache = Cache.CacheGuildThread(thread)).ConfigureAwait(false);
                }
                break;
            case "THREAD_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var thread = (GuildThread)Channel.CreateFromJson(json, Rest);
                    await InvokeEventAsync(GuildThreadUpdate, thread, t => Cache = Cache.CacheGuildThread(t)).ConfigureAwait(false);
                }
                break;
            case "THREAD_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildThreadDelete, () => new(json.Id, guildId, json.ParentId.GetValueOrDefault(), json.Type), () => Cache = Cache.RemoveGuildThread(guildId, json.Id)).ConfigureAwait(false);
                }
                break;
            case "THREAD_LIST_SYNC":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildThreadListSyncEventArgs);
                    GuildThreadListSyncEventArgs args = new(json, Rest);
                    var guildId = args.GuildId;
                    await InvokeEventAsync(GuildThreadListSync, args, args => Cache = Cache.SyncGuildActiveThreads(guildId, args.Threads)).ConfigureAwait(false);
                }
                break;
            case "THREAD_MEMBER_UPDATE":
                {
                    await InvokeEventAsync(GuildThreadUserUpdate, () => new(new(data.ToObject(Serialization.Default.JsonThreadUser), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "THREAD_MEMBERS_UPDATE":
                {
                    await InvokeEventAsync(GuildThreadUsersUpdate, () => new(data.ToObject(Serialization.Default.JsonGuildThreadUsersUpdateEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_CREATE":
                {
                    var jsonGuild = data.ToObject(Serialization.Default.JsonGuild);
                    var id = jsonGuild.Id;
                    if (jsonGuild.IsUnavailable)
                        await InvokeEventAsync(GuildCreate, () => new(id, null)).ConfigureAwait(false);
                    else
                    {
                        Guild guild = new(jsonGuild, Rest);
                        await InvokeEventAsync(GuildCreate, () => new(id, guild), () => Cache = Cache.CacheGuild(guild)).ConfigureAwait(false);
                    }
                }
                break;
            case "GUILD_UPDATE":
                {
                    var guildId = GetGuildId();
                    if (Cache.Guilds.TryGetValue(guildId, out var oldGuild))
                    {
                        await InvokeEventAsync(GuildUpdate, new(data.ToObject(Serialization.Default.JsonGuild), oldGuild), guild => Cache = Cache.CacheGuild(guild)).ConfigureAwait(false);
                    }
                }
                break;
            case "GUILD_DELETE":
                {
                    var jsonGuild = data.ToObject(Serialization.Default.JsonGuild);
                    await InvokeEventAsync(GuildDelete, () => new(jsonGuild.Id, !jsonGuild.IsUnavailable), () => Cache = Cache.RemoveGuild(jsonGuild.Id)).ConfigureAwait(false);
                }
                break;
            case "GUILD_AUDIT_LOG_ENTRY_CREATE":
                {
                    await InvokeEventAsync(GuildAuditLogEntryCreate, () => new(data.ToObject(Serialization.Default.JsonAuditLogEntry))).ConfigureAwait(false);
                }
                break;
            case "GUILD_BAN_ADD":
                {
                    await InvokeEventAsync(GuildBanAdd, () => new(data.ToObject(Serialization.Default.JsonGuildBanEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_BAN_REMOVE":
                {
                    await InvokeEventAsync(GuildBanRemove, () => new(data.ToObject(Serialization.Default.JsonGuildBanEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_EMOJIS_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildEmojisUpdateEventArgs);
                    await InvokeEventAsync(GuildEmojisUpdate, new(json, Rest), args => Cache = Cache.CacheGuildEmojis(args.GuildId, args.Emojis)).ConfigureAwait(false);
                }
                break;
            case "GUILD_STICKERS_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildStickersUpdateEventArgs);
                    await InvokeEventAsync(GuildStickersUpdate, new(json, Rest), args => Cache = Cache.CacheGuildStickers(args.GuildId, args.Stickers)).ConfigureAwait(false);
                }
                break;
            case "GUILD_INTEGRATIONS_UPDATE":
                {
                    await InvokeEventAsync(GuildIntegrationsUpdate, () => new(data.ToObject(Serialization.Default.JsonGuildIntegrationsUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_ADD":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUser);
                    await InvokeEventAsync(GuildUserAdd, new(json, GetGuildId(), Rest), user => Cache = Cache.CacheGuildUser(user)).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUser);
                    await InvokeEventAsync(GuildUserUpdate, new(json, GetGuildId(), Rest), user => Cache = Cache.CacheGuildUser(user)).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_REMOVE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUserRemoveEventArgs);
                    await InvokeEventAsync(GuildUserRemove, new(json, Rest), args => Cache = Cache.RemoveGuildUser(args.GuildId, args.User.Id)).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBERS_CHUNK":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUserChunkEventArgs);
                    await InvokeEventAsync(GuildUserChunk, new(json, Rest), args =>
                    {
                        var guildId = args.GuildId;
                        var cache = Cache.CacheGuildUsers(guildId, args.Users);
                        var presences = args.Presences;
                        if (presences is not null)
                            cache = cache.CachePresences(guildId, presences);
                        Cache = cache;
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonRoleEventArgs);
                    await InvokeEventAsync(RoleCreate, new(json, Rest), args => Cache = Cache.CacheRole(args.GuildId, args.Role)).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonRoleEventArgs);
                    await InvokeEventAsync(RoleUpdate, new(json, Rest), args => Cache = Cache.CacheRole(args.GuildId, args.Role)).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonRoleDeleteEventArgs);
                    await InvokeEventAsync(RoleDelete, new(json), args => Cache = Cache.RemoveRole(args.GuildId, args.RoleId)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventCreate, new(json, Rest), scheduledEvent => Cache = Cache.CacheGuildScheduledEvent(scheduledEvent)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventUpdate, new(json, Rest), scheduledEvent => Cache = Cache.CacheGuildScheduledEvent(scheduledEvent)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventDelete, new(json, Rest), scheduledEvent => Cache = Cache.RemoveGuildScheduledEvent(scheduledEvent.GuildId, scheduledEvent.Id)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_ADD":
                {
                    await InvokeEventAsync(GuildScheduledEventUserAdd, () => new(data.ToObject(Serialization.Default.JsonGuildScheduledEventUserEventArgs))).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                {
                    await InvokeEventAsync(GuildScheduledEventUserRemove, () => new(data.ToObject(Serialization.Default.JsonGuildScheduledEventUserEventArgs))).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_CREATE":
                {
                    await InvokeEventAsync(GuildIntegrationCreate, () => new(new(data.ToObject(Serialization.Default.JsonIntegration), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_UPDATE":
                {
                    await InvokeEventAsync(GuildIntegrationUpdate, () => new(new(data.ToObject(Serialization.Default.JsonIntegration), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_DELETE":
                {
                    await InvokeEventAsync(GuildIntegrationDelete, () => new(data.ToObject(Serialization.Default.JsonGuildIntegrationDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "INTERACTION_CREATE":
                {
                    await InvokeEventAsync(InteractionCreate, () => Interaction.CreateFromJson(data.ToObject(Serialization.Default.JsonInteraction), Cache, Rest)).ConfigureAwait(false);
                }
                break;
            case "INVITE_CREATE":
                {
                    await InvokeEventAsync(GuildInviteCreate, () => new(data.ToObject(Serialization.Default.JsonGuildInvite), Rest)).ConfigureAwait(false);
                }
                break;
            case "INVITE_DELETE":
                {
                    await InvokeEventAsync(GuildInviteDelete, () => new(data.ToObject(Serialization.Default.JsonGuildInviteDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_CREATE":
                {
                    await InvokeEventAsync(
                        MessageCreate,
                        () => data.ToObject(Serialization.Default.JsonMessage),
                        json => Message.CreateFromJson(json, Cache, Rest),
                        json => _configuration.CacheDMChannels && !json.GuildId.HasValue && !json.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral),
                        json =>
                        {
                            var channelId = json.ChannelId;
                            if (!_DMSemaphores!.TryGetValue(channelId, out var semaphore))
                                _DMSemaphores.Add(channelId, semaphore = new(1, 1));
                            return semaphore;
                        },
                        json => CacheChannelAsync(json.ChannelId)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_UPDATE":
                {
                    await InvokeEventAsync(
                        MessageUpdate,
                        () => data.ToObject(Serialization.Default.JsonMessage),
                        json => Message.CreateFromJson(json, Cache, Rest),
                        json => _configuration.CacheDMChannels && !json.GuildId.HasValue && !json.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral),
                        json =>
                        {
                            var channelId = json.ChannelId;
                            if (!_DMSemaphores!.TryGetValue(channelId, out var semaphore))
                                _DMSemaphores.Add(channelId, semaphore = new(1, 1));
                            return semaphore;
                        },
                        json => CacheChannelAsync(json.ChannelId)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_DELETE":
                {
                    await InvokeEventAsync(MessageDelete, () => new(data.ToObject(Serialization.Default.JsonMessageDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_DELETE_BULK":
                {
                    await InvokeEventAsync(MessageDeleteBulk, () => new(data.ToObject(Serialization.Default.JsonMessageDeleteBulkEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_ADD":
                {
                    await InvokeEventAsync(MessageReactionAdd, () => new(data.ToObject(Serialization.Default.JsonMessageReactionAddEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE":
                {
                    await InvokeEventAsync(MessageReactionRemove, () => new(data.ToObject(Serialization.Default.JsonMessageReactionRemoveEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE_ALL":
                {
                    await InvokeEventAsync(MessageReactionRemoveAll, () => new(data.ToObject(Serialization.Default.JsonMessageReactionRemoveAllEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                {
                    await InvokeEventAsync(MessageReactionRemoveEmoji, () => new(data.ToObject(Serialization.Default.JsonMessageReactionRemoveEmojiEventArgs))).ConfigureAwait(false);
                }
                break;
            case "PRESENCE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonPresence);
                    await InvokeEventAsync(PresenceUpdate, new(json, null, Rest), presence => Cache = Cache.CachePresence(presence)).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceCreate, new(json, Rest), stageInstance => Cache = Cache.CacheStageInstance(stageInstance)).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceUpdate, new(json, Rest), stageInstance => Cache = Cache.CacheStageInstance(stageInstance)).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceDelete, new(json, Rest), stageInstance => Cache = Cache.RemoveStageInstance(stageInstance.GuildId, stageInstance.Id)).ConfigureAwait(false);
                }
                break;
            case "TYPING_START":
                {
                    await InvokeEventAsync(TypingStart, () => new(data.ToObject(Serialization.Default.JsonTypingStartEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "USER_UPDATE":
                {
                    await InvokeEventAsync(CurrentUserUpdate, new(data.ToObject(Serialization.Default.JsonUser), Rest), user => Cache = Cache.CacheCurrentUser(user)).ConfigureAwait(false);
                }
                break;
            case "VOICE_STATE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonVoiceState);
                    await InvokeEventAsync(VoiceStateUpdate, new(json, Rest), voiceState =>
                    {
                        if (voiceState.ChannelId.HasValue)
                            Cache = Cache.CacheVoiceState(voiceState.GuildId.GetValueOrDefault(), voiceState);
                        else
                            Cache = Cache.RemoveVoiceState(voiceState.GuildId.GetValueOrDefault(), voiceState.UserId);
                    }).ConfigureAwait(false);
                }
                break;
            case "VOICE_SERVER_UPDATE":
                {
                    await InvokeEventAsync(VoiceServerUpdate, () => new(data.ToObject(Serialization.Default.JsonVoiceServerUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "WEBHOOKS_UPDATE":
                {
                    await InvokeEventAsync(WebhooksUpdate, () => new(data.ToObject(Serialization.Default.JsonWebhooksUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "ENTITLEMENT_CREATE":
                {
                    await InvokeEventAsync(EntitlementCreate, () => new(data.ToObject(Serialization.Default.JsonEntitlement))).ConfigureAwait(false);
                }
                break;
            case "ENTITLEMENT_UPDATE":
                {
                    await InvokeEventAsync(EntitlementUpdate, () => new(data.ToObject(Serialization.Default.JsonEntitlement))).ConfigureAwait(false);
                }
                break;
            case "ENTITLEMENT_DELETE":
                {
                    await InvokeEventAsync(EntitlementDelete, () => new(data.ToObject(Serialization.Default.JsonEntitlement))).ConfigureAwait(false);
                }
                break;
            default:
                {
                    await InvokeEventAsync(UnknownEvent, () => new(name, data)).ConfigureAwait(false);
                }
                break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ulong GetGuildId() => data.GetProperty("guild_id").ToObject(Serialization.Default.UInt64);

        async ValueTask CacheChannelAsync(ulong channelId)
        {
            var cache = Cache;
            if (!cache.DMChannels.ContainsKey(channelId))
            {
                var channel = await Rest.GetChannelAsync(channelId).ConfigureAwait(false);
                if (channel is DMChannel dMChannel)
                {
                    lock (_DMsLock!)
                        Cache = Cache.CacheDMChannel(dMChannel);
                }
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _compression.Dispose();
        if (_disposeRest)
            Rest.Dispose();
        Cache?.Dispose();
        if (_configuration.CacheDMChannels)
        {
            foreach (var semaphore in _DMSemaphores!.Values)
                semaphore.Dispose();
        }
    }
}
