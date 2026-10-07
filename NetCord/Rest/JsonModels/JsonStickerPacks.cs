using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonStickerPacks
{
    [JsonPropertyName("sticker_packs")]
    public JsonStickerPack[] StickerPacks { get; set; }
}
