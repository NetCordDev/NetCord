using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public static class CommandServiceHostExtensions
{
    public static IHost AddCommandModule(
        this IHost host,
        [DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] Type type)
    {
        var service = ServiceProviderServiceHelper.GetSingle<ICommandService>(host.Services);
        service.AddModule(type);
        return host;
    }

    public static IHost AddCommandModule<[DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] T>(
        this IHost host)
    {
        var service = ServiceProviderServiceHelper.GetSingle<ICommandService>(host.Services);
        service.AddModule<T>();
        return host;
    }

    public static IHost AddCommandModule<TContext>(
        this IHost host,
        [DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] Type type)

        where TContext : ICommandContext
    {
        var service = host.Services.GetRequiredService<CommandService<TContext>>();
        service.AddModule(type);
        return host;
    }

    public static IHost AddCommandModule<TContext,
                                         [DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] T>(
        this IHost host)

        where TContext : ICommandContext
    {
        var service = host.Services.GetRequiredService<CommandService<TContext>>();
        service.AddModule<T>();
        return host;
    }

    public static CommandBuilder AddCommand(this IHost host, IEnumerable<string> aliases, Delegate handler)
    {
        var commandsBuilder = ServiceProviderServiceHelper.GetSingle<ICommandsBuilder>(host.Services);
        return commandsBuilder.AddCommand(aliases, handler);
    }

    public static CommandGroupBuilder AddCommandGroup(this IHost host, IEnumerable<string> aliases)
    {
        var commandsBuilder = ServiceProviderServiceHelper.GetSingle<ICommandsBuilder>(host.Services);
        return commandsBuilder.AddCommandGroup(aliases);
    }

    public static CommandGroupBuilder AddCommandGroup(this IHost host, IEnumerable<string> aliases, Action<CommandGroupBuilder> builder)
    {
        var commandsBuilder = ServiceProviderServiceHelper.GetSingle<ICommandsBuilder>(host.Services);
        return commandsBuilder.AddCommandGroup(aliases, builder);
    }

    public static CommandBuilder AddCommand<TContext>(this IHost host, IEnumerable<string> aliases, Delegate handler)
        where TContext : ICommandContext
    {
        var builder = host.Services.GetRequiredService<ICommandsBuilder<TContext>>();
        return builder.AddCommand(aliases, handler);
    }

    public static CommandGroupBuilder AddCommandGroup<TContext>(this IHost host, IEnumerable<string> aliases)
        where TContext : ICommandContext
    {
        var builder = host.Services.GetRequiredService<ICommandsBuilder<TContext>>();
        return builder.AddCommandGroup(aliases);
    }

    public static CommandGroupBuilder AddCommandGroup<TContext>(this IHost host, IEnumerable<string> aliases, Action<CommandGroupBuilder> builder)
        where TContext : ICommandContext
    {
        var commandsBuilder = host.Services.GetRequiredService<ICommandsBuilder<TContext>>();
        return commandsBuilder.AddCommandGroup(aliases, builder);
    }
}
