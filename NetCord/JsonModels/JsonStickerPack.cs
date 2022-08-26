using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonStickerPack : JsonEntity
{
    [JsonPropertyName("stickers")]
    public JsonSticker[] Stickers { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("sku_id")]
    public Snowflake SkuId { get; init; }

    [JsonPropertyName("cover_sticker_id")]
    public Snowflake? CoverStickerId { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("banner_asset_id")]
    public Snowflake? BannerAssetId { get; init; }
}
