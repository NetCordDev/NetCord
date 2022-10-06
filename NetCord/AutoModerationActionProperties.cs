using System.Text.Json.Serialization;

namespace NetCord;

public partial class AutoModerationActionProperties
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

    [JsonSerializable(typeof(AutoModerationActionProperties))]
    public partial class AutoModerationActionPropertiesSerializerContext : JsonSerializerContext
    {
        public static AutoModerationActionPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
