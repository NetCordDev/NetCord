using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal partial class UnknownTextChannel(JsonChannel jsonModel, RestClient client) : TextChannelBase(jsonModel, client), IUnknownTextChannel
{
    public ChannelType Type => _jsonModel.Type;
}
