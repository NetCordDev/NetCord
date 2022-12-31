using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Utils;

namespace NetCord.Services.ApplicationCommands;

public class SlashCommandParameter<TContext> where TContext : IApplicationCommandContext
{
    public SlashCommandTypeReader<TContext> TypeReader { get; }
    public Type Type { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public IAutocompleteProvider? AutocompleteProvider { get; }
    public IChoicesProvider<TContext>? ChoicesProvider { get; }
    public double? MaxValue { get; }
    public double? MinValue { get; }
    public int? MaxLength { get; }
    public int? MinLength { get; }
    public IEnumerable<ChannelType>? AllowedChannelTypes { get; }
    public IReadOnlyList<ParameterPreconditionAttribute<TContext>> Preconditions { get; }

    internal SlashCommandParameter(ParameterInfo parameter, MethodInfo method, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        HasDefaultValue = parameter.HasDefaultValue;
        var attributesIEnumerable = parameter.GetCustomAttributes();
        Attributes = attributesIEnumerable.ToRankedDictionary(a => a.GetType());

        var type = parameter.ParameterType;
        var underlyingType = Nullable.GetUnderlyingType(type);

        var typeReaders = configuration.TypeReaders;

        if (Attributes.TryGetValue(typeof(TypeReaderAttribute), out var attributes))
        {
            if (underlyingType != null)
            {
                if (HasDefaultValue)
                {
                    var d = parameter.DefaultValue;
                    if (underlyingType.IsEnum && d != null)
                        DefaultValue = Enum.ToObject(underlyingType, d);
                    else
                        DefaultValue = d;
                }
                Type = underlyingType;
            }
            else
            {
                if (HasDefaultValue)
                    DefaultValue = parameter.DefaultValue;
                Type = type;
            }

            TypeReader = TypeReaderAttributeHelper.GetTypeReader<TContext, ISlashCommandTypeReader, SlashCommandTypeReader<TContext>>((TypeReaderAttribute)attributes[0]);
        }
        else if (underlyingType != null)
        {
            if (typeReaders.TryGetValue(type, out var typeReader) || typeReaders.TryGetValue(underlyingType, out typeReader))
            {
                if (HasDefaultValue)
                {
                    var d = parameter.DefaultValue;
                    if (underlyingType.IsEnum && d != null)
                        DefaultValue = Enum.ToObject(underlyingType, d);
                    else
                        DefaultValue = d;
                }
                TypeReader = typeReader;
            }
            else if (underlyingType.IsEnum)
            {
                if (HasDefaultValue)
                {
                    var d = parameter.DefaultValue;
                    if (d != null)
                        DefaultValue = Enum.ToObject(underlyingType, d);
                    else
                        DefaultValue = d;
                }
                TypeReader = configuration.EnumTypeReader;
            }
            else
                throw new TypeReaderNotFoundException($"Type name: '{underlyingType.FullName}' or '{type.FullName}'.");
            Type = underlyingType;
        }
        else
        {
            if (HasDefaultValue)
                DefaultValue = parameter.DefaultValue;

            if (typeReaders.TryGetValue(type, out var typeReader))
                TypeReader = typeReader;
            else if (type.IsEnum)
                TypeReader = configuration.EnumTypeReader;
            else
                throw new TypeReaderNotFoundException($"Type name: '{type.FullName}'.");
            Type = type;
        }

        if (Attributes.TryGetValue(typeof(SlashCommandParameterAttribute), out attributes))
        {
            var slashCommandParameterAttribute = (SlashCommandParameterAttribute)attributes[0];
            Name = slashCommandParameterAttribute.Name ?? parameter.Name!;
            Description = slashCommandParameterAttribute.Description ?? $"Parameter of name {Name}";

            if (slashCommandParameterAttribute.NameTranslationsProviderType != null)
            {
                if (!slashCommandParameterAttribute.NameTranslationsProviderType.IsAssignableTo(typeof(ITranslationsProvider)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.NameTranslationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'.");
                NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandParameterAttribute.NameTranslationsProviderType)!;
            }
            else
                NameTranslationsProvider = TypeReader.NameTranslationsProvider;

            if (slashCommandParameterAttribute.DescriptionTranslationsProviderType != null)
            {
                if (!slashCommandParameterAttribute.DescriptionTranslationsProviderType.IsAssignableTo(typeof(ITranslationsProvider)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.DescriptionTranslationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'.");
                DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandParameterAttribute.DescriptionTranslationsProviderType)!;
            }
            else
                DescriptionTranslationsProvider = TypeReader.DescriptionTranslationsProvider;

            if (slashCommandParameterAttribute.ChoicesProviderType != null)
            {
                if (!slashCommandParameterAttribute.ChoicesProviderType.IsAssignableTo(typeof(IChoicesProvider<TContext>)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.ChoicesProviderType}' is not assignable to '{nameof(IChoicesProvider<TContext>)}<{typeof(TContext).Name}>'.");
                ChoicesProvider = (IChoicesProvider<TContext>)Activator.CreateInstance(slashCommandParameterAttribute.ChoicesProviderType)!;
            }
            else
                ChoicesProvider = TypeReader.ChoicesProvider;

            if (slashCommandParameterAttribute.AutocompleteProviderType != null)
            {
                if (!slashCommandParameterAttribute.AutocompleteProviderType.IsAssignableTo(typeof(IAutocompleteProvider)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.AutocompleteProviderType}' is not assignable to '{nameof(IAutocompleteProvider)}'.");
                AutocompleteProvider = (IAutocompleteProvider)Activator.CreateInstance(slashCommandParameterAttribute.AutocompleteProviderType)!;
            }
            else
                AutocompleteProvider = TypeReader.AutocompleteProvider;

            AllowedChannelTypes = slashCommandParameterAttribute.AllowedChannelTypes ?? TypeReader.AllowedChannelTypes;
            MaxValue = slashCommandParameterAttribute._maxValue ?? TypeReader.GetMaxValue(this);
            MinValue = slashCommandParameterAttribute._minValue ?? TypeReader.GetMinValue(this);
            MaxLength = slashCommandParameterAttribute._maxLength ?? TypeReader.GetMaxLength(this);
            MinLength = slashCommandParameterAttribute._minLength ?? TypeReader.GetMinLength(this);
        }
        else
        {
            Name = parameter.Name!;
            NameTranslationsProvider = TypeReader.NameTranslationsProvider;
            Description = $"Parameter of name {Name}";
            DescriptionTranslationsProvider = TypeReader.DescriptionTranslationsProvider;
            ChoicesProvider = TypeReader.ChoicesProvider;
            AutocompleteProvider = TypeReader.AutocompleteProvider;
            AllowedChannelTypes = TypeReader.AllowedChannelTypes;
        }

        Preconditions = ParameterPreconditionAttributeHelper.GetPreconditionAttributes<TContext>(attributesIEnumerable, method);
    }

    public ApplicationCommandOptionProperties GetRawValue()
    {
        var autocomplete = AutocompleteProvider != null;
        return new(TypeReader.Type, Name, Description)
        {
            NameLocalizations = NameTranslationsProvider?.Translations,
            DescriptionLocalizations = DescriptionTranslationsProvider?.Translations,
            MaxValue = MaxValue,
            MinValue = MinValue,
            MaxLength = MaxLength,
            MinLength = MinLength,
            Required = !HasDefaultValue,
            Autocomplete = autocomplete,
            Choices = ChoicesProvider?.GetChoices(this),
            ChannelTypes = AllowedChannelTypes,
        };
    }

    internal async Task EnsureCanExecuteAsync(object? value, TContext context)
    {
        foreach (var preconditionAttribute in Preconditions)
            await preconditionAttribute.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
    }
}
