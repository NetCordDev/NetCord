using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAuditLogEntryInfo
{
    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("delete_member_days")]
    public int? DeleteGuildUserDays { get; init; }

    [JsonPropertyName("id")]
    public Snowflake? Id { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("members_removed")]
    public int? GuildUsersRemoved { get; init; }

    [JsonPropertyName("message_id")]
    public Snowflake? MessageId { get; init; }

    [JsonPropertyName("role_name")]
    public string? RoleName { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("type")]
    public int? Type { get; init; } //PermissionOverwriteType
}
