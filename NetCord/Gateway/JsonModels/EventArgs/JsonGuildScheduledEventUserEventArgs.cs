using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildScheduledEventUserEventArgs
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong GuildScheduledEventId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
