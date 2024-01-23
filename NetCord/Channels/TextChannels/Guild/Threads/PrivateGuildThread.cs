using NetCord.Rest;

namespace NetCord;

public partial class PrivateGuildThread : GuildThread
{
    public PrivateGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }
}
