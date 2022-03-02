using System.Text.Json.Serialization;

namespace NetCord;

public class EmojiProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public DiscordId? Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Unicode { get; }

    public EmojiProperties(DiscordId customEmojiId)
    {
        Id = customEmojiId;
    }

    public EmojiProperties(string unicode)
    {
        Unicode = unicode;
    }
}
