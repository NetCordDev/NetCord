using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonMessageReactionRemoveEventArgs
{
    [JsonPropertyName("user_id")]
    public DiscordId UserId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("message_id")]
    public DiscordId MessageId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; init; }
}