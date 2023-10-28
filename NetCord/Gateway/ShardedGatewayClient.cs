using System.Collections;

using NetCord.Rest;

namespace NetCord.Gateway;

public class ShardedGatewayClient : IReadOnlyList<GatewayClient>, IDisposable
{
    private readonly Token _botToken;
    private readonly ShardedGatewayClientConfiguration _configuration;
    private readonly ShardedGatewayClientEventManager _eventManager;

    private readonly object _clientsLock = new();
    private IReadOnlyList<GatewayClient>? _clients;

    private CancellationTokenSource? _startCancellationTokenSource;
    private readonly object _startLock = new();

    private bool _initialized;

    public ShardedGatewayClient(Token token, ShardedGatewayClientConfiguration? configuration = null)
    {
        _botToken = token;
        _configuration = configuration = CreateConfiguration(configuration);
        _eventManager = new();
        Rest = new(token, configuration.RestClientConfiguration);
    }

    private static ShardedGatewayClientConfiguration CreateConfiguration(ShardedGatewayClientConfiguration? configuration)
    {
        if (configuration is null)
        {
            return new()
            {
                WebSocketFactory = _ => null,
                ReconnectTimerFactory = _ => null,
                LatencyTimerFactory = _ => null,
                CompressionFactory = _ => null,
                Hostname = null,
                ConnectionPropertiesFactory = _ => null,
                VersionFactory = _ => ApiVersion.V10,
                IntentsFactory = _ => GatewayIntents.AllNonPrivileged,
                LargeThresholdFactory = _ => null,
                PresenceFactory = _ => null,
                ShardCount = null,
                RestClientConfiguration = null,
            };
        }

        return new()
        {
            WebSocketFactory = configuration.WebSocketFactory ?? (_ => null),
            ReconnectTimerFactory = configuration.ReconnectTimerFactory ?? (_ => null),
            LatencyTimerFactory = configuration.LatencyTimerFactory ?? (_ => null),
            CompressionFactory = configuration.CompressionFactory ?? (_ => null),
            Hostname = configuration.Hostname,
            ConnectionPropertiesFactory = configuration.ConnectionPropertiesFactory ?? (_ => null),
            VersionFactory = configuration.VersionFactory ?? (_ => ApiVersion.V10),
            IntentsFactory = configuration.IntentsFactory ?? (_ => GatewayIntents.AllNonPrivileged),
            LargeThresholdFactory = configuration.LargeThresholdFactory ?? (_ => null),
            PresenceFactory = configuration.PresenceFactory ?? (_ => null),
            ShardCount = configuration.ShardCount,
            CacheDMChannels = configuration.CacheDMChannels,
            RestClientConfiguration = configuration.RestClientConfiguration,
        };
    }

    public RestClient Rest { get; }

    public GatewayClient this[int shardId] => _clients![shardId];

    public GatewayClient this[ulong guildId]
    {
        get
        {
            var clients = _clients!;
            return clients[Snowflake.ShardId(guildId, clients.Count)];
        }
    }

    public int Count => _clients!.Count;

    public async Task StartAsync(Func<Shard, PresenceProperties?>? presenceFactory = null, Func<Shard, IGatewayClientCache?>? cacheFactory = null)
    {
        CancellationTokenSource startCancellationTokenSource;
        lock (_startLock)
        {
            if (_startCancellationTokenSource is not null)
                throw new InvalidOperationException("The client is already connected.");

            startCancellationTokenSource = _startCancellationTokenSource = new();
        }
        var token = startCancellationTokenSource.Token;

        var oldInitialized = _initialized;
        _initialized = false;

        presenceFactory ??= _ => null;
        cacheFactory ??= _ => null;

        try
        {
            var rest = Rest;
            var info = await rest.GetGatewayBotAsync().ConfigureAwait(false);

            var configuration = _configuration;
            var shardCount = configuration.ShardCount ?? info.ShardCount;
            var startLimit = info.SessionStartLimit;
            var total = startLimit.Total;

            if (shardCount > total)
                throw new InvalidOperationException($"Shard count ({shardCount}) is greater than the total ({total}).");

            var remaining = startLimit.Remaining;
            var now = Environment.TickCount64;
            var resetAfter = startLimit.ResetAfter;

            var maxConcurrency = Math.Min(startLimit.MaxConcurrency, shardCount);

            DisposeClients(_clients, oldInitialized);

            var clientsLock = _clientsLock;
            var clients = new GatewayClient[shardCount];
            _clients = clients;

            var tasks = new Task[maxConcurrency];

            var botToken = _botToken;

            await ConnectBucketAsync(0).ConfigureAwait(false);

            for (int bucket = maxConcurrency; bucket < shardCount; bucket += maxConcurrency)
            {
                await Task.Delay(5000, token).ConfigureAwait(false);
                await ConnectBucketAsync(bucket).ConfigureAwait(false);
            }

            _initialized = true;

            async Task ConnectBucketAsync(int bucket)
            {
                int count = Math.Min(maxConcurrency, shardCount - bucket);
                for (int i = 0; i < count; i++)
                {
                    await WaitForRateLimitAsync().ConfigureAwait(false);

                    tasks[i] = ConnectShardAsync(bucket + i);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);

                async ValueTask WaitForRateLimitAsync()
                {
                    if (remaining == 0)
                    {
                        TimeSpan elapsed = new((Environment.TickCount64 - now) * TimeSpan.TicksPerMillisecond);
                        if (elapsed < resetAfter)
                            await Task.Delay(resetAfter - elapsed, token).ConfigureAwait(false);

                        remaining = total - 1;
                    }
                    else
                        remaining--;
                }

                async Task ConnectShardAsync(int shardId)
                {
                    Shard shard = new(shardId, shardCount);
                    var gatewayClientConfiguration = GetGatewayClientConfiguration(shard);
                    GatewayClient client;
                    lock (clientsLock)
                    {
                        token.ThrowIfCancellationRequested();
                        clients[shardId] = client = new(botToken, rest, gatewayClientConfiguration);
                    }
                    HookEvents(client);
                    await client.StartAsync(presenceFactory(shard), cacheFactory(shard)).ConfigureAwait(false);
                }
            }
        }
        catch
        {
            var clients = _clients;
            _clients = null;
            if (clients is not null)
            {
                int count = clients.Count;
                for (int i = 0; i < count; i++)
                {
                    var client = clients[i];
                    if (client is null)
                        break;

                    client.Dispose();
                }
            }
            _startCancellationTokenSource = null;
            throw;
        }
    }

    private GatewayClientConfiguration GetGatewayClientConfiguration(Shard shard)
    {
        var configuration = _configuration;
        return new()
        {
            WebSocket = configuration.WebSocketFactory!(shard),
            ReconnectTimer = configuration.ReconnectTimerFactory!(shard),
            LatencyTimer = configuration.LatencyTimerFactory!(shard),
            Compression = configuration.CompressionFactory!(shard),
            Hostname = configuration.Hostname,
            ConnectionProperties = configuration.ConnectionPropertiesFactory!(shard),
            Version = configuration.VersionFactory!(shard),
            Intents = configuration.IntentsFactory!(shard),
            LargeThreshold = configuration.LargeThresholdFactory!(shard),
            Presence = configuration.PresenceFactory!(shard),
            Shard = shard,
            CacheDMChannels = configuration.CacheDMChannels,
        };
    }

    public async Task CloseAsync(System.Net.WebSockets.WebSocketCloseStatus status = System.Net.WebSockets.WebSocketCloseStatus.NormalClosure)
    {
        var startCancellationTokenSource = _startCancellationTokenSource!;
        var clients = _clients;
        if (startCancellationTokenSource is null || clients is null)
            throw new InvalidOperationException("The client is not connected.");

        lock (_clientsLock)
            startCancellationTokenSource.Cancel();
        startCancellationTokenSource.Dispose();

        await Task.WhenAll((_initialized ? clients : clients.TakeWhile(clients => clients is not null)).Select(client => client.CloseAsync(status))).ConfigureAwait(false);

        _startCancellationTokenSource = null;
    }

    public IEnumerator<GatewayClient> GetEnumerator() => _clients!.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_clients!).GetEnumerator();

    private void HookEvents(GatewayClient client)
    {
        HookEvent(client, _readyLock, ref _ready, a => _ready!(client, a), (c, e) => c.Ready += e);
        HookEvent(client, _latencyUpdateLock, ref _latencyUpdate, a => _latencyUpdate!(client, a), (c, e) => c.LatencyUpdate += e);
        HookEvent(client, _resumeLock, ref _resume, () => _resume!(client), (c, e) => c.Resume += e);
        HookEvent(client, _connectingLock, ref _connecting, () => _connecting!(client), (c, e) => c.Connecting += e);
        HookEvent(client, _connectLock, ref _connect, () => _connect!(client), (c, e) => c.Connect += e);
        HookEvent(client, _disconnectLock, ref _disconnect, a => _disconnect!(client, a), (c, e) => c.Disconnect += e);
        HookEvent(client, _closeLock, ref _close, () => _close!(client), (c, e) => c.Close += e);
        HookEvent(client, _logLock, ref _log, a => _log!(client, a), (c, e) => c.Log += e);
        HookEvent(client, _applicationCommandPermissionsUpdateLock, ref _applicationCommandPermissionsUpdate, a => _applicationCommandPermissionsUpdate!(client, a), (c, e) => c.ApplicationCommandPermissionsUpdate += e);
        HookEvent(client, _autoModerationRuleCreateLock, ref _autoModerationRuleCreate, a => _autoModerationRuleCreate!(client, a), (c, e) => c.AutoModerationRuleCreate += e);
        HookEvent(client, _autoModerationRuleUpdateLock, ref _autoModerationRuleUpdate, a => _autoModerationRuleUpdate!(client, a), (c, e) => c.AutoModerationRuleUpdate += e);
        HookEvent(client, _autoModerationRuleDeleteLock, ref _autoModerationRuleDelete, a => _autoModerationRuleDelete!(client, a), (c, e) => c.AutoModerationRuleDelete += e);
        HookEvent(client, _autoModerationActionExecutionLock, ref _autoModerationActionExecution, a => _autoModerationActionExecution!(client, a), (c, e) => c.AutoModerationActionExecution += e);
        HookEvent(client, _guildChannelCreateLock, ref _guildChannelCreate, a => _guildChannelCreate!(client, a), (c, e) => c.GuildChannelCreate += e);
        HookEvent(client, _guildChannelUpdateLock, ref _guildChannelUpdate, a => _guildChannelUpdate!(client, a), (c, e) => c.GuildChannelUpdate += e);
        HookEvent(client, _guildChannelDeleteLock, ref _guildChannelDelete, a => _guildChannelDelete!(client, a), (c, e) => c.GuildChannelDelete += e);
        HookEvent(client, _channelPinsUpdateLock, ref _channelPinsUpdate, a => _channelPinsUpdate!(client, a), (c, e) => c.ChannelPinsUpdate += e);
        HookEvent(client, _guildThreadCreateLock, ref _guildThreadCreate, a => _guildThreadCreate!(client, a), (c, e) => c.GuildThreadCreate += e);
        HookEvent(client, _guildThreadUpdateLock, ref _guildThreadUpdate, a => _guildThreadUpdate!(client, a), (c, e) => c.GuildThreadUpdate += e);
        HookEvent(client, _guildThreadDeleteLock, ref _guildThreadDelete, a => _guildThreadDelete!(client, a), (c, e) => c.GuildThreadDelete += e);
        HookEvent(client, _guildThreadListSyncLock, ref _guildThreadListSync, a => _guildThreadListSync!(client, a), (c, e) => c.GuildThreadListSync += e);
        HookEvent(client, _guildThreadUserUpdateLock, ref _guildThreadUserUpdate, a => _guildThreadUserUpdate!(client, a), (c, e) => c.GuildThreadUserUpdate += e);
        HookEvent(client, _guildThreadUsersUpdateLock, ref _guildThreadUsersUpdate, a => _guildThreadUsersUpdate!(client, a), (c, e) => c.GuildThreadUsersUpdate += e);
        HookEvent(client, _guildCreateLock, ref _guildCreate, a => _guildCreate!(client, a), (c, e) => c.GuildCreate += e);
        HookEvent(client, _guildUpdateLock, ref _guildUpdate, a => _guildUpdate!(client, a), (c, e) => c.GuildUpdate += e);
        HookEvent(client, _guildDeleteLock, ref _guildDelete, a => _guildDelete!(client, a), (c, e) => c.GuildDelete += e);
        HookEvent(client, _guildAuditLogEntryCreateLock, ref _guildAuditLogEntryCreate, a => _guildAuditLogEntryCreate!(client, a), (c, e) => c.GuildAuditLogEntryCreate += e);
        HookEvent(client, _guildBanAddLock, ref _guildBanAdd, a => _guildBanAdd!(client, a), (c, e) => c.GuildBanAdd += e);
        HookEvent(client, _guildBanRemoveLock, ref _guildBanRemove, a => _guildBanRemove!(client, a), (c, e) => c.GuildBanRemove += e);
        HookEvent(client, _guildEmojisUpdateLock, ref _guildEmojisUpdate, a => _guildEmojisUpdate!(client, a), (c, e) => c.GuildEmojisUpdate += e);
        HookEvent(client, _guildStickersUpdateLock, ref _guildStickersUpdate, a => _guildStickersUpdate!(client, a), (c, e) => c.GuildStickersUpdate += e);
        HookEvent(client, _guildIntegrationsUpdateLock, ref _guildIntegrationsUpdate, a => _guildIntegrationsUpdate!(client, a), (c, e) => c.GuildIntegrationsUpdate += e);
        HookEvent(client, _guildUserAddLock, ref _guildUserAdd, a => _guildUserAdd!(client, a), (c, e) => c.GuildUserAdd += e);
        HookEvent(client, _guildUserUpdateLock, ref _guildUserUpdate, a => _guildUserUpdate!(client, a), (c, e) => c.GuildUserUpdate += e);
        HookEvent(client, _guildUserRemoveLock, ref _guildUserRemove, a => _guildUserRemove!(client, a), (c, e) => c.GuildUserRemove += e);
        HookEvent(client, _guildUserChunkLock, ref _guildUserChunk, a => _guildUserChunk!(client, a), (c, e) => c.GuildUserChunk += e);
        HookEvent(client, _roleCreateLock, ref _roleCreate, a => _roleCreate!(client, a), (c, e) => c.RoleCreate += e);
        HookEvent(client, _roleUpdateLock, ref _roleUpdate, a => _roleUpdate!(client, a), (c, e) => c.RoleUpdate += e);
        HookEvent(client, _roleDeleteLock, ref _roleDelete, a => _roleDelete!(client, a), (c, e) => c.RoleDelete += e);
        HookEvent(client, _guildScheduledEventCreateLock, ref _guildScheduledEventCreate, a => _guildScheduledEventCreate!(client, a), (c, e) => c.GuildScheduledEventCreate += e);
        HookEvent(client, _guildScheduledEventUpdateLock, ref _guildScheduledEventUpdate, a => _guildScheduledEventUpdate!(client, a), (c, e) => c.GuildScheduledEventUpdate += e);
        HookEvent(client, _guildScheduledEventDeleteLock, ref _guildScheduledEventDelete, a => _guildScheduledEventDelete!(client, a), (c, e) => c.GuildScheduledEventDelete += e);
        HookEvent(client, _guildScheduledEventUserAddLock, ref _guildScheduledEventUserAdd, a => _guildScheduledEventUserAdd!(client, a), (c, e) => c.GuildScheduledEventUserAdd += e);
        HookEvent(client, _guildScheduledEventUserRemoveLock, ref _guildScheduledEventUserRemove, a => _guildScheduledEventUserRemove!(client, a), (c, e) => c.GuildScheduledEventUserRemove += e);
        HookEvent(client, _guildIntegrationCreateLock, ref _guildIntegrationCreate, a => _guildIntegrationCreate!(client, a), (c, e) => c.GuildIntegrationCreate += e);
        HookEvent(client, _guildIntegrationUpdateLock, ref _guildIntegrationUpdate, a => _guildIntegrationUpdate!(client, a), (c, e) => c.GuildIntegrationUpdate += e);
        HookEvent(client, _guildIntegrationDeleteLock, ref _guildIntegrationDelete, a => _guildIntegrationDelete!(client, a), (c, e) => c.GuildIntegrationDelete += e);
        HookEvent(client, _guildInviteCreateLock, ref _guildInviteCreate, a => _guildInviteCreate!(client, a), (c, e) => c.GuildInviteCreate += e);
        HookEvent(client, _guildInviteDeleteLock, ref _guildInviteDelete, a => _guildInviteDelete!(client, a), (c, e) => c.GuildInviteDelete += e);
        HookEvent(client, _messageCreateLock, ref _messageCreate, a => _messageCreate!(client, a), (c, e) => c.MessageCreate += e);
        HookEvent(client, _messageUpdateLock, ref _messageUpdate, a => _messageUpdate!(client, a), (c, e) => c.MessageUpdate += e);
        HookEvent(client, _messageDeleteLock, ref _messageDelete, a => _messageDelete!(client, a), (c, e) => c.MessageDelete += e);
        HookEvent(client, _messageDeleteBulkLock, ref _messageDeleteBulk, a => _messageDeleteBulk!(client, a), (c, e) => c.MessageDeleteBulk += e);
        HookEvent(client, _messageReactionAddLock, ref _messageReactionAdd, a => _messageReactionAdd!(client, a), (c, e) => c.MessageReactionAdd += e);
        HookEvent(client, _messageReactionRemoveLock, ref _messageReactionRemove, a => _messageReactionRemove!(client, a), (c, e) => c.MessageReactionRemove += e);
        HookEvent(client, _messageReactionRemoveAllLock, ref _messageReactionRemoveAll, a => _messageReactionRemoveAll!(client, a), (c, e) => c.MessageReactionRemoveAll += e);
        HookEvent(client, _messageReactionRemoveEmojiLock, ref _messageReactionRemoveEmoji, a => _messageReactionRemoveEmoji!(client, a), (c, e) => c.MessageReactionRemoveEmoji += e);
        HookEvent(client, _presenceUpdateLock, ref _presenceUpdate, a => _presenceUpdate!(client, a), (c, e) => c.PresenceUpdate += e);
        HookEvent(client, _typingStartLock, ref _typingStart, a => _typingStart!(client, a), (c, e) => c.TypingStart += e);
        HookEvent(client, _currentUserUpdateLock, ref _currentUserUpdate, a => _currentUserUpdate!(client, a), (c, e) => c.CurrentUserUpdate += e);
        HookEvent(client, _voiceStateUpdateLock, ref _voiceStateUpdate, a => _voiceStateUpdate!(client, a), (c, e) => c.VoiceStateUpdate += e);
        HookEvent(client, _voiceServerUpdateLock, ref _voiceServerUpdate, a => _voiceServerUpdate!(client, a), (c, e) => c.VoiceServerUpdate += e);
        HookEvent(client, _webhooksUpdateLock, ref _webhooksUpdate, a => _webhooksUpdate!(client, a), (c, e) => c.WebhooksUpdate += e);
        HookEvent(client, _interactionCreateLock, ref _interactionCreate, a => _interactionCreate!(client, a), (c, e) => c.InteractionCreate += e);
        HookEvent(client, _stageInstanceCreateLock, ref _stageInstanceCreate, a => _stageInstanceCreate!(client, a), (c, e) => c.StageInstanceCreate += e);
        HookEvent(client, _stageInstanceUpdateLock, ref _stageInstanceUpdate, a => _stageInstanceUpdate!(client, a), (c, e) => c.StageInstanceUpdate += e);
        HookEvent(client, _stageInstanceDeleteLock, ref _stageInstanceDelete, a => _stageInstanceDelete!(client, a), (c, e) => c.StageInstanceDelete += e);
        HookEvent(client, _entitlementCreateLock, ref _entitlementCreate, a => _entitlementCreate!(client, a), (c, e) => c.EntitlementCreate += e);
        HookEvent(client, _entitlementUpdateLock, ref _entitlementUpdate, a => _entitlementUpdate!(client, a), (c, e) => c.EntitlementUpdate += e);
        HookEvent(client, _entitlementDeleteLock, ref _entitlementDelete, a => _entitlementDelete!(client, a), (c, e) => c.EntitlementDelete += e);
        HookEvent(client, _unknownEventLock, ref _unknownEvent, a => _unknownEvent!(client, a), (c, e) => c.UnknownEvent += e);
    }

    private readonly object _readyLock = new();
    private Func<GatewayClient, ReadyEventArgs, ValueTask>? _ready;
    public event Func<GatewayClient, ReadyEventArgs, ValueTask>? Ready
    {
        add
        {
            HookEvent(_readyLock, value, ref _ready, client => a => _ready!(client, a), (c, e) => c.Ready += e);
        }
        remove
        {
            UnhookEvent(_readyLock, value, ref _ready, (c, e) => c.Ready -= e);
        }
    }

    private readonly object _latencyUpdateLock = new();
    private Func<GatewayClient, TimeSpan, ValueTask>? _latencyUpdate;
    public event Func<GatewayClient, TimeSpan, ValueTask>? LatencyUpdate
    {
        add
        {
            HookEvent(_latencyUpdateLock, value, ref _latencyUpdate, client => a => _latencyUpdate!(client, a), (c, e) => c.LatencyUpdate += e);
        }
        remove
        {
            UnhookEvent(_latencyUpdateLock, value, ref _latencyUpdate, (c, e) => c.LatencyUpdate -= e);
        }
    }

    private readonly object _resumeLock = new();
    private Func<GatewayClient, ValueTask>? _resume;
    public event Func<GatewayClient, ValueTask>? Resume
    {
        add
        {
            HookEvent(_resumeLock, value, ref _resume, client => () => _resume!(client), (c, e) => c.Resume += e);
        }
        remove
        {
            UnhookEvent(_resumeLock, value, ref _resume, (c, e) => c.Resume -= e);
        }
    }

    private readonly object _connectingLock = new();
    private Func<GatewayClient, ValueTask>? _connecting;
    public event Func<GatewayClient, ValueTask>? Connecting
    {
        add
        {
            HookEvent(_connectingLock, value, ref _connecting, client => () => _connecting!(client), (c, e) => c.Connecting += e);
        }
        remove
        {
            UnhookEvent(_connectingLock, value, ref _connecting, (c, e) => c.Connecting -= e);
        }
    }

    private readonly object _connectLock = new();
    private Func<GatewayClient, ValueTask>? _connect;
    public event Func<GatewayClient, ValueTask>? Connect
    {
        add
        {
            HookEvent(_connectLock, value, ref _connect, client => () => _connect!(client), (c, e) => c.Connect += e);
        }
        remove
        {
            UnhookEvent(_connectLock, value, ref _connect, (c, e) => c.Connect -= e);
        }
    }

    private readonly object _disconnectLock = new();
    private Func<GatewayClient, bool, ValueTask>? _disconnect;
    public event Func<GatewayClient, bool, ValueTask>? Disconnect
    {
        add
        {
            HookEvent(_disconnectLock, value, ref _disconnect, client => a => _disconnect!(client, a), (c, e) => c.Disconnect += e);
        }
        remove
        {
            UnhookEvent(_disconnectLock, value, ref _disconnect, (c, e) => c.Disconnect -= e);
        }
    }

    private readonly object _closeLock = new();
    private Func<GatewayClient, ValueTask>? _close;
    public event Func<GatewayClient, ValueTask>? Close
    {
        add
        {
            HookEvent(_closeLock, value, ref _close, client => () => _close!(client), (c, e) => c.Close += e);
        }
        remove
        {
            UnhookEvent(_closeLock, value, ref _close, (c, e) => c.Close -= e);
        }
    }

    private readonly object _logLock = new();
    private Func<GatewayClient, LogMessage, ValueTask>? _log;
    public event Func<GatewayClient, LogMessage, ValueTask>? Log
    {
        add
        {
            HookEvent(_logLock, value, ref _log, client => a => _log!(client, a), (c, e) => c.Log += e);
        }
        remove
        {
            UnhookEvent(_logLock, value, ref _log, (c, e) => c.Log -= e);
        }
    }

    private readonly object _applicationCommandPermissionsUpdateLock = new();
    private Func<GatewayClient, ApplicationCommandPermission, ValueTask>? _applicationCommandPermissionsUpdate;
    public event Func<GatewayClient, ApplicationCommandPermission, ValueTask>? ApplicationCommandPermissionsUpdate
    {
        add
        {
            HookEvent(_applicationCommandPermissionsUpdateLock, value, ref _applicationCommandPermissionsUpdate, client => a => _applicationCommandPermissionsUpdate!(client, a), (c, e) => c.ApplicationCommandPermissionsUpdate += e);
        }
        remove
        {
            UnhookEvent(_applicationCommandPermissionsUpdateLock, value, ref _applicationCommandPermissionsUpdate, (c, e) => c.ApplicationCommandPermissionsUpdate -= e);
        }
    }

    private readonly object _autoModerationRuleCreateLock = new();
    private Func<GatewayClient, AutoModerationRule, ValueTask>? _autoModerationRuleCreate;
    public event Func<GatewayClient, AutoModerationRule, ValueTask>? AutoModerationRuleCreate
    {
        add
        {
            HookEvent(_autoModerationRuleCreateLock, value, ref _autoModerationRuleCreate, client => a => _autoModerationRuleCreate!(client, a), (c, e) => c.AutoModerationRuleCreate += e);
        }
        remove
        {
            UnhookEvent(_autoModerationRuleCreateLock, value, ref _autoModerationRuleCreate, (c, e) => c.AutoModerationRuleCreate -= e);
        }
    }

    private readonly object _autoModerationRuleUpdateLock = new();
    private Func<GatewayClient, AutoModerationRule, ValueTask>? _autoModerationRuleUpdate;
    public event Func<GatewayClient, AutoModerationRule, ValueTask>? AutoModerationRuleUpdate
    {
        add
        {
            HookEvent(_autoModerationRuleUpdateLock, value, ref _autoModerationRuleUpdate, client => a => _autoModerationRuleUpdate!(client, a), (c, e) => c.AutoModerationRuleUpdate += e);
        }
        remove
        {
            UnhookEvent(_autoModerationRuleUpdateLock, value, ref _autoModerationRuleUpdate, (c, e) => c.AutoModerationRuleUpdate -= e);
        }
    }

    private readonly object _autoModerationRuleDeleteLock = new();
    private Func<GatewayClient, AutoModerationRule, ValueTask>? _autoModerationRuleDelete;
    public event Func<GatewayClient, AutoModerationRule, ValueTask>? AutoModerationRuleDelete
    {
        add
        {
            HookEvent(_autoModerationRuleDeleteLock, value, ref _autoModerationRuleDelete, client => a => _autoModerationRuleDelete!(client, a), (c, e) => c.AutoModerationRuleDelete += e);
        }
        remove
        {
            UnhookEvent(_autoModerationRuleDeleteLock, value, ref _autoModerationRuleDelete, (c, e) => c.AutoModerationRuleDelete -= e);
        }
    }

    private readonly object _autoModerationActionExecutionLock = new();
    private Func<GatewayClient, AutoModerationActionExecutionEventArgs, ValueTask>? _autoModerationActionExecution;
    public event Func<GatewayClient, AutoModerationActionExecutionEventArgs, ValueTask>? AutoModerationActionExecution
    {
        add
        {
            HookEvent(_autoModerationActionExecutionLock, value, ref _autoModerationActionExecution, client => a => _autoModerationActionExecution!(client, a), (c, e) => c.AutoModerationActionExecution += e);
        }
        remove
        {
            UnhookEvent(_autoModerationActionExecutionLock, value, ref _autoModerationActionExecution, (c, e) => c.AutoModerationActionExecution -= e);
        }
    }

    private readonly object _guildChannelCreateLock = new();
    private Func<GatewayClient, GuildChannelEventArgs, ValueTask>? _guildChannelCreate;
    public event Func<GatewayClient, GuildChannelEventArgs, ValueTask>? GuildChannelCreate
    {
        add
        {
            HookEvent(_guildChannelCreateLock, value, ref _guildChannelCreate, client => a => _guildChannelCreate!(client, a), (c, e) => c.GuildChannelCreate += e);
        }
        remove
        {
            UnhookEvent(_guildChannelCreateLock, value, ref _guildChannelCreate, (c, e) => c.GuildChannelCreate -= e);
        }
    }

    private readonly object _guildChannelUpdateLock = new();
    private Func<GatewayClient, GuildChannelEventArgs, ValueTask>? _guildChannelUpdate;
    public event Func<GatewayClient, GuildChannelEventArgs, ValueTask>? GuildChannelUpdate
    {
        add
        {
            HookEvent(_guildChannelUpdateLock, value, ref _guildChannelUpdate, client => a => _guildChannelUpdate!(client, a), (c, e) => c.GuildChannelUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildChannelUpdateLock, value, ref _guildChannelUpdate, (c, e) => c.GuildChannelUpdate -= e);
        }
    }

    private readonly object _guildChannelDeleteLock = new();
    private Func<GatewayClient, GuildChannelEventArgs, ValueTask>? _guildChannelDelete;
    public event Func<GatewayClient, GuildChannelEventArgs, ValueTask>? GuildChannelDelete
    {
        add
        {
            HookEvent(_guildChannelDeleteLock, value, ref _guildChannelDelete, client => a => _guildChannelDelete!(client, a), (c, e) => c.GuildChannelDelete += e);
        }
        remove
        {
            UnhookEvent(_guildChannelDeleteLock, value, ref _guildChannelDelete, (c, e) => c.GuildChannelDelete -= e);
        }
    }

    private readonly object _channelPinsUpdateLock = new();
    private Func<GatewayClient, ChannelPinsUpdateEventArgs, ValueTask>? _channelPinsUpdate;
    public event Func<GatewayClient, ChannelPinsUpdateEventArgs, ValueTask>? ChannelPinsUpdate
    {
        add
        {
            HookEvent(_channelPinsUpdateLock, value, ref _channelPinsUpdate, client => a => _channelPinsUpdate!(client, a), (c, e) => c.ChannelPinsUpdate += e);
        }
        remove
        {
            UnhookEvent(_channelPinsUpdateLock, value, ref _channelPinsUpdate, (c, e) => c.ChannelPinsUpdate -= e);
        }
    }

    private readonly object _guildThreadCreateLock = new();
    private Func<GatewayClient, GuildThreadCreateEventArgs, ValueTask>? _guildThreadCreate;
    public event Func<GatewayClient, GuildThreadCreateEventArgs, ValueTask>? GuildThreadCreate
    {
        add
        {
            HookEvent(_guildThreadCreateLock, value, ref _guildThreadCreate, client => a => _guildThreadCreate!(client, a), (c, e) => c.GuildThreadCreate += e);
        }
        remove
        {
            UnhookEvent(_guildThreadCreateLock, value, ref _guildThreadCreate, (c, e) => c.GuildThreadCreate -= e);
        }
    }

    private readonly object _guildThreadUpdateLock = new();
    private Func<GatewayClient, GuildThread, ValueTask>? _guildThreadUpdate;
    public event Func<GatewayClient, GuildThread, ValueTask>? GuildThreadUpdate
    {
        add
        {
            HookEvent(_guildThreadUpdateLock, value, ref _guildThreadUpdate, client => a => _guildThreadUpdate!(client, a), (c, e) => c.GuildThreadUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildThreadUpdateLock, value, ref _guildThreadUpdate, (c, e) => c.GuildThreadUpdate -= e);
        }
    }

    private readonly object _guildThreadDeleteLock = new();
    private Func<GatewayClient, GuildThreadDeleteEventArgs, ValueTask>? _guildThreadDelete;
    public event Func<GatewayClient, GuildThreadDeleteEventArgs, ValueTask>? GuildThreadDelete
    {
        add
        {
            HookEvent(_guildThreadDeleteLock, value, ref _guildThreadDelete, client => a => _guildThreadDelete!(client, a), (c, e) => c.GuildThreadDelete += e);
        }
        remove
        {
            UnhookEvent(_guildThreadDeleteLock, value, ref _guildThreadDelete, (c, e) => c.GuildThreadDelete -= e);
        }
    }

    private readonly object _guildThreadListSyncLock = new();
    private Func<GatewayClient, GuildThreadListSyncEventArgs, ValueTask>? _guildThreadListSync;
    public event Func<GatewayClient, GuildThreadListSyncEventArgs, ValueTask>? GuildThreadListSync
    {
        add
        {
            HookEvent(_guildThreadListSyncLock, value, ref _guildThreadListSync, client => a => _guildThreadListSync!(client, a), (c, e) => c.GuildThreadListSync += e);
        }
        remove
        {
            UnhookEvent(_guildThreadListSyncLock, value, ref _guildThreadListSync, (c, e) => c.GuildThreadListSync -= e);
        }
    }

    private readonly object _guildThreadUserUpdateLock = new();
    private Func<GatewayClient, GuildThreadUserUpdateEventArgs, ValueTask>? _guildThreadUserUpdate;
    public event Func<GatewayClient, GuildThreadUserUpdateEventArgs, ValueTask>? GuildThreadUserUpdate
    {
        add
        {
            HookEvent(_guildThreadUserUpdateLock, value, ref _guildThreadUserUpdate, client => a => _guildThreadUserUpdate!(client, a), (c, e) => c.GuildThreadUserUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildThreadUserUpdateLock, value, ref _guildThreadUserUpdate, (c, e) => c.GuildThreadUserUpdate -= e);
        }
    }

    private readonly object _guildThreadUsersUpdateLock = new();
    private Func<GatewayClient, GuildThreadUsersUpdateEventArgs, ValueTask>? _guildThreadUsersUpdate;
    public event Func<GatewayClient, GuildThreadUsersUpdateEventArgs, ValueTask>? GuildThreadUsersUpdate
    {
        add
        {
            HookEvent(_guildThreadUsersUpdateLock, value, ref _guildThreadUsersUpdate, client => a => _guildThreadUsersUpdate!(client, a), (c, e) => c.GuildThreadUsersUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildThreadUsersUpdateLock, value, ref _guildThreadUsersUpdate, (c, e) => c.GuildThreadUsersUpdate -= e);
        }
    }

    private readonly object _guildCreateLock = new();
    private Func<GatewayClient, GuildCreateEventArgs, ValueTask>? _guildCreate;
    public event Func<GatewayClient, GuildCreateEventArgs, ValueTask>? GuildCreate
    {
        add
        {
            HookEvent(_guildCreateLock, value, ref _guildCreate, client => a => _guildCreate!(client, a), (c, e) => c.GuildCreate += e);
        }
        remove
        {
            UnhookEvent(_guildCreateLock, value, ref _guildCreate, (c, e) => c.GuildCreate -= e);
        }
    }

    private readonly object _guildUpdateLock = new();
    private Func<GatewayClient, Guild, ValueTask>? _guildUpdate;
    public event Func<GatewayClient, Guild, ValueTask>? GuildUpdate
    {
        add
        {
            HookEvent(_guildUpdateLock, value, ref _guildUpdate, client => a => _guildUpdate!(client, a), (c, e) => c.GuildUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildUpdateLock, value, ref _guildUpdate, (c, e) => c.GuildUpdate -= e);
        }
    }

    private readonly object _guildDeleteLock = new();
    private Func<GatewayClient, GuildDeleteEventArgs, ValueTask>? _guildDelete;
    public event Func<GatewayClient, GuildDeleteEventArgs, ValueTask>? GuildDelete
    {
        add
        {
            HookEvent(_guildDeleteLock, value, ref _guildDelete, client => a => _guildDelete!(client, a), (c, e) => c.GuildDelete += e);
        }
        remove
        {
            UnhookEvent(_guildDeleteLock, value, ref _guildDelete, (c, e) => c.GuildDelete -= e);
        }
    }

    private readonly object _guildAuditLogEntryCreateLock = new();
    private Func<GatewayClient, AuditLogEntry, ValueTask>? _guildAuditLogEntryCreate;
    public event Func<GatewayClient, AuditLogEntry, ValueTask>? GuildAuditLogEntryCreate
    {
        add
        {
            HookEvent(_guildAuditLogEntryCreateLock, value, ref _guildAuditLogEntryCreate, client => a => _guildAuditLogEntryCreate!(client, a), (c, e) => c.GuildAuditLogEntryCreate += e);
        }
        remove
        {
            UnhookEvent(_guildAuditLogEntryCreateLock, value, ref _guildAuditLogEntryCreate, (c, e) => c.GuildAuditLogEntryCreate -= e);
        }
    }

    private readonly object _guildBanAddLock = new();
    private Func<GatewayClient, GuildBanEventArgs, ValueTask>? _guildBanAdd;
    public event Func<GatewayClient, GuildBanEventArgs, ValueTask>? GuildBanAdd
    {
        add
        {
            HookEvent(_guildBanAddLock, value, ref _guildBanAdd, client => a => _guildBanAdd!(client, a), (c, e) => c.GuildBanAdd += e);
        }
        remove
        {
            UnhookEvent(_guildBanAddLock, value, ref _guildBanAdd, (c, e) => c.GuildBanAdd -= e);
        }
    }

    private readonly object _guildBanRemoveLock = new();
    private Func<GatewayClient, GuildBanEventArgs, ValueTask>? _guildBanRemove;
    public event Func<GatewayClient, GuildBanEventArgs, ValueTask>? GuildBanRemove
    {
        add
        {
            HookEvent(_guildBanRemoveLock, value, ref _guildBanRemove, client => a => _guildBanRemove!(client, a), (c, e) => c.GuildBanRemove += e);
        }
        remove
        {
            UnhookEvent(_guildBanRemoveLock, value, ref _guildBanRemove, (c, e) => c.GuildBanRemove -= e);
        }
    }

    private readonly object _guildEmojisUpdateLock = new();
    private Func<GatewayClient, GuildEmojisUpdateEventArgs, ValueTask>? _guildEmojisUpdate;
    public event Func<GatewayClient, GuildEmojisUpdateEventArgs, ValueTask>? GuildEmojisUpdate
    {
        add
        {
            HookEvent(_guildEmojisUpdateLock, value, ref _guildEmojisUpdate, client => a => _guildEmojisUpdate!(client, a), (c, e) => c.GuildEmojisUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildEmojisUpdateLock, value, ref _guildEmojisUpdate, (c, e) => c.GuildEmojisUpdate -= e);
        }
    }

    private readonly object _guildStickersUpdateLock = new();
    private Func<GatewayClient, GuildStickersUpdateEventArgs, ValueTask>? _guildStickersUpdate;
    public event Func<GatewayClient, GuildStickersUpdateEventArgs, ValueTask>? GuildStickersUpdate
    {
        add
        {
            HookEvent(_guildStickersUpdateLock, value, ref _guildStickersUpdate, client => a => _guildStickersUpdate!(client, a), (c, e) => c.GuildStickersUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildStickersUpdateLock, value, ref _guildStickersUpdate, (c, e) => c.GuildStickersUpdate -= e);
        }
    }

    private readonly object _guildIntegrationsUpdateLock = new();
    private Func<GatewayClient, GuildIntegrationsUpdateEventArgs, ValueTask>? _guildIntegrationsUpdate;
    public event Func<GatewayClient, GuildIntegrationsUpdateEventArgs, ValueTask>? GuildIntegrationsUpdate
    {
        add
        {
            HookEvent(_guildIntegrationsUpdateLock, value, ref _guildIntegrationsUpdate, client => a => _guildIntegrationsUpdate!(client, a), (c, e) => c.GuildIntegrationsUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildIntegrationsUpdateLock, value, ref _guildIntegrationsUpdate, (c, e) => c.GuildIntegrationsUpdate -= e);
        }
    }

    private readonly object _guildUserAddLock = new();
    private Func<GatewayClient, GuildUser, ValueTask>? _guildUserAdd;
    public event Func<GatewayClient, GuildUser, ValueTask>? GuildUserAdd
    {
        add
        {
            HookEvent(_guildUserAddLock, value, ref _guildUserAdd, client => a => _guildUserAdd!(client, a), (c, e) => c.GuildUserAdd += e);
        }
        remove
        {
            UnhookEvent(_guildUserAddLock, value, ref _guildUserAdd, (c, e) => c.GuildUserAdd -= e);
        }
    }

    private readonly object _guildUserUpdateLock = new();
    private Func<GatewayClient, GuildUser, ValueTask>? _guildUserUpdate;
    public event Func<GatewayClient, GuildUser, ValueTask>? GuildUserUpdate
    {
        add
        {
            HookEvent(_guildUserUpdateLock, value, ref _guildUserUpdate, client => a => _guildUserUpdate!(client, a), (c, e) => c.GuildUserUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildUserUpdateLock, value, ref _guildUserUpdate, (c, e) => c.GuildUserUpdate -= e);
        }
    }

    private readonly object _guildUserRemoveLock = new();
    private Func<GatewayClient, GuildUserRemoveEventArgs, ValueTask>? _guildUserRemove;
    public event Func<GatewayClient, GuildUserRemoveEventArgs, ValueTask>? GuildUserRemove
    {
        add
        {
            HookEvent(_guildUserRemoveLock, value, ref _guildUserRemove, client => a => _guildUserRemove!(client, a), (c, e) => c.GuildUserRemove += e);
        }
        remove
        {
            UnhookEvent(_guildUserRemoveLock, value, ref _guildUserRemove, (c, e) => c.GuildUserRemove -= e);
        }
    }

    private readonly object _guildUserChunkLock = new();
    private Func<GatewayClient, GuildUserChunkEventArgs, ValueTask>? _guildUserChunk;
    public event Func<GatewayClient, GuildUserChunkEventArgs, ValueTask>? GuildUserChunk
    {
        add
        {
            HookEvent(_guildUserChunkLock, value, ref _guildUserChunk, client => a => _guildUserChunk!(client, a), (c, e) => c.GuildUserChunk += e);
        }
        remove
        {
            UnhookEvent(_guildUserChunkLock, value, ref _guildUserChunk, (c, e) => c.GuildUserChunk -= e);
        }
    }

    private readonly object _roleCreateLock = new();
    private Func<GatewayClient, RoleEventArgs, ValueTask>? _roleCreate;
    public event Func<GatewayClient, RoleEventArgs, ValueTask>? RoleCreate
    {
        add
        {
            HookEvent(_roleCreateLock, value, ref _roleCreate, client => a => _roleCreate!(client, a), (c, e) => c.RoleCreate += e);
        }
        remove
        {
            UnhookEvent(_roleCreateLock, value, ref _roleCreate, (c, e) => c.RoleCreate -= e);
        }
    }

    private readonly object _roleUpdateLock = new();
    private Func<GatewayClient, RoleEventArgs, ValueTask>? _roleUpdate;
    public event Func<GatewayClient, RoleEventArgs, ValueTask>? RoleUpdate
    {
        add
        {
            HookEvent(_roleUpdateLock, value, ref _roleUpdate, client => a => _roleUpdate!(client, a), (c, e) => c.RoleUpdate += e);
        }
        remove
        {
            UnhookEvent(_roleUpdateLock, value, ref _roleUpdate, (c, e) => c.RoleUpdate -= e);
        }
    }

    private readonly object _roleDeleteLock = new();
    private Func<GatewayClient, RoleDeleteEventArgs, ValueTask>? _roleDelete;
    public event Func<GatewayClient, RoleDeleteEventArgs, ValueTask>? RoleDelete
    {
        add
        {
            HookEvent(_roleDeleteLock, value, ref _roleDelete, client => a => _roleDelete!(client, a), (c, e) => c.RoleDelete += e);
        }
        remove
        {
            UnhookEvent(_roleDeleteLock, value, ref _roleDelete, (c, e) => c.RoleDelete -= e);
        }
    }

    private readonly object _guildScheduledEventCreateLock = new();
    private Func<GatewayClient, GuildScheduledEvent, ValueTask>? _guildScheduledEventCreate;
    public event Func<GatewayClient, GuildScheduledEvent, ValueTask>? GuildScheduledEventCreate
    {
        add
        {
            HookEvent(_guildScheduledEventCreateLock, value, ref _guildScheduledEventCreate, client => a => _guildScheduledEventCreate!(client, a), (c, e) => c.GuildScheduledEventCreate += e);
        }
        remove
        {
            UnhookEvent(_guildScheduledEventCreateLock, value, ref _guildScheduledEventCreate, (c, e) => c.GuildScheduledEventCreate -= e);
        }
    }

    private readonly object _guildScheduledEventUpdateLock = new();
    private Func<GatewayClient, GuildScheduledEvent, ValueTask>? _guildScheduledEventUpdate;
    public event Func<GatewayClient, GuildScheduledEvent, ValueTask>? GuildScheduledEventUpdate
    {
        add
        {
            HookEvent(_guildScheduledEventUpdateLock, value, ref _guildScheduledEventUpdate, client => a => _guildScheduledEventUpdate!(client, a), (c, e) => c.GuildScheduledEventUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildScheduledEventUpdateLock, value, ref _guildScheduledEventUpdate, (c, e) => c.GuildScheduledEventUpdate -= e);
        }
    }

    private readonly object _guildScheduledEventDeleteLock = new();
    private Func<GatewayClient, GuildScheduledEvent, ValueTask>? _guildScheduledEventDelete;
    public event Func<GatewayClient, GuildScheduledEvent, ValueTask>? GuildScheduledEventDelete
    {
        add
        {
            HookEvent(_guildScheduledEventDeleteLock, value, ref _guildScheduledEventDelete, client => a => _guildScheduledEventDelete!(client, a), (c, e) => c.GuildScheduledEventDelete += e);
        }
        remove
        {
            UnhookEvent(_guildScheduledEventDeleteLock, value, ref _guildScheduledEventDelete, (c, e) => c.GuildScheduledEventDelete -= e);
        }
    }

    private readonly object _guildScheduledEventUserAddLock = new();
    private Func<GatewayClient, GuildScheduledEventUserEventArgs, ValueTask>? _guildScheduledEventUserAdd;
    public event Func<GatewayClient, GuildScheduledEventUserEventArgs, ValueTask>? GuildScheduledEventUserAdd
    {
        add
        {
            HookEvent(_guildScheduledEventUserAddLock, value, ref _guildScheduledEventUserAdd, client => a => _guildScheduledEventUserAdd!(client, a), (c, e) => c.GuildScheduledEventUserAdd += e);
        }
        remove
        {
            UnhookEvent(_guildScheduledEventUserAddLock, value, ref _guildScheduledEventUserAdd, (c, e) => c.GuildScheduledEventUserAdd -= e);
        }
    }

    private readonly object _guildScheduledEventUserRemoveLock = new();
    private Func<GatewayClient, GuildScheduledEventUserEventArgs, ValueTask>? _guildScheduledEventUserRemove;
    public event Func<GatewayClient, GuildScheduledEventUserEventArgs, ValueTask>? GuildScheduledEventUserRemove
    {
        add
        {
            HookEvent(_guildScheduledEventUserRemoveLock, value, ref _guildScheduledEventUserRemove, client => a => _guildScheduledEventUserRemove!(client, a), (c, e) => c.GuildScheduledEventUserRemove += e);
        }
        remove
        {
            UnhookEvent(_guildScheduledEventUserRemoveLock, value, ref _guildScheduledEventUserRemove, (c, e) => c.GuildScheduledEventUserRemove -= e);
        }
    }

    private readonly object _guildIntegrationCreateLock = new();
    private Func<GatewayClient, GuildIntegrationEventArgs, ValueTask>? _guildIntegrationCreate;
    public event Func<GatewayClient, GuildIntegrationEventArgs, ValueTask>? GuildIntegrationCreate
    {
        add
        {
            HookEvent(_guildIntegrationCreateLock, value, ref _guildIntegrationCreate, client => a => _guildIntegrationCreate!(client, a), (c, e) => c.GuildIntegrationCreate += e);
        }
        remove
        {
            UnhookEvent(_guildIntegrationCreateLock, value, ref _guildIntegrationCreate, (c, e) => c.GuildIntegrationCreate -= e);
        }
    }

    private readonly object _guildIntegrationUpdateLock = new();
    private Func<GatewayClient, GuildIntegrationEventArgs, ValueTask>? _guildIntegrationUpdate;
    public event Func<GatewayClient, GuildIntegrationEventArgs, ValueTask>? GuildIntegrationUpdate
    {
        add
        {
            HookEvent(_guildIntegrationUpdateLock, value, ref _guildIntegrationUpdate, client => a => _guildIntegrationUpdate!(client, a), (c, e) => c.GuildIntegrationUpdate += e);
        }
        remove
        {
            UnhookEvent(_guildIntegrationUpdateLock, value, ref _guildIntegrationUpdate, (c, e) => c.GuildIntegrationUpdate -= e);
        }
    }

    private readonly object _guildIntegrationDeleteLock = new();
    private Func<GatewayClient, GuildIntegrationDeleteEventArgs, ValueTask>? _guildIntegrationDelete;
    public event Func<GatewayClient, GuildIntegrationDeleteEventArgs, ValueTask>? GuildIntegrationDelete
    {
        add
        {
            HookEvent(_guildIntegrationDeleteLock, value, ref _guildIntegrationDelete, client => a => _guildIntegrationDelete!(client, a), (c, e) => c.GuildIntegrationDelete += e);
        }
        remove
        {
            UnhookEvent(_guildIntegrationDeleteLock, value, ref _guildIntegrationDelete, (c, e) => c.GuildIntegrationDelete -= e);
        }
    }

    private readonly object _guildInviteCreateLock = new();
    private Func<GatewayClient, GuildInvite, ValueTask>? _guildInviteCreate;
    public event Func<GatewayClient, GuildInvite, ValueTask>? GuildInviteCreate
    {
        add
        {
            HookEvent(_guildInviteCreateLock, value, ref _guildInviteCreate, client => a => _guildInviteCreate!(client, a), (c, e) => c.GuildInviteCreate += e);
        }
        remove
        {
            UnhookEvent(_guildInviteCreateLock, value, ref _guildInviteCreate, (c, e) => c.GuildInviteCreate -= e);
        }
    }

    private readonly object _guildInviteDeleteLock = new();
    private Func<GatewayClient, GuildInviteDeleteEventArgs, ValueTask>? _guildInviteDelete;
    public event Func<GatewayClient, GuildInviteDeleteEventArgs, ValueTask>? GuildInviteDelete
    {
        add
        {
            HookEvent(_guildInviteDeleteLock, value, ref _guildInviteDelete, client => a => _guildInviteDelete!(client, a), (c, e) => c.GuildInviteDelete += e);
        }
        remove
        {
            UnhookEvent(_guildInviteDeleteLock, value, ref _guildInviteDelete, (c, e) => c.GuildInviteDelete -= e);
        }
    }

    private readonly object _messageCreateLock = new();
    private Func<GatewayClient, Message, ValueTask>? _messageCreate;
    public event Func<GatewayClient, Message, ValueTask>? MessageCreate
    {
        add
        {
            HookEvent(_messageCreateLock, value, ref _messageCreate, client => a => _messageCreate!(client, a), (c, e) => c.MessageCreate += e);
        }
        remove
        {
            UnhookEvent(_messageCreateLock, value, ref _messageCreate, (c, e) => c.MessageCreate -= e);
        }
    }

    private readonly object _messageUpdateLock = new();
    private Func<GatewayClient, Message, ValueTask>? _messageUpdate;
    public event Func<GatewayClient, Message, ValueTask>? MessageUpdate
    {
        add
        {
            HookEvent(_messageUpdateLock, value, ref _messageUpdate, client => a => _messageUpdate!(client, a), (c, e) => c.MessageUpdate += e);
        }
        remove
        {
            UnhookEvent(_messageUpdateLock, value, ref _messageUpdate, (c, e) => c.MessageUpdate -= e);
        }
    }

    private readonly object _messageDeleteLock = new();
    private Func<GatewayClient, MessageDeleteEventArgs, ValueTask>? _messageDelete;
    public event Func<GatewayClient, MessageDeleteEventArgs, ValueTask>? MessageDelete
    {
        add
        {
            HookEvent(_messageDeleteLock, value, ref _messageDelete, client => a => _messageDelete!(client, a), (c, e) => c.MessageDelete += e);
        }
        remove
        {
            UnhookEvent(_messageDeleteLock, value, ref _messageDelete, (c, e) => c.MessageDelete -= e);
        }
    }

    private readonly object _messageDeleteBulkLock = new();
    private Func<GatewayClient, MessageDeleteBulkEventArgs, ValueTask>? _messageDeleteBulk;
    public event Func<GatewayClient, MessageDeleteBulkEventArgs, ValueTask>? MessageDeleteBulk
    {
        add
        {
            HookEvent(_messageDeleteBulkLock, value, ref _messageDeleteBulk, client => a => _messageDeleteBulk!(client, a), (c, e) => c.MessageDeleteBulk += e);
        }
        remove
        {
            UnhookEvent(_messageDeleteBulkLock, value, ref _messageDeleteBulk, (c, e) => c.MessageDeleteBulk -= e);
        }
    }

    private readonly object _messageReactionAddLock = new();
    private Func<GatewayClient, MessageReactionAddEventArgs, ValueTask>? _messageReactionAdd;
    public event Func<GatewayClient, MessageReactionAddEventArgs, ValueTask>? MessageReactionAdd
    {
        add
        {
            HookEvent(_messageReactionAddLock, value, ref _messageReactionAdd, client => a => _messageReactionAdd!(client, a), (c, e) => c.MessageReactionAdd += e);
        }
        remove
        {
            UnhookEvent(_messageReactionAddLock, value, ref _messageReactionAdd, (c, e) => c.MessageReactionAdd -= e);
        }
    }

    private readonly object _messageReactionRemoveLock = new();
    private Func<GatewayClient, MessageReactionRemoveEventArgs, ValueTask>? _messageReactionRemove;
    public event Func<GatewayClient, MessageReactionRemoveEventArgs, ValueTask>? MessageReactionRemove
    {
        add
        {
            HookEvent(_messageReactionRemoveLock, value, ref _messageReactionRemove, client => a => _messageReactionRemove!(client, a), (c, e) => c.MessageReactionRemove += e);
        }
        remove
        {
            UnhookEvent(_messageReactionRemoveLock, value, ref _messageReactionRemove, (c, e) => c.MessageReactionRemove -= e);
        }
    }

    private readonly object _messageReactionRemoveAllLock = new();
    private Func<GatewayClient, MessageReactionRemoveAllEventArgs, ValueTask>? _messageReactionRemoveAll;
    public event Func<GatewayClient, MessageReactionRemoveAllEventArgs, ValueTask>? MessageReactionRemoveAll
    {
        add
        {
            HookEvent(_messageReactionRemoveAllLock, value, ref _messageReactionRemoveAll, client => a => _messageReactionRemoveAll!(client, a), (c, e) => c.MessageReactionRemoveAll += e);
        }
        remove
        {
            UnhookEvent(_messageReactionRemoveAllLock, value, ref _messageReactionRemoveAll, (c, e) => c.MessageReactionRemoveAll -= e);
        }
    }

    private readonly object _messageReactionRemoveEmojiLock = new();
    private Func<GatewayClient, MessageReactionRemoveEmojiEventArgs, ValueTask>? _messageReactionRemoveEmoji;
    public event Func<GatewayClient, MessageReactionRemoveEmojiEventArgs, ValueTask>? MessageReactionRemoveEmoji
    {
        add
        {
            HookEvent(_messageReactionRemoveEmojiLock, value, ref _messageReactionRemoveEmoji, client => a => _messageReactionRemoveEmoji!(client, a), (c, e) => c.MessageReactionRemoveEmoji += e);
        }
        remove
        {
            UnhookEvent(_messageReactionRemoveEmojiLock, value, ref _messageReactionRemoveEmoji, (c, e) => c.MessageReactionRemoveEmoji -= e);
        }
    }

    private readonly object _presenceUpdateLock = new();
    private Func<GatewayClient, Presence, ValueTask>? _presenceUpdate;
    public event Func<GatewayClient, Presence, ValueTask>? PresenceUpdate
    {
        add
        {
            HookEvent(_presenceUpdateLock, value, ref _presenceUpdate, client => a => _presenceUpdate!(client, a), (c, e) => c.PresenceUpdate += e);
        }
        remove
        {
            UnhookEvent(_presenceUpdateLock, value, ref _presenceUpdate, (c, e) => c.PresenceUpdate -= e);
        }
    }

    private readonly object _typingStartLock = new();
    private Func<GatewayClient, TypingStartEventArgs, ValueTask>? _typingStart;
    public event Func<GatewayClient, TypingStartEventArgs, ValueTask>? TypingStart
    {
        add
        {
            HookEvent(_typingStartLock, value, ref _typingStart, client => a => _typingStart!(client, a), (c, e) => c.TypingStart += e);
        }
        remove
        {
            UnhookEvent(_typingStartLock, value, ref _typingStart, (c, e) => c.TypingStart -= e);
        }
    }

    private readonly object _currentUserUpdateLock = new();
    private Func<GatewayClient, CurrentUser, ValueTask>? _currentUserUpdate;
    public event Func<GatewayClient, CurrentUser, ValueTask>? CurrentUserUpdate
    {
        add
        {
            HookEvent(_currentUserUpdateLock, value, ref _currentUserUpdate, client => a => _currentUserUpdate!(client, a), (c, e) => c.CurrentUserUpdate += e);
        }
        remove
        {
            UnhookEvent(_currentUserUpdateLock, value, ref _currentUserUpdate, (c, e) => c.CurrentUserUpdate -= e);
        }
    }

    private readonly object _voiceStateUpdateLock = new();
    private Func<GatewayClient, VoiceState, ValueTask>? _voiceStateUpdate;
    public event Func<GatewayClient, VoiceState, ValueTask>? VoiceStateUpdate
    {
        add
        {
            HookEvent(_voiceStateUpdateLock, value, ref _voiceStateUpdate, client => a => _voiceStateUpdate!(client, a), (c, e) => c.VoiceStateUpdate += e);
        }
        remove
        {
            UnhookEvent(_voiceStateUpdateLock, value, ref _voiceStateUpdate, (c, e) => c.VoiceStateUpdate -= e);
        }
    }

    private readonly object _voiceServerUpdateLock = new();
    private Func<GatewayClient, VoiceServerUpdateEventArgs, ValueTask>? _voiceServerUpdate;
    public event Func<GatewayClient, VoiceServerUpdateEventArgs, ValueTask>? VoiceServerUpdate
    {
        add
        {
            HookEvent(_voiceServerUpdateLock, value, ref _voiceServerUpdate, client => a => _voiceServerUpdate!(client, a), (c, e) => c.VoiceServerUpdate += e);
        }
        remove
        {
            UnhookEvent(_voiceServerUpdateLock, value, ref _voiceServerUpdate, (c, e) => c.VoiceServerUpdate -= e);
        }
    }

    private readonly object _webhooksUpdateLock = new();
    private Func<GatewayClient, WebhooksUpdateEventArgs, ValueTask>? _webhooksUpdate;
    public event Func<GatewayClient, WebhooksUpdateEventArgs, ValueTask>? WebhooksUpdate
    {
        add
        {
            HookEvent(_webhooksUpdateLock, value, ref _webhooksUpdate, client => a => _webhooksUpdate!(client, a), (c, e) => c.WebhooksUpdate += e);
        }
        remove
        {
            UnhookEvent(_webhooksUpdateLock, value, ref _webhooksUpdate, (c, e) => c.WebhooksUpdate -= e);
        }
    }

    private readonly object _interactionCreateLock = new();
    private Func<GatewayClient, Interaction, ValueTask>? _interactionCreate;
    public event Func<GatewayClient, Interaction, ValueTask>? InteractionCreate
    {
        add
        {
            HookEvent(_interactionCreateLock, value, ref _interactionCreate, client => a => _interactionCreate!(client, a), (c, e) => c.InteractionCreate += e);
        }
        remove
        {
            UnhookEvent(_interactionCreateLock, value, ref _interactionCreate, (c, e) => c.InteractionCreate -= e);
        }
    }

    private readonly object _stageInstanceCreateLock = new();
    private Func<GatewayClient, StageInstance, ValueTask>? _stageInstanceCreate;
    public event Func<GatewayClient, StageInstance, ValueTask>? StageInstanceCreate
    {
        add
        {
            HookEvent(_stageInstanceCreateLock, value, ref _stageInstanceCreate, client => a => _stageInstanceCreate!(client, a), (c, e) => c.StageInstanceCreate += e);
        }
        remove
        {
            UnhookEvent(_stageInstanceCreateLock, value, ref _stageInstanceCreate, (c, e) => c.StageInstanceCreate -= e);
        }
    }

    private readonly object _stageInstanceUpdateLock = new();
    private Func<GatewayClient, StageInstance, ValueTask>? _stageInstanceUpdate;
    public event Func<GatewayClient, StageInstance, ValueTask>? StageInstanceUpdate
    {
        add
        {
            HookEvent(_stageInstanceUpdateLock, value, ref _stageInstanceUpdate, client => a => _stageInstanceUpdate!(client, a), (c, e) => c.StageInstanceUpdate += e);
        }
        remove
        {
            UnhookEvent(_stageInstanceUpdateLock, value, ref _stageInstanceUpdate, (c, e) => c.StageInstanceUpdate -= e);
        }
    }

    private readonly object _stageInstanceDeleteLock = new();
    private Func<GatewayClient, StageInstance, ValueTask>? _stageInstanceDelete;
    public event Func<GatewayClient, StageInstance, ValueTask>? StageInstanceDelete
    {
        add
        {
            HookEvent(_stageInstanceDeleteLock, value, ref _stageInstanceDelete, client => a => _stageInstanceDelete!(client, a), (c, e) => c.StageInstanceDelete += e);
        }
        remove
        {
            UnhookEvent(_stageInstanceDeleteLock, value, ref _stageInstanceDelete, (c, e) => c.StageInstanceDelete -= e);
        }
    }

    private readonly object _entitlementCreateLock = new();
    private Func<GatewayClient, Entitlement, ValueTask>? _entitlementCreate;
    public event Func<GatewayClient, Entitlement, ValueTask>? EntitlementCreate
    {
        add
        {
            HookEvent(_entitlementCreateLock, value, ref _entitlementCreate, client => a => _entitlementCreate!(client, a), (c, e) => c.EntitlementCreate += e);
        }
        remove
        {
            UnhookEvent(_entitlementCreateLock, value, ref _entitlementCreate, (c, e) => c.EntitlementCreate -= e);
        }
    }

    private readonly object _entitlementUpdateLock = new();
    private Func<GatewayClient, Entitlement, ValueTask>? _entitlementUpdate;
    public event Func<GatewayClient, Entitlement, ValueTask>? EntitlementUpdate
    {
        add
        {
            HookEvent(_entitlementUpdateLock, value, ref _entitlementUpdate, client => a => _entitlementUpdate!(client, a), (c, e) => c.EntitlementUpdate += e);
        }
        remove
        {
            UnhookEvent(_entitlementUpdateLock, value, ref _entitlementUpdate, (c, e) => c.EntitlementUpdate -= e);
        }
    }

    private readonly object _entitlementDeleteLock = new();
    private Func<GatewayClient, Entitlement, ValueTask>? _entitlementDelete;
    public event Func<GatewayClient, Entitlement, ValueTask>? EntitlementDelete
    {
        add
        {
            HookEvent(_entitlementDeleteLock, value, ref _entitlementDelete, client => a => _entitlementDelete!(client, a), (c, e) => c.EntitlementDelete += e);
        }
        remove
        {
            UnhookEvent(_entitlementDeleteLock, value, ref _entitlementDelete, (c, e) => c.EntitlementDelete -= e);
        }
    }

    private readonly object _unknownEventLock = new();
    private Func<GatewayClient, UnknownEventEventArgs, ValueTask>? _unknownEvent;
    public event Func<GatewayClient, UnknownEventEventArgs, ValueTask>? UnknownEvent
    {
        add
        {
            HookEvent(_unknownEventLock, value, ref _unknownEvent, client => a => _unknownEvent!(client, a), (c, e) => c.UnknownEvent += e);
        }
        remove
        {
            UnhookEvent(_unknownEventLock, value, ref _unknownEvent, (c, e) => c.UnknownEvent -= e);
        }
    }

    private void HookEvent(GatewayClient client, object @lock, ref Func<GatewayClient, ValueTask>? eventRef, Func<ValueTask> @delegate, Action<GatewayClient, Func<ValueTask>> addGatewayClientEvent)
    {
        lock (@lock)
        {
            if (eventRef is not null)
            {
                if (_eventManager.AddEvent(client, @lock, @delegate))
                    addGatewayClientEvent(client, @delegate);
            }
        }
    }

    private void HookEvent<T>(GatewayClient client, object @lock, ref Func<GatewayClient, T, ValueTask>? eventRef, Func<T, ValueTask> @delegate, Action<GatewayClient, Func<T, ValueTask>> addGatewayClientEvent)
    {
        lock (@lock)
        {
            if (eventRef is not null)
            {
                if (_eventManager.AddEvent(client, @lock, @delegate))
                    addGatewayClientEvent(client, @delegate);
            }
        }
    }

    private void HookEvent(object @lock, Func<GatewayClient, ValueTask>? value, ref Func<GatewayClient, ValueTask>? eventRef, Func<GatewayClient, Func<ValueTask>> getDelegate, Action<GatewayClient, Func<ValueTask>> addGatewayClientEvent)
    {
        lock (@lock)
        {
            var oldEvent = eventRef;
            eventRef += value;
            if (oldEvent is null && eventRef is not null)
            {
                var clients = _clients;
                if (clients is not null)
                {
                    var eventManager = _eventManager;
                    int count = clients.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var client = clients[i];
                        if (client is null)
                            break;

                        var @delegate = getDelegate(client);
                        eventManager.AddEvent(client, @lock, @delegate);
                        addGatewayClientEvent(client, @delegate);
                    }
                }
            }
        }
    }

    private void HookEvent<T>(object @lock, Func<GatewayClient, T, ValueTask>? value, ref Func<GatewayClient, T, ValueTask>? eventRef, Func<GatewayClient, Func<T, ValueTask>> getDelegate, Action<GatewayClient, Func<T, ValueTask>> addGatewayClientEvent)
    {
        lock (@lock)
        {
            var oldEvent = eventRef;
            eventRef += value;
            if (oldEvent is null && eventRef is not null)
            {
                var clients = _clients;
                if (clients is not null)
                {
                    var eventManager = _eventManager;
                    int count = clients.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var client = clients[i];
                        if (client is null)
                            break;

                        var @delegate = getDelegate(client);
                        eventManager.AddEvent(client, @lock, @delegate);
                        addGatewayClientEvent(client, @delegate);
                    }
                }
            }
        }
    }

    private void UnhookEvent(object @lock, Func<GatewayClient, ValueTask>? value, ref Func<GatewayClient, ValueTask>? eventRef, Action<GatewayClient, Func<ValueTask>> removeGatewayClientEvent)
    {
        lock (@lock)
        {
            var newEvent = eventRef - value;
            if (eventRef is not null && newEvent is null)
            {
                var clients = _clients;
                if (clients is not null)
                {
                    var eventManager = _eventManager;
                    int count = clients.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var client = clients[i];
                        if (client is null)
                            break;

                        if (eventManager.RemoveEvent(client, @lock, out var @delegate))
                            removeGatewayClientEvent(client, @delegate);
                    }
                }
            }
            eventRef = newEvent;
        }
    }

    private void UnhookEvent<T>(object @lock, Func<GatewayClient, T, ValueTask>? value, ref Func<GatewayClient, T, ValueTask>? eventRef, Action<GatewayClient, Func<T, ValueTask>> removeGatewayClientEvent)
    {
        lock (@lock)
        {
            var newEvent = (Func<GatewayClient, T, ValueTask>?)Delegate.Remove(eventRef, value);
            if (eventRef is not null && newEvent is null)
            {
                var clients = _clients;
                if (clients is not null)
                {
                    var eventManager = _eventManager;
                    int count = clients.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var client = clients[i];
                        if (client is null)
                            break;

                        if (eventManager.RemoveEvent<T>(client, @lock, out var @delegate))
                            removeGatewayClientEvent(client, @delegate);
                    }
                }
            }
            eventRef = newEvent;
        }
    }

    private static void DisposeClients(IReadOnlyList<GatewayClient>? clients, bool initialized)
    {
        if (clients is null)
            return;

        var count = clients.Count;
        if (initialized)
        {
            for (int i = 0; i < count; i++)
                clients[i]!.Dispose();
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var client = clients[i];
                if (client is null)
                    break;

                client.Dispose();
            }
        }
    }

    public void Dispose()
    {
        var startCancellationTokenSource = _startCancellationTokenSource;
        if (startCancellationTokenSource is null)
            return;

        lock (_clientsLock)
            startCancellationTokenSource.Cancel();
        startCancellationTokenSource.Dispose();

        DisposeClients(_clients, _initialized);
    }
}
