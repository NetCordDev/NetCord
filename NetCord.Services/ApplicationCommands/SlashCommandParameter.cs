using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using NetCord.Rest;
using NetCord.Services.Helpers;
using NetCord.Services.Utils;

namespace NetCord.Services.ApplicationCommands;

public class SlashCommandParameter<TContext> where TContext : IApplicationCommandContext
{
    public SlashCommandTypeReader<TContext> TypeReader { get; }
    public Type Type { get; }
    public Type NonNullableType { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public string Name { get; }
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public string Description { get; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type? AutocompleteProviderType { get; }
    public IChoicesProvider<TContext>? ChoicesProvider { get; }
    public double? MaxValue { get; }
    public double? MinValue { get; }
    public int? MaxLength { get; }
    public int? MinLength { get; }
    public IEnumerable<ChannelType>? AllowedChannelTypes { get; }
    public IReadOnlyList<ParameterPreconditionAttribute<TContext>> Preconditions { get; }

    private Delegate? _invokeAutocompleteAsync;

    internal SlashCommandParameter(ParameterInfo parameter, MethodInfo method, ApplicationCommandServiceConfiguration<TContext> configuration, ImmutableList<LocalizationPathSegment> path)
    {
        HasDefaultValue = parameter.HasDefaultValue;
        var attributesIEnumerable = parameter.GetCustomAttributes();
        Attributes = attributesIEnumerable.ToRankedFrozenDictionary(a => a.GetType());
        LocalizationsProvider = configuration.LocalizationsProvider;

        var type = Type = parameter.ParameterType;

        if (Attributes.TryGetValue(typeof(SlashCommandParameterAttribute), out var attributes))
        {
            var slashCommandParameterAttribute = (SlashCommandParameterAttribute)attributes[0];
            (TypeReader, NonNullableType, DefaultValue) = ParametersHelper.GetParameterInfo<TContext, ISlashCommandTypeReader, SlashCommandTypeReader<TContext>>(type, parameter, slashCommandParameterAttribute.TypeReaderType, configuration.TypeReaders, configuration.EnumTypeReader);

            var name = Name = slashCommandParameterAttribute.Name ?? configuration.ParameterNameProcessor.ProcessParameterName(parameter.Name!, configuration);
            Description = slashCommandParameterAttribute.Description ?? string.Format(configuration.DefaultParameterDescriptionFormat, name);

            LocalizationPath = path.Add(new SlashCommandParameterLocalizationPathSegment(name));

            var choicesProviderType = slashCommandParameterAttribute.ChoicesProviderType;
            if (choicesProviderType is null)
                ChoicesProvider = TypeReader.ChoicesProvider;
            else
            {
                if (!choicesProviderType.IsAssignableTo(typeof(IChoicesProvider<TContext>)))
                    throw new InvalidOperationException($"'{choicesProviderType}' is not assignable to '{nameof(IChoicesProvider<TContext>)}<{typeof(TContext).Name}>'.");
                ChoicesProvider = (IChoicesProvider<TContext>)Activator.CreateInstance(choicesProviderType)!;
            }

            AutocompleteProviderType = slashCommandParameterAttribute.AutocompleteProviderType ?? TypeReader.AutocompleteProviderType;
            AllowedChannelTypes = slashCommandParameterAttribute.AllowedChannelTypes ?? TypeReader.AllowedChannelTypes;
            MaxValue = slashCommandParameterAttribute._maxValue ?? TypeReader.GetMaxValue(this, configuration);
            MinValue = slashCommandParameterAttribute._minValue ?? TypeReader.GetMinValue(this, configuration);
            MaxLength = slashCommandParameterAttribute._maxLength ?? TypeReader.GetMaxLength(this, configuration);
            MinLength = slashCommandParameterAttribute._minLength ?? TypeReader.GetMinLength(this, configuration);
        }
        else
        {
            (TypeReader, NonNullableType, DefaultValue) = ParametersHelper.GetParameterInfo<TContext, ISlashCommandTypeReader, SlashCommandTypeReader<TContext>>(type, parameter, null, configuration.TypeReaders, configuration.EnumTypeReader);

            var name = Name = configuration.ParameterNameProcessor.ProcessParameterName(parameter.Name!, configuration);
            LocalizationPath = path.Add(new SlashCommandParameterLocalizationPathSegment(name));
            Description = string.Format(configuration.DefaultParameterDescriptionFormat, name);
            ChoicesProvider = TypeReader.ChoicesProvider;
            AutocompleteProviderType = TypeReader.AutocompleteProviderType;
            AllowedChannelTypes = TypeReader.AllowedChannelTypes;
            MaxValue = TypeReader.GetMaxValue(this, configuration);
            MinValue = TypeReader.GetMinValue(this, configuration);
            MaxLength = TypeReader.GetMaxLength(this, configuration);
            MinLength = TypeReader.GetMinLength(this, configuration);
        }

        Preconditions = PreconditionsHelper.GetParameterPreconditions<TContext>(attributesIEnumerable, method);
    }

    public async ValueTask<ApplicationCommandOptionProperties> GetRawValueAsync()
    {
        return new(TypeReader.Type, Name, Description)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance)).ConfigureAwait(false),
            DescriptionLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(DescriptionLocalizationPathSegment.Instance)).ConfigureAwait(false),
            MaxValue = MaxValue,
            MinValue = MinValue,
            MaxLength = MaxLength,
            MinLength = MinLength,
            Required = !HasDefaultValue,
            Autocomplete = _invokeAutocompleteAsync is not null,
            Choices = ChoicesProvider is null ? null : await ChoicesProvider.GetChoicesAsync(this).ConfigureAwait(false),
            ChannelTypes = AllowedChannelTypes,
        };
    }

    internal ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
        => PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, value, context, serviceProvider);

    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> InvokeAutocompleteAsync<TAutocompleteContext>(TAutocompleteContext context, ApplicationCommandInteractionDataOption option, IServiceProvider? serviceProvider) where TAutocompleteContext : IAutocompleteInteractionContext
        => Unsafe.As<Func<ApplicationCommandInteractionDataOption, TAutocompleteContext, IServiceProvider?, ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?>>>(_invokeAutocompleteAsync!)(option, context, serviceProvider);

    internal void InitializeAutocomplete<TAutocompleteContext>() where TAutocompleteContext : IAutocompleteInteractionContext
    {
        var autocompleteProviderType = AutocompleteProviderType;
        if (autocompleteProviderType is null)
            return;

        var autocompleteProviderBaseType = typeof(IAutocompleteProvider<TAutocompleteContext>);
        if (!autocompleteProviderType.IsAssignableTo(autocompleteProviderBaseType))
            throw new InvalidOperationException($"'{autocompleteProviderType}' is not assignable to '{autocompleteProviderBaseType}'.");

        var option = Expression.Parameter(typeof(ApplicationCommandInteractionDataOption));
        var context = Expression.Parameter(typeof(TAutocompleteContext));
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));
        var getChoicesAsyncMethod = autocompleteProviderBaseType.GetMethod(nameof(IAutocompleteProvider<TAutocompleteContext>.GetChoicesAsync), BindingFlags.Instance | BindingFlags.Public)!;
        var call = Expression.Call(TypeHelper.GetCreateInstanceExpression(autocompleteProviderType, serviceProvider),
                                   getChoicesAsyncMethod,
                                   option, context);
        var lambda = Expression.Lambda(call, option, context, serviceProvider);
        _invokeAutocompleteAsync = lambda.Compile();
    }
}
