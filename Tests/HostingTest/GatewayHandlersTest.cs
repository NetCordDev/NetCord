using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Gateway.Compression;
using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Gateway.WebSockets;
using NetCord.Hosting.Gateway;

namespace HostingTest;

[TestClass]
public partial class GatewayHandlersTest(TestContext testContext)
{
    private sealed partial class RateLimitedWebSocketConnection : MockWebSocketConnection
    {
        protected override JsonGatewayMessage CreateMessage(int seq)
        {
            return new()
            {
                SequenceNumber = seq,
                Event = "RATE_LIMITED",
                Data = JsonSerializer.SerializeToElement(new JsonRateLimitedEventArgs()
                {
                    Opcode = GatewayOpcode.RequestGuildUsers,
                    RetryAfter = 1,
                    Metadata = JsonSerializer.SerializeToElement(new JsonRequestGuildUsersRateLimitMetadata()
                    {
                        GuildId = 123,
                    }),
                }),
                Opcode = GatewayOpcode.Dispatch,
            };
        }
    }

    private abstract partial class MockWebSocketConnection : IWebSocketConnection
    {
        private int _seq;

        private MemoryStream? _stream;

        private CancellationTokenSource? _cancellationTokenSource;

        public int? CloseStatus => null;

        public string? CloseStatusDescription => null;

        public void Abort()
        {
            _cancellationTokenSource?.Cancel();
        }

        public ValueTask CloseAsync(int closeStatus, string? closeStatusDescription, CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource?.Cancel();

            return default;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }

        public ValueTask OpenAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            _seq = 0;
            _cancellationTokenSource = new();

            return default;
        }

        protected abstract JsonGatewayMessage CreateMessage(int seq);

        public async ValueTask<WebSocketConnectionReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_stream is { } existingStream)
            {
                var existingResult = Copy(buffer, existingStream);

                if (existingResult.EndOfMessage)
                    _stream = null;

                return existingResult;
            }

            if (_cancellationTokenSource is { IsCancellationRequested: true })
                return new(0, WebSocketMessageType.Close, true);

            var message = CreateMessage(_seq++);

            MemoryStream stream = new();

            JsonSerializer.Serialize(new Utf8JsonWriter(stream), message);

            stream.Position = 0;

            var result = Copy(buffer, stream);

            if (!result.EndOfMessage)
                _stream = stream;

            return result;

            static WebSocketConnectionReceiveResult Copy(Memory<byte> buffer, MemoryStream stream)
            {
                var remainingBytes = (int)(stream.Length - stream.Position);

                var length = Math.Min(buffer.Length, remainingBytes);

                stream.ReadExactly(buffer.Span[..length]);

                return new(length, WebSocketMessageType.Text, remainingBytes <= buffer.Length);
            }
        }

        public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, WebSocketMessageFlags messageFlags, CancellationToken cancellationToken = default)
        {
            return default;
        }

        protected class JsonGatewayMessage
        {
            [JsonPropertyName("op")]
            public GatewayOpcode Opcode { get; set; }

            [JsonPropertyName("d")]
            public JsonElement? Data { get; set; }

            [JsonPropertyName("s")]
            public int? SequenceNumber { get; set; }

            [JsonPropertyName("t")]
            public string? Event { get; set; }
        }
    }

    private sealed class MockWebSocketConnectionProvider<TConnection> : IWebSocketConnectionProvider where TConnection : IWebSocketConnection, new()
    {
        public IWebSocketConnection CreateConnection() => new TConnection();
    }

    private static HostApplicationBuilder CreateMockedGatewayBuilder(IWebSocketConnectionProvider webSocketConnectionProvider)
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);

        builder.Logging.AddSimpleConsole();

        builder.Services
            .AddDiscordGateway(o =>
            {
                o.WebSocketConnectionProvider = webSocketConnectionProvider;
                o.Compression = new UncompressedGatewayCompression();
                o.Token = "NO.T.A.REAL.TOKEN";
            });

        return builder;
    }

    private static HostApplicationBuilder CreateMockedShardedGatewayBuilder(IWebSocketConnectionProvider webSocketConnectionProvider)
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
    public async ValueTask ClassSingletonGetsCalledAndIsSingleton()
    {
        var counter = await ClassCountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was called more than once.");
    }

    [TestMethod]
    public ValueTask ClassTransientGetsCalledAndIsNotSingleton()
    {
        return ClassTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask ClassScopedGetsCalledAndIsNotSingleton()
    {
        return ClassTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime.Scoped);
    }

    private async ValueTask ClassTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime lifetime)
    {
        var counter = await ClassCountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<Counter> ClassCountAsync(ServiceLifetime lifetime)
    {
        var builder = CreateMockedGatewayBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        Counter counter = new();

        builder.Services
            .AddGatewayHandler<RateLimitedGatewayHandler>(_ => new(counter), lifetime);

        var host = builder.Build();

        await host.StartAsync(testContext.CancellationToken).ConfigureAwait(false);

        Assert.IsTrue(SpinWait.SpinUntil(() => counter.HandlerCount >= 10, TimeSpan.FromSeconds(10)), "Handler was not called enough times for 10 seconds.");

        await host.StopAsync(testContext.CancellationToken).ConfigureAwait(false);

        return counter;
    }

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
        var builder = CreateMockedGatewayBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

        Counter counter = new();

        builder.Services
            .AddGatewayHandler(GatewayEvent.RateLimited, () =>
            {
                counter.HandlerCount++;
            }, lifetime);

        var host = builder.Build();

        await host.StartAsync(testContext.CancellationToken).ConfigureAwait(false);

        Assert.IsTrue(SpinWait.SpinUntil(() => counter.HandlerCount >= 10, TimeSpan.FromSeconds(10)), "Handler was not called enough times for 10 seconds.");

        await host.StopAsync(testContext.CancellationToken).ConfigureAwait(false);

        return counter;
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
        var builder = CreateMockedShardedGatewayBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

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
        var builder = CreateMockedShardedGatewayBuilder(new MockWebSocketConnectionProvider<RateLimitedWebSocketConnection>());

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
