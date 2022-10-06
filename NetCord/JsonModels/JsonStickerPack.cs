using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonStickerPack : JsonEntity
{
    [JsonPropertyName("stickers")]
    public JsonSticker[] Stickers { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("sku_id")]
    public Snowflake SkuId { get; set; }

    [JsonPropertyName("cover_sticker_id")]
    public Snowflake? CoverStickerId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("banner_asset_id")]
    public Snowflake? BannerAssetId { get; set; }

    [JsonSerializable(typeof(JsonStickerPack))]
    public partial class JsonStickerPackSerializerContext : JsonSerializerContext
    {
        public static JsonStickerPackSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
