using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public record JsonAuditLogEntry : JsonEntity
{
    [JsonPropertyName("target_id")]
    public Snowflake? TargetId { get; init; }

    [JsonPropertyName("changes")]
    public JsonAuditLogChange[]? Changes { get; init; }

    [JsonPropertyName("user_id")]
    public Snowflake? UserId { get; init; }

    [JsonPropertyName("action_type")]
    public AuditLogEvent? ActionType { get; init; } //https://github.com/discord/discord-api-docs/issues/5055

    [JsonPropertyName("options")]
    public JsonAuditLogEntryInfo? Options { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}
