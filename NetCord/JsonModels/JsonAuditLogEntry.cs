using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonAuditLogEntry : JsonEntity
{
    [JsonPropertyName("target_id")]
    public ulong? TargetId { get; set; }

    [JsonPropertyName("changes")]
    public JsonAuditLogChange[]? Changes { get; set; }

    [JsonPropertyName("user_id")]
    public ulong? UserId { get; set; }

    [JsonPropertyName("action_type")]
    public AuditLogEvent? ActionType { get; set; } //https://github.com/discord/discord-api-docs/issues/5055

    [JsonPropertyName("options")]
    public JsonAuditLogEntryInfo? Options { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonSerializable(typeof(JsonAuditLogEntry))]
    public partial class JsonAuditLogEntrySerializerContext : JsonSerializerContext
    {
        public static JsonAuditLogEntrySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
