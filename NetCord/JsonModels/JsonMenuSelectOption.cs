using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMenuSelectOption
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("default")]
    public bool? Default { get; set; }

    [JsonSerializable(typeof(JsonMenuSelectOption))]
    public partial class JsonMenuSelectOptionSerializerContext : JsonSerializerContext
    {
        public static JsonMenuSelectOptionSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
