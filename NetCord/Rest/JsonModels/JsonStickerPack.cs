using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonStickerPack : JsonEntity
{
    [JsonPropertyName("stickers")]
    public required JsonSticker[] Stickers { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("sku_id")]
    public required ulong SkuId { get; set; }

    [JsonPropertyName("cover_sticker_id")]
    public ulong? CoverStickerId { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("banner_asset_id")]
    public ulong? BannerAssetId { get; set; }
}
