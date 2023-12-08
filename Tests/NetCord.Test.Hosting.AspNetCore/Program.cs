using NetCord;
using NetCord.Hosting;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Interactions;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Interactions;
using NetCord.Test.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseDiscordRest()
    .UseApplicationCommandService<SlashCommandInteraction, HttpSlashCommandContext>()
    .UseInteractionService<ButtonInteraction, HttpButtonInteractionContext>();

builder.Services.AddHttpInteractionHandler<InteractionHandler>();

var app = builder.Build();

app
    .AddSlashCommand<HttpSlashCommandContext>("ping", "Ping!", () => "Pong!")
    .AddSlashCommand<HttpSlashCommandContext>("button", "Button!", (string s) => new InteractionMessageProperties().AddComponents(new ActionRowProperties([new ActionButtonProperties($"button:{s}", "Button!", ButtonStyle.Primary)])))
    .AddInteraction<HttpButtonInteractionContext>("button", (string s) => $"Button! {s}");

app.UseHttpInteractions("/");

await app.RunAsync();
