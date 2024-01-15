using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal class UnknownTextChannel : TextChannel, IUnknownTextChannel
{
    public UnknownTextChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public ChannelType Type => _jsonModel.Type;
}
