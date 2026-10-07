namespace NetCord.Rest;

/// <summary>
/// Represents a pack of standard stickers.
/// </summary>
public class StickerPack(JsonModels.JsonStickerPack jsonModel)
{
    /// <summary>
    /// A list of the stickers available within the pack.
    /// </summary>
    public IReadOnlyList<Sticker> Stickers { get; } = [.. jsonModel.Stickers.Select(s => new StandardSticker(s))];

    /// <summary>
    /// The sticker pack's name.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// The sticker pack's SKU ID.
    /// </summary>
    public ulong SkuId { get; } = jsonModel.SkuId;

    /// <summary>
    /// An optional ID, corresponding to the sticker displayed as the pack icon.
    /// </summary>
    public ulong? CoverStickerId { get; } = jsonModel.CoverStickerId;

    /// <summary>
    /// The sticker pack's description.
    /// </summary>
    public string Description { get; } = jsonModel.Description;

    /// <summary>
    /// The ID corresponding to the sticker pack's banner image.
    /// </summary>
    public ulong? BannerAssetId { get; } = jsonModel.BannerAssetId;
}
