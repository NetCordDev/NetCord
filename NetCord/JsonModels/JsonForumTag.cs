using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonForumTag : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("moderated")]
    public bool Moderated { get; init; }

    [JsonPropertyName("emoji_id")]
    public Snowflake? EmojiId { get; init; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; init; }
}
