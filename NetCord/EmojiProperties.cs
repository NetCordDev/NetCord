using System.Text.Json.Serialization;

namespace NetCord;

public partial class EmojiProperties
{
    public EmojiProperties(ulong customEmojiId)
    {
        Id = customEmojiId;
    }

    public EmojiProperties(string unicode)
    {
        Unicode = unicode;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Unicode { get; set; }
}
