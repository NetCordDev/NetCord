using System.Text.Json.Serialization;

namespace NetCord;

public partial class AutoModerationActionProperties(AutoModerationActionType type)
{
    [JsonPropertyName("type")]
    public AutoModerationActionType Type { get; set; } = type;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("metadata")]
    public AutoModerationActionMetadataProperties? Metadata { get; set; }
}
