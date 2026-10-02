using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonAutoModerationActionExecutionEventArgs
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("action")]
    public required JsonAutoModerationAction Action { get; set; }

    [JsonPropertyName("rule_id")]
    public required ulong RuleId { get; set; }

    [JsonPropertyName("rule_trigger_type")]
    public required AutoModerationRuleTriggerType RuleTriggerType { get; set; }

    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong? MessageId { get; set; }

    [JsonPropertyName("alert_system_message_id")]
    public ulong? AlertSystemMessageId { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("matched_keyword")]
    public string? MatchedKeyword { get; set; }

    [JsonPropertyName("matched_content")]
    public string? MatchedContent { get; set; }
}
