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
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }

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

    internal SlashCommandParameter(ParameterInfo parameter, MethodInfo method, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        HasDefaultValue = parameter.HasDefaultValue;
        var attributesIEnumerable = parameter.GetCustomAttributes();
        Attributes = attributesIEnumerable.ToRankedFrozenDictionary(a => a.GetType());

        var type = Type = parameter.ParameterType;

        if (Attributes.TryGetValue(typeof(SlashCommandParameterAttribute), out var attributes))
        {
            var slashCommandParameterAttribute = (SlashCommandParameterAttribute)attributes[0];
            (TypeReader, NonNullableType, DefaultValue) = ParametersHelper.GetParameterInfo<TContext, ISlashCommandTypeReader, SlashCommandTypeReader<TContext>>(type, parameter, slashCommandParameterAttribute.TypeReaderType, configuration.TypeReaders, configuration.EnumTypeReader);

            var name = Name = slashCommandParameterAttribute.Name ?? parameter.Name!;
            Description = slashCommandParameterAttribute.Description ?? string.Format(configuration.DefaultParameterDescriptionFormat, name);

            var nameTranslationsProviderType = slashCommandParameterAttribute.NameTranslationsProviderType;
            if (nameTranslationsProviderType is null)
                NameTranslationsProvider = TypeReader.NameTranslationsProvider;
            else
            {
                if (!nameTranslationsProviderType.IsAssignableTo(typeof(ITranslationsProvider)))
                    throw new InvalidOperationException($"'{nameTranslationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'.");
                NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(nameTranslationsProviderType)!;
            }

            var descriptionTranslationsProviderType = slashCommandParameterAttribute.DescriptionTranslationsProviderType;
            if (descriptionTranslationsProviderType is null)
                DescriptionTranslationsProvider = TypeReader.DescriptionTranslationsProvider;
            else
            {
                if (!descriptionTranslationsProviderType.IsAssignableTo(typeof(ITranslationsProvider)))
                    throw new InvalidOperationException($"'{descriptionTranslationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'.");
                DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(descriptionTranslationsProviderType)!;
            }

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

            var name = Name = parameter.Name!;
            NameTranslationsProvider = TypeReader.NameTranslationsProvider;
            Description = string.Format(configuration.DefaultParameterDescriptionFormat, name);
            DescriptionTranslationsProvider = TypeReader.DescriptionTranslationsProvider;
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

    public ApplicationCommandOptionProperties GetRawValue()
    {
        return new(TypeReader.Type, Name, Description)
        {
            NameLocalizations = NameTranslationsProvider?.Translations,
            DescriptionLocalizations = DescriptionTranslationsProvider?.Translations,
            MaxValue = MaxValue,
            MinValue = MinValue,
            MaxLength = MaxLength,
            MinLength = MinLength,
            Required = !HasDefaultValue,
            Autocomplete = _invokeAutocompleteAsync is not null,
            Choices = ChoicesProvider?.GetChoices(this),
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
