using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAuditLogEntryInfo
{
    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("auto_moderation_rule_name")]
    public string? AutoModerationRuleName { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("auto_moderation_rule_trigger_type")]
    public int? AutoModerationRuleTriggerType { get; set; } //AutoModerationRuleTriggerType

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("delete_member_days")]
    public int? DeleteGuildUserDays { get; set; }

    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("members_removed")]
    public int? GuildUsersRemoved { get; set; }

    [JsonPropertyName("message_id")]
    public ulong? MessageId { get; set; }

    [JsonPropertyName("role_name")]
    public string? RoleName { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("type")]
    public int? Type { get; set; } //PermissionOverwriteType
}
