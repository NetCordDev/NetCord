using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Compression;
using NetCord.Gateway.WebSockets;
using NetCord.Hosting.Gateway;

namespace HostingTest;

[TestClass]
public class GatewayHandlersTest(TestContext testContext) : GatewayHandlersTestBase
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

    // Factory
    [TestMethod]
    public async ValueTask ClassFactorySingletonGetsCalledAndIsSingleton()
    {
        var counter = await ClassFactoryGetsCalledAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassFactoryTransientGetsCalledAndIsNotSingleton()
    {
        return ClassFactoryTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassFactoryScopedGetsCalledAndIsNotSingleton()
    {
        return ClassFactoryTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassFactoryTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryGetsCalledAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");
    }

    private ValueTask<Counter> ClassFactoryGetsCalledAsync(ServiceLifetime lifetime)
    {
        return ClassCountFactoryAsync<RateLimitedGatewayHandler, RateLimitedWebSocketConnection>(c => new(c), lifetime);
    }

    private async ValueTask<Counter> ClassCountFactoryAsync<THandler, TConnection>(Func<Counter, THandler> createHandler, ServiceLifetime lifetime)
        where THandler : class, IGatewayHandler
        where TConnection : IWebSocketConnection, new()
    {
        Counter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<TConnection>());

            builder.Services
                .AddGatewayHandler(_ => createHandler(counter), lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // No Factory
    [TestMethod]
    public async ValueTask ClassSingletonGetsCalledAndIsSingleton()
    {
        var counter = await ClassCountAsync<RateLimitedGatewayHandler, RateLimitedWebSocketConnection>(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassTransientGetsCalledAndIsNotSingleton()
    {
        return ClassTransientOrScopedGetsCalledAndIsNotSingletonAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassScopedGetsCalledAndIsNotSingleton()
    {
        return ClassTransientOrScopedGetsCalledAndIsNotSingletonAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassTransientOrScopedGetsCalledAndIsNotSingletonAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassCountAsync<RateLimitedGatewayHandler, RateLimitedWebSocketConnection>(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<Counter> ClassCountAsync<THandler, TConnection>(ServiceLifetime lifetime)
        where THandler : class, IGatewayHandler
        where TConnection : IWebSocketConnection, new()
    {
        Counter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<TConnection>());

            builder.Services
                .AddSingleton(counter)
                .AddGatewayHandler<THandler>(lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Single Class Multiple Handlers
    [TestMethod]
    public async ValueTask ClassSingletonSingleClassMultipleHandlersSupported()
    {
        var (rateLimitedCounter, applicationCommandPermissionsUpdateCounter) = await ClassSingleClassMultipleHandlersSupportedAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, rateLimitedCounter.ConstructorCount, "RateLimited handler constructor was called more than once.");

        Assert.AreEqual(1, applicationCommandPermissionsUpdateCounter.ConstructorCount, "ApplicationCommandPermissionsUpdate handler constructor was called more than once.");
    }

    [TestMethod]
    public ValueTask ClassTransientSingleClassMultipleHandlersSupported()
    {
        return ClassTransientOrScopedSingleClassMultipleHandlersSupportedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassScopedSingleClassMultipleHandlersSupported()
    {
        return ClassTransientOrScopedSingleClassMultipleHandlersSupportedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassTransientOrScopedSingleClassMultipleHandlersSupportedAsync(ServiceLifetime lifetime)
    {
        var (rateLimitedCounter, applicationCommandPermissionsUpdateCounter) = await ClassSingleClassMultipleHandlersSupportedAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(rateLimitedCounter.HandlerCount + applicationCommandPermissionsUpdateCounter.HandlerCount, rateLimitedCounter.ConstructorCount, "RateLimited handler constructor was not called the same amount of times as both handlers were called.");

        Assert.AreEqual(applicationCommandPermissionsUpdateCounter.HandlerCount + rateLimitedCounter.HandlerCount, applicationCommandPermissionsUpdateCounter.ConstructorCount, "ApplicationCommandPermissionsUpdate handler constructor was not called the same amount of times as both handlers were called.");
    }

    private async ValueTask<(Counter RateLimitedCounter, Counter ApplicationCommandPermissionsUpdateCounter)> ClassSingleClassMultipleHandlersSupportedAsync(ServiceLifetime lifetime)
    {
        Counter rateLimitedCounter = new();
        Counter applicationCommandPermissionsUpdateCounter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<ByTurnsWebSocketConnection>());

            builder.Services
                .AddGatewayHandler<RateLimitedAndApplicationCommandPermissionsUpdateGatewayHandler>(_ => new(rateLimitedCounter, applicationCommandPermissionsUpdateCounter), lifetime);

            return builder;
        }, () => rateLimitedCounter.HandlerCount >= 10 && applicationCommandPermissionsUpdateCounter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return (rateLimitedCounter, applicationCommandPermissionsUpdateCounter);
    }

    // Factory Disposable
    [TestMethod]
    public async ValueTask ClassFactoryDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryDisposableCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeCount, "Handler Dispose was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassFactoryDisposableTransientGetsDisposed()
    {
        return ClassFactoryDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassFactoryDisposableScopedGetsDisposed()
    {
        return ClassFactoryDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassFactoryDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryDisposableCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeCount, "Handler Dispose was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<DisposableCounter> ClassFactoryDisposableCountAsync(ServiceLifetime lifetime)
    {
        DisposableCounter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

            builder.Services
                .AddGatewayHandler<DisposableRateLimitedGatewayHandler>(_ => new(counter), lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Hidden Disposable
    [TestMethod]
    public async ValueTask ClassFactoryHiddenDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryHiddenDisposableCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeCount, "Handler Dispose was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassFactoryHiddenDisposableTransientGetsDisposed()
    {
        return ClassFactoryHiddenDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassFactoryHiddenDisposableScopedGetsDisposed()
    {
        return ClassFactoryHiddenDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassFactoryHiddenDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryHiddenDisposableCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeCount, "Handler Dispose was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<DisposableCounter> ClassFactoryHiddenDisposableCountAsync(ServiceLifetime lifetime)
    {
        DisposableCounter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

            builder.Services
                .AddGatewayHandler<IRateLimitedGatewayHandler>(_ => new DisposableRateLimitedGatewayHandler(counter), lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryAsyncDisposableCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassFactoryAsyncDisposableTransientGetsDisposed()
    {
        return ClassFactoryAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassFactoryAsyncDisposableScopedGetsDisposed()
    {
        return ClassFactoryAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassFactoryAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryAsyncDisposableCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryAsyncDisposableCountAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

            builder.Services
                .AddGatewayHandler<AsyncDisposableRateLimitedGatewayHandler>(_ => new(counter), lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Hidden Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryHiddenAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryHiddenAsyncDisposableCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassFactoryHiddenAsyncDisposableTransientGetsDisposed()
    {
        return ClassFactoryHiddenAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassFactoryHiddenAsyncDisposableScopedGetsDisposed()
    {
        return ClassFactoryHiddenAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassFactoryHiddenAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryHiddenAsyncDisposableCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryHiddenAsyncDisposableCountAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

            builder.Services
                .AddGatewayHandler<IRateLimitedGatewayHandler>(_ => new AsyncDisposableRateLimitedGatewayHandler(counter), lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Disposable and Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryDisposableAndAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryDisposableAndAsyncDisposableCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassFactoryDisposableAndAsyncDisposableTransientGetsDisposed()
    {
        return ClassFactoryDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassFactoryDisposableAndAsyncDisposableScopedGetsDisposed()
    {
        return ClassFactoryDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassFactoryDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryDisposableAndAsyncDisposableCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryDisposableAndAsyncDisposableCountAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

            builder.Services
                .AddGatewayHandler<DisposableAndAsyncDisposableRateLimitedGatewayHandler>(_ => new(counter), lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Hidden Disposable and Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryHiddenDisposableAndAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryHiddenDisposableAndAsyncDisposableCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    public ValueTask ClassFactoryHiddenDisposableAndAsyncDisposableTransientGetsDisposed()
    {
        return ClassFactoryHiddenDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassFactoryHiddenDisposableAndAsyncDisposableScopedGetsDisposed()
    {
        return ClassFactoryHiddenDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassFactoryHiddenDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryHiddenDisposableAndAsyncDisposableCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryHiddenDisposableAndAsyncDisposableCountAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

            builder.Services
                .AddGatewayHandler<IRateLimitedGatewayHandler>(_ => new DisposableAndAsyncDisposableRateLimitedGatewayHandler(counter), lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Delegate
    [TestMethod]
    public ValueTask DelegateSingletonGetsCalled()
    {
        return DelegateGetsCalledAsync(ServiceLifetime.Singleton);
    }

    [TestMethod]
    public ValueTask DelegateTransientGetsCalled()
    {
        return DelegateGetsCalledAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask DelegateScopedGetsCalled()
    {
        return DelegateGetsCalledAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask DelegateGetsCalledAsync(ServiceLifetime lifetime)
    {
        var counter = await DelegateCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(0, counter.ConstructorCount);
    }

    private async ValueTask<Counter> DelegateCountAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        await AnyDelegateAsync(counter, () =>
        {
            counter.HandlerCount++;
        }, lifetime).ConfigureAwait(false);

        return counter;
    }

    // Delegate with parameters
    [TestMethod]
    public ValueTask DelegateWithParametersSingletonGetsCalled()
    {
        return DelegateWithParametersGetsCalledAsync(ServiceLifetime.Singleton);
    }

    [TestMethod]
    public ValueTask DelegateWithParametersTransientGetsCalled()
    {
        return DelegateWithParametersGetsCalledAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask DelegateWithParametersScopedGetsCalled()
    {
        return DelegateWithParametersGetsCalledAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask DelegateWithParametersGetsCalledAsync(ServiceLifetime lifetime)
    {
        var counter = await DelegateWithParametersCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(0, counter.ConstructorCount);
    }

    private async ValueTask<Counter> DelegateWithParametersCountAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        await AnyDelegateAsync(counter, (RateLimitedEventArgs arg) =>
        {
            counter.HandlerCount++;
        }, lifetime).ConfigureAwait(false);

        return counter;
    }

    private ValueTask AnyDelegateAsync(Counter counter, Delegate handler, ServiceLifetime lifetime)
    {
        return Helper.RunUntilAsync(() =>
        {
            var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

            builder.Services
                .AddGatewayHandler(GatewayEvent.RateLimited, handler, lifetime);

            return builder;
        }, () => counter.HandlerCount >= 10, testContext.CancellationToken);
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
}
