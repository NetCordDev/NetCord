using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonMessageReactionRemoveAllEventArgs
{
    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("message_id")]
    public DiscordId MessageId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }
}