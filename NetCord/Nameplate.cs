using NetCord.JsonModels;

namespace NetCord;

public class Nameplate(JsonNameplate jsonModel) : IJsonModel<JsonNameplate>
{
    JsonNameplate IJsonModel<JsonNameplate>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the nameplate SKU.
    /// </summary>
    public ulong SkuId => jsonModel.SkuId;

    /// <summary>
    /// The path to the nameplate asset.
    /// </summary>
    public string Asset => jsonModel.Asset;

    /// <summary>
    /// The label of this nameplate.
    /// </summary>
    public string Label => jsonModel.Label;

    /// <summary>
    /// Background color of the nameplate.
    /// </summary>
    public string Palette => jsonModel.Palette;
}
