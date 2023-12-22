using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class EmbedFieldProperties
{
    /// <summary>
    /// Name of the field.
    /// </summary>
    [JsonConverter(typeof(EmptyWhenNullStringConverter))]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Value of the field.
    /// </summary>
    [JsonConverter(typeof(EmptyWhenNullStringConverter))]
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// Whether or not the field should display inline.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("inline")]
    public bool Inline { get; set; }

    public partial class EmptyWhenNullStringConverter : JsonConverter<string>
    {
        public override bool HandleNull => true;

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetString();

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value ?? string.Empty);
        }
    }
}
