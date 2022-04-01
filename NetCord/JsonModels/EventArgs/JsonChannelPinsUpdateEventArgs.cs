using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonChannelPinsUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("last_pin_timestamp")]
    public DateTimeOffset? LastPinTimestamp { get; init; }
}
