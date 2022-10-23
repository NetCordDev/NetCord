using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonSticker : JsonEntity
{
    [JsonPropertyName("pack_id")]
    public ulong? PackId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("tags")]
    public string Tags { get; set; }

    [JsonPropertyName("type")]
    public JsonStickerType Type { get; set; }

    [JsonPropertyName("format_type")]
    public StickerFormat Format { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? Creator { get; set; }

    [JsonPropertyName("sort_value")]
    public int SortValue { get; set; }

    [JsonSerializable(typeof(JsonSticker))]
    public partial class JsonStickerSerializerContext : JsonSerializerContext
    {
        public static JsonStickerSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonSticker[]))]
    public partial class JsonStickerArraySerializerContext : JsonSerializerContext
    {
        public static JsonStickerArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

public enum JsonStickerType
{
    Standard = 1,
    Guild = 2,
}
