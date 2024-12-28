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

    public static IHost AddSlashCommand(this IHost host,
                                        string name,
                                        string description,
                                        Delegate handler,
                                        Permissions? defaultGuildUserPermissions = null,
                                        bool? dMPermission = null,
                                        bool defaultPermission = true,
                                        IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                        IEnumerable<InteractionContextType>? contexts = null,
                                        bool nsfw = false,
                                        ulong? guildId = null)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IApplicationCommandService>(host.Services);
        service.AddSlashCommand(name,
                                description,
                                handler,
                                defaultGuildUserPermissions,
                                dMPermission,
                                defaultPermission,
                                integrationTypes,
                                contexts,
                                nsfw,
                                guildId);
        return host;
    }

    public static IHost AddUserCommand(this IHost host,
                                       string name,
                                       Delegate handler,
                                       Permissions? defaultGuildUserPermissions = null,
                                       bool? dMPermission = null,
                                       bool defaultPermission = true,
                                       IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                       IEnumerable<InteractionContextType>? contexts = null,
                                       bool nsfw = false,
                                       ulong? guildId = null)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IApplicationCommandService>(host.Services);
        service.AddUserCommand(name,
                               handler,
                               defaultGuildUserPermissions,
                               dMPermission,
                               defaultPermission,
                               integrationTypes,
                               contexts,
                               nsfw,
                               guildId);
        return host;
    }

    public static IHost AddMessageCommand(this IHost host,
                                          string name,
                                          Delegate handler,
                                          Permissions? defaultGuildUserPermissions = null,
                                          bool? dMPermission = null,
                                          bool defaultPermission = true,
                                          IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                          IEnumerable<InteractionContextType>? contexts = null,
                                          bool nsfw = false,
                                          ulong? guildId = null)
    {
        var service = ServiceProviderServiceHelper.GetSingle<IApplicationCommandService>(host.Services);
        service.AddMessageCommand(name,
                                  handler,
                                  defaultGuildUserPermissions,
                                  dMPermission,
                                  defaultPermission,
                                  integrationTypes,
                                  contexts,
                                  nsfw,
                                  guildId);
        return host;
    }

    public static IHost AddSlashCommand<TContext>(this IHost host,
                                                  string name,
                                                  string description,
                                                  Delegate handler,
                                                  Permissions? defaultGuildUserPermissions = null,
                                                  bool? dMPermission = null,
                                                  bool defaultPermission = true,
                                                  IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                                  IEnumerable<InteractionContextType>? contexts = null,
                                                  bool nsfw = false,
                                                  ulong? guildId = null) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddSlashCommand(name,
                                description,
                                handler,
                                defaultGuildUserPermissions,
                                dMPermission,
                                defaultPermission,
                                integrationTypes,
                                contexts,
                                nsfw,
                                guildId);
        return host;
    }

    public static IHost AddUserCommand<TContext>(this IHost host,
                                                 string name,
                                                 Delegate handler,
                                                 Permissions? defaultGuildUserPermissions = null,
                                                 bool? dMPermission = null,
                                                 bool defaultPermission = true,
                                                 IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                                 IEnumerable<InteractionContextType>? contexts = null,
                                                 bool nsfw = false,
                                                 ulong? guildId = null) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddUserCommand(name,
                               handler,
                               defaultGuildUserPermissions,
                               dMPermission,
                               defaultPermission,
                               integrationTypes,
                               contexts,
                               nsfw,
                               guildId);
        return host;
    }

    public static IHost AddMessageCommand<TContext>(this IHost host,
                                                    string name,
                                                    Delegate handler,
                                                    Permissions? defaultGuildUserPermissions = null,
                                                    bool? dMPermission = null,
                                                    bool defaultPermission = true,
                                                    IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                                    IEnumerable<InteractionContextType>? contexts = null,
                                                    bool nsfw = false,
                                                    ulong? guildId = null) where TContext : IApplicationCommandContext
    {
        var service = host.Services.GetRequiredService<ApplicationCommandService<TContext>>();
        service.AddMessageCommand(name,
                                  handler,
                                  defaultGuildUserPermissions,
                                  dMPermission,
                                  defaultPermission,
                                  integrationTypes,
                                  contexts,
                                  nsfw,
                                  guildId);
        return host;
    }
}
