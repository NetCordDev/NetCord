using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonForumGuildChannelDefaultReaction
{
    [JsonPropertyName("emoji_id")]
    public Snowflake? EmojiId { get; set; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }
}
