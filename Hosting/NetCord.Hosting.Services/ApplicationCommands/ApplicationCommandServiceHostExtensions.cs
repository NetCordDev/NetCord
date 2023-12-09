using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public static class ApplicationCommandServiceHostExtensions
{
    public static IHost AddSlashCommand<TContext>(this IHost host,
                                                  string name,
                                                  string description,
                                                  Delegate handler,
                                                  Type? nameTranslationsProviderType = null,
                                                  Type? descriptionTranslationsProviderType = null,
                                                  Permissions? defaultGuildUserPermissions = null,
                                                  bool? dMPermission = null,
                                                  bool defaultPermission = true,
                                                  bool nsfw = false,
                                                  ulong? guildId = null) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddSlashCommand(name, description, handler, nameTranslationsProviderType, descriptionTranslationsProviderType, defaultGuildUserPermissions, dMPermission, defaultPermission, nsfw, guildId);
        return host;
    }

    public static IHost AddUserCommand<TContext>(this IHost host,
                                                 string name,
                                                 Delegate handler,
                                                 Type? nameTranslationsProviderType = null,
                                                 Permissions? defaultGuildUserPermissions = null,
                                                 bool? dMPermission = null,
                                                 bool defaultPermission = true,
                                                 bool nsfw = false,
                                                 ulong? guildId = null) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddUserCommand(name, handler, nameTranslationsProviderType, defaultGuildUserPermissions, dMPermission, defaultPermission, nsfw, guildId);
        return host;
    }

    public static IHost AddMessageCommand<TContext>(this IHost host,
                                                    string name,
                                                    Delegate handler,
                                                    Type? nameTranslationsProviderType = null,
                                                    Permissions? defaultGuildUserPermissions = null,
                                                    bool? dMPermission = null,
                                                    bool defaultPermission = true,
                                                    bool nsfw = false,
                                                    ulong? guildId = null) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddMessageCommand(name, handler, nameTranslationsProviderType, defaultGuildUserPermissions, dMPermission, defaultPermission, nsfw, guildId);
        return host;
    }

    public static IHost AddApplicationCommandModule<TContext>(this IHost host, Type type) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddModule(type);
        return host;
    }

    public static IHost AddApplicationCommandModule<TContext, T>(this IHost host) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddModule<T>();
        return host;
    }
}
