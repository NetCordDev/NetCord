using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public abstract class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    private protected ApplicationCommandInfo(ApplicationCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        Name = attribute.Name;

        var nameTranslationsProviderType = attribute.NameTranslationsProviderType;
        if (nameTranslationsProviderType is not null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(nameTranslationsProviderType)!;

        DefaultGuildUserPermissions = attribute._defaultGuildUserPermissions;
        DMPermission = attribute._dMPermission.HasValue ? attribute._dMPermission.GetValueOrDefault() : configuration.DefaultDMPermission;
#pragma warning disable CS0618 // Type or member is obsolete
        DefaultPermission = attribute.DefaultPermission;
#pragma warning restore CS0618 // Type or member is obsolete
        Nsfw = attribute.Nsfw;
        if (attribute._guildId.HasValue)
            GuildId = attribute._guildId.GetValueOrDefault();
    }

    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }

    public abstract ValueTask InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
    public abstract ApplicationCommandProperties GetRawValue();
}
