using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildScheduledEventUserEventArgs
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public DiscordId GuildScheduledEventId { get; init; }

    [JsonPropertyName("user_id")]
    public DiscordId UserId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }
}