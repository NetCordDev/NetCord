using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonAuditLog
{
    [JsonPropertyName("application_commands")]
    public required JsonApplicationCommand[] ApplicationCommands { get; set; }

    [JsonPropertyName("audit_log_entries")]
    public required JsonAuditLogEntry[] AuditLogEntries { get; set; }

    [JsonPropertyName("auto_moderation_rules")]
    public required JsonAutoModerationRule[] AutoModerationRules { get; set; }

    [JsonPropertyName("guild_scheduled_events")]
    public required JsonGuildScheduledEvent[] GuildScheduledEvents { get; set; }

    [JsonPropertyName("integrations")]
    public required JsonIntegration[] Integrations { get; set; }

    [JsonPropertyName("threads")]
    public required JsonChannel[] Threads { get; set; }

    [JsonPropertyName("users")]
    public required JsonUser[] Users { get; set; }

    [JsonPropertyName("webhooks")]
    public required JsonWebhook[] Webhooks { get; set; }
}
