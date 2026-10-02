using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonStickerPacks
{
    [JsonPropertyName("sticker_packs")]
    public required JsonStickerPack[] StickerPacks { get; set; }
}
