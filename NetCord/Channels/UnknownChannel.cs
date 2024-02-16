using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal partial class UnknownChannel(JsonChannel jsonModel, RestClient client) : Channel(jsonModel, client), IUnknownChannel
{
    public ChannelType Type => _jsonModel.Type;
}
