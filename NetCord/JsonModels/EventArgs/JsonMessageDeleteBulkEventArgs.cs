using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonMessageDeleteBulkEventArgs
{
    [JsonPropertyName("ids")]
    public DiscordId[] MessageIds { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }
}