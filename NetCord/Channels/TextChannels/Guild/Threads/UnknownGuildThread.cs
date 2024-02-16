using NetCord.Rest;

namespace NetCord;

public partial class UnknownGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client), IUnknownGuildThread
{
    public ChannelType Type => _jsonModel.Type;
}
