using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAutoModerationRule : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("creator_id")]
    public ulong CreatorId { get; set; }

    [JsonPropertyName("event_type")]
    public AutoModerationRuleEventType EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public AutoModerationRuleTriggerType TriggerType { get; set; }

    [JsonPropertyName("trigger_metadata")]
    public JsonAutoModerationRuleTriggerMetadata TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public JsonAutoModerationAction[] Actions { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public ulong[] ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public ulong[] ExemptChannels { get; set; }
}
