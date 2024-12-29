using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MyBot;

using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSingleton<ISomeService, SomeService>()
    .AddDiscordGateway(o => o.Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent)
    .AddCommands<CommandContext>()
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);

host.AddSlashCommand(
        name: "data",
        description: "Shows the data!",
        (ISomeService someService, ApplicationCommandContext context, int count) => string.Join(' ',
                                                                                                someService.GetSomeData()
                                                                                                           .Take(count)));

host.UseGatewayEventHandlers();

await host.RunAsync();
