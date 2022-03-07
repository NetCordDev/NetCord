using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonWebhooksUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }
}