using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonTypingStartEventArgs
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonConverter(typeof(JsonConverters.SecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("member")]
    public JsonGuildUser? User { get; init; }
}
