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
        ApplicationCommandServiceManager applicationCommandServiceManager = new();

        foreach (var service in _services.GetServices<IApplicationCommandService>())
            applicationCommandServiceManager.AddService(service);

        var client = _services.GetRequiredService<RestClient>();

        var token = client.Token ?? throw new InvalidOperationException("The token is required to create application commands.");

        _logger.LogInformation("Creating application commands...");
        var commands = await applicationCommandServiceManager.CreateCommandsAsync(client, token.Id, true).ConfigureAwait(false);
        _logger.LogInformation("{count} application command(s) created", commands.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
