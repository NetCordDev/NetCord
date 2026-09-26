namespace NetCord.Rest;

/// <summary>
/// Represents a pack of standard stickers.
/// </summary>
public class StickerPack(JsonModels.JsonStickerPack jsonModel) : IJsonModel<JsonModels.JsonStickerPack>
{
    JsonModels.JsonStickerPack IJsonModel<JsonModels.JsonStickerPack>.JsonModel => jsonModel;

    /// <summary>
    /// A list of the stickers available within the pack.
    /// </summary>
    public IReadOnlyList<Sticker> Stickers { get; } = jsonModel.Stickers.Select(s => new StandardSticker(s)).ToArray();

    /// <summary>
    /// The sticker pack's name.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The sticker pack's SKU ID.
    /// </summary>
    public ulong SkuId => jsonModel.SkuId;

    /// <summary>
    /// An optional ID, corresponding to the sticker displayed as the pack icon.
    /// </summary>
    public ulong? CoverStickerId => jsonModel.CoverStickerId;

    /// <summary>
    /// The sticker pack's description.
    /// </summary>
    public string Description => jsonModel.Description;

    /// <summary>
    /// The ID corresponding to the sticker pack's banner image.
    /// </summary>
    public ulong? BannerAssetId => jsonModel.BannerAssetId;
}
