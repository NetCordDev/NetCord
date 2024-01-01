using System.Diagnostics.CodeAnalysis;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public abstract class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    private protected ApplicationCommandInfo(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? nameTranslationsProviderType, Permissions? defaultGuildUserPermissions, bool? dMPermission, bool defaultPermission, bool nsfw, ulong? guildId, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        Name = name;

        if (nameTranslationsProviderType is not null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(nameTranslationsProviderType)!;

        DefaultGuildUserPermissions = defaultGuildUserPermissions;
        DMPermission = dMPermission.HasValue ? dMPermission.GetValueOrDefault() : configuration.DefaultDMPermission;
        DefaultPermission = defaultPermission;
        Nsfw = nsfw;
        GuildId = guildId;
    }

    private protected ApplicationCommandInfo(ApplicationCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration) : this(attribute.Name,
                                                                                                     attribute.NameTranslationsProviderType,
                                                                                                     attribute._defaultGuildUserPermissions,
                                                                                                     attribute._dMPermission,
#pragma warning disable CS0618 // Type or member is obsolete
                                                                                                     attribute.DefaultPermission,
#pragma warning restore CS0618 // Type or member is obsolete
                                                                                                     attribute.Nsfw,
                                                                                                     attribute._guildId,
                                                                                                     configuration)
    {
    }

    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }

    public abstract ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
    public abstract ApplicationCommandProperties GetRawValue();
}
