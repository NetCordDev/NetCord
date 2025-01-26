using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDiscordRest()
    .AddHttpApplicationCommands();

var app = builder.Build();

app.AddSlashCommand("ping", "Ping!", () => "Pong!");

// You can specify any pattern here, but remember to update it in the Discord Developer Portal
app.UseHttpInteractions("/interactions");

await app.RunAsync();
