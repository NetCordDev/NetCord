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

    public EmojiProperties(ulong customEmojiId)
    {
        Id = customEmojiId;
    }

    public EmojiProperties(string unicode)
    {
        Unicode = unicode;
    }

    [JsonSerializable(typeof(EmojiProperties))]
    public partial class EmojiPropertiesSerializerContext : JsonSerializerContext
    {
        public static EmojiPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
