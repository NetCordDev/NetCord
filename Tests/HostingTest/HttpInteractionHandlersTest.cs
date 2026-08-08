using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord;
using NetCord.Hosting;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Rest;

namespace HostingTest;

[TestClass]
public class HttpInteractionHandlersTest(TestContext testContext)
{
    private static HostApplicationBuilder CreateHttpInteractionHostBuilder()
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);

        builder.Logging.AddSimpleConsole();

        builder.Services
            .AddDiscordRest()
            .AddHttpInteractionHandlerInvoker();

        return builder;
    }

    [TestMethod]
    public async ValueTask Singleton()
    {
        var counter = await CountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was called more than once.");
    }

    [TestMethod]
    public ValueTask Transient()
    {
        return TransientOrScopedAsync(ServiceLifetime.Transient);
    }

    [TestMethod]
    public ValueTask Scoped()
    {
        return TransientOrScopedAsync(ServiceLifetime.Scoped);
    }

    private async ValueTask TransientOrScopedAsync(ServiceLifetime lifetime)
    {
        var counter = await CountAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");
    }

    private async ValueTask<Counter> CountAsync(ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder();

        Counter counter = new();

        builder.Services
            .AddHttpInteractionHandler<HttpInteractionHandler>(_ => new(counter), lifetime);

        var host = builder.Build();

        await host.StartAsync(testContext.CancellationToken).ConfigureAwait(false);

        var client = host.Services.GetRequiredService<RestClient>();
        var invoker = host.Services.GetRequiredService<IHttpInteractionHandlerInvoker>();

        var interaction = Interaction.CreateFromJson(
            new()
            {
                Type = InteractionType.ApplicationCommand,
                Data = new()
                {
                    Type = ApplicationCommandType.ChatInput,
                    Name = "test",
                    Id = 123,
                    Options = [],
                },
                User = new()
                {
                    Id = 1234,
                    Username = "test",
                },
                Channel = new()
                {
                    Id = 12345,
                    Type = ChannelType.DMChannel,
                },
                Entitlements = [],
            },
            null,
            (_, _, _, _, _) => Task.FromResult((InteractionCallbackResponse?)null),
            client);

        for (int i = 0; i < 10; i++)
            await invoker.InvokeAsync(interaction).ConfigureAwait(false);

        await host.StopAsync(testContext.CancellationToken).ConfigureAwait(false);

        Assert.AreEqual(10, counter.HandlerCount, "Handler constructor was not called the same amount of times as the handler was called.");

        return counter;
    }

    private class HttpInteractionHandler : IHttpInteractionHandler
    {
        private readonly Counter _counter;

        public HttpInteractionHandler(Counter counter)
        {
            _counter = counter;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(Interaction interaction)
        {
            _counter.HandlerCount++;

            return default;
        }
    }
}
