using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;

var builder = Host.CreateDefaultBuilder(args)
    .UseApplicationCommands<SlashCommandInteraction, SlashCommandContext>(options =>
    {
        options.Configuration = ApplicationCommandServiceConfiguration<SlashCommandContext>.Default with
        {
            LocalizationsProvider = new JsonLocalizationsProvider(),
        };
    })
    .UseDiscordGateway();

var host = builder.Build()
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
