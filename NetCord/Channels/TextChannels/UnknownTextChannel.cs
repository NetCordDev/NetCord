using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal partial class UnknownTextChannel : TextChannel, IUnknownTextChannel
{
    public UnknownTextChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public ChannelType Type => _jsonModel.Type;
}
