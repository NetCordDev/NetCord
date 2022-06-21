using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class AutoModerationActionProperties
{
    public AutoModerationActionProperties(AutoModerationActionType type)
    {
        Type = type;
    }

    [JsonPropertyName("type")]
    public AutoModerationActionType Type { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("metadata")]
    public AutoModerationActionMetadataProperties? Metadata { get; set; }
}