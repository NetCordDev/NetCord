using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services.Interactions;

namespace NetCord.Hosting.Services.Interactions;

public static class InteractionServiceHostExtensions
{
    public static IHost AddInteraction<TContext>(this IHost host,
                                                 string customId,
                                                 Delegate handler) where TContext : IInteractionContext
    {
        var service = host.Services.GetRequiredService<InteractionService<TContext>>();
        service.AddInteraction(customId, handler);
        return host;
    }

    public static IHost AddInteractionModule<TContext>(this IHost host, Type type) where TContext : IInteractionContext
    {
        var service = host.Services.GetRequiredService<InteractionService<TContext>>();
        service.AddModule(type);
        return host;
    }

    public static IHost AddInteractionModule<TContext, T>(this IHost host) where TContext : IInteractionContext
    {
        var service = host.Services.GetRequiredService<InteractionService<TContext>>();
        service.AddModule<T>();
        return host;
    }
}
