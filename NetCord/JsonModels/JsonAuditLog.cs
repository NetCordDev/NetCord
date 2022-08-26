using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAuditLog
{
    [JsonPropertyName("audit_log_entries")]
    public JsonAuditLogEntry[] AuditLogEntries { get; init; }

    [JsonPropertyName("guild_scheduled_events")]
    public JsonGuildScheduledEvent[] GuildScheduledEvents { get; init; }

    [JsonPropertyName("integrations")]
    public JsonIntegration[] Integrations { get; init; }

    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; init; }

    [JsonPropertyName("users")]
    public JsonUser[] Users { get; init; }

    [JsonPropertyName("webhooks")]
    public JsonWebhook[] Webhooks { get; init; }
}
