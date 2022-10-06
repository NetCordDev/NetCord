using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonAutoModerationAction
{
    [JsonPropertyName("type")]
    public AutoModerationActionType Type { get; set; }

    [JsonPropertyName("metadata")]
    public JsonAutoModerationActionMetadata? Metadata { get; set; }

    [JsonSerializable(typeof(JsonAutoModerationAction))]
    public partial class JsonAutoModerationActionSerializerContext : JsonSerializerContext
    {
        public static JsonAutoModerationActionSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
