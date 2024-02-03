using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

internal class ApplicationCommandServiceHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ApplicationCommandServiceHostedService> _logger;

    public ApplicationCommandServiceHostedService(IServiceProvider services, ILogger<ApplicationCommandServiceHostedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var client = _services.GetRequiredService<RestClient>();

        if (client.Token is not IEntityToken token)
            throw new InvalidOperationException($"'{nameof(IEntityToken)}' is required to create application commands.");

        _logger.LogInformation("Creating application commands...");

        ApplicationCommandServiceManager applicationCommandServiceManager = new();

        foreach (var service in _services.GetServices<IApplicationCommandService>())
            applicationCommandServiceManager.AddService(service);

        var commands = await applicationCommandServiceManager.CreateCommandsAsync(client, token.Id, true).ConfigureAwait(false);

        _logger.LogInformation("{count} application command(s) created", commands.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
