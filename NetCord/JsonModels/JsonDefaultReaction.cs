using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonDefaultReaction
{
    [JsonPropertyName("emoji_id")]
    public Snowflake? EmojiId { get; init; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; init; }
}
