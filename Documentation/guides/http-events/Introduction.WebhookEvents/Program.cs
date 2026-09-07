using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Rest;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDiscordRest()
    .AddWebhookHandler(WebhookEvent.ApplicationAuthorized, (ApplicationAuthorizedWebhookEventArgs args,
                                                            ILogger<Program> logger) =>
    {
        logger.LogInformation("User '{Username}' authorized with scopes: {Scopes}",
                              args.User.Username,
                              args.Scopes);
    });

var app = builder.Build();

// You can specify any pattern here, but remember to update it in the Discord Developer Portal
app.UseWebhookEvents("/webhooks");

await app.RunAsync();
