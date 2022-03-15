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
    public string Description { get; }
    public IAutocompleteProvider? AutocompleteProvider { get; }
    public IEnumerable<ChannelType>? AllowedChannelTypes { get; }

    internal SlashCommandParameter(ParameterInfo parameter, ApplicationCommandServiceOptions<TContext> options)
    {
        HasDefaultValue = parameter.HasDefaultValue;
        Attributes = parameter.GetCustomAttributes().ToRankedDictionary(a => a.GetType());
        if (Attributes.TryGetValue(typeof(SlashCommandParameterAttribute), out var attributes))
        {
            var slashCommandParameterAttribute = (SlashCommandParameterAttribute)attributes[0];
            Name = slashCommandParameterAttribute.Name ?? parameter.Name!;
            Description = slashCommandParameterAttribute.Description;
        }
        else
        {
            Name = parameter.Name!;
            Description = $"Parameter of name {Name}";
        }

        var type = parameter.ParameterType;
        var underlyingType = Nullable.GetUnderlyingType(type);

        var typeReaders = options.TypeReaders;

        if (Attributes.TryGetValue(typeof(TypeReaderAttribute), out attributes))
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

        if (Attributes.TryGetValue(typeof(AutocompleteAttribute), out attributes))
        {
            var autocompleteAttribute = (AutocompleteAttribute)attributes[0];
            AutocompleteProvider = (IAutocompleteProvider)Activator.CreateInstance(autocompleteAttribute.AutocompleteProviderType)!;
        }
        else
        {
            AutocompleteProvider = TypeReader.GetAutocompleteProvider(this);
        }

        var allowedChannelTypes = TypeReader.GetAllowedChannelTypes(this);
        if (allowedChannelTypes != null)
        {
            AllowedChannelTypes = allowedChannelTypes;
        }
        else
        {
            if (Attributes.TryGetValue(typeof(AllowedChannelTypesAttribute), out attributes))
            {
                AllowedChannelTypes = ((AllowedChannelTypesAttribute)attributes[0]).ChannelTypes;
            }
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
            MaxValue = maxValue,
            MinValue = minValue,
            Required = !HasDefaultValue,
            Autocomplete = autocomplete,
            Choices = autocomplete ? null : TypeReader.GetChoices(this),
            ChannelTypes = AllowedChannelTypes,
        };
    }
}