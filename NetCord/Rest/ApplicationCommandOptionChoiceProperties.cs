using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ApplicationCommandOptionChoicePropertiesConverter))]
public partial class ApplicationCommandOptionChoiceProperties
{
    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    public string? ValueString { get; }

    public double? ValueNumeric { get; }

    public ApplicationCommandOptionChoiceValueType ValueType { get; }

    public ApplicationCommandOptionChoiceProperties(string name, string stringValue)
    {
        Name = name;
        ValueString = stringValue;
        ValueType = ApplicationCommandOptionChoiceValueType.String;
    }

    public ApplicationCommandOptionChoiceProperties(string name, double valueNumeric)
    {
        Name = name;
        ValueNumeric = valueNumeric;
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
                writer.WriteStringValue(value.ValueString);
            else
                writer.WriteNumberValue(value.ValueNumeric.GetValueOrDefault());
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
