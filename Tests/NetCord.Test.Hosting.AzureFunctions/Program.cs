using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord;
using NetCord.Hosting;
using NetCord.Hosting.AzureFunctions;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddDiscordRest();
        services.AddHttpApplicationCommands();
        services.AddDiscordInteractions();
    })
    .Build();

host.AddSlashCommand("ping", "Ping!", () => "Pong!");

host.Run();
