using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonAutoModerationActionExecutionEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("action")]
    public JsonAutoModerationAction Action { get; set; }

    [JsonPropertyName("rule_id")]
    public ulong RuleId { get; set; }

    [JsonPropertyName("rule_trigger_type")]
    public AutoModerationRuleTriggerType RuleTriggerType { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong? MessageId { get; set; }

    [JsonPropertyName("alert_system_message_id")]
    public ulong? AlertSystemMessageId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("matched_keyword")]
    public string? MatchedKeyword { get; set; }

    [JsonPropertyName("matched_content")]
    public string? MatchedContent { get; set; }

    [JsonSerializable(typeof(JsonAutoModerationActionExecutionEventArgs))]
    public partial class JsonAutoModerationActionExecutionEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonAutoModerationActionExecutionEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
