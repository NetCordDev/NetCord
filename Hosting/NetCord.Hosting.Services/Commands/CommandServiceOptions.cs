using System.Collections.Immutable;
using System.Globalization;

using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandServiceOptions
{
    /// <summary>
    /// Default = ' ', '\n'
    /// </summary>
    public IEnumerable<char>? ParameterSeparators { get; set; }

    /// <summary>
    /// Default = <see langword="true"/>
    /// </summary>
    public bool? IgnoreCase { get; set; }

    public CultureInfo? CultureInfo { get; set; }

    public bool? UseScopes { get; set; }

    public string? Prefix { get; set; }

    public IEnumerable<string>? Prefixes { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, ValueTask<int>>? GetPrefixLengthAsync { get; set; }
}

public class CommandServiceOptions<TContext>
    where TContext : ICommandContext
{
    public Dictionary<Type, CommandTypeReader<TContext>> TypeReaders { get; set; } = CommandServiceConfiguration<TContext>.Default.TypeReaders.ToDictionary();

    public CommandTypeReader<TContext>? EnumTypeReader { get; set; }

    /// <summary>
    /// Default = ' ', '\n'
    /// </summary>
    public IEnumerable<char>? ParameterSeparators { get; set; }

    /// <summary>
    /// Default = <see langword="true"/>
    /// </summary>
    public bool? IgnoreCase { get; set; }

    public CultureInfo? CultureInfo { get; set; }

    public IResultResolverProvider<TContext>? ResultResolverProvider { get; set; }

    public bool? UseScopes { get; set; }

    public string? Prefix { get; set; }

    public IEnumerable<string>? Prefixes { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, ValueTask<int>>? GetPrefixLengthAsync { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, TContext>? CreateContext { get; set; }

    public ICommandResultHandler<TContext>? ResultHandler { get; set; }

    internal void Apply(IOptions<CommandServiceOptions> options)
    {
        var value = options.Value;

        var parameterSeparators = value.ParameterSeparators;
        if (parameterSeparators is not null)
            ParameterSeparators = parameterSeparators;

        var ignoreCase = value.IgnoreCase;
        if (ignoreCase.HasValue)
            IgnoreCase = ignoreCase;

        var cultureInfo = value.CultureInfo;
        if (cultureInfo is not null)
            CultureInfo = cultureInfo;

        var useScopes = value.UseScopes;
        if (useScopes.HasValue)
            UseScopes = useScopes;

        var prefix = value.Prefix;
        if (prefix is not null)
            Prefix = prefix;

        var prefixes = value.Prefixes;
        if (prefixes is not null)
            Prefixes = prefixes;

        var getPrefixLengthAsync = value.GetPrefixLengthAsync;
        if (getPrefixLengthAsync is not null)
            GetPrefixLengthAsync = getPrefixLengthAsync;
    }

    internal CommandServiceConfiguration<TContext> CreateConfiguration()
    {
        var configuration = CommandServiceConfiguration<TContext>.Default;

        return configuration with
        {
            TypeReaders = TypeReaders.ToImmutableDictionary(),
            EnumTypeReader = EnumTypeReader ?? configuration.EnumTypeReader,
            ParameterSeparators = ParameterSeparators ?? configuration.ParameterSeparators,
            IgnoreCase = IgnoreCase ?? configuration.IgnoreCase,
            CultureInfo = CultureInfo ?? configuration.CultureInfo,
            ResultResolverProvider = ResultResolverProvider ?? configuration.ResultResolverProvider,
        };
    }
}
