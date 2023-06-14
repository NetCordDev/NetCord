using System.Linq.Expressions;
using System.Reflection;

using NetCord.Rest;
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
    public Delegate? InvokeAutocomplete { get; }
    public IChoicesProvider<TContext>? ChoicesProvider { get; }
    public double? MaxValue { get; }
    public double? MinValue { get; }
    public int? MaxLength { get; }
    public int? MinLength { get; }
    public IEnumerable<ChannelType>? AllowedChannelTypes { get; }
    public IReadOnlyList<ParameterPreconditionAttribute<TContext>> Preconditions { get; }

    internal SlashCommandParameter(ParameterInfo parameter, MethodInfo method, ApplicationCommandServiceConfiguration<TContext> configuration, bool supportsAutocomplete, Type? autocompleteContextType, Type? autocompleteBaseType)
    {
        HasDefaultValue = parameter.HasDefaultValue;
        var attributesIEnumerable = parameter.GetCustomAttributes();
        Attributes = attributesIEnumerable.ToRankedDictionary(a => a.GetType());

        var type = Type = parameter.ParameterType;

        if (Attributes.TryGetValue(typeof(SlashCommandParameterAttribute), out var attributes))
        {
            var slashCommandParameterAttribute = (SlashCommandParameterAttribute)attributes[0];
            (TypeReader, NonNullableType, DefaultValue) = ParameterHelper.GetParameterInfo<TContext, ISlashCommandTypeReader, SlashCommandTypeReader<TContext>>(type, parameter, slashCommandParameterAttribute.TypeReaderType, configuration.TypeReaders, configuration.EnumTypeReader);

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

            var autocompleteProviderType = slashCommandParameterAttribute.AutocompleteProviderType;
            if (autocompleteProviderType is not null)
            {
                EnsureAutocompleteProviderValid(autocompleteProviderType);
                InvokeAutocomplete = CreateAutocompleteDelegate(autocompleteProviderType, autocompleteContextType!, autocompleteBaseType!);
            }
            else
            {
                autocompleteProviderType = TypeReader.AutocompleteProviderType;
                if (autocompleteProviderType is not null)
                {
                    EnsureAutocompleteProviderValid(autocompleteProviderType);
                    InvokeAutocomplete = CreateAutocompleteDelegate(autocompleteProviderType, autocompleteContextType!, autocompleteBaseType!);
                }
            }

            AllowedChannelTypes = slashCommandParameterAttribute.AllowedChannelTypes ?? TypeReader.AllowedChannelTypes;
            MaxValue = slashCommandParameterAttribute._maxValue ?? TypeReader.GetMaxValue(this, configuration);
            MinValue = slashCommandParameterAttribute._minValue ?? TypeReader.GetMinValue(this, configuration);
            MaxLength = slashCommandParameterAttribute._maxLength ?? TypeReader.GetMaxLength(this, configuration);
            MinLength = slashCommandParameterAttribute._minLength ?? TypeReader.GetMinLength(this, configuration);
        }
        else
        {
            (TypeReader, NonNullableType, DefaultValue) = ParameterHelper.GetParameterInfo<TContext, ISlashCommandTypeReader, SlashCommandTypeReader<TContext>>(type, parameter, null, configuration.TypeReaders, configuration.EnumTypeReader);

            var name = Name = parameter.Name!;
            NameTranslationsProvider = TypeReader.NameTranslationsProvider;
            Description = string.Format(configuration.DefaultParameterDescriptionFormat, name);
            DescriptionTranslationsProvider = TypeReader.DescriptionTranslationsProvider;
            ChoicesProvider = TypeReader.ChoicesProvider;
            var autocompleteProviderType = TypeReader.AutocompleteProviderType;
            if (autocompleteProviderType is not null)
            {
                EnsureAutocompleteProviderValid(autocompleteProviderType);
                InvokeAutocomplete = CreateAutocompleteDelegate(autocompleteProviderType, autocompleteContextType!, autocompleteBaseType!);
            }
            AllowedChannelTypes = TypeReader.AllowedChannelTypes;
            MaxValue = TypeReader.GetMaxValue(this, configuration);
            MinValue = TypeReader.GetMinValue(this, configuration);
            MaxLength = TypeReader.GetMaxLength(this, configuration);
            MinLength = TypeReader.GetMinLength(this, configuration);
        }

        Preconditions = ParameterPreconditionAttributeHelper.GetPreconditionAttributes<TContext>(attributesIEnumerable, method);

        void EnsureAutocompleteProviderValid(Type type)
        {
            if (!supportsAutocomplete)
                throw new InvalidOperationException($"Autocomplete is not supported by this service. Use {typeof(ApplicationCommandService<,>)} instead.");

            if (!type.IsAssignableTo(autocompleteBaseType))
                throw new InvalidOperationException($"'{type}' is not assignable to '{autocompleteBaseType}'.");
        }
    }

    private static Delegate CreateAutocompleteDelegate(Type autocompleteType, Type autocompleteContextType, Type autocompleteBaseType)
    {
        var option = Expression.Parameter(typeof(ApplicationCommandInteractionDataOption));
        var context = Expression.Parameter(autocompleteContextType);
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));
        var getChoicesAsyncMethod = autocompleteBaseType.GetMethod("GetChoicesAsync", BindingFlags.Instance | BindingFlags.Public)!;
        var call = Expression.Call(TypeHelper.GetCreateInstanceExpression(autocompleteType, serviceProvider),
                                   getChoicesAsyncMethod,
                                   option, context);
        var lambda = Expression.Lambda(call, option, context, serviceProvider);
        return lambda.Compile();
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
            Autocomplete = InvokeAutocomplete is not null,
            Choices = ChoicesProvider?.GetChoices(this),
            ChannelTypes = AllowedChannelTypes,
        };
    }

    internal async Task EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        var preconditions = Preconditions;
        var count = preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            var preconditionAttribute = preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
        }
    }
}
