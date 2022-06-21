using NetCord.JsonModels;

namespace NetCord.Rest;

public class StickerPack : IJsonModel<JsonStickerPack>
{
    JsonStickerPack IJsonModel<JsonStickerPack>.JsonModel => _jsonModel;
    private readonly JsonStickerPack _jsonModel;

    public StickerPack(JsonStickerPack jsonModel)
    {
        _jsonModel = jsonModel;
        Stickers = jsonModel.Stickers.Select(s => new StandardSticker(s));
    }

    public IEnumerable<Sticker> Stickers { get; }
    public string Name => _jsonModel.Name;
    public Snowflake SkuId => _jsonModel.SkuId;
    public Snowflake? CoverStickerId => _jsonModel.CoverStickerId;
    public string Description => _jsonModel.Description;
    public Snowflake? BannerAssetId => _jsonModel.BannerAssetId;
}