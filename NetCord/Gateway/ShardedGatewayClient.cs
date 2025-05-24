using System.Collections;

using NetCord.Rest;

namespace NetCord.Gateway;

public sealed partial class ShardedGatewayClient : IReadOnlyList<GatewayClient>, IEntity, IDisposable
{
    private readonly ShardedGatewayClientConfiguration _configuration;
    private readonly ShardedGatewayClientEventManager _eventManager;

    private readonly object _clientsLock = new();
    private GatewayClient[] _clients = [];

    private CancellationTokenSource? _startCancellationTokenSource;
    private readonly object _startLock = new();

    private bool _initialized;

    public ShardedGatewayClient(IEntityToken token, ShardedGatewayClientConfiguration? configuration = null)
    {
        Token = token;
        _configuration = configuration = CreateConfiguration(configuration);
        _eventManager = new();
        Rest = new(token, configuration.RestClientConfiguration);
    }

    private static ShardedGatewayClientConfiguration CreateConfiguration(ShardedGatewayClientConfiguration? configuration)
    {
        if (configuration is null)
        {
            return ShardedGatewayClientConfigurationFactory.Create(_ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   _ => null,
                                                                   null,
                                                                   null);
        }

        return ShardedGatewayClientConfigurationFactory.Create(configuration.WebSocketConnectionProviderFactory ?? (_ => null),
                                                               configuration.RateLimiterProviderFactory ?? (_ => null),
                                                               configuration.DefaultPayloadPropertiesFactory ?? (_ => null),
                                                               configuration.ReconnectStrategyFactory ?? (_ => null),
                                                               configuration.LatencyTimerFactory ?? (_ => null),
                                                               configuration.VersionFactory ?? (_ => ApiVersion.V10),
                                                               configuration.CacheFactory ?? (_ => null),
                                                               configuration.CompressionFactory ?? (_ => null),
                                                               configuration.IntentsFactory ?? (_ => GatewayIntents.AllNonPrivileged),
                                                               configuration.Hostname,
                                                               configuration.ConnectionPropertiesFactory ?? (_ => null),
                                                               configuration.LargeThresholdFactory ?? (_ => null),
                                                               configuration.PresenceFactory ?? (_ => null),
                                                               configuration.ShardCount,
                                                               configuration.RestClientConfiguration);
    }

    /// <summary>
    /// The <see cref="Token"/> of the <see cref="ShardedGatewayClient"/>.
    /// </summary>
    public IEntityToken Token { get; }

    /// <summary>
    /// The <see cref="RestClient"/> of the <see cref="ShardedGatewayClient"/>.
    /// </summary>
    public RestClient Rest { get; }

    public ulong Id => Token.Id;

    public DateTimeOffset CreatedAt => Token.CreatedAt;

    public GatewayClient this[int shardId] => _clients[shardId];

    public GatewayClient this[ulong guildId]
    {
        get
        {
            var clients = _clients;
            return clients[Snowflake.ShardId(guildId, clients.Length)];
        }
    }

    /// <summary>
    /// The count of shards of the <see cref="ShardedGatewayClient"/>.
    /// </summary>
    public int Count => _clients.Length;

    public async Task StartAsync(Func<Shard, PresenceProperties?>? presenceFactory = null)
    {
        CancellationTokenSource startCancellationTokenSource;
        lock (_startLock)
        {
            if (_startCancellationTokenSource is not null)
                throw new InvalidOperationException("The client is already connected.");

            startCancellationTokenSource = _startCancellationTokenSource = new();
        }
        var cancellationToken = startCancellationTokenSource.Token;

        var oldInitialized = _initialized;
        _initialized = false;

        presenceFactory ??= _ => null;

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

            var token = Token;

            await ConnectBucketAsync(0).ConfigureAwait(false);

            for (int bucket = maxConcurrency; bucket < shardCount; bucket += maxConcurrency)
            {
                await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
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
                            await Task.Delay(resetAfter - elapsed, cancellationToken).ConfigureAwait(false);

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
                        cancellationToken.ThrowIfCancellationRequested();
                        clients[shardId] = client = new(token, rest, gatewayClientConfiguration);
                    }
                    HookEvents(client);
                    await client.StartAsync(presenceFactory(shard)).ConfigureAwait(false);
                }
            }
        }
        catch
        {
            var clients = _clients;
            _clients = [];
            if (clients is not null)
            {
                int count = clients.Length;
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
        return GatewayClientConfigurationFactory.Create(configuration.WebSocketConnectionProviderFactory!(shard),
                                                        configuration.RateLimiterProviderFactory!(shard),
                                                        configuration.DefaultPayloadPropertiesFactory!(shard),
                                                        configuration.ReconnectStrategyFactory!(shard),
                                                        configuration.LatencyTimerFactory!(shard),
                                                        configuration.VersionFactory!(shard),
                                                        configuration.CacheFactory!(shard),
                                                        configuration.CompressionFactory!(shard),
                                                        configuration.IntentsFactory!(shard),
                                                        configuration.Hostname,
                                                        configuration.ConnectionPropertiesFactory!(shard),
                                                        configuration.LargeThresholdFactory!(shard),
                                                        configuration.PresenceFactory!(shard),
                                                        shard,
                                                        null);
    }

    public async Task CloseAsync(System.Net.WebSockets.WebSocketCloseStatus status = System.Net.WebSockets.WebSocketCloseStatus.NormalClosure)
    {
        var startCancellationTokenSource = _startCancellationTokenSource;
        var clients = _clients;
        if (startCancellationTokenSource is null)
            throw new InvalidOperationException("The client is not connected.");

        lock (_clientsLock)
            startCancellationTokenSource.Cancel();
        startCancellationTokenSource.Dispose();

        await Task.WhenAll((_initialized ? clients : clients.TakeWhile(clients => clients is not null)).Select(client => client.CloseAsync(status))).ConfigureAwait(false);

        _startCancellationTokenSource = null;
    }

    public IEnumerator<GatewayClient> GetEnumerator() => ((IEnumerable<GatewayClient>)_clients).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _clients.GetEnumerator();

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
                var eventManager = _eventManager;
                int count = clients.Length;
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

    private void HookEvent<T>(object @lock, Func<GatewayClient, T, ValueTask>? value, ref Func<GatewayClient, T, ValueTask>? eventRef, Func<GatewayClient, Func<T, ValueTask>> getDelegate, Action<GatewayClient, Func<T, ValueTask>> addGatewayClientEvent)
    {
        lock (@lock)
        {
            var oldEvent = eventRef;
            eventRef += value;
            if (oldEvent is null && eventRef is not null)
            {
                var clients = _clients;
                    var eventManager = _eventManager;
                    int count = clients.Length;
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

    private void UnhookEvent(object @lock, Func<GatewayClient, ValueTask>? value, ref Func<GatewayClient, ValueTask>? eventRef, Action<GatewayClient, Func<ValueTask>> removeGatewayClientEvent)
    {
        lock (@lock)
        {
            var newEvent = eventRef - value;
            if (eventRef is not null && newEvent is null)
            {
                var clients = _clients;
                    var eventManager = _eventManager;
                    int count = clients.Length;
                    for (int i = 0; i < count; i++)
                    {
                        var client = clients[i];
                        if (client is null)
                            break;

                        if (eventManager.RemoveEvent(client, @lock, out var @delegate))
                            removeGatewayClientEvent(client, @delegate);
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
                    var eventManager = _eventManager;
                    int count = clients.Length;
                    for (int i = 0; i < count; i++)
                    {
                        var client = clients[i];
                        if (client is null)
                            break;

                        if (eventManager.RemoveEvent<T>(client, @lock, out var @delegate))
                            removeGatewayClientEvent(client, @delegate);
                    }
            }
            eventRef = newEvent;
        }
    }

    private static void DisposeClients(GatewayClient[] clients, bool initialized)
    {
        var length = clients.Length;
        if (initialized)
        {
            for (int i = 0; i < length; i++)
                clients[i].Dispose();
        }
        else
        {
            for (int i = 0; i < length; i++)
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
