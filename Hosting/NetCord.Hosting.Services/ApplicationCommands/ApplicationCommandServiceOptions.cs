using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandServiceOptions
{
    public bool? DefaultDMPermission { get; set; }

    public IEnumerable<ApplicationIntegrationType>? DefaultIntegrationTypes { get; set; }

    public IEnumerable<InteractionContextType>? DefaultContexts { get; set; }

    /// <summary>
    /// {0} - parameter name
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
    public string? DefaultParameterDescriptionFormat { get; set; }

    public ILocalizationsProvider? LocalizationsProvider { get; set; }

    public bool? UseScopes { get; set; }
}

public class ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    public Dictionary<Type, SlashCommandTypeReader<TContext>> TypeReaders { get; set; } = ApplicationCommandServiceConfiguration<TContext>.Default.TypeReaders.ToDictionary();

    public SlashCommandTypeReader<TContext>? EnumTypeReader { get; set; }

    public bool? DefaultDMPermission { get; set; }

    public IEnumerable<ApplicationIntegrationType>? DefaultIntegrationTypes { get; set; }

    public IEnumerable<InteractionContextType>? DefaultContexts { get; set; }

    public ISlashCommandParameterNameProcessor<TContext>? ParameterNameProcessor { get; set; }

    /// <summary>
    /// {0} - parameter name
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
    public string? DefaultParameterDescriptionFormat { get; set; }

    public IResultResolverProvider<TContext>? ResultResolverProvider { get; set; }

    public ILocalizationsProvider? LocalizationsProvider { get; set; }

    public bool? UseScopes { get; set; }

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public IApplicationCommandResultHandler<TContext>? ResultHandler { get; set; }

    internal void Apply(IOptions<ApplicationCommandServiceOptions> options)
    {
        var value = options.Value;

        var defaultDMPermission = value.DefaultDMPermission;
        if (defaultDMPermission.HasValue)
            DefaultDMPermission = defaultDMPermission.GetValueOrDefault();

        var defaultIntegrationTypes = value.DefaultIntegrationTypes;
        if (defaultIntegrationTypes is not null)
            DefaultIntegrationTypes = defaultIntegrationTypes;

        var defaultContexts = value.DefaultContexts;
        if (defaultContexts is not null)
            DefaultContexts = defaultContexts;

        var defaultParameterDescriptionFormat = value.DefaultParameterDescriptionFormat;
        if (defaultParameterDescriptionFormat is not null)
            DefaultParameterDescriptionFormat = defaultParameterDescriptionFormat;

        var localizationsProvider = value.LocalizationsProvider;
        if (localizationsProvider is not null)
            LocalizationsProvider = localizationsProvider;

        var useScopes = value.UseScopes;
        if (useScopes.HasValue)
            UseScopes = useScopes;
    }

    internal ApplicationCommandServiceConfiguration<TContext> CreateConfiguration()
    {
        var configuration = ApplicationCommandServiceConfiguration<TContext>.Default;

        return configuration with
        {
            TypeReaders = TypeReaders.ToImmutableDictionary(),
            EnumTypeReader = EnumTypeReader ?? configuration.EnumTypeReader,
            DefaultDMPermission = DefaultDMPermission ?? configuration.DefaultDMPermission,
            DefaultIntegrationTypes = DefaultIntegrationTypes ?? configuration.DefaultIntegrationTypes,
            DefaultContexts = DefaultContexts ?? configuration.DefaultContexts,
            ParameterNameProcessor = ParameterNameProcessor ?? configuration.ParameterNameProcessor,
            DefaultParameterDescriptionFormat = DefaultParameterDescriptionFormat ?? configuration.DefaultParameterDescriptionFormat,
            ResultResolverProvider = ResultResolverProvider ?? configuration.ResultResolverProvider,
            LocalizationsProvider = LocalizationsProvider ?? configuration.LocalizationsProvider,
        };
    }
}

public class ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext> : ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public Func<AutocompleteInteraction, GatewayClient?, IServiceProvider, TAutocompleteContext>? CreateAutocompleteContext { get; set; }

    public IAutocompleteInteractionResultHandler<TAutocompleteContext>? AutocompleteResultHandler { get; set; }
}
