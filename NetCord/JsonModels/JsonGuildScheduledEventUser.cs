using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildScheduledEventUser
{
    [JsonPropertyName("")]
    public Snowflake ScheduledEventId { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; init; }
}