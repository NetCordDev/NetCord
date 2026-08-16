using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;

using NetCord.Gateway.WebSockets;
using NetCord.Gateway.Compression;
using NetCord.Gateway;
using NetCord;

namespace HandlersTest;

public sealed class ShardedGatewayHandlerTester : GatewayHandlerTesterBase, ISingleClassMultipleHandlersSupportedHandlerTester
{
    private static HostApplicationBuilder CreateBuilder(IWebSocketConnectionProvider webSocketConnectionProvider)
    {
        var builder = Helper.CreateBuilder();

        builder.Services
            .AddDiscordShardedGateway(o =>
            {
                o.WebSocketConnectionProvider = webSocketConnectionProvider;
                o.Compression = new UncompressedGatewayCompression();
                o.Token = "NO.T.A.REAL.TOKEN";
                o.TotalShardCount = 1;
                o.MaxConcurrency = 1;
            });

        return builder;
    }

    public static IHost CreateClassTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddShardedGatewayHandler<RateLimitedShardedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddScoped(_ => string.Empty)
            .AddShardedGatewayHandler(lifetime is ServiceLifetime.Singleton ? typeof(RejectingStringRateLimitedShardedGatewayHandler)
                                                                            : typeof(RequiringStringRateLimitedShardedGatewayHandler), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<RateLimitedShardedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddScoped(_ => string.Empty)
            .AddShardedGatewayHandler(_ => new RateLimitedShardedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddShardedGatewayHandler<DisposableRateLimitedShardedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddShardedGatewayHandler<AsyncDisposableRateLimitedShardedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddShardedGatewayHandler<DisposableAndAsyncDisposableRateLimitedShardedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<DisposableRateLimitedShardedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<AsyncDisposableRateLimitedShardedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<DisposableAndAsyncDisposableRateLimitedShardedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<IRateLimitedShardedGatewayHandler>(_ => new DisposableRateLimitedShardedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<IRateLimitedShardedGatewayHandler>(_ => new AsyncDisposableRateLimitedShardedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<IRateLimitedShardedGatewayHandler>(_ => new DisposableAndAsyncDisposableRateLimitedShardedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler(GatewayEvent.RateLimited, () =>
            {
                counter.HandlerCount++;
            }, lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateWithParametersTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler(GatewayEvent.RateLimited, (GatewayClient client, RateLimitedEventArgs arg) =>
            {
                counter.HandlerCount++;
            }, lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        Action<IServiceProvider> handler;

        if (lifetime is ServiceLifetime.Singleton)
            handler = services =>
            {
                try
                {
                    _ = services.GetRequiredService<string>();
                }
                catch (InvalidOperationException)
                {
                    counter.HandlerCount++;
                }
            };
        else
            handler = services =>
            {
                _ = services.GetRequiredService<string>();

                counter.HandlerCount++;
            };

        builder.Services
            .AddScoped(_ => string.Empty)
            .AddShardedGatewayHandler(GatewayEvent.RateLimited, handler, lifetime);

        return builder.Build();
    }

    public static IHost CreateClassSingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedAndApplicationCommandPermissionsUpdateWebSocketConnection>());

        builder.Services
            .AddSingleton(counter1)
            .AddSingleton(counter2)
            .AddShardedGatewayHandler<RateLimitedAndApplicationCommandPermissionsUpdateShardedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactorySingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedAndApplicationCommandPermissionsUpdateWebSocketConnection>());

        builder.Services
            .AddShardedGatewayHandler<RateLimitedAndApplicationCommandPermissionsUpdateShardedGatewayHandler>(_ => new([counter1, counter2]), lifetime);

        return builder.Build();
    }

    private class RateLimitedShardedGatewayHandler : IRateLimitedShardedGatewayHandler
    {
        private readonly Counter _counter;

        public RateLimitedShardedGatewayHandler(Counter counter)
        {
            _counter = counter;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(GatewayClient client, RateLimitedEventArgs arg)
        {
            _counter.HandlerCount++;

            return default;
        }
    }

    private class DisposableRateLimitedShardedGatewayHandler(DisposableCounter counter) : RateLimitedShardedGatewayHandler(counter), IDisposable
    {
        public void Dispose()
        {
            counter.DisposeCount++;
        }
    }

    private class AsyncDisposableRateLimitedShardedGatewayHandler(AsyncDisposableCounter counter) : RateLimitedShardedGatewayHandler(counter), IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            counter.DisposeAsyncCount++;

            return default;
        }
    }

    private class DisposableAndAsyncDisposableRateLimitedShardedGatewayHandler(AsyncDisposableCounter counter) : RateLimitedShardedGatewayHandler(counter), IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            counter.DisposeCount++;
        }

        public ValueTask DisposeAsync()
        {
            counter.DisposeAsyncCount++;

            return default;
        }
    }

    private class RateLimitedAndApplicationCommandPermissionsUpdateShardedGatewayHandler : IRateLimitedShardedGatewayHandler, IApplicationCommandPermissionsUpdateShardedGatewayHandler
    {
        private readonly Counter _rateLimitedCounter;
        private readonly Counter _applicationCommandPermissionsUpdateCounter;

        public RateLimitedAndApplicationCommandPermissionsUpdateShardedGatewayHandler(IEnumerable<Counter> counters)
        {
            (_rateLimitedCounter, _applicationCommandPermissionsUpdateCounter) = Helper.ExtractCounters(counters);

            _rateLimitedCounter.ConstructorCount++;
            _applicationCommandPermissionsUpdateCounter.ConstructorCount++;
        }

        public ValueTask HandleAsync(GatewayClient client, RateLimitedEventArgs arg)
        {
            _rateLimitedCounter.HandlerCount++;

            return default;
        }

        public ValueTask HandleAsync(GatewayClient client, ApplicationCommandPermission arg)
        {
            _applicationCommandPermissionsUpdateCounter.HandlerCount++;

            return default;
        }
    }

    private class RequiringStringRateLimitedShardedGatewayHandler : IRateLimitedShardedGatewayHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RequiringStringRateLimitedShardedGatewayHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(GatewayClient client, RateLimitedEventArgs arg)
        {
            _ = _services.GetRequiredService<string>();

            _counter.HandlerCount++;

            return default;
        }
    }

    private class RejectingStringRateLimitedShardedGatewayHandler : IRateLimitedShardedGatewayHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RejectingStringRateLimitedShardedGatewayHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(GatewayClient client, RateLimitedEventArgs arg)
        {
            try
            {
                _ = _services.GetRequiredService<string>();
            }
            catch (InvalidOperationException)
            {
                _counter.HandlerCount++;
            }

            return default;
        }
    }
}

