using System.Text.Json.Serialization;

namespace NetCord;

public partial class AutoModerationActionProperties
{
    public AutoModerationActionProperties(AutoModerationActionType type)
    {
        Type = type;
    }

    [JsonPropertyName("type")]
    public AutoModerationActionType Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("metadata")]
    public AutoModerationActionMetadataProperties? Metadata { get; set; }
}
