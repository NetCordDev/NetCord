using NetCord.JsonModels;

namespace NetCord;

public class Nameplate(JsonNameplate jsonModel)
{
    /// <summary>
    /// The ID of the nameplate SKU.
    /// </summary>
    public ulong SkuId { get; } = jsonModel.SkuId;

    /// <summary>
    /// The path to the nameplate asset.
    /// </summary>
    public string Asset { get; } = jsonModel.Asset;

    /// <summary>
    /// The label of this nameplate.
    /// </summary>
    public string Label { get; } = jsonModel.Label;

    /// <summary>
    /// Background color of the nameplate.
    /// </summary>
    public string Palette { get; } = jsonModel.Palette;
}
