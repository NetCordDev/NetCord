using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonStickerPack : JsonEntity
{
    [JsonPropertyName("stickers")]
    public JsonSticker[] Stickers { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }

    [JsonPropertyName("cover_sticker_id")]
    public ulong? CoverStickerId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("banner_asset_id")]
    public ulong? BannerAssetId { get; set; }
}
