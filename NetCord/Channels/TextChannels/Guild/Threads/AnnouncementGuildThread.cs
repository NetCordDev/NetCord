using NetCord.Rest;

namespace NetCord;

public class AnnouncementGuildThread : GuildThread
{
    public AnnouncementGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }
}
