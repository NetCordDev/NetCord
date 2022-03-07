using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonMessageDeleteEventArgs
{
    [JsonPropertyName("id")]
    public DiscordId MessageId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }
}