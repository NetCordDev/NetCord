using NetCord.Rest;

namespace NetCord;

internal partial class UnknownDMChannel : DMChannel, IUnknownDMChannel
{
    public UnknownDMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public ChannelType Type => _jsonModel.Type;
}
