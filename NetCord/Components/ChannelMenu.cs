using NetCord.JsonModels;

namespace NetCord;

public class ChannelMenu(JsonComponent jsonModel) : EntityMenu(jsonModel)
{
    public IReadOnlyList<ChannelType> ChannelTypes => _jsonModel.ChannelTypes!;
}
