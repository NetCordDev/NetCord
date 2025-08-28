using NetCord.JsonModels;

namespace NetCord;

public class ChannelMenu(JsonComponent jsonModel, int parentId) : EntityMenu(jsonModel, parentId)
{
    public IReadOnlyList<ChannelType> ChannelTypes { get; } = jsonModel.ChannelTypes ?? [];
}
