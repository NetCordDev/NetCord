using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Gateway.Compression;
using NetCord.Gateway.WebSockets;
using NetCord.Hosting.Gateway;

namespace HostingTest;

[TestClass]
public class ShardedGatewayHandlersTest : GatewayHandlersTestBase
{
    private readonly TestContext testContext;

    public ShardedGatewayHandlersTest(TestContext testContext)
    {
        this.testContext = testContext;
    }

    private static HostApplicationBuilder CreateBuilder(IWebSocketConnectionProvider webSocketConnectionProvider)
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);

        builder.Logging.AddSimpleConsole();

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

    [TestMethod]
    public async ValueTask ShardedClassSingletonGetsCalledAndIsSingleton()
    {
        var counter = await ShardedCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was called more than once.");
    }

    [TestMethod]
    public ValueTask ShardedClassTransientGetsCalledAndIsNotSingleton()
    {
        return ShardedClassTransientOrScopedGetsCalledAndIsNotSingletonAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ShardedClassScopedCalledAndIsNotSingleton()
    {
        return ShardedClassTransientOrScopedGetsCalledAndIsNotSingletonAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ShardedClassTransientOrScopedGetsCalledAndIsNotSingletonAsync(ServiceLifetime lifetime)
    {
        var counter = await ShardedCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<Counter> ShardedCountAsync(ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        Counter counter = new();

        builder.Services
            .AddShardedGatewayHandler<RateLimitedShardedGatewayHandler>(_ => new(counter), lifetime);

        var host = builder.Build();

        await host.StartAsync(testContext.CancellationToken).ConfigureAwait(false);

        Assert.IsTrue(SpinWait.SpinUntil(() => counter.HandlerCount >= 10, TimeSpan.FromSeconds(10)), "Handler was not called enough times for 10 seconds.");

        await host.StopAsync(testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    [TestMethod]
    public ValueTask ShardedDelegateSingletonGetsCalled()
    {
        return ShardedDelegateGetsCalledAsync(ServiceLifetime.Singleton);
    }

    [TestMethod]
    public ValueTask ShardedDelegateTransientGetsCalled()
    {
        return ShardedDelegateGetsCalledAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ShardedDelegateScopedGetsCalled()
    {
        return ShardedDelegateGetsCalledAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask ShardedDelegateGetsCalledAsync(ServiceLifetime lifetime)
    {
        var counter = await ShardedDelegateCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(0, counter.ConstructorCount);
    }

    private async ValueTask<Counter> ShardedDelegateCountAsync(ServiceLifetime lifetime)
    {
        var builder = CreateBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        Counter counter = new();

        builder.Services
            .AddShardedGatewayHandler(GatewayEvent.RateLimited, () =>
            {
                counter.HandlerCount++;
            }, lifetime);

        var host = builder.Build();

        await host.StartAsync(testContext.CancellationToken).ConfigureAwait(false);

        Assert.IsTrue(SpinWait.SpinUntil(() => counter.HandlerCount >= 10, TimeSpan.FromSeconds(10)), "Handler was not called enough times for 10 seconds.");

        await host.StopAsync(testContext.CancellationToken).ConfigureAwait(false);

        return counter;
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
}

