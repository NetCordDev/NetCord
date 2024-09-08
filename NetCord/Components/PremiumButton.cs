using NetCord.JsonModels;

namespace NetCord;
public class PremiumButton(JsonComponent jsonModel) : IButton, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ulong SkuId => jsonModel.SkuId.GetValueOrDefault();
    public bool Disabled => jsonModel.Disabled.GetValueOrDefault();
}
