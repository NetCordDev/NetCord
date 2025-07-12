using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

internal class ApplicationCommandServiceHostedService(IServiceProvider services, ILogger<ApplicationCommandServiceHostedService> logger, IOptions<ApplicationCommandServiceOptions> options) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var register = options.Value.AutoRegisterCommands;

        if (register.HasValue && !register.GetValueOrDefault())
            return;

        var client = services.GetRequiredService<RestClient>();

        if (client.Token is not IEntityToken token)
            throw new InvalidOperationException($"'{nameof(IEntityToken)}' is required to create application commands.");

        logger.LogInformation("Registering application commands.");

        ApplicationCommandServiceManager applicationCommandServiceManager = new();

        foreach (var service in services.GetServices<IApplicationCommandService>())
            applicationCommandServiceManager.AddService(service);

        var commands = await applicationCommandServiceManager.RegisterCommandsAsync(client, token.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

        logger.LogInformation("{count} application command(s) registered.", commands.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
