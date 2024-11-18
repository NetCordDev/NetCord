using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public static class ComponentInteractionServiceHostExtensions
{
    public static IHost AddComponentInteractionModule(
        this IHost host,
        [DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] Type type)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IComponentInteractionService>(host.Services);
        service.AddModule(type);
        return host;
    }

    public static IHost AddComponentInteractionModule<[DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] T>(
        this IHost host)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IComponentInteractionService>(host.Services);
        service.AddModule<T>();
        return host;
    }

    public static IHost AddComponentInteractionModule<TContext>(
        this IHost host,
        [DAM(DAMT.PublicConstructors | DAMT.PublicMethods)] Type type)

        where TContext : IComponentInteractionContext
    {
        var service = host.Services.GetRequiredService<ComponentInteractionService<TContext>>();
        service.AddModule(type);
        return host;
    }

    public static IHost AddComponentInteractionModule<TContext,
                                                      [DAM(DAMT.PublicConstructors | DAMT.PublicMethods)] T>(
        this IHost host)

        where TContext : IComponentInteractionContext
    {
        var service = host.Services.GetRequiredService<ComponentInteractionService<TContext>>();
        service.AddModule<T>();
        return host;
    }

    public static IHost AddComponentInteraction(this IHost host,
                                                          string customId,
                                                          Delegate handler)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IComponentInteractionService>(host.Services);
        service.AddInteraction(customId, handler);
        return host;
    }

    public static IHost AddComponentInteraction<TContext>(this IHost host,
                                                          string customId,
                                                          Delegate handler) where TContext : IComponentInteractionContext
    {
        var service = host.Services.GetRequiredService<ComponentInteractionService<TContext>>();
        service.AddInteraction(customId, handler);
        return host;
    }
}
