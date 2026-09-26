using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public static class ApplicationCommandServiceHostExtensions
{
    public static IHost AddApplicationCommandModule(
        this IHost host,
        [DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] Type type)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IApplicationCommandService>(host.Services);
        service.AddModule(type);
        return host;
    }

    public static IHost AddApplicationCommandModule<[DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] T>(
        this IHost host)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IApplicationCommandService>(host.Services);
        service.AddModule<T>();
        return host;
    }

    public static IHost AddApplicationCommandModule<TContext>(
        this IHost host,
        [DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] Type type)

        where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddModule(type);
        return host;
    }

    public static IHost AddApplicationCommandModule<TContext,
                                                    [DAM(DAMT.PublicConstructors | DAMT.PublicMethods | DAMT.PublicNestedTypes)] T>(
        this IHost host)

        where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddModule<T>();
        return host;
    }

    public static SlashCommandBuilder AddSlashCommand(this IHost host, string name, string description, Delegate handler)
    {
        var builder = ServiceProviderServiceHelper.GetSingle<IApplicationCommandsBuilder>(host.Services);
        return builder.AddSlashCommand(name, description, handler);
    }

    public static SlashCommandGroupBuilder AddSlashCommandGroup(this IHost host, string name, string description)
    {
        var builder = ServiceProviderServiceHelper.GetSingle<IApplicationCommandsBuilder>(host.Services);
        return builder.AddSlashCommandGroup(name, description);
    }

    public static SlashCommandGroupBuilder AddSlashCommandGroup(this IHost host, string name, string description, Action<SlashCommandGroupBuilder> builder)
    {
        var commandsBuilder = ServiceProviderServiceHelper.GetSingle<IApplicationCommandsBuilder>(host.Services);
        return commandsBuilder.AddSlashCommandGroup(name, description, builder);
    }

    public static UserCommandBuilder AddUserCommand(this IHost host, string name, Delegate handler)
    {
        var builder = ServiceProviderServiceHelper.GetSingle<IApplicationCommandsBuilder>(host.Services);
        return builder.AddUserCommand(name, handler);
    }

    public static MessageCommandBuilder AddMessageCommand(this IHost host, string name, Delegate handler)
    {
        var builder = ServiceProviderServiceHelper.GetSingle<IApplicationCommandsBuilder>(host.Services);
        return builder.AddMessageCommand(name, handler);
    }

    public static EntryPointCommandBuilder AddEntryPointCommand(this IHost host, string name, string description)
    {
        var builder = ServiceProviderServiceHelper.GetSingle<IApplicationCommandsBuilder>(host.Services);
        return builder.AddEntryPointCommand(name, description);
    }

    public static SlashCommandBuilder AddSlashCommand<TContext>(this IHost host, string name, string description, Delegate handler)
        where TContext : IApplicationCommandContext
    {
        var builder = host.Services.GetRequiredService<IApplicationCommandsBuilder<TContext>>();
        return builder.AddSlashCommand(name, description, handler);
    }

    public static SlashCommandGroupBuilder AddSlashCommandGroup<TContext>(this IHost host, string name, string description)
        where TContext : IApplicationCommandContext
    {
        var builder = host.Services.GetRequiredService<IApplicationCommandsBuilder<TContext>>();
        return builder.AddSlashCommandGroup(name, description);
    }

    public static SlashCommandGroupBuilder AddSlashCommandGroup<TContext>(this IHost host, string name, string description, Action<SlashCommandGroupBuilder> builder)
        where TContext : IApplicationCommandContext
    {
        var commandsBuilder = host.Services.GetRequiredService<IApplicationCommandsBuilder<TContext>>();
        return commandsBuilder.AddSlashCommandGroup(name, description, builder);
    }

    public static UserCommandBuilder AddUserCommand<TContext>(this IHost host, string name, Delegate handler)
        where TContext : IApplicationCommandContext
    {
        var builder = host.Services.GetRequiredService<IApplicationCommandsBuilder<TContext>>();
        return builder.AddUserCommand(name, handler);
    }

    public static MessageCommandBuilder AddMessageCommand<TContext>(this IHost host, string name, Delegate handler)
        where TContext : IApplicationCommandContext
    {
        var builder = host.Services.GetRequiredService<IApplicationCommandsBuilder<TContext>>();
        return builder.AddMessageCommand(name, handler);
    }

    public static EntryPointCommandBuilder AddEntryPointCommand<TContext>(this IHost host, string name, string description)
        where TContext : IApplicationCommandContext
    {
        var builder = host.Services.GetRequiredService<IApplicationCommandsBuilder<TContext>>();
        return builder.AddEntryPointCommand(name, description);
    }
}
