
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NetCord.Hosting.Services.ComponentInteractions;

internal class ComponentInteractionServiceHostedService(IServiceProvider services) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var serviceData in services.GetServices<ComponentInteractionServiceData>())
            serviceData.Builder.Build();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
