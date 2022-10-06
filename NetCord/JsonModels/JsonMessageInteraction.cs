using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMessageInteraction : JsonEntity
{
    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonSerializable(typeof(JsonMessageInteraction))]
    public partial class JsonMessageInteractionSerializerContext : JsonSerializerContext
    {
        public static JsonMessageInteractionSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
