using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ApplicationCommandOptionChoicePropertiesConverter))]
public partial class ApplicationCommandOptionChoiceProperties
{
    /// <summary>
    /// Name of the choice (1-100 characters).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Translations of <see cref="Name"/> (1-100 characters each).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    /// <summary>
    /// String value for the choice (max 100 characters).
    /// </summary>
    public string? StringValue { get; set; }

    /// <summary>
    /// Numeric value for the choice (max 100 characters).
    /// </summary>
    public double? NumericValue { get; set; }

    /// <summary>
    /// Type of value.
    /// </summary>
    public ApplicationCommandOptionChoiceValueType ValueType { get; set; }

    public ApplicationCommandOptionChoiceProperties(string name, string stringValue)
    {
        Name = name;
        StringValue = stringValue;
        ValueType = ApplicationCommandOptionChoiceValueType.String;
    }

    public ApplicationCommandOptionChoiceProperties(string name, double numericValue)
    {
        Name = name;
        NumericValue = numericValue;
        ValueType = ApplicationCommandOptionChoiceValueType.Numeric;
    }

    internal partial class ApplicationCommandOptionChoicePropertiesConverter : JsonConverter<ApplicationCommandOptionChoiceProperties>
    {
        public override ApplicationCommandOptionChoiceProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, ApplicationCommandOptionChoiceProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("name", value.Name);
            if (value.NameLocalizations != null)
            {
                writer.WritePropertyName("name_localizations");
                JsonSerializer.Serialize(writer, value.NameLocalizations, IReadOnlyDictionaryOfCultureInfoStringSerializerContext.WithOptions.IReadOnlyDictionaryCultureInfoString);
            }
            writer.WritePropertyName("value");
            if (value.ValueType == ApplicationCommandOptionChoiceValueType.String)
                writer.WriteStringValue(value.StringValue);
            else
                writer.WriteNumberValue(value.NumericValue.GetValueOrDefault());
            writer.WriteEndObject();
        }

        [JsonSerializable(typeof(IReadOnlyDictionary<CultureInfo, string>))]
        public partial class IReadOnlyDictionaryOfCultureInfoStringSerializerContext : JsonSerializerContext
        {
            public static IReadOnlyDictionaryOfCultureInfoStringSerializerContext WithOptions { get; } = new(Serialization.Options);
        }
    }

    [JsonSerializable(typeof(ApplicationCommandOptionChoiceProperties))]
    public partial class ApplicationCommandOptionChoicePropertiesSerializerContext : JsonSerializerContext
    {
        public static ApplicationCommandOptionChoicePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
