using System.Reflection;

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
    public ITranslateProvider? NameTranslateProvider { get; }
    public string Description { get; }
    public ITranslateProvider? DescriptionTranslateProvider { get; }
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

            if (slashCommandParameterAttribute.NameTranslateProviderType != null)
            {
                if (!slashCommandParameterAttribute.NameTranslateProviderType.IsAssignableTo(typeof(ITranslateProvider)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.NameTranslateProviderType}' is not assignable to '{nameof(ITranslateProvider)}'");
                NameTranslateProvider = (ITranslateProvider)Activator.CreateInstance(slashCommandParameterAttribute.NameTranslateProviderType)!;
            }
            else
                NameTranslateProvider = TypeReader.NameTranslateProvider;

            if (slashCommandParameterAttribute.DescriptionTranslateProviderType != null)
            {
                if (!slashCommandParameterAttribute.DescriptionTranslateProviderType.IsAssignableTo(typeof(ITranslateProvider)))
                    throw new InvalidOperationException($"'{slashCommandParameterAttribute.DescriptionTranslateProviderType}' is not assignable to '{nameof(ITranslateProvider)}'");
                DescriptionTranslateProvider = (ITranslateProvider)Activator.CreateInstance(slashCommandParameterAttribute.DescriptionTranslateProviderType)!;
            }
            else
                DescriptionTranslateProvider = TypeReader.DescriptionTranslateProvider;

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
            NameTranslateProvider = TypeReader.NameTranslateProvider;
            Description = $"Parameter of name {Name}";
            DescriptionTranslateProvider = TypeReader.DescriptionTranslateProvider;
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
        if (maxValue.HasValue)
            maxValue = maxValue.GetValueOrDefault();

        double? minValue;
        if (Attributes.TryGetValue(typeof(MinValueAttribute), out attributes))
            minValue = ((MinValueAttribute)attributes[0]).MinValue;
        else
            minValue = TypeReader.GetMinValue(this);
        if (minValue.HasValue)
            minValue = minValue.GetValueOrDefault();

        var autocomplete = AutocompleteProvider != null;
        return new(TypeReader.Type, Name, Description)
        {
            NameLocalizations = NameTranslateProvider?.Translations,
            DescriptionLocalizations = DescriptionTranslateProvider?.Translations,
            MaxValue = maxValue,
            MinValue = minValue,
            Required = !HasDefaultValue,
            Autocomplete = autocomplete,
            Choices = ChoicesProvider?.GetChoices(this),
            ChannelTypes = AllowedChannelTypes,
        };
    }
}