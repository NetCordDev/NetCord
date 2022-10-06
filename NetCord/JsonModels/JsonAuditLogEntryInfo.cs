using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAuditLogEntryInfo
{
    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("delete_member_days")]
    public int? DeleteGuildUserDays { get; set; }

    [JsonPropertyName("id")]
    public Snowflake? Id { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("members_removed")]
    public int? GuildUsersRemoved { get; set; }

    [JsonPropertyName("message_id")]
    public Snowflake? MessageId { get; set; }

    [JsonPropertyName("role_name")]
    public string? RoleName { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("type")]
    public int? Type { get; set; } //PermissionOverwriteType
}
