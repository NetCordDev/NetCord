using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAutoModerationRule : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("creator_id")]
    public required ulong CreatorId { get; set; }

    [JsonPropertyName("event_type")]
    public required AutoModerationRuleEventType EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public required AutoModerationRuleTriggerType TriggerType { get; set; }

    [JsonPropertyName("trigger_metadata")]
    public required JsonAutoModerationRuleTriggerMetadata TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public required JsonAutoModerationAction[] Actions { get; set; }

    [JsonPropertyName("enabled")]
    public required bool Enabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public required ulong[] ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public required ulong[] ExemptChannels { get; set; }
}
