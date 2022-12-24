using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonForumTag : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("moderated")]
    public bool Moderated { get; set; }

    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }

    [JsonSerializable(typeof(JsonForumTag))]
    public partial class JsonForumTagSerializerContext : JsonSerializerContext
    {
        public static JsonForumTagSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
