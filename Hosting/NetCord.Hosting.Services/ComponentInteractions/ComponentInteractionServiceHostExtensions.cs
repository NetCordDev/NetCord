using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public static class ComponentInteractionServiceHostExtensions
{
    public static IHost AddComponentInteraction<TContext>(this IHost host,
                                                          string customId,
                                                          Delegate handler) where TContext : IComponentInteractionContext
    {
        var service = host.Services.GetRequiredService<ComponentInteractionService<TContext>>();
        service.AddInteraction(customId, handler);
        return host;
    }

    public static IHost AddComponentInteractionModule<TContext>(this IHost host, Type type) where TContext : IComponentInteractionContext
    {
        var service = host.Services.GetRequiredService<ComponentInteractionService<TContext>>();
        service.AddModule(type);
        return host;
    }

    public static IHost AddComponentInteractionModule<TContext, T>(this IHost host) where TContext : IComponentInteractionContext
    {
        var service = host.Services.GetRequiredService<ComponentInteractionService<TContext>>();
        service.AddModule<T>();
        return host;
    }
}
