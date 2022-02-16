using System.Text.Json.Serialization;

namespace NetCord;

public class ComponentEmojiProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public DiscordId? Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Unicode { get; }

    public ComponentEmojiProperties(DiscordId customEmojiId)
    {
        Id = customEmojiId;
    }

    public ComponentEmojiProperties(string unicode)
    {
        Unicode = unicode;
    }
}
