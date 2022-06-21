using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public record JsonAutoModerationRule : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("creator_id")]
    public Snowflake CreatorId { get; init; }

    [JsonPropertyName("event_type")]
    public AutoModerationRuleEventType EventType { get; init; }

    [JsonPropertyName("trigger_type")]
    public AutoModerationRuleTriggerType TriggerType { get; init; }

    [JsonPropertyName("trigger_metadata")]
    public JsonAutoModerationRuleTriggerMetadata TriggerMetadata { get; init; }

    [JsonPropertyName("actions")]
    public JsonAutoModerationAction[] Actions { get; init; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    [JsonPropertyName("exempt_roles")]
    public Snowflake[] ExemptRoles { get; init; }

    [JsonPropertyName("exempt_channels")]
    public Snowflake[] ExemptChannels { get; init; }
}