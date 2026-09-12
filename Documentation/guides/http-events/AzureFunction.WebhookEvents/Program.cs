using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Rest;

using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddDiscordRest()
    .AddWebhookHandler(WebhookEvent.ApplicationAuthorized, (ApplicationAuthorizedWebhookEventArgs args,
                                                            ILogger<Program> logger) =>
    {
        logger.LogInformation("User '{Username}' authorized with scopes: {Scopes}",
                              args.User.Username,
                              args.Scopes);
    })
    .AddWebhookEventProcessor();

var host = builder.Build();

await host.RunAsync();

public class Webhook(IWebhookEventProcessor processor)
{
    [Function("webhook")]
    public ValueTask RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
    {
        return processor.ProcessAsync(request.HttpContext);
    }
}
