using NetCord.Rest;

namespace NetCord;

public class NewsGuildThread : GuildThread
{
    public NewsGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }
}
