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
public class HttpEventHandlersTest(TestContext testContext)
{
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

    [TestMethod]
    public async ValueTask HttpInteractionSingleton()
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);

        builder.Logging.AddSimpleConsole();

        Counter counter = new();

        builder.Services
            .AddDiscordRest()
            .AddHttpInteractionHandlerInvoker()
            .AddHttpInteractionHandler<HttpInteractionHandler>(_ => new(counter));

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

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was called more than once.");

        Assert.AreEqual(10, counter.HandlerCount, "Handler constructor was not called the same amount of times as the handler was called.");
    }
}

