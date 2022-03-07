using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonChannelPinsUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("last_pin_timestamp")]
    public DateTimeOffset? LastPinTimestamp { get; init; }
}
