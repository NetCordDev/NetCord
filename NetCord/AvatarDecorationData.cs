using NetCord.JsonModels;

namespace NetCord;

public class AvatarDecorationData(JsonAvatarDecorationData jsonModel) : IJsonModel<JsonAvatarDecorationData>
{
    JsonAvatarDecorationData IJsonModel<JsonAvatarDecorationData>.JsonModel => jsonModel;

    public string Hash => jsonModel.Hash;

    public ulong SkuId => jsonModel.SkuId;
}
