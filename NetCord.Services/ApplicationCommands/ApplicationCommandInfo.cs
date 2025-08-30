using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public abstract class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    private protected ApplicationCommandInfo(ApplicationCommandAttribute attribute,
                                             ApplicationCommandServiceConfiguration<TContext> configuration) : this(attribute.Name,
                                                                                                                    attribute._defaultGuildUserPermissions,
                                                                                                                    attribute._dMPermission,
#pragma warning disable CS0618 // Type or member is obsolete
                                                                                                                    attribute.DefaultPermission,
#pragma warning restore CS0618 // Type or member is obsolete
                                                                                                                    attribute.IntegrationTypes,
                                                                                                                    attribute.Contexts,
                                                                                                                    attribute.Nsfw,
                                                                                                                    attribute.Register,
                                                                                                                    configuration)
    {
    }

    private protected ApplicationCommandInfo(ApplicationCommandBuilder builder,
                                             ApplicationCommandServiceConfiguration<TContext> configuration) : this(builder.Name,
                                                                                                                    builder.DefaultGuildUserPermissions,
                                                                                                                    builder.DMPermission,
                                                                                                                    builder.DefaultPermission,
                                                                                                                    builder.IntegrationTypes,
                                                                                                                    builder.Contexts,
                                                                                                                    builder.Nsfw,
                                                                                                                    builder.Register,
                                                                                                                    configuration)
    {
    }

    private ApplicationCommandInfo(string name,
                                   Permissions? defaultGuildUserPermissions,
                                   bool? dMPermission,
                                   bool defaultPermission,
                                   IEnumerable<ApplicationIntegrationType>? integrationTypes,
                                   IEnumerable<InteractionContextType>? contexts,
                                   bool nsfw,
                                   bool register,
                                   ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        Name = name;
        LocalizationsProvider = configuration.LocalizationsProvider;
        LocalizationPath = [new ApplicationCommandLocalizationPathSegment(name)];
        DefaultGuildUserPermissions = defaultGuildUserPermissions;
        DMPermission = dMPermission.HasValue ? dMPermission.GetValueOrDefault() : configuration.DefaultDMPermission;
        DefaultPermission = defaultPermission;
        IntegrationTypes = integrationTypes ?? configuration.DefaultIntegrationTypes;
        Contexts = contexts ?? configuration.DefaultContexts;
        Nsfw = nsfw;
        Register = register;
    }

    public string Name { get; }
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public IEnumerable<ApplicationIntegrationType>? IntegrationTypes { get; }
    public IEnumerable<InteractionContextType>? Contexts { get; }
    public bool Nsfw { get; }
    public bool Register { get; }

    public abstract ApplicationCommandType Type { get; }

    public abstract ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
    public abstract ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default);
}
