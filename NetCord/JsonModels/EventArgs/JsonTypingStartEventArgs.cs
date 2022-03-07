using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonTypingStartEventArgs
{
    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("user_id")]
    public DiscordId UserId { get; init; }

    [JsonConverter(typeof(JsonConverters.SecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("member")]
    public JsonGuildUser? User { get; init; }
}