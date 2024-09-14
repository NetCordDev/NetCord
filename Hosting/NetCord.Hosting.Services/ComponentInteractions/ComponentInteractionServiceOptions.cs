using System.Collections.Immutable;
using System.Globalization;

using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionServiceOptions
{
    /// <summary>
    /// Default = <see langword="false"/>
    /// </summary>
    public bool? IgnoreCase { get; set; }

    public char? ParameterSeparator { get; set; }

    public CultureInfo? CultureInfo { get; set; }

    public bool? UseScopes { get; set; }
}

public class ComponentInteractionServiceOptions<TInteraction, TContext> where TInteraction : Interaction where TContext : IComponentInteractionContext
{
    public Dictionary<Type, ComponentInteractionTypeReader<TContext>> TypeReaders { get; set; } = ComponentInteractionServiceConfiguration<TContext>.Default.TypeReaders.ToDictionary();

    public ComponentInteractionTypeReader<TContext>? EnumTypeReader { get; set; }

    /// <summary>
    /// Default = <see langword="false"/>
    /// </summary>
    public bool? IgnoreCase { get; set; }

    public char? ParameterSeparator { get; set; }

    public CultureInfo? CultureInfo { get; set; }

    public IResultResolverProvider<TContext>? ResultResolverProvider { get; set; }

    public bool? UseScopes { get; set; }

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public IComponentInteractionResultHandler<TContext>? ResultHandler { get; set; }

    internal void Apply(IOptions<ComponentInteractionServiceOptions> options)
    {
        var value = options.Value;

        var ignoreCase = value.IgnoreCase;
        if (ignoreCase.HasValue)
            IgnoreCase = ignoreCase;

        var parameterSeparator = value.ParameterSeparator;
        if (parameterSeparator.HasValue)
            ParameterSeparator = parameterSeparator;

        var cultureInfo = value.CultureInfo;
        if (cultureInfo is not null)
            CultureInfo = cultureInfo;

        var useScopes = value.UseScopes;
        if (useScopes.HasValue)
            UseScopes = useScopes;
    }

    internal ComponentInteractionServiceConfiguration<TContext> CreateConfiguration()
    {
        var configuration = ComponentInteractionServiceConfiguration<TContext>.Default;

        return configuration with
        {
            TypeReaders = TypeReaders.ToImmutableDictionary(),
            EnumTypeReader = EnumTypeReader ?? configuration.EnumTypeReader,
            IgnoreCase = IgnoreCase ?? configuration.IgnoreCase,
            ParameterSeparator = ParameterSeparator ?? configuration.ParameterSeparator,
            CultureInfo = CultureInfo ?? configuration.CultureInfo,
            ResultResolverProvider = ResultResolverProvider ?? configuration.ResultResolverProvider,
        };
    }
}
