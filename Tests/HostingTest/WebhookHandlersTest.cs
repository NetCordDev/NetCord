// using System.Text.Json;
// using System.Text.Json.Serialization;
//
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
//
// using NetCord;
// using NetCord.Hosting.AspNetCore;
// using NetCord.Hosting.Rest;
// using NetCord.JsonModels;
// using NetCord.Rest;
// using NetCord.Rest.JsonModels;
//
// namespace HostingTest;
//
// [TestClass]
// public class WebhookHandlersTest(TestContext testContext)
// {
//     private static HostApplicationBuilder CreateWebhookHostBuilder()
//     {
//         var builder = Host.CreateEmptyApplicationBuilder(null);
//
//         builder.Logging.AddSimpleConsole();
//
//         builder.Services
//             .AddDiscordRest()
//             .AddWebhookEventHandlerInvoker();
//
//         return builder;
//     }
//
//     [TestMethod]
//     public async ValueTask Singleton()
//     {
//         var counter = await CountAsync(ServiceLifetime.Singleton).ConfigureAwait(false);
//
//         Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was called more than once.");
//     }
//
//     [TestMethod]
//     public ValueTask Transient()
//     {
//         return TransientOrScopedAsync(ServiceLifetime.Transient);
//     }
//
//     [TestMethod]
//     public ValueTask Scoped()
//     {
//         return TransientOrScopedAsync(ServiceLifetime.Scoped);
//     }
//
//     private async ValueTask TransientOrScopedAsync(ServiceLifetime lifetime)
//     {
//         var counter = await CountAsync(lifetime).ConfigureAwait(false);
//
//         Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same amount of times as the handler was called.");
//     }
//
//     private async ValueTask<Counter> CountAsync(ServiceLifetime lifetime)
//     {
//         var builder = CreateWebhookHostBuilder();
//
//         Counter counter = new();
//
//         builder.Services
//             .AddWebhookHandler<ApplicationDeauthorizedWebhookHandler>(_ => new(counter), lifetime);
//
//         var host = builder.Build();
//
//         await host.StartAsync(testContext.CancellationToken).ConfigureAwait(false);
//
//         var client = host.Services.GetRequiredService<RestClient>();
//         var invoker = host.Services.GetRequiredService<IWebhookEventHandlerInvoker>();
//
//         var args = WebhookEventArgs.CreateFromJson(new JsonWebhookEventArgs
//         {
//             Type = WebhookEventType.Event,
//             Event = new()
//             {
//                 Type = "APPLICATION_DEAUTHORIZED",
//                 Data = JsonSerializer.SerializeToElement(new JsonApplicationAuthorizedWebhookEventData()
//                 {
//                     User = new()
//                     {
//                         Id = 1234,
//                         Username = "test",
//                     },
//                     Scopes = [],
//                 })
//             }
//         }, client);
//
//         for (int i = 0; i < 10; i++)
//             await invoker.InvokeAsync(args).ConfigureAwait(false);
//
//         await host.StopAsync(testContext.CancellationToken).ConfigureAwait(false);
//
//         Assert.AreEqual(10, counter.HandlerCount, "Handler constructor was not called the same amount of times as the handler was called.");
//
//         return counter;
//     }
//
//     private class JsonApplicationAuthorizedWebhookEventData
//     {
//         [JsonPropertyName("integration_type")]
//         public ApplicationIntegrationType? IntegrationType { get; set; }
//
//         [JsonPropertyName("user")]
//         public required JsonUser User { get; set; }
//
//         [JsonPropertyName("scopes")]
//         public required string[] Scopes { get; set; }
//
//         [JsonPropertyName("guild")]
//         public JsonGuild? Guild { get; set; }
//     }
//
//     private class ApplicationDeauthorizedWebhookHandler : IApplicationDeauthorizedWebhookHandler
//     {
//         private readonly Counter _counter;
//
//         public ApplicationDeauthorizedWebhookHandler(Counter counter)
//         {
//             _counter = counter;
//
//             counter.ConstructorCount++;
//         }
//
//         public ValueTask HandleAsync(ApplicationDeauthorizedWebhookEventArgs arg)
//         {
//             _counter.HandlerCount++;
//
//             return default;
//         }
//     }
// }
