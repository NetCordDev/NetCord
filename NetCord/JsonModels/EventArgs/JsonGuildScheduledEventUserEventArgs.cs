using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildScheduledEventUserEventArgs
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public Snowflake GuildScheduledEventId { get; init; }

    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }
}