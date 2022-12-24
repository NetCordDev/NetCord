using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMessageSticker : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("format_type")]
    public StickerFormat Format { get; set; }

    [JsonSerializable(typeof(JsonMessageSticker))]
    public partial class JsonMessageStickerSerializerContext : JsonSerializerContext
    {
        public static JsonMessageStickerSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
