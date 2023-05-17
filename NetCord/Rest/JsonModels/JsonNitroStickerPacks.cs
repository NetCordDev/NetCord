using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal partial class JsonNitroStickerPacks
{
    [JsonPropertyName("sticker_packs")]
    public JsonStickerPack[] StickerPacks { get; set; }

    [JsonSerializable(typeof(JsonNitroStickerPacks))]
    public partial class JsonNitroStickerPacksSerializerContext : JsonSerializerContext
    {
        public static JsonNitroStickerPacksSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
