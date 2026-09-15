using NetCord.Rest;

namespace NetCord;

internal partial class UnknownDMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : DMChannel(jsonModel, client), IUnknownDMChannel
{
    public ChannelType Type => jsonModel.Type;
}
