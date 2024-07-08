using NetCord;
using NetCord.Hosting;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using NetCord.Test.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseDiscordRest()
    .UseApplicationCommands<SlashCommandInteraction, HttpSlashCommandContext>()
    .UseComponentInteractions<ButtonInteraction, HttpButtonInteractionContext>();

builder.Services
    .AddHttpInteractionHandler<InteractionHandler>()
    .AddHttpInteractionHandler((Interaction interaction, ILogger<Interaction> logger) => logger.LogInformation("Id: {}", interaction.Id));

var app = builder.Build();

app
    .AddSlashCommand<HttpSlashCommandContext>("ping", "Ping!", (IServiceProvider provider, HttpSlashCommandContext context) => "Pong!")
    .AddSlashCommand<HttpSlashCommandContext>("button", "Button!", (string s) => new InteractionMessageProperties().AddComponents(new ActionRowProperties([new ButtonProperties($"button:{s}", "Button!", ButtonStyle.Primary)])))
    .AddComponentInteraction<HttpButtonInteractionContext>("button", (string s) => $"Button! {s}");

app.UseHttpInteractions("/");

await app.RunAsync();
