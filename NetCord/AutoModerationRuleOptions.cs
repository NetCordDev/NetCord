using System.Text.Json.Serialization;

namespace NetCord;

public partial class AutoModerationRuleOptions
{
    internal AutoModerationRuleOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("event_type")]
    public AutoModerationRuleEventType? EventType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("trigger_metadata")]
    public AutoModerationRuleTriggerMetadataProperties? TriggerMetadata { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("actions")]
    public IEnumerable<AutoModerationActionProperties>? Actions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exempt_roles")]
    public IEnumerable<ulong>? ExemptRoles { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exempt_channels")]
    public IEnumerable<ulong>? ExemptChannels { get; set; }

    [JsonSerializable(typeof(AutoModerationRuleOptions))]
    public partial class AutoModerationRuleOptionsSerializerContext : JsonSerializerContext
    {
        public static AutoModerationRuleOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
