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
    public IEnumerable<ChannelType>? AllowedChannelTypes { get; }

    internal SlashCommandParameter(ParameterInfo parameter, ApplicationCommandServiceOptions<TContext> options)
    {
        HasDefaultValue = parameter.HasDefaultValue;
        Attributes = parameter.GetCustomAttributes().ToRankedDictionary(a => a.GetType());

        var type = parameter.ParameterType;
        var underlyingType = Nullable.GetUnderlyingType(type);

        var typeReaders = options.TypeReaders;

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
                TypeReader = options.EnumTypeReader;
            }
            else
                throw new TypeReaderNotFoundException("Type name: " + underlyingType.FullName + " or " + type.FullName);
            Type = underlyingType;
        }
        else
        {
            if (HasDefaultValue)
                DefaultValue = parameter.DefaultValue;

            if (typeReaders.TryGetValue(type, out var typeReader))
                TypeReader = typeReader;
            else if (type.IsEnum)
                TypeReader = options.EnumTypeReader;
            else
                throw new TypeReaderNotFoundException("Type name: " + type.FullName);
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
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.NameTranslationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'");
                NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandParameterAttribute.NameTranslationsProviderType)!;
            }
            else
                NameTranslationsProvider = TypeReader.NameTranslationsProvider;

            if (slashCommandParameterAttribute.DescriptionTranslationsProviderType != null)
            {
                if (!slashCommandParameterAttribute.DescriptionTranslationsProviderType.IsAssignableTo(typeof(ITranslationsProvider)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.DescriptionTranslationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'");
                DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandParameterAttribute.DescriptionTranslationsProviderType)!;
            }
            else
                DescriptionTranslationsProvider = TypeReader.DescriptionTranslationsProvider;

            if (slashCommandParameterAttribute.ChoicesProviderType != null)
            {
                if (!slashCommandParameterAttribute.ChoicesProviderType.IsAssignableTo(typeof(IChoicesProvider<TContext>)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.ChoicesProviderType}' is not assignable to '{nameof(IChoicesProvider<TContext>)}<{typeof(TContext).Name}>'");
                ChoicesProvider = (IChoicesProvider<TContext>)Activator.CreateInstance(slashCommandParameterAttribute.ChoicesProviderType)!;
            }
            else
                ChoicesProvider = TypeReader.ChoicesProvider;

            if (slashCommandParameterAttribute.AutocompleteProviderType != null)
            {
                if (!slashCommandParameterAttribute.AutocompleteProviderType.IsAssignableTo(typeof(IAutocompleteProvider)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.AutocompleteProviderType}' is not assignable to '{nameof(IAutocompleteProvider)}'");
                AutocompleteProvider = (IAutocompleteProvider)Activator.CreateInstance(slashCommandParameterAttribute.AutocompleteProviderType)!;
            }
            else
                AutocompleteProvider = TypeReader.AutocompleteProvider;

            AllowedChannelTypes = slashCommandParameterAttribute.AllowedChannelTypes ?? TypeReader.AllowedChannelTypes;
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
    }

    public ApplicationCommandOptionProperties GetRawValue()
    {
        double? maxValue;
        if (Attributes.TryGetValue(typeof(MaxValueAttribute), out var attributes))
            maxValue = ((MaxValueAttribute)attributes[0]).MaxValue;
        else
            maxValue = TypeReader.GetMaxValue(this);

        double? minValue;
        if (Attributes.TryGetValue(typeof(MinValueAttribute), out attributes))
            minValue = ((MinValueAttribute)attributes[0]).MinValue;
        else
            minValue = TypeReader.GetMinValue(this);

        int? maxLength;
        if (Attributes.TryGetValue(typeof(MaxLengthAttribute), out attributes))
            maxLength = ((MaxLengthAttribute)attributes[0]).MaxLength;
        else
            maxLength = TypeReader.GetMaxLength(this);

        int? minLength;
        if (Attributes.TryGetValue(typeof(MinLengthAttribute), out attributes))
            minLength = ((MinLengthAttribute)attributes[0]).MinLength;
        else
            minLength = TypeReader.GetMinLength(this);

        var autocomplete = AutocompleteProvider != null;
        return new(TypeReader.Type, Name, Description)
        {
            NameLocalizations = NameTranslationsProvider?.Translations,
            DescriptionLocalizations = DescriptionTranslationsProvider?.Translations,
            MaxValue = maxValue,
            MinValue = minValue,
            MaxLength = maxLength,
            MinLength = minLength,
            Required = !HasDefaultValue,
            Autocomplete = autocomplete,
            Choices = ChoicesProvider?.GetChoices(this),
            ChannelTypes = AllowedChannelTypes,
        };
    }
}