using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class AutoModerationRuleProperties
{
    public AutoModerationRuleProperties(string name, AutoModerationRuleEventType eventType, AutoModerationRuleTriggerType triggerType, IEnumerable<AutoModerationActionProperties> actions)
    {
        Name = name;
        EventType = eventType;
        TriggerType = triggerType;
        Actions = actions;
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("event_type")]
    public AutoModerationRuleEventType EventType { get; }

    [JsonPropertyName("trigger_type")]
    public AutoModerationRuleTriggerType TriggerType { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("trigger_metadata")]
    public AutoModerationRuleTriggerMetadataProperties? TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public IEnumerable<AutoModerationActionProperties> Actions { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exempt_roles")]
    public IEnumerable<Snowflake>? ExemptRoles { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exempt_channels")]
    public IEnumerable<Snowflake>? ExemptChannels { get; set; }
}