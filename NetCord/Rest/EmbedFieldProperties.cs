using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="EmbedField"/>
[GenerateMethodsForProperties]
public partial class EmbedFieldProperties
{
    /// <inheritdoc cref="EmbedField.Name"/>
    [JsonConverter(typeof(EmptyWhenNullStringConverter))]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <inheritdoc cref="EmbedField.Value"/>
    [JsonConverter(typeof(EmptyWhenNullStringConverter))]
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// When set alongside another field with <see cref="Inline"/> set, displays the fields side by side when supported.
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
