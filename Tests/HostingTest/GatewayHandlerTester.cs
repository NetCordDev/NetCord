using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;

using NetCord.Gateway.WebSockets;
using NetCord.Gateway.Compression;
using NetCord.Gateway;
using NetCord;

namespace HostingTest;

public sealed class GatewayHandlerTester : GatewayHandlerTesterBase, ISingleClassMultipleHandlersSupportedHandlerTester
{
    private static HostApplicationBuilder CreateBuilder(IWebSocketConnectionProvider webSocketConnectionProvider)
    {
        var builder = Helper.CreateBuilder();

        builder.Services
            .AddDiscordGateway(o =>
            {
                o.WebSocketConnectionProvider = webSocketConnectionProvider;
                o.Compression = new UncompressedGatewayCompression();
                o.Token = "NO.T.A.REAL.TOKEN";
            });

        return builder;
    }

    public static IHost CreateClassTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddGatewayHandler<RateLimitedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddScoped(_ => string.Empty)
            .AddGatewayHandler(lifetime is ServiceLifetime.Singleton ? typeof(RejectingStringRateLimitedGatewayHandler)
                                                                     : typeof(RequiringStringRateLimitedGatewayHandler), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler<RateLimitedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddScoped(_ => string.Empty)
            .AddGatewayHandler(_ => new RateLimitedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddGatewayHandler<DisposableRateLimitedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddGatewayHandler<AsyncDisposableRateLimitedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddSingleton(counter)
            .AddGatewayHandler<DisposableAndAsyncDisposableRateLimitedGatewayHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler<DisposableRateLimitedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler<AsyncDisposableRateLimitedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler<DisposableAndAsyncDisposableRateLimitedGatewayHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler<IRateLimitedGatewayHandler>(_ => new DisposableRateLimitedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler<IRateLimitedGatewayHandler>(_ => new AsyncDisposableRateLimitedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler<IRateLimitedGatewayHandler>(_ => new DisposableAndAsyncDisposableRateLimitedGatewayHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler(GatewayEvent.RateLimited, () =>
            {
                counter.HandlerCount++;
            }, lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateWithParametersTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        builder.Services
            .AddGatewayHandler(GatewayEvent.RateLimited, (RateLimitedEventArgs arg) =>
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
            .AddGatewayHandler(GatewayEvent.RateLimited, handler, lifetime);

        return builder.Build();
    }

    public static IHost CreateClassSingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedAndApplicationCommandPermissionsUpdateWebSocketConnection>());

        builder.Services
            .AddSingleton(counter1)
            .AddSingleton(counter2)
            .AddGatewayHandler<RateLimitedAndApplicationCommandPermissionsUpdateGatewayHandler>(lifetime);

        return builder.Build();
    }

    private class RateLimitedGatewayHandler : IRateLimitedGatewayHandler
    {
        private readonly Counter _counter;

        public RateLimitedGatewayHandler(Counter counter)
        {
            _counter = counter;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(RateLimitedEventArgs arg)
        {
            _counter.HandlerCount++;

            return default;
        }
    }

    private class DisposableRateLimitedGatewayHandler(DisposableCounter counter) : RateLimitedGatewayHandler(counter), IDisposable
    {
        public void Dispose()
        {
            counter.DisposeCount++;
        }
    }

    private class AsyncDisposableRateLimitedGatewayHandler(AsyncDisposableCounter counter) : RateLimitedGatewayHandler(counter), IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            counter.DisposeAsyncCount++;

            return default;
        }
    }

    private class DisposableAndAsyncDisposableRateLimitedGatewayHandler(AsyncDisposableCounter counter) : RateLimitedGatewayHandler(counter), IDisposable, IAsyncDisposable
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

    private class RateLimitedAndApplicationCommandPermissionsUpdateGatewayHandler : IRateLimitedGatewayHandler, IApplicationCommandPermissionsUpdateGatewayHandler
    {
        private readonly Counter _rateLimitedCounter;
        private readonly Counter _applicationCommandPermissionsUpdateCounter;

        public RateLimitedAndApplicationCommandPermissionsUpdateGatewayHandler(Counter rateLimitedCounter, Counter applicationCommandPermissionsUpdateCounter)
        {
            _rateLimitedCounter = rateLimitedCounter;
            _applicationCommandPermissionsUpdateCounter = applicationCommandPermissionsUpdateCounter;

            rateLimitedCounter.ConstructorCount++;
            applicationCommandPermissionsUpdateCounter.ConstructorCount++;
        }

        public ValueTask HandleAsync(RateLimitedEventArgs arg)
        {
            _rateLimitedCounter.HandlerCount++;

            return default;
        }

        public ValueTask HandleAsync(ApplicationCommandPermission arg)
        {
            _applicationCommandPermissionsUpdateCounter.HandlerCount++;

            return default;
        }
    }

    private class RequiringStringRateLimitedGatewayHandler : IRateLimitedGatewayHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RequiringStringRateLimitedGatewayHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(RateLimitedEventArgs arg)
        {
            _ = _services.GetRequiredService<string>();

            _counter.HandlerCount++;

            return default;
        }
    }

    private class RejectingStringRateLimitedGatewayHandler : IRateLimitedGatewayHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RejectingStringRateLimitedGatewayHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(RateLimitedEventArgs arg)
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

