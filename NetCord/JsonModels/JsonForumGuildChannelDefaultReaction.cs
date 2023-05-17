using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonForumGuildChannelDefaultReaction
{
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }
}
