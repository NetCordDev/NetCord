using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

internal class ApplicationCommandServiceHostedService(IServiceProvider services, ILogger<ApplicationCommandServiceHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var client = services.GetRequiredService<RestClient>();

        if (client.Token is not IEntityToken token)
            throw new InvalidOperationException($"'{nameof(IEntityToken)}' is required to create application commands.");

        logger.LogInformation("Creating application commands...");

        ApplicationCommandServiceManager applicationCommandServiceManager = new();

        foreach (var service in services.GetServices<IApplicationCommandService>())
            applicationCommandServiceManager.AddService(service);

        var commands = await applicationCommandServiceManager.CreateCommandsAsync(client, token.Id, true).ConfigureAwait(false);

        logger.LogInformation("{count} application command(s) created", commands.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
