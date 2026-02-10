using NetCord.JsonModels;

namespace NetCord;

public class PremiumButton(JsonButtonComponent jsonModel) : IButton, IJsonModel<JsonButtonComponent>
{
    JsonButtonComponent IJsonModel<JsonButtonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public ulong SkuId => jsonModel.SkuId.GetValueOrDefault();
    public bool Disabled => jsonModel.Disabled.GetValueOrDefault();
}
