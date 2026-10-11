using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAuditLogEntry : JsonEntity
{
    [JsonPropertyName("target_id")]
    public ulong? TargetId { get; set; }

    [JsonPropertyName("changes")]
    public JsonAuditLogChange[]? Changes { get; set; }

    [JsonPropertyName("user_id")]
    public ulong? UserId { get; set; }

    [JsonPropertyName("action_type")]
    public AuditLogEvent ActionType { get; set; }

    [JsonPropertyName("options")]
    public JsonAuditLogEntryInfo? Options { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
