using System.Text.Json.Serialization;

namespace NetCord;

public partial class AutoModerationRuleProperties
{
    public AutoModerationRuleProperties(string name, AutoModerationRuleEventType eventType, AutoModerationRuleTriggerType triggerType, IEnumerable<AutoModerationActionProperties> actions)
    {
        Name = name;
        EventType = eventType;
        TriggerType = triggerType;
        Actions = actions;
    }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("event_type")]
    public AutoModerationRuleEventType EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public AutoModerationRuleTriggerType TriggerType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("trigger_metadata")]
    public AutoModerationRuleTriggerMetadataProperties? TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public IEnumerable<AutoModerationActionProperties> Actions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exempt_roles")]
    public IEnumerable<ulong>? ExemptRoles { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exempt_channels")]
    public IEnumerable<ulong>? ExemptChannels { get; set; }
}
