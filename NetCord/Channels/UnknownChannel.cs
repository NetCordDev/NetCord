using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal partial class UnknownChannel : Channel, IUnknownChannel
{
    public UnknownChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public ChannelType Type => _jsonModel.Type;
}
