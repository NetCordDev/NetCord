using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

internal class ApplicationCommandServiceHostedService(IServiceProvider services, ILogger<ApplicationCommandServiceHostedService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        List<IApplicationCommandService> managerServices = [];

        foreach (var serviceData in services.GetServices<ApplicationCommandServiceData>())
        {
            serviceData.Builder.Build();

            var register = serviceData.AutoRegisterCommandsFunc();
            if (register.GetValueOrDefault(true))
                managerServices.Add(serviceData.Service);
        }

        if (managerServices.Count is 0)
            return Task.CompletedTask;

        ApplicationCommandServiceManager manager = new(managerServices);

        var client = services.GetRequiredService<RestClient>();

        if (client.Token is not IEntityToken token)
            throw new InvalidOperationException($"'{nameof(IEntityToken)}' is required to create application commands.");

        return RegisterCommandsAsync(manager, client, token.Id, cancellationToken);
    }

    private async Task RegisterCommandsAsync(ApplicationCommandServiceManager manager, RestClient client, ulong applicationId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering application commands.");

        var commands = await manager.RegisterCommandsAsync(client, applicationId, cancellationToken: cancellationToken).ConfigureAwait(false);

        logger.LogInformation("{count} application command(s) registered.", commands.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
