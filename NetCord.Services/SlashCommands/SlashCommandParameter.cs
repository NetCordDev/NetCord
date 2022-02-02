using System.Reflection;

using NetCord.Services.Utils;

namespace NetCord.Services.SlashCommands;

public class SlashCommandParameter<TContext> where TContext : BaseSlashCommandContext
{
    public SlashCommandTypeReader<TContext> TypeReader { get; }
    public Type Type { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public string Name { get; }
    public string Description { get; }
    public bool Autocomplete { get; }

    internal SlashCommandParameter(ParameterInfo parameter, SlashCommandServiceOptions<TContext> options, out Autocomplete? autocomplete)
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
        if (Attributes.TryGetValue(typeof(AutocompleteAttribute), out attributes))
        {
            var autocompleteAttribute = (AutocompleteAttribute)attributes[0];
            autocomplete = ((IAutocompleteProvider)Activator.CreateInstance(autocompleteAttribute.AutocompleteProviderType)!).GetChoicesAsync;
            Autocomplete = true;
        }
        else
            autocomplete = null;

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
    }

    public ApplicationCommandOptionProperties GetRawValue()
    {
        const double discordMaxValue = 9007199254740991;
        const double discordMinValue = -discordMaxValue;

        var maxValue = TypeReader.GetMaxValue(this);
        if (maxValue.HasValue)
            maxValue = Math.Min(maxValue.GetValueOrDefault(), discordMaxValue);

        var minValue = TypeReader.GetMinValue(this);
        if (minValue.HasValue)
            minValue = Math.Max(minValue.GetValueOrDefault(), discordMinValue);

        return new(TypeReader.Type, Name, Description)
        {
            MaxValue = maxValue,
            MinValue = minValue,
            Required = !HasDefaultValue,
            Autocomplete = Autocomplete,
            Choices = Autocomplete ? null : TypeReader.GetChoices(this)?.ToList(),
        };
    }
}