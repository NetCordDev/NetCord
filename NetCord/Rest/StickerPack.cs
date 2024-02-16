namespace NetCord.Rest;

public class StickerPack(JsonModels.JsonStickerPack jsonModel) : IJsonModel<JsonModels.JsonStickerPack>
{
    JsonModels.JsonStickerPack IJsonModel<JsonModels.JsonStickerPack>.JsonModel => jsonModel;

    public IReadOnlyList<Sticker> Stickers { get; } = jsonModel.Stickers.Select(s => new StandardSticker(s)).ToArray();
    public string Name => jsonModel.Name;
    public ulong SkuId => jsonModel.SkuId;
    public ulong? CoverStickerId => jsonModel.CoverStickerId;
    public string Description => jsonModel.Description;
    public ulong? BannerAssetId => jsonModel.BannerAssetId;
}
