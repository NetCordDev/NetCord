using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Gateway.WebSockets;
using NetCord.JsonModels;
using NetCord.JsonModels.EventArgs;
using NetCord.Utils;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway;

public partial class GatewayClient : WebSocketClient
{
    private readonly string _botToken;
    private readonly GatewayClientConfiguration _configuration;
    private readonly Uri _url;
    private bool _disposed;

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
    public event Func<SelfUser, ValueTask>? CurrentUserUpdate;
    public event Func<VoiceState, ValueTask>? VoiceStateUpdate;
    public event Func<VoiceServerUpdateEventArgs, ValueTask>? VoiceServerUpdate;
    public event Func<WebhooksUpdateEventArgs, ValueTask>? WebhooksUpdate;
    public event Func<Interaction, ValueTask>? InteractionCreate;
    public event Func<StageInstance, ValueTask>? StageInstanceCreate;
    public event Func<StageInstance, ValueTask>? StageInstanceUpdate;
    public event Func<StageInstance, ValueTask>? StageInstanceDelete;
    public event Func<UnknownEventEventArgs, ValueTask>? UnknownEvent;

    public ImmutableDictionary<ulong, Guild> Guilds { get; private set; } = CollectionsUtils.CreateImmutableDictionary<ulong, Guild>();
    public ImmutableDictionary<ulong, DMChannel> DMChannels { get; private set; } = CollectionsUtils.CreateImmutableDictionary<ulong, DMChannel>();
    public ImmutableDictionary<ulong, GroupDMChannel> GroupDMChannels { get; private set; } = CollectionsUtils.CreateImmutableDictionary<ulong, GroupDMChannel>();

    private readonly object? _dmsLock;
    private readonly Dictionary<ulong, SemaphoreSlim>? _dmSemaphores;

    /// <summary>
    /// Is <see langword="null"/> before <see cref="Ready"/> event.
    /// </summary>
    public SelfUser? User => _user;
    private SelfUser? _user;
    public string? SessionId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Shard? Shard { get; private set; }
    public ulong ApplicationId => _applicationId;
    private ulong _applicationId;
    public ApplicationFlags ApplicationFlags { get; private set; }
    public Rest.RestClient Rest { get; }

    public GatewayClient(Token token, GatewayClientConfiguration? configuration = null) : base((configuration ??= new()).WebSocket ?? new WebSocket())
    {
        _botToken = token.RawToken;

        _configuration = configuration;
        _url = new($"wss://{configuration.Hostname ?? Discord.GatewayHostname}/?v={(int)configuration.Version}&encoding=json", UriKind.Absolute);
        Rest = new(token, configuration.RestClientConfiguration);
        if (configuration.CacheDMChannels)
        {
            _dmsLock = new();
            _dmSemaphores = new();
        }
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
        }).Serialize(GatewayPayloadProperties.GatewayPayloadPropertiesOfGatewayIdentifyPropertiesSerializerContext.WithOptions.GatewayPayloadPropertiesGatewayIdentifyProperties);
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload);
    }

    /// <summary>
    /// Connects the <see cref="GatewayClient"/> to the gateway.
    /// </summary>
    /// <returns></returns>
    public async Task StartAsync(PresenceProperties? presence = null)
    {
        ThrowIfDisposed();
        await _webSocket.ConnectAsync(_url).ConfigureAwait(false);
        await SendIdentifyAsync(presence).ConfigureAwait(false);
    }

    /// <summary>
    /// Disconnects the <see cref="GatewayClient"/> from the gateway.
    /// </summary>
    /// <returns></returns>
    public Task CloseAsync()
    {
        ThrowIfDisposed();
        return CloseAsync(WebSocketCloseStatus.NormalClosure);
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
        => status is not ((WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4010 or (WebSocketCloseStatus)4011 or (WebSocketCloseStatus)4012 or (WebSocketCloseStatus)4013 or (WebSocketCloseStatus)4014);

    private protected override async Task ResumeAsync()
    {
        await _webSocket.ConnectAsync(_url).ConfigureAwait(false);

        var serializedPayload = new GatewayPayloadProperties<GatewayResumeProperties>(GatewayOpcode.Resume, new(_botToken, SessionId!, SequenceNumber)).Serialize(GatewayPayloadProperties.GatewayPayloadPropertiesOfGatewayResumePropertiesSerializerContext.WithOptions.GatewayPayloadPropertiesGatewayResumeProperties);
        _latencyTimer.Start();
        await _webSocket.SendAsync(serializedPayload).ConfigureAwait(false);
    }

    private protected override ValueTask HeartbeatAsync()
    {
        var serializedPayload = new GatewayPayloadProperties<int>(GatewayOpcode.Heartbeat, SequenceNumber).Serialize(GatewayPayloadProperties.GatewayPayloadPropertiesOfInt32SerializerContext.WithOptions.GatewayPayloadPropertiesInt32);
        _latencyTimer.Start();
        return _webSocket.SendAsync(serializedPayload);
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
                    await CloseAsync(WebSocketCloseStatus.Empty).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    InvokeLog(LogMessage.Error(ex));
                }
                await ReconnectAsync().ConfigureAwait(false);
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
                BeginHeartbeating(payload.Data.GetValueOrDefault().ToObject(JsonHello.JsonHelloSerializerContext.WithOptions.JsonHello).HeartbeatInterval);
                break;
            case GatewayOpcode.HeartbeatACK:
                await UpdateLatencyAsync(_latencyTimer.Elapsed).ConfigureAwait(false);
                break;
        }
    }

    public ValueTask UpdateVoiceStateAsync(VoiceStateProperties voiceState)
    {
        GatewayPayloadProperties<VoiceStateProperties> payload = new(GatewayOpcode.VoiceStateUpdate, voiceState);
        return _webSocket.SendAsync(payload.Serialize(GatewayPayloadProperties.GatewayPayloadPropertiesOfVoiceStatePropertiesSerializerContext.WithOptions.GatewayPayloadPropertiesVoiceStateProperties));
    }

    public ValueTask UpdatePresenceAsync(PresenceProperties presence)
    {
        GatewayPayloadProperties<PresenceProperties> payload = new(GatewayOpcode.PresenceUpdate, presence);
        return _webSocket.SendAsync(payload.Serialize(GatewayPayloadProperties.GatewayPayloadPropertiesOfPresencePropertiesSerializerContext.WithOptions.GatewayPayloadPropertiesPresenceProperties));
    }

    public ValueTask RequestGuildUsersAsync(GuildUsersRequestProperties requestProperties)
    {
        GatewayPayloadProperties<GuildUsersRequestProperties> payload = new(GatewayOpcode.RequestGuildUsers, requestProperties);
        return _webSocket.SendAsync(payload.Serialize(GatewayPayloadProperties.GatewayPayloadPropertiesOfGuildUsersRequestPropertiesSerializerContext.WithOptions.GatewayPayloadPropertiesGuildUsersRequestProperties));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessEvent(JsonPayload payload)
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
                    ReadyEventArgs args = new(data.ToObject(JsonReadyEventArgs.JsonReadyEventArgsSerializerContext.WithOptions.JsonReadyEventArgs), Rest);
                    await InvokeEventAsync(Ready, args, data =>
                    {
                        _user = args.User;
                        if (_configuration.CacheDMChannels)
                        {
                            lock (_dmsLock!)
                            {
                                foreach (var channel in args.DMChannels)
                                {
                                    if (channel is GroupDMChannel groupDm)
                                        GroupDMChannels = GroupDMChannels.SetItem(groupDm.Id, groupDm);
                                    else if (channel is DMChannel dm)
                                        DMChannels = DMChannels.SetItem(dm.Id, dm);
                                }
                            }
                        }
                        SessionId = args.SessionId;
                        Shard = args.Shard;
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
                    await InvokeResumedEventAsync().ConfigureAwait(false);
                    await updateLatencyTask.ConfigureAwait(false);
                }
                break;
            case "APPLICATION_COMMAND_PERMISSIONS_UPDATE":
                {
                    await InvokeEventAsync(ApplicationCommandPermissionsUpdate, () => new(data.ToObject(JsonApplicationCommandGuildPermission.JsonApplicationCommandGuildPermissionSerializerContext.WithOptions.JsonApplicationCommandGuildPermission))).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_CREATE":
                {
                    await InvokeEventAsync(AutoModerationRuleCreate, () => new(data.ToObject(JsonAutoModerationRule.JsonAutoModerationRuleSerializerContext.WithOptions.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_UPDATE":
                {
                    await InvokeEventAsync(AutoModerationRuleUpdate, () => new(data.ToObject(JsonAutoModerationRule.JsonAutoModerationRuleSerializerContext.WithOptions.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_DELETE":
                {
                    await InvokeEventAsync(AutoModerationRuleDelete, () => new(data.ToObject(JsonAutoModerationRule.JsonAutoModerationRuleSerializerContext.WithOptions.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_ACTION_EXECUTION":
                {
                    await InvokeEventAsync(AutoModerationActionExecution, () => new(data.ToObject(JsonAutoModerationActionExecutionEventArgs.JsonAutoModerationActionExecutionEventArgsSerializerContext.WithOptions.JsonAutoModerationActionExecutionEventArgs))).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_CREATE":
                {
                    var json = data.ToObject(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel);
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildChannelCreate, () => new(channel, guildId), () =>
                    {
                        if (TryGetGuild(guildId, out var guild))
                        {
                            guild.Channels = guild.Channels.SetItem(channel.Id, channel);
                            guild._jsonModel.Channels = guild._jsonModel.Channels.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_UPDATE":
                {
                    var json = data.ToObject(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel);
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildChannelUpdate, () => new(channel, guildId), () =>
                    {
                        if (TryGetGuild(guildId, out var guild))
                        {
                            guild.Channels = guild.Channels.SetItem(channel.Id, channel);
                            guild._jsonModel.Channels = guild._jsonModel.Channels.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_DELETE":
                {
                    var json = data.ToObject(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel);
                    var channel = (IGuildChannel)Channel.CreateFromJson(json, Rest);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildChannelDelete, () => new(channel, guildId), () =>
                    {
                        if (TryGetGuild(guildId, out var guild))
                        {
                            guild.Channels = guild.Channels.Remove(channel.Id);
                            guild._jsonModel.Channels = guild._jsonModel.Channels.Remove(json.Id);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_PINS_UPDATE":
                {
                    await InvokeEventAsync(ChannelPinsUpdate, () => new(data.ToObject(JsonChannelPinsUpdateEventArgs.JsonChannelPinsUpdateEventArgsSerializerContext.WithOptions.JsonChannelPinsUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "THREAD_CREATE":
                {
                    var json = data.ToObject(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel);
                    var thread = (GuildThread)Channel.CreateFromJson(json, Rest);
                    await InvokeEventAsync(GuildThreadCreate, () => new(thread, json.NewlyCreated.GetValueOrDefault()), () =>
                    {
                        if (TryGetGuild(thread.GuildId, out var guild))
                        {
                            guild.ActiveThreads = guild.ActiveThreads.SetItem(thread.Id, thread);
                            guild._jsonModel.ActiveThreads = guild._jsonModel.ActiveThreads.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "THREAD_UPDATE":
                {
                    var json = data.ToObject(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel);
                    var thread = (GuildThread)Channel.CreateFromJson(json, Rest);
                    await InvokeEventAsync(GuildThreadUpdate, thread, t =>
                    {
                        if (TryGetGuild(thread.GuildId, out var guild))
                        {
                            guild.ActiveThreads = guild.ActiveThreads.SetItem(t.Id, t);
                            guild._jsonModel.ActiveThreads = guild._jsonModel.ActiveThreads.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "THREAD_DELETE":
                {
                    var json = data.ToObject(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildThreadDelete, () => new(json.Id, guildId, json.ParentId.GetValueOrDefault(), json.Type), () =>
                    {
                        if (TryGetGuild(guildId, out var guild))
                        {
                            guild.ActiveThreads = guild.ActiveThreads.Remove(json.Id);
                            guild._jsonModel.ActiveThreads = guild._jsonModel.ActiveThreads.Remove(json.Id);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "THREAD_LIST_SYNC":
                {
                    var json = data.ToObject(JsonGuildThreadListSyncEventArgs.JsonGuildThreadListSyncEventArgsSerializerContext.WithOptions.JsonGuildThreadListSyncEventArgs);
                    GuildThreadListSyncEventArgs args = new(json, Rest);
                    var guildId = args.GuildId;
                    await InvokeEventAsync(GuildThreadListSync, args, args =>
                    {
                        if (TryGetGuild(guildId, out var guild))
                        {
                            guild.ActiveThreads = args.Threads;
                            guild._jsonModel.ActiveThreads = json.Threads.ToImmutableDictionary(t => t.Id);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "THREAD_MEMBER_UPDATE":
                {
                    await InvokeEventAsync(GuildThreadUserUpdate, () => new(new(data.ToObject(JsonThreadUser.JsonThreadUserSerializerContext.WithOptions.JsonThreadUser), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "THREAD_MEMBERS_UPDATE":
                {
                    await InvokeEventAsync(GuildThreadUsersUpdate, () => new(data.ToObject(JsonGuildThreadUsersUpdateEventArgs.JsonGuildThreadUsersUpdateEventArgsSerializerContext.WithOptions.JsonGuildThreadUsersUpdateEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_CREATE":
                {
                    var jsonGuild = data.ToObject(JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild);
                    var id = jsonGuild.Id;
                    if (jsonGuild.IsUnavailable)
                        await InvokeEventAsync(GuildCreate, () => new(id, null)).ConfigureAwait(false);
                    else
                    {
                        Guild guild = new(jsonGuild, Rest);
                        await InvokeEventAsync(GuildCreate, () => new(id, guild), () =>
                        {
                            Guilds = Guilds.SetItem(id, guild);
                        }).ConfigureAwait(false);
                    }
                }
                break;
            case "GUILD_UPDATE":
                {
                    var guildId = GetGuildId();
                    if (Guilds.TryGetValue(guildId, out var oldGuild))
                    {
                        await InvokeEventAsync(GuildUpdate, new(data.ToObject(JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild), oldGuild), guild =>
                        {
                            Guilds = Guilds.SetItem(guildId, guild);
                        }).ConfigureAwait(false);
                    }
                }
                break;
            case "GUILD_DELETE":
                {
                    var jsonGuild = data.ToObject(JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild);
                    await InvokeEventAsync(GuildDelete, () => new(jsonGuild.Id, !jsonGuild.IsUnavailable), () =>
                    {
                        Guilds = Guilds.Remove(jsonGuild.Id);
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_AUDIT_LOG_ENTRY_CREATE":
                {
                    await InvokeEventAsync(GuildAuditLogEntryCreate, () => new(data.ToObject(JsonAuditLogEntry.JsonAuditLogEntrySerializerContext.WithOptions.JsonAuditLogEntry))).ConfigureAwait(false);
                }
                break;
            case "GUILD_BAN_ADD":
                {
                    await InvokeEventAsync(GuildBanAdd, () => new(data.ToObject(JsonGuildBanEventArgs.JsonGuildBanEventArgsSerializerContext.WithOptions.JsonGuildBanEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_BAN_REMOVE":
                {
                    await InvokeEventAsync(GuildBanRemove, () => new(data.ToObject(JsonGuildBanEventArgs.JsonGuildBanEventArgsSerializerContext.WithOptions.JsonGuildBanEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_EMOJIS_UPDATE":
                {
                    var json = data.ToObject(JsonGuildEmojisUpdateEventArgs.JsonGuildEmojisUpdateEventArgsSerializerContext.WithOptions.JsonGuildEmojisUpdateEventArgs);
                    await InvokeEventAsync(GuildEmojisUpdate, new(json, Rest), args =>
                    {
                        if (TryGetGuild(args.GuildId, out var guild))
                        {
                            guild.Emojis = args.Emojis;
                            guild._jsonModel.Emojis = json.Emojis;
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_STICKERS_UPDATE":
                {
                    var json = data.ToObject(JsonGuildStickersUpdateEventArgs.JsonGuildStickersUpdateEventArgsSerializerContext.WithOptions.JsonGuildStickersUpdateEventArgs);
                    await InvokeEventAsync(GuildStickersUpdate, new(json, Rest), args =>
                    {
                        if (TryGetGuild(args.GuildId, out var guild))
                        {
                            guild.Stickers = args.Stickers;
                            guild._jsonModel.Stickers = json.Stickers;
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_INTEGRATIONS_UPDATE":
                {
                    await InvokeEventAsync(GuildIntegrationsUpdate, () => new(data.ToObject(JsonGuildIntegrationsUpdateEventArgs.JsonGuildIntegrationsUpdateEventArgsSerializerContext.WithOptions.JsonGuildIntegrationsUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_ADD":
                {
                    var json = data.ToObject(JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser);
                    await InvokeEventAsync(GuildUserAdd, new(json, GetGuildId(), Rest), user =>
                    {
                        if (TryGetGuild(user.GuildId, out var guild))
                        {
                            guild.Users = guild.Users.SetItem(user.Id, user);
                            guild._jsonModel.Users = guild._jsonModel.Users.SetItem(json.User.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_UPDATE":
                {
                    var json = data.ToObject(JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser);
                    await InvokeEventAsync(GuildUserUpdate, new(json, GetGuildId(), Rest), user =>
                    {
                        if (TryGetGuild(user.GuildId, out var guild))
                        {
                            guild.Users = guild.Users.SetItem(user.Id, user);
                            guild._jsonModel.Users = guild._jsonModel.Users.SetItem(json.User.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_REMOVE":
                {
                    var json = data.ToObject(JsonGuildUserRemoveEventArgs.JsonGuildUserRemoveEventArgsSerializerContext.WithOptions.JsonGuildUserRemoveEventArgs);
                    await InvokeEventAsync(GuildUserRemove, new(json, Rest), args =>
                    {
                        if (TryGetGuild(args.GuildId, out var guild))
                        {
                            guild.Users = guild.Users.Remove(args.User.Id);
                            guild._jsonModel.Users = guild._jsonModel.Users.Remove(json.User.Id);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBERS_CHUNK":
                {
                    var json = data.ToObject(JsonGuildUserChunkEventArgs.JsonGuildUserChunkEventArgsSerializerContext.WithOptions.JsonGuildUserChunkEventArgs);
                    await InvokeEventAsync(GuildUserChunk, new(json, Rest), args =>
                    {
                        if (TryGetGuild(args.GuildId, out var guild))
                        {
                            guild.Users = guild.Users.SetItems(args.Users);
                            guild._jsonModel.Users = guild._jsonModel.Users.SetItems(json.Users.Select(u => new KeyValuePair<ulong, JsonGuildUser>(u.User.Id, u)));

                            if (args.Presences != null)
                            {
                                guild.Presences = guild.Presences.SetItems(args.Presences);
                                guild._jsonModel.Presences = guild._jsonModel.Presences.SetItems(json.Presences!.Select(p => new KeyValuePair<ulong, JsonPresence>(p.User.Id, p)));
                            }
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_CREATE":
                {
                    var json = data.ToObject(JsonRoleEventArgs.JsonRoleEventArgsSerializerContext.WithOptions.JsonRoleEventArgs);
                    await InvokeEventAsync(RoleCreate, new(json, Rest), args =>
                    {
                        if (TryGetGuild(args.GuildId, out var guild))
                        {
                            guild.Roles = guild.Roles.SetItem(args.Role.Id, args.Role);
                            guild._jsonModel.Roles = guild._jsonModel.Roles.SetItem(json.Role.Id, json.Role);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_UPDATE":
                {
                    var json = data.ToObject(JsonRoleEventArgs.JsonRoleEventArgsSerializerContext.WithOptions.JsonRoleEventArgs);
                    await InvokeEventAsync(RoleUpdate, new(json, Rest), args =>
                    {
                        if (TryGetGuild(args.GuildId, out var guild))
                        {
                            guild.Roles = guild.Roles.SetItem(args.Role.Id, args.Role);
                            guild._jsonModel.Roles = guild._jsonModel.Roles.SetItem(json.Role.Id, json.Role);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_DELETE":
                {
                    var json = data.ToObject(JsonRoleDeleteEventArgs.JsonRoleDeleteEventArgsSerializerContext.WithOptions.JsonRoleDeleteEventArgs);
                    await InvokeEventAsync(RoleDelete, new(json), args =>
                    {
                        if (TryGetGuild(args.GuildId, out var guild))
                        {
                            guild.Roles = guild.Roles.Remove(args.RoleId);
                            guild._jsonModel.Roles = guild._jsonModel.Roles.Remove(json.RoleId);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_CREATE":
                {
                    var json = data.ToObject(JsonGuildScheduledEvent.JsonGuildScheduledEventSerializerContext.WithOptions.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventCreate, new(json, Rest), scheduledEvent =>
                    {
                        if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                        {
                            guild.ScheduledEvents = guild.ScheduledEvents.SetItem(scheduledEvent.Id, scheduledEvent);
                            guild._jsonModel.ScheduledEvents = guild._jsonModel.ScheduledEvents.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_UPDATE":
                {
                    var json = data.ToObject(JsonGuildScheduledEvent.JsonGuildScheduledEventSerializerContext.WithOptions.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventUpdate, new(json, Rest), scheduledEvent =>
                    {
                        if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                        {
                            guild.ScheduledEvents = guild.ScheduledEvents.SetItem(scheduledEvent.Id, scheduledEvent);
                            guild._jsonModel.ScheduledEvents = guild._jsonModel.ScheduledEvents.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_DELETE":
                {
                    var json = data.ToObject(JsonGuildScheduledEvent.JsonGuildScheduledEventSerializerContext.WithOptions.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventDelete, new(json, Rest), scheduledEvent =>
                    {
                        if (TryGetGuild(scheduledEvent.GuildId, out var guild))
                        {
                            guild.ScheduledEvents = guild.ScheduledEvents.Remove(scheduledEvent.Id);
                            guild._jsonModel.ScheduledEvents = guild._jsonModel.ScheduledEvents.Remove(json.Id);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_ADD":
                {
                    await InvokeEventAsync(GuildScheduledEventUserAdd, () => new(data.ToObject(JsonGuildScheduledEventUserEventArgs.JsonGuildScheduledEventUserEventArgsSerializerContext.WithOptions.JsonGuildScheduledEventUserEventArgs))).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                {
                    await InvokeEventAsync(GuildScheduledEventUserRemove, () => new(data.ToObject(JsonGuildScheduledEventUserEventArgs.JsonGuildScheduledEventUserEventArgsSerializerContext.WithOptions.JsonGuildScheduledEventUserEventArgs))).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_CREATE":
                {
                    await InvokeEventAsync(GuildIntegrationCreate, () => new(new(data.ToObject(JsonIntegration.JsonIntegrationSerializerContext.WithOptions.JsonIntegration), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_UPDATE":
                {
                    await InvokeEventAsync(GuildIntegrationUpdate, () => new(new(data.ToObject(JsonIntegration.JsonIntegrationSerializerContext.WithOptions.JsonIntegration), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_DELETE":
                {
                    await InvokeEventAsync(GuildIntegrationDelete, () => new(data.ToObject(JsonGuildIntegrationDeleteEventArgs.JsonIntegrationDeleteEventArgsSerializerContext.WithOptions.JsonGuildIntegrationDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "INTERACTION_CREATE":
                {
                    await InvokeEventAsync(InteractionCreate, () => Interaction.CreateFromJson(data.ToObject(JsonInteraction.JsonInteractionSerializerContext.WithOptions.JsonInteraction), this)).ConfigureAwait(false);
                }
                break;
            case "INVITE_CREATE":
                {
                    await InvokeEventAsync(GuildInviteCreate, () => new(data.ToObject(JsonGuildInvite.JsonGuildInviteSerializerContext.WithOptions.JsonGuildInvite), Rest)).ConfigureAwait(false);
                }
                break;
            case "INVITE_DELETE":
                {
                    await InvokeEventAsync(GuildInviteDelete, () => new(data.ToObject(JsonGuildInviteDeleteEventArgs.JsonGuildInviteDeleteEventArgsSerializerContext.WithOptions.JsonGuildInviteDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_CREATE":
                {
                    await InvokeEventAsync(
                        MessageCreate,
                        () => data.ToObject(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage),
                        json => Message.CreateFromJson(json, this),
                        json => _configuration.CacheDMChannels && !json.GuildId.HasValue && !json.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral),
                        json =>
                        {
                            var channelId = json.ChannelId;
                            if (!_dmSemaphores!.TryGetValue(channelId, out var semaphore))
                                _dmSemaphores.Add(channelId, semaphore = new(1, 1));
                            return semaphore;
                        },
                        json => CacheChannelAsync(json.ChannelId)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_UPDATE":
                {
                    await InvokeEventAsync(
                        MessageUpdate,
                        () => data.ToObject(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage),
                        json => Message.CreateFromJson(json, this),
                        json => _configuration.CacheDMChannels && !json.GuildId.HasValue && !json.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral),
                        json =>
                        {
                            var channelId = json.ChannelId;
                            if (!_dmSemaphores!.TryGetValue(channelId, out var semaphore))
                                _dmSemaphores.Add(channelId, semaphore = new(1, 1));
                            return semaphore;
                        },
                        json => CacheChannelAsync(json.ChannelId)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_DELETE":
                {
                    await InvokeEventAsync(MessageDelete, () => new(data.ToObject(JsonMessageDeleteEventArgs.JsonMessageDeleteEventArgsSerializerContext.WithOptions.JsonMessageDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_DELETE_BULK":
                {
                    await InvokeEventAsync(MessageDeleteBulk, () => new(data.ToObject(JsonMessageDeleteBulkEventArgs.JsonMessageDeleteBulkEventArgsSerializerContext.WithOptions.JsonMessageDeleteBulkEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_ADD":
                {
                    await InvokeEventAsync(MessageReactionAdd, () => new(data.ToObject(JsonMessageReactionAddEventArgs.JsonMessageReactionAddEventArgsSerializerContext.WithOptions.JsonMessageReactionAddEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE":
                {
                    await InvokeEventAsync(MessageReactionRemove, () => new(data.ToObject(JsonMessageReactionRemoveEventArgs.JsonMessageReactionRemoveEventArgsSerializerContext.WithOptions.JsonMessageReactionRemoveEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE_ALL":
                {
                    await InvokeEventAsync(MessageReactionRemoveAll, () => new(data.ToObject(JsonMessageReactionRemoveAllEventArgs.JsonMessageReactionRemoveAllEventArgsSerializerContext.WithOptions.JsonMessageReactionRemoveAllEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                {
                    await InvokeEventAsync(MessageReactionRemoveEmoji, () => new(data.ToObject(JsonMessageReactionRemoveEmojiEventArgs.JsonMessageReactionRemoveEmojiEventArgsSerializerContext.WithOptions.JsonMessageReactionRemoveEmojiEventArgs))).ConfigureAwait(false);
                }
                break;
            case "PRESENCE_UPDATE":
                {
                    var json = data.ToObject(JsonPresence.JsonPresenceSerializerContext.WithOptions.JsonPresence);
                    await InvokeEventAsync(PresenceUpdate, new(json, null, Rest), presence =>
                    {
                        if (TryGetGuild(presence.GuildId, out var guild))
                        {
                            guild.Presences = guild.Presences.SetItem(presence.User.Id, presence);
                            guild._jsonModel.Presences = guild._jsonModel.Presences.SetItem(json.User.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_CREATE":
                {
                    var json = data.ToObject(JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceCreate, new(json, Rest), stageInstance =>
                    {
                        if (TryGetGuild(stageInstance.GuildId, out var guild))
                        {
                            guild.StageInstances = guild.StageInstances.SetItem(stageInstance.Id, stageInstance);
                            guild._jsonModel.StageInstances = guild._jsonModel.StageInstances.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_UPDATE":
                {
                    var json = data.ToObject(JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceUpdate, new(json, Rest), stageInstance =>
                    {
                        if (TryGetGuild(stageInstance.GuildId, out var guild))
                        {
                            guild.StageInstances = guild.StageInstances.SetItem(stageInstance.Id, stageInstance);
                            guild._jsonModel.StageInstances = guild._jsonModel.StageInstances.SetItem(json.Id, json);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_DELETE":
                {
                    var json = data.ToObject(JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceDelete, new(json, Rest), stageInstance =>
                    {
                        if (TryGetGuild(stageInstance.GuildId, out var guild))
                        {
                            guild.StageInstances = guild.StageInstances.Remove(stageInstance.Id);
                            guild._jsonModel.StageInstances = guild._jsonModel.StageInstances.Remove(json.Id);
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "TYPING_START":
                {
                    await InvokeEventAsync(TypingStart, () => new(data.ToObject(JsonTypingStartEventArgs.JsonTypingStartEventArgsSerializerContext.WithOptions.JsonTypingStartEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "USER_UPDATE":
                {
                    await InvokeEventAsync(CurrentUserUpdate, new(data.ToObject(JsonUser.JsonUserSerializerContext.WithOptions.JsonUser), Rest), out _user).ConfigureAwait(false);
                }
                break;
            case "VOICE_STATE_UPDATE":
                {
                    var json = data.ToObject(JsonVoiceState.JsonVoiceStateSerializerContext.WithOptions.JsonVoiceState);
                    await InvokeEventAsync(VoiceStateUpdate, new(json, Rest), voiceState =>
                    {
                        if (TryGetGuild(voiceState.GuildId.GetValueOrDefault(), out var guild))
                        {
                            if (voiceState.ChannelId.HasValue)
                            {
                                guild.VoiceStates = guild.VoiceStates.SetItem(voiceState.UserId, voiceState);
                                guild._jsonModel.VoiceStates = guild._jsonModel.VoiceStates.SetItem(json.UserId, json);
                            }
                            else
                            {
                                guild.VoiceStates = guild.VoiceStates.Remove(voiceState.UserId);
                                guild._jsonModel.VoiceStates = guild._jsonModel.VoiceStates.Remove(json.UserId);
                            }
                        }
                    }).ConfigureAwait(false);
                }
                break;
            case "VOICE_SERVER_UPDATE":
                {
                    await InvokeEventAsync(VoiceServerUpdate, () => new(data.ToObject(JsonVoiceServerUpdateEventArgs.JsonVoiceServerUpdateEventArgsSerializerContext.WithOptions.JsonVoiceServerUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "WEBHOOKS_UPDATE":
                {
                    await InvokeEventAsync(WebhooksUpdate, () => new(data.ToObject(JsonWebhooksUpdateEventArgs.JsonWebhooksUpdateEventArgsSerializerContext.WithOptions.JsonWebhooksUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            default:
                {
                    await InvokeEventAsync(UnknownEvent, () => new(name, data)).ConfigureAwait(false);
                }
                break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ulong GetGuildId() => data.GetProperty("guild_id").ToObject(UInt64Utils.UInt64SerializerContext.WithOptions.UInt64);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryGetGuild(ulong guildId, [NotNullWhen(true)] out Guild? guild) => Guilds.TryGetValue(guildId, out guild);

        async ValueTask CacheChannelAsync(ulong channelId)
        {
            if (!(DMChannels.ContainsKey(channelId) || GroupDMChannels.ContainsKey(channelId)))
            {
                var channel = await Rest.GetChannelAsync(channelId).ConfigureAwait(false);
                if (channel is GroupDMChannel groupDMChannel)
                {
                    lock (_dmsLock!)
                        GroupDMChannels = GroupDMChannels.SetItem(channelId, groupDMChannel);
                }
                else if (channel is DMChannel dMChannel)
                {
                    lock (_dmsLock!)
                        DMChannels = DMChannels.SetItem(channelId, dMChannel);
                }
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GatewayClient));
    }

    public override void Dispose()
    {
        _disposed = true;
        Guilds = null!;
        DMChannels = null!;
        GroupDMChannels = null!;
        if (_configuration.CacheDMChannels)
        {
            foreach (var semaphore in _dmSemaphores!.Values)
                semaphore.Dispose();
        }
        base.Dispose();
    }
}
