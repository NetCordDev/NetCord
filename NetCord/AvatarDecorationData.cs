using NetCord.JsonModels;

namespace NetCord;

public class AvatarDecorationData(JsonAvatarDecorationData jsonModel)
{
    public string Hash { get; } = jsonModel.Hash;

    public ulong SkuId { get; } = jsonModel.SkuId;
}
