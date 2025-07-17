using System.Text.Json.Serialization;

namespace NetCord;

public partial class EmojiProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Unicode { get; set; }

    public EmojiProperties(ulong id)
    {
        Id = id;
    }

    public EmojiProperties(string unicode)
    {
        Unicode = unicode;
    }
}
