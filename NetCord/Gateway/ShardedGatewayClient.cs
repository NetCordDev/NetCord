using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using NetCord.Rest;

using static NetCord.Gateway.GatewayClientThrowHelper;

namespace NetCord.Gateway;

public sealed partial class ShardedGatewayClient : IReadOnlyList<GatewayClient>, IEntity, IDisposable
{
    private class State : IDisposable
    {
        public GatewayClient[]? Clients { get; set; }

        public Task ConnectingTask => _connectingCompletionSource.Task;

        private readonly TaskCompletionSource _connectingCompletionSource = new();

        public CancellationTokenProvider DisconnectedTokenProvider { get; } = new();

        public void IndicateStoppedConnecting()
        {
            _connectingCompletionSource.TrySetResult();
        }

        public void Dispose()
        {
            if (Clients is { } clients)
            {
                int length = clients.Length;
                for (int i = 0; i < length; i++)
                    clients[i].Dispose();
            }

            DisconnectedTokenProvider.Dispose();
        }
    }

    private readonly ShardedGatewayClientConfiguration _configuration;

    private readonly ReaderWriterLockSlim _eventsLock = new();

    private State? _state;

    public ShardedGatewayClient(IEntityToken token, ShardedGatewayClientConfiguration? configuration = null)
    {
        Token = token;
        _configuration = configuration = CreateConfiguration(configuration);
        Rest = new(token, configuration);
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
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   _ => null);
        }

        var (shardRange, totalShardCount) = (configuration.ShardRange, configuration.TotalShardCount);
        if (shardRange.HasValue)
        {
            if (!totalShardCount.HasValue)
                throw new InvalidOperationException($"When '{nameof(ShardedGatewayClientConfiguration.ShardRange)}' is specified in the configuration, '{nameof(ShardedGatewayClientConfiguration.TotalShardCount)}' must also be specified.");
            else if (totalShardCount.GetValueOrDefault() <= 0)
                ThrowInvalidTotalShardCount();

            int length = totalShardCount.GetValueOrDefault();

            if (!TryGetOffsetAndLength(shardRange.GetValueOrDefault(), length, out _, out _))
                ThrowInvalidShardRange();
        }
        else if (totalShardCount.HasValue && totalShardCount.GetValueOrDefault() <= 0)
            ThrowInvalidTotalShardCount();

        return ShardedGatewayClientConfigurationFactory.Create(configuration.WebSocketConnectionProviderFactory ?? (_ => null),
                                                                   configuration.RateLimiterProviderFactory ?? (_ => null),
                                                                   configuration.DefaultPayloadPropertiesFactory ?? (_ => null),
                                                                   configuration.ReconnectStrategyFactory ?? (_ => null),
                                                                   configuration.LatencyTimerFactory ?? (_ => null),
                                                                   configuration.VersionFactory ?? (_ => ApiVersion.V10),
                                                                   configuration.CacheProviderFactory ?? (_ => null),
                                                                   configuration.CompressionFactory ?? (_ => null),
                                                                   configuration.IntentsFactory ?? (_ => GatewayIntents.AllNonPrivileged),
                                                                   configuration.Hostname,
                                                                   configuration.ConnectionPropertiesFactory ?? (_ => null),
                                                                   configuration.LargeThresholdFactory ?? (_ => null),
                                                                   configuration.PresenceFactory ?? (_ => null),
                                                                   configuration.MaxConcurrency,
                                                                   shardRange,
                                                                   totalShardCount,
                                                                   configuration.RestClientConfiguration,
                                                                   configuration.LoggerFactory ?? (_ => null));
    }

    [DoesNotReturn]
    private static void ThrowInvalidShardRange()
    {
        throw new InvalidOperationException($"'{nameof(ShardedGatewayClientConfiguration.ShardRange)}' specified in the configuration is invalid for '{nameof(ShardedGatewayClientConfiguration.TotalShardCount)}'.");
    }

    [DoesNotReturn]
    private static void ThrowInvalidTotalShardCount()
    {
        throw new InvalidOperationException($"'{nameof(ShardedGatewayClientConfiguration.TotalShardCount)}' specified in the configuration cannot be lower than or equal to 0.");
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

    public GatewayClient this[int shardId]
    {
        get
        {
            if (_state is not { Clients: { } clients })
            {
                ThrowConnectionNotStarted();
                return null!;
            }

            return clients[shardId];
        }
    }

    public GatewayClient this[ulong guildId]
    {
        get
        {
            if (_state is not { Clients: { } clients })
            {
                ThrowConnectionNotStarted();
                return null!;
            }

            return clients[Snowflake.ShardId(guildId, clients.Length)];
        }
    }

    /// <summary>
    /// The count of shards of the <see cref="ShardedGatewayClient"/>.
    /// </summary>
    public int Count => _state is { Clients: { } clients } ? clients.Length : 0;

    public async ValueTask StartAsync(Func<Shard, PresenceProperties?>? presenceFactory = null, CancellationToken cancellationToken = default)
    {
        State newState = new();
        if (Interlocked.CompareExchange(ref _state, newState, null) is not null)
        {
            newState.Dispose();
            ThrowConnectionAlreadyStarted();
        }

        var disconnectedToken = newState.DisconnectedTokenProvider.Token;

        using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(disconnectedToken, cancellationToken);

        try
        {
            await StartAsyncCore(newState, presenceFactory, linkedTokenSource.Token).ConfigureAwait(false);
        }
        finally
        {
            newState.IndicateStoppedConnecting();
        }
    }

    private static bool TryGetOffsetAndLength(Range range, int length, out int offset, out int outLength)
    {
        var start = range.Start.GetOffset(length);
        var end = range.End.GetOffset(length);

        if ((uint)end > (uint)length || (uint)start >= (uint)end)
        {
            offset = default;
            outLength = default;
            return false;
        }

        offset = start;
        outLength = end - start;
        return true;
    }

    private async ValueTask StartAsyncCore(State newState, Func<Shard, PresenceProperties?>? presenceFactory, CancellationToken cancellationToken)
    {
        var configuration = _configuration;

        var (configMaxConcurrency, configTotalShardCount, configShardRange) = (configuration.MaxConcurrency, configuration.TotalShardCount, configuration.ShardRange);

        int maxConcurrency, totalShardCount;

        if (!configMaxConcurrency.HasValue || !configTotalShardCount.HasValue)
        {
            var info = await Rest.GetGatewayBotAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            maxConcurrency = configMaxConcurrency ?? info.SessionStartLimit.MaxConcurrency;
            totalShardCount = configTotalShardCount ?? info.ShardCount;
        }
        else
            (maxConcurrency, totalShardCount) = (configMaxConcurrency.GetValueOrDefault(), configTotalShardCount.GetValueOrDefault());

        int startShardId, shardCount;

        if (configShardRange.HasValue)
        {
            if (!TryGetOffsetAndLength(configShardRange.GetValueOrDefault(), totalShardCount, out startShardId, out shardCount))
                ThrowInvalidShardRange();
        }
        else
            (startShardId, shardCount) = (0, totalShardCount);

        var token = Token;
        var rest = Rest;

        var clients = new GatewayClient[shardCount];

        for (int i = 0; i < shardCount; i++)
        {
            Shard shard = new(startShardId + i, totalShardCount);
            clients[i] = new(token, rest, GetGatewayClientConfiguration(configuration, shard));
        }

        var eventsLock = _eventsLock;

        eventsLock.EnterWriteLock();
        try
        {
            HookHandlers(clients);

            newState.Clients = clients;
        }
        finally
        {
            eventsLock.ExitWriteLock();
        }

        var tasks = ArrayPool<Task>.Shared.Rent(maxConcurrency);

        int exceedingShardId = startShardId + shardCount;

        int remainder = startShardId % maxConcurrency;

        int bucketShardOffset = startShardId - remainder;
        int bucketStartShardId = startShardId;

        // 'shardCount = exceedingShardId - startShardId' in this case
        int bucketShardCount = Math.Min(maxConcurrency - remainder, shardCount);

        while (true)
        {
            for (int i = 0; i < bucketShardCount; i++)
            {
                int shardId = bucketStartShardId + i;
                tasks[i] = clients[shardId - startShardId].StartAsync(presenceFactory?.Invoke(new(shardId, totalShardCount)), cancellationToken).AsTask();
            }

            await Task.WhenAll(tasks.AsSpan(0, bucketShardCount)).ConfigureAwait(false);

            if ((bucketShardOffset += maxConcurrency) >= exceedingShardId)
                break;

            await Task.Delay(5000, cancellationToken).ConfigureAwait(false);

            // 'maxConcurrency = maxConcurrency - remainder' in this case
            bucketShardCount = Math.Min(maxConcurrency, exceedingShardId - (bucketStartShardId = bucketShardOffset));
        }

        ArrayPool<Task>.Shared.Return(tasks);
    }

    private static GatewayClientConfiguration GetGatewayClientConfiguration(ShardedGatewayClientConfiguration configuration, Shard shard)
    {
        return GatewayClientConfigurationFactory.Create(configuration.WebSocketConnectionProviderFactory!(shard),
                                                        configuration.RateLimiterProviderFactory!(shard),
                                                        configuration.DefaultPayloadPropertiesFactory!(shard),
                                                        configuration.ReconnectStrategyFactory!(shard),
                                                        configuration.LatencyTimerFactory!(shard),
                                                        configuration.VersionFactory!(shard),
                                                        configuration.CacheProviderFactory!(shard),
                                                        configuration.CompressionFactory!(shard),
                                                        configuration.IntentsFactory!(shard),
                                                        configuration.Hostname,
                                                        configuration.ConnectionPropertiesFactory!(shard),
                                                        configuration.LargeThresholdFactory!(shard),
                                                        configuration.PresenceFactory!(shard),
                                                        shard,
                                                        null,
                                                        configuration.LoggerFactory!(shard));
    }

    public async ValueTask CloseAsync(System.Net.WebSockets.WebSocketCloseStatus status = System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, string? statusDescription = null, CancellationToken cancellationToken = default)
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is null)
            ThrowConnectionNotStarted();

        try
        {
            state.DisconnectedTokenProvider.Cancel();

            await state.ConnectingTask.ConfigureAwait(false);

            if (state.Clients is { } clients)
                await Task.WhenAll(clients.Select(c => (Task)c.CloseIfStartedAsync(status, statusDescription, cancellationToken).AsTask())).ConfigureAwait(false);
        }
        finally
        {
            state.Dispose();
        }
    }

    public void Abort()
    {
        var state = Interlocked.Exchange(ref _state, null);

        if (state is not { Clients: { } clients })
            return;

        try
        {
            int length = clients.Length;
            for (int i = 0; i < length; i++)
                clients[i].Abort();
        }
        finally
        {
            state.Dispose();
        }
    }

    public IEnumerator<GatewayClient> GetEnumerator()
    {
        return ((IEnumerable<GatewayClient>)(_state?.Clients ?? [])).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (_state?.Clients ?? []).GetEnumerator();
    }

    private static void HookHandlersToClients(GatewayClient[] clients, Dictionary<Func<GatewayClient, ValueTask>, List<Func<ValueTask>[]>> @event, Action<GatewayClient, Func<ValueTask>> addHandler)
    {
        foreach (var pair in @event)
        {
            var handler = pair.Key;
            var list = pair.Value;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                HookHandlerToClients(handler, clients, addHandler, out var handlers);
                list[i] = handlers;
            }
        }
    }

    private static void HookHandlersToClients<T>(GatewayClient[] clients, Dictionary<Func<GatewayClient, T, ValueTask>, List<Func<T, ValueTask>[]>> @event, Action<GatewayClient, Func<T, ValueTask>> addHandler)
    {
        foreach (var pair in @event)
        {
            var handler = pair.Key;
            var list = pair.Value;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                HookHandlerToClients(handler, clients, addHandler, out var handlers);
                list[i] = handlers;
            }
        }
    }

    private void AddHandler(Dictionary<Func<GatewayClient, ValueTask>, List<Func<ValueTask>[]>> @event, Lock eventLock, Func<GatewayClient, ValueTask>? handler, Action<GatewayClient, Func<ValueTask>> addHandler)
    {
        if (handler is null)
            return;

        var eventsLock = _eventsLock;

        eventsLock.EnterReadLock();
        try
        {
            lock (eventLock)
            {
                ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(@event, handler, out var exists);

                if (!exists)
                    list = new(1);

                Func<ValueTask>[] handlers;

                if (_state is { Clients: { } clients })
                    HookHandlerToClients(handler, clients, addHandler, out handlers);
                else
                    handlers = [];

                list!.Add(handlers);
            }
        }
        finally
        {
            eventsLock.ExitReadLock();
        }
    }

    private void AddHandler<T>(Dictionary<Func<GatewayClient, T, ValueTask>, List<Func<T, ValueTask>[]>> @event, Lock eventLock, Func<GatewayClient, T, ValueTask>? handler, Action<GatewayClient, Func<T, ValueTask>> addHandler)
    {
        if (handler is null)
            return;

        var eventsLock = _eventsLock;

        eventsLock.EnterReadLock();
        try
        {
            lock (eventLock)
            {
                ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(@event, handler, out var exists);

                if (!exists)
                    list = new(1);

                Func<T, ValueTask>[] handlers;

                if (_state is { Clients: { } clients })
                    HookHandlerToClients(handler, clients, addHandler, out handlers);
                else
                    handlers = [];

                list!.Add(handlers);
            }
        }
        finally
        {
            eventsLock.ExitReadLock();
        }
    }

    private static void HookHandlerToClients(Func<GatewayClient, ValueTask> handler, GatewayClient[] clients, Action<GatewayClient, Func<ValueTask>> addHandler, out Func<ValueTask>[] handlers)
    {
        int length = clients.Length;

        handlers = new Func<ValueTask>[length];

        for (int i = 0; i < length; i++)
        {
            var client = clients[i];

            addHandler(client, handlers[i] = () => handler(client));
        }
    }

    private static void HookHandlerToClients<T>(Func<GatewayClient, T, ValueTask> handler, GatewayClient[] clients, Action<GatewayClient, Func<T, ValueTask>> addHandler, out Func<T, ValueTask>[] handlers)
    {
        int length = clients.Length;

        handlers = new Func<T, ValueTask>[length];

        for (int i = 0; i < length; i++)
        {
            var client = clients[i];

            addHandler(client, handlers[i] = args => handler(client, args));
        }
    }

    private void RemoveHandler(Dictionary<Func<GatewayClient, ValueTask>, List<Func<ValueTask>[]>> @event, Lock eventLock, Func<GatewayClient, ValueTask>? handler, Action<GatewayClient, Func<ValueTask>> removeHandler)
    {
        if (handler is null)
            return;

        var eventsLock = _eventsLock;

        eventsLock.EnterReadLock();
        try
        {
            lock (eventLock)
            {
                if (!@event.TryGetValue(handler, out var list))
                    return;

                int lastIndex = list.Count - 1;
                var handlers = list[lastIndex];

                if (lastIndex is 0)
                    @event.Remove(handler);
                else
                {
                    list.RemoveAt(lastIndex);
                    list.TrimExcess();
                }

                if (_state is { Clients: { } clients })
                {
                    var length = clients.Length;

                    for (int i = 0; i < length; i++)
                        removeHandler(clients[i], handlers[i]);
                }
            }
        }
        finally
        {
            eventsLock.ExitReadLock();
        }
    }

    private void RemoveHandler<T>(Dictionary<Func<GatewayClient, T, ValueTask>, List<Func<T, ValueTask>[]>> @event, Lock eventLock, Func<GatewayClient, T, ValueTask>? handler, Action<GatewayClient, Func<T, ValueTask>> removeHandler)
    {
        if (handler is null)
            return;

        var eventsLock = _eventsLock;

        eventsLock.EnterReadLock();
        try
        {
            lock (eventLock)
            {
                if (!@event.TryGetValue(handler, out var list))
                    return;

                int lastIndex = list.Count - 1;
                var handlers = list[lastIndex];

                if (lastIndex is 0)
                    @event.Remove(handler);
                else
                {
                    list.RemoveAt(lastIndex);
                    list.TrimExcess();
                }

                if (_state is { Clients: { } clients })
                {
                    var length = clients.Length;

                    for (int i = 0; i < length; i++)
                        removeHandler(clients[i], handlers[i]);
                }
            }
        }
        finally
        {
            eventsLock.ExitReadLock();
        }
    }

    public void Dispose()
    {
        _eventsLock.Dispose();

        _state?.Dispose();
    }
}
