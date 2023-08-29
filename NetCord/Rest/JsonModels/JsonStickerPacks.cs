using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal partial class JsonStickerPacks
{
    [JsonPropertyName("sticker_packs")]
    public JsonStickerPack[] StickerPacks { get; set; }

    [JsonSerializable(typeof(JsonStickerPacks))]
    public partial class JsonStickerPacksSerializerContext : JsonSerializerContext
    {
        public static JsonStickerPacksSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
