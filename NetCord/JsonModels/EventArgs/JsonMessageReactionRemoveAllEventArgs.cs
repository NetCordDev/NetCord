using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonMessageReactionRemoveAllEventArgs
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("message_id")]
    public Snowflake MessageId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }
}
