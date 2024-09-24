﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public static class CommandServiceHostExtensions
{
    public static IHost AddCommand<TContext>(this IHost host,
                                             IEnumerable<string> aliases,
                                             Delegate handler,
                                             int priority = 0) where TContext : ICommandContext
    {
        var service = host.Services.GetRequiredService<CommandService<TContext>>();
        service.AddCommand(aliases, handler, priority);
        return host;
    }

    public static IHost AddCommandModule<TContext>(
        this IHost host,
        [DAM(DAMT.PublicConstructors | DAMT.PublicMethods)] Type type)

        where TContext : ICommandContext
    {
        var service = host.Services.GetRequiredService<CommandService<TContext>>();
        service.AddModule(type);
        return host;
    }

    public static IHost AddCommandModule<TContext,
                                         [DAM(DAMT.PublicConstructors | DAMT.PublicMethods)] T>(
        this IHost host)

        where TContext : ICommandContext
    {
        var service = host.Services.GetRequiredService<CommandService<TContext>>();
        service.AddModule<T>();
        return host;
    }
}
