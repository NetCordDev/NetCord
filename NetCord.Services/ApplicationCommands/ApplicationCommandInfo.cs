using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Utils;

namespace NetCord.Services.ApplicationCommands;

public abstract class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    private protected ApplicationCommandInfo(ApplicationCommandAttribute attribute,
                                             ApplicationCommandServiceConfiguration<TContext> configuration,
                                             MemberInfo? member,
                                             out Attribute[] memberAttributes) : this(attribute.Name,
                                                                        attribute._defaultGuildPermissions,
                                                                        attribute.IntegrationTypes,
                                                                        attribute.Contexts,
                                                                        attribute.Nsfw,
                                                                        attribute.Register,
                                                                        configuration,
                                                                        member,
                                                                        out memberAttributes)
    {
    }

    private protected ApplicationCommandInfo(ApplicationCommandBuilder builder,
                                             ApplicationCommandServiceConfiguration<TContext> configuration,
                                             MemberInfo? member,
                                             out Attribute[] memberAttributes) : this(builder.Name,
                                                                        builder.DefaultGuildPermissions,
                                                                        builder.IntegrationTypes,
                                                                        builder.Contexts,
                                                                        builder.Nsfw,
                                                                        builder.Register,
                                                                        configuration,
                                                                        member,
                                                                        out memberAttributes)
    {
    }

    private ApplicationCommandInfo(string name,
                                   Permissions? defaultGuildPermissions,
                                   IEnumerable<ApplicationIntegrationType>? integrationTypes,
                                   IEnumerable<InteractionContextType>? contexts,
                                   bool nsfw,
                                   bool register,
                                   ApplicationCommandServiceConfiguration<TContext> configuration,
                                   MemberInfo? member,
                                   out Attribute[] memberAttributes)
    {
        Name = name;
        LocalizationsProvider = configuration.LocalizationsProvider;
        LocalizationPath = [new ApplicationCommandLocalizationPathSegment(name)];
        DefaultGuildPermissions = defaultGuildPermissions;
        IntegrationTypes = integrationTypes ?? configuration.DefaultIntegrationTypes;
        Contexts = contexts ?? configuration.DefaultContexts;
        Nsfw = nsfw;
        Register = register;

        if (member is null)
        {
            memberAttributes = [];
            Attributes = FrozenDictionary<Type, IReadOnlyList<Attribute>>.Empty;
        }
        else
        {
            memberAttributes = Attribute.GetCustomAttributes(member);
            Attributes = memberAttributes.ToRankedFrozenDictionary(a => a.GetType());
        }
    }

    public abstract ApplicationCommandType Type { get; }
    public string Name { get; }
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public Permissions? DefaultGuildPermissions { get; }
    public IEnumerable<ApplicationIntegrationType>? IntegrationTypes { get; }
    public IEnumerable<InteractionContextType>? Contexts { get; }
    public bool Nsfw { get; }
    public bool Register { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }

    public abstract ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
    public abstract ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default);
}
