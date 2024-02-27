using NetCord.JsonModels;

namespace NetCord;

public class ChannelMenu(JsonComponent jsonModel) : EntityMenu(jsonModel)
{
    public IReadOnlyList<ulong> DefaultValues { get; } = jsonModel.DefaultValues!.Select(d => d.Id).ToArray();

    public IReadOnlyList<ChannelType> ChannelTypes => _jsonModel.ChannelTypes!;
}
