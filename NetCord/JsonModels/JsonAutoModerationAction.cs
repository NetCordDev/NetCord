using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAutoModerationAction
{
    [JsonPropertyName("type")]
    public AutoModerationActionType Type { get; set; }

    [JsonPropertyName("metadata")]
    public JsonAutoModerationActionMetadata? Metadata { get; set; }
}
