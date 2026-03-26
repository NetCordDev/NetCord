using NetCord;
using NetCord.Hosting;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Test.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDiscordRest()
    .AddHttpApplicationCommands()
    .AddHttpComponentInteractions()
    .AddHttpInteractionHandler<InteractionHandler>()
    .AddHttpInteractionHandler((Interaction interaction, ILogger<Interaction> logger) => logger.LogInformation("Id: {}", interaction.Id))
    .AddWebhookHandler(WebhookEvent.ApplicationAuthorized, (ApplicationAuthorizedWebhookEventArgs eventArgs, ILogger<WebhookEventArgs> logger) =>
    {
        logger.LogInformation("Authorized with {} at {}", eventArgs.User.Username, eventArgs.Timestamp);
    })
    .AddWebhookHandler(WebhookEvent.ApplicationDeauthorized, (ApplicationDeauthorizedWebhookEventArgs eventArgs, ILogger<WebhookEventArgs> logger) =>
    {
        logger.LogInformation("Deauthorized with {} at {}", eventArgs.User.Username, eventArgs.Timestamp);
    })
    .AddWebhookHandler(WebhookEvent.EntitlementCreate, (EntitlementCreateWebhookEventArgs eventArgs, ILogger<WebhookEventArgs> logger) =>
    {
        logger.LogInformation("Entitlement created for {} at {}", eventArgs.Entitlement.UserId, eventArgs.Timestamp);
    })
    .AddWebhookHandler(WebhookEvent.UnknownEvent, (UnknownEventWebhookEventArgs eventArgs, ILogger<WebhookEventArgs> logger) =>
    {
        logger.LogInformation("An unknown event received {} at {}", eventArgs.Type, eventArgs.Timestamp);
    });

var app = builder.Build();

app.AddSlashCommand("ping", "Ping!", (IServiceProvider provider, HttpApplicationCommandContext context) => "Pong!");
app.AddSlashCommand("button", "Button!", (string s) => new InteractionMessageProperties().AddComponents(new ActionRowProperties([new ButtonProperties($"button:{s}", "Button!", ButtonStyle.Primary)])));
app.AddSlashCommand("echo", "Echo!", ([SlashCommandParameter(AutocompleteProviderType = typeof(EchoAutocompleteProvider))] string s) => s);
app.AddComponentInteraction("button", (string s) => $"Button! {s}");

app.UseHttpInteractions("/interactions");

app.UseWebhookEvents("/webhooks");

await app.RunAsync();
