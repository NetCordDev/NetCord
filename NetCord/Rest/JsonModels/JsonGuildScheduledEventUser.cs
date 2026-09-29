using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildScheduledEventUser
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public required ulong ScheduledEventId { get; set; }

    [JsonPropertyName("user")]
    public required JsonUser User { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }
}
