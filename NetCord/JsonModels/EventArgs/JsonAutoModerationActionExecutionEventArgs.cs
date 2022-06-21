using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonAutoModerationActionExecutionEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("action")]
    public JsonAutoModerationAction Action { get; init; }

    [JsonPropertyName("rule_id")]
    public Snowflake RuleId { get; init; }

    [JsonPropertyName("rule_trigger_type")]
    public AutoModerationRuleTriggerType RuleTriggerType { get; init; }

    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonPropertyName("message_id")]
    public Snowflake? MessageId { get; init; }

    [JsonPropertyName("alert_system_message_id")]
    public Snowflake? AlertSystemMessageId { get; init; }

    [JsonPropertyName("content")]
    public string Content { get; init; }

    [JsonPropertyName("matched_keyword")]
    public string? MatchedKeyword { get; init; }

    [JsonPropertyName("matched_content")]
    public string? MatchedContent { get; init; }
}
