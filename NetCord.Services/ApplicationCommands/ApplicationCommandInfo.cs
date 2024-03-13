using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public abstract class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    private protected ApplicationCommandInfo(string name,
                                             Permissions? defaultGuildUserPermissions,
                                             bool? dMPermission,
                                             bool defaultPermission,
                                             bool nsfw,
                                             ulong? guildId,
                                             ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        Name = name;

        LocalizationsProvider = configuration.LocalizationsProvider;

        LocalizationPath = [new ApplicationCommandLocalizationPathSegment(name)];

        DefaultGuildUserPermissions = defaultGuildUserPermissions;
        DMPermission = dMPermission.HasValue ? dMPermission.GetValueOrDefault() : configuration.DefaultDMPermission;
        DefaultPermission = defaultPermission;
        Nsfw = nsfw;
        GuildId = guildId;
    }

    private protected ApplicationCommandInfo(ApplicationCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration) : this(attribute.Name,
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
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }
    public abstract LocalizationPathSegment LocalizationPathSegment { get; }

    public abstract ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
    public abstract ValueTask<ApplicationCommandProperties> GetRawValueAsync();
}
