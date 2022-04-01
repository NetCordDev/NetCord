using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonMessageReactionRemoveEmojiEventArgs
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("message_id")]
    public Snowflake MessageId { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; init; }
}